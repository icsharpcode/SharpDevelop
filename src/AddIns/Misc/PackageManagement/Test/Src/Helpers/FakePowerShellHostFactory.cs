// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class FakePowerShellHostFactory : IPowerShellHostFactory
	{
		public IScriptingConsole ScriptingConsolePassedToCreatePowerShellHost;
		public FakePowerShellHost FakePowerShellHost = new FakePowerShellHost();
		
		public IPowerShellHost CreatePowerShellHost(IScriptingConsole scriptingConsole)
		{
			ScriptingConsolePassedToCreatePowerShellHost = scriptingConsole;
			return FakePowerShellHost;
		}
	}
}
