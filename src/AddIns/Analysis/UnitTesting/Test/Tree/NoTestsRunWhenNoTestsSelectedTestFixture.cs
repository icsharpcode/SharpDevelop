// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class NoTestsRunWhenNoTestsSelectedTestFixture : RunTestCommandTestFixtureBase
	{
		[SetUp]
		public void Init()
		{
			base.InitBase();
			runTestCommand.Run();
		}
		
		[Test]
		public void OnBeforeRunIsNotCalled()
		{
			Assert.IsFalse(runTestCommand.IsOnBeforeBuildMethodCalled);
		}
	}
}
