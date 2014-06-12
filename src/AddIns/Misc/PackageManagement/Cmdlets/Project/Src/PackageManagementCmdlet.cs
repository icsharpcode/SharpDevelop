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
using System.Management.Automation.Runspaces;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	public abstract class PackageManagementCmdlet : PSCmdlet, ITerminatingCmdlet, IPackageScriptSession, IPackageScriptRunner, ICmdletLogger
	{
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
			SetSessionVariable("env:path", path);
		}
		
		protected virtual void SetSessionVariable(string name, object value)
		{
			SessionState.PSVariable.Set(name, value);
		}
		
		public string GetEnvironmentPath()
		{
			return (string)GetSessionVariable("env:path");
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
		
		public virtual void InvokeScript(string script)
		{
			var resultTypes = PipelineResultTypes.Error | PipelineResultTypes.Output;
			InvokeCommand.InvokeScript(script, false, resultTypes, null, null);
		}
		
		public void Run(IPackageScript script)
		{
			if (script.Exists()) {
				script.Run(this);
			}
		}
		
		void ICmdletLogger.WriteLine(string message)
		{
			Host.UI.WriteLine(message);
		}
	}
}
