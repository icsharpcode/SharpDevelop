// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading.Tasks;
using NuGetConsole;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class ConsoleInitializer : IConsoleInitializer
	{
		IPackageManagementConsoleHost consoleHost;
		
		public ConsoleInitializer(IPackageManagementConsoleHost consoleHost)
		{
			this.consoleHost = consoleHost;
		}
		
		public Task<Action> Initialize()
		{
			return Task.Factory.StartNew(() => new Action(consoleHost.SetDefaultRunspace));
		}
	}
}
