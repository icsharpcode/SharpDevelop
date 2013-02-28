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
		WebProject webProject;
		
		public string StartProgram {
			get { return GetProjectProperty("StartProgram"); }
			set { SetProjectProperty("StartProgram", value); }
		}
		
		public string StartUrl {
			get { return GetProjectProperty("StartURL"); }
			set { SetProjectProperty("StartURL", value); }
		}
		
		public string StartArguments {
			get { return GetProjectProperty("StartArguments"); }
			set { SetProjectProperty("StartArguments", value); }
		}
		
		public string StartWorkingDirectory {
			get { return GetProjectProperty("StartWorkingDirectory"); }
			set { SetProjectProperty("StartWorkingDirectory", value); }
		}
		
		string GetProjectProperty(string name)
		{
			return MSBuildProject.GetEvaluatedProperty(name) ?? String.Empty;
		}
		
		void SetProjectProperty(string name, string value)
		{
			MSBuildProject.SetProperty(name, String.IsNullOrEmpty(value) ? null : value);
		}
		
		MSBuildBasedProject MSBuildProject {
			get { return (MSBuildBasedProject)Project; }
		}
		
		CompilableProject CompilableProject {
			get { return (CompilableProject)Project; }
		}
		
		WebProject WebProject {
			get {
				if (webProject == null) {
					webProject = new WebProject(MSBuildProject);
				}
				return webProject;
			}
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
			try {
				WebProjectProperties properties = WebProject.GetWebProjectProperties();
				if (CheckWebProjectStartInfo()) {
					AttachToWebWorkerProcessOrStartIISExpress(properties, withDebugging);
				}
				
				// start default application(e.g. browser) or the one specified
				switch (CompilableProject.StartAction) {
					case StartAction.Project:
						if (FileUtility.IsUrl(properties.IISUrl)) {
							Process.Start(properties.IISUrl);
						} else {
							MessageService.ShowError("${res:ICSharpCode.WebProjectOptionsPanel.NoProjectUrlOrProgramAction}");
							DisposeProcessMonitor();
						}
						break;
					case StartAction.Program:
						ProcessStartInfo processInfo = DotNetStartBehavior.CreateStartInfo(StartProgram, Project.Directory, StartWorkingDirectory, StartArguments);
						if (withDebugging) {
							DebuggerService.CurrentDebugger.Start(processInfo);
						} else {
							Process.Start(processInfo);
						}
						break;
					case StartAction.StartURL:
						if (FileUtility.IsUrl(StartUrl)) {
							Process.Start(StartUrl);
						} else {
							string url = string.Concat(properties.IISUrl, StartUrl);
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
						throw new Exception("Invalid value for StartAction");
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex.Message);
				LoggingService.Error(ex.ToString());
				DisposeProcessMonitor();
			}
		}
		
		void AttachToWebWorkerProcessOrStartIISExpress(WebProjectProperties properties, bool withDebugging)
		{
			string processName = WebProjectService.GetWorkerProcessName(properties);
			
			// try find the worker process directly or using the process monitor callback
			Process[] processes = System.Diagnostics.Process.GetProcesses();
			int index = processes.FindIndex(p => p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase));
			if (index > -1) {
				if (withDebugging)
					DebuggerService.CurrentDebugger.Attach(processes[index]);
			} else {
				if (properties.UseIISExpress) {
					// start IIS express and attach to it
					if (WebProjectService.IsIISExpressInstalled) {
						ProcessStartInfo processInfo = IISExpressProcessStartInfo.Create(WebProject);
						if (withDebugging)
							DebuggerService.CurrentDebugger.Start(processInfo);
						else
							Process.Start(processInfo);
					}
				} else {
					DisposeProcessMonitor();
					this.monitor = new ProcessMonitor(processName);
					this.monitor.ProcessCreated += delegate {
						WorkbenchSingleton.SafeThreadCall((Action)(() => OnProcessCreated(properties, withDebugging)));
					};
					this.monitor.Start();
				}
			}
		}
		
		bool CheckWebProjectStartInfo()
		{
			if (WebProject.HasWebProjectProperties() && WebProject.GetWebProjectProperties().IsConfigured()) {
				return true;
			}
			return false;
		}
		
		void OnProcessCreated(WebProjectProperties properties, bool withDebugging)
		{
			string processName = WebProjectService.GetWorkerProcessName(properties);
			Process[] processes = Process.GetProcesses();
			int index = processes.FindIndex(p => p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase));
			if (index == -1)
				return;
			if (withDebugging) {
				DebuggerService.CurrentDebugger.Attach(processes[index]);
				
				if (!DebuggerService.CurrentDebugger.IsAttached) {
					if(properties.UseIIS) {
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
	}
}
