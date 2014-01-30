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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;
using DTE = ICSharpCode.PackageManagement.EnvDTE;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class FileProjectItemsTests
	{
		TestableDTEProject project;
		FileProjectItems fileProjectItems;
		TestableProject msbuildProject;
		FakeFileService fakeFileService;
		
		void CreateProjectWithOneFileInProjectFolder(
			string include,
			string projectFileName = @"c:\projects\MyProject\MyProject.csproj")
		{
			project = new TestableDTEProject();
			msbuildProject = project.TestableProject;
			fakeFileService = project.FakeFileService;
			msbuildProject.FileName = new FileName(projectFileName);
			msbuildProject.AddFile(include);
		}
		
		void CreateFileProjectItemsFromFileInProjectFolder(string include)
		{
			DTE.ProjectItem projectItem = (DTE.ProjectItem)project.ProjectItems.Item(include);
			fileProjectItems = new DTE.FileProjectItems(projectItem);
		}
		
		IProjectBrowserUpdater CreateProjectBrowserUpdater()
		{
			IProjectBrowserUpdater projectBrowserUpdater = MockRepository.GenerateStub<IProjectBrowserUpdater>();
			project.FakeProjectService.ProjectBrowserUpdater = projectBrowserUpdater;
			return projectBrowserUpdater;
		}
		
		[Test]
		public void AddFromFile_AddFromFileFromProjectItemsBelongingToFile_FileIsParsed()
		{
			string projectFileName = @"d:\projects\myproject\MyProject.csproj";
			CreateProjectWithOneFileInProjectFolder("MainForm.cs", projectFileName);
			CreateFileProjectItemsFromFileInProjectFolder("MainForm.cs");
			string fileName = @"d:\projects\myproject\MainForm.Designer.cs";
			
			fileProjectItems.AddFromFile(fileName);
			
			string parsedFileName = fakeFileService.FileNamePassedToParseFile;
			Assert.AreEqual(fileName, parsedFileName);
		}
		
		[Test]
		public void AddFromFile_AddFromFileFromProjectItemsBelongingToFile_ReturnsProjectItemAdded()
		{
			string projectFileName = @"d:\projects\myproject\MyProject.csproj";
			CreateProjectWithOneFileInProjectFolder("MainForm.cs", projectFileName);
			CreateFileProjectItemsFromFileInProjectFolder("MainForm.cs");
			string fileName = @"d:\projects\myproject\MainForm.Designer.cs";
			
			global::EnvDTE.ProjectItem itemAdded = fileProjectItems.AddFromFile(fileName);
			
			string fullPath = (string)itemAdded.Properties.Item("FullPath").Value;
			Assert.AreEqual("MainForm.Designer.cs", itemAdded.Name);
			Assert.AreEqual(fileName, fullPath);
		}
		
		[Test]
		public void AddFromFile_AddFromFileFromProjectItemsBelongingToFile_ProjectIsSaved()
		{
			string projectFileName = @"d:\projects\myproject\MyProject.csproj";
			CreateProjectWithOneFileInProjectFolder("MainForm.cs", projectFileName);
			CreateFileProjectItemsFromFileInProjectFolder("MainForm.cs");
			string fileName = @"d:\projects\myproject\MainForm.Designer.cs";
			
			fileProjectItems.AddFromFile(fileName);
			
			bool saved = msbuildProject.IsSaved;
			Assert.IsTrue(saved);
		}
		
		[Test]
		public void AddFromFile_AddFromFileFromProjectItemsBelongingToFile_ProjectBrowserUpdaterIsDisposed()
		{
			string projectFileName = @"d:\projects\myproject\MyProject.csproj";
			CreateProjectWithOneFileInProjectFolder("MainForm.cs", projectFileName);
			IProjectBrowserUpdater projectBrowserUpdater = CreateProjectBrowserUpdater();
			CreateFileProjectItemsFromFileInProjectFolder("MainForm.cs");
			string fileName = @"d:\projects\myproject\MainForm.Designer.cs";
			
			fileProjectItems.AddFromFile(fileName);
			
			projectBrowserUpdater.AssertWasCalled(updater => updater.Dispose());
		}
	}
}
