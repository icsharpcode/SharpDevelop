// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.CodeCoverage;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.CodeCoverage.Tests.Utils
{
	public class MockFileSystem : IFileSystem
	{
		public bool FileExistsReturnValue;
		public string FileExistsPathParameter;
		public string DeleteFilePathParameter;
		public bool DirectoryExistsReturnValue;
		public string DirectoryExistsPathParameter;
		public string CreateDirectoryPathParameter;		
		public TextReader CreateTextReaderReturnValue;
		public string CreateTextReaderPathParameter;
		
		public bool FileExists(FileName path)
		{
			FileExistsPathParameter = path;
			return FileExistsReturnValue;
		}
		
		public void Delete(FileName path)
		{
			DeleteFilePathParameter = path;
		}
		
		public bool DirectoryExists(DirectoryName path)
		{
			DirectoryExistsPathParameter = path;
			return DirectoryExistsReturnValue;
		}
		
		public void CreateDirectory(DirectoryName path)
		{
			CreateDirectoryPathParameter = path;
		}
		
		public TextReader OpenText(FileName path)
		{
			CreateTextReaderPathParameter = path;
			return CreateTextReaderReturnValue;
		}
		
		void IFileSystem.CopyFile(FileName source, FileName destination, bool overwrite)
		{
			throw new NotImplementedException();
		}
		
		Stream IFileSystem.OpenWrite(FileName fileName)
		{
			throw new NotImplementedException();
		}
		
		Stream IReadOnlyFileSystem.OpenRead(FileName fileName)
		{
			throw new NotImplementedException();
		}
		
		System.Collections.Generic.IEnumerable<FileName> IReadOnlyFileSystem.GetFiles(DirectoryName directory, string searchPattern, DirectorySearchOptions searchOptions)
		{
			throw new NotImplementedException();
		}
	}
}
