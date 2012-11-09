// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
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
			project.FileName = projectFileName;
		}
		
		FileProjectItem AddFileToProject(string include)
		{
			return project.AddFile(include);
		}
		
		void CreateDependentFile()
		{
			dependentFile = new DependentFile(project);
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
