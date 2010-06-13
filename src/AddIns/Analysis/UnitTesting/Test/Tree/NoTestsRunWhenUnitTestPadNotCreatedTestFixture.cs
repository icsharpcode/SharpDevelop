// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class NoTestsRunWhenUnitTestPadNotCreatedTestFixture
	{
		DerivedRunTestCommand runTestCommand;
		MockRunTestCommandContext runTestCommandContext;
		
		[SetUp]
		public void Init()
		{
			runTestCommandContext = new MockRunTestCommandContext();
			runTestCommand = new DerivedRunTestCommand(runTestCommandContext);
		}
		
		[Test]
		public void RunTestCommandRunMethodDoesNotThrowNullReferenceException()
		{
			Assert.DoesNotThrow(delegate { runTestCommand.Run(); });
		}
		
		[Test]
		public void OnBeforeRunIsNotCalled()
		{
			runTestCommand.Run();
			Assert.IsFalse(runTestCommand.IsOnBeforeRunTestsMethodCalled);
		}
	}
}
