// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using DebuggerInterop.Core;

namespace DebuggerLibrary
{
	public partial class NDebugger
	{
		List<Thread> threadCollection = new List<Thread>();

		public event EventHandler<ThreadEventArgs> ThreadStarted;
		public event EventHandler<ThreadEventArgs> ThreadExited;
		public event EventHandler<ThreadEventArgs> ThreadStateChanged;

		protected void OnThreadStarted(Thread thread)
		{
			if (ThreadStarted != null) {
				ThreadStarted(this, new ThreadEventArgs(this, thread));
			}
		}

		protected void OnThreadExited(Thread thread)
		{
			if (ThreadExited != null) {
				ThreadExited(this, new ThreadEventArgs(this, thread));
			}
		}

		protected void OnThreadStateChanged(object sender, ThreadEventArgs e)
		{
			if (ThreadStateChanged != null) {
				ThreadStateChanged(this, new ThreadEventArgs(this, e.Thread));
			}
		}

		public IList<Thread> Threads {
			get {
				return threadCollection.AsReadOnly();
			}
		}
		
		public Thread CurrentThread {
			get {
				if (CurrentProcess == null) return null;
				return CurrentProcess.CurrentThread;
			}
			set {
				CurrentProcess.CurrentThread = value;
			}
		}
		
		public bool IsCurrentThreadSafeForInspection {
			get {
				return IsCurrentProcessSafeForInspection;
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
