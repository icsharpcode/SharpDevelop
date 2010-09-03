// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class SelectedTestsWithTestMethodTestFixture
	{
		SelectedTests selectedTestsWithTestMethod;
		
		[SetUp]
		public void Init()
		{
			selectedTestsWithTestMethod = SelectedTestsHelper.CreateSelectedTestMethod();
		}
		
		[Test]
		public void SelectedTestsHasProjectSelected()
		{
			Assert.IsNotNull(selectedTestsWithTestMethod.Project);
		}
		
		[Test]
		public void SelectedTestsHasOneProjectSelected()
		{
			Assert.AreEqual(1, selectedTestsWithTestMethod.Projects.Count);
		}
		
		[Test]
		public void SelectedTestsHasClassWithDotNetNameMyTestsMyTestClass()
		{
			Assert.AreEqual("MyTests.MyTestClass", selectedTestsWithTestMethod.Class.DotNetName);
		}
		
		[Test]
		public void SelectedTestsHasMethodWithNameMyTestMethod()
		{
			Assert.AreEqual("MyTestMethod", selectedTestsWithTestMethod.Method.Name);
		}
	}
}
