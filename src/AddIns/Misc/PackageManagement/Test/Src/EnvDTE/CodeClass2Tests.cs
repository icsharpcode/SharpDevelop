// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
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
		IClassKindUpdater classKindUpdater;
		
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
			classKindUpdater = MockRepository.GenerateStub<IClassKindUpdater>();
			codeClass = new CodeClass2(helper.ProjectContentHelper.ProjectContent, helper.Class, classKindUpdater);
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
		
		void ClassIsAbstract()
		{
			helper.MakeClassAbstract();
		}
		
		void ClassIsPartial()
		{
			helper.MakeClassPartial();
		}
		
		void ClassIsGeneric()
		{
			helper.SetDotNetName("MyClass`1");
		}
		
		void ClassIsNotGeneric()
		{
			helper.SetDotNetName("MyClass");
		}
		
		[Test]
		public void Language_CSharpProject_ReturnsCSharpModelLanguage()
		{
			CreateProjectContent();
			helper.ProjectContentHelper.ProjectContentIsForCSharpProject();
			CreateClass("MyClass");
			
			string language = codeClass.Language;
			
			Assert.AreEqual(global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp, language);
		}
		
		[Test]
		public void Language_VisualBasicProject_ReturnsVisualBasicModelLanguage()
		{
			CreateProjectContent();
			helper.ProjectContentHelper.ProjectContentIsForVisualBasicProject();
			CreateClass("MyClass");
			
			string language = codeClass.Language;
			
			Assert.AreEqual(global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB, language);
		}
		
		[Test]
		public void Access_PublicClass_ReturnsPublic()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			
			global::EnvDTE.vsCMAccess access = codeClass.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_PrivateClass_ReturnsPrivate()
		{
			CreateProjectContent();
			CreatePrivateClass("MyClass");
			
			global::EnvDTE.vsCMAccess access = codeClass.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void ImplementedInterfaces_ClassImplementsGenericICollectionOfString_ReturnsCodeInterfaceForICollection()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			AddInterfaceToClassBaseTypes("System.Collections.Generic.ICollection", "System.Collections.Generic.ICollection{System.String}");
			
			global::EnvDTE.CodeElements codeElements = codeClass.ImplementedInterfaces;
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
			
			global::EnvDTE.CodeElements codeElements = codeClass.ImplementedInterfaces;
			
			Assert.AreEqual(0, codeElements.Count);
		}
		
		[Test]
		public void BaseTypes_ClassBaseTypeIsSystemObject_ReturnsSystemObject()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			AddBaseTypeToClass("System.Object");
			
			global::EnvDTE.CodeElements codeElements = codeClass.Bases;
			CodeClass2 baseClass = codeElements.FirstCodeClass2OrDefault();
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("System.Object", baseClass.FullName);
			Assert.AreEqual("Object", baseClass.Name);
		}
		
		[Test]
		public void BaseTypes_ClassBaseTypeIsNull_ReturnsNoCodeElements()
		{
			CreateProjectContent();
			CreatePublicClass("System.Object");
			
			global::EnvDTE.CodeElements codeElements = codeClass.Bases;
			
			Assert.AreEqual(0, codeElements.Count);
		}
		
		[Test]
		public void Members_ClassHasOneMethod_ReturnsOneMethod()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			AddMethodToClass("MyClass.MyMethod");
			
			global::EnvDTE.CodeElements codeElements = codeClass.Members;
			CodeFunction2 codeFunction = codeElements.FirstCodeFunction2OrDefault();
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("MyMethod", codeFunction.Name);
		}
		
		[Test]
		public void Members_ClassHasOneProperty_ReturnsOneProperty()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			AddPropertyToClass("MyClass.MyProperty");
			
			global::EnvDTE.CodeElements codeElements = codeClass.Members;
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
			
			global::EnvDTE.CodeElements codeElements = codeClass.Members;
			CodeVariable codeVariable = codeElements.FirstCodeVariableOrDefault();
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("MyField", codeVariable.Name);
		}
		
		[Test]
		public void Kind_PublicClass_ReturnsClass()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			
			global::EnvDTE.vsCMElement kind = codeClass.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementClass, kind);
		}
		
		[Test]
		public void Namespace_PublicClass_ReturnsClassNamespace()
		{
			CreateProjectContent();
			helper.CreatePublicClass("MyNamespace.Test.MyClass");
			helper.AddClassNamespace("MyNamespace.Test");
			CreateClass();
			
			global::EnvDTE.CodeNamespace codeNamespace = codeClass.Namespace;
			
			Assert.AreEqual("MyNamespace.Test", codeNamespace.FullName);
		}
		
		[Test]
		public void Namespace_PublicClassAndNamespaceNameChecked_ReturnsFullyQualifiedClassNamespace()
		{
			CreateProjectContent();
			helper.CreatePublicClass("MyNamespace.Test.MyClass");
			helper.AddClassNamespace("MyNamespace.Test");
			CreateClass();
			
			global::EnvDTE.CodeNamespace codeNamespace = codeClass.Namespace;
			
			Assert.AreEqual("MyNamespace.Test", codeNamespace.Name);
		}
		
		[Test]
		public void PartialClasses_ClassIsNotPartial_ReturnsClass()
		{
			CreateProjectContent();
			CreatePublicClass("MyNamespace.MyClass");
			CreateClass();
			
			global::EnvDTE.CodeElements partialClasses = codeClass.PartialClasses;
			CodeClass firstClass = partialClasses.FirstCodeClass2OrDefault();
			
			Assert.AreEqual(1, partialClasses.Count);
			Assert.AreEqual(codeClass, firstClass);
		}
		
		[Test]
		public void Members_GetFirstPropertyTwice_PropertiesAreConsideredEqualWhenAddedToList()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			helper.AddPropertyToClass("MyClass.MyProperty");
			CodeProperty2 property = codeClass.Members.FirstCodeProperty2OrDefault();
			var properties = new List<CodeProperty2>();
			properties.Add(property);
			
			CodeProperty2 property2 = codeClass.Members.FirstCodeProperty2OrDefault();
			
			bool contains = properties.Contains(property2);
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void IsAbstract_ClassIsAbstract_ReturnsTrue()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			ClassIsAbstract();
			
			bool isAbstract = codeClass.IsAbstract;
			
			Assert.IsTrue(isAbstract);
		}
		
		[Test]
		public void IsAbstract_ClassIsNotAbstract_ReturnsFalse()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			
			bool isAbstract = codeClass.IsAbstract;
			
			Assert.IsFalse(isAbstract);
		}
		
		[Test]
		public void ClassKind_ClassIsPartial_ReturnsPartialClassKind()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			ClassIsPartial();
			
			global::EnvDTE.vsCMClassKind kind = codeClass.ClassKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMClassKind.vsCMClassKindPartialClass, kind);
		}
		
		[Test]
		public void ClassKind_ClassIsNotPartial_ReturnsMainClassKind()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			
			global::EnvDTE.vsCMClassKind kind = codeClass.ClassKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMClassKind.vsCMClassKindMainClass, kind);
		}
		
		[Test]
		public void IsGeneric_ClassIsGeneric_ReturnsTrue()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			ClassIsGeneric();
			
			bool generic = codeClass.IsGeneric;
			
			Assert.IsTrue(generic);
		}
		
		[Test]
		public void IsGeneric_ClassIsNotGeneric_ReturnsFalse()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			ClassIsNotGeneric();
			
			bool generic = codeClass.IsGeneric;
			
			Assert.IsFalse(generic);
		}
		
		[Test]
		public void ClassKind_ChangeClassToBePartial_UsesClassKindUpdaterToModifyClass()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			
			codeClass.ClassKind = global::EnvDTE.vsCMClassKind.vsCMClassKindPartialClass;
			
			classKindUpdater.AssertWasCalled(updater => updater.MakeClassPartial());
		}
		
		[Test]
		public void ClassKind_ChangeClassToBeMainClass_ThrowsNotImplementedException()
		{
			CreateProjectContent();
			CreatePublicClass("MyClass");
			
			Assert.Throws<NotImplementedException>(() => codeClass.ClassKind = global::EnvDTE.vsCMClassKind.vsCMClassKindMainClass);
		}
	}
}
