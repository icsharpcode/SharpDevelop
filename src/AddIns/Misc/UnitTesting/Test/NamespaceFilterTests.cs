// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.ConsoleRunner;
using NUnit.Core;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests
{
	/// <summary>
	/// Tests the NamespaceFilter class that is a part of SharpDevelop's
	/// customised nunit-console.exe.
	/// </summary>
	[TestFixture]
	public class NamespaceFilterTests
	{
		[Test]
		public void TestCaseExcluded()
		{
			NamespaceFilter filter = new NamespaceFilter("Project.Tests");
			MockTestCase testCase = new MockTestCase("Project.NotTests.MyTest");
			Assert.IsFalse(filter.Pass(testCase));
		}
		
		[Test]
		public void TestCaseIncluded()
		{
			NamespaceFilter filter = new NamespaceFilter("Project.Tests");
			MockTestCase testCase = new MockTestCase("Project.Tests.MyTest");
			Assert.IsTrue(filter.Pass(testCase));
		}
		
		[Test]
		public void NullTestCase()
		{
			NamespaceFilter filter = new NamespaceFilter("Project.Tests");
			MockTestCase testCase = null;
			Assert.IsFalse(filter.Pass(testCase));
		}
		
		[Test]
		public void NullTestCaseFullName()
		{
			NamespaceFilter filter = new NamespaceFilter("Project.Tests");
			MockTestCase testCase = new MockTestCase("Project.Tests.MyTest");
			testCase.TestName.FullName = null;
			Assert.IsFalse(filter.Pass(testCase));
		}		
		
		[Test]
		public void TestCaseNameMatchesNamespace()
		{
			NamespaceFilter filter = new NamespaceFilter("Project.Test");
			MockTestCase testCase = new MockTestCase("Project.Test");
			Assert.IsFalse(filter.Pass(testCase));
		}

		[Test]
		public void NullTestFixture()
		{
			TestFixture testFixture = null;
			NamespaceFilter filter = new NamespaceFilter("Project.Tests");
			Assert.IsFalse(filter.Pass(testFixture));
		}
		
		[Test]
		public void TestFixtureIncluded()
		{
			MockTestFixture testFixture = new MockTestFixture("Project.Tests.MyTestFixture");
			NamespaceFilter filter = new NamespaceFilter("Project.Tests");
			Assert.IsTrue(filter.Pass(testFixture));
		}
		
		[Test]
		public void TestFixtureExcluded()
		{
			MockTestFixture testFixture = new MockTestFixture("Project.Different");
			NamespaceFilter filter = new NamespaceFilter("Project.Tests");
			Assert.IsFalse(filter.Pass(testFixture));
		}
		
		[Test]
		public void RootNamespaceTestSuiteExcluded()
		{
			MockTestFixture testSuite = new MockTestFixture("Root");
			NamespaceFilter filter = new NamespaceFilter("Project.Tests");
			Assert.IsFalse(filter.Pass(testSuite));
		}
		
		/// <summary>
		/// All test suite classes should pass. NUnit passes namespaces and
		/// the assembly itself to the filter as a TestSuite object.
		/// </summary>		
		[Test]
		public void TestSuitePasses()
		{
			TestSuite testSuite = new TestSuite("TestSuite");
			NamespaceFilter filter = new NamespaceFilter("Tests");
			Assert.IsTrue(filter.Pass(testSuite));
		}
		
		[Test]
		public void NamespaceFilterClassIsSerializable()
		{
			Assert.IsTrue(typeof(NamespaceFilter).IsSerializable);
		}
	}
}
