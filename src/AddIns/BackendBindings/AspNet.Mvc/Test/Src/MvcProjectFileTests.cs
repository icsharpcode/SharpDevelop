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
using ICSharpCode.Core;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	public class MvcProjectFileTests : MvcTestsBase
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
			projectItem.FileName = FileName.Create(fullPath);
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
