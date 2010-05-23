using System;
using System.Collections.Generic;
using MSHelpSystem.Core;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace MSHelpSystem.Commands
{
	// <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
	// <version>$Revision: 3555 $</version>
	public class ShowErrorHelpCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ICSharpCode.SharpDevelop.Gui.TaskView view = (TaskView)Owner;

			foreach (Task t in new List<Task>(view.SelectedTasks)) {
				if (t.BuildError == null)
					continue;

				string code = t.BuildError.ErrorCode;
				if (string.IsNullOrEmpty(code))
					return;

				if (Help3Environment.IsHelp3ProtocolRegistered) {
					LoggingService.Debug(string.Format("Help 3.0: Getting description of \"{0}\"", code));
					if (Help3Environment.IsLocalHelp)
						DisplayHelp.Keywords(code);
					else
						DisplayHelp.ContextualHelp(code);
				}
				else {
					LoggingService.Error("Help 3.0: Help system ist not initialized");
				}
			}
		}
	}
}
