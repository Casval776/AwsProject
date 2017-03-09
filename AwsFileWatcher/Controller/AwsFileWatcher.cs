using System;
using System.IO;
using AwsFileWatcher.Global;
using AwsQueue;
using AwsQueue.Model;

namespace AwsFileWatcher.Controller
{
    /// <summary>
    /// Custom wrapper to hold functionality of FileSystemWatcher.
    /// Handles fired events and redirects accordingly.
    /// </summary>
    public class AwsFileWatcher
    {
        #region Private Members
        private static readonly Lazy<AwsFileWatcher> Watcher = new Lazy<AwsFileWatcher>(() => new AwsFileWatcher());
        private static FileSystemWatcher _watcher;
        private static AwsQueueClient _queueClient;
        private static AwsFileStructure _fileStructure;
        #endregion

        #region Constructors
        private AwsFileWatcher()
        {
            //Begin initialization
            Startup();
        }

        static AwsFileWatcher() { }
        #endregion

        #region Private Functions
        private static void Startup()
        {
            //If directory doesn't exist, throw an error
            //@TODO: Add Logging
            if (!Directory.Exists(Data.FileSystem.WatchPath)) throw new IOException();

            //If directory does exist, initialize watcher
            _watcher = new FileSystemWatcher
            {
                Path = Data.FileSystem.WatchPath,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.Attributes |
                               NotifyFilters.CreationTime |
                               NotifyFilters.DirectoryName |
                               NotifyFilters.FileName |
                               NotifyFilters.LastWrite |
                               NotifyFilters.Security |
                               NotifyFilters.Size,
                Filter = "*.*"
            };

            //Set event delegates
            _watcher.Changed += OnChanged;
            _watcher.Renamed += OnRenamed;

            //Start watching
            _watcher.EnableRaisingEvents = true;

            //Initialize queue client
            _queueClient = AwsQueueClient.GetInstance();

            //Initialize File Structure
            _fileStructure = new AwsFileStructure();
        }
        #endregion

        #region Public Functions
        public static AwsFileWatcher GetInstance => Watcher.Value;

        public void StartWatching() => _watcher.EnableRaisingEvents = true;

        public void StopWatching() => _watcher.EnableRaisingEvents = false;

        public AwsFileStructure GetFileStructure() => _fileStructure;
        #endregion

        #region Private Events
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            //Send message to queue
            _queueClient.Enqueue(new AwsEventArgs
            {
                Args = e,
                ChangeType = e.ChangeType
            });
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            //Send message to queue
            _queueClient.Enqueue(new AwsEventArgs
            {
                Args = e,
                ChangeType = e.ChangeType
            });
        }
        #endregion
    }
}
