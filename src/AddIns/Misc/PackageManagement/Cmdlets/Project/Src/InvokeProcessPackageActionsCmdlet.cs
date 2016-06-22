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
	[Cmdlet(VerbsLifecycle.Invoke, "ProcessPackageActions", DefaultParameterSetName = ParameterAttribute.AllParameterSets)]
	public class InvokeProcessPackageActionsCmdlet : PackageManagementCmdlet
	{
		PackageActionsToRun actionsToRun;
		
		public InvokeProcessPackageActionsCmdlet()
			: this(
				PackageManagementServices.PackageActionsToRun,
				PackageManagementServices.ConsoleHost,
				null)
		{
		}
		
		public InvokeProcessPackageActionsCmdlet(
			PackageActionsToRun actionsToRun,
			IPackageManagementConsoleHost consoleHost,
			ICmdletTerminatingError terminatingError)
			: base(consoleHost, terminatingError)
		{
			this.actionsToRun = actionsToRun;
		}
		
		protected override void ProcessRecord()
		{
			ExecutePackageActions();
		}
		
		void ExecutePackageActions()
		{
			IPackageAction action = null;
			while (GetNextAction(out action)) {
				Execute(action);
			}
		}
		
		bool GetNextAction(out IPackageAction action)
		{
			return actionsToRun.GetNextAction(out action);
		}
		
		void Execute(IPackageAction action)
		{
			action.PackageScriptRunner = this;
			action.Execute();
		}
	}
}
