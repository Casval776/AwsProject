using System.IO;
using System.Linq;
using AwsFileWatcher.Global;
using AwsFileWatcher.Models;

namespace AwsFileWatcher.Controller
{
    public class AwsFileStructure
    {
        private static AwsFolder _rootFolder;

        public AwsFileStructure()
        {
            //Initialize collection
            if (_rootFolder == null) _rootFolder = FillFileStructure(Data.FileSystem.WatchPath);
        }

        private static AwsFolder FillFileStructure(string path)
        {
            //Initialize object for current folder
            var curFolder = new AwsFolder
            {
                Info = new DirectoryInfo(path)
            };

            //Generate file collection
            curFolder.Files = curFolder.Info.GetFiles().Select(file => new AwsFile
            {
                Info = file
            }).ToList();

            //Iterate to next directory in curFolder and recursively trace through it
            var folderList = curFolder.Info
                .GetDirectories()
                .Select(dir => FillFileStructure(dir.FullName))
                .ToList();

            //Assign folder collection
            curFolder.Folders = folderList;

            //Return object
            return curFolder;
        }
    }
}
