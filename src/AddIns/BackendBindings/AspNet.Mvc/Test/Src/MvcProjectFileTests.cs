// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	public class MvcProjectFileTests
	{
		TestableProject project;
		MvcProjectFile file;
		
		void CreateProject(string fileName)
		{
			project = TestableProject.CreateProject(fileName, "MyProject");
		}
		
		MvcProjectFile CreateProjectFile(string fullPath)
		{
			var projectItem = new FileProjectItem(project, ItemType.Compile);
			projectItem.FileName = fullPath;
			file = new MvcProjectFile(projectItem);
			return file;
		}
		
		[Test]
		public void FullPath_CreatedFromFileProjectItem_ReturnsFullFileNameIncludingFolder()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
			string expectedFullPath = @"d:\projects\MyProject\Views\Shared\Site.Master";
			CreateProjectFile(expectedFullPath);
			
			string fullPath = file.FullPath;
			
			Assert.AreEqual(expectedFullPath, fullPath);
		}
		
		[Test]
		public void FileName_CreatedFromFileProjectItem_ReturnsFileNameWithoutFolder()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
			CreateProjectFile(@"d:\projects\MyProject\Views\Shared\Site.Master");
			
			string fileName = file.FileName;
			
			Assert.AreEqual("Site.Master", fileName);
		}
		
		[Test]
		public void FolderRelativeToProject_CreatedFromFileProjectItem_ReturnsFileNameWithoutFolder()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
			CreateProjectFile(@"d:\projects\MyProject\Views\Shared\Site.Master");
			
			string folder = file.FolderRelativeToProject;
			
			Assert.AreEqual(@"Views\Shared", folder);
		}
		
		[Test]
		public void VirtualPath_CreatedFromFileProjectItem_ReturnsAspNetVirtualPathForFileName()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
			CreateProjectFile(@"d:\projects\MyProject\Views\Shared\Site.Master");
			
			string virtualPath = file.VirtualPath;
			
			Assert.AreEqual("~/Views/Shared/Site.Master", virtualPath);
		}
		
		[Test]
		public void VirtualPath_FileInProjectRootDirectory_ReturnsAspNetVirtualPathForFileName()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
			CreateProjectFile(@"d:\projects\MyProject\Site.Master");
			
			string virtualPath = file.VirtualPath;
			
			Assert.AreEqual("~/Site.Master", virtualPath);
		}
		
		[Test]
		public void CompareTo_FileNamesAreTheSame_ReturnsZero()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
			MvcProjectFile lhs = CreateProjectFile(@"d:\projects\MyProject\Site.Master");
			MvcProjectFile rhs = CreateProjectFile(@"d:\projects\MyProject\Site.Master");
			
			int result = lhs.CompareTo(rhs);
			
			Assert.AreEqual(0, result);
		}
		
		[Test]
		public void CompareTo_FoldersAreSameAndSecondFileNameIsGreaterThanFirstFileName_ReturnsMinusOne()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
			MvcProjectFile lhs = CreateProjectFile(@"d:\projects\MyProject\Shared\A.Master");
			MvcProjectFile rhs = CreateProjectFile(@"d:\projects\MyProject\Shared\Z.Master");
			
			int result = lhs.CompareTo(rhs);
			
			Assert.AreEqual(-1, result);
		}
		
		[Test]
		public void CompareTo_FoldersAreSameAndSecondFileNameIsLessThanFirstFileName_ReturnsPlusOne()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
			MvcProjectFile lhs = CreateProjectFile(@"d:\projects\MyProject\Shared\Z.Master");
			MvcProjectFile rhs = CreateProjectFile(@"d:\projects\MyProject\Shared\A.Master");
			
			int result = lhs.CompareTo(rhs);
			
			Assert.AreEqual(1, result);
		}
		
		[Test]
		public void CompareTo_FileNamesAreSameAndSecondFolderIsLessThanFirstFolder_ReturnsPlusOne()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
			MvcProjectFile lhs = CreateProjectFile(@"d:\projects\MyProject\Z\site.Master");
			MvcProjectFile rhs = CreateProjectFile(@"d:\projects\MyProject\A\site.Master");
			
			int result = lhs.CompareTo(rhs);
			
			Assert.AreEqual(1, result);
		}
		
		[Test]
		public void CompareTo_FileNamesAreSameAndSecondFolderIsGreaterThanFirstFolder_ReturnsMinussOne()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
			MvcProjectFile lhs = CreateProjectFile(@"d:\projects\MyProject\A\site.Master");
			MvcProjectFile rhs = CreateProjectFile(@"d:\projects\MyProject\Z\site.Master");
			
			int result = lhs.CompareTo(rhs);
			
			Assert.AreEqual(-1, result);
		}
	}
}
