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

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class TestableThreadSafeScriptingConsoleEvents : ThreadSafeScriptingConsoleEvents
	{
		public FakeScriptingConsole NonThreadSafeScriptingConsole;
		public FakeControlDispatcher Dispatcher;
		
		public bool IsWaitForLineCalled;
		public bool WaitForLineReturnValue;
		public bool DispatcherCheckAccessReturnValueAfterWaitForLineCalled;
		public bool IsLineReceivedEventResetBeforeReadLineCalled;
		public bool IsLineReceivedEventReset;
		public bool IsSetDisposedEventCalled;
		public bool IsSetLineReceivedEventCalled;
		
		public override bool WaitForLine()
		{
			IsWaitForLineCalled = true;
			Dispatcher.CheckAccessReturnValue = DispatcherCheckAccessReturnValueAfterWaitForLineCalled;
			return WaitForLineReturnValue;
		}
		
		public override void ResetLineReceivedEvent()
		{
			IsLineReceivedEventReset = true;
			IsLineReceivedEventResetBeforeReadLineCalled = !NonThreadSafeScriptingConsole.IsReadLineCalled;
		}
		
		public override void SetDisposedEvent()
		{
			IsSetDisposedEventCalled = true;
		}
		
		public override void SetLineReceivedEvent()
		{
			IsSetLineReceivedEventCalled = true;
		}
	}
}
