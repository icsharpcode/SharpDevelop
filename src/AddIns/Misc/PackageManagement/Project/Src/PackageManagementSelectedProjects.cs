// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementSelectedProjects
	{
		IPackageManagementSolution solution;
		bool? singleProjectSelected;
		IProject singleMSBuildProjectSelected;
		
		public PackageManagementSelectedProjects(IPackageManagementSolution solution)
		{
			this.solution = solution;
			GetHasSingleProjectSelected();
		}
		
		public IEnumerable<IPackageManagementSelectedProject> GetProjects(IPackageFromRepository package)
		{
			if (HasSingleProjectSelected()) {
				yield return GetSingleProjectSelected(package);
			} else {
				foreach (IProject project in GetOpenProjects()) {
					yield return CreateSelectedProject(project, package);
				}
			}
		}
		
		public bool HasSingleProjectSelected()
		{
			if (!singleProjectSelected.HasValue) {
				GetHasSingleProjectSelected();
			}
			return singleProjectSelected.Value;
		}
		
		void GetHasSingleProjectSelected()
		{
			singleMSBuildProjectSelected = solution.GetActiveMSBuildProject();
			singleProjectSelected = singleMSBuildProjectSelected != null;
		}
		
		IEnumerable<IProject> GetOpenProjects()
		{
			return solution.GetMSBuildProjects();
		}
		
		IPackageManagementSelectedProject GetSingleProjectSelected(IPackageFromRepository package)
		{
			return CreateSelectedProject(singleMSBuildProjectSelected, package);
		}
		
		IPackageManagementSelectedProject CreateSelectedProject(IProject msbuildProject, IPackageFromRepository package)
		{
			IPackageManagementProject project = solution.GetProject(package.Repository, msbuildProject);
			return CreateSelectedProject(project, package);
		}
		
		IPackageManagementSelectedProject CreateSelectedProject(
			IPackageManagementProject project,
			IPackageFromRepository package)
		{
			bool enabled = IsProjectEnabled(project, package);
			bool selected = IsProjectSelected(project, package);
			return new PackageManagementSelectedProject(project, selected, enabled);
		}
		
		protected virtual bool IsProjectSelected(IPackageManagementProject project, IPackageFromRepository package)
		{
			return false;
		}
		
		protected virtual bool IsProjectEnabled(IPackageManagementProject project, IPackageFromRepository package)
		{
			return true;
		}
		
		public bool HasMultipleProjects()
		{
			if (HasSingleProjectSelected()) {
				return false;
			}
			return solution.HasMultipleProjects();
		}
		
		public string SelectionName {
			get { return GetSelectionName(); }
		}
		
		string GetSelectionName()
		{
			if (HasSingleProjectSelected()) {
				return GetSingleProjectSelectedName();
			}
			return GetSolutionFileNameWithoutFullPath();
		}
		
		string GetSingleProjectSelectedName()
		{
			return singleMSBuildProjectSelected.Name;
		}
		
		string GetSolutionFileNameWithoutFullPath()
		{
			return Path.GetFileName(solution.FileName);
		}
		
		/// <summary>
		/// Returns true if the package is installed in the selected projects.
		/// </summary>
		public bool IsPackageInstalled(IPackageFromRepository package)
		{
			if (HasSingleProjectSelected()) {
				IPackageManagementProject project = GetSingleProjectSelected(package.Repository);
				return project.IsPackageInstalled(package);
			}
			return IsPackageInstalledInSolution(package);
		}
		
		public bool IsPackageInstalledInSolution(IPackage package)
		{
			return solution.IsPackageInstalled(package);
		}
		
		public IQueryable<IPackage> GetPackagesInstalledInSolution()
		{
			return solution.GetPackages();
		}
		
		public IQueryable<IPackage> GetInstalledPackages(IPackageRepository sourceRepository)
		{
			if (HasSingleProjectSelected()) {
				IPackageManagementProject project = GetSingleProjectSelected(sourceRepository);
				return project.GetPackages();
			}
			return GetPackagesInstalledInSolution();
		}
		
		public IPackageManagementProject GetSingleProjectSelected(IPackageRepository repository)
		{
			if (HasSingleProjectSelected()) {
				return solution.GetProject(repository, singleMSBuildProjectSelected);
			}
			return null;
		}
	}
}
