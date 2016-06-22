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
using System.Management.Automation;
using ICSharpCode.PackageManagement.Scripting;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	[Cmdlet(VerbsLifecycle.Invoke, "UpdateWorkingDirectory", DefaultParameterSetName = ParameterAttribute.AllParameterSets)]
	public class InvokeUpdateWorkingDirectoryCmdlet : PackageManagementCmdlet
	{
		IPackageManagementProjectService projectService;
		
		public InvokeUpdateWorkingDirectoryCmdlet()
			: this(
				PackageManagementServices.ProjectService,
				PackageManagementServices.ConsoleHost,
				null)
		{
		}
		
		public InvokeUpdateWorkingDirectoryCmdlet(
			IPackageManagementProjectService projectService,
			IPackageManagementConsoleHost consoleHost,
			ICmdletTerminatingError terminatingError)
			: base(consoleHost, terminatingError)
		{
			this.projectService = projectService;
		}
		
		protected override void ProcessRecord()
		{
			UpdateWorkingDirectory();
		}
		
		void UpdateWorkingDirectory()
		{
			string directory = GetWorkingDirectory();
			UpdateWorkingDirectory(directory);
		}
		
		string GetWorkingDirectory()
		{
			var workingDirectory = new PowerShellWorkingDirectory(projectService);
			return workingDirectory.GetWorkingDirectory();
		}
		
		void UpdateWorkingDirectory(string directory)
		{
			string command = String.Format("Set-Location {0}", directory);
			InvokeScript(command);
		}
	}
}
