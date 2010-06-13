// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
