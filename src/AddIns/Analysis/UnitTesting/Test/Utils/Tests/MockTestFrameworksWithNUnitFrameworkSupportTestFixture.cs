// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
