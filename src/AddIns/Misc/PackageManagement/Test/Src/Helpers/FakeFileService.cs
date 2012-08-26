// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

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
			ProjectItem item = project.Items[0];
			ProjectService.RemoveProjectItem(project, item);
		}
		
		public string FileNamePassedToOpenFile;
		
		public void OpenFile(string fileName)
		{
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
		public ICompilationUnit CompilationUnitToReturnFromGetCompilationUnit =
			new DefaultCompilationUnit(new DefaultProjectContent());
		
		public ICompilationUnit GetCompilationUnit(string fileName)
		{
			FileNamePassedToGetCompilationUnit = fileName;
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
	}
}
