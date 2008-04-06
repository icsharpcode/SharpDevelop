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
			projectContent.Language = LanguageProperties.None;
			
			// Create the base test class.
			MockClass baseClass = new MockClass("ICSharpCode.SharpDevelop.Tests.ReflectionOrCecilLayerTests");
			baseClass.ProjectContent = projectContent;
			MockMethod baseMethod = new MockMethod("InheritanceTests");
			baseMethod.Attributes.Add(new MockAttribute("Test"));
			baseMethod.DeclaringType = baseClass;
			baseClass.Methods.Add(baseMethod);

			// Add a second method that does not have a Test attribute.
			baseMethod = new MockMethod("NonTestMethod");
			baseMethod.DeclaringType = baseClass;
			baseClass.Methods.Add(baseMethod);
			
			// Create the derived test class.
			c = new MockClass("ICSharpCode.SharpDevelop.Tests.CecilLayerTests");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			c.ProjectContent = projectContent;
			projectContent.Classes.Add(c);

			// Set derived class's base class.
			c.BaseClass = baseClass;
			
			// Create TestClass.
			testClass = new TestClass(c);
		}

		[Test]
		public void BaseMethodExists()
		{
			Assert.IsTrue(testClass.TestMethods.Contains("ReflectionOrCecilLayerTests.InheritanceTests"));
		}

		[Test]
		public void NonTestBaseMethodDoesNotExist()
		{
			Assert.IsFalse(testClass.TestMethods.Contains("ReflectionOrCecilLayerTests.NonTestMethod"));
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
			TestMethod method = testClass.TestMethods["ReflectionOrCecilLayerTests.InheritanceTests"];
			Assert.AreEqual(c, method.Method.DeclaringType);
		}

		[Test]
		public void UpdateTestResult()
		{
			TestClassCollection testClasses = new TestClassCollection();
			testClasses.Add(testClass);

			TestResult testResult = new TestResult("ICSharpCode.SharpDevelop.Tests.CecilLayerTests.InheritanceTests");
			testResult.IsFailure = true;
			testClasses.UpdateTestResult(testResult);
			
			Assert.AreEqual(TestResultType.Failure, testClass.Result);
		}		
	}
}
