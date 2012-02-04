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
	/// The Unit Tests window was not updating the tree when a test was run if the test
	/// was in an abstract base class that did not use the [TestFixture] attribute. 
	/// 
	/// This is the case with the CecilLayerTests class:
	/// 
	/// [TestFixture]
	/// public class CecilLayerTests : ReflectionOrCecilLayerTests
	/// { ... }
	/// 
	/// public abstract class ReflectionOrCecilLayerTests
	/// {
	///		
	///     [Test]
	///     public void InheritanceTest()
	///     { ... }
	/// }
	/// 
	/// Note that the test result for the test method is written to the file with the name
	/// Namespace.CecilLayerTests.InheritanceTests but the unit tests window displays it with the
	/// base class name prefixed to it to be consistent with NUnit GUI.
	/// </summary>
	[TestFixture]
	public class AbstractBaseClassWithTestMethodsTestFixture
	{
		TestClass testClass;
		MockClass c;
		
		[SetUp]
		public void SetUp()
		{
			MockProjectContent projectContent = new MockProjectContent();
			
			// Create the base test class.
			MockClass baseClass = new MockClass(projectContent, "ICSharpCode.SharpDevelop.Tests.ReflectionOrCecilLayerTests");
			MockMethod baseMethod = new MockMethod(baseClass, "InheritanceTests");
			baseMethod.Attributes.Add(new MockAttribute("Test"));
			baseClass.Methods.Add(baseMethod);
			
			// Add a second method that does not have a Test attribute.
			baseMethod = new MockMethod(baseClass, "NonTestMethod");
			baseClass.Methods.Add(baseMethod);
			
			// Create the derived test class.
			c = new MockClass(projectContent, "ICSharpCode.SharpDevelop.Tests.CecilLayerTests");
			c.SetDotNetName(c.FullyQualifiedName);
			c.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(c);

			// Set derived class's base class.
			c.AddBaseClass(baseClass);
			
			// Create TestClass.
			MockTestFrameworksWithNUnitFrameworkSupport testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			testClass = new TestClass(c, testFrameworks);
		}

		[Test]
		public void BaseMethodExists()
		{
			Assert.IsTrue(testClass.TestMembers.Contains("ReflectionOrCecilLayerTests.InheritanceTests"));
		}

		[Test]
		public void NonTestBaseMethodDoesNotExist()
		{
			Assert.IsFalse(testClass.TestMembers.Contains("ReflectionOrCecilLayerTests.NonTestMethod"));
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
			TestMember method = testClass.TestMembers["ReflectionOrCecilLayerTests.InheritanceTests"];
			Assert.AreEqual(c, method.Member.DeclaringType);
		}

		[Test]
		public void UpdateTestResult()
		{
			TestClassCollection testClasses = new TestClassCollection();
			testClasses.Add(testClass);

			TestResult testResult = new TestResult("ICSharpCode.SharpDevelop.Tests.CecilLayerTests.InheritanceTests");
			testResult.ResultType = TestResultType.Failure;
			testClasses.UpdateTestResult(testResult);
			
			Assert.AreEqual(TestResultType.Failure, testClass.Result);
		}		
	}
}
