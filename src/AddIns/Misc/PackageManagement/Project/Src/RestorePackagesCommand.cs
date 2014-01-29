// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

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
			
			var runner = new ProcessRunner();
			runner.WorkingDirectory = Path.GetDirectoryName(solution.FileName);
			runner.RunInOutputPadAsync(outputMessagesView.OutputCategory, commandLine.Command, commandLine.Arguments).FireAndForget();
		}
	}
}
