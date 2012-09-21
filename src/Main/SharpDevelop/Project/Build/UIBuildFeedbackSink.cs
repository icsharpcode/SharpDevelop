// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// This error message sink is used for GUI builds.
	/// </summary>
	sealed class UIBuildFeedbackSink : IBuildFeedbackSink
	{
		MessageViewCategory messageView;
		IStatusBarService statusBarService;
		
		public UIBuildFeedbackSink(MessageViewCategory messageView, IStatusBarService statusBarService)
		{
			Debug.Assert(messageView != null);
			Debug.Assert(statusBarService != null);
			
			this.messageView = messageView;
			this.statusBarService = statusBarService;
		}
		
		public void ReportError(BuildError error)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					TaskService.Add(new SDTask(error));
				});
		}
		
		public void ReportMessage(string message)
		{
			messageView.AppendLine(message);
		}
	}
}
