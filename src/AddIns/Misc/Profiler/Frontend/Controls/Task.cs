using System;
using System.Threading;
using System.Windows.Threading;

namespace ICSharpCode.Profiler.Controls
{
	/// <summary>
	/// A task that is run in the background and can be cancelled.
	/// </summary>
	/// <example>
	/// Usage:
	/// <code>Task t = Task.Start(TaskMethod);
	/// t.RunWhenComplete(CompleteCallback);</code>
	/// Tasks can be cancelled:
	/// <code>t.Cancel();</code>
	/// The task method must poll for being cancelled:
	/// <code>if (Task.Current.IsCancelled) return;</code>
	/// </example>
	sealed class Task
	{
		[ThreadStatic] static Task currentTask;
		
		readonly object lockObj = new object();
		Action action;
		
		volatile bool cancel;
		bool isComplete;
		Action onCompleteActions;
		
		private Task(Action action)
		{
			this.action = action;
		}
		
		public static Task Start(Action action)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			Task t = new Task(action);
			ThreadPool.QueueUserWorkItem(t.Run, null);
			return t;
		}
		
		/// <summary>
		/// Gets the task running on the current thread.
		/// </summary>
		public static Task Current {
			get {
				return currentTask;
			}
		}
		
		void Run(object state)
		{
			currentTask = this;
			try {
				action();
			} finally {
				currentTask = null;
				// let the GC collect the action delegate and any objects referenced by its closure
				action = null;
			}
			
			lock (lockObj) {
				isComplete = true;
			}
			// The lock above ensures that now onCompleteActions is not modified anymore.
			if (onCompleteActions != null) {
				onCompleteActions();
				// let the GC collect the onCompleteActions and objects referenced by their closures
				onCompleteActions = null;
			}
		}
		
		/// <summary>
		/// Gets whether the task was cancelled.
		/// </summary>
		public bool IsCancelled {
			get { return cancel; }
		}
		
		/// <summary>
		/// Cancels the task (simply sets IsCancelled to true).
		/// If the task has already finished, the cancel flag is not changed.
		/// (this is done to ensure that IsCancelled cannot change asynchronously to the
		/// execution of "RunWhenComplete" callbacks)
		/// </summary>
		public void Cancel()
		{
			lock (lockObj) {
				if (!isComplete) {
					cancel = true;
				}
			}
		}
		
		/// <summary>
		/// Runs the action after the task has completed. If the task already has completed,
		/// the action is run immediately.
		/// The action will run on a thread pool thread (not necessarily on the same thread that executed the task).
		/// 
		/// When you call RunWhenComplete multiple times, it is possible that the completion actions will
		/// execute in parallel.
		/// </summary>
		public void RunWhenComplete(Action action)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			lock (lockObj) {
				if (isComplete) {
					// already complete: start action immediately (but on another thread!)
					ThreadPool.QueueUserWorkItem(state => action());
				} else {
					// not yet complete: store action and run it when complete
					onCompleteActions += action;
				}
			}
		}
		
		/// <summary>
		/// Runs the action after the task has completed. If the task already has completed,
		/// the action is run immediately.
		/// The action will be run on the Dispatcher with normal priority.
		/// </summary>
		public void RunWhenComplete(Dispatcher dispatcher, Action action)
		{
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");
			if (action == null)
				throw new ArgumentNullException("action");
			lock (lockObj) {
				if (isComplete) {
					// already complete: start action immediately
					dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
				} else {
					// not yet complete: store action and run it when complete
					onCompleteActions += delegate {
						dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
					};
				}
			}
		}
	}
}
