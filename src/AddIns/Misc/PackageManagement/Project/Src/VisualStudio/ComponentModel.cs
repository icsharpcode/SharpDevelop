// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using Microsoft.VisualStudio.ComponentModelHost;
using NuGetConsole;

namespace ICSharpCode.PackageManagement.VisualStudio
{
	public class ComponentModel : SComponentModel, IComponentModel
	{
		IPackageManagementConsoleHost consoleHost;
		
		public ComponentModel(IPackageManagementConsoleHost consoleHost)
		{
			this.consoleHost = consoleHost;
		}
		
		public ComponentModel()
		{
		}
		
		public T GetService<T>()
			where T : class
		{
			return GetService(typeof(T)) as T;
		}
		
		object GetService(Type type)
		{
			if (type.FullName == typeof(IConsoleInitializer).FullName) {
				return new ConsoleInitializer(GetConsoleHost());
			}
			return null;
		}
		
		protected virtual IPackageManagementConsoleHost GetConsoleHost()
		{
			if (consoleHost != null) {
				return consoleHost;
			}
			return PackageManagementServices.ConsoleHost;
		}
	}
}
