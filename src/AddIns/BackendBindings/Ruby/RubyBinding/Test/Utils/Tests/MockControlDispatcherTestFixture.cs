// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class MockControlDispatcherTestFixture
	{
		MockControlDispatcher dispatcher;
		
		[SetUp]
		public void Init()
		{
			dispatcher = new MockControlDispatcher();
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
