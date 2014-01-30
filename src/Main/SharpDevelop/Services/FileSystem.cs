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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	sealed class FileSystem : IFileSystem
	{
		// We wrap UnauthorizedAccessException in IOException
		// so that the consuming code can handle both at once.
		
		public void Delete(FileName path)
		{
			try {
				File.Delete(path);
			} catch (UnauthorizedAccessException ex) {
				throw new IOException(ex.Message, ex);
			}
		}
		
		public void CreateDirectory(DirectoryName path)
		{
			try {
				Directory.CreateDirectory(path);
			} catch (UnauthorizedAccessException ex) {
				throw new IOException(ex.Message, ex);
			}
		}
		
		public bool FileExists(FileName path)
		{
			return File.Exists(path);
		}
		
		public bool DirectoryExists(DirectoryName path)
		{
			return Directory.Exists(path);
		}
		
		public Stream OpenRead(FileName fileName)
		{
			try {
				return File.OpenRead(fileName);
			} catch (UnauthorizedAccessException ex) {
				throw new IOException(ex.Message, ex);
			}
		}
		
		public Stream OpenWrite(FileName fileName)
		{
			try {
				return File.OpenWrite(fileName);
			} catch (UnauthorizedAccessException ex) {
				throw new IOException(ex.Message, ex);
			}
		}
		
		public TextReader OpenText(FileName fileName)
		{
			try {
				return File.OpenText(fileName);
			} catch (UnauthorizedAccessException ex) {
				throw new IOException(ex.Message, ex);
			}
		}
		
		public IEnumerable<FileName> GetFiles(DirectoryName directory, string searchPattern, DirectorySearchOptions searchOptions)
		{
			try {
				bool searchSubdirectories = (searchOptions & DirectorySearchOptions.IncludeSubdirectories) != 0;
				bool ignoreHidden = (searchOptions & DirectorySearchOptions.IncludeHidden) == 0;
				return FileUtility.LazySearchDirectory(directory, searchPattern, searchSubdirectories, ignoreHidden);
			} catch (UnauthorizedAccessException ex) {
				throw new IOException(ex.Message, ex);
			}
		}
		
		public void CopyFile(FileName source, FileName target, bool overwrite)
		{
			try {
				File.Copy(source, target, overwrite);
			} catch (UnauthorizedAccessException ex) {
				throw new IOException(ex.Message, ex);
			}
		}
	}
}
