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
//using NUnit.Framework;
//using PackageManagement.Tests.Helpers;
//
//namespace PackageManagement.Tests.EnvDTE
//{
//	[TestFixture]
//	public class CodeNamespaceTests
//	{
//		CodeNamespace codeNamespace;
//		ProjectContentHelper helper;
//		
//		void CreateCodeNamespace(string namespaceName)
//		{
//			codeNamespace = new CodeNamespace(helper.ProjectContent, namespaceName);
//		}
//		
//		void CreateProjectContent()
//		{
//			helper = new ProjectContentHelper();
//		}
//		
//		void AddClassToProjectContent(string namespaceName, string className)
//		{
//			helper.AddClassToProjectContentAndCompletionEntries(namespaceName, className);
//		}
//				
//		void AddUnknownCompletionEntryToNamespace(string namespaceName)
//		{
//			helper.AddUnknownCompletionEntryTypeToNamespace(namespaceName);
//		}
//		
//		[Test]
//		public void Members_NamespaceHasOneClass_ReturnsOneClass()
//		{
//			CreateProjectContent();
//			AddClassToProjectContent("Tests", "Tests.MyClass");
//			CreateCodeNamespace("Tests");
//			
//			global::EnvDTE.CodeElements members = codeNamespace.Members;
//			CodeClass2 codeClass = members.FirstCodeClass2OrDefault();
//			
//			Assert.AreEqual(1, members.Count);
//			Assert.AreEqual("Tests.MyClass", codeClass.FullName); 
//		}
//		
//		[Test]
//		public void Members_NamespaceHasOneChildNamespace_ReturnsOneChildNamespace()
//		{
//			CreateProjectContent();
//			helper.AddNamespaceCompletionEntryInNamespace("First", "Second");
//			CreateCodeNamespace("First");
//			
//			global::EnvDTE.CodeElements members = codeNamespace.Members;
//			CodeNamespace childNamespace = members.FirstCodeNamespaceOrDefault();
//			
//			Assert.AreEqual("First", codeNamespace.Name);
//			Assert.AreEqual(1, members.Count);
//			Assert.AreEqual("Second", childNamespace.Name);
//			Assert.AreEqual("First.Second", childNamespace.FullName);
//		}
//		
//		[Test]
//		public void InfoLocation_NamespaceHasNoClasses_ReturnsExternal()
//		{
//			CreateProjectContent();
//			CreateCodeNamespace("Test");
//			
//			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal, codeNamespace.InfoLocation);
//		}
//		
//		[Test]
//		public void Members_NamespaceHasOneChildNamespaceWithThreeNamespaceParts_ReturnsOneChildNamespaceWhichHasOneChildNamespace()
//		{
//			CreateProjectContent();
//			helper.AddNamespaceCompletionEntryInNamespace("First", "Second");
//			helper.AddNamespaceCompletionEntryInNamespace("First.Second", "Third");
//			helper.NoCompletionItemsInNamespace("First.Second.Third");
//			CreateCodeNamespace("First");
//			
//			global::EnvDTE.CodeElements members = codeNamespace.Members;
//			CodeNamespace secondNamespace = members.FirstCodeNamespaceOrDefault();
//			CodeNamespace thirdNamespace = secondNamespace.Members.FirstCodeNamespaceOrDefault();
//			
//			Assert.AreEqual("First", codeNamespace.Name);
//			Assert.AreEqual(1, members.Count);
//			Assert.AreEqual("Second", secondNamespace.Name);
//			Assert.AreEqual("First.Second", secondNamespace.FullName);
//			Assert.AreEqual(1, secondNamespace.Members.Count);
//			Assert.AreEqual("First.Second.Third", thirdNamespace.FullName);
//			Assert.AreEqual(0, thirdNamespace.Members.Count);
//		}
//		
//		[Test]
//		public void Members_ProjectHasTwoNamespacesWithCommonFirstAndSecondPartOfThreePartNamespace_ReturnsOneChildNamespaceWhichHasOneChildNamespace()
//		{
//			CreateProjectContent();
//			helper.AddNamespaceCompletionEntryInNamespace("First", "Second");
//			helper.AddNamespaceCompletionEntriesInNamespace("First.Second", "Third", "Different");
//			CreateCodeNamespace("First");
//			
//			global::EnvDTE.CodeElements members = codeNamespace.Members;
//			CodeNamespace secondNamespace = members.FirstOrDefault() as CodeNamespace;
//			
//			Assert.AreEqual("First", codeNamespace.Name);
//			Assert.AreEqual(1, members.Count);
//			Assert.AreEqual("Second", secondNamespace.Name);
//			Assert.AreEqual("First.Second", secondNamespace.FullName);
//			Assert.AreEqual(2, secondNamespace.Members.Count);
//		}
//		
//		[Test]
//		public void Members_NamespaceHasUnknownNamespaceEntryType_ReturnsNoItems()
//		{
//			CreateProjectContent();
//			AddUnknownCompletionEntryToNamespace("Tests");
//			CreateCodeNamespace("Tests");
//			
//			global::EnvDTE.CodeElements members = codeNamespace.Members;
//			
//			Assert.AreEqual(0, members.Count);
//		}
//		
//		[Test]
//		public void Language_CSharpProject_ReturnsCSharpModelLanguage()
//		{
//			CreateProjectContent();
//			helper.ProjectContentIsForCSharpProject();
//			CreateCodeNamespace(String.Empty);
//			
//			string language = codeNamespace.Language;
//			
//			Assert.AreEqual(global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp, language);
//		}
//		
//		[Test]
//		public void Language_VisualBasicProject_ReturnsVisualBasicModelLanguage()
//		{
//			CreateProjectContent();
//			helper.ProjectContentIsForVisualBasicProject();
//			CreateCodeNamespace(String.Empty);
//			
//			string language = codeNamespace.Language;
//			
//			Assert.AreEqual(global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB, language);
//		}
//		
//		[Test]
//		public void Kind_EmptyStringNamespace_ReturnsNamespace()
//		{
//			CreateProjectContent();
//			CreateCodeNamespace(String.Empty);
//			
//			global::EnvDTE.vsCMElement kind = codeNamespace.Kind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementNamespace, kind);
//		}
//	}
//}
