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

//using System;
//using ICSharpCode.PackageManagement.EnvDTE;
//using ICSharpCode.SharpDevelop.Dom;
//using SDProject = ICSharpCode.SharpDevelop.Project;
//using NUnit.Framework;
//using PackageManagement.Tests.Helpers;
//
//namespace PackageManagement.Tests.EnvDTE
//{
//	[TestFixture]
//	public class CodeTypeTests
//	{
//		CodeType codeType;
//		ClassHelper helper;
//		
//		void CreateProjectContent()
//		{
//			helper = new ClassHelper();
//		}
//		
//		void CreateClass(string name)
//		{
//			helper.CreateClass(name);
//		}
//		
//		void CreateCodeType()
//		{
//			codeType = new CodeType(helper.ProjectContentHelper.ProjectContent, helper.Class);
//		}
//		
//		TestableProject AddProjectToProjectContent()
//		{
//			TestableProject project = ProjectHelper.CreateTestProject();
//			helper.ProjectContentHelper.SetProjectForProjectContent(project);
//			return project;
//		}
//		
//		void AddAttributeToClass(string name)
//		{
//			helper.AddAttributeToClass(name);
//		}
//		
//		SDProject.FileProjectItem AddFileToProjectAndProjectContent(TestableProject project, string fileName)
//		{
//			helper.CompilationUnitHelper.SetFileName(fileName);
//			return project.AddFile(fileName);
//		}
//		
//		/// <summary>
//		/// Classes at the end of the array are at the top of the inheritance tree.
//		/// </summary>
//		void AddClassInheritanceTree(params string[] classNames)
//		{
//			helper.AddClassInheritanceTreeClassesOnly(classNames);
//		}
//		
//		[Test]
//		public void Attributes_ClassHasOneAttribute_ReturnsOneAttribute()
//		{
//			CreateProjectContent();
//			CreateClass("TestClass");
//			AddAttributeToClass("TestAttribute");
//			CreateCodeType();
//			
//			global::EnvDTE.CodeElements attributes = codeType.Attributes;
//			
//			CodeAttribute2 attribute = attributes.Item(1) as CodeAttribute2;
//			
//			Assert.AreEqual(1, attributes.Count);
//			Assert.AreEqual("Test", attribute.Name);
//		}
//		
//		[Test]
//		public void ProjectItem_ProjectContentHasNullProject_ReturnsNull()
//		{
//			CreateProjectContent();
//			CreateClass("Class1");
//			CreateCodeType();
//			
//			global::EnvDTE.ProjectItem item = codeType.ProjectItem;
//			
//			Assert.IsNull(item);
//		}
//		
//		[Test]
//		public void ProjectItem_ProjectContentHasProject_ReturnsNonNullProjectItem()
//		{
//			CreateProjectContent();
//			TestableProject project = AddProjectToProjectContent();
//			AddFileToProjectAndProjectContent(project, @"d:\projects\MyProject\class1.cs");
//			CreateClass("Class1");
//			CreateCodeType();
//			
//			global::EnvDTE.ProjectItem item = codeType.ProjectItem;
//			
//			Assert.IsNotNull(item);
//		}
//		
//		[Test]
//		public void ProjectItem_ProjectContentHasProject_ReturnsProjectItemThatUsesProject()
//		{
//			CreateProjectContent();
//			TestableProject project = AddProjectToProjectContent();
//			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
//			AddFileToProjectAndProjectContent(project, @"d:\projects\MyProject\class1.cs");
//			CreateClass("Class1");
//			CreateCodeType();
//			
//			global::EnvDTE.ProjectItem item = codeType.ProjectItem;
//			
//			Assert.AreEqual(@"d:\projects\MyProject\MyProject.csproj", item.ContainingProject.FileName);
//		}
//		
//		[Test]
//		public void ProjectItem_ProjectContentHasProject_ReturnsProjectItemThatUsesProjectFileItem()
//		{
//			CreateProjectContent();
//			TestableProject project = AddProjectToProjectContent();
//			string fileName = @"d:\projects\MyProject\test.cs";
//			SDProject.FileProjectItem fileProjectItem = AddFileToProjectAndProjectContent(project, fileName);
//			
//			CreateClass("Class1");
//			CreateCodeType();
//			
//			global::EnvDTE.ProjectItem item = codeType.ProjectItem;
//			
//			Assert.AreEqual("test.cs", item.Name);
//		}
//		
//		[Test]
//		public void IsDerivedFrom_ClassFullyQualifiedNameMatchesTypeNameBeingChecked_ReturnsTrue()
//		{
//			CreateProjectContent();
//			CreateClass("System.Web.Mvc.ActionResult");
//			CreateCodeType();
//			
//			bool derivedFrom = codeType.get_IsDerivedFrom("System.Web.Mvc.ActionResult");
//			
//			Assert.IsTrue(derivedFrom);
//		}
//		
//		[Test]
//		public void IsDerivedFrom_ClassFullyQualifiedNameDoesNotMatcheTypeNameBeingChecked_ReturnsFalse()
//		{
//			CreateProjectContent();
//			CreateClass("TestClass");
//			AddClassInheritanceTree("System.Object");
//			CreateCodeType();
//			
//			bool derivedFrom = codeType.get_IsDerivedFrom("System.Web.Mvc.ActionResult");
//			
//			Assert.IsFalse(derivedFrom);
//		}
//		
//		[Test]
//		public void IsDerivedFrom_ClassBaseTypeFullyQualifiedNameMatchesTypeName_ReturnsTrue()
//		{
//			CreateProjectContent();
//			CreateClass("CustomActionResult");
//			helper.AddBaseTypeToClass("System.Web.Mvc.ActionResult");
//			CreateCodeType();
//			
//			bool derivedFrom = codeType.get_IsDerivedFrom("System.Web.Mvc.ActionResult");
//			
//			Assert.IsTrue(derivedFrom);
//		}
//		
//		[Test]
//		public void IsDerivedFrom_ClassHasTypeInClassInheritanceTreeButNotImmediateBaseType_ReturnsTrue()
//		{
//			CreateProjectContent();
//			CreateClass("CustomActionResult");
//			AddClassInheritanceTree("CustomActionResultBase", "System.Web.Mvc.ActionResult");
//			CreateCodeType();
//			
//			bool derivedFrom = codeType.get_IsDerivedFrom("System.Web.Mvc.ActionResult");
//			
//			Assert.IsTrue(derivedFrom);
//		}
//		
//		[Test]
//		public void IsDerivedFrom_ClassHasClassInInheritanceTreeButNotImmediateParentAndClassBaseTypePropertyIsNotNull_ReturnsTrue()
//		{
//			CreateProjectContent();
//			CreateClass("CustomActionResult");
//			helper.AddBaseTypeToClass("CustomActionResultBase");
//			AddClassInheritanceTree("CustomActionResultBase", "System.Web.Mvc.ActionResult");
//			CreateCodeType();
//			
//			bool derivedFrom = codeType.get_IsDerivedFrom("System.Web.Mvc.ActionResult");
//			
//			Assert.IsTrue(derivedFrom);
//		}
//	}
//}
