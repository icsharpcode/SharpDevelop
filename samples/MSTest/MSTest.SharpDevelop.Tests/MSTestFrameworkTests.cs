// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
				.Return(new ReadOnlyCollection<ProjectItem>(projectItems));
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
		
		IClass CreateClassWithoutAnyAttributes()
		{
			IClass fakeClass = MockRepository.GenerateStub<IClass>();
			AddAttributesToClass(fakeClass, new List<IAttribute>());
			return fakeClass;
		}
		
		void AddAttributesToClass(IClass fakeClass, List<IAttribute> attributes)
		{
			fakeClass.Stub(c => c.Attributes).Return(attributes);
		}
		
		IClass CreateClassWithAttributes(params string[] attributeNames)
		{
			IClass fakeClass = MockRepository.GenerateStub<IClass>();
			
			List<IAttribute> attributes = CreateAttributes(attributeNames);
			
			AddAttributesToClass(fakeClass, attributes);
			
			return fakeClass;
		}
		
		List<IAttribute> CreateAttributes(params string[] attributeNames)
		{
			return attributeNames.Select(name => CreateAttribute(name)).ToList();
		}
		
		IAttribute CreateAttribute(string name)
		{
			IReturnType returnType = MockRepository.GenerateStub<IReturnType>();
			returnType.Stub(t => t.FullyQualifiedName).Return(name);
			
			IAttribute attribute = MockRepository.GenerateStub<IAttribute>();
			attribute.Stub(a => a.AttributeType).Return(returnType);
			return attribute;
		}
		
		void MakeClassAbstract(IClass fakeClass)
		{
			fakeClass.Stub(c => c.IsAbstract).Return(true);
		}
		
		IMethod CreateMethodWithoutAnyAttributes()
		{
			IMethod fakeMethod = MockRepository.GenerateStub<IMethod>();
			AddAttributesToMethod(fakeMethod, new List<IAttribute>());
			return fakeMethod;
		}
		
		IMethod CreateMethodWithAttributes(params string[] attributeNames)
		{
			IMethod fakeMethod = MockRepository.GenerateStub<IMethod>();
			List<IAttribute> attributes = CreateAttributes(attributeNames);
			AddAttributesToMethod(fakeMethod, attributes);
			return fakeMethod;
		}
		
		void AddAttributesToMethod(IMethod method, List<IAttribute> attributes)
		{
			method.Stub(m => m.Attributes).Return(attributes);
		}
		
		List<TestMember> GetTestMembersFor(IClass fakeClass)
		{
			return testFramework.GetTestMembersFor(fakeClass).ToList();
		}
		
		void AddMethodsToClass(IClass fakeClass, List<IMethod> methods)
		{
			fakeClass.Stub(c => c.Methods).Return(methods);
		}
		
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
		
		[Test]
		public void IsTestClass_ClassHasNoAttributes_ReturnsFalse()
		{
			IClass fakeClass = CreateClassWithoutAnyAttributes();
			
			bool result = testFramework.IsTestClass(fakeClass);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestClass_ClassHasTestFixtureAttributeMissingAttributePart_ReturnsTrue()
		{
			IClass fakeClass = CreateClassWithAttributes("TestClass");
			
			bool result = testFramework.IsTestClass(fakeClass);
			
			Assert.IsTrue(result);
		}
				
		[Test]
		public void IsTestClass_ClassHasTestClassAttributeAndIsAbstract_ReturnsFalse()
		{
			IClass fakeClass = CreateClassWithAttributes("TestClass");
			MakeClassAbstract(fakeClass);
			
			bool result = testFramework.IsTestClass(fakeClass);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestClass_ClassHasTestClassAttributeIncludingAttributePart_ReturnsTrue()
		{
			IClass fakeClass = CreateClassWithAttributes("TestClassAttribute");
			
			bool result = testFramework.IsTestClass(fakeClass);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestClass_ClassHasFullyQualifiedMSTestClassAttribute_ReturnsTrue()
		{
			IClass fakeClass = CreateClassWithAttributes("Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute");
			
			bool result = testFramework.IsTestClass(fakeClass);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestClass_ClassIsNull_ReturnsFalse()
		{
			bool result = testFramework.IsTestClass(null);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestMember_MethodHasNoAttributes_ReturnsFalse()
		{
			IMethod method = CreateMethodWithoutAnyAttributes();
			
			bool result = testFramework.IsTestMember(method);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestMember_MethodHasTestMethodAttributeWithoutAttributePart_ReturnsTrue()
		{
			IMethod method = CreateMethodWithAttributes("TestMethod");
			
			bool result = testFramework.IsTestMember(method);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestMember_MethodHasTestMethodAttributeAttribute_ReturnsTrue()
		{
			IMethod method = CreateMethodWithAttributes("TestMethodAttribute");
			
			bool result = testFramework.IsTestMember(method);
			
			Assert.IsTrue(result);
		}

		[Test]
		public void IsTestMember_MethodHasFullyQualifiedMSTestTestMethodAttribute_ReturnsTrue()
		{
			IMethod method = CreateMethodWithAttributes("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute");
			
			bool result = testFramework.IsTestMember(method);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestMember_MethodIsNull_ReturnsFalse()
		{
			bool result = testFramework.IsTestMember(null);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestMember_MemberNotMethod_ReturnsFalse()
		{
			IMember member = MockRepository.GenerateStub<IMember>();
			
			bool result = testFramework.IsTestMember(member);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void GetTestMembersFor_ClassHasNoMethods_ReturnsFalse()
		{
			IClass fakeClass = CreateClassWithAttributes("TestClass");
			AddMethodsToClass(fakeClass, new List<IMethod>());
			
			List<TestMember> testMembers = GetTestMembersFor(fakeClass);
			
			Assert.AreEqual(0, testMembers.Count);
		}
	
		[Test]
		public void GetTestMembersFor_ClassHasTwoMethodsAndSecondOneIsTestMethod_ReturnsSecondTestMethodOnly()
		{
			IClass fakeClass = CreateClassWithAttributes("TestClass");
			
			var methods = new List<IMethod>();
			methods.Add(CreateMethodWithoutAnyAttributes());
			IMethod testMethod = CreateMethodWithAttributes("TestMethod");
			methods.Add(testMethod);
			AddMethodsToClass(fakeClass, methods);
			
			List<TestMember> testMembers = GetTestMembersFor(fakeClass);
			
			Assert.AreEqual(1, testMembers.Count);
			Assert.AreEqual(testMethod, testMembers[0].Member);
		}
	}
}
