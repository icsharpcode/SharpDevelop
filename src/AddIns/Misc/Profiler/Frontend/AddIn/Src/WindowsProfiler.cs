// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop.Profiling;
using ICSharpCode.SharpDevelop.Project;
using System.IO;

namespace ICSharpCode.Profiler.AddIn
{
	/// <summary>
	/// Description of WindowsProfiler.
	/// </summary>
	public class WindowsProfiler : IProfiler
	{
		ProfilerRunner runner;
		bool isRunning = false;
		
		public WindowsProfiler()
		{
		}
		
		public bool CanProfile(IProject project)
		{
			return project != null && project.IsStartable;
		}
		
		public void Start(ProcessStartInfo info, string outputPath, Action afterFinishAction)
		{
			this.runner = new ProfilerRunner(info, true, new ProfilingDataSQLiteWriter(outputPath));
			
			if (runner != null) {
				runner.RunFinished += delegate {
					try {
						 afterFinishAction(); 
					} finally {
						this.isRunning = false;
					}
				};
				this.isRunning = true;
				runner.Run();
			}
		}
		
		public static ProfilerRunner CreateRunner(IProfilingDataWriter writer)
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
		
		public void Dispose()
		{
		}
		
		public bool IsRunning {
			get {
				return this.isRunning;
			}
		}
		
		public void Stop()
		{
			this.runner.Stop();
		}
	}
}
