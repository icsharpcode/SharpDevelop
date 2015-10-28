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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
			runner.RunInOutputPadAsync(outputMessagesView.OutputCategory, commandLine.Command, commandLine.Arguments)
				.ContinueWith(task => OnNuGetPackageRestoreComplete(task));
		}
		
		void OnNuGetPackageRestoreComplete(Task<int> task)
		{
			if (task.Exception != null) {
				LoggingService.Debug(task.Exception.ToString());
				outputMessagesView.AppendLine(task.Exception.Message);
			} else {
				ForceGenerationOfRepositoriesConfigFile();
			}
		}
		
		/// <summary>
		/// Create a Package Manager for each project to force a new repositories.config file
		/// to be generated with references to all projects that have NuGet packages.
		/// </summary>
		void ForceGenerationOfRepositoriesConfigFile()
		{
			try {
				var repository = PackageManagementServices.RegisteredPackageRepositories.CreateAggregateRepository();
				var projects = solution.GetProjects(repository).ToList();
			} catch (Exception ex) {
				LoggingService.Debug(ex.ToString());
				outputMessagesView.AppendLine(ex.Message);
			}
		}
	}
}
