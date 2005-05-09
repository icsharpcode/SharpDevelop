// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
	
	public class StartWorkbenchCommand : AbstractCommand
	{
		const string workbenchMemento = "WorkbenchMemento";
		
		EventHandler idleEventHandler;
		bool isCalled = false;
		
		/// <remarks>
		/// The worst workaround in the whole project
		/// </remarks>
		void ShowTipOfTheDay(object sender, EventArgs e)
		{
			if (isCalled) {
				Application.Idle -= idleEventHandler;
				return;
			}
			isCalled = true;
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

		
		public override void Run()
		{
			Form f = (Form)WorkbenchSingleton.Workbench;
			f.Show();
			
			idleEventHandler = new EventHandler(ShowTipOfTheDay);
			Application.Idle += idleEventHandler;
			
			/*
			bool didLoadCombineOrFile = false;
			
			foreach (string file in SplashScreenForm.GetRequestedFileList()) {
				didLoadCombineOrFile = true;
				switch (System.IO.Path.GetExtension(file).ToUpper()) {
					case ".CMBX":
					case ".PRJX":
						FileUtility.ObservedLoad(new NamedFileOperationDelegate(ProjectService.LoadSolution), file);
						break;
					default:
						try {
							FileService.OpenFile(file);
						} catch (Exception e) {
							Console.WriteLine("unable to open file {0} exception was :\n{1}", file, e.ToString());
						}
						break;
				}
			}
			*/
			// !didLoadCombineOrFile && 
			// load previous combine
			if ((bool)PropertyService.Get("SharpDevelop.LoadPrevProjectOnStartup", false)) {
				object recentOpenObj = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.MainWindow.RecentOpen");
				if (recentOpenObj is ICSharpCode.Core.RecentOpen) {
					ICSharpCode.Core.RecentOpen recOpen = (ICSharpCode.Core.RecentOpen)recentOpenObj;
					if (recOpen.RecentProject.Count > 0) { 
						ProjectService.LoadSolution(recOpen.RecentProject[0].ToString());
					}
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
				Console.WriteLine("Exception while saving workbench state: " + e.ToString());
			}
		}
	}
}
