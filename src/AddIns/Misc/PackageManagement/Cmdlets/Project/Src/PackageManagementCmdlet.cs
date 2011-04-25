// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	public abstract class PackageManagementCmdlet : PSCmdlet, ITerminatingCmdlet, IPackageScriptSession
	{
		static readonly string EnvironmentPathVariableName = "env:path";
		
		IPackageManagementConsoleHost consoleHost;
		ICmdletTerminatingError terminatingError;
		
		public PackageManagementCmdlet(
			IPackageManagementConsoleHost consoleHost,
			ICmdletTerminatingError terminatingError)
		{
			this.consoleHost = consoleHost;
			this.terminatingError = terminatingError;
		}
		
		protected IPackageManagementConsoleHost ConsoleHost {
			get { return consoleHost; }
		}
		
		protected MSBuildBasedProject DefaultProject {
			get { return consoleHost.DefaultProject as MSBuildBasedProject; }
		}

		protected ICmdletTerminatingError TerminatingError {
			get {
				if (terminatingError == null) {
					terminatingError = new CmdletTerminatingError(this);
				}
				return terminatingError;
			}
		}
		
		protected void ThrowErrorIfProjectNotOpen()
		{
			if (DefaultProject == null) {
				ThrowProjectNotOpenTerminatingError();
			}
		}
			
		protected void ThrowProjectNotOpenTerminatingError()
		{
			TerminatingError.ThrowNoProjectOpenError();
		}
		
		public void SetEnvironmentPath(string path)
		{
			SetSessionVariable(EnvironmentPathVariableName, path);
		}
		
		protected virtual void SetSessionVariable(string name, object value)
		{
			SessionState.PSVariable.Set(name, value);
		}
		
		public string GetEnvironmentPath()
		{
			return (string)GetSessionVariable(EnvironmentPathVariableName);
		}
		
		protected virtual object GetSessionVariable(string name)
		{
			return GetVariableValue(name);
		}
		
		public void AddVariable(string name, object value)
		{
			SetSessionVariable(name, value);
		}
		
		public void RemoveVariable(string name)
		{
			RemoveSessionVariable(name);
		}
		
		protected virtual void RemoveSessionVariable(string name)
		{
			SessionState.PSVariable.Remove(name);
		}
		
		public void InvokeScript(string script)
		{
			InvokeCommand.InvokeScript(script);
		}
	}
}
