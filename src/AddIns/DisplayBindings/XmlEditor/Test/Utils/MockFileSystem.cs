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
using System.Collections.Specialized;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	public class MockFileSystem : IFileSystem
	{
		List<string> searchedFolders = new List<string>();
		List<string> foldersCheckedThatTheyExist = new List<string>();
		List<string> foldersThatExist = new List<string>();
		List<string> searchedForFileExtensions = new List<string>();
		List<string> createdFolders = new List<string>();
		List<string> deletedFiles = new List<string>();
		List<string> filesCheckedThatTheyExist = new List<string>();
		Dictionary<FileName, bool> existingFiles = new Dictionary<FileName, bool>();
		MultiDictionary<DirectoryName, FileName> directoryFiles = new MultiDictionary<DirectoryName, FileName>();
		NameValueCollection copiedFileLocations = new NameValueCollection();
		Exception exceptionToThrowWhenCopyFileCalled;

		public MockFileSystem()
		{
		}
		
		public void AddDirectoryFiles(string folder, string[] files)
		{
			foreach (var file in files)
				directoryFiles.Add(DirectoryName.Create(folder), FileName.Create(file));
		}
		
		public IEnumerable<FileName> GetFiles(DirectoryName folder, string extension, DirectorySearchOptions searchOptions = DirectorySearchOptions.None)
		{
			searchedFolders.Add(folder);
			searchedForFileExtensions.Add(extension);
			
			return directoryFiles[folder];
		}
		
		public string[] SearchedFolders {
			get { return searchedFolders.ToArray(); }
		}	
		
		public string[] SearchedForFileExtensions {
			get { return searchedForFileExtensions.ToArray(); }
		}		
		
		public void AddExistingFolders(IEnumerable<string> folders) 
		{
			foreach (string folder in folders) {
				AddExistingFolder(folder);
			}
		}
		
		public void AddExistingFolder(string folder)
		{
			foldersThatExist.Add(folder);
		}
		
		public bool DirectoryExists(DirectoryName folder)
		{
			if (folder == null)
				return false;
			foldersCheckedThatTheyExist.Add(folder);
			return foldersThatExist.Contains(folder);
		}
		
		public string[] FoldersCheckedThatTheyExist {
			get { return foldersCheckedThatTheyExist.ToArray(); }
		}
				
		public NameValueCollection CopiedFileLocations {
			get { return copiedFileLocations; }
		}
		
		public void CopyFile(FileName source, FileName destination, bool overwrite = false)
		{
			copiedFileLocations.Add(source, destination);
			
			if (exceptionToThrowWhenCopyFileCalled != null) {
				throw exceptionToThrowWhenCopyFileCalled;
			}
		}
		
		public Exception ExceptionToThrowWhenCopyFileCalled {
			get { return exceptionToThrowWhenCopyFileCalled; }
			set { exceptionToThrowWhenCopyFileCalled = value; }
		}
		
		public List<string> CreatedFolders {
			get { return createdFolders; }
		}
		
		public void CreateDirectory(DirectoryName folder)
		{
			createdFolders.Add(folder);
		}
		
		public string[] DeletedFiles {
			get { return deletedFiles.ToArray(); }
		}
		
		public void Delete(FileName fileName)
		{
			deletedFiles.Add(fileName);
		}
		
		public string[] FilesCheckedThatTheyExist {
			get { return filesCheckedThatTheyExist.ToArray(); }
		}
		
		public bool FileExists(FileName fileName)
		{
			if (fileName == null)
				return false;
			filesCheckedThatTheyExist.Add(fileName);
			bool fileExists;
			if (existingFiles.TryGetValue(fileName, out fileExists)) {
				return fileExists;
			}
			return false;
		}
		
		public void AddExistingFile(string fileName, bool exists)
		{
			existingFiles.Add(FileName.Create(fileName), exists);
		}		
		
		Stream IFileSystem.OpenWrite(FileName fileName)
		{
			throw new NotImplementedException();
		}
		
		Stream IReadOnlyFileSystem.OpenRead(FileName fileName)
		{
			throw new NotImplementedException();
		}
		
		TextReader IReadOnlyFileSystem.OpenText(FileName fileName)
		{
			throw new NotImplementedException();
		}
	}
}
