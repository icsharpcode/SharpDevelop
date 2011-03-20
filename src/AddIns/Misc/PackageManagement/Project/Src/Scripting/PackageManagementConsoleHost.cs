// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		IPowerShellHost powerShellHost;
		int autoIndentSize = 0;
		string prompt = "PM> ";
		
		public PackageManagementConsoleHost(IPowerShellHostFactory powerShellHostFactory)
		{
			this.powerShellHostFactory = powerShellHostFactory;
		}
		
		public PackageManagementConsoleHost()
			: this(new PowerShellHostFactory())
		{
		}
		
		public IProject DefaultProject { get; set; }
		public PackageSource ActivePackageSource { get; set; }
		public IScriptingConsole ScriptingConsole { get; set; }
		
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
			WritePrompt();
			ProcessUserCommands();
		}
		
		void InitPowerShell()
		{
			CreatePowerShellHost();
			powerShellHost.SetRemoteSignedExecutionPolicy();
		}
		
		void CreatePowerShellHost()
		{
			powerShellHost = powerShellHostFactory.CreatePowerShellHost(ScriptingConsole);
		}
		
		void WritePrompt()
		{
			ScriptingConsole.Write(prompt, ScriptingStyle.Prompt);
			//textEditor.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { textEditor.ScrollToEnd(); }));
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
	}
}
