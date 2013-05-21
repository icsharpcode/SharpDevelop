// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.AddInManager2
{
	public class AddInManagerTask
	{
		public static AddInManagerTask<TResult> Create<TResult>(
			Func<TResult> function,
			Action<AddInManagerTask<TResult>> continueWith)
		{
			return new AddInManagerTask<TResult>(function, continueWith);
		}
	}
	
	public class AddInManagerTask<TResult>
	{
		Task<TResult> task;
		Action<AddInManagerTask<TResult>> continueWith;
		CancellationTokenSource cancellationTokenSource;
		
		public AddInManagerTask(
			Func<TResult> function,
			Action<AddInManagerTask<TResult>> continueWith)
		{
			this.continueWith = continueWith;
			CreateTask(function);
		}

		private void CreateTask(Func<TResult> function)
		{
			TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
			cancellationTokenSource = new CancellationTokenSource();
			task = new Task<TResult>(function, cancellationTokenSource.Token);
			task.ContinueWith(result => OnContinueWith(result), scheduler);
		}
		
		private void OnContinueWith(Task<TResult> task)
		{
			continueWith(this);
		}
		
		public void Start()
		{
			task.Start();
		}
		
		public TResult Result
		{
			get
			{
				return task.Result;
			}
		}
		
		public void Cancel()
		{
			cancellationTokenSource.Cancel();
		}
		
		public bool IsCancelled
		{
			get
			{
				return cancellationTokenSource.IsCancellationRequested;
			}
		}
		
		public bool IsFaulted
		{
			get
			{
				return task.IsFaulted;
			}
		}
		
		public AggregateException Exception
		{
			get
			{
				return task.Exception;
			}
		}
	}
}
