// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
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
			foreach (IWorkbenchWindow window in Linq.ToArray(WorkbenchSingleton.Workbench.WorkbenchWindowCollection)) {
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
				if (window.ActiveViewContent.IsViewOnly) {
					return;
				}
				foreach (OpenedFile file in window.ActiveViewContent.Files) {
					SaveFile.Save(file);
				}
			}
		}
	}
	
	public class SaveFileAsTab : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			
			if (window != null) {
				if (window.ActiveViewContent.IsViewOnly) {
					return;
				}
				foreach (OpenedFile file in window.ActiveViewContent.Files) {
					SaveFileAs.Save(file);
				}
			}
		}
	}
	
	public class CopyPathName : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			ClipboardWrapper.SetText(window.ActiveViewContent.PrimaryFileName ?? "");
		}
	}
}
