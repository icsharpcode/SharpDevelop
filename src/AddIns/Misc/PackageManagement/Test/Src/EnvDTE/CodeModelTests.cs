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
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeModelTests : CodeModelTestBase
	{
		void CreateCodeModelWithCSharpProject()
		{
			CreateCodeModel();
			msbuildProject.FileName = new FileName(@"c:\projects\MyProject.csproj");
		}
		
		void CreateCodeModelWithVisualBasicProject()
		{
			CreateCodeModel();
			msbuildProject.FileName = new FileName(@"c:\projects\MyProject.vbproj");
		}
		
		void AddClassToProject(string code)
		{
			AddCodeFile("class.cs", code);
		}
		
		[Test]
		public void CodeTypeFromFullName_NoSuchTypeInProject_ReturnsNull()
		{
			CreateCodeModel();
			
			global::EnvDTE.CodeType codeType = codeModel.CodeTypeFromFullName("UnknownType");
			
			Assert.IsNull(codeType);
		}
		
		[Test]
		public void CodeTypeFromFullName_ClassExistsInProject_ReturnsCodeClass2()
		{
			CreateCodeModel();
			AddClassToProject(
				"namespace Tests {\r\n" +
				"    public class TestClass {} \r\n" +
				"}");
			
			var codeClass = codeModel.CodeTypeFromFullName("Tests.TestClass") as CodeClass2;
			
			Assert.AreEqual("Tests.TestClass", codeClass.FullName);
			Assert.AreEqual("TestClass", codeClass.Name);
		}
		
		[Test]
		public void CodeTypeFromFullName_ClassWithoutNamespaceExistsInProject_ReturnsCodeClass2()
		{
			CreateCodeModel();
			AddClassToProject("public class TestClass {}");
			
			var codeClass = codeModel.CodeTypeFromFullName("TestClass") as CodeClass2;
			
			Assert.AreEqual("TestClass", codeClass.FullName);
		}
		
		[Test]
		public void CodeTypeFromFullName_InterfaceExistsInProject_ReturnsCodeInterface()
		{
			CreateCodeModel();
			AddClassToProject("public interface Interface1 {}");
			
			var codeInterface = codeModel.CodeTypeFromFullName("Interface1") as CodeInterface;
			
			Assert.AreEqual("Interface1", codeInterface.FullName);
		}
		
		[Test]
		public void CodeElements_OneNamespaceInProject_ReturnsOneCodeNamespaceItem()
		{
			CreateCodeModel();
			AddClassToProject("namespace Test {}");
			
			global::EnvDTE.CodeElements codeElements = codeModel.CodeElements;
			global::EnvDTE.CodeNamespace codeNamespace = codeElements
				.FindFirstCodeNamespaceOrDefault(e => e.Name == "Test");
			
			Assert.AreEqual("Test", codeNamespace.FullName);
			Assert.AreEqual("Test", codeNamespace.Name);
		}
		
		[Test]
		public void CodeElements_OneNamespaceInProjectWithTwoPartsToName_ReturnsOneCodeNamespaceItemWithFirstPartOfNamespaceAsName()
		{
			CreateCodeModel();
			AddClassToProject("namespace First.Second {}");
			
			global::EnvDTE.CodeElements codeElements = codeModel.CodeElements;
			global::EnvDTE.CodeNamespace codeNamespace = codeElements
				.FindFirstCodeNamespaceOrDefault(e => e.Name == "First");
			
			global::EnvDTE.CodeNamespace secondCodeNamespace = codeNamespace.Members.FirstCodeNamespaceOrDefault();
			Assert.AreEqual("First", codeNamespace.FullName);
			Assert.AreEqual("First", codeNamespace.Name);
			Assert.AreEqual("Second", secondCodeNamespace.Name);
			Assert.AreEqual("First.Second", secondCodeNamespace.FullName);
		}
		
		[Test]
		public void CodeElements_OneClassWithNoNamespaceInProject_ReturnsOneCodeClassItem()
		{
			CreateCodeModel();
			AddClassToProject("public class TestClass { }");
			
			global::EnvDTE.CodeElements codeElements = codeModel.CodeElements;
			CodeClass2 codeClass = codeElements
				.FindFirstCodeClass2OrDefault(e => e.FullName == "TestClass");
			
			Assert.AreEqual("TestClass", codeClass.Name);
		}
		
		[Test]
		public void CodeElements_TwoNamespacesInProjectWithFirstPartsTheName_ReturnsOneParentNamespaceWithTwoChildNamespaces()
		{
			CreateCodeModel();
			string code = 
				"namespace First.A { }\r\n" +
				"namespace First.B { }\r\n";
			AddClassToProject(code);
			
			global::EnvDTE.CodeElements codeElements = codeModel.CodeElements;
			CodeNamespace codeNamespace = codeElements.FirstCodeNamespaceOrDefault();
			
			global::EnvDTE.CodeElements members = codeNamespace.Members;
			CodeNamespace firstChildNamespace = members.FirstCodeNamespaceOrDefault();
			CodeNamespace secondChildNamespace = members.LastCodeNamespaceOrDefault();
			
			Assert.AreEqual("First", codeNamespace.FullName);
			Assert.AreEqual(2, codeNamespace.Members.Count);
			Assert.AreEqual("A", firstChildNamespace.Name);
			Assert.AreEqual("B", secondChildNamespace.Name);
		}
		
		[Test]
		public void CodeTypeFromFullName_ClassExistsInProject_InfoLocationIsLocalProject()
		{
			CreateCodeModel();
			AddClassToProject(
				"namespace Tests {\r\n" +
				"    public class TestClass {} \r\n" +
				"}");
			
			var codeClass = codeModel.CodeTypeFromFullName("Tests.TestClass") as CodeClass2;
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject, codeClass.InfoLocation);
		}
		
		[Test]
		public void CodeTypeFromFullName_ClassExistsInDifferentAssembly_InfoLocationIsExternal()
		{
			CreateCodeModel();
			
			var codeClass = codeModel.CodeTypeFromFullName("System.String") as CodeClass2;
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal, codeClass.InfoLocation);
		}
		
		[Test]
		public void CodeTypeFromFullName_InterfaceExistsInProject_InfoLocationIsLocalProject()
		{
			CreateCodeModel();
			AddClassToProject(
				"namespace Tests {\r\n" +
				"    public interface ITest {} \r\n" +
				"}");
			
			var codeInterface = codeModel.CodeTypeFromFullName("Tests.ITest") as CodeInterface;
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject, codeInterface.InfoLocation);
		}
		
		[Test]
		public void CodeTypeFromFullName_InterfaceExistsInDifferentAssembly_InfoLocationIsExternal()
		{
			CreateCodeModel();
			
			var codeInterface = codeModel.CodeTypeFromFullName("System.IDisposable") as CodeInterface;
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal, codeInterface.InfoLocation);
		}
		
		[Test]
		public void Language_CSharpProject_ReturnsCSharpProjectGuid()
		{
			CreateCodeModelWithCSharpProject();
			
			string language = codeModel.Language;
			
			Assert.AreEqual(global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp, language);
		}
		
		[Test]
		public void Language_VisualBasicProject_ReturnsVisualBasicProjectGuid()
		{
			CreateCodeModelWithVisualBasicProject();
			
			string language = codeModel.Language;
			
			Assert.AreEqual(global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB, language);
		}
		
		[Test]
		public void CodeTypeFromFullName_SystemString_ReturnsCodeClass2()
		{
			CreateCodeModel();
			AddClassToProject("public class TestClass {}");
			
			var codeClass = codeModel.CodeTypeFromFullName("System.String") as CodeClass2;
			
			Assert.AreEqual("System.String", codeClass.FullName);
		}
	}
}
