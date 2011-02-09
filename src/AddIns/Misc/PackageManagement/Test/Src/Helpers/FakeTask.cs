// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	public class FakeTask<TResult> : ITask<TResult>
	{
		public bool IsStartCalled;
		public bool IsCancelCalled;
		
		public bool RunTaskSynchronously;
		
		Func<TResult> function;
		Action<ITask<TResult>> continueWith;
		
		public FakeTask(Func<TResult> function, Action<ITask<TResult>> continueWith, bool runTaskSynchronously)
		{
			this.function = function;
			this.continueWith = continueWith;
			RunTaskSynchronously = runTaskSynchronously;
			Exception = new AggregateException(new Exception("FakeTaskAggregateInnerException"));
		}
		
		public void Start()
		{
			IsStartCalled = true;
			if (RunTaskSynchronously) {
				ExecuteTaskCompletely();
			}
		}
		
		public TResult Result { get; set; }
		
		public void ExecuteTaskCompletely()
		{
			ExecuteTaskButNotContinueWith();
			ExecuteContinueWith();
		}

		public TResult ExecuteTaskButNotContinueWith()
		{
			Result = function();
			return Result;
		}
		
		public void ExecuteContinueWith()
		{
			continueWith(this);
		}
		
		public void Cancel()
		{
			IsCancelCalled = true;
		}
		
		public bool IsCancelled { get;set; }
		public bool IsFaulted { get; set; }
		
		public AggregateException Exception { get; set; }
	}
}
