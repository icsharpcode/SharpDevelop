// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.PackageManagement
{
	public class RestorePackagesCommand : AbstractMenuCommand
	{
		IPackageManagementOutputMessagesView outputMessagesView;
		IPackageManagementSolution solution;
		
		public RestorePackagesCommand()
			: this(PackageManagementServices.OutputMessagesView, PackageManagementServices.Solution)
		{
		}
		
		public RestorePackagesCommand(
			IPackageManagementOutputMessagesView outputMessagesView,
			IPackageManagementSolution solution)
		{
			this.outputMessagesView = outputMessagesView;
			this.solution = solution;
		}
		
		public override void Run()
		{
			try {
				ClearOutputWindow();
				BringOutputWindowToFront();
				RunRestore();
			} catch (Exception ex) {
				LoggingService.Debug(ex.ToString());
				outputMessagesView.AppendLine(ex.Message);
			}
		}
		
		void ClearOutputWindow()
		{
			outputMessagesView.Clear();
		}
		
		void BringOutputWindowToFront()
		{
			CompilerMessageView.Instance.BringToFront();
		}
		
		void RunRestore()
		{
			var commandLine = new NuGetPackageRestoreCommandLine(solution);
			commandLine.Command = NuGetExePath.GetPath();
			
			outputMessagesView.AppendLine(commandLine.ToString());
			
			ProcessRunner runner = CreateProcessRunner();
			runner.WorkingDirectory = Path.GetDirectoryName(solution.FileName);
			runner.Start(commandLine.Command, commandLine.Arguments);
		}
		
		ProcessRunner CreateProcessRunner()
		{
			var runner = new ProcessRunner();
			runner.LogStandardOutputAndError = false;
			runner.OutputLineReceived += (sender, e) => outputMessagesView.AppendLine(e.Line);
			runner.ErrorLineReceived  += (sender, e) => outputMessagesView.AppendLine(e.Line);
			runner.ProcessExited += (sender, e) => {
				if (runner.ExitCode != 0) {
					outputMessagesView.AppendLine("Exit code " + runner.ExitCode);
				}
			};
			return runner;
		}
	}
}
