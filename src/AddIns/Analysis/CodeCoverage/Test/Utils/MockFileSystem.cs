// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
