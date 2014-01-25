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
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcModelClassLocatorTests : MvcTestsBase
	{
		MvcModelClassLocator locator;
		FakeMvcProject fakeProject;
		FakeMvcParserService fakeParserService;
		
		void CreateLocator()
		{
			fakeProject = new FakeMvcProject();
			fakeParserService = new FakeMvcParserService();
			locator = new MvcModelClassLocator(fakeParserService);
		}
		
		List<IMvcClass> GetModelClasses()
		{
			return locator.GetModelClasses(fakeProject).ToList();
		}
		
		FakeMvcClass AddModelClass(string className)
		{
			return fakeParserService.AddModelClassToProjectContent(className);
		}
		
		FakeMvcClass AddModelClassWithBaseClass(string baseClassName, string className)
		{
			return fakeParserService.AddModelClassWithBaseClassToProjectContent(baseClassName, className);
		}
		
		string GetFirstModelClassName()
		{
			return GetModelClasses().First().FullName;
		}
		
		int GetModelClassCount()
		{
			return GetModelClasses().Count;
		}
		
		void UseVisualBasicProject()
		{
			fakeProject.SetVisualBasicAsTemplateLanguage();
		}
		
		void UseCSharpProject()
		{
			fakeProject.SetCSharpAsTemplateLanguage();
		}
		
		[Test]
		public void GetModelClasses_OneModelClassInProject_ReturnModelClassWithExpectedName()
		{
			CreateLocator();
			AddModelClass("MyNamespace.MyClass");
			string modelClassName = GetFirstModelClassName();
				
			Assert.AreEqual("MyNamespace.MyClass", modelClassName);
		}
		
		[Test]
		public void GetModelClasses_OneModelClassInProject_ReturnOneModelClass()
		{
			CreateLocator();
			AddModelClass("MyNamespace.MyClass");
			int count = GetModelClassCount();
				
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void GetModelClasses_NoModelClassesInProject_GetsProjectContentFromParserService()
		{
			CreateLocator();
			GetModelClasses();
			
			Assert.AreEqual(fakeProject, fakeParserService.ProjectPassedToGetProjectContent);
		}
		
		[Test]
		public void GetModelClasses_OneClassInProjectIsNotModelClass_ReturnNoModelClasses()
		{
			CreateLocator();
			FakeMvcClass fakeClass = AddModelClass("MyNamespace.MyClass");
			fakeClass.SetIsNotModelClass();
			int count = GetModelClassCount();
				
			Assert.AreEqual(0, count);
		}
	}
}
