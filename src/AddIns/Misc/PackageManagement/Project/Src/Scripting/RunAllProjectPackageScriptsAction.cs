// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class RunAllProjectPackageScriptsAction : IDisposable
	{
		IPackageScriptRunner scriptRunner;
		List<IPackageManagementProject> projects;
		IPackageScriptFactory scriptFactory;
		IGlobalMSBuildProjectCollection projectCollection;
		
		List<EventHandler<PackageOperationEventArgs>> packageInstalledHandlers =
			new List<EventHandler<PackageOperationEventArgs>>();
		
		List<EventHandler<PackageOperationEventArgs>> packageReferenceAddedHandlers =
			new List<EventHandler<PackageOperationEventArgs>>();
		
		List<EventHandler<PackageOperationEventArgs>> packageReferenceRemovedHandlers =
			new List<EventHandler<PackageOperationEventArgs>>();
		
		public RunAllProjectPackageScriptsAction(
			IPackageScriptRunner scriptRunner,
			IEnumerable<IPackageManagementProject> projects)
			: this(scriptRunner, projects, new PackageScriptFactory(), new GlobalMSBuildProjectCollection())
		{
		}
		
		public RunAllProjectPackageScriptsAction(
			IPackageScriptRunner scriptRunner,
			IEnumerable<IPackageManagementProject> projects,
			IPackageScriptFactory scriptFactory,
			IGlobalMSBuildProjectCollection projectCollection)
		{
			this.scriptRunner = scriptRunner;
			this.projects = projects.ToList();
			this.scriptFactory = scriptFactory;
			this.projectCollection = projectCollection;
			
			AddProjectsToGlobalCollection();
			RegisterEvents();
		}
		
		public void Dispose()
		{
			IsDisposed = true;
			UnregisterEvents();
			projectCollection.Dispose();
		}
		
		public bool IsDisposed { get; private set; }
		
		void AddProjectsToGlobalCollection()
		{
			foreach (IPackageManagementProject project in projects) {
				projectCollection.AddProject(project);
			}
		}
		
		void RegisterEvents()
		{
			foreach (IPackageManagementProject project in projects) {
				RegisterPackageInstalledEvent(project);
				RegisterPackageReferenceAddedEvent(project);
				RegisterPackageReferenceRemovedEvent(project);
			}
		}
		
		void RegisterPackageInstalledEvent(IPackageManagementProject project)
		{
			EventHandler<PackageOperationEventArgs> installHandler =
				(_, e) => PackageInstalled(project, e);
			packageInstalledHandlers.Add(installHandler);
			project.PackageInstalled += installHandler;
		}
		
		void RegisterPackageReferenceAddedEvent(IPackageManagementProject project)
		{
			EventHandler<PackageOperationEventArgs> referenceAddedHandler =
				(_, e) => PackageReferenceAdded(project, e);
			packageReferenceAddedHandlers.Add(referenceAddedHandler);
			project.PackageReferenceAdded += referenceAddedHandler;
		}
		
		void RegisterPackageReferenceRemovedEvent(IPackageManagementProject project)
		{
			EventHandler<PackageOperationEventArgs> referenceRemovedHandler =
				(_, e) => PackageReferenceRemoved(project, e);
			packageReferenceRemovedHandlers.Add(referenceRemovedHandler);
			project.PackageReferenceRemoved += referenceRemovedHandler;
		}
		
		void UnregisterEvents()
		{
			foreach (IPackageManagementProject project in projects) {
				UnregisterPackageInstalledEvent(project);
				UnregisterPackageReferenceAddedEvent(project);
				UnregisterPackageReferenceRemovedEvent(project);
			}
		}
		
		void UnregisterPackageInstalledEvent(IPackageManagementProject project)
		{
			EventHandler<PackageOperationEventArgs> handler = packageInstalledHandlers.First();
			packageInstalledHandlers.Remove(handler);
			project.PackageInstalled -= handler;
		}
		
		void UnregisterPackageReferenceAddedEvent(IPackageManagementProject project)
		{
			EventHandler<PackageOperationEventArgs> handler = packageReferenceAddedHandlers.First();
			packageReferenceAddedHandlers.Remove(handler);
			project.PackageReferenceAdded -= handler;
		}
		
		void UnregisterPackageReferenceRemovedEvent(IPackageManagementProject project)
		{
			EventHandler<PackageOperationEventArgs> handler = packageReferenceRemovedHandlers.First();
			packageReferenceRemovedHandlers.Remove(handler);
			project.PackageReferenceRemoved -= handler;
		}
		
		void PackageInstalled(IPackageManagementProject project, PackageOperationEventArgs e)
		{
			RunInitScript(project, e);
		}
		
		void PackageReferenceAdded(IPackageManagementProject project, PackageOperationEventArgs e)
		{
			RunInstallScript(project, e);
		}
		
		void PackageReferenceRemoved(IPackageManagementProject project, PackageOperationEventArgs e)
		{
			RunUninstallScript(project, e);
		}
		
		void RunInitScript(IPackageManagementProject project, PackageOperationEventArgs e)
		{
			IPackageScript script = scriptFactory.CreatePackageInitializeScript(e.Package, e.InstallPath);
			RunScript(project, script);
		}
		
		void RunScript(IPackageManagementProject project, IPackageScript script)
		{
			script.Project = project;
			scriptRunner.Run(script);
		}
		
		void RunInstallScript(IPackageManagementProject project, PackageOperationEventArgs e)
		{
			IPackageScript script = scriptFactory.CreatePackageInstallScript(e.Package, e.InstallPath);
			RunScript(project, script);
		}
		
		void RunUninstallScript(IPackageManagementProject project, PackageOperationEventArgs e)
		{
			IPackageScript script = scriptFactory.CreatePackageUninstallScript(e.Package, e.InstallPath);
			RunScript(project, script);
		}
	}
}
