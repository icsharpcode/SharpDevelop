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
			lock (sharpDevelopProject.SyncRoot) {
				int sharpDevelopProjectImportCount = sharpDevelopProject.MSBuildProjectFile.Imports.Count;
				if (msbuildProjectImportCount > sharpDevelopProjectImportCount) {
					AddNewImports();
				} else if (msbuildProjectImportCount < sharpDevelopProjectImportCount) {
					RemoveMissingImports();
				}
			}
		}
		
		void RemoveMissingImports()
		{
			var importsToRemove = new List<ProjectImportElement>();
			lock (sharpDevelopProject.SyncRoot) {
				foreach (ProjectImportElement import in sharpDevelopProject.MSBuildProjectFile.Imports) {
					if (msbuildProject.Xml.FindImport(import.Project) == null) {
						importsToRemove.Add(import);
					}
				}
			
				foreach (ProjectImportElement importToRemove in importsToRemove) {
					sharpDevelopProject.MSBuildProjectFile.RemoveChild(importToRemove);
				}
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
