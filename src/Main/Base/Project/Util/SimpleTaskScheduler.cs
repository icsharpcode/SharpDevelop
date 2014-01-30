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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// A simple scheduler that adds tasks to a queue.
	/// This scheduler does not create any worker threads on its own,
	/// but requires external code to call <see cref="RunNextTask"/>.
	/// </summary>
	public class SimpleTaskScheduler : TaskScheduler, IDisposable
	{
		[ThreadStatic]
		static SimpleTaskScheduler activeScheduler;
		
		BlockingCollection<Task> queue = new BlockingCollection<Task>();
		
		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			return activeScheduler == this && base.TryExecuteTask(task);
		}
		
		protected override void QueueTask(Task task)
		{
			queue.Add(task);
		}
		
		protected override IEnumerable<Task> GetScheduledTasks()
		{
			return queue;
		}
		
		protected int ScheduledTaskCount {
			get { return queue.Count; }
		}
		
		/// <summary>
		/// Runs the next task in the queue.
		/// If no task is available, this method will block.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token that can be used to cancel
		/// waiting for a task to become available. It cannot be used to cancel task execution!</param>
		public void RunNextTask(CancellationToken cancellationToken = default(CancellationToken))
		{
			Task task = queue.Take(cancellationToken);
			RunTask(task);
		}
		
		public bool TryRunNextTask()
		{
			Task task;
			if (queue.TryTake(out task)) {
				RunTask(task);
				return true;
			} else {
				return false;
			}
		}
		
		void RunTask(Task task)
		{
			var oldActiveScheduler = activeScheduler;
			activeScheduler = this;
			try {
				base.TryExecuteTask(task);
			} finally {
				activeScheduler = oldActiveScheduler;
			}
		}
		
		public virtual void Dispose()
		{
			queue.Dispose();
		}
	}
}
