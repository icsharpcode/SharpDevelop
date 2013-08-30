// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class MSBuildProjectImportsMerger
	{
		IPackageManagementProjectService projectService;
		Project msbuildProject;
		MSBuildBasedProject sharpDevelopProject;
		MSBuildProjectImportsMergeResult result = new MSBuildProjectImportsMergeResult();
		
		public MSBuildProjectImportsMerger(Project msbuildProject, MSBuildBasedProject sharpDevelopProject)
			: this(msbuildProject, sharpDevelopProject, new PackageManagementProjectService())
		{
		}
		
		public MSBuildProjectImportsMerger(
			Project msbuildProject,
			MSBuildBasedProject sharpDevelopProject,
			IPackageManagementProjectService projectService)
		{
			this.msbuildProject = msbuildProject;
			this.sharpDevelopProject = sharpDevelopProject;
			this.projectService = projectService;
		}
		
		public MSBuildProjectImportsMergeResult Result {
			get { return result; }
		}
		
		public void Merge()
		{
			int msbuildProjectImportCount = msbuildProject.Xml.Imports.Count;
			int sharpDevelopProjectImportCount = sharpDevelopProject.MSBuildProjectFile.Imports.Count;
			if (msbuildProjectImportCount > sharpDevelopProjectImportCount) {
				AddNewImports();
			} else if (msbuildProjectImportCount < sharpDevelopProjectImportCount) {
				RemoveMissingImports();
			}
		}
		
		void RemoveMissingImports()
		{
			var importsToRemove = new List<ProjectImportElement>();
			foreach (ProjectImportElement import in sharpDevelopProject.MSBuildProjectFile.Imports) {
				if (msbuildProject.Xml.FindImport(import.Project) == null) {
					importsToRemove.Add(import);
				}
			}
			
			foreach (ProjectImportElement importToRemove in importsToRemove) {
				sharpDevelopProject.MSBuildProjectFile.RemoveChild(importToRemove);
			}
			
			result.AddProjectImportsRemoved(importsToRemove);
			
			projectService.Save(sharpDevelopProject);
		}
		
		void AddNewImports()
		{
			var importsToAdd = new List<ProjectImportElement>();
			foreach (ProjectImportElement import in msbuildProject.Xml.Imports) {
				if (!sharpDevelopProject.ImportExists(import.Project)) {
					importsToAdd.Add(import);
				}
			}
			
			foreach (ProjectImportElement importToAdd in importsToAdd) {
				sharpDevelopProject.AddImportIfMissing(importToAdd.Project, ProjectImportLocation.Bottom);
			}
			
			result.AddProjectImportsAdded(importsToAdd);
			
			projectService.Save(sharpDevelopProject);
		}
	}
}
