// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakeFileSystem : IFileSystem
	{
		public bool FileExistsReturnValue;
		public string PathPassedToFileExists;
		public string FileToReturnFromOpenFile;
		public string PathToReturnFromGetFullPath;
		public bool DirectoryExistsReturnValue;
		public string PathPassedToDirectoryExists;
		
		public ILogger Logger { get; set; }
		
		public string Root { get; set; }
		
		public void DeleteDirectory(string path, bool recursive)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<string> GetFiles(string path)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<string> GetFiles(string path, string filter)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<string> GetDirectories(string path)
		{
			throw new NotImplementedException();
		}
		
		public string GetFullPath(string path)
		{
			return PathToReturnFromGetFullPath;
		}
		
		public void DeleteFile(string path)
		{
			throw new NotImplementedException();
		}
		
		public bool FileExists(string path)
		{
			PathPassedToFileExists = path;
			return FileExistsReturnValue;
		}
		
		public bool DirectoryExists(string path)
		{
			PathPassedToDirectoryExists = path;
			return DirectoryExistsReturnValue;
		}
		
		public void AddFile(string path, Stream stream)
		{
			throw new NotImplementedException();
		}
		
		public Stream OpenFile(string path)
		{
			byte[] bytes = UTF8Encoding.UTF8.GetBytes(FileToReturnFromOpenFile);
			return new MemoryStream(bytes);
		}
		
		public DateTimeOffset GetLastModified(string path)
		{
			throw new NotImplementedException();
		}
		
		public DateTimeOffset GetCreated(string path)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<string> GetFiles(string path, string filter, bool recursive)
		{
			throw new NotImplementedException();
		}
		
		public DateTimeOffset GetLastAccessed(string path)
		{
			throw new NotImplementedException();
		}
	}
}
