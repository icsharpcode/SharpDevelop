// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
