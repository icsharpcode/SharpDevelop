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
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using FileProjectItem = ICSharpCode.SharpDevelop.Project.FileProjectItem;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;
using SDProject = ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeTypeTests : CodeModelTestBase
	{
		CodeType codeType;
		
		void CreateCodeType(string code, string fileName = "class.cs")
		{
			AddCodeFile(fileName, code);
			ITypeDefinition typeDefinition = assemblyModel.TopLevelTypeDefinitions.Single().Resolve();
			CreateCodeType(typeDefinition);
		}
		
		void CreateCodeType(ITypeDefinition typeDefinition)
		{
			codeType = new CodeType(codeModelContext, typeDefinition);
		}
		
		FileProjectItem AddFileToProject(string fileName)
		{
			var projectItem = new FileProjectItem(project, ICSharpCode.SharpDevelop.Project.ItemType.Compile);
			project
				.Stub(p => p.FindFile(new FileName(fileName)))
				.Return(projectItem);
			
			project
				.Stub(p => p.SyncRoot)
				.Return(new object());
			
			return projectItem;
		}
		
		[Test]
		public void Attributes_ClassHasOneAttribute_ReturnsOneAttribute()
		{
			CreateCodeType("[TestAttribute] public class TestClass {}");
			
			global::EnvDTE.CodeElements attributes = codeType.Attributes;
			
			CodeAttribute2 attribute = attributes.Item(1) as CodeAttribute2;
			Assert.AreEqual(1, attributes.Count);
			Assert.AreEqual("Test", attribute.Name);
		}
		
		[Test]
		public void ProjectItem_TypeNotFromProject_ReturnsNull()
		{
			AddCodeFile("class.cs", "public class TestClass {}");
			ITypeDefinition typeDefinition = projectContent
				.CreateCompilation()
				.ReferencedAssemblies
				.FirstOrDefault()
				.TopLevelTypeDefinitions.First();
			CreateCodeType(typeDefinition);
			
			global::EnvDTE.ProjectItem item = codeType.ProjectItem;
			
			Assert.IsNull(item);
		}
		
		[Test]
		public void ProjectItem_TypeIsFromProject_ReturnsNonNullProjectItem()
		{
			string fileName = @"d:\projects\MyProject\class1.cs";
			CreateCodeType("public class TestClass {}", fileName);
			AddFileToProject(fileName);
			
			global::EnvDTE.ProjectItem item = codeType.ProjectItem;
			
			Assert.IsNotNull(item);
		}

		[Test]
		public void ProjectItem_TypeIsFromProject_ReturnsProjectItemThatUsesProject()
		{
			string fileName = @"d:\projects\MyProject\class1.cs";
			CreateCodeType("public class TestClass {}", fileName);
			TestableProject testableProject = ProjectHelper.CreateTestProject();
			testableProject.FileName = new FileName(@"d:\projects\MyProject\MyProject.csproj");
			codeModelContext.CurrentProject = testableProject;
			testableProject.AddFile(fileName);
			
			global::EnvDTE.ProjectItem item = codeType.ProjectItem;

			Assert.AreEqual(@"d:\projects\MyProject\MyProject.csproj", item.ContainingProject.FileName);
		}

		[Test]
		public void ProjectItem_ProjectContentHasProject_ReturnsProjectItemThatUsesProjectFileItem()
		{
			string fileName = @"d:\projects\MyProject\test.cs";
			CreateCodeType("public class TestClass {}", fileName);
			TestableProject testableProject = ProjectHelper.CreateTestProject();
			testableProject.FileName = new FileName(@"d:\projects\MyProject\MyProject.csproj");
			codeModelContext.CurrentProject = testableProject;
			testableProject.AddFile(fileName);

			global::EnvDTE.ProjectItem item = codeType.ProjectItem;

			Assert.AreEqual("test.cs", item.Name);
		}

		[Test]
		public void IsDerivedFrom_ClassFullyQualifiedNameMatchesTypeNameBeingChecked_ReturnsTrue()
		{
			CreateCodeType(
				"namespace System.Web.Mvc {\r\n" +
				"    public class ActionResult {}\r\n" +
				"}");

			bool derivedFrom = codeType.get_IsDerivedFrom("System.Web.Mvc.ActionResult");

			Assert.IsTrue(derivedFrom);
		}

		[Test]
		public void IsDerivedFrom_ClassFullyQualifiedNameDoesNotMatchTypeNameBeingChecked_ReturnsFalse()
		{
			CreateCodeType("public class Test {}");

			bool derivedFrom = codeType.get_IsDerivedFrom("System.Web.Mvc.ActionResult");

			Assert.IsFalse(derivedFrom);
		}

		[Test]
		public void IsDerivedFrom_ClassBaseTypeFullyQualifiedNameMatchesTypeName_ReturnsTrue()
		{
			AddCodeFile(
				"class.cs",
				"namespace System.Web.Mvc {\r\n" +
				"    public class CustomActionResult : ActionResult {}\r\n" +
				"    public class ActionResult {}\r\n" +
				"}");
			CreateCodeType(assemblyModel.TopLevelTypeDefinitions.First().Resolve());
			
			bool derivedFrom = codeType.get_IsDerivedFrom("System.Web.Mvc.ActionResult");

			Assert.IsTrue(derivedFrom);
		}

		[Test]
		public void IsDerivedFrom_ClassHasTypeInClassInheritanceTreeButNotImmediateBaseType_ReturnsTrue()
		{
			AddCodeFile(
				"class.cs",
				"namespace System.Web.Mvc {\r\n" +
				"    public class CustomActionResult : CustomActionResultBase {}\r\n" +
				"    public class CustomActionResultBase : ActionResult {}\r\n" +
				"    public class ActionResult {}\r\n" +
				"}");
			CreateCodeType(assemblyModel.TopLevelTypeDefinitions.First().Resolve());

			bool derivedFrom = codeType.get_IsDerivedFrom("System.Web.Mvc.ActionResult");

			Assert.IsTrue(derivedFrom);
		}
	}
}
