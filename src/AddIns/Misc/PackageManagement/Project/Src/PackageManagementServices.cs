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
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public static class PackageManagementServices
	{
		static readonly PackageManagementOptions options;
		static readonly PackageManagementSolution solution;
		static readonly PackageManagementConsoleHostProvider consoleHostProvider;
		static readonly RegisteredPackageRepositories registeredPackageRepositories;
		static readonly PackageManagementEvents packageManagementEvents = new PackageManagementEvents();
		static readonly PackageManagementProjectService projectService = new PackageManagementProjectService();
		static readonly ProjectBrowserRefresher projectBrowserRefresher;
		static readonly PackageManagementOutputMessagesView outputMessagesView;
		static readonly RunPackageInitializationScriptsOnSolutionOpen runPackageInitializationScripts;
		static readonly ResetPowerShellWorkingDirectoryOnSolutionClosed resetPowerShellWorkingDirectory;
		static readonly PackageActionsToRun packageActionsToRun = new PackageActionsToRun();
		static readonly PackageActionRunner packageActionRunner;
		static readonly PackageRepositoryCache packageRepositoryCache;
		static readonly UserAgentGeneratorForRepositoryRequests userAgentGenerator;
		
		static PackageManagementServices()
		{
			options = new PackageManagementOptions();
			packageRepositoryCache = new PackageRepositoryCache(options.PackageSources, options.RecentPackages);
			userAgentGenerator = new UserAgentGeneratorForRepositoryRequests(packageRepositoryCache);
			registeredPackageRepositories = new RegisteredPackageRepositories(packageRepositoryCache, options);
			
			outputMessagesView = new PackageManagementOutputMessagesView(packageManagementEvents);
			projectBrowserRefresher = new ProjectBrowserRefresher(projectService, packageManagementEvents);
			solution = new PackageManagementSolution(registeredPackageRepositories, packageManagementEvents);
			
			consoleHostProvider = new PackageManagementConsoleHostProvider(solution, registeredPackageRepositories);
			runPackageInitializationScripts = new RunPackageInitializationScriptsOnSolutionOpen(projectService);
			resetPowerShellWorkingDirectory = new ResetPowerShellWorkingDirectoryOnSolutionClosed(projectService, ConsoleHost);
			var consolePackageActionRunner = new ConsolePackageActionRunner(ConsoleHost, packageActionsToRun);
			packageActionRunner = new PackageActionRunner(consolePackageActionRunner, packageManagementEvents);
			
			InitializeCredentialProvider();
		}
		
		static void InitializeCredentialProvider()
		{
			ISettings settings = Settings.LoadDefaultSettings(null, null, null);
			var packageSourceProvider = new PackageSourceProvider(settings);
			var credentialProvider = new SettingsCredentialProvider(new SharpDevelopCredentialProvider(), packageSourceProvider);
			
			HttpClient.DefaultCredentialProvider = credentialProvider;
		}
		
		public static PackageManagementOptions Options {
			get { return options; }
		}
		
		public static IPackageManagementSolution Solution {
			get { return solution; }
		}
		
		public static IPackageManagementConsoleHost ConsoleHost {
			get { return consoleHostProvider.ConsoleHost; }
		}
		
		public static IRegisteredPackageRepositories RegisteredPackageRepositories {
			get { return registeredPackageRepositories; }
		}
		
		public static IPackageRepositoryCache PackageRepositoryCache {
			get { return packageRepositoryCache; }
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
		
		public static PackageActionsToRun PackageActionsToRun {
			get { return packageActionsToRun; }
		}
		
		public static IPackageActionRunner PackageActionRunner {
			get { return packageActionRunner; }
		}
	}
}
