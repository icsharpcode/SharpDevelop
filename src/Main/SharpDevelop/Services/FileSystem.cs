// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
