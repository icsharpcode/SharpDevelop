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
