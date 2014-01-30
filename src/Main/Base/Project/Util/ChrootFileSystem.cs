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
	/// <summary>
	/// Wraps an <see cref="IReadOnlyFileSystem"/> to allow the use of relative paths.
	/// </summary>
	public class ReadOnlyChrootFileSystem : IReadOnlyFileSystem
	{
		readonly IReadOnlyFileSystem fileSystem;
		readonly DirectoryName basePath;
		
		public ReadOnlyChrootFileSystem(IReadOnlyFileSystem fileSystem, DirectoryName basePath)
		{
			if (fileSystem == null)
				throw new ArgumentNullException("fileSystem");
			if (basePath == null)
				throw new ArgumentNullException("basePath");
			this.fileSystem = fileSystem;
			this.basePath = basePath;
		}
		
		public bool FileExists(FileName path)
		{
			return fileSystem.FileExists(basePath.Combine(path));
		}
		
		public bool DirectoryExists(DirectoryName path)
		{
			return fileSystem.DirectoryExists(basePath.Combine(path));
		}
		
		public Stream OpenRead(FileName fileName)
		{
			return fileSystem.OpenRead(basePath.Combine(fileName));
		}
		
		public TextReader OpenText(FileName fileName)
		{
			return fileSystem.OpenText(basePath.Combine(fileName));
		}
		
		public IEnumerable<FileName> GetFiles(DirectoryName directory, string searchPattern, DirectorySearchOptions searchOptions)
		{
			return fileSystem.GetFiles(basePath.Combine(directory), searchPattern, searchOptions)
				.Select(file => basePath.GetRelativePath(file));
		}
	}
}
