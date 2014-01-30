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
		
		public List<SDTask> TasksAdded = new List<SDTask>();
		
		public SDTask FirstTaskAdded {
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
		
		public void AddTask(SDTask task)
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
