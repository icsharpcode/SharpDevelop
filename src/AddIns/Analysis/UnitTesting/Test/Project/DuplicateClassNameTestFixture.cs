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
