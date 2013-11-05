// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
