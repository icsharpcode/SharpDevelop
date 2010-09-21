// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
