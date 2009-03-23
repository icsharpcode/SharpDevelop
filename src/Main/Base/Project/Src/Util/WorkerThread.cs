// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Threading;

namespace ICSharpCode.SharpDevelop.Util
{
	/// <summary>
	/// A worker thread that normally sleeps, but can run a queue of commands.
	/// 
	/// This class does not create a worker thread on its own, it merely manages tasks for
	/// the worker thread that calls <see cref="RunLoop"/>.
	/// </summary>
	public class WorkerThread
	{
		sealed class AsyncTask : IAsyncResult
		{
			internal readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);
			internal readonly Action method;
			volatile bool isCompleted;
			
			internal AsyncTask(Action method)
			{
				this.method = method;
			}
			
			internal void SetCompleted()
			{
				isCompleted = true;
				manualResetEvent.Set();
			}
			
			public bool IsCompleted {
				get { return isCompleted; }
			}
			
			public WaitHandle AsyncWaitHandle {
				get { return manualResetEvent; }
			}
			
			public object AsyncState { get; set; }
			public bool CompletedSynchronously { get { return false; } }
		}
		
		/// <summary>
		/// Runs <paramref name="method"/> on the worker thread.
		/// </summary>
		/// <param name="method">The method to run.</param>
		/// <returns></returns>
		public IAsyncResult Enqueue(Action method)
		{
			if (method == null)
				throw new ArgumentNullException("method");
			AsyncTask task = new AsyncTask(method);
			lock (lockObject) {
				taskQueue.Enqueue(task);
				Monitor.Pulse(lockObject);
			}
			return task;
		}
		
		readonly object lockObject = new object();
		Queue<AsyncTask> taskQueue = new Queue<AsyncTask>();
		bool workerRunning;
		bool exitWorker;
		
		/// <summary>
		/// Runs the worker thread loop on the current thread.
		/// </summary>
		public void RunLoop()
		{
			lock (lockObject) {
				if (workerRunning)
					throw new InvalidOperationException("There already is a worker running");
				workerRunning = true;
			}
			try {
				exitWorker = false;
				while (!exitWorker) {
					AsyncTask task;
					lock (lockObject) {
						while (taskQueue.Count == 0)
							Monitor.Wait(lockObject);
						task = taskQueue.Dequeue();
					}
					task.method();
					task.SetCompleted();
				}
			} finally {
				lock (lockObject) {
					workerRunning = false;
				}
			}
		}
		
		/// <summary>
		/// Exits running the worker thread after executing all currently enqueued methods.
		/// </summary>
		public void ExitWorkerThread()
		{
			Enqueue(delegate { exitWorker = true; });
		}
	}
}
