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
	public class EmptyProjectTestFixture : ProjectTestFixtureBase
	{
		/// <summary>
		/// Tests that a new class is added to the TestProject
		/// from the parse info when the old compilation unit is null.
		/// </summary>
		[Test]
		public void AddClassWithTestFixtureAttribute()
		{
			// Create an empty project
			CreateNUnitProject();
			// Add new compilation unit with extra class.
			UpdateCodeFile("namespace RootNamespace { [NUnit.Framework.TestFixture] class MyTextFixture {} }");
			
			Assert.IsTrue(testProject.TestClasses.Any(c => c.QualifiedName == "RootNamespace.MyNewTestFixture"));
		}
		
		/// <summary>
		/// The class exists in both the old compilation unit and the
		/// new compilation unit, but in the new compilation unit
		/// it has an added [TestFixture] attribute.
		/// </summary>
		[Test]
		public void AddTestFixtureAttributeToExistingClass()
		{
			CreateNUnitProject();
			// Create an old compilation unit with the test class
			// but without a [TestFixture] attribute.
			UpdateCodeFile("namespace RootNamespace { class MyTextFixture {} }");
			
			// Create a new compilation unit with the test class
			// having a [TestFixture] attribute.
			UpdateCodeFile("namespace RootNamespace { [NUnit.Framework.TestFixture] class MyTextFixture {} }");
			
			Assert.IsTrue(testProject.TestClasses.Any(c => c.QualifiedName == "RootNamespace.MyNewTestFixture"),
			              "New class should have been added to the set of TestClasses.");
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
			
			// Create an old compilation unit with the test class
			// having a [TestFixture] attribute.
			UpdateCodeFile("namespace RootNamespace { class MyTextFixture {} }");
			
			Assert.IsFalse(testProject.TestClasses.Any(c => c.QualifiedName == "RootNamespace.MyNewTestFixture"),
			               "Class should have been removed.");
		}
	}
}
