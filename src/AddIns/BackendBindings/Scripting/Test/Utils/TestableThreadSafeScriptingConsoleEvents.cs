// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
