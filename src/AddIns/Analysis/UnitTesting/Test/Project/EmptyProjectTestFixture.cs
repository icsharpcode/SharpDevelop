// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
