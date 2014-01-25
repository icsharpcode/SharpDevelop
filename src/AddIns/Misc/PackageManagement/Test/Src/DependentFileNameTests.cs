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
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class DependentFileTests
	{
		TestableProject project;
		DependentFile dependentFile;
		
		void CreateCSharpProject(string projectFileName)
		{
			project = ProjectHelper.CreateTestProject();
			project.FileName = new FileName(projectFileName);
		}
		
		FileProjectItem AddFileToProject(string include)
		{
			return project.AddFile(include);
		}
		
		void CreateDependentFile()
		{
			dependentFile = new DependentFile(project);
		}
		
		[SetUp]
		public void Init()
		{
			SD.InitializeForUnitTests();
		}
		
		[Test]
		public void GetParentFileProjectItem_ResxFileNameAndCSharpParentFileExistsInProject_ReturnsParentFileProjectItem()
		{
			CreateCSharpProject(@"d:\projects\MyProject\MyProject.csproj");
			FileProjectItem expectedProjectItem = AddFileToProject("test.cs");
			CreateDependentFile();
			string fileName = @"d:\projects\MyProject\test.resx";
			
			FileProjectItem projectItem = dependentFile.GetParentFileProjectItem(fileName);
			
			Assert.AreEqual(expectedProjectItem, projectItem);
		}
		
		[Test]
		public void GetParentFileProjectItem_ResxFileNameAndProjectHasNoFiles_ReturnsNull()
		{
			CreateCSharpProject(@"d:\projects\MyProject\MyProject.csproj");
			CreateDependentFile();
			string fileName = @"d:\projects\MyProject\test.resx";
			
			FileProjectItem projectItem = dependentFile.GetParentFileProjectItem(fileName);
			
			Assert.IsNull(projectItem);
		}
		
		[Test]
		public void GetParentFileProjectItem_ResxFileNameAndProjectHasOneCSharpFileWithDifferentFileNameToResxFile_ReturnsNull()
		{
			CreateCSharpProject(@"d:\projects\MyProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\MyProject\program.cs");
			CreateDependentFile();
			string fileName = @"d:\projects\MyProject\test.resx";
			
			FileProjectItem projectItem = dependentFile.GetParentFileProjectItem(fileName);
			
			Assert.IsNull(projectItem);
		}
		
		[Test]
		public void GetParentFileProjectItem_ResxFileNameAndVisualBasicParentFileExistsInProject_ReturnsParentFileProjectItem()
		{
			CreateCSharpProject(@"d:\projects\MyProject\MyProject.vbproj");
			FileProjectItem expectedProjectItem = AddFileToProject("test.vb");
			CreateDependentFile();
			string fileName = @"d:\projects\MyProject\test.resx";
			
			FileProjectItem projectItem = dependentFile.GetParentFileProjectItem(fileName);
			
			Assert.AreEqual(expectedProjectItem, projectItem);
		}
		
		[Test]
		public void GetParentFileProjectItem_ResxFileNameAndVisualBasicParentFileExistsInProjectWhenProjectFileExtensionIsInUpperCase_ReturnsParentFileProjectItem()
		{
			CreateCSharpProject(@"d:\projects\MyProject\MyProject.VBPROJ");
			FileProjectItem expectedProjectItem = AddFileToProject("test.vb");
			CreateDependentFile();
			string fileName = @"d:\projects\MyProject\test.resx";
			
			FileProjectItem projectItem = dependentFile.GetParentFileProjectItem(fileName);
			
			Assert.AreEqual(expectedProjectItem, projectItem);
		}
		
		[Test]
		public void GetParentFileProjectItem_DesignerFileNameAndCSharpParentFileExistsInProject_ReturnsParentFileProjectItem()
		{
			CreateCSharpProject(@"d:\projects\MyProject\MyProject.csproj");
			FileProjectItem expectedProjectItem = AddFileToProject("test.cs");
			CreateDependentFile();
			string fileName = @"d:\projects\MyProject\test.Designer.cs";
			
			FileProjectItem projectItem = dependentFile.GetParentFileProjectItem(fileName);
			
			Assert.AreEqual(expectedProjectItem, projectItem);
		}
		
		[Test]
		public void GetParentFileProjectItem_DesignerFileNameInLowerCaseAndCSharpParentFileExistsInProject_ReturnsParentFileProjectItem()
		{
			CreateCSharpProject(@"d:\projects\MyProject\MyProject.csproj");
			FileProjectItem expectedProjectItem = AddFileToProject("test.cs");
			CreateDependentFile();
			string fileName = @"d:\projects\MyProject\test.designer.cs";
			
			FileProjectItem projectItem = dependentFile.GetParentFileProjectItem(fileName);
			
			Assert.AreEqual(expectedProjectItem, projectItem);
		}
		
		[Test]
		public void GetParentFileProjectItem_DesignerInFileNameButMissingDot_ReturnsNull()
		{
			CreateCSharpProject(@"d:\projects\MyProject\MyProject.csproj");
			AddFileToProject("abc.cs");
			CreateDependentFile();
			string fileName = @"d:\projects\MyProject\abc2Designer.cs";
			
			FileProjectItem projectItem = dependentFile.GetParentFileProjectItem(fileName);
			
			Assert.IsNull(projectItem);
		}
		
		[Test]
		public void GetParentFileProjectItem_DesignerInFolderName_ReturnsNull()
		{
			CreateCSharpProject(@"d:\projects\MyProject\MyProject.csproj");
			AddFileToProject("abc.cs");
			CreateDependentFile();
			string fileName = @"d:\projects\MyProject\abc.Designer\foo.cs";
			
			FileProjectItem projectItem = dependentFile.GetParentFileProjectItem(fileName);
			
			Assert.IsNull(projectItem);
		}
		
		[Test]
		public void GetParentFileProjectItem_DesignerInFolderNameAndInFileName_ReturnsMatchingParentProjectItem()
		{
			CreateCSharpProject(@"d:\projects\MyProject\MyProject.csproj");
			FileProjectItem expectedProjectItem = AddFileToProject(@"Form.Designer\abc.cs");
			CreateDependentFile();
			string fileName = @"d:\projects\MyProject\Form.Designer\abc.Designer.cs";
			
			FileProjectItem projectItem = dependentFile.GetParentFileProjectItem(fileName);
			
			Assert.AreEqual(expectedProjectItem, projectItem);
		}
	}
}
