// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
