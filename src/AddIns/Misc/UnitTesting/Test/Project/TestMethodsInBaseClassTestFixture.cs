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
	/// the base class name to them.
	/// 
	/// [TestFixture]
	/// public DerivedClass : BaseClass
	/// 
	/// Fully qualified test method is named:
	/// 
	/// RootNamespace.DerivedClass.BaseClass.BaseClassMethod.
	/// </summary>
	[TestFixture]
	public class TestMethodsInBaseClassTestFixture
	{
		TestClass testClass;
		
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
			MockClass c = new MockClass("RootNamespace.MyTestFixture");
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
		
		[Test]
		public void GetTestClass()
		{
			TestClassCollection testClasses = new TestClassCollection();
			testClasses.Add(testClass);

			TestResult testResult = new TestResult("RootNamespace.MyTestFixture.TestFixtureBase.BaseMethod");
			testResult.IsFailure = true;
			testClasses.UpdateTestResult(testResult);
			
			Assert.AreEqual(TestResultType.Failure, testClass.Result);
		}
	}
}
