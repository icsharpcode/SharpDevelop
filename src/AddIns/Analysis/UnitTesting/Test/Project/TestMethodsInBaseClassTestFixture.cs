// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		
		[SetUp]
		public void SetUp()
		{			
			MockProjectContent projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			
			// Create the base test class.
			MockClass baseClass = new MockClass("RootNamespace.TestFixtureBase");
			baseClass.Attributes.Add(new MockAttribute("TestFixture"));
			baseClass.ProjectContent = projectContent;
			MockMethod baseMethod = new MockMethod("BaseMethod");
			baseMethod.Attributes.Add(new MockAttribute("Test"));
			baseMethod.DeclaringType = baseClass;
			baseClass.Methods.Add(baseMethod);
			
			// Create the derived test class.
			c = new MockClass("RootNamespace.MyTestFixture");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			c.ProjectContent = projectContent;
			MockMethod method = new MockMethod("DerivedMethod");
			method.DeclaringType = c;
			method.Attributes.Add(new MockAttribute("Test"));
			c.Methods.Add(method);
			projectContent.Classes.Add(c);

			// Set derived class's base class.
			c.BaseClass = baseClass;
			
			// Create TestClass.
			testClass = new TestClass(c);
		}
		
		[Test]
		public void TwoTestMethods()
		{
			Assert.AreEqual(2, testClass.TestMethods.Count);
		}
		
		[Test]
		public void DerivedMethod()
		{
			Assert.IsTrue(testClass.TestMethods.Contains("DerivedMethod"));
		}
		
		[Test]
		public void BaseMethod()
		{
			Assert.IsTrue(testClass.TestMethods.Contains("TestFixtureBase.BaseMethod"));
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
			TestMethod method = testClass.TestMethods["TestFixtureBase.BaseMethod"];
			Assert.AreEqual(c, method.Method.DeclaringType);
		}
		
		[Test]
		public void UpdateTestResultUsingPrefixBaseClassName()
		{
			TestClassCollection testClasses = new TestClassCollection();
			testClasses.Add(testClass);

			TestResult testResult = new TestResult("RootNamespace.MyTestFixture.TestFixtureBase.BaseMethod");
			testResult.IsFailure = true;
			testClasses.UpdateTestResult(testResult);
			
			Assert.AreEqual(TestResultType.Failure, testClass.Result);
		}
		
		[Test]
		public void UpdateTestResult()
		{
			TestClassCollection testClasses = new TestClassCollection();
			testClasses.Add(testClass);

			TestResult testResult = new TestResult("RootNamespace.MyTestFixture.BaseMethod");
			testResult.IsFailure = true;
			testClasses.UpdateTestResult(testResult);
			
			Assert.AreEqual(TestResultType.Failure, testClass.Result);
		}
	}
}
