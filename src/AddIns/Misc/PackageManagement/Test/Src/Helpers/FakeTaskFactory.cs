// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakeTaskFactory : ITaskFactory
	{
		public bool IsCreateTaskCalled;
		public bool RunTasksSynchronously;
		
		public FakeTask<PackagesForSelectedPageResult> FirstFakeTaskCreated { 
			get { return FakeTasksCreated[0] as FakeTask<PackagesForSelectedPageResult>; }
		}
		
		public List<object> FakeTasksCreated = new List<object>();
		
		public ITask<TResult> CreateTask<TResult>(
			Func<TResult> function,
			Action<ITask<TResult>> continueWith)
		{
			IsCreateTaskCalled = true;
			var task = new FakeTask<TResult>(function, continueWith, RunTasksSynchronously);
			FakeTasksCreated.Add(task);
			return task;
		}
		
		public void ExecuteAllFakeTasks()
		{
			foreach (FakeTask<PackagesForSelectedPageResult> task in FakeTasksCreated) {
				task.ExecuteTaskCompletely();
			}
		}
		
		public void ClearAllFakeTasks()
		{
			FakeTasksCreated.Clear();
		}
	}
}
