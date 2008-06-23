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
		Thread selectedThread;
		List<Thread> threadCollection = new List<Thread>();
		
		public event EventHandler<ThreadEventArgs> ThreadStarted;
		
		protected virtual void OnThreadStarted(Thread thread)
		{
			if (ThreadStarted != null) {
				ThreadStarted(this, new ThreadEventArgs(thread));
			}
		}
		
		public Thread SelectedThread {
			get {
				return selectedThread;
			}
			set {
				selectedThread = value;
			}
		}
		
		public IList<Thread> Threads {
			get {
				List<Thread> threads = new List<Thread>();
				foreach(Thread thread in threadCollection) {
					if (!thread.HasExited) {
						threads.Add(thread);
					}
				}
				return threads.AsReadOnly();
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
		
		internal void AddThread(ICorDebugThread corThread)
		{
			Thread thread = new Thread(this, corThread);
			threadCollection.Add(thread);
			OnThreadStarted(thread);
			
			thread.Exited += delegate {
				threadCollection.Remove(thread);
			};
		}
	}
}
