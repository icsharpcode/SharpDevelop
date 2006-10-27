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
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			IViewContent lastContent = null;
			for (int i = 0 ; i < WorkbenchSingleton.Workbench.ViewContentCollection.Count;) {
				IViewContent content = WorkbenchSingleton.Workbench.ViewContentCollection[i];
				if (content.WorkbenchWindow != window && content != lastContent) {
					content.WorkbenchWindow.CloseWindow(false);
					lastContent = content;
				} else {
					++i;
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
				if (window.ViewContent.IsViewOnly) {
					return;
				}
				if (window.ViewContent.IsUntitled) {
					SaveFileAsTab.SaveFileAs(window);
				} else {
					
					ProjectService.MarkFileDirty(window.ViewContent.FileName);
					
					
					FileUtility.ObservedSave(new FileOperationDelegate(window.ViewContent.Save), window.ViewContent.FileName);
				}
			}
		}
	}
	
	public class SaveFileAsTab : AbstractMenuCommand
	{
		public static void SaveFileAs(IWorkbenchWindow window)
		{
			using (SaveFileDialog fdiag = new SaveFileDialog()) {
				fdiag.OverwritePrompt = true;
				fdiag.AddExtension    = true;
				
			 	fdiag.Filter          = String.Join("|", (string[])(AddInTree.GetTreeNode("/SharpDevelop/Workbench/FileFilter").BuildChildItems(null)).ToArray(typeof(string)));
				
				string[] fileFilters  = (string[])(AddInTree.GetTreeNode("/SharpDevelop/Workbench/FileFilter").BuildChildItems(null)).ToArray(typeof(string));
				fdiag.Filter          = String.Join("|", fileFilters);
				for (int i = 0; i < fileFilters.Length; ++i) {
					if (fileFilters[i].IndexOf(Path.GetExtension(window.ViewContent.FileName == null ? window.ViewContent.UntitledName : window.ViewContent.FileName)) >= 0) {
						fdiag.FilterIndex = i + 1;
						break;
					}
				}
				
				if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					string fileName = fdiag.FileName;
					// currently useless, because the fdiag.FileName can't
					// handle wildcard extensions :(
					if (Path.GetExtension(fileName).StartsWith("?") || Path.GetExtension(fileName) == "*") {
						fileName = Path.ChangeExtension(fileName, "");
					}
					
					window.ViewContent.Save(fileName);
					
					MessageService.ShowMessage(fileName, "${res:ICSharpCode.SharpDevelop.Commands.SaveFile.FileSaved}");
				}
			}
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			
			if (window != null) {
				if (window.ViewContent.IsViewOnly) {
					return;
				}
				SaveFileAs(window);
			}
		}
	}
	
	public class CopyPathName : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			if (window != null && window.ViewContent.FileName != null) {
				ClipboardWrapper.SetText(window.ViewContent.FileName);
			}
		}
	}
}
