// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

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
	/// Opens the containing folder in the clipboard.
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
