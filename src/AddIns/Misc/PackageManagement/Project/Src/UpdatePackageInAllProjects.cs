// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UpdatePackageInAllProjects : IUpdatePackageActions
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
		
		public bool UpdateDependencies { get; set; }
		public IPackageScriptRunner PackageScriptRunner { get; set; }
		
		public IEnumerable<UpdatePackageAction> CreateActions()
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
			UpdatePackageAction action = project.CreateUpdatePackageAction();
			SetUpdatePackageActionProperties(action);
			return action;
		}
		
		void SetUpdatePackageActionProperties(UpdatePackageAction action)
		{
			action.PackageId = packageReference.Id;
			action.PackageScriptRunner = PackageScriptRunner;
			action.PackageVersion = packageReference.Version;
			action.UpdateDependencies = UpdateDependencies;
			action.UpdateIfPackageDoesNotExistInProject = false;
		}
	}
}
