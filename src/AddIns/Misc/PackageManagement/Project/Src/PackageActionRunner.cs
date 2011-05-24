// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement
{
	public class PackageActionRunner : IPackageActionRunner
	{
		IPackageActionRunner consolePackageActionRunner;
		
		public PackageActionRunner(IPackageActionRunner consolePackageActionRunner)
		{
			this.consolePackageActionRunner = consolePackageActionRunner;
		}
		
		public void Run(ProcessPackageAction action)
		{
			if (action.HasPackageScriptsToRun()) {
				consolePackageActionRunner.Run(action);
			} else {
				action.Execute();
			}
		}
	}
}
