// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that the TestProject can handle the user
	/// opening a project with two test classes with the same
	/// fully qualified name.
	/// </summary>
	[TestFixture]
	public class DuplicateClassNameTestFixture : ProjectTestFixtureBase
	{
		[SetUp]
		public void Init()
		{
			CreateNUnitProject(
				Parse("namespace RootNamespace { [NUnit.Framework.TestFixture] class MyTextFixture {} }", "file1.cs"),
				Parse("namespace RootNamespace { [NUnit.Framework.TestFixture] class MyTextFixture {} }", "file2.cs"));
		}
		
		/// <summary>
		/// If one or more classes exist with the same fully qualified
		/// name only one should be added to the test project. The
		/// project will not compile anyway due to the duplicate class
		/// name so only having one test class is probably an OK
		/// workaround.
		/// </summary>
		[Test]
		public void OneTestClass()
		{
			Assert.AreEqual(1, testProject.TestClasses.Count);
		}
		
		[Test]
		public void TestClassName()
		{
			Assert.AreEqual("RootNamespace.MyTestFixture", testProject.TestClasses[0].QualifiedName);
		}
	}
}
