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
		public const string LocalHost = "http://localhost";
		
		ProcessMonitor monitor;
		
		public string StartProgram {
			get { return GetProjectProperty("StartProgram"); }
			set { SetProjectProperty("StartProgram", value); }
		}
		
		public string StartUrl {
			get { return GetProjectProperty("StartURL"); }
			set { SetProjectProperty("StartURL", value); }
		}
		
		string GetProjectProperty(string name)
		{
			return MSBuildProject.GetEvaluatedProperty(name) ?? String.Empty;
		}
		
		void SetProjectProperty(string name, string value)
		{
			MSBuildProject.SetProperty("StartProgram", String.IsNullOrEmpty(value) ? null : value);
		}
		
		MSBuildBasedProject MSBuildProject {
			get { return (MSBuildBasedProject)Project; }
		}
		
		public override bool IsStartable {
			get { return true; }
		}
		
		public override ProcessStartInfo CreateStartInfo()
		{
			return new ProcessStartInfo(LocalHost);
		}
		
		public override void Start(bool withDebugging)
		{
			ProcessStartInfo processStartInfo = MSBuildProject.CreateStartInfo();
			if (FileUtility.IsUrl(processStartInfo.FileName)) {
				if (!CheckWebProjectStartInfo())
					return;
				// we deal with a WebProject
				try {
					var project = ProjectService.OpenSolution.StartupProject as CompilableProject;
					WebProjectOptions options = WebProjectsOptions.Instance.GetWebProjectOptions(project.Name);
					
					string processName = WebProjectService.GetWorkerProcessName(options.Data.WebServer);
					
					// try find the worker process directly or using the process monitor callback
					Process[] processes = System.Diagnostics.Process.GetProcesses();
					int index = processes.FindIndex(p => p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase));
					if (index > -1) {
						if (withDebugging)
							DebuggerService.CurrentDebugger.Attach(processes[index]);
					} else {
						if (options.Data.WebServer == WebServer.IISExpress) {
							// start IIS express and attach to it
							if (WebProjectService.IsIISExpressInstalled) {
								DebuggerService.CurrentDebugger.Start(new ProcessStartInfo(WebProjectService.IISExpressProcessLocation));
							} else {
								DisposeProcessMonitor();
								MessageService.ShowError("${res:ICSharpCode.WepProjectOptionsPanel.NoProjectUrlOrProgramAction}");
								return;
							}
						} else {
							DisposeProcessMonitor();
							this.monitor = new ProcessMonitor(processName);
							this.monitor.ProcessCreated += delegate {
								WorkbenchSingleton.SafeThreadCall((Action)(() => OnProcessCreated(options, withDebugging)));
							};
							this.monitor.Start();
						}
					}
					
					// start default application(e.g. browser) or the one specified
					switch (project.StartAction) {
						case StartAction.Project:
							if (FileUtility.IsUrl(options.Data.ProjectUrl)) {
								Process.Start(options.Data.ProjectUrl);
							} else {
								MessageService.ShowError("${res:ICSharpCode.WebProjectOptionsPanel.NoProjectUrlOrProgramAction}");
								DisposeProcessMonitor();
							}
							break;
						case StartAction.Program:
							Process.Start(StartProgram);
							break;
						case StartAction.StartURL:
							if (FileUtility.IsUrl(StartUrl)) {
								Process.Start(StartUrl);
							} else {
								string url = string.Concat(options.Data.ProjectUrl, StartUrl);
								if (FileUtility.IsUrl(url)) {
									Process.Start(url);
								} else {
									MessageService.ShowError("${res:ICSharpCode.WebProjectOptionsPanel.NoProjectUrlOrProgramAction}");
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
				MessageService.ShowError("${res:ICSharpCode.WebProjectOptionsPanel.NoProjectUrlOrProgramAction}");
				return false;
			}
			
			// check the options
			WebProjectOptions options = WebProjectsOptions.Instance.GetWebProjectOptions(project.Name);
			if (options == null || options.Data == null || string.IsNullOrEmpty(options.ProjectName) ||
			    options.Data.WebServer == WebServer.None) {
				MessageService.ShowError("${res:ICSharpCode.WebProjectOptionsPanel.NoProjectUrlOrProgramAction}");
				return false;
			}
			
			return true;
		}
		
		void OnProcessCreated(WebProjectOptions options, bool withDebugging)
		{
			string processName = WebProjectService.GetWorkerProcessName(options.Data.WebServer);
			Process[] processes = Process.GetProcesses();
			int index = processes.FindIndex(p => p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase));
			if (index == -1)
				return;
			if (withDebugging) {
				DebuggerService.CurrentDebugger.Attach(processes[index]);
				
				if (!DebuggerService.CurrentDebugger.IsAttached) {
					if(options.Data.WebServer == WebServer.IIS) {
						string format = ResourceService.GetString("ICSharpCode.WebProjectOptionsPanel.NoIISWP");
						MessageService.ShowMessage(string.Format(format, processName));
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
		
		public override Properties CreateMemento()
		{
			Properties properties = base.CreateMemento();
			WebProjectOptions webOptions = WebProjectsOptions.Instance.GetWebProjectOptions(Project.Name);
			if (webOptions != null)
				properties.Set("WebProjectOptions", webOptions);
			return properties;
		}
		
		public override void SetMemento(Properties memento)
		{
			// web project properties
			WebProjectsOptions.Instance.SetWebProjectOptions(Project.Name, memento.Get("WebProjectOptions", new WebProjectOptions()) as WebProjectOptions);
			base.SetMemento(memento);
		}
	}
}
