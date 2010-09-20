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
	public class RunAllTestsInPadTestFixture
	{
		RunAllTestsInPadCommand runAllTestsInPadCommand;
		
		[SetUp]
		public void Init()
		{
			MockRunTestCommandContext context = new MockRunTestCommandContext();
			runAllTestsInPadCommand = new RunAllTestsInPadCommand(context);
			runAllTestsInPadCommand.Owner = this;
			runAllTestsInPadCommand.Run();
		}
		
		[Test]
		public void OwnerIsSetToNullWhenRunMethodIsCalled()
		{
			Assert.IsNull(runAllTestsInPadCommand.Owner);
		}
	}
}
