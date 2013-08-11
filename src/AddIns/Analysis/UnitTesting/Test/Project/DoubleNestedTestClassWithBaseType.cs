// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that a class with base type nested inside test fixture is recognized, e.g.
	/// 
	/// public abstract class BaseClass {
	/// 	[Test]
	/// 	public void BaseFoo() {
	/// 	}
	/// }
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
	/// 		public class InnerTestLevel2 : BaseClass {
	/// 		}
	/// 	}
	/// }
	/// </summary>
	[TestFixture]
	public class DoubleNestedTestClassWithBaseType : InnerClassTestFixtureBase
	{
		MockClass classNestedInInnerClass;
		MockClass baseClass;
		MockMethod testMethodInBaseClass;
		
		[SetUp]
		public void SetUp() {
			base.InitBase();
			
			baseClass = new MockClass(projectContent, "MyTests.BaseClass");
			testMethodInBaseClass = new MockMethod(baseClass, "BaseFoo");
			testMethodInBaseClass.Attributes.Add(new MockAttribute("Test"));
			baseClass.Methods.Add(testMethodInBaseClass);

			//Add TestFixture attribute to outer class
			outerClass.Attributes.Add(new MockAttribute("TestFixture"));

			//Add inner class nested in test class
			classNestedInInnerClass = new MockClass(projectContent, "MyTests.A.InnerATest.InnerTestLevel2", "MyTests.A+InnerATest+InnerTestLevel2", innerClass);
			innerClass.InnerClasses.Add(classNestedInInnerClass);
			classNestedInInnerClass.AddBaseClass(baseClass);
			
			testProject = new TestProject(null, projectContent, testFrameworks);
		}
		
		[Test]
		public void DoubleNestedClassShouldHaveTestMemberImportedFromBaseClass() {
			var nestedClass = testProject.TestClasses.Single(c => c.Class == classNestedInInnerClass);
			Assert.AreEqual("BaseClass.BaseFoo", nestedClass.TestMembers.Single().Name);
		}
	}
}
