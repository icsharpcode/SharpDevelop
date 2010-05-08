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
		
		[SetUp]
		public void SetUp()
		{			
			MockProjectContent projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;

			// Create the top base test class.
			MockClass baseBaseClass = new MockClass("ICSharpCode.SharpDevelop.Tests.BaseBaseTestFixture");
			baseBaseClass.ProjectContent = projectContent;
			MockMethod baseMethod = new MockMethod("BaseBaseTest");
			baseMethod.Attributes.Add(new MockAttribute("Test"));
			baseMethod.DeclaringType = baseBaseClass;
			baseBaseClass.Methods.Add(baseMethod);
			
			// Create the next level test class.
			MockClass baseClass = new MockClass("ICSharpCode.SharpDevelop.Tests.BaseTestFixture");
			baseClass.ProjectContent = projectContent;
			baseMethod = new MockMethod("BaseTest");
			baseMethod.Attributes.Add(new MockAttribute("Test"));
			baseMethod.DeclaringType = baseClass;
			baseClass.Methods.Add(baseMethod);

			// Create the derived test class.
			c = new MockClass("ICSharpCode.SharpDevelop.Tests.MainTestFixture");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			c.ProjectContent = projectContent;
			projectContent.Classes.Add(c);

			// Set the base class for each class in the hierarchy.
			c.BaseClass = baseClass;
			baseClass.BaseClass = baseBaseClass;
			
			// Create TestClass.
			testClass = new TestClass(c);
		}

		[Test]
		public void BaseBaseTestMethodExists()
		{
			Assert.IsTrue(testClass.TestMethods.Contains("BaseBaseTestFixture.BaseBaseTest"));
		}

		[Test]
		public void BaseMethodExists()
		{
			Assert.IsTrue(testClass.TestMethods.Contains("BaseTestFixture.BaseTest"));
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
			TestMethod method = testClass.TestMethods["BaseBaseTestFixture.BaseBaseTest"];
			Assert.AreEqual(c, method.Method.DeclaringType);
		}
	
		[Test]
		public void UpdateTestResult()
		{
			TestClassCollection testClasses = new TestClassCollection();
			testClasses.Add(testClass);

			TestResult testResult = new TestResult("ICSharpCode.SharpDevelop.Tests.MainTestFixture.BaseBaseTest");
			testResult.IsFailure = true;
			testClasses.UpdateTestResult(testResult);
			
			Assert.AreEqual(TestResultType.Failure, testClass.Result);
		}
	}
}
