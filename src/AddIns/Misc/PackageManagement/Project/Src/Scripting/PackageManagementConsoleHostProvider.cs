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
		IPackageManagementEvents packageEvents;
		
		public PackageManagementConsoleHostProvider(
			IPackageManagementSolution solution,
			IRegisteredPackageRepositories registeredRepositories)
			: this(solution,
				registeredRepositories,
				new PowerShellDetection(),
				PackageManagementServices.PackageManagementEvents)
		{
		}
		
		public PackageManagementConsoleHostProvider(
			IPackageManagementSolution solution,
			IRegisteredPackageRepositories registeredRepositories,
			IPowerShellDetection powerShellDetection,
			IPackageManagementEvents packageEvents)
		{
			this.solution = solution;
			this.registeredRepositories = registeredRepositories;
			this.powerShellDetection = powerShellDetection;
			this.packageEvents = packageEvents;
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
				consoleHost = new PackageManagementConsoleHost(solution, registeredRepositories, packageEvents);
			} else {
				consoleHost = new PowerShellMissingConsoleHost();
			}
		}
	}
}
