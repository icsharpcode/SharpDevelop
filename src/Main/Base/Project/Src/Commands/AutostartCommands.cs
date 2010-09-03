// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Interop;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Commands
{
	/// <summary>
	/// Runs workbench initialization.
	/// Is called by ICSharpCode.SharpDevelop.Sda and should not be called manually!
	/// </summary>
	public class WorkbenchStartup
	{
		const string workbenchMemento = "WorkbenchMemento";
		App app;
		
		public void InitializeWorkbench()
		{
			app = new App();
			System.Windows.Forms.Integration.WindowsFormsHost.EnableWindowsFormsInterop();
			ComponentDispatcher.ThreadIdle -= ComponentDispatcher_ThreadIdle; // ensure we don't register twice
			ComponentDispatcher.ThreadIdle += ComponentDispatcher_ThreadIdle;
			WorkbenchSingleton.InitializeWorkbench(new WpfWorkbench(), new AvalonDockLayout());
		}
		
		static void ComponentDispatcher_ThreadIdle(object sender, EventArgs e)
		{
			System.Windows.Forms.Application.RaiseIdle(e);
		}
		
		public void Run(IList<string> fileList)
		{
			bool didLoadSolutionOrFile = false;
			
			NavigationService.SuspendLogging();
			
			foreach (string file in fileList) {
				LoggingService.Info("Open file " + file);
				didLoadSolutionOrFile = true;
				try {
					string fullFileName = Path.GetFullPath(file);
					
					IProjectLoader loader = ProjectService.GetProjectLoader(fullFileName);
					if (loader != null) {
						loader.Load(fullFileName);
					} else {
						FileService.OpenFile(fullFileName);
					}
				} catch (Exception e) {
					MessageService.ShowException(e, "unable to open file " + file);
				}
			}
			
			// load previous solution
			if (!didLoadSolutionOrFile && PropertyService.Get("SharpDevelop.LoadPrevProjectOnStartup", false)) {
				if (FileService.RecentOpen.RecentProject.Count > 0) {
					ProjectService.LoadSolution(FileService.RecentOpen.RecentProject[0]);
					didLoadSolutionOrFile = true;
				}
			}
			
			if (!didLoadSolutionOrFile) {
				foreach (ICommand command in AddInTree.BuildItems<ICommand>("/Workspace/AutostartNothingLoaded", null, false)) {
					try {
						command.Run();
					} catch (Exception ex) {
						MessageService.ShowException(ex);
					}
				}
			}
			
			NavigationService.ResumeLogging();
			
			ParserService.StartParserThread();
			
			// finally run the workbench window ...
			app.Run(WorkbenchSingleton.MainWindow);
			
			// save the workbench memento in the ide properties
			try {
				PropertyService.Set(workbenchMemento, WorkbenchSingleton.Workbench.CreateMemento());
			} catch (Exception e) {
				MessageService.ShowException(e, "Exception while saving workbench state.");
			}
		}
	}
}
