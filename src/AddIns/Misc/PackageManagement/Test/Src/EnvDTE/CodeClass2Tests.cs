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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeClass2Tests : CodeModelTestBase
	{
		CodeClass2 codeClass;
		
		void CreateClass(string code)
		{
			AddCodeFile("class.cs", code);
			ITypeDefinitionModel typeModel = assemblyModel.TopLevelTypeDefinitions.Single();
			codeClass = new CodeClass2(codeModelContext, typeModel);
		}
		
		[Test]
		public void Language_CSharpProject_ReturnsCSharpModelLanguage()
		{
			CreateClass("class MyClass {}");
			
			string language = codeClass.Language;
			
			Assert.AreEqual(global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp, language);
		}
		
		[Test]
		public void Language_VisualBasicProject_ReturnsVisualBasicModelLanguage()
		{
			projectLanguage = "VB";
			CreateClass("class MyClass {}");
			
			string language = codeClass.Language;
			
			Assert.AreEqual(global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB, language);
		}
		
		[Test]
		public void Access_PublicClass_ReturnsPublic()
		{
			CreateClass("public class MyClass {}");
			
			global::EnvDTE.vsCMAccess access = codeClass.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_InternalClass_ReturnsPrivate()
		{
			CreateClass("class MyClass {}");
			
			global::EnvDTE.vsCMAccess access = codeClass.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
		}
		
//		[Test]
//		public void ImplementedInterfaces_ClassImplementsGenericICollectionOfString_ReturnsCodeInterfaceForICollection()
//		{
//			CreateClass("using System.Collection.Generic;" +
//			            "class MyClass : ICollection<string> {}");
//			
//			global::EnvDTE.CodeElements codeElements = codeClass.ImplementedInterfaces;
//			CodeInterface codeInterface = codeElements.FirstCodeInterfaceOrDefault();
//			
//			Assert.AreEqual(1, codeElements.Count);
//			Assert.AreEqual("System.Collections.Generic.ICollection<System.String>", codeInterface.FullName);
//		}
//		
//		[Test]
//		public void ImplementedInterfaces_ClassHasBaseTypeButNoInterfaces_ReturnsNoItems()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			AddClassToClassBaseTypes("MyNamespace.MyBaseClass");
//			
//			global::EnvDTE.CodeElements codeElements = codeClass.ImplementedInterfaces;
//			
//			Assert.AreEqual(0, codeElements.Count);
//		}
//		
//		[Test]
//		public void BaseTypes_ClassBaseTypeIsSystemObject_ReturnsSystemObject()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			AddBaseTypeToClass("System.Object");
//			
//			global::EnvDTE.CodeElements codeElements = codeClass.Bases;
//			CodeClass2 baseClass = codeElements.FirstCodeClass2OrDefault();
//			
//			Assert.AreEqual(1, codeElements.Count);
//			Assert.AreEqual("System.Object", baseClass.FullName);
//			Assert.AreEqual("Object", baseClass.Name);
//		}
//		
//		[Test]
//		public void BaseTypes_ClassBaseTypeIsNull_ReturnsNoCodeElements()
//		{
//			CreateProjectContent();
//			CreatePublicClass("System.Object");
//			
//			global::EnvDTE.CodeElements codeElements = codeClass.Bases;
//			
//			Assert.AreEqual(0, codeElements.Count);
//		}
//		
//		[Test]
//		public void Members_ClassHasOneMethod_ReturnsOneMethod()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			AddMethodToClass("MyClass.MyMethod");
//			
//			global::EnvDTE.CodeElements codeElements = codeClass.Members;
//			CodeFunction2 codeFunction = codeElements.FirstCodeFunction2OrDefault();
//			
//			Assert.AreEqual(1, codeElements.Count);
//			Assert.AreEqual("MyMethod", codeFunction.Name);
//		}
//		
//		[Test]
//		public void Members_ClassHasOneProperty_ReturnsOneProperty()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			AddPropertyToClass("MyClass.MyProperty");
//			
//			global::EnvDTE.CodeElements codeElements = codeClass.Members;
//			CodeProperty2 codeFunction = codeElements.FirstCodeProperty2OrDefault();
//			
//			Assert.AreEqual(1, codeElements.Count);
//			Assert.AreEqual("MyProperty", codeFunction.Name);
//		}
//		
//		[Test]
//		public void Members_ClassHasOneField_ReturnsOneField()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			AddFieldToClass("MyClass.MyField");
//			
//			global::EnvDTE.CodeElements codeElements = codeClass.Members;
//			CodeVariable codeVariable = codeElements.FirstCodeVariableOrDefault();
//			
//			Assert.AreEqual(1, codeElements.Count);
//			Assert.AreEqual("MyField", codeVariable.Name);
//		}
//		
//		[Test]
//		public void Kind_PublicClass_ReturnsClass()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			
//			global::EnvDTE.vsCMElement kind = codeClass.Kind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementClass, kind);
//		}
//		
//		[Test]
//		public void Namespace_PublicClass_ReturnsClassNamespace()
//		{
//			CreateProjectContent();
//			helper.CreatePublicClass("MyNamespace.Test.MyClass");
//			helper.AddClassNamespace("MyNamespace.Test");
//			CreateClass();
//			
//			global::EnvDTE.CodeNamespace codeNamespace = codeClass.Namespace;
//			
//			Assert.AreEqual("MyNamespace.Test", codeNamespace.FullName);
//		}
//		
//		[Test]
//		public void Namespace_PublicClassAndNamespaceNameChecked_ReturnsFullyQualifiedClassNamespace()
//		{
//			CreateProjectContent();
//			helper.CreatePublicClass("MyNamespace.Test.MyClass");
//			helper.AddClassNamespace("MyNamespace.Test");
//			CreateClass();
//			
//			global::EnvDTE.CodeNamespace codeNamespace = codeClass.Namespace;
//			
//			Assert.AreEqual("MyNamespace.Test", codeNamespace.Name);
//		}
//		
//		[Test]
//		public void PartialClasses_ClassIsNotPartial_ReturnsClass()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyNamespace.MyClass");
//			CreateClass();
//			
//			global::EnvDTE.CodeElements partialClasses = codeClass.PartialClasses;
//			CodeClass firstClass = partialClasses.FirstCodeClass2OrDefault();
//			
//			Assert.AreEqual(1, partialClasses.Count);
//			Assert.AreEqual(codeClass, firstClass);
//		}
//		
//		[Test]
//		public void Members_GetFirstPropertyTwice_PropertiesAreConsideredEqualWhenAddedToList()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			helper.AddPropertyToClass("MyClass.MyProperty");
//			CodeProperty2 property = codeClass.Members.FirstCodeProperty2OrDefault();
//			var properties = new List<CodeProperty2>();
//			properties.Add(property);
//			
//			CodeProperty2 property2 = codeClass.Members.FirstCodeProperty2OrDefault();
//			
//			bool contains = properties.Contains(property2);
//			Assert.IsTrue(contains);
//		}
//		
//		[Test]
//		public void IsAbstract_ClassIsAbstract_ReturnsTrue()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			ClassIsAbstract();
//			
//			bool isAbstract = codeClass.IsAbstract;
//			
//			Assert.IsTrue(isAbstract);
//		}
//		
//		[Test]
//		public void IsAbstract_ClassIsNotAbstract_ReturnsFalse()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			
//			bool isAbstract = codeClass.IsAbstract;
//			
//			Assert.IsFalse(isAbstract);
//		}
//		
//		[Test]
//		public void ClassKind_ClassIsPartial_ReturnsPartialClassKind()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			ClassIsPartial();
//			
//			global::EnvDTE.vsCMClassKind kind = codeClass.ClassKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMClassKind.vsCMClassKindPartialClass, kind);
//		}
//		
//		[Test]
//		public void ClassKind_ClassIsNotPartial_ReturnsMainClassKind()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			
//			global::EnvDTE.vsCMClassKind kind = codeClass.ClassKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMClassKind.vsCMClassKindMainClass, kind);
//		}
//		
//		[Test]
//		public void IsGeneric_ClassIsGeneric_ReturnsTrue()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			ClassIsGeneric();
//			
//			bool generic = codeClass.IsGeneric;
//			
//			Assert.IsTrue(generic);
//		}
//		
//		[Test]
//		public void IsGeneric_ClassIsNotGeneric_ReturnsFalse()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			ClassIsNotGeneric();
//			
//			bool generic = codeClass.IsGeneric;
//			
//			Assert.IsFalse(generic);
//		}
//		
//		[Test]
//		public void ClassKind_ChangeClassToBePartial_UsesClassKindUpdaterToModifyClass()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			
//			codeClass.ClassKind = global::EnvDTE.vsCMClassKind.vsCMClassKindPartialClass;
//			
//			classKindUpdater.AssertWasCalled(updater => updater.MakeClassPartial());
//		}
//		
//		[Test]
//		public void ClassKind_ChangeClassToBeMainClass_ThrowsNotImplementedException()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			
//			Assert.Throws<NotImplementedException>(() => codeClass.ClassKind = global::EnvDTE.vsCMClassKind.vsCMClassKindMainClass);
//		}
	}
}
