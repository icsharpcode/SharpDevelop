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
		
		List<EventHandler<PackageOperationEventArgs>> packageReferenceRemovingHandlers =
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
				RegisterPackageReferenceRemovingEvent(project);
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
		
		void RegisterPackageReferenceRemovingEvent(IPackageManagementProject project)
		{
			EventHandler<PackageOperationEventArgs> referenceRemovingHandler =
				(_, e) => PackageReferenceRemoving(project, e);
			packageReferenceRemovingHandlers.Add(referenceRemovingHandler);
			project.PackageReferenceRemoving += referenceRemovingHandler;
		}
		
		void UnregisterEvents()
		{
			foreach (IPackageManagementProject project in projects) {
				UnregisterPackageInstalledEvent(project);
				UnregisterPackageReferenceAddedEvent(project);
				UnregisterPackageReferenceRemovingEvent(project);
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
		
		void UnregisterPackageReferenceRemovingEvent(IPackageManagementProject project)
		{
			EventHandler<PackageOperationEventArgs> handler = packageReferenceRemovingHandlers.First();
			packageReferenceRemovingHandlers.Remove(handler);
			project.PackageReferenceRemoving -= handler;
		}
		
		void PackageInstalled(IPackageManagementProject project, PackageOperationEventArgs e)
		{
			RunInitScript(project, e);
		}
		
		void PackageReferenceAdded(IPackageManagementProject project, PackageOperationEventArgs e)
		{
			RunInstallScript(project, e);
		}
		
		void PackageReferenceRemoving(IPackageManagementProject project, PackageOperationEventArgs e)
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
