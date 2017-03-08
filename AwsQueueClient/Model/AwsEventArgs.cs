using System;
using System.IO;
using AwsQueue.Interface;

namespace AwsQueue.Model
{
    public class AwsEventArgs : IAwsArgs
    {
        public EventArgs Args { get; set; }
        public WatcherChangeTypes ChangeType { get; set; }
    }
}
