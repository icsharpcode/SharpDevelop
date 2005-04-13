// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
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
			IHelpProvider    helpProvider = window != null ? window.ActiveViewContent as IHelpProvider : null;
			foreach (PadDescriptor descriptor in WorkbenchSingleton.Workbench.PadContentCollection) {
				if (descriptor.HasFocus && descriptor.PadContent is IHelpProvider) {
					((IHelpProvider)descriptor.PadContent).ShowHelp();
					return;
				}
			}
			
			if (helpProvider != null) {
				helpProvider.ShowHelp();
			}
		}
	}
	
	public class ShowHelp : AbstractMenuCommand
	{
		public override void Run()
		{
			
			string fileName = FileUtility.SharpDevelopRootPath + 
			              Path.DirectorySeparatorChar + "doc" +
			              Path.DirectorySeparatorChar + "help" +
			              Path.DirectorySeparatorChar + "sharpdevelop.chm";
			if (FileUtility.TestFileExists(fileName)) {
				Help.ShowHelp((Form)WorkbenchSingleton.Workbench, fileName);
				((Form)WorkbenchSingleton.Workbench).Select();
			}
		}
	}
	
	public class ViewGPL : AbstractMenuCommand
	{
		public override void Run()
		{
			using (ViewGPLDialog totdd = new ViewGPLDialog()) {
				totdd.Owner = (Form)WorkbenchSingleton.Workbench;
				totdd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			}
		}
	}
	
	public class GotoWebSite : AbstractMenuCommand
	{
		string site;
		
		public GotoWebSite(string site)
		{
			this.site = site;
		}
		
		public override void Run()
		{
			
			FileService.OpenFile(site);
		}
	}
	
	public class GotoLink : AbstractMenuCommand
	{
		string site;
		
		public GotoLink(string site)
		{
			this.site = site;
		}
		
		public override void Run()
		{
			
			string file = site.StartsWith("home://") ? FileUtility.Combine(FileUtility.SharpDevelopRootPath, "bin", site.Substring(7).Replace('/', Path.DirectorySeparatorChar)) : site;
			try {
				Process.Start(file);
			} catch (Exception) {
				
				MessageService.ShowError("Can't execute/view " + file + "\n Please check that the file exists and that you can open this file.");
			}
		}
	}
	
	public class ViewTipOfTheDay : AbstractMenuCommand
	{
		public override void Run()
		{
			using (TipOfTheDayDialog totdd = new TipOfTheDayDialog()) {
				totdd.Owner = (Form)WorkbenchSingleton.Workbench;
				totdd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			}
		}
	}
	
	public class AboutSharpDevelop : AbstractMenuCommand
	{
		public override void Run()
		{
			using (CommonAboutDialog ad = new CommonAboutDialog()) {
				ad.Owner = (Form)WorkbenchSingleton.Workbench;
				ad.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			}
		}
	}
}
