// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Profiler.AddIn.Views;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.BuildEngine;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	using ICSharpCode.Profiler.Controller;
	
	/// <summary>
	/// Description of RunProject
	/// </summary>
	public class ProfileProject : ProfilerMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			AbstractProject currentProj = ProjectService.CurrentProject as AbstractProject;
			
			string filePart = @"ProfilingSessions\Session" +
				DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) +
				".sdps";
			
			string path = Path.Combine(currentProj.Directory, filePart);
			
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			
			IProfilingDataWriter writer = new ProfilingDataSQLiteWriter(path);
			ProfilerRunner runner = CreateRunner(writer);
			
			if (runner != null) {
				runner.RunFinished += delegate {
					Action updater = () => {
						FileService.OpenFile(path);
						FileProjectItem file = new FileProjectItem(currentProj, ItemType.Content, "ProfilingSessions\\" + Path.GetFileName(path));
						ProjectService.AddProjectItem(currentProj, file);
						ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
						currentProj.Save();
					};
					
					WorkbenchSingleton.SafeThreadCall(updater);
				};
				
				runner.Run();
			}
		}
		
		static ProfilerRunner CreateRunner(IProfilingDataWriter writer)
		{
			AbstractProject currentProj = ProjectService.CurrentProject as AbstractProject;
			
			if (currentProj == null)
				return null;
			
			if (!currentProj.IsStartable) {
				if (MessageService.AskQuestion("This project cannot be started. Do you want to profile the solution's StartUp project instead?")) {
					currentProj = ProjectService.OpenSolution.StartupProject as AbstractProject;
					if (currentProj == null) {
						MessageService.ShowError("No startable project was found. Aborting ...");
						return null;
					}
				} else
					return null;
			}
			if (!File.Exists(currentProj.OutputAssemblyFullPath)) {
				MessageService.ShowError("This project cannot be started because the executable file was not found, " +
				                         "please ensure that the project and all its depencies are built correctly!");
				return null;
			}
			
			ProfilerRunner runner = new ProfilerRunner(currentProj.CreateStartInfo(), true, writer);
			return runner;
		}
	}
}
