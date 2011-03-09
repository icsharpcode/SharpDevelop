// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingCustomToolContext : ITextTemplatingCustomToolContext
	{
		CustomToolContext context;
		
		public TextTemplatingCustomToolContext(CustomToolContext context)
		{
			this.context = context;
		}
		
		public FileProjectItem EnsureOutputFileIsInProject(FileProjectItem baseItem, string outputFileName)
		{
			return context.EnsureOutputFileIsInProject(baseItem, outputFileName);
		}
		
		public void ClearTasksExceptCommentTasks()
		{
			TaskService.ClearExceptCommentTasks();
		}
		
		public void AddTask(Task task)
		{
			TaskService.Add(task);
		}
		
		public void BringErrorsPadToFront()
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
		}
	}
}
