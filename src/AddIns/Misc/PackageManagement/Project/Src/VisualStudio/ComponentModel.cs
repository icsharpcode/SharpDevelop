// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using Microsoft.VisualStudio.ComponentModelHost;
using NuGet.VisualStudio;
using NuGetConsole;

namespace ICSharpCode.PackageManagement.VisualStudio
{
	public class ComponentModel : SComponentModel, IComponentModel
	{
		IPackageManagementConsoleHost consoleHost;
		IPackageManagementSolution solution;
		
		public ComponentModel(IPackageManagementConsoleHost consoleHost, IPackageManagementSolution solution)
		{
			this.consoleHost = consoleHost;
			this.solution = solution;
		}
		
		public ComponentModel()
		{
		}
		
		public T GetService<T>()
			where T : class
		{
			return GetService(typeof(T)) as T;
		}
		
		public object GetService(Type type)
		{
			if (type.FullName == typeof(IConsoleInitializer).FullName) {
				return new ConsoleInitializer(GetConsoleHost());
			} else if (type.FullName == typeof(IVsPackageInstallerServices).FullName) {
				return new VsPackageInstallerServices(GetSolution());
			}
			return null;
		}
		
		IPackageManagementConsoleHost GetConsoleHost()
		{
			if (consoleHost != null) {
				return consoleHost;
			}
			return PackageManagementServices.ConsoleHost;
		}
		
		IPackageManagementSolution GetSolution()
		{
			if (solution != null) {
				return solution;
			}
			return PackageManagementServices.Solution;
		}
	}
}
