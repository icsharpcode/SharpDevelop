// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests the TestClass.RootNamespace property.
	/// </summary>
	[TestFixture]
	public class ClassRootNamespaceTests
	{
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void Init()
		{
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
		}
		
		[Test]
		public void OneParentNamespace()
		{
			MockClass mockClass = new MockClass("TestRootNamespace.MyFixture");
			TestClass testClass = new TestClass(mockClass, testFrameworks);
			Assert.AreEqual("TestRootNamespace", testClass.RootNamespace);
		}
		
		[Test]
		public void TwoParentNamespaces()
		{
			MockClass mockClass = new MockClass("TestRootNamespace.Tests.MyFixture");
			TestClass testClass = new TestClass(mockClass, testFrameworks);
			Assert.AreEqual("TestRootNamespace", testClass.RootNamespace);
		}
		
		[Test]
		public void NoNamespace()
		{
			MockClass mockClass = new MockClass("MyFixture");
			TestClass testClass = new TestClass(mockClass, testFrameworks);
			Assert.AreEqual(String.Empty, testClass.RootNamespace);
		}
	}
}
