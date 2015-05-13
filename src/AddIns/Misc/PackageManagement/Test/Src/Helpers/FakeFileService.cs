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

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

namespace PackageManagement.Tests.Helpers
{
	public class FakeFileService : IPackageManagementFileService
	{
		public string PathPassedToRemoveFile;
		public string PathPassedToRemoveDirectory;
		
		MSBuildBasedProject project;
		
		public FakeFileService(MSBuildBasedProject project)
		{
			this.project = project;
		}
		
		public void RemoveFile(string path)
		{
			PathPassedToRemoveFile = path;
			
			RemoveFirstProjectItem();
		}
		
		public void RemoveDirectory(string path)
		{
			PathPassedToRemoveDirectory = path;
			
			RemoveFirstProjectItem();
		}
		
		void RemoveFirstProjectItem()
		{
			ProjectItem item = project.Items.First();
			ProjectService.RemoveProjectItem(project, item);
		}
		
		public string FileNamePassedToOpenFile;
		public bool IsOpenFileCalled;
		
		public void OpenFile(string fileName)
		{
			IsOpenFileCalled = true;
			FileNamePassedToOpenFile = fileName;
		}
		
		public string OldFileNamePassedToCopyFile;
		public string NewFileNamePassedToCopyFile;
		
		public void CopyFile(string oldFileName, string newFileName)
		{
			OldFileNamePassedToCopyFile = oldFileName;
			NewFileNamePassedToCopyFile = newFileName;
		}
		
		public List<string> ExistingFileNames = new List<string>();
		
		public bool FileExists(string fileName)
		{
			return ExistingFileNames.Contains(fileName);
		}
		
		Dictionary<string, string[]> directoryFiles = new Dictionary<string, string[]>();
		
		public void AddFilesToFakeFileSystem(string directory, params string[] filePathsRelativeToDirectory)
		{
			string[] fullPathFiles = ConvertToFullPaths(directory, filePathsRelativeToDirectory);
			directoryFiles.Add(directory, fullPathFiles);
		}
		
		string[] ConvertToFullPaths(string directory, string[] pathsRelativeToDirectory)
		{
			return pathsRelativeToDirectory
				.Select(relativePath => Path.Combine(directory, relativePath))
				.ToArray();
		}
		
		public string[] GetFiles(string path)
		{
			string[] files;
			if (directoryFiles.TryGetValue(path, out files)) {
				return files;
			}
			return new string[0];
		}
		
		Dictionary<string, string[]> directories = new Dictionary<string, string[]>();
		
		public void AddDirectoryToFakeFileSystem(string parentDirectory, params string[] childDirectoryPathsRelativeToParent)
		{
			string[] fullPathChildDirectories = ConvertToFullPaths(parentDirectory, childDirectoryPathsRelativeToParent);
			directories.Add(parentDirectory, fullPathChildDirectories);
		}
		
		public string[] GetDirectories(string path)
		{
			string[] childDirectories;
			if (directories.TryGetValue(path, out childDirectories)) {
				return childDirectories;
			}
			return new string[0];
		}
		
		public string FileNamePassedToParseFile;
		
		public void ParseFile(string fileName)
		{
			FileNamePassedToParseFile = fileName;
		}
		
		public string FileNamePassedToGetCompilationUnit;
		public ICompilation CompilationUnitToReturnFromGetCompilationUnit;
		
		public ICompilation GetCompilationUnit(string fileName)
		{
			FileNamePassedToGetCompilationUnit = fileName;
			return CompilationUnitToReturnFromGetCompilationUnit;
		}
		
		public IProject ProjectPassedToGetCompilationUnit;
		
		public ICompilation GetCompilationUnit(IProject project)
		{
			ProjectPassedToGetCompilationUnit = project;
			return CompilationUnitToReturnFromGetCompilationUnit;
		}
		
		Dictionary<string, IViewContent> openViews = new Dictionary<string, IViewContent>();
		
		public void AddOpenView(IViewContent view, string fileName)
		{
			openViews.Add(fileName, view);
		}
		
		public IViewContent GetOpenFile(string fileName)
		{
			if (openViews.ContainsKey(fileName)) {
				return openViews[fileName];
			}
			return null;
		}
		
		public IViewContent ViewContentPassedToSaveFile;
		public bool IsSaveFileCalled;
		
		public void SaveFile(IViewContent view)
		{
			IsSaveFileCalled = true;
			ViewContentPassedToSaveFile = view;
		}
	}
}
