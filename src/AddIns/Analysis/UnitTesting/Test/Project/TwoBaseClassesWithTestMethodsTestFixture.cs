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
	/// Tests that the test methods of a second parent base class are detected:
	/// 
	/// class BaseBaseTestFixture { [Test] public void BaseBaseTest() ... }
	/// class BaseTestFixture : BaseBaseTestFixture ...
	/// class TestFixture : BaseTestFixture
	/// </summary>
	[TestFixture]
	public class TwoBaseClassesWithTestMethodsTestFixture
	{
		TestClass testClass;
		MockClass c;
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void SetUp()
		{
			MockProjectContent projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			
			// Create the top base test class.
			MockClass baseBaseClass = new MockClass(projectContent, "ICSharpCode.SharpDevelop.Tests.BaseBaseTestFixture");
			MockMethod baseMethod = new MockMethod(baseBaseClass, "BaseBaseTest");
			baseMethod.Attributes.Add(new MockAttribute("Test"));
			baseBaseClass.Methods.Add(baseMethod);
			
			// Create the next level test class.
			MockClass baseClass = new MockClass(projectContent, "ICSharpCode.SharpDevelop.Tests.BaseTestFixture");
			baseMethod = new MockMethod(baseClass, "BaseTest");
			baseMethod.Attributes.Add(new MockAttribute("Test"));
			baseClass.Methods.Add(baseMethod);
			
			// Create the derived test class.
			c = new MockClass(projectContent, "ICSharpCode.SharpDevelop.Tests.MainTestFixture");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(c);

			// Set the base class for each class in the hierarchy.
			c.AddBaseClass(baseClass);
			baseClass.AddBaseClass(baseBaseClass);
			
			// Create TestClass.
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			testClass = new TestClass(c, testFrameworks);
		}

		[Test]
		public void BaseBaseTestMethodExists()
		{
			Assert.IsTrue(testClass.TestMembers.Contains("BaseBaseTestFixture.BaseBaseTest"));
		}

		[Test]
		public void BaseMethodExists()
		{
			Assert.IsTrue(testClass.TestMembers.Contains("BaseTestFixture.BaseTest"));
		}
		
		/// <summary>
		/// The TestMethod.Method property should return an IMethod 
		/// that returns the derived class from the DeclaringType property
		/// and not the base class. This ensures that the correct
		/// test is run when selected in the unit test tree.
		/// </summary>
		[Test]
		public void BaseBaseMethodDeclaringTypeIsDerivedClass()
		{
			TestMember method = testClass.TestMembers["BaseBaseTestFixture.BaseBaseTest"];
			Assert.AreEqual(c, method.Member.DeclaringType);
		}
	
		[Test]
		public void UpdateTestResult()
		{
			TestClassCollection testClasses = new TestClassCollection();
			testClasses.Add(testClass);

			TestResult testResult = new TestResult("ICSharpCode.SharpDevelop.Tests.MainTestFixture.BaseBaseTest");
			testResult.ResultType = TestResultType.Failure;
			testClasses.UpdateTestResult(testResult);
			
			Assert.AreEqual(TestResultType.Failure, testClass.Result);
		}
	}
}
