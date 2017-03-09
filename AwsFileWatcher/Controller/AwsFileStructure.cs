using System.IO;
using System.Linq;
using AwsFileWatcher.Global;
using AwsFileWatcher.Models;

namespace AwsFileWatcher.Controller
{
    /// <summary>
    /// Controller class to handle functionality of file-system model
    /// </summary>
    public class AwsFileStructure
    {
        #region Private Members
        public AwsFolder RootFolder { get; set; }
        #endregion

        #region Constructors
        public AwsFileStructure()
        {
            //Initialize collection
            if (RootFolder == null) RootFolder = FillFileStructure(Data.FileSystem.WatchPath, true);
        }
        #endregion

        #region Private Functions
        private static AwsFolder FillFileStructure(string path, bool root)
        {
            //Initialize object for current folder
            var curFolder = new AwsFolder
            {
                Info = new DirectoryInfo(path),
                Root = root
            };

            //Generate file collection
            curFolder.Files = curFolder.Info.GetFiles().Select(file => new AwsFile
            {
                Info = file
            }).ToList();

            //Iterate to next directory in curFolder and recursively trace through it
            var folderList = curFolder.Info
                .GetDirectories()
                .Select(dir => FillFileStructure(dir.FullName, false))
                .ToList();

            //Assign folder collection
            curFolder.Folders = folderList;

            //Return object
            return curFolder;
        }
        #endregion
    }
}
