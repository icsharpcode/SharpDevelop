// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageInitializationScriptsConsole
	{
		IPackageManagementConsoleHost consoleHost;
		
		public PackageInitializationScriptsConsole(
			IPackageManagementConsoleHost consoleHost)
		{
			this.consoleHost = consoleHost;
		}
		
		public void ExecuteCommand(string command)
		{
			CreateConsolePadIfConsoleHostNotRunning();
			consoleHost.ScriptingConsole.SendLine(command);
		}
		
		void CreateConsolePadIfConsoleHostNotRunning()
		{
			if (!consoleHost.IsRunning) {
				CreateConsolePad();
			}
		}
		
		protected virtual void CreateConsolePad()
		{
			PadDescriptor pad = WorkbenchSingleton.Workbench.GetPad(typeof(PackageManagementConsolePad));
			PackageManagementConsolePad consolePad = pad.PadContent.Control as PackageManagementConsolePad;
		}
	}
}
