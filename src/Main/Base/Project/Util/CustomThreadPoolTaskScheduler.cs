// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
