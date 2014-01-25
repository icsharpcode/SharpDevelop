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
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.MSTest;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using Rhino.Mocks;

namespace MSTest.SharpDevelop.Tests
{
	[TestFixture]
	public class MSTestFrameworkTests
	{
		MSTestFramework testFramework;
		IProject fakeProject;
		
		[SetUp]
		public void Init()
		{
			testFramework = new MSTestFramework();
			
			fakeProject = MockRepository.GenerateStub<IProject>();
			fakeProject.Stub(p => p.SyncRoot).Return(fakeProject);
		}
		
		void AddReferencesToProject(params string[] referenceNames)
		{
			List<ProjectItem> projectItems = referenceNames
				.Select(name => new ReferenceProjectItem(fakeProject, name) as ProjectItem)
				.ToList();
			
			AddItemsToProject(projectItems);
		}
		
		void AddItemsToProject(List<ProjectItem> projectItems)
		{
			fakeProject
				.Stub(project => project.Items)
				.Return(new SimpleModelCollection<ProjectItem>(projectItems));
		}
		
		void AddFileAndReferenceToProject(string fileName, string reference)
		{
			var projectItems = new List<ProjectItem>();
			projectItems.Add(new FileProjectItem(fakeProject, ItemType.Compile, fileName));
			projectItems.Add(new ReferenceProjectItem(fakeProject, reference));
			
			AddItemsToProject(projectItems);
		}
		
		void NoItemsInProject()
		{
			AddReferencesToProject();
		}
		
//		IClass CreateClassWithoutAnyAttributes()
//		{
//			IClass fakeClass = MockRepository.GenerateStub<IClass>();
//			AddAttributesToClass(fakeClass, new List<IAttribute>());
//			return fakeClass;
//		}
//		
//		void AddAttributesToClass(IClass fakeClass, List<IAttribute> attributes)
//		{
//			fakeClass.Stub(c => c.Attributes).Return(attributes);
//		}
//		
//		IClass CreateClassWithAttributes(params string[] attributeNames)
//		{
//			IClass fakeClass = MockRepository.GenerateStub<IClass>();
//			
//			List<IAttribute> attributes = CreateAttributes(attributeNames);
//			
//			AddAttributesToClass(fakeClass, attributes);
//			
//			return fakeClass;
//		}
//		
//		List<IAttribute> CreateAttributes(params string[] attributeNames)
//		{
//			return attributeNames.Select(name => CreateAttribute(name)).ToList();
//		}
//		
//		IAttribute CreateAttribute(string name)
//		{
//			IReturnType returnType = MockRepository.GenerateStub<IReturnType>();
//			returnType.Stub(t => t.FullyQualifiedName).Return(name);
//			
//			IAttribute attribute = MockRepository.GenerateStub<IAttribute>();
//			attribute.Stub(a => a.AttributeType).Return(returnType);
//			return attribute;
//		}
//		
//		void MakeClassAbstract(IClass fakeClass)
//		{
//			fakeClass.Stub(c => c.IsAbstract).Return(true);
//		}
//		
//		IMethod CreateMethodWithoutAnyAttributes()
//		{
//			IMethod fakeMethod = MockRepository.GenerateStub<IMethod>();
//			AddAttributesToMethod(fakeMethod, new List<IAttribute>());
//			return fakeMethod;
//		}
//		
//		IMethod CreateMethodWithAttributes(params string[] attributeNames)
//		{
//			IMethod fakeMethod = MockRepository.GenerateStub<IMethod>();
//			List<IAttribute> attributes = CreateAttributes(attributeNames);
//			AddAttributesToMethod(fakeMethod, attributes);
//			return fakeMethod;
//		}
//		
//		void AddAttributesToMethod(IMethod method, List<IAttribute> attributes)
//		{
//			method.Stub(m => m.Attributes).Return(attributes);
//		}
//		
//		List<TestMember> GetTestMembersFor(IClass fakeClass)
//		{
//			return testFramework.GetTestMembersFor(fakeClass).ToList();
//		}
//		
//		void AddMethodsToClass(IClass fakeClass, List<IMethod> methods)
//		{
//			fakeClass.Stub(c => c.Methods).Return(methods);
//		}
		
		[Test]
		public void IsTestProject_NullProject_ReturnsFalse()
		{
			bool result = testFramework.IsTestProject(null);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestProject_ProjectWithMSTestAssemblyReference_ReturnsTrue()
		{
			AddReferencesToProject("System", "Microsoft.VisualStudio.QualityTools.UnitTestFramework");
			
			bool result = testFramework.IsTestProject(fakeProject);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestProject_ProjectWithoutMSTestAssemblyReference_ReturnsFalse()
		{
			NoItemsInProject();
			bool result = testFramework.IsTestProject(fakeProject);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestProject_ProjectWithMSTestAssemblyReferenceInUpperCase_ReturnsTrue()
		{
			AddReferencesToProject("MICROSOFT.VISUALSTUDIO.QUALITYTOOLS.UNITTESTFRAMEWORK");
			
			bool result = testFramework.IsTestProject(fakeProject);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestProject_ProjectWithMSTestAssemblyReferenceAndFileProjectItem_ReturnsTrue()
		{
			AddFileAndReferenceToProject("test.cs", "Microsoft.VisualStudio.QualityTools.UnitTestFramework");

			bool result = testFramework.IsTestProject(fakeProject);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestProject_ProjectWithMSTestAssemblyReferenceUsingFullName_ReturnsTrue()
		{
			AddReferencesToProject("Microsoft.VisualStudio.QualityTools.UnitTestFramework, Version=2.5.3.9345, Culture=neutral, PublicKeyToken=96d09a1eb7f44a77");
			
			bool result = testFramework.IsTestProject(fakeProject);
			
			Assert.IsTrue(result);
		}
	}
}
