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
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementSelectedProjects
	{
		bool? singleProjectSelected;
		IProject singleMSBuildProjectSelected;
		
		public PackageManagementSelectedProjects(IPackageManagementSolution solution)
		{
			this.Solution = solution;
			GetHasSingleProjectSelected();
		}
		
		public IPackageManagementSolution Solution { get; private set; }
		
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
			singleMSBuildProjectSelected = Solution.GetActiveMSBuildProject();
			singleProjectSelected = singleMSBuildProjectSelected != null;
		}
		
		IEnumerable<IProject> GetOpenProjects()
		{
			return Solution.GetMSBuildProjects();
		}
		
		IPackageManagementSelectedProject GetSingleProjectSelected(IPackageFromRepository package)
		{
			return CreateSelectedProject(singleMSBuildProjectSelected, package);
		}
		
		IPackageManagementSelectedProject CreateSelectedProject(IProject msbuildProject, IPackageFromRepository package)
		{
			IPackageManagementProject project = Solution.GetProject(package.Repository, msbuildProject);
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
			return Solution.HasMultipleProjects();
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
			return Path.GetFileName(Solution.FileName);
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
			return Solution.IsPackageInstalled(package);
		}
		
		public IQueryable<IPackage> GetPackages(IPackageRepository sourceRepository)
		{
			if (HasSingleProjectSelected()) {
				IPackageManagementProject project = GetSingleProjectSelected(sourceRepository);
				return project.GetPackages();
			}
			return Solution.GetPackages();
		}
		
		public IPackageManagementProject GetSingleProjectSelected(IPackageRepository repository)
		{
			if (HasSingleProjectSelected()) {
				return Solution.GetProject(repository, singleMSBuildProjectSelected);
			}
			return null;
		}
		
		public IPackageConstraintProvider GetConstraintProvider(IPackageRepository repository)
		{
			if (HasSingleProjectSelected()) {
				return GetSingleProjectSelected(repository).ConstraintProvider;
			}
			return NullConstraintProvider.Instance;
		}
	}
}
