// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;

namespace ICSharpCode.Scripting
{
	public class ThreadSafeScriptingConsoleEvents
	{
		// The index into the waitHandles array where the lineReceivedEvent is stored.
		int lineReceivedEventIndex = 1; 
		ManualResetEvent lineReceivedEvent = new ManualResetEvent(false);
		ManualResetEvent disposedEvent = new ManualResetEvent(false);
		WaitHandle[] waitHandles;

		public ThreadSafeScriptingConsoleEvents()
		{
			waitHandles = new WaitHandle[] {disposedEvent, lineReceivedEvent};
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
