// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Win32;
using MSHelpSystem.Core;
using MSHelpSystem.Controls;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

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
					LoggingService.Debug(string.Format("Help 3.0: Getting description of \"{0}\"", code));
					if (Help3Environment.IsLocalHelp)
						DisplayHelp.Keywords(code);
					else
						DisplayHelp.ContextualHelp(code);
				} else {
					LoggingService.Error("Help 3.0: Help system ist not initialized");
				}
			}
		}
	}
	
	public class DisplayContent : AbstractMenuCommand
	{
		public override void Run()
		{
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
			PadDescriptor search = WorkbenchSingleton.Workbench.GetPad(typeof(Help3SearchPad));
			if (search != null) search.BringPadToFront();
		}
	}
	
	public class LaunchHelpLibraryManager : AbstractMenuCommand
	{
		public override void Run()
		{
			string path;
			if (!RegistryService.GetRegistryValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Help\v1.0",
			                                      "AppRoot", RegistryValueKind.String, out path)) {
				MessageService.ShowError("${res:AddIns.HelpViewer.HLMNotFound}");
				return;
			}
			path = Path.Combine(path, "HelpLibManager.exe");
			if (!File.Exists(path)) {
				MessageService.ShowError("${res:AddIns.HelpViewer.HLMNotFound}");
				return;
			}
			if (string.IsNullOrEmpty(Help3Service.Config.ActiveCatalogId)) {
				MessageService.ShowError("${res:AddIns.HelpViewer.HLMNoActiveCatalogError}");
				return;
			}
			Process.Start(path, string.Format("/product {0} /version {1} /locale {2}", Help3Service.Config.ActiveCatalogId.Split('/')));
		}
	}
}
