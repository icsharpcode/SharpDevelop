// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Collections.Generic;

using DebuggerInterop.Core;

namespace DebuggerLibrary
{
	public partial class NDebugger
	{
		List<Thread> threadCollection = new List<Thread>();

		public event ThreadEventHandler ThreadStarted;
		public event ThreadEventHandler ThreadExited;
		public event ThreadEventHandler ThreadStateChanged;

		protected void OnThreadStarted(Thread thread)
		{
			if (ThreadStarted != null) {
				ThreadStarted(this, new ThreadEventArgs(thread));
			}
		}

		protected void OnThreadExited(Thread thread)
		{
			if (ThreadExited != null) {
				ThreadExited(this, new ThreadEventArgs(thread));
			}
		}

		protected void OnThreadStateChanged(object sender, ThreadEventArgs e)
		{
			if (ThreadStateChanged != null) {
				ThreadStateChanged(this, new ThreadEventArgs(e.Thread));
			}
		}

		public IList<Thread> Threads {
			get {
				return threadCollection.AsReadOnly();
			}
		}

		internal Thread GetThread(ICorDebugThread corThread)
		{
			foreach(Thread thread in threadCollection) {
				if (thread.CorThread == corThread) {
					return thread;
				}
			}

			throw new UnableToGetPropertyException(this, "this[ICorDebugThread]", "Thread is not in collection");
		}

		internal void AddThread(Thread thread)
		{
			threadCollection.Add(thread);
			thread.ThreadStateChanged += new ThreadEventHandler(OnThreadStateChanged);
			OnThreadStarted(thread);
		}

		internal void AddThread(ICorDebugThread corThread)
		{
			AddThread(new Thread(corThread));
		}

		internal void RemoveThread(Thread thread)
		{
			threadCollection.Remove(thread);
			thread.ThreadStateChanged -= new ThreadEventHandler(OnThreadStateChanged);
			OnThreadExited(thread);
		}

		internal void RemoveThread(ICorDebugThread corThread)
		{
			RemoveThread(GetThread(corThread));
		}

		internal void ClearThreads()
		{
            foreach (Thread thread in threadCollection) {
				thread.ThreadStateChanged -= new ThreadEventHandler(OnThreadStateChanged);
				OnThreadExited(thread);
			}
			threadCollection.Clear();
		}
	}
}
