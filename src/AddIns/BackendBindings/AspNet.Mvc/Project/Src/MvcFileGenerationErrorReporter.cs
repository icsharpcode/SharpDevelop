// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextTemplating;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcFileGenerationErrorReporter : IMvcFileGenerationErrorReporter
	{
		public void ShowErrors(CompilerErrorCollection errors)
		{
			TaskService.ClearExceptCommentTasks();
			foreach (CompilerError error in errors) {
				TaskService.Add(new CompilerErrorTask(error));
			}
			WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
		}
	}
}
