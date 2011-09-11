// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcMasterPageFileNameTests
	{
		TestableProject project;
		MvcMasterPageFileName masterPageFileName;
		
		void CreateProject(string fileName)
		{
			project = TestableProject.CreateProject(fileName, "MyProject");
		}
		
		void CreateMasterPageFileName(string fullPath)
		{
			var projectItem = new FileProjectItem(project, ItemType.Compile);
			projectItem.FileName = fullPath;
			masterPageFileName = new MvcMasterPageFileName(projectItem);
		}
		
		[Test]
		public void FullPath_CreatedFromFileProjectItem_ReturnsFullFileNameIncludingFolder()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
			string expectedFullPath = @"d:\projects\MyProject\Views\Shared\Site.Master";
			CreateMasterPageFileName(expectedFullPath);
			
			string fullPath = masterPageFileName.FullPath;
			
			Assert.AreEqual(expectedFullPath, fullPath);
		}
		
		[Test]
		public void FileName_CreatedFromFileProjectItem_ReturnsFileNameWithoutFolder()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
			CreateMasterPageFileName(@"d:\projects\MyProject\Views\Shared\Site.Master");
			
			string fileName = masterPageFileName.FileName;
			
			Assert.AreEqual("Site.Master", fileName);
		}
		
		[Test]
		public void FolderRelativeToProject_CreatedFromFileProjectItem_ReturnsFileNameWithoutFolder()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
			CreateMasterPageFileName(@"d:\projects\MyProject\Views\Shared\Site.Master");
			
			string folder = masterPageFileName.FolderRelativeToProject;
			
			Assert.AreEqual(@"Views\Shared", folder);
		}
		
		[Test]
		public void VirtualPath_CreatedFromFileProjectItem_ReturnsAspNetVirtualPathForFileName()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
			CreateMasterPageFileName(@"d:\projects\MyProject\Views\Shared\Site.Master");
			
			string virtualPath = masterPageFileName.VirtualPath;
			
			Assert.AreEqual("~/Views/Shared/Site.Master", virtualPath);
		}
		
		[Test]
		public void VirtualPath_FileInProjectRootDirectory_ReturnsAspNetVirtualPathForFileName()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
			CreateMasterPageFileName(@"d:\projects\MyProject\Site.Master");
			
			string virtualPath = masterPageFileName.VirtualPath;
			
			Assert.AreEqual("~/Site.Master", virtualPath);
		}
	}
}
