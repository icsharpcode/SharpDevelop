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
		FakeClass fakeClass;
		
		void CreateClass(string name)
		{
			fakeClass = new FakeClass(name);
			mvcClass = new MvcClass(fakeClass);
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
	}
}
