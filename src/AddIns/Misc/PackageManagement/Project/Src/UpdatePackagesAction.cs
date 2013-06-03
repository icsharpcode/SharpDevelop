// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UpdatePackagesAction : IUpdatePackagesAction
	{
		List<IPackage> packages = new List<IPackage>();
		List<PackageOperation> operations = new List<PackageOperation>();
		IPackageManagementEvents packageManagementEvents;
		
		public UpdatePackagesAction(
			IPackageManagementProject project,
			IPackageManagementEvents packageManagementEvents)
		{
			Project = project;
			this.packageManagementEvents = packageManagementEvents;
			UpdateDependencies = true;
		}
		
		public IPackageScriptRunner PackageScriptRunner { get; set; }
		public IPackageManagementProject Project { get; private set; }
		
		public IEnumerable<IPackage> Packages {
			get { return packages; }
		}
		
		public IEnumerable<PackageOperation> Operations {
			get { return operations; }
		}
		
		public bool UpdateDependencies { get; set; }
		public bool AllowPrereleaseVersions { get; set; }
		public ILogger Logger { get; set; }
		
		public bool HasPackageScriptsToRun()
		{
			var files = new PackageFilesForOperations(Operations);
			return files.HasAnyPackageScripts();
		}
		
		public void AddOperations(IEnumerable<PackageOperation> operations)
		{
			this.operations.AddRange(operations);
		}
		
		public void AddPackages(IEnumerable<IPackageFromRepository> packages)
		{
			this.packages.AddRange(packages);
		}
		
		public void Execute()
		{
			if (PackageScriptRunner != null) {
				ExecuteWithScriptRunner();
			} else {
				ExecuteCore();
			}
		}
		
		protected virtual void ExecuteCore()
		{
			Project.UpdatePackages(this);
			packageManagementEvents.OnParentPackagesUpdated(Packages);
		}
		
		void ExecuteWithScriptRunner()
		{
			using (RunPackageScriptsAction runScriptsAction = CreateRunPackageScriptsAction()) {
				ExecuteCore();
			}
		}
		
		RunPackageScriptsAction CreateRunPackageScriptsAction()
		{
			return CreateRunPackageScriptsAction(PackageScriptRunner, Project);
		}
		
		protected virtual RunPackageScriptsAction CreateRunPackageScriptsAction(
			IPackageScriptRunner scriptRunner,
			IPackageManagementProject project)
		{
			return new RunPackageScriptsAction(scriptRunner, project);
		}
	}
}
