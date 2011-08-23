// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public interface ITextTemplatingCustomToolContext
	{
		FileProjectItem EnsureOutputFileIsInProject(FileProjectItem baseItem, string outputFileName);
		
		void ClearTasksExceptCommentTasks();
		void AddTask(Task task);
		void BringErrorsPadToFront();
		void DebugLog(string message, Exception ex);
		
		void SetLogicalCallContextData(string name, object data);
	}
}
