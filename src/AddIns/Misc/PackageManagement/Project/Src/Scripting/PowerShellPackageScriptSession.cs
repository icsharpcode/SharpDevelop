// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PowerShellPackageScriptSession : IPackageScriptSession
	{
		IPowerShellHost powerShellHost;
		IScriptingConsole console;
		
		public PowerShellPackageScriptSession(
			IPowerShellHost powerShellHost,
			IScriptingConsole console)
		{
			this.powerShellHost = powerShellHost;
			this.console = console;
		}
		
		public void SetEnvironmentPath(string path)
		{
			powerShellHost.SetEnvironmentPath(path);
		}
		
		public string GetEnvironmentPath()
		{
			return powerShellHost.GetEnvironmentPath();
		}
		
		public void AddVariable(string name, object value)
		{
			powerShellHost.AddVariable(name, value);
		}
		
		public void RemoveVariable(string name)
		{
			powerShellHost.RemoveVariable(name);
		}
		
		public void InvokeScript(string script)
		{
			console.SendLine(script);
		}
	}
}
