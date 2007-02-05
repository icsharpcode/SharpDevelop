// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class ShowSensitiveHelp : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window       = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			IContextHelpProvider    helpProvider = window != null ? window.ActiveViewContent as IContextHelpProvider : null;
			foreach (PadDescriptor descriptor in WorkbenchSingleton.Workbench.PadContentCollection) {
				if (descriptor.HasFocus && descriptor.PadContent is IContextHelpProvider) {
					((IContextHelpProvider)descriptor.PadContent).ShowHelp();
					return;
				}
			}
			
			if (helpProvider != null) {
				helpProvider.ShowHelp();
			}
		}
	}
	
	public class LinkCommand : AbstractMenuCommand
	{
		string site;
		
		public LinkCommand(string site)
		{
			this.site = site;
		}
		
		public override void Run()
		{
			if (site.StartsWith("home://")) {
				string file = Path.Combine(FileUtility.ApplicationRootPath, site.Substring(7).Replace('/', Path.DirectorySeparatorChar));
				try {
					Process.Start(file);
				} catch (Exception) {
					MessageService.ShowError("Can't execute/view " + file + "\n Please check that the file exists and that you can open this file.");
				}
			} else {
				FileService.OpenFile(site);
			}
		}
	}
	
	public class AboutSharpDevelop : AbstractMenuCommand
	{
		public override void Run()
		{
			using (CommonAboutDialog ad = new CommonAboutDialog()) {
				ad.Owner = WorkbenchSingleton.MainForm;
				ad.ShowDialog(WorkbenchSingleton.MainForm);
			}
		}
	}
}
