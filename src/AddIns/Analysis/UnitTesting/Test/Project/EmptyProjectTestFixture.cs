// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Linq;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Creates a TestProject object with no test classes.
	/// </summary>
	[TestFixture]
	public class EmptyProjectTestFixture : NUnitTestProjectFixtureBase
	{
		public override void SetUp()
		{
			base.SetUp();
			// Initialize the project while it is empty
			testProject.EnsureNestedTestsInitialized();
		}
		
		[Test]
		public void EmptyProjectHasNoNestedTests()
		{
			Assert.IsEmpty(testProject.NestedTests);
		}
		
		/// <summary>
		/// Tests that a new class is added to the TestProject
		/// from the parse info when the old compilation unit is null.
		/// </summary>
		[Test]
		public void AddClassWithTestFixtureAttribute()
		{
			// Add new compilation unit with extra class.
			AddCodeFileInNamespace("test.cs", "[TestFixture] class MyTestFixture {}");
			
			NUnitTestClass testClass = (NUnitTestClass)testProject.NestedTests.Single();
			Assert.AreEqual("RootNamespace.MyTestFixture", testClass.ReflectionName);
		}
		
		/// <summary>
		/// The class exists in both the old compilation unit and the
		/// new compilation unit, but in the new compilation unit
		/// it has an added [TestFixture] attribute.
		/// </summary>
		[Test]
		public void AddTestFixtureAttributeToExistingClass()
		{
			// Create an old compilation unit with the test class
			// but without a [TestFixture] attribute.
			AddCodeFileInNamespace("test.cs", "class MyTestFixture {}");
			
			Assert.IsEmpty(testProject.NestedTests);
			
			// Create a new compilation unit with the test class
			// having a [TestFixture] attribute.
			UpdateCodeFileInNamespace("test.cs", "[TestFixture] class MyTestFixture {}");
			
			NUnitTestClass testClass = (NUnitTestClass)testProject.NestedTests.Single();
			Assert.AreEqual("RootNamespace.MyTestFixture", testClass.ReflectionName);
		}
		
		/// <summary>
		/// The class exists in both the old compilation unit and the
		/// new compilation unit, but in the new compilation unit
		/// the [TestFixture] attribute has been removed.
		/// </summary>
		[Test]
		public void TestFixtureAttributeRemoved()
		{
			// Add the test class first.
			AddClassWithTestFixtureAttribute();
			
			UpdateCodeFileInNamespace("test.cs", "class MyTestFixture {}");
			
			Assert.IsEmpty(testProject.NestedTests);
		}
		
		[Test]
		public void TestFixtureFileRemoved()
		{
			// Add the test class first.
			AddClassWithTestFixtureAttribute();
			
			RemoveCodeFile("test.cs");
			
			Assert.IsEmpty(testProject.NestedTests);
		}
	}
}
