// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
			msbuildProject.FileName = projectFileName;
			msbuildProject.AddFile(include);
		}
		
		void CreateFileProjectItemsFromFileInProjectFolder(string include)
		{
			DTE.ProjectItem projectItem = (DTE.ProjectItem)project.ProjectItems.Item(include);
			fileProjectItems = new DTE.FileProjectItems(projectItem, fakeFileService);
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
