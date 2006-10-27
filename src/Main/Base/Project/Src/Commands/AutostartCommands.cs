// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class StartWorkbenchCommand
	{
		const string workbenchMemento = "WorkbenchMemento";
		
		class FormKeyHandler : IMessageFilter
		{
			const int keyPressedMessage          = 0x100;
			
			void SelectActiveWorkbenchWindow()
			{
				if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
					if (!WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent.Control.ContainsFocus) {
						if (Form.ActiveForm == WorkbenchSingleton.MainForm) {
							WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent.Control.Focus();
						}
					}
				}
			}
			
			bool PadHasFocus()
			{
				foreach (PadDescriptor padContent in WorkbenchSingleton.Workbench.PadContentCollection) {
					if (padContent.HasFocus) {
						return true;
						
					}
				}
				return false;
			}
			string oldLayout = "Default";
			public bool PreFilterMessage(ref Message m)
			{
				if (m.Msg != keyPressedMessage) {
					return false;
				}
				Keys keyPressed = (Keys)m.WParam.ToInt32() | Control.ModifierKeys;
				
				if (keyPressed == Keys.Escape) {
					if (PadHasFocus() && !MenuService.IsContextMenuOpen) {
						SelectActiveWorkbenchWindow();
						return true;
					}
					return false;
				}
				
				if (keyPressed == (Keys.Escape | Keys.Shift)) {
					if (LayoutConfiguration.CurrentLayoutName == "Plain") {
						LayoutConfiguration.CurrentLayoutName = oldLayout;
					} else {
						WorkbenchSingleton.Workbench.WorkbenchLayout.StoreConfiguration();
						oldLayout = LayoutConfiguration.CurrentLayoutName;
						LayoutConfiguration.CurrentLayoutName = "Plain";
					}
					SelectActiveWorkbenchWindow();
					return true;
				}
				return false;
			}
		}
		
		public void Run(IList<string> fileList)
		{
			Form f = (Form)WorkbenchSingleton.Workbench;
			f.Show();
			
			bool didLoadCombineOrFile = false;
			
			foreach (string file in fileList) {
				didLoadCombineOrFile = true;
				try {
					IProjectLoader loader = ProjectService.GetProjectLoader(file);
					if (loader != null) {
						FileUtility.ObservedLoad(new NamedFileOperationDelegate(loader.Load), file);
					} else {
						FileService.OpenFile(file);
					}
				} catch (Exception e) {
					MessageService.ShowError(e, "unable to open file " + file);
				}
			}
			
			// load previous combine
			if (!didLoadCombineOrFile && PropertyService.Get("SharpDevelop.LoadPrevProjectOnStartup", false)) {
				if (FileService.RecentOpen.RecentProject.Count > 0) {
					ProjectService.LoadSolution(FileService.RecentOpen.RecentProject[0].ToString());
					didLoadCombineOrFile = true;
				}
			}
			
			if (!didLoadCombineOrFile) {
				foreach (ICommand command in AddInTree.BuildItems("/Workspace/AutostartNothingLoaded", null, false)) {
					command.Run();
				}
			}
			
			f.Focus(); // windows.forms focus workaround
			
			ParserService.StartParserThread();
			
			// finally run the workbench window ...
			Application.AddMessageFilter(new FormKeyHandler());
			Application.Run(f);
			
			// save the workbench memento in the ide properties
			try {
				PropertyService.Set(workbenchMemento, WorkbenchSingleton.Workbench.CreateMemento());
			} catch (Exception e) {
				MessageService.ShowError(e, "Exception while saving workbench state.");
			}
		}
	}
}
