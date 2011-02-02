// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementTask<TResult> : ITask<TResult>
	{
		Task<TResult> task;
		Action<ITask<TResult>> continueWith;
		CancellationTokenSource cancellationTokenSource;
			
		public PackageManagementTask(
			Func<TResult> function,
			Action<ITask<TResult>> continueWith)
		{
			this.continueWith = continueWith;			
			CreateTask(function);
		}

		void CreateTask(Func<TResult> function)
		{
			TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
			cancellationTokenSource = new CancellationTokenSource();
			task = new Task<TResult>(function, cancellationTokenSource.Token);
			task.ContinueWith(result => OnContinueWith(result), scheduler);
		}
		
		void OnContinueWith(Task<TResult> task)
		{
			continueWith(this);
		}
		
		public void Start()
		{
			task.Start();
		}
		
		public TResult Result {
			get { return task.Result; }
		}
		
		public void Cancel()
		{
			cancellationTokenSource.Cancel();
		}
		
		public bool IsCancelled {
			get { return task.IsCanceled; }
		}
		
		public bool IsFaulted {
			get { return task.IsFaulted; }
		}
		
		public AggregateException Exception {
			get { return task.Exception; }
		}
	}
}
