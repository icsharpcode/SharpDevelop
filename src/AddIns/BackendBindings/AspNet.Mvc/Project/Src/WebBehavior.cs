// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
							SD.Debugger.Start(processInfo);
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
		
		int LastStartedIISExpressProcessId = -1;
		
		void AttachToWebWorkerProcessOrStartIISExpress(WebProjectProperties properties, bool withDebugging)
		{
			string processName = WebProjectService.GetWorkerProcessName(properties);
			
			// try find the worker process directly or using the process monitor callback
			Process[] processes = System.Diagnostics.Process.GetProcesses();
			int index = Array.FindIndex(processes, p => properties.UseIISExpress ? p.Id == LastStartedIISExpressProcessId : p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase));
			if (index > -1) {
				if (withDebugging)
					SD.Debugger.Attach(processes[index]);
			} else {
				if (properties.UseIISExpress) {
					// start IIS express and attach to it
					if (WebProjectService.IsIISExpressInstalled) {
						ProcessStartInfo processInfo = IISExpressProcessStartInfo.Create(WebProject);
						if (withDebugging) {
							SD.Debugger.Start(processInfo);
						} else {
							var process = Process.Start(processInfo);
							LastStartedIISExpressProcessId = process.Id;
						}
					}
				} else {
					DisposeProcessMonitor();
					this.monitor = new ProcessMonitor(processName);
					this.monitor.ProcessCreated += delegate {
						SD.MainThread.InvokeAsyncAndForget(() => OnProcessCreated(properties, withDebugging));
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
			int index = Array.FindIndex(processes, p => p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase));
			if (index == -1)
				return;
			if (withDebugging) {
				SD.Debugger.Attach(processes[index]);
				
				if (!SD.Debugger.IsAttached) {
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
