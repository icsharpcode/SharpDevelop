// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// A task scheduler that manages its own thread pool.
	/// </summary>
	public class CustomThreadPoolTaskScheduler : SimpleTaskScheduler
	{
		int currentThreadCount;
		readonly int maxThreadCount;
		
		public CustomThreadPoolTaskScheduler(int maxThreadCount)
		{
			this.maxThreadCount = maxThreadCount;
		}
		
		public override int MaximumConcurrencyLevel {
			get { return maxThreadCount; }
		}
		
		protected override void QueueTask(Task task)
		{
			base.QueueTask(task);
			if (IncrementThreadCount()) {
				// Successfully incremented the thread count, we may start a thread
				StartThread(RunThread);
				return;
			}
		}
		
		protected virtual void StartThread(ThreadStart start)
		{
			var t = new Thread(start);
			t.IsBackground = true;
			t.Start();
		}
		
		bool IncrementThreadCount()
		{
			int c = Volatile.Read(ref currentThreadCount);
			while (c < maxThreadCount) {
				if (Interlocked.CompareExchange(ref currentThreadCount, c + 1, c) == c) {
					return true;
				}
			}
			return false;
		}
		
		void RunThread()
		{
			do {
				// Run tasks while they are available:
				while (TryRunNextTask());
				// Decrement the thread count:
				Interlocked.Decrement(ref currentThreadCount);
				// Tasks might have been added while we were decrementing the thread count,
				// so if the queue isn't empty anymore, resume this thread
			} while(ScheduledTaskCount > 0 && IncrementThreadCount());
		}
	}
}
