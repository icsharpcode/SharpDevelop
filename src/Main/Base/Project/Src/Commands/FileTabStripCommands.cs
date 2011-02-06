// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Commands.TabStrip
{
	public class CloseFileTab : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			if (window != null) {
				window.CloseWindow(false);
			}
		}
	}
	
	public class CloseAllButThisFileTab : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow thisWindow = Owner as IWorkbenchWindow;
			foreach (IWorkbenchWindow window in WorkbenchSingleton.Workbench.WorkbenchWindowCollection.ToArray()) {
				if (window != thisWindow) {
					if (!window.CloseWindow(false))
						break;
				}
			}
		}
	}
	
	public class SaveFileTab : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			if (window != null) {
				SaveFile.Save(window);
			}
		}
	}
	
	public class SaveFileAsTab : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			
			if (window != null) {
				SaveFileAs.Save(window);
			}
		}
	}
	
	/// <summary>
	/// Copies the path to the clipboard.
	/// </summary>
	public class CopyPathName : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			ClipboardWrapper.SetText(window.ActiveViewContent.PrimaryFileName ?? "");
		}
	}
	
	/// <summary>
	/// Expands and scrolls to this file in the project browser.
	/// </summary>
	public class NavigateToFileInProjectBrowser : AbstractMenuCommand
	{
		public override void Run()
		{
			var projectBrowser = (ProjectBrowserPad.Instance.Control as ProjectBrowserPanel).ProjectBrowserControl;
			var fileName = ((IWorkbenchWindow)Owner).ActiveViewContent.PrimaryFileName;
			if (fileName != null) {
				projectBrowser.SelectFileAndExpand(fileName);
				projectBrowser.Focus();
			}
		}
	}
	
	/// <summary>
	/// Opens the folder containing this file in the windows explorer.
	/// </summary>
	public class OpenFolderContainingFile : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			ICSharpCode.SharpDevelop.Project.Commands.OpenFolderContainingFile.OpenContainingFolderInExplorer(
				window.ActiveViewContent.PrimaryFileName);
		}
	}
}
