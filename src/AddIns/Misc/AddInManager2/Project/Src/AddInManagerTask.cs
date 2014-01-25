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
