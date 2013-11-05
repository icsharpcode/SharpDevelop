// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using MSHelpSystem.Controls;
using MSHelpSystem.Core;

namespace MSHelpSystem.Commands
{
	public class ShowErrorHelpCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			var view = (System.Windows.Controls.ListView)Owner;
			foreach (var t in view.SelectedItems.OfType<SDTask>().ToArray()) {
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
					form.ShowDialog(SD.WinForms.MainWin32Window);
				}
				return;
			}
			if (Help3Service.Config.ExternalHelp) DisplayHelp.Catalog();
			else {
				PadDescriptor toc = SD.Workbench.GetPad(typeof(Help3TocPad));
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
					form.ShowDialog(SD.WinForms.MainWin32Window);
				}
				return;
			}
			PadDescriptor search = SD.Workbench.GetPad(typeof(Help3SearchPad));
			if (search != null) search.BringPadToFront();
		}
	}
	
	public class LaunchHelpLibraryManager : AbstractMenuCommand
	{
		public override void Run()
		{
			if (string.IsNullOrEmpty(HelpLibraryManager.Manager)) {
				using (HelpLibraryManagerNotFoundForm form = new HelpLibraryManagerNotFoundForm()) {
					form.ShowDialog(SD.WinForms.MainWin32Window);
				}
				return;
			}
			HelpLibraryManager.Start();
		}
	}
}
