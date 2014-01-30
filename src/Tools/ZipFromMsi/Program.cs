using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ZipFromMsi
{
    class Program
    {
        // Xml Constants
        private const string ElementNameFragment = "Fragment";
        private const string ElementNameDirectoryRef = "DirectoryRef";
        private const string ElementNameDirectory = "Directory";
        private const string ElementNameComponent = "Component";
        private const string ElementNameFile = "File";

        // NOTE: App has to be started in bin\Debug working directory, otherwise those paths don't work
        private const string WxsFilename = "../../../../Setup/Files.wxs";
        public static string RelativePathToolToSetupFolder = "..\\..\\..\\";

        // App Constants
        private static readonly XNamespace Namespace = "http://schemas.microsoft.com/wix/2006/wi";
        private const string ZipRootDirectory = "SharpDevelop";

        static void Main(string[] args)
        {
            CreateZip("SharpDevelopStandalone.zip", LoadWxsFile(WxsFilename));
        }

        static void CreateZip(string zipFileName, XDocument doc)
        {
            XElement root = doc.Root;

            var fragment = root.Element(Namespace + ElementNameFragment);
            var directoryRef = fragment.Element(Namespace + ElementNameDirectoryRef);

            var programDirectory = directoryRef.Elements().First();

            if ("ProgramFilesFolder" == (string)programDirectory.Attribute("Id"))
            {
                var sdfolder = programDirectory.Elements().First();
                var installdir = sdfolder.Elements().First();

                using (ZipArchive theZip = ZipFile.Open(zipFileName, ZipArchiveMode.Create))
                {
                    foreach (var folder in installdir.Elements(Namespace + ElementNameDirectory))
                    {
                        ProcessFolder(folder, theZip, ZipRootDirectory);
                    }
                }
            }
        }

        static void ProcessFolder(XElement folder, ZipArchive theZip, string folderRelativePath)
        {
            string targetDirectory = (string)folder.Attribute("Name");
            string currentRelativePath = AppendRelativePath(folderRelativePath, targetDirectory);

            Console.WriteLine("Processing folder " + currentRelativePath);

            foreach (var component in folder.Elements(Namespace + ElementNameComponent))
            {
                foreach (var file in component.Elements(Namespace + ElementNameFile))
                {
                    string source = (string)file.Attribute("Source");
                    string name = (string)file.Attribute("Name");

                    theZip.CreateEntryFromFile(RelativePathToolToSetupFolder + source, 
                        AppendRelativePath(currentRelativePath, name),
                        CompressionLevel.Optimal);
                }
            }

            foreach (var secondaryFolder in folder.Elements(Namespace + ElementNameDirectory))
            {
                ProcessFolder(secondaryFolder, theZip, currentRelativePath);
            }
        }

        static string AppendRelativePath(string firstPart, string newPart)
        {
            return firstPart + "/" + newPart;
        }

        static XDocument LoadWxsFile(string filename)
        {
            return XDocument.Load(filename);
        }
    }
}
