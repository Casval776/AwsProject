using System;
using AwsFileWatcher.Controller;
using System.ServiceProcess;
using System.Threading;

namespace AwsProject
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            //Testing hash code
            var watcher = AwsFileWatcher.Controller.AwsFileWatcher.GetInstance;
            Console.WriteLine($"First: {watcher.GetHashCode()}");
            Thread.Sleep((int)(new TimeSpan(0, 0, 0, 30).TotalMilliseconds));
            Console.WriteLine($"Second: {watcher.GetHashCode()}");
            Console.ReadKey();
            var servicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
