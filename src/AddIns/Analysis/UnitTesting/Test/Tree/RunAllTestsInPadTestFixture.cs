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
