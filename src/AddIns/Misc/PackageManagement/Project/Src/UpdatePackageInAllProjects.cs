// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UpdatePackageInAllProjects : UpdatePackageActions
	{
		IPackageManagementSolution solution;
		IPackageRepository sourceRepository;
		List<IPackageManagementProject> projects;
		PackageReference packageReference;
		
		public UpdatePackageInAllProjects(
			PackageReference packageReference,
			IPackageManagementSolution solution,
			IPackageRepository sourceRepository)
		{
			this.packageReference = packageReference;
			this.solution = solution;
			this.sourceRepository = sourceRepository;
		}
		
		public override IEnumerable<UpdatePackageAction> CreateActions()
		{
			GetProjects();
			foreach (IPackageManagementProject project in projects) {
				yield return CreateUpdatePackageAction(project);
			}
		}
		
		void GetProjects()
		{
			projects = new List<IPackageManagementProject>();
			projects.AddRange(solution.GetProjects(sourceRepository));
		}
		
		UpdatePackageAction CreateUpdatePackageAction(IPackageManagementProject project)
		{
			UpdatePackageAction action = CreateDefaultUpdatePackageAction(project);
			action.PackageId = packageReference.Id;
			action.PackageVersion = packageReference.Version;
			return action;
		}
	}
}
