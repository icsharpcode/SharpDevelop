// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class WebBehavior : ProjectBehavior
	{
		ProcessMonitor monitor;
		
		public WebBehavior(MSBuildBasedProject project, ProjectBehavior next = null)
			: base(project, next)
		{
			
		}
		
		new MSBuildBasedProject Project {
			get {
				return (MSBuildBasedProject)base.Project;
			}
		}
		
		public string StartProgram {
			get {
				return ((MSBuildBasedProject)Project).GetEvaluatedProperty("StartProgram") ?? "";
			}
			set {
				((MSBuildBasedProject)Project).SetProperty("StartProgram", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		
		public string StartUrl {
			get {
				return ((MSBuildBasedProject)Project).GetEvaluatedProperty("StartURL") ?? "";
			}
			set {
				((MSBuildBasedProject)Project).SetProperty("StartURL", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		
		public const string LocalHost = "http://localhost";
		
		public override bool IsStartable {
			get { return true; }
		}
		
		public override ProcessStartInfo CreateStartInfo()
		{
			return new ProcessStartInfo(LocalHost);
		}
		
		public override void Start(bool withDebugging)
		{
			var processStartInfo = Project.CreateStartInfo();
			if (FileUtility.IsUrl(processStartInfo.FileName)) {
				if (!CheckWebProjectStartInfo())
					return;
				// we deal with a WebProject
				try {
					var project = ProjectService.OpenSolution.StartupProject as CompilableProject;
					WebProjectOptions options = WebProjectsOptions.Instance.GetWebProjectOptions(project.Name);
					System.Diagnostics.Process defaultAppProcess = null;
					
					string processName = WebProjectService.GetWorkerProcessName(options.Data.WebServer);
					
					// try find the worker process directly or using the process monitor callback
					var processes = System.Diagnostics.Process.GetProcesses();
					int index = processes.FindIndex(p => p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase));
					if (index > -1){
						if (withDebugging)
							DebuggerService.CurrentDebugger.Attach(processes[index]);
					} else {
						this.monitor = new ProcessMonitor(processName);
						this.monitor.ProcessCreated += delegate {
							WorkbenchSingleton.SafeThreadCall((Action)(() => OnProcessCreated(defaultAppProcess, options, withDebugging)));
						};
						this.monitor.Start();
						
						if (options.Data.WebServer == WebServer.IISExpress) {
							// start IIS express and attach to it
							if (WebProjectService.IsIISExpressInstalled) {
								defaultAppProcess = System.Diagnostics.Process.Start(WebProjectService.IISExpressProcessLocation);
							} else {
								DisposeProcessMonitor();
								MessageService.ShowError("${res:ICSharpCode.WepProjectOptionsPanel.NoProjectUrlOrProgramAction}");
								return;
							}
						}
					}
					
					// start default application(e.g. browser) or the one specified
					switch (project.StartAction) {
						case StartAction.Project:
							if (FileUtility.IsUrl(options.Data.ProjectUrl)) {
								defaultAppProcess = System.Diagnostics.Process.Start(options.Data.ProjectUrl);
							} else {
								MessageService.ShowError("${res:ICSharpCode.WepProjectOptionsPanel.NoProjectUrlOrProgramAction}");
								DisposeProcessMonitor();
							}
							break;
						case StartAction.Program:
							defaultAppProcess = System.Diagnostics.Process.Start(StartProgram);
							break;
						case StartAction.StartURL:
							if (FileUtility.IsUrl(StartUrl))
								defaultAppProcess = System.Diagnostics.Process.Start(StartUrl);
							else {
								string url = string.Concat(options.Data.ProjectUrl, StartUrl);
								if (FileUtility.IsUrl(url)) {
									defaultAppProcess = System.Diagnostics.Process.Start(url);
								} else {
									MessageService.ShowError("${res:ICSharpCode.WepProjectOptionsPanel.NoProjectUrlOrProgramAction}");
									DisposeProcessMonitor();
									return;
								}
							}
							break;
						default:
							throw new System.Exception("Invalid value for StartAction");
					}
				} catch (System.Exception ex) {
					string err = "Error: " + ex.Message;
					MessageService.ShowError(err);
					LoggingService.Error(err);
					DisposeProcessMonitor();
				}
			}
		}
		
		bool CheckWebProjectStartInfo()
		{
			// check if we have startup project
			var project = ProjectService.OpenSolution.StartupProject as CompilableProject;
			if (project == null) {
				MessageService.ShowError("${res:ICSharpCode.NoStartupProject}");
				return false;
			}
			
			// check if we have options
			if (WebProjectsOptions.Instance == null) {
				MessageService.ShowError("${res:ICSharpCode.WepProjectOptionsPanel.NoProjectUrlOrProgramAction}");
				return false;
			}
			
			// check the options
			var options = WebProjectsOptions.Instance.GetWebProjectOptions(project.Name);
			if (options == null || options.Data == null || string.IsNullOrEmpty(options.ProjectName) ||
			    options.Data.WebServer == WebServer.None) {
				MessageService.ShowError("${res:ICSharpCode.WepProjectOptionsPanel.NoProjectUrlOrProgramAction}");
				return false;
			}
			
			return true;
		}
		
		void OnProcessCreated(Process defaultAppProcess, WebProjectOptions options, bool withDebugging)
		{
			if (defaultAppProcess == null)
				return;
			string processName = WebProjectService.GetWorkerProcessName(options.Data.WebServer);
			var processes = Process.GetProcesses();
			int index = processes.FindIndex(p => p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase));
			if (index == -1)
				return;
			if (withDebugging) {
				DebuggerService.CurrentDebugger.Attach(processes[index]);
				
				if (!DebuggerService.CurrentDebugger.IsAttached) {
					if(options.Data.WebServer == WebServer.IIS) {
						string format = ResourceService.GetString("ICSharpCode.WepProjectOptionsPanel.NoIISWP");
						MessageService.ShowMessage(string.Format(format, processName));
					} else {
						DebuggerService.CurrentDebugger.Attach(defaultAppProcess);
						if (!DebuggerService.CurrentDebugger.IsAttached) {
							MessageService.ShowMessage(ResourceService.GetString("ICSharpCode.WepProjectOptionsPanel.UnableToAttach"));
						}
					}
				}
			}
		}
		
		void DisposeProcessMonitor()
		{
			if (monitor != null) {
				monitor.Stop();
				monitor.Dispose();
				monitor = null;
			}
		}
		
		public override ICSharpCode.Core.Properties CreateMemento()
		{
			var properties = base.CreateMemento();
			// web project properties
			var webOptions = WebProjectsOptions.Instance.GetWebProjectOptions(Project.Name);
			if (webOptions != null)
				properties.Set("WebProjectOptions", webOptions);
			return properties;
		}
		
		public override void SetMemento(ICSharpCode.Core.Properties memento)
		{
			// web project properties
			WebProjectsOptions.Instance.SetWebProjectOptions(Project.Name, memento.Get("WebProjectOptions", new WebProjectOptions()) as WebProjectOptions);
			base.SetMemento(memento);
		}
	}
}
