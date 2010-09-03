// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
		Dictionary<string, bool> existingFiles = new Dictionary<string, bool>();
		Dictionary<string, string[]> directoryFiles = new Dictionary<string, string[]>();
		NameValueCollection copiedFileLocations = new NameValueCollection();
		Exception exceptionToThrowWhenCopyFileCalled;

		public MockFileSystem()
		{
		}
		
		public void AddDirectoryFiles(string folder, string[] files)
		{
			directoryFiles.Add(folder, files);
		}
		
		public string[] GetFilesInDirectory(string folder, string extension)
		{
			searchedFolders.Add(folder);
			searchedForFileExtensions.Add(extension);
			
			string[] files;
			if (directoryFiles.TryGetValue(folder, out files)) {
				return files;
			}
			return new string[0];
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
		
		public bool DirectoryExists(string folder)
		{
			foldersCheckedThatTheyExist.Add(folder);
			return foldersThatExist.Contains(folder);
		}
		
		public string[] FoldersCheckedThatTheyExist {
			get { return foldersCheckedThatTheyExist.ToArray(); }
		}
				
		public NameValueCollection CopiedFileLocations {
			get { return copiedFileLocations; }
		}
		
		public void CopyFile(string source, string destination)
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
		
		public void CreateDirectory(string folder)
		{
			createdFolders.Add(folder);
		}
		
		public string[] DeletedFiles {
			get { return deletedFiles.ToArray(); }
		}
		
		public void DeleteFile(string fileName)
		{
			deletedFiles.Add(fileName);
		}
		
		public string[] FilesCheckedThatTheyExist {
			get { return filesCheckedThatTheyExist.ToArray(); }
		}
		
		public bool FileExists(string fileName)
		{
			filesCheckedThatTheyExist.Add(fileName);
			bool fileExists;
			if (existingFiles.TryGetValue(fileName, out fileExists)) {
				return fileExists;
			}
			return false;
		}
		
		public void AddExistingFile(string fileName, bool exists)
		{
			existingFiles.Add(fileName, exists);
		}		
	}
}
