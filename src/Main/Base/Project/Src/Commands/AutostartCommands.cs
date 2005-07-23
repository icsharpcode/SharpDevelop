// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Runtime.Remoting;
using System.Security.Policy;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class InitializeWorkbenchCommand : AbstractCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.InitializeWorkbench();
		}
	}
	
	public class StartWorkbenchCommand // : AbstractCommand
	{
		const string workbenchMemento = "WorkbenchMemento";
		
		/// <remarks>
		/// The worst workaround in the whole project
		/// </remarks>
		void ShowTipOfTheDay(object sender, EventArgs e)
		{
			Application.Idle -= ShowTipOfTheDay;
			
			// show tip of the day
			if (PropertyService.Get("ShowTipsAtStartup", true)) {
				ViewTipOfTheDay dview = new ViewTipOfTheDay();
				dview.Run();
			}
		}
		
		class FormKeyHandler : IMessageFilter
		{
			const int keyPressedMessage          = 0x100;
			
			void SelectActiveWorkbenchWindow()
			{
				if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
					if (!WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent.Control.ContainsFocus) {
						if (Form.ActiveForm == (Form)WorkbenchSingleton.Workbench) {
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
					if (PadHasFocus()) {
						SelectActiveWorkbenchWindow();
						return true;
					}
					return false;
				}
				
				if (keyPressed == (Keys.Escape | Keys.Shift)) {
					if (LayoutConfiguration.CurrentLayoutName == "Plain") {
						LayoutConfiguration.CurrentLayoutName = oldLayout;
					} else {
						oldLayout = LayoutConfiguration.CurrentLayoutName;
						LayoutConfiguration.CurrentLayoutName = "Plain";
					}
					SelectActiveWorkbenchWindow();
					return true;
				}
				return false;
			}
		}
		
		public void Run(string[] fileList)
		{
			Form f = (Form)WorkbenchSingleton.Workbench;
			f.Show();
			
			Application.Idle += ShowTipOfTheDay;
			
			bool didLoadCombineOrFile = false;
			
			foreach (string file in fileList) {
				didLoadCombineOrFile = true;
				switch (Path.GetExtension(file).ToUpper()) {
					case ".CMBX":
					case ".PRJX":
					case ".SLN":
					case ".CSPROJ":
					case ".VBPROJ":
						FileUtility.ObservedLoad(new NamedFileOperationDelegate(ProjectService.LoadSolution), file);
						break;
					default:
						try {
							FileService.OpenFile(file);
						} catch (Exception e) {
							MessageService.ShowError(e, "unable to open file " + file);
						}
						break;
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
			
			ParserService.StopParserThread();
			
			// save the workbench memento in the ide properties
			try {
				PropertyService.Set(workbenchMemento, WorkbenchSingleton.Workbench.CreateMemento());
			} catch (Exception e) {
				MessageService.ShowError(e, "Exception while saving workbench state.");
			}
		}
	}
}
