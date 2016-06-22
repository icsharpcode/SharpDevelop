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
using System.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageManagementConsoleHost : IPackageManagementConsoleHost
	{
		IThread thread;
		IRegisteredPackageRepositories registeredRepositories;
		IPowerShellHostFactory powerShellHostFactory;
		IPowerShellHost powerShellHost;
		IPackageManagementAddInPath addinPath;
		IPackageManagementEvents packageEvents;
		int autoIndentSize = 0;
		string prompt = "PM> ";
		
		public PackageManagementConsoleHost(
			IPackageManagementSolution solution,
			IRegisteredPackageRepositories registeredRepositories,
			IPackageManagementEvents packageEvents,
			IPowerShellHostFactory powerShellHostFactory,
			IPackageManagementAddInPath addinPath)
		{
			this.Solution = solution;
			this.registeredRepositories = registeredRepositories;
			this.powerShellHostFactory = powerShellHostFactory;
			this.addinPath = addinPath;
			this.packageEvents = packageEvents;
		}
		
		public PackageManagementConsoleHost(
			IPackageManagementSolution solution,
			IRegisteredPackageRepositories registeredRepositories,
			IPackageManagementEvents packageEvents)
			: this(
				solution,
				registeredRepositories,
				packageEvents,
				new PowerShellHostFactory(),
				new PackageManagementAddInPath())
		{
		}
		
		public bool IsRunning { get; private set; }
		public IProject DefaultProject { get; set; }
		
		public PackageSource ActivePackageSource {
			get { return registeredRepositories.ActivePackageSource; }
			set { registeredRepositories.ActivePackageSource = value; }
		}
		
		public IScriptingConsole ScriptingConsole { get; set; }
		public IPackageManagementSolution Solution { get; private set; }
		
		public void Dispose()
		{
			ShutdownConsole();
			
			if (thread != null) {
				if (thread.Join(100)) {
					thread = null;
					IsRunning = false;
				}
			}
		}
		
		public void Clear()
		{
			ScriptingConsole.Clear();
		}
		
		public void WritePrompt()
		{
			ScriptingConsole.Write(prompt, ScriptingStyle.Prompt);
		}
		
		public void Run()
		{
			thread = CreateThread(RunSynchronous);
			thread.Start();
			IsRunning = true;
		}
		
		protected virtual IThread CreateThread(ThreadStart threadStart)
		{
			return new PackageManagementThread(threadStart);
		}
		
		void RunSynchronous()
		{
			InitPowerShell();
			WriteInfoBeforeFirstPrompt();
			InitializePackageScriptsForOpenSolution();
			WritePrompt();
			ProcessUserCommands();
		}
		
		void InitPowerShell()
		{
			CreatePowerShellHost();
			AddModulesToImport();
			powerShellHost.SetRemoteSignedExecutionPolicy();
			UpdateFormatting();
			RedefineClearHostFunction();
			DefineTabExpansionFunction();
			UpdateWorkingDirectory();
		}
		
		void CreatePowerShellHost()
		{
			var clearConsoleHostCommand = new ClearPackageManagementConsoleHostCommand(this);
			powerShellHost = 
				powerShellHostFactory.CreatePowerShellHost(
					this.ScriptingConsole,
					GetNuGetVersion(),
					clearConsoleHostCommand,
					new DTE());
		}
		
		protected virtual Version GetNuGetVersion()
		{
			return NuGetVersion.Version;
		}
		
		void AddModulesToImport()
		{
			string module = addinPath.CmdletsAssemblyFileName;
			powerShellHost.ModulesToImport.Add(module);
		}
		
		void UpdateFormatting()
		{
			IEnumerable<string> fileNames = addinPath.GetPowerShellFormattingFileNames();
			powerShellHost.UpdateFormatting(fileNames);
		}
		
		void RedefineClearHostFunction()
		{
			string command = "function Clear-Host { $host.PrivateData.ClearHost() }";
			powerShellHost.ExecuteCommand(command);
		}
		
		void DefineTabExpansionFunction()
		{
			string command =
				"function TabExpansion($line, $lastWord) {" +
				"    return New-Object PSObject -Property @{ NoResult = $true }" +
				"}";
			powerShellHost.ExecuteCommand(command);
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
		
		void UpdateWorkingDirectory()
		{
			string command = "Invoke-UpdateWorkingDirectory";
			powerShellHost.ExecuteCommand(command);
		}
		
		void InitializePackageScriptsForOpenSolution()
		{
			if (Solution.IsOpen) {
				string command = "Invoke-InitializePackages";
				powerShellHost.ExecuteCommand(command);
			}
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
			
			return Solution.GetProject(source, projectName);
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
			return Solution.GetProject(sourceRepository, projectName);
		}
		
		public void ShutdownConsole()
		{
			if (ScriptingConsole != null) {
				ScriptingConsole.Dispose();
			}
		}
		
		public void ExecuteCommand(string command)
		{
			ScriptingConsole.SendLine(command);
		}
		
		public IPackageRepository GetPackageRepository(PackageSource packageSource)
		{
			return registeredRepositories.CreateRepository(packageSource);
		}
		
		public void SetDefaultRunspace()
		{
			powerShellHost.SetDefaultRunspace();
		}
		
		public IConsoleHostFileConflictResolver CreateFileConflictResolver(FileConflictAction fileConflictAction)
		{
			return new ConsoleHostFileConflictResolver(packageEvents, fileConflictAction);
		}
		
		public IDisposable CreateLogger(ICmdletLogger logger)
		{
			return new PackageManagementConsoleHostLogger(logger, packageEvents);
		}
	}
}
