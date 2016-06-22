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
	/// A wrapper around commonly used System.IO methods
	/// for accessing the file system. Allows mocking file system access in unit tests.
	/// </summary>
	[SDService("SD.FileSystem", FallbackImplementation = typeof(EmptyFileSystem))]
	public interface IFileSystem : IReadOnlyFileSystem
	{
		/// <inheritdoc cref="System.IO.File.Delete"/>
		void Delete(FileName path);
		
		/// <inheritdoc cref="System.IO.File.CopyFile"/>
		void CopyFile(FileName source, FileName destination, bool overwrite = false);
		
		/// <inheritdoc cref="System.IO.Directory.Delete"/>
		void CreateDirectory(DirectoryName path);
		
		/// <inheritdoc cref="System.IO.File.OpenWrite"/>
		Stream OpenWrite(FileName fileName);
	}
	
	public interface IReadOnlyFileSystem
	{
		/// <inheritdoc cref="System.IO.File.Exists"/>
		bool FileExists(FileName path);
		
		/// <inheritdoc cref="System.IO.Directory.Exists"/>
		bool DirectoryExists(DirectoryName path);
		
		/// <inheritdoc cref="System.IO.File.OpenRead"/>
		Stream OpenRead(FileName fileName);
		
		/// <inheritdoc cref="System.IO.File.OpenText"/>
		TextReader OpenText(FileName fileName);
		
		/// <summary>
		/// Retrieves the list of files in the specified directory.
		/// </summary>
		/// <param name="directory">The directory to search in.</param>
		/// <param name="searchPattern">The search pattern used to filter the result list.
		/// For example: "*.exe".
		/// This method does not use 8.3 patterns; so "*.htm" will not match ".html".</param>
		/// <param name="searchOptions">
		/// Options that influence the search.
		/// </param>
		/// <returns>An enumerable that iterates through the directory contents</returns>
		/// <exception cref="IOExcption">The directory does not exist / access is denied.</exception>
		IEnumerable<FileName> GetFiles(DirectoryName directory, string searchPattern = "*", DirectorySearchOptions searchOptions = DirectorySearchOptions.None);
	}
	
	[Flags]
	public enum DirectorySearchOptions
	{
		/// <summary>
		/// Search top directory only; skip hidden files.
		/// </summary>
		None = 0,
		/// <summary>
		/// Include hidden files/subdirectories.
		/// </summary>
		IncludeHidden = 1,
		/// <summary>
		/// Perform a recursive search into subdirectories.
		/// </summary>
		IncludeSubdirectories = 2
	}
	
	public static class FileSystemExtensions
	{
		public static string ReadAllText(this IReadOnlyFileSystem fileSystem, FileName fileName)
		{
			using (var reader = fileSystem.OpenText(fileName)) {
				return reader.ReadToEnd();
			}
		}
	}
	
	sealed class EmptyFileSystem : IFileSystem
	{
		void IFileSystem.Delete(FileName path)
		{
			throw new FileNotFoundException();
		}
		
		void IFileSystem.CopyFile(FileName source, FileName destination, bool overwrite)
		{
			throw new FileNotFoundException();
		}
		
		void IFileSystem.CreateDirectory(DirectoryName path)
		{
			throw new NotSupportedException();
		}
		
		Stream IFileSystem.OpenWrite(FileName fileName)
		{
			throw new NotSupportedException();
		}
		
		bool IReadOnlyFileSystem.FileExists(FileName path)
		{
			return false;
		}
		
		bool IReadOnlyFileSystem.DirectoryExists(DirectoryName path)
		{
			return false;
		}
		
		Stream IReadOnlyFileSystem.OpenRead(FileName fileName)
		{
			throw new FileNotFoundException();
		}
		
		TextReader IReadOnlyFileSystem.OpenText(FileName fileName)
		{
			throw new FileNotFoundException();
		}
		
		IEnumerable<FileName> IReadOnlyFileSystem.GetFiles(DirectoryName directory, string searchPattern, DirectorySearchOptions searchOptions)
		{
			return Enumerable.Empty<FileName>();
		}
	}
}
