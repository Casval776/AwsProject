using AwsQueue.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using AwsQueue;

namespace AwsPoll.Controller
{
    public class AwsPollManager
    {
        #region Private Members
        private static readonly Lazy<AwsPollManager> PollManager = new Lazy<AwsPollManager>(() => new AwsPollManager());
        private static AwsQueueClient _queueClient;
        private static bool _isRunning;
        private static readonly int Timeout = (int) new TimeSpan(0, 0, 5, 0).TotalMilliseconds; //5 minutes
        private static BackgroundWorker _worker;
        #endregion

        #region Public Members
        public static event EventHandler<IEnumerable<IAwsArgs>> DequeueEvent;
        #endregion

        #region Constructors
        private AwsPollManager()
        {
            //Initialize
            Startup();
        }

        static AwsPollManager() { }
        #endregion

        #region Private Functions
        private static void Startup()
        {
            //Initialize AwsQueueClient
            if (_queueClient == null) _queueClient = AwsQueueClient.GetInstance;

            //Initialize default run status
            _isRunning = false;

            //Start polling
            if (_worker == null) _worker = new BackgroundWorker();
            _worker.DoWork += Poll;
        }

        private static void Poll(object obj, DoWorkEventArgs dwea)
        {
            while (_isRunning)
            {
                //Get messages from the queue
                var result = _queueClient.Dequeue();

                //If count > 0
                if (result.Result.Any())
                {
                    //Fire event to subscribers
                    OnDequeue(result.Result);
                }

                //Sleep
                Thread.Sleep(Timeout);
            }
        }
        #endregion

        #region Public Functions
        public static AwsPollManager GetInstance => PollManager.Value;

        public void StartWork()
        {
            _isRunning = true;
            _worker.RunWorkerAsync();
        }

        public void StopWork()
        {
            _isRunning = false;
            _worker.CancelAsync();
        }

        public static void OnDequeue(IEnumerable<IAwsArgs> args)
        {
            DequeueEvent?.Invoke(GetInstance, args);
        }
        #endregion
    }
}
