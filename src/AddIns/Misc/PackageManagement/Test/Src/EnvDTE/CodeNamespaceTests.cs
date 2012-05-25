// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeNamespaceTests
	{
		CodeNamespace codeNamespace;
		ProjectContentHelper helper;
		
		void CreateCodeNamespace(string namespaceName)
		{
			codeNamespace = new CodeNamespace(helper.FakeProjectContent, namespaceName);
		}
		
		void CreateProjectContent()
		{
			helper = new ProjectContentHelper();
		}
		
		void AddClassToProjectContent(string namespaceName, string className)
		{
			helper.AddClassToProjectContent(namespaceName, className);
		}
		
		void AddNamespaceToProjectContent(string name)
		{
			helper.AddNamespaceNameToProjectContent(name);
		}
		
		void NoClassesInNamespace(string name)
		{
			helper.AddEmptyNamespaceContents(name);
		}
				
		void AddUnknownCompletionEntryToNamespace(string namespaceName)
		{
			helper.AddUnknownCompletionEntryTypeToNamespace(namespaceName);
		}
		
		[Test]
		public void Members_NamespaceHasOneClass_ReturnsOneClass()
		{
			CreateProjectContent();
			AddNamespaceToProjectContent("Tests");
			AddClassToProjectContent("Tests", "Tests.MyClass");
			CreateCodeNamespace("Tests");
			
			CodeElements members = codeNamespace.Members;
			CodeClass2 codeClass = members.FirstOrDefault() as CodeClass2;
			
			Assert.AreEqual(1, members.Count);
			Assert.AreEqual("Tests.MyClass", codeClass.FullName); 
		}
		
		[Test]
		public void Members_NamespaceHasOneChildNamespace_ReturnsOneChildNamespace()
		{
			CreateProjectContent();
			AddNamespaceToProjectContent("First.Second");
			NoClassesInNamespace("First");
			NoClassesInNamespace("First.Second");
			CreateCodeNamespace("First");
			
			CodeElements members = codeNamespace.Members;
			CodeNamespace childNamespace = members.FirstOrDefault() as CodeNamespace;
			
			Assert.AreEqual("First", codeNamespace.Name);
			Assert.AreEqual(1, members.Count);
			Assert.AreEqual("Second", childNamespace.Name);
			Assert.AreEqual("Second", childNamespace.FullName);
		}
		
		[Test]
		public void InfoLocation_NamespaceHasNoClasses_ReturnsExternal()
		{
			CreateProjectContent();
			CreateCodeNamespace("Test");
			
			Assert.AreEqual(vsCMInfoLocation.vsCMInfoLocationExternal, codeNamespace.InfoLocation);
		}
		
		[Test]
		public void Members_NamespaceHasOneChildNamespaceWithThreeNamespaceParts_ReturnsOneChildNamespaceWhichHasOneChildNamespace()
		{
			CreateProjectContent();
			AddNamespaceToProjectContent("First.Second.Third");
			NoClassesInNamespace("First");
			NoClassesInNamespace("First.Second");
			NoClassesInNamespace("First.Second.Third");
			CreateCodeNamespace("First");
			
			CodeElements members = codeNamespace.Members;
			CodeNamespace secondNamespace = members.FirstOrDefault() as CodeNamespace;
			CodeNamespace thirdNamespace = secondNamespace.Members.FirstOrDefault() as CodeNamespace;
			
			Assert.AreEqual("First", codeNamespace.Name);
			Assert.AreEqual(1, members.Count);
			Assert.AreEqual("Second", secondNamespace.Name);
			Assert.AreEqual("Second", secondNamespace.FullName);
			Assert.AreEqual(1, secondNamespace.Members.Count);
			Assert.AreEqual("Third", thirdNamespace.FullName);
			Assert.AreEqual(0, thirdNamespace.Members.Count);
		}
		
		[Test]
		public void Members_ProjectHasTwoNamespacesWithCommonFirstAndSecondPartOfThreePartNamespace_ReturnsOneChildNamespaceWhichHasOneChildNamespace()
		{
			CreateProjectContent();
			AddNamespaceToProjectContent("First.Second.Third");
			AddNamespaceToProjectContent("First.Second.Different");
			NoClassesInNamespace("First");
			NoClassesInNamespace("First.Second");
			NoClassesInNamespace("First.Second.Third");
			NoClassesInNamespace("First.Second.Different");
			CreateCodeNamespace("First");
			
			CodeElements members = codeNamespace.Members;
			CodeNamespace secondNamespace = members.FirstOrDefault() as CodeNamespace;
			
			Assert.AreEqual("First", codeNamespace.Name);
			Assert.AreEqual(1, members.Count);
			Assert.AreEqual("Second", secondNamespace.Name);
			Assert.AreEqual("Second", secondNamespace.FullName);
			Assert.AreEqual(2, secondNamespace.Members.Count);
		}
		
		[Test]
		public void Members_NamespaceHasUnknownNamespaceEntryType_ReturnsNoItems()
		{
			CreateProjectContent();
			AddNamespaceToProjectContent("Tests");
			AddUnknownCompletionEntryToNamespace("Tests");
			CreateCodeNamespace("Tests");
			
			CodeElements members = codeNamespace.Members;
			
			Assert.AreEqual(0, members.Count);
		}
	}
}
