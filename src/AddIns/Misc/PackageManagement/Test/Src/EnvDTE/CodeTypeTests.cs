// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using SDProject = ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeTypeTests
	{
		CodeType codeType;
		ClassHelper helper;
		
		void CreateProjectContent()
		{
			helper = new ClassHelper();
		}
		
		void CreateClass(string name)
		{
			helper.CreateClass(name);
		}
		
		void CreateCodeType()
		{
			codeType = new CodeType(helper.ProjectContentHelper.ProjectContent, helper.Class);
		}
		
		TestableProject AddProjectToProjectContent()
		{
			TestableProject project = ProjectHelper.CreateTestProject();
			helper.ProjectContentHelper.SetProjectForProjectContent(project);
			return project;
		}
		
		void AddAttributeToClass(string name)
		{
			helper.AddAttributeToClass(name);
		}
		
		SDProject.FileProjectItem AddFileToProjectAndProjectContent(TestableProject project, string fileName)
		{
			helper.CompilationUnitHelper.SetFileName(fileName);
			return project.AddFile(fileName);			
		}
		
		[Test]
		public void Attributes_ClassHasOneAttribute_ReturnsOneAttribute()
		{
			CreateProjectContent();
			CreateClass("TestClass");
			AddAttributeToClass("TestAttribute");
			CreateCodeType();
			
			CodeElements attributes = codeType.Attributes;
			
			CodeAttribute2 attribute = attributes.Item(1) as CodeAttribute2;
			
			Assert.AreEqual(1, attributes.Count);
			Assert.AreEqual("Test", attribute.Name);
		}
		
		[Test]
		public void ProjectItem_ProjectContentHasNullProject_ReturnsNull()
		{
			CreateProjectContent();
			CreateClass("Class1");
			CreateCodeType();
			
			ProjectItem item = codeType.ProjectItem;
			
			Assert.IsNull(item);
		}
		
		[Test]
		public void ProjectItem_ProjectContentHasProject_ReturnsNonNullProjectItem()
		{
			CreateProjectContent();
			TestableProject project = AddProjectToProjectContent();
			AddFileToProjectAndProjectContent(project, @"d:\projects\MyProject\class1.cs");
			CreateClass("Class1");
			CreateCodeType();
			
			ProjectItem item = codeType.ProjectItem;
			
			Assert.IsNotNull(item);
		}
		
		[Test]
		public void ProjectItem_ProjectContentHasProject_ReturnsProjectItemThatUsesProject()
		{
			CreateProjectContent();
			TestableProject project = AddProjectToProjectContent();
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
			AddFileToProjectAndProjectContent(project, @"d:\projects\MyProject\class1.cs");
			CreateClass("Class1");
			CreateCodeType();
			
			ProjectItem item = codeType.ProjectItem;
			
			Assert.AreEqual(@"d:\projects\MyProject\MyProject.csproj", item.ContainingProject.FileName);
		}
		
		[Test]
		public void ProjectItem_ProjectContentHasProject_ReturnsProjectItemThatUsesProjectFileItem()
		{
			CreateProjectContent();
			TestableProject project = AddProjectToProjectContent();
			string fileName = @"d:\projects\MyProject\test.cs";
			SDProject.FileProjectItem fileProjectItem = AddFileToProjectAndProjectContent(project, fileName);
			
			CreateClass("Class1");
			CreateCodeType();
			
			ProjectItem item = codeType.ProjectItem;
			
			Assert.AreEqual("test.cs", item.Name);
		}
	}
}
