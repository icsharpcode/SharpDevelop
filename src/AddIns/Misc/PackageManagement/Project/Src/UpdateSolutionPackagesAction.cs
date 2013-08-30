// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UpdateSolutionPackagesAction : IUpdatePackagesAction
	{
		List<IPackageFromRepository> packages = new List<IPackageFromRepository>();
		List<PackageOperation> operations = new List<PackageOperation>();
		List<IPackageManagementProject> projects;
		IPackageManagementEvents packageManagementEvents;
		
		public UpdateSolutionPackagesAction(
			IPackageManagementSolution solution,
			IPackageManagementEvents packageManagementEvents)
		{
			this.Solution = solution;
			this.UpdateDependencies = true;
			this.packageManagementEvents = packageManagementEvents;
		}
		
		public IPackageManagementSolution Solution { get; private set; }
		public IPackageScriptRunner PackageScriptRunner { get; set; }
		public bool UpdateDependencies { get; set; }
		public bool AllowPrereleaseVersions { get; set; }
		public ILogger Logger { get; set; }
		
		public IEnumerable<PackageOperation> Operations {
			get { return operations; }
		}
		
		public IEnumerable<IPackageFromRepository> Packages {
			get { return packages; }
		}
		
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
		
		void ExecuteWithScriptRunner()
		{
			using (RunAllProjectPackageScriptsAction runScriptsAction = CreateRunPackageScriptsAction()) {
				ExecuteCore();
			}
		}
		
		RunAllProjectPackageScriptsAction CreateRunPackageScriptsAction()
		{
			return CreateRunPackageScriptsAction(PackageScriptRunner, GetProjects());
		}
		
		void ExecuteCore()
		{
			RunPackageOperations();
			UpdatePackageReferences();
			packageManagementEvents.OnParentPackagesUpdated(Packages);
		}
		
		void RunPackageOperations()
		{
			IPackageManagementProject project = GetProjects().First();
			project.RunPackageOperations(operations);
		}
		
		IEnumerable<IPackageManagementProject> GetProjects()
		{
			if (projects == null) {
				IPackageFromRepository package = packages.First();
				projects = Solution
					.GetProjects(package.Repository)
					.Select(project => {
						project.Logger = Logger;
						return project;
					})
					.ToList();
			}
			return projects;
		}
		
		void UpdatePackageReferences()
		{
			foreach (IPackageManagementProject project in GetProjects()) {
				foreach (IPackageFromRepository package in packages) {
					if (project.HasOlderPackageInstalled(package)) {
						project.UpdatePackageReference(package, this);
					}
				}
			}
		}
		
		protected virtual RunAllProjectPackageScriptsAction CreateRunPackageScriptsAction(
			IPackageScriptRunner scriptRunner,
			IEnumerable<IPackageManagementProject> projects)
		{
			return new RunAllProjectPackageScriptsAction(scriptRunner, projects);
		}
	}
}
