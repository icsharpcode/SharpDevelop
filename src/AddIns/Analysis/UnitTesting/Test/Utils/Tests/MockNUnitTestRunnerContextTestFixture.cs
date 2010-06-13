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
	public class MockNUnitTestRunnerContextTestFixture
	{
		MockNUnitTestRunnerContext context;
		
		[SetUp]
		public void Init()
		{
			context = new MockNUnitTestRunnerContext();
		}
		
		[Test]
		public void MockProcessRunnerIsNotNull()
		{
			Assert.IsNotNull(context.MockProcessRunner);
		}
		
		[Test]
		public void MockTestResultsMonitorExists()
		{
			Assert.IsNotNull(context.MockTestResultsMonitor);
		}
		
		[Test]
		public void MockTestsResultsMonitorFileNameIsTmp66DotTmp()
		{
			string expectedFileName = @"c:\temp\tmp66.tmp";
			Assert.AreEqual(expectedFileName, context.MockTestResultsMonitor.FileName.ToString());
		}
	}
}
