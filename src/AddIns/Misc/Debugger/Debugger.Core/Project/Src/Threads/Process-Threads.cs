// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public partial class Process
	{
		List<Thread> threadCollection = new List<Thread>();

		public event EventHandler<ThreadEventArgs> ThreadStarted;
		public event EventHandler<ThreadEventArgs> ThreadExited;
		public event EventHandler<ThreadEventArgs> ThreadStateChanged;

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

			throw new DebuggerException("Thread is not in collection");
		}

		internal void AddThread(Thread thread)
		{
			threadCollection.Add(thread);
			thread.ThreadStateChanged += new EventHandler<ThreadEventArgs>(OnThreadStateChanged);
			OnThreadStarted(thread);
		}

		internal void AddThread(ICorDebugThread corThread)
		{
			AddThread(new Thread(this, corThread));
		}

		internal void RemoveThread(Thread thread)
		{
			threadCollection.Remove(thread);
			thread.ThreadStateChanged -= new EventHandler<ThreadEventArgs>(OnThreadStateChanged);
			OnThreadExited(thread);
		}

		internal void RemoveThread(ICorDebugThread corThread)
		{
			RemoveThread(GetThread(corThread));
		}

		internal void ClearThreads()
		{
            foreach (Thread thread in threadCollection) {
				thread.ThreadStateChanged -= new EventHandler<ThreadEventArgs>(OnThreadStateChanged);
				OnThreadExited(thread);
			}
			threadCollection.Clear();
		}
	}
}
