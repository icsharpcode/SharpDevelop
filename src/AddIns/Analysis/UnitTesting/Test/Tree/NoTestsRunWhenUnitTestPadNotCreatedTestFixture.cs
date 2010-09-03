// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			Assert.IsFalse(runTestCommand.IsOnBeforeBuildMethodCalled);
		}
	}
}
