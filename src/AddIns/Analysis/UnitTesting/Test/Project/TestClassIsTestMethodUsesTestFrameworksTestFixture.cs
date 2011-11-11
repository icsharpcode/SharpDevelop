// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	[TestFixture]
	public class TestClassIsTestMethodUsesTestFrameworksTestFixture
	{
		TestClass testClass;
		MockClass mockTestClass;
		MockMethod testMethod;
		MockRegisteredTestFrameworks testFrameworks;
		MockClass mockBaseTestClass;
		MockMethod baseClassTestMethod;
		
		[SetUp]
		public void Init()
		{
			mockTestClass = MockClass.CreateMockClassWithoutAnyAttributes();
			mockTestClass.FullyQualifiedName = "DerivedClass";
			testMethod = new MockMethod(mockTestClass, "myTestMethod");
			mockTestClass.Methods.Add(testMethod);

			mockBaseTestClass = MockClass.CreateMockClassWithoutAnyAttributes();
			mockBaseTestClass.FullyQualifiedName = "BaseClass";
			mockBaseTestClass.Methods.Add(baseClassTestMethod);
			baseClassTestMethod = new MockMethod(mockBaseTestClass, "myBaseTestMethod");
			
			testFrameworks = new MockRegisteredTestFrameworks();
			testFrameworks.AddTestMember(testMethod);
			testFrameworks.AddTestMember(baseClassTestMethod);

			mockTestClass.AddBaseClass(mockBaseTestClass);
			
			testClass = new TestClass(mockTestClass, testFrameworks);
		}
		
		[Test]
		public void TestClassHasTestMethod()
		{
			Assert.AreEqual(testMethod, testClass.TestMembers[0].Member);
		}
		
		[Test]
		public void TestClassHasBaseClassTestMethod()
		{
			BaseTestMember baseTestMethod = testClass.TestMembers[1].Member as BaseTestMember;
			Assert.AreEqual(baseClassTestMethod, baseTestMethod.Member);
		}
	}
}
