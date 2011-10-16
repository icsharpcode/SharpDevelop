// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
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
	/// 	}
	/// }
	/// 
	/// In this case the FooBar test is identified via: "A+InnerATest.FooBar".
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
		public void InnerTestClassWithoutTestFixtureAttributeFound()
		{
			AssertTestResultContainsClass(nonTestInnerClass);
		}

		[Test]
		public void InnerClassInInnerClassFound()
		{
			AssertTestResultContainsClass(classNestedInInnerClass);
		}

		void AssertTestResultContainsClass(IClass clazz)
		{
			var count = testProject.TestClasses.Count(c => c.Class == clazz);
			if (count != 1)
				throw new AssertionException(string.Format("Test result should contain class {0}.", clazz.FullyQualifiedName));
		}
	}
}
