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
		ClassHelper helper;
		
		void CreateProjectContent()
		{
			helper = new ClassHelper();
		}
		
		void CreateClass(string name)
		{
			helper.CreateClass(name);
			CreateClass();
		}
		
		void CreatePublicClass(string name)
		{
			helper.CreatePublicClass(name);
			CreateClass();
		}
		
		void CreatePrivateClass(string name)
		{
			helper.CreatePrivateClass(name);
			CreateClass();
		}
		
		void CreateClass()
		{
			codeClass = new CodeClass2(helper.ProjectContentHelper.FakeProjectContent, helper.Class);
		}
		
		void AddInterfaceToProjectContent(string fullName)
		{
			helper.ProjectContentHelper.AddInterfaceToProjectContent(fullName);
		}
		
		void AddClassToProjectContent(string fullName)
		{
			helper.CreateClass(fullName);
		}
		
		void AddInterfaceToClassBaseTypes(string fullName, string dotNetName)
		{
			helper.AddInterfaceToClassBaseTypes(fullName, dotNetName);
		}
		
		void AddClassToClassBaseTypes(string fullName)
		{
			helper.AddClassToClassBaseTypes(fullName);
		}
		
		void AddBaseTypeToClass(string fullName)
		{
			helper.AddBaseTypeToClass(fullName);
		}
		
		void AddMethodToClass(string fullyQualifiedName)
		{
			helper.AddMethodToClass(fullyQualifiedName);
		}
		
		void AddPropertyToClass(string fullyQualifiedName)
		{
			helper.AddPropertyToClass(fullyQualifiedName);
		}
		
		void AddFieldToClass(string fullyQualifiedName)
		{
			helper.AddFieldToClass(fullyQualifiedName);
		}
		
		[Test]
		public void Language_CSharpProject_ReturnsCSharpModelLanguage()
		{
			CreateProjectContent();
			helper.ProjectContentHelper.ProjectContentIsForCSharpProject();
			CreateClass("MyClass");
			
			string language = codeClass.Language;
			
			Assert.AreEqual(CodeModelLanguageConstants.vsCMLanguageCSharp, language);
		}
		
		[Test]
		public void Language_VisualBasicProject_ReturnsVisualBasicModelLanguage()
		{
			CreateProjectContent();
			helper.ProjectContentHelper.ProjectContentIsForVisualBasicProject();
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
			AddInterfaceToClassBaseTypes("System.Collections.Generic.ICollection", "System.Collections.Generic.ICollection{System.String}");
			
			CodeElements codeElements = codeClass.ImplementedInterfaces;
			CodeInterface codeInterface = codeElements.FirstCodeInterfaceOrDefault();
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("System.Collections.Generic.ICollection<System.String>", codeInterface.FullName);
		}
		
		[Test]
		public void ImplementedInterfaces_ClassHasBaseTypeButNoInterfaces_ReturnsNoItems()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			AddClassToClassBaseTypes("MyNamespace.MyBaseClass");
			
			CodeElements codeElements = codeClass.ImplementedInterfaces;			
			
			Assert.AreEqual(0, codeElements.Count);
		}
		
		[Test]
		public void BaseTypes_ClassBaseTypeIsSystemObject_ReturnsSystemObject()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			AddBaseTypeToClass("System.Object");
			
			CodeElements codeElements = codeClass.Bases;
			CodeClass2 baseClass = codeElements.FirstCodeClass2OrDefault();
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("System.Object", baseClass.FullName);
			Assert.AreEqual("Object", baseClass.Name);
		}
		
		[Test]
		public void Members_ClassHasOneMethod_ReturnsOneMethod()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			AddMethodToClass("MyClass.MyMethod");
			
			CodeElements codeElements = codeClass.Members;
			CodeFunction codeFunction = codeElements.FirstCodeFunctionOrDefault();
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("MyMethod", codeFunction.Name);
		}
		
		[Test]
		public void Members_ClassHasOneProperty_ReturnsOneProperty()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			AddPropertyToClass("MyClass.MyProperty");
			
			CodeElements codeElements = codeClass.Members;
			CodeProperty2 codeFunction = codeElements.FirstCodeProperty2OrDefault();
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("MyProperty", codeFunction.Name);
		}
		
		[Test]
		public void Members_ClassHasOneField_ReturnsOneField()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			AddFieldToClass("MyClass.MyField");
			
			CodeElements codeElements = codeClass.Members;
			CodeVariable codeVariable = codeElements.FirstCodeVariableOrDefault();
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("MyField", codeVariable.Name);
		}
		
		[Test]
		public void Kind_PublicClass_ReturnsClass()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			
			vsCMElement kind = codeClass.Kind;
			
			Assert.AreEqual(vsCMElement.vsCMElementClass, kind);
		}
	}
}
