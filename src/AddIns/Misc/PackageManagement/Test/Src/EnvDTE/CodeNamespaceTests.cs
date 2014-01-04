// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeNamespaceTests : CodeModelTestBase
	{
		CodeNamespace codeNamespace;
		
		void CreateCodeNamespace(string code, string rootNamespace)
		{
			CreateCodeModel();
			AddCodeFile("class.cs", code);
			
			codeNamespace = codeModel
				.CodeElements
				.FindFirstCodeNamespaceOrDefault(n => n.Name == rootNamespace);
		}
		
		[Test]
		public void Members_NamespaceHasOneClass_ReturnsOneClass()
		{
			string code =
				"namespace Tests {\r\n" +
				"    public class MyClass {}\r\n" +
				"}";
			CreateCodeNamespace(code, "Tests");
			
			global::EnvDTE.CodeElements members = codeNamespace.Members;
			CodeClass2 codeClass = members.FirstCodeClass2OrDefault();
			
			Assert.AreEqual(1, members.Count);
			Assert.AreEqual("Tests.MyClass", codeClass.FullName);
		}
		
		[Test]
		public void Members_NamespaceHasOneChildNamespace_ReturnsOneChildNamespace()
		{
			string code = "namespace First.Second {}";
			CreateCodeNamespace(code, "First");
			
			global::EnvDTE.CodeElements members = codeNamespace.Members;
			CodeNamespace childNamespace = members.FirstCodeNamespaceOrDefault();
			
			Assert.AreEqual("First", codeNamespace.Name);
			Assert.AreEqual(1, members.Count);
			Assert.AreEqual("Second", childNamespace.Name);
			Assert.AreEqual("First.Second", childNamespace.FullName);
		}
		
		[Test]
		public void InfoLocation_NamespaceHasNoClasses_ReturnsExternal()
		{
			string code = "namespace Test {}";
			CreateCodeNamespace(code, "Test");
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal, codeNamespace.InfoLocation);
		}
		
		[Test]
		public void Members_NamespaceHasOneChildNamespaceWithThreeNamespaceParts_ReturnsOneChildNamespaceWhichHasOneChildNamespace()
		{
			string code = "namespace First.Second.Third {}";
			CreateCodeNamespace(code, "First");
			
			global::EnvDTE.CodeElements members = codeNamespace.Members;
			CodeNamespace secondNamespace = members.FirstCodeNamespaceOrDefault();
			CodeNamespace thirdNamespace = secondNamespace.Members.FirstCodeNamespaceOrDefault();
			
			Assert.AreEqual("First", codeNamespace.Name);
			Assert.AreEqual(1, members.Count);
			Assert.AreEqual("Second", secondNamespace.Name);
			Assert.AreEqual("First.Second", secondNamespace.FullName);
			Assert.AreEqual(1, secondNamespace.Members.Count);
			Assert.AreEqual("First.Second.Third", thirdNamespace.FullName);
			Assert.AreEqual(0, thirdNamespace.Members.Count);
		}
		
		[Test]
		public void Members_ProjectHasTwoNamespacesWithCommonFirstAndSecondPartOfThreePartNamespace_ReturnsOneChildNamespaceWhichHasOneChildNamespace()
		{
			string code =
				"namespace First.Second {\r\n" +
				"    namespace Third {}\r\n" +
				"    namespace Different {}\r\n" +
				"}";
			CreateCodeNamespace(code, "First");
			
			global::EnvDTE.CodeElements members = codeNamespace.Members;
			CodeNamespace secondNamespace = members.FirstOrDefault() as CodeNamespace;
			
			Assert.AreEqual("First", codeNamespace.Name);
			Assert.AreEqual(1, members.Count);
			Assert.AreEqual("Second", secondNamespace.Name);
			Assert.AreEqual("First.Second", secondNamespace.FullName);
			Assert.AreEqual(2, secondNamespace.Members.Count);
		}
		
		[Test]
		public void Language_CSharpProject_ReturnsCSharpModelLanguage()
		{
			string code = "namespace Test {}";
			CreateCodeNamespace(code, "Test");
			
			string language = codeNamespace.Language;
			
			Assert.AreEqual(global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp, language);
		}
		
		[Test]
		[Ignore("VB.NET not supported")]
		public void Language_VisualBasicProject_ReturnsVisualBasicModelLanguage()
		{
			string code =
				"Namespace Test\r\n" +
				"End Namespace";
			CreateCodeNamespace(code, "Test");
			
			string language = codeNamespace.Language;
			
			Assert.AreEqual(global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB, language);
		}
		
		[Test]
		public void Kind_NamespaceHasNoClasses_ReturnsNamespace()
		{
			string code = "namespace Test {}";
			CreateCodeNamespace(code, "Test");
			
			global::EnvDTE.vsCMElement kind = codeNamespace.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementNamespace, kind);
		}
		
		[Test]
		public void Members_ParentChildAndGrandChildNamespaces_ReturnsOneCodeNamespaceWhichHasGrandChildNamespace()
		{
			string code = "namespace Parent.Child.GrandChild {}";
			CreateCodeNamespace(code, "Parent");
			
			CodeNamespace childNamespace = codeNamespace.Members.FirstCodeNamespaceOrDefault();
			CodeNamespace grandChildNamespace = childNamespace.Members.FirstCodeNamespaceOrDefault();
			
			Assert.AreEqual("GrandChild", grandChildNamespace.Name);
		}
		
		[Test]
		public void Members_OneInterfaceCompletionEntryAndItemSelectedByName_ReturnsOneCodeInterface()
		{
			string code =
				"namespace Test {\r\n" +
				"    public interface IClass {}\r\n" +
				"}";
			CreateCodeNamespace(code, "Test");
			
			var codeInterface = codeNamespace.Members.Item("IClass") as CodeInterface;
			
			Assert.AreEqual("Test.IClass", codeInterface.FullName);
		}
		
		[Test]
		public void Members_OneClassCompletionEntryAndFirstItemSelected_ReturnsOneCodeClass()
		{
			string code =
				"namespace Test {\r\n" +
				"    public class MyClass {}\r\n" +
				"}";
			CreateCodeNamespace(code, "Test");
			
			var codeClass = codeNamespace.Members.Item(1) as CodeClass2;
			
			Assert.AreEqual("Test.MyClass", codeClass.FullName);
		}
		
		[Test]
		public void Members_OneClassCompletionEntryAndItemSelectedByName_ReturnsOneCodeClass()
		{
			string code =
				"namespace Test {\r\n" +
				"    public class MyClass {}\r\n" +
				"}";
			CreateCodeNamespace(code, "Test");
			
			var codeClass = codeNamespace.Members.Item("MyClass") as CodeClass2;
			
			Assert.AreEqual("Test.MyClass", codeClass.FullName);
		}
	}
}
