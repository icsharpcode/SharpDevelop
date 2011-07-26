// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeTextTemplatingCustomToolContext : ITextTemplatingCustomToolContext
	{
		public FileProjectItem BaseItemPassedToEnsureOutputFileIsInProject;
		public string OutputFileNamePassedToEnsureOutputFileIsInProject;
		public bool IsOutputFileNamePassedToEnsureOutputFileIsInProject;
		public bool IsClearTasksExceptCommentTasksCalled;
		public bool IsBringErrorsPadToFrontCalled;
		public Exception ExceptionPassedToDebugLog;
		public string MessagePassedToDebugLog;
		public string NamePassedToSetLogicalCallContextData;
		public object DataPassedToSetLogicalCallContextData;
		
		public List<Task> TasksAdded = new List<Task>();
		
		public Task FirstTaskAdded {
			get { return TasksAdded[0]; }
		}
		
		public TestableFileProjectItem EnsureOutputFileIsInProjectReturnValue = 
			new TestableFileProjectItem(@"d:\Projects\MyProject\template.tt");
		
		public FileProjectItem EnsureOutputFileIsInProject(FileProjectItem baseItem, string outputFileName)
		{
			BaseItemPassedToEnsureOutputFileIsInProject = baseItem;
			OutputFileNamePassedToEnsureOutputFileIsInProject = outputFileName;
			
			IsOutputFileNamePassedToEnsureOutputFileIsInProject = true;
			
			return EnsureOutputFileIsInProjectReturnValue;
		}
		
		public void ClearTasksExceptCommentTasks()
		{
			IsClearTasksExceptCommentTasksCalled = true;
		}
		
		public void AddTask(Task task)
		{
			TasksAdded.Add(task);
		}
		
		public void BringErrorsPadToFront()
		{
			IsBringErrorsPadToFrontCalled = true;
		}
		
		public void DebugLog(string message, Exception ex)
		{
			MessagePassedToDebugLog = message;
			ExceptionPassedToDebugLog = ex;
		}
		
		public void SetLogicalCallContextData(string name, object data)
		{
			NamePassedToSetLogicalCallContextData = name;
			DataPassedToSetLogicalCallContextData = data;
		}
	}
}
