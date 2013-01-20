// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using Rhino.Mocks;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcClassTests : MvcTestsBase
	{
		MvcClass mvcClass;
		FakeMvcProject fakeProject;
		ITypeDefinition fakeTypeDefinition;
		List<IType> baseTypes;
		
		void CreateCSharpProject()
		{
			fakeProject = new FakeMvcProject();
			fakeProject.SetCSharpAsTemplateLanguage();
		}
		
		void CreateVisualBasicProject()
		{
			fakeProject = new FakeMvcProject();
			fakeProject.SetVisualBasicAsTemplateLanguage();
		}
		
		void CreateClass(string name)
		{
			CreateClassWithCSharpProject(name);
		}
		
		void CreateClassWithCSharpProject(string name)
		{
			CreateCSharpProject();
			CreateClass(name, fakeProject);
		}
		
		void CreateClass(string name, FakeMvcProject fakeProject)
		{
			fakeTypeDefinition = CreateFakeTypeDefinition(name);
			mvcClass = new MvcClass(fakeTypeDefinition, fakeProject);
		}
		
		ITypeDefinition CreateFakeTypeDefinition(string name)
		{
			var typeDefinition = MockRepository.GenerateStub<ITypeDefinition>();
			typeDefinition.Stub(t => t.FullName).Return(name);
			baseTypes = new List<IType>();
			typeDefinition.Stub(t => t.DirectBaseTypes).Return(baseTypes);
			return typeDefinition;
		}
		
		void CreateClassWithVisualBasicProject(string name)
		{
			CreateVisualBasicProject();
			CreateClass(name, fakeProject);
		}
		
		void AddBaseClass(string name)
		{
			IType type = MockRepository.GenerateStub<IType>();
			type.Stub(t => t.FullName).Return(name);
			type.Stub(t => t.Kind).Return(TypeKind.Class);
			baseTypes.Add(type);
		}
		
		[Test]
		public void FullName_ClassIsICSharpCodeTestClass_ReturnsICSharpCodeTestClass()
		{
			CreateClass("ICSharpCode.TestClass");
			string name = mvcClass.FullName;
			
			Assert.AreEqual("ICSharpCode.TestClass", name);
		}
		
		[Test]
		public void Namespace_ClassIsICSharpCodeTestClass_ReturnsICSharpCode()
		{
			CreateClass("ICSharpCode.TestClass");
			fakeTypeDefinition.Stub(t => t.Namespace).Return("ICSharpCode");
			string @namespace = mvcClass.Namespace;
			
			Assert.AreEqual("ICSharpCode", @namespace);
		}
		
		[Test]
		public void Name_ClassIsICSharpCodeTestClass_ReturnsTestClass()
		{
			CreateClass("ICSharpCode.TestClass");
			fakeTypeDefinition.Stub(t => t.Name).Return("TestClass");
			string name = mvcClass.Name;
			
			Assert.AreEqual("TestClass", name);
		}
		
		[Test]
		public void BaseClassFullName_ClassHasNoBaseClass_ReturnsEmptyString()
		{
			CreateClass("ICSharpCode.TestClass");
			string name = mvcClass.BaseClassFullName;
			
			Assert.AreEqual(String.Empty, name);
		}
		
		[Test]
		public void BaseClassFullName_ClassHasBaseClass_ReturnsBaseClassName()
		{
			CreateClass("ICSharpCode.TestClass");
			AddBaseClass("ICSharpCode.BaseClass");
			string name = mvcClass.BaseClassFullName;
			
			Assert.AreEqual("ICSharpCode.BaseClass", name);
		}
		
		[Test]
		public void AssemblyLocation_ProjectHasOutputAssemblyPath_ReturnsProjectOutputAssemblyPath()
		{
			CreateClass("ICSharpCode.TestClass");
			string expectedOutputAssemblyLocation = @"d:\test\bin\debug\test.dll";
			fakeProject.OutputAssemblyFullPath = expectedOutputAssemblyLocation;
			
			string assemblyLocation = mvcClass.AssemblyLocation;
			
			Assert.AreEqual(expectedOutputAssemblyLocation, assemblyLocation);
		}
		
		[Test]
		public void IsModelClass_ClassNameIsTest_ReturnsTrue()
		{
			CreateClass("Test");
			bool result = mvcClass.IsModelClass();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsModelClass_BaseClassIsMvcController_ReturnsFalse()
		{
			CreateClass("ICSharpCode.FooController");
			AddBaseClass("System.Web.Mvc.Controller");
			
			bool result = mvcClass.IsModelClass();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsModelClass_HttpApplicationDerivedClass_ReturnsFalse()
		{
			CreateClass("ICSharpCode.MvcApplication");
			AddBaseClass("System.Web.HttpApplication");
			
			bool result = mvcClass.IsModelClass();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsModelClass_VisualBasicProjectAndClassIsVisualBasicMyApplication_ReturnsFalse()
		{
			CreateClassWithVisualBasicProject("VbApp.My.MyApplication");
			bool result = mvcClass.IsModelClass();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsModelClass_VisualBasicProjectAndClassIsVisualBasicMySettings_ReturnsFalse()
		{
			CreateClassWithVisualBasicProject("TestVisualBasicApp.My.MySettings");
			bool result = mvcClass.IsModelClass();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsModelClass_CSharpProjectAndClassIsMySettings_ReturnsFalse()
		{
			CreateClassWithCSharpProject("TestApp.My.MySettings");
			bool result = mvcClass.IsModelClass();
			
			Assert.IsTrue(result);
		}
	}
}
