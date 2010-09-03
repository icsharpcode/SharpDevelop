// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.CodeCoverage
{
	public class FileSystem : IFileSystem
	{
		public bool FileExists(string path)
		{
			return File.Exists(path);
		}
		
		public void DeleteFile(string path)
		{
			File.Delete(path);
		}
		
		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}
		
		public void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}
		
		public TextReader CreateTextReader(string path)
		{
			return new StreamReader(path);
		}
	}
}
