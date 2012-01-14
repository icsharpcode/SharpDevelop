// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UpdateAllPackagesInSolution : UpdatePackageActions
	{
		IPackageManagementSolution solution;
		IPackageRepository sourceRepository;
		List<IPackageManagementProject> projects;
		
		public UpdateAllPackagesInSolution(
			IPackageManagementSolution solution,
			IPackageRepository sourceRepository)
		{
			this.solution = solution;
			this.sourceRepository = sourceRepository;
		}
		
		public override IEnumerable<UpdatePackageAction> CreateActions()
		{
			GetProjects();
			foreach (IPackage package in GetPackages()) {
				foreach (IPackageManagementProject project in projects) {
					yield return CreateAction(project, package);
				}
			}
		}
		
		void GetProjects()
		{
			projects = new List<IPackageManagementProject>();
			projects.AddRange(solution.GetProjects(sourceRepository));
		}
		
		IEnumerable<IPackage> GetPackages()
		{
			return solution.GetPackagesInReverseDependencyOrder();
		}
		
		UpdatePackageAction CreateAction(IPackageManagementProject project, IPackage package)
		{
			UpdatePackageAction action = CreateDefaultUpdatePackageAction(project);
			action.PackageId = package.Id;
			return action;
		}
	}
}
