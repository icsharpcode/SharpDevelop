// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using ICSharpCode.AvalonEdit;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageManagementConsoleHost : IPackageManagementConsoleHost
	{
		IThread thread;
		IPowerShellHostFactory powerShellHostFactory;
		IPackageManagementProjectService projectService;
		IPowerShellHost powerShellHost;
		IPackageManagementAddInPath addinPath;
		int autoIndentSize = 0;
		string prompt = "PM> ";
		Version nuGetVersion;
		
		public PackageManagementConsoleHost(
			IPowerShellHostFactory powerShellHostFactory,
			IPackageManagementProjectService projectService,
			IPackageManagementAddInPath addinPath)
		{
			this.powerShellHostFactory = powerShellHostFactory;
			this.projectService = projectService;
			this.addinPath = addinPath;
		}
		
		public PackageManagementConsoleHost()
			: this(
				new PowerShellHostFactory(),
				new PackageManagementProjectService(),
				new PackageManagementAddInPath())
		{
		}
		
		public IProject DefaultProject { get; set; }
		public PackageSource ActivePackageSource { get; set; }
		public IScriptingConsole ScriptingConsole { get; set; }
		
		public IPackageManagementProjectService ProjectService {
			get { return projectService; }
		}
		
		public void Dispose()
		{
			if (ScriptingConsole != null) {
				ScriptingConsole.Dispose();
			}
			
			if (thread != null) {
				thread.Join();
				thread = null;
			}
		}
		
		public void Clear()
		{
		}
		
		public void Run()
		{
			thread = CreateThread(RunSynchronous);
			thread.Start();
		}
		
		protected virtual IThread CreateThread(ThreadStart threadStart)
		{
			return new PackageManagementThread(threadStart);
		}
		
		void RunSynchronous()
		{
			InitPowerShell();
			WriteInfoBeforeFirstPrompt();
			WritePrompt();
			ProcessUserCommands();
		}
		
		void InitPowerShell()
		{
			CreatePowerShellHost();
			AddModulesToImport();
			powerShellHost.SetRemoteSignedExecutionPolicy();
			UpdateFormatting();
		}
		
		void UpdateFormatting()
		{
			IEnumerable<string> fileNames = addinPath.GetPowerShellFormattingFileNames();
			powerShellHost.UpdateFormatting(fileNames);
		}
		
		void CreatePowerShellHost()
		{
			powerShellHost = powerShellHostFactory.CreatePowerShellHost(ScriptingConsole);
		}
		
		void AddModulesToImport()
		{
			string module = addinPath.CmdletsAssemblyFileName;
			powerShellHost.ModulesToImport.Add(module);
		}
		
		void WriteInfoBeforeFirstPrompt()
		{
			WriteNuGetVersionInfo();
			WriteHelpInfo();
			WriteLine();
		}
		
		void WriteNuGetVersionInfo()
		{
			string versionInfo = String.Format("NuGet {0}", GetNuGetVersion());
			WriteLine(versionInfo);
		}
		
		protected virtual Version GetNuGetVersion()
		{
			if (nuGetVersion == null) {
				AssemblyName name = typeof(PackageSource).Assembly.GetName();
				nuGetVersion = name.Version;
			}
			return nuGetVersion;
		}
		
		void WriteLine(string message)
		{
			ScriptingConsole.WriteLine(message, ScriptingStyle.Out);
		}
		
		void WriteLine()
		{
			WriteLine(String.Empty);
		}
		
		void WriteHelpInfo()
		{
			string helpInfo = GetHelpInfo();
			WriteLine(helpInfo);
		}
		
		protected virtual string GetHelpInfo()
		{
			return "Type 'get-help NuGet' for more information.";
		}
		
		void WritePrompt()
		{
			ScriptingConsole.Write(prompt, ScriptingStyle.Prompt);
		}
		
		void ProcessUserCommands()
		{
			while (true) {
				string line = ScriptingConsole.ReadLine(autoIndentSize);
				if (line != null) {
					ProcessLine(line);
					WritePrompt();
				} else {
					break;
				}
			}
		}
	
		void ProcessLine(string line)
		{
			powerShellHost.ExecuteCommand(line);
		}
		
		public MSBuildBasedProject GetProject(string name)
		{
			foreach (IProject project in projectService.GetOpenProjects()) {
				if (IsProjectNameMatch(project.Name, name)) {
					return project as MSBuildBasedProject;
				}
			}
			return null;
		}
		
		bool IsProjectNameMatch(string a, string b)
		{
			return String.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
