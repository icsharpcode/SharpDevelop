// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting.Tests.Utils;

namespace PackageManagement.Tests.Helpers
{
	public class TestablePowerShellMissingConsoleHost : PowerShellMissingConsoleHost
	{
		public FakeScriptingConsole FakeScriptingConsole;
		
		public TestablePowerShellMissingConsoleHost()
			: this(new FakeScriptingConsole())
		{
		}
		
		public TestablePowerShellMissingConsoleHost(FakeScriptingConsole scriptingConsole)
		{
			this.FakeScriptingConsole = scriptingConsole;
			this.ScriptingConsole = scriptingConsole;
		}
		
		public string GetPowerShellIsNotInstalledMessageReturnValue = "PowerShell 2.0 is not installed.";
		
		protected override string GetPowerShellIsNotInstalledMessage()
		{
			return GetPowerShellIsNotInstalledMessageReturnValue;
		}
	}
}
