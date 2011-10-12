// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcClassTests
	{
		MvcClass mvcClass;
		FakeMvcProject fakeProject;
		FakeClass fakeClass;
		
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
			fakeClass = new FakeClass(name);
			mvcClass = new MvcClass(fakeClass, fakeProject);
		}
		
		void CreateClassWithVisualBasicProject(string name)
		{
			CreateVisualBasicProject();
			CreateClass(name, fakeProject);
		}
		
		void AddBaseClass(string name)
		{
			fakeClass.AddBaseClass(name);
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
			string @namespace = mvcClass.Namespace;
			
			Assert.AreEqual("ICSharpCode", @namespace);
		}
		
		[Test]
		public void Name_ClassIsICSharpCodeTestClass_ReturnsTestClass()
		{
			CreateClass("ICSharpCode.TestClass");
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
			fakeClass.TestableProject.SetOutputAssemblyFullPath(expectedOutputAssemblyLocation);
			
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
