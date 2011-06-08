// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			ProcessPackageAction action = null;
			while (GetNextAction(out action)) {
				Execute(action);
			}
		}
		
		bool GetNextAction(out ProcessPackageAction action)
		{
			return actionsToRun.GetNextAction(out action);
		}
		
		void Execute(ProcessPackageAction action)
		{
			action.PackageScriptRunner = this;
			action.Execute();
		}
	}
}
