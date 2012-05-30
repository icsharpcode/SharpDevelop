// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeClass2Tests
	{
		CodeClass2 codeClass;
		ProjectContentHelper helper;
		IClass fakeClass;
		
		void CreateProjectContent()
		{
			helper = new ProjectContentHelper();
		}
		
		void CreateClass(string name)
		{
			fakeClass = helper.AddClassToProjectContent(name);
			CreateClass();
		}
		
		void CreatePublicClass(string name)
		{
			fakeClass = helper.AddPublicClassToProjectContent(name);
			CreateClass();
		}
		
		void CreatePrivateClass(string name)
		{
			fakeClass = helper.AddPrivateClassToProjectContent(name);
			CreateClass();
		}
		
		void CreateClass()
		{
			codeClass = new CodeClass2(helper.FakeProjectContent, fakeClass);
		}
		
		void AddInterfaceToProjectContent(string fullName)
		{
			helper.AddInterfaceToProjectContent(fullName);
		}
		
		void AddClassToProjectContent(string fullName)
		{
			helper.AddClassToProjectContent(fullName);
		}
		
		void AddBaseTypeInterfaceToClass(string fullName, string dotNetName)
		{
			helper.AddInterfaceToClassBaseTypes(fakeClass, fullName, dotNetName);
		}
		
		void AddBaseTypeClassToClass(string fullName)
		{
			helper.AddClassToClassBaseTypes(fakeClass, fullName);
		}
		
		[Test]
		public void Language_CSharpProject_ReturnsCSharpModelLanguage()
		{
			CreateProjectContent();
			helper.ProjectContentIsForCSharpProject();
			CreateClass("MyClass");
			
			string language = codeClass.Language;
			
			Assert.AreEqual(CodeModelLanguageConstants.vsCMLanguageCSharp, language);
		}
		
		[Test]
		public void Language_VisualBasicProject_ReturnsVisualBasicModelLanguage()
		{
			CreateProjectContent();
			helper.ProjectContentIsForVisualBasicProject();
			CreateClass("MyClass");
			
			string language = codeClass.Language;
			
			Assert.AreEqual(CodeModelLanguageConstants.vsCMLanguageVB, language);
		}
		
		[Test]
		public void Access_PublicClass_ReturnsPublic()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			
			vsCMAccess access = codeClass.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_PrivateClass_ReturnsPrivate()
		{
			CreateProjectContent();
			CreatePrivateClass("MyClass");
			
			vsCMAccess access = codeClass.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void ImplementedInterfaces_ClassImplementsGenericICollectionOfString_ReturnsCodeInterfaceForICollection()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			AddBaseTypeInterfaceToClass("System.Collections.Generic.ICollection", "System.Collections.Generic.ICollection{System.String}");
			
			CodeElements codeElements = codeClass.ImplementedInterfaces;
			CodeInterface codeInterface = codeElements.FirstOrDefault() as CodeInterface;
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("System.Collections.Generic.ICollection<System.String>", codeInterface.FullName);
		}
		
		[Test]
		public void ImplementedInterfaces_ClassHasBaseTypeButNoInterfaces_ReturnsNoItems()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			AddBaseTypeClassToClass("MyNamespace.MyBaseClass");
			
			CodeElements codeElements = codeClass.ImplementedInterfaces;			
			
			Assert.AreEqual(0, codeElements.Count);
		}
	}
}
