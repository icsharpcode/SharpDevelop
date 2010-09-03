// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.XmlEditor
{
	public interface IFileSystem
	{
		bool FileExists(string fileName);
		void DeleteFile(string fileName);
		void CopyFile(string source, string destination);
		void CreateDirectory(string path);
		bool DirectoryExists(string path);
		string[] GetFilesInDirectory(string path, string searchPattern);
	}
}
