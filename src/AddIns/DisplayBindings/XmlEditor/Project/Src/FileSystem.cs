// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.XmlEditor
{
	public class FileSystem : IFileSystem
	{
		public FileSystem()
		{
		}
		
		public bool FileExists(string fileName)
		{
			return File.Exists(fileName);
		}
		
		public void DeleteFile(string fileName)
		{
			File.Delete(fileName);
		}
		
		public void CopyFile(string source, string destination)
		{
			File.Copy(source, destination);
		}
		
		public void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}
		
		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}
		
		public string[] GetFilesInDirectory(string path, string searchPattern)
		{
			return Directory.GetFiles(path, searchPattern);
		}
	}
}
