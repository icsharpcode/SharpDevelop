// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockTestFrameworksWithNUnitFrameworkSupportTestFixture
	{
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void Init()
		{
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
		}
		
		[Test]
		public void ImplementsIRegisteredTestFrameworkInterface()
		{
			Assert.IsNotNull(testFrameworks as IRegisteredTestFrameworks);
		}
		
		[Test]
		public void IsNUnitTestFramework()
		{
			Assert.IsInstanceOf(typeof(NUnitTestFramework), testFrameworks);
		}
	}
}
