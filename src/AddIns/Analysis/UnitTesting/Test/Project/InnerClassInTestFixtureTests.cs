// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;
using System.Linq;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that a class nested inside test fixture is recognized, e.g.
	/// 
	/// [TestFixture]
	/// public class A
	/// {	
	/// 	public class InnerATest
	/// 	{
	/// 		[Test]
	/// 		public void FooBar()
	/// 		{
	/// 		}
	/// 
	/// 		[TestFixture]
	/// 		public class InnerTestLevel2 {
	/// 		}
	/// 	}
	/// 	
	///		public class InnerBClass() {}
	/// }
	///
	/// </summary>
	[TestFixture]
	public class InnerClassInTestFixtureTests : InnerClassTestFixtureBase
	{
		MockClass classNestedInInnerClass;

		[SetUp]
		public void Init()
		{
			base.InitBase();

			//Add TestFixture attribute to outer class
			outerClass.Attributes.Add(new MockAttribute("TestFixture"));
			testProject = new TestProject(null, projectContent, testFrameworks);

			//Add inner class nested in test class
			classNestedInInnerClass = new MockClass(projectContent, "MyTests.A.InnerATest.InnerTestLevel2", "MyTests.A+InnerATest+InnerTestLevel2", innerClass);
			classNestedInInnerClass.Attributes.Add(new MockAttribute("TestFixture"));
			innerClass.InnerClasses.Add(classNestedInInnerClass);
		}
		
		[Test]
		public void OuterTestClassFound()
		{
			AssertTestResultContainsClass(outerClass);
		}

		[Test]
		public void InnerTestClassWithTestFixtureAttributeFound()
		{
			AssertTestResultContainsClass(innerClass);
		}

		[Test]
		public void InnerNonTestClassWithoutWasNotMarkedAsTestClass()
		{
			AssertTestResultDoesNotContainClass(nonTestInnerClass);
		}
		
		[Test]
		public void InnerClassInInnerClassFound()
		{
			AssertTestResultContainsClass(classNestedInInnerClass);
		}

		[Test]
		public void TestClassNameShouldBeDotNetNameOfTheDoubleNestedClass()
		{
			Assert.AreEqual("A+InnerATest+InnerTestLevel2", TestClassRelatedTo(classNestedInInnerClass).Name);
		}

		[Test]
		public void TestClassNamespaceShouldBeValid()
		{
			Assert.AreEqual("MyTests", TestClassRelatedTo(classNestedInInnerClass).Namespace);
		}

		void AssertTestResultContainsClass(IClass clazz)
		{
			var testClazz = TestClassRelatedTo(clazz);
			if (testClazz == null)
				throw new AssertionException(string.Format("Test result should contain class {0}.", clazz.FullyQualifiedName));
		}

		void AssertTestResultDoesNotContainClass(MockClass clazz)
		{
			var testClazz = TestClassRelatedTo(clazz);
			if (testClazz != null)
				throw new AssertionException(string.Format("Test result should not contain class {0}.", clazz.FullyQualifiedName));
		}

		TestClass TestClassRelatedTo(IClass clazz)
		{
			return testProject.TestClasses.SingleOrDefault(c => c.Class == clazz);
		}
	}
}
