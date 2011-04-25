// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class RunPackageScriptsAction : IDisposable
	{
		IPackageManagementProject project;
		IPackageScriptFactory scriptFactory;
		
		public RunPackageScriptsAction(
			IPackageScriptSession session,
			IPackageManagementProject project)
			: this(project, new PackageScriptFactory(session))
		{
		}
		
		public RunPackageScriptsAction(
			IPackageManagementProject project,
			IPackageScriptFactory scriptFactory)
		{
			this.project = project;
			this.scriptFactory = scriptFactory;
			
			RegisterEvents();
		}
		
		void RegisterEvents()
		{
			project.PackageInstalled += PackageInstalled;
			project.PackageReferenceAdded += PackageReferenceAdded;
			project.PackageReferenceRemoved += PackageReferenceRemoved;
		}
		
		void UnregisterEvents()
		{
			project.PackageInstalled -= PackageInstalled;
			project.PackageReferenceAdded -= PackageReferenceAdded;
			project.PackageReferenceRemoved -= PackageReferenceRemoved;
		}
		
		void PackageInstalled(object sender, PackageOperationEventArgs e)
		{
			RunInitScript(e);
		}
		
		void PackageReferenceRemoved(object sender, PackageOperationEventArgs e)
		{
			RunUninstallScript(e);
		}
		
		void PackageReferenceAdded(object sender, PackageOperationEventArgs e)
		{
			RunInstallScript(e);
		}
		
		void RunInitScript(PackageOperationEventArgs e)
		{
			IPackageScript script = scriptFactory.CreatePackageInitializeScript(e.InstallPath);
			script.Execute();
		}
		
		void RunUninstallScript(PackageOperationEventArgs e)
		{
			IPackageScript script = scriptFactory.CreatePackageUninstallScript(e.InstallPath);
			script.Project = project;
			script.Execute();
		}
		
		void RunInstallScript(PackageOperationEventArgs e)
		{
			IPackageScript script = scriptFactory.CreatePackageInstallScript(e.InstallPath);
			script.Project = project;
			script.Execute();
		}
		
		public void Dispose()
		{
			IsDisposed = true;
			UnregisterEvents();
		}
		
		public bool IsDisposed { get; private set; }
	}
}
