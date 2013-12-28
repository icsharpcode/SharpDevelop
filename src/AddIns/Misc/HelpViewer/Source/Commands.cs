// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using MSHelpSystem.Controls;
using MSHelpSystem.Core;

namespace MSHelpSystem.Commands
{
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
					LoggingService.Debug(string.Format("HelpViewer: Getting description of \"{0}\"", code));
					if (Help3Environment.IsLocalHelp)
						DisplayHelp.Keywords(code);
					else
						DisplayHelp.ContextualHelp(code);
				} else {
					LoggingService.Error("HelpViewer: Help system ist not initialized");
				}
			}
		}
	}
	
	public class DisplayContent : AbstractMenuCommand
	{
		public override void Run()
		{
			if (!Help3Environment.IsHelp3ProtocolRegistered) {
				using (HelpLibraryManagerNotFoundForm form = new HelpLibraryManagerNotFoundForm()) {
					form.ShowDialog(WorkbenchSingleton.MainWin32Window);
				}
				return;
			}
			if (Help3Service.Config.ExternalHelp) DisplayHelp.Catalog();
			else {
				PadDescriptor toc = WorkbenchSingleton.Workbench.GetPad(typeof(Help3TocPad));
				if (toc != null) toc.BringPadToFront();
			}
		}
	}

	public class DisplaySearch : AbstractMenuCommand
	{
		public override void Run()
		{
			if (!Help3Environment.IsHelp3ProtocolRegistered) {
				using (HelpLibraryManagerNotFoundForm form = new HelpLibraryManagerNotFoundForm()) {
					form.ShowDialog(WorkbenchSingleton.MainWin32Window);
				}
				return;
			}
			PadDescriptor search = WorkbenchSingleton.Workbench.GetPad(typeof(Help3SearchPad));
			if (search != null) search.BringPadToFront();
		}
	}
	
	public class LaunchHelpLibraryManager : AbstractMenuCommand
	{
		public override void Run()
		{
			if (string.IsNullOrEmpty(HelpLibraryManager.Manager)) {
				using (HelpLibraryManagerNotFoundForm form = new HelpLibraryManagerNotFoundForm()) {
					form.ShowDialog(WorkbenchSingleton.MainWin32Window);
				}
				return;
			}
			HelpLibraryManager.Start();
		}
	}
}
