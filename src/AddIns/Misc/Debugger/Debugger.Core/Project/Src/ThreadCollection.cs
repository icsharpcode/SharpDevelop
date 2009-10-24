// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public class ThreadCollection: CollectionWithEvents<Thread>
	{
		public ThreadCollection(NDebugger debugger): base(debugger) {}
		
		Thread selected;
		
		public Thread Selected {
			get { return selected; }
			set { selected = value; }
		}
		
		internal bool Contains(ICorDebugThread corThread)
		{
			foreach(Thread thread in this) {
				if (thread.CorThread == corThread) return true;
			}
			return false;
		}
		
		internal Thread this[ICorDebugThread corThread] {
			get {
				foreach(Thread thread in this) {
					if (thread.CorThread == corThread) {
						return thread;
					}
				}
				throw new DebuggerException("Thread is not in collection");
			}
		}
	}
}
