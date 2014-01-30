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
