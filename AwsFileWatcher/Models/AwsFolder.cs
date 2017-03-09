using System.Collections.Generic;
using System.IO;

namespace AwsFileWatcher.Models
{
    public class AwsFolder
    {
        public IEnumerable<AwsFolder> Folders { get; set; }
        public IEnumerable<AwsFile> Files { get; set; }
        public DirectoryInfo Info { get; set; }
        public bool Root { get; set; }
    }
}
