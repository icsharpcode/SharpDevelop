// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Threading;

namespace ICSharpCode.Scripting
{
	public class ThreadSafeScriptingConsoleEvents
	{
		// The index into the waitHandles array where the lineReceivedEvent is stored.
		int lineReceivedEventIndex = 0; 
		ManualResetEvent lineReceivedEvent = new ManualResetEvent(false);
		ManualResetEvent disposedEvent = new ManualResetEvent(false);
		WaitHandle[] waitHandles;

		public ThreadSafeScriptingConsoleEvents()
		{
			waitHandles = new WaitHandle[] {lineReceivedEvent, disposedEvent};
		}
		
		public virtual bool WaitForLine()
		{
			int result = WaitHandle.WaitAny(waitHandles);
			return lineReceivedEventIndex == result;
		}
		
		public virtual void SetDisposedEvent()
		{
			disposedEvent.Set();
		}
		
		public virtual void SetLineReceivedEvent()
		{
			lineReceivedEvent.Set();
		}
		
		public virtual void ResetLineReceivedEvent()
		{
			lineReceivedEvent.Reset();
		}
	}
}
