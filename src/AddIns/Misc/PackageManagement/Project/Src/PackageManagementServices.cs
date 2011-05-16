// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;

namespace ICSharpCode.PackageManagement
{
	public static class PackageManagementServices
	{
		static readonly PackageManagementOptions options;
		static readonly PackageManagementSolution solution;
		static readonly PackageManagementConsoleHost consoleHost;
		static readonly RegisteredPackageRepositories registeredPackageRepositories;
		static readonly PackageManagementEvents packageManagementEvents = new PackageManagementEvents();
		static readonly PackageManagementProjectService projectService = new PackageManagementProjectService();
		static readonly ProjectBrowserRefresher projectBrowserRefresher;
		static readonly PackageManagementOutputMessagesView outputMessagesView;
		static readonly RunPackageInitializationScriptsOnSolutionOpen runPackageInitializationScripts;
		static readonly ResetPowerShellWorkingDirectoryOnSolutionClosed resetPowerShellWorkingDirectory;
		
		static PackageManagementServices()
		{
			options = new PackageManagementOptions();
			registeredPackageRepositories = new RegisteredPackageRepositories(options);
			outputMessagesView = new PackageManagementOutputMessagesView(packageManagementEvents);
			solution = new PackageManagementSolution(registeredPackageRepositories, packageManagementEvents);
			consoleHost = new PackageManagementConsoleHost(solution, registeredPackageRepositories);
			projectBrowserRefresher = new ProjectBrowserRefresher(projectService, packageManagementEvents);
			runPackageInitializationScripts = new RunPackageInitializationScriptsOnSolutionOpen(projectService);
			resetPowerShellWorkingDirectory = new ResetPowerShellWorkingDirectoryOnSolutionClosed(projectService, consoleHost);
		}
		
		public static PackageManagementOptions Options {
			get { return options; }
		}
		
		public static IPackageManagementSolution Solution {
			get { return solution; }
		}
		
		public static IPackageManagementConsoleHost ConsoleHost {
			get { return consoleHost; }
		}
		
		public static IRegisteredPackageRepositories RegisteredPackageRepositories {
			get { return registeredPackageRepositories; }
		}
		
		public static IPackageManagementEvents PackageManagementEvents {
			get { return packageManagementEvents; }
		}
		
		public static IPackageManagementOutputMessagesView OutputMessagesView {
			get { return outputMessagesView; }
		}
		
		public static IPackageManagementProjectService ProjectService {
			get { return projectService; }
		}
	}
}
