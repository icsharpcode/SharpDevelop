// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// SD2-1213 - Creating a unit test with the same name as an existing test
	///
	/// Two files exist in a project each having the same unit test class. In one file the
	/// name of the class is changed. This should result in both test classes being displayed in the unit
	/// test tree.
	/// </summary>
	[TestFixture]
	public class DuplicateClassNameTestFixture : NUnitTestProjectFixtureBase
	{
		const string program = @"
using NUnit.Framework;
namespace RootNamespace {
	class MyTestFixture {
		[Test]
		public void Foo() {}
	}
}
";
		
		NUnitTestClass myTestFixture;
		
		public override void SetUp()
		{
			base.SetUp();
			AddCodeFile("file1.cs", program);
			
			myTestFixture = (NUnitTestClass)testProject.NestedTests.Single();
			myTestFixture.EnsureNestedTestsInitialized();
			
			AddCodeFile("file2.cs", program);
		}
		
		[Test]
		public void SingleTestClass()
		{
			Assert.AreSame(myTestFixture, testProject.NestedTests.Single());
		}
		
		[Test]
		public void SingleMethod()
		{
			Assert.AreEqual(1, myTestFixture.NestedTests.Count);
		}
		
		[Test]
		public void RenameOneCopyOfDuplicateClass()
		{
			UpdateCodeFile("file2.cs", program.Replace("MyTestFixture", "NewTestFixture"));
			
			Assert.AreEqual(2, testProject.NestedTests.Count);
			Assert.AreEqual(1, myTestFixture.NestedTests.Count);
		}
	}
}
