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
		IPackageScriptRunner scriptRunner;
		
		public RunPackageScriptsAction(
			IPackageScriptRunner scriptRunner,
			IPackageManagementProject project)
			: this(project, scriptRunner, new PackageScriptFactory())
		{
		}
		
		public RunPackageScriptsAction(
			IPackageManagementProject project,
			IPackageScriptRunner scriptRunner,
			IPackageScriptFactory scriptFactory)
		{
			this.project = project;
			this.scriptRunner = scriptRunner;
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
			RunScript(script, e.Package);
		}
		
		void RunScript(IPackageScript script, IPackage package)
		{
			script.Project = project;
			script.Package = package;
			scriptRunner.Run(script);
		}
		
		void RunUninstallScript(PackageOperationEventArgs e)
		{
			IPackageScript script = scriptFactory.CreatePackageUninstallScript(e.InstallPath);
			RunScript(script, e.Package);
		}
		
		void RunInstallScript(PackageOperationEventArgs e)
		{
			IPackageScript script = scriptFactory.CreatePackageInstallScript(e.InstallPath);
			RunScript(script, e.Package);
		}
		
		public void Dispose()
		{
			IsDisposed = true;
			UnregisterEvents();
		}
		
		public bool IsDisposed { get; private set; }
	}
}
