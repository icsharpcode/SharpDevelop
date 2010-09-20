// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using ICSharpCode.Scripting.Tests.Utils;

namespace ICSharpCode.Scripting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockControlDispatcherTestFixture
	{
		FakeControlDispatcher dispatcher;
		
		[SetUp]
		public void Init()
		{
			dispatcher = new FakeControlDispatcher();
		}
		
		[Test]
		public void CheckAccessReturnsFalseWhenCheckAccessReturnValueSetToFalse()
		{
			dispatcher.CheckAccessReturnValue = false;
			Assert.IsFalse(dispatcher.CheckAccess());
		}
		
		[Test]
		public void CheckAccessReturnsTrueWhenCheckAccessReturnValueSetToTrue()
		{
			dispatcher.CheckAccessReturnValue = true;
			Assert.IsTrue(dispatcher.CheckAccess());
		}
		
		[Test]
		public void MethodInvokedReturnsDelegatePassedToInvokeMethod()
		{
			Action expectedDelegate = MethodInvokedReturnsDelegatePassedToInvokeMethod;
			dispatcher.MethodInvoked = null;
			dispatcher.Invoke(expectedDelegate);
			Assert.AreEqual(expectedDelegate, dispatcher.MethodInvoked);
		}
		
		[Test]
		public void MethodInvokedArgsReturnsArgumentsPassedToInvokeMethod()
		{
			dispatcher.MethodInvokedArgs = null;
			dispatcher.Invoke(null, "a", "b");
			
			object[] expectedArgs = new object [] { "a", "b" };
			Assert.AreEqual(expectedArgs, dispatcher.MethodInvokedArgs);
		}
	}
}
