// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
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
		IPackageManagementSolution solution;
		IRegisteredPackageRepositories registeredRepositories;
		IPowerShellHostFactory powerShellHostFactory;
		IPackageManagementProjectService projectService;
		IPowerShellHost powerShellHost;
		IPackageManagementAddInPath addinPath;
		int autoIndentSize = 0;
		string prompt = "PM> ";
		
		public PackageManagementConsoleHost(
			IPackageManagementSolution solution,
			IRegisteredPackageRepositories registeredRepositories,
			IPowerShellHostFactory powerShellHostFactory,
			IPackageManagementProjectService projectService,
			IPackageManagementAddInPath addinPath)
		{
			this.solution = solution;
			this.registeredRepositories = registeredRepositories;
			this.powerShellHostFactory = powerShellHostFactory;
			this.projectService = projectService;
			this.addinPath = addinPath;
		}
		
		public PackageManagementConsoleHost(
			IPackageManagementSolution solution,
			IRegisteredPackageRepositories registeredRepositories)
			: this(
				solution,
				registeredRepositories,
				new PowerShellHostFactory(),
				new PackageManagementProjectService(),
				new PackageManagementAddInPath())
		{
		}
		
		public IProject DefaultProject { get; set; }
		
		public PackageSource ActivePackageSource {
			get { return registeredRepositories.ActivePackageSource; }
			set { registeredRepositories.ActivePackageSource = value; }
		}
		
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
			string versionInfo = String.Format("NuGet {0}", powerShellHost.Version);
			WriteLine(versionInfo);
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
		
		public IPackageManagementProject GetProject(string packageSource, string projectName)
		{
			PackageSource source = GetActivePackageSource(packageSource);
			projectName = GetActiveProjectName(projectName);
			
			return solution.GetProject(source, projectName);
		}
		
		public PackageSource GetActivePackageSource(string source)
		{
			if (source != null) {
				return new PackageSource(source);
			}
			return ActivePackageSource;
		}
		
		string GetActiveProjectName(string projectName)
		{
			if (projectName != null) {
				return projectName;
			}
			return DefaultProject.Name;
		}
		
		public IPackageManagementProject GetProject(IPackageRepository sourceRepository, string projectName)
		{
			projectName = GetActiveProjectName(projectName);
			return solution.GetProject(sourceRepository, projectName);
		}
		
		public IEnumerable<IProject> GetOpenProjects()
		{
			return projectService.GetOpenProjects();
		}
	}
}
