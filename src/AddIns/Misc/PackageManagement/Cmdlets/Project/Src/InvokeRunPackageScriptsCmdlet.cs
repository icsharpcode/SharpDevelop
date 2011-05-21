// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement.Scripting;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	[Cmdlet(VerbsLifecycle.Invoke, "RunPackageScripts", DefaultParameterSetName = ParameterAttribute.AllParameterSets)]
	public class InvokeRunPackageScriptsCmdlet : PackageManagementCmdlet
	{
		PackageScriptsToRun scriptsToBeRun;
		
		public InvokeRunPackageScriptsCmdlet()
			: this(
				PackageManagementServices.PackageScriptsToRun,
				PackageManagementServices.ConsoleHost,
				null)
		{
		}
		
		public InvokeRunPackageScriptsCmdlet(
			PackageScriptsToRun scriptsToBeRun,
			IPackageManagementConsoleHost consoleHost,
			ICmdletTerminatingError terminatingError)
			: base(consoleHost, terminatingError)
		{
			this.scriptsToBeRun = scriptsToBeRun;
		}
		
		protected override void ProcessRecord()
		{
			RunPackageScripts();
		}
		
		void RunPackageScripts()
		{
			IPackageScript script = null;
			while (GetNextScript(out script)) {
				Run(script);
			}
		}
		
		bool GetNextScript(out IPackageScript script)
		{
			script = scriptsToBeRun.GetNextScript();
			return script != null;
		}
	}
}
