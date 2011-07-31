// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageManagementConsoleHostProvider
	{
		IPackageManagementSolution solution;
		IRegisteredPackageRepositories registeredRepositories;
		IPowerShellDetection powerShellDetection;
		IPackageManagementConsoleHost consoleHost;
		
		public PackageManagementConsoleHostProvider(
			IPackageManagementSolution solution,
			IRegisteredPackageRepositories registeredRepositories)
			: this(solution, registeredRepositories, new PowerShellDetection())
		{
		}
		
		public PackageManagementConsoleHostProvider(
			IPackageManagementSolution solution,
			IRegisteredPackageRepositories registeredRepositories,
			IPowerShellDetection powerShellDetection)
		{
			this.solution = solution;
			this.registeredRepositories = registeredRepositories;
			this.powerShellDetection = powerShellDetection;
		}
		
		public IPackageManagementConsoleHost ConsoleHost {
			get {
				if (consoleHost == null) {
					CreateConsoleHost();
				}
				return consoleHost;
			}
		}
		
		void CreateConsoleHost()
		{
			if (powerShellDetection.IsPowerShell2Installed()) {
				consoleHost = new PackageManagementConsoleHost(solution, registeredRepositories);
			} else {
				consoleHost = new PowerShellMissingConsoleHost();
			}
		}
	}
}
