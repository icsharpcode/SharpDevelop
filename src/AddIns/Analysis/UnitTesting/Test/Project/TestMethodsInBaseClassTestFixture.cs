// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// If a test fixture's base class has test attributes then
	/// NUnit includes the base class test methods by prefixing
	/// the base class name to them. In NUnit 2.2 this was true for
	/// both the GUI and the test result information. In 
	/// NUnit 2.4 the GUI prefixes the base class to the method
	/// name but the test result does not prefix the base class
	/// to the name of the method.
	/// 
	/// [TestFixture]
	/// public DerivedClass : BaseClass
	/// 
	/// Test method is shown in UI as:
	/// 
	/// BaseClass.BaseClassMethod
	/// 
	/// Test method name returned by NUnit-Console:
	/// 
	/// RootNamespace.DerivedClass.BaseClassMethod
	/// </summary>
	[TestFixture]
	public class TestMethodsInBaseClassTestFixture
	{
		TestClass testClass;
		MockClass c;
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void SetUp()
		{
			MockProjectContent projectContent = new MockProjectContent();
			
			// Create the base test class.
			MockClass baseClass = new MockClass(projectContent, "RootNamespace.TestFixtureBase");
			baseClass.Attributes.Add(new MockAttribute("TestFixture"));
			MockMethod baseMethod = new MockMethod(baseClass, "BaseMethod");
			baseMethod.Attributes.Add(new MockAttribute("Test"));
			baseClass.Methods.Add(baseMethod);
			
			// Create the derived test class.
			c = new MockClass(projectContent, "RootNamespace.MyTestFixture");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			MockMethod method = new MockMethod(c, "DerivedMethod");
			method.Attributes.Add(new MockAttribute("Test"));
			c.Methods.Add(method);
			projectContent.Classes.Add(c);

			// Set derived class's base class.
			c.AddBaseClass(baseClass);
			
			// Create TestClass.
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			testClass = new TestClass(c, testFrameworks);
		}
		
		[Test]
		public void TwoTestMethods()
		{
			Assert.AreEqual(2, testClass.TestMembers.Count);
		}
		
		[Test]
		public void DerivedMethod()
		{
			Assert.IsTrue(testClass.TestMembers.Contains("DerivedMethod"));
		}
		
		[Test]
		public void BaseMethod()
		{
			Assert.IsTrue(testClass.TestMembers.Contains("TestFixtureBase.BaseMethod"));
		}
		
		/// <summary>
		/// The TestMethod.Method property should return an IMethod 
		/// that returns the derived class from the DeclaringType property
		/// and not the base class. This ensures that the correct
		/// test is run when selected in the unit test tree.
		/// </summary>
		[Test]
		public void BaseMethodDeclaringTypeIsDerivedClass()
		{
			TestMember method = testClass.TestMembers["TestFixtureBase.BaseMethod"];
			Assert.AreEqual(c, method.Member.DeclaringType);
		}
		
		[Test]
		public void UpdateTestResultUsingPrefixBaseClassName()
		{
			TestClassCollection testClasses = new TestClassCollection();
			testClasses.Add(testClass);

			TestResult testResult = new TestResult("RootNamespace.MyTestFixture.TestFixtureBase.BaseMethod");
			testResult.ResultType = TestResultType.Failure;
			testClasses.UpdateTestResult(testResult);
			
			Assert.AreEqual(TestResultType.Failure, testClass.Result);
		}
		
		[Test]
		public void UpdateTestResult()
		{
			TestClassCollection testClasses = new TestClassCollection();
			testClasses.Add(testClass);

			TestResult testResult = new TestResult("RootNamespace.MyTestFixture.BaseMethod");
			testResult.ResultType = TestResultType.Failure;
			testClasses.UpdateTestResult(testResult);
			
			Assert.AreEqual(TestResultType.Failure, testClass.Result);
		}
	}
}
