// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class MSBuildProjectPropertiesMerger
	{
		IPackageManagementProjectService projectService;
		Project msbuildProject;
		MSBuildBasedProject sharpDevelopProject;
		MSBuildProjectPropertiesMergeResult result = new MSBuildProjectPropertiesMergeResult();
		
		public MSBuildProjectPropertiesMerger(Project msbuildProject, MSBuildBasedProject sharpDevelopProject)
			: this(msbuildProject, sharpDevelopProject, new PackageManagementProjectService())
		{
		}
		
		public MSBuildProjectPropertiesMerger(
			Project msbuildProject,
			MSBuildBasedProject sharpDevelopProject,
			IPackageManagementProjectService projectService)
		{
			this.msbuildProject = msbuildProject;
			this.sharpDevelopProject = sharpDevelopProject;
			this.projectService = projectService;
		}
		
		public MSBuildProjectPropertiesMergeResult Result {
			get { return result; }
		}
		
		public void Merge()
		{
			foreach (ProjectPropertyElement property in msbuildProject.Xml.Properties) {
				UpdateProperty(property);
			}
			
			projectService.Save(sharpDevelopProject);
		}
		
		void UpdateProperty(ProjectPropertyElement msbuildProjectProperty)
		{
			List<ProjectPropertyElement> sharpDevelopProjectProperties = FindSharpDevelopProjectProperties(msbuildProjectProperty);
			if (sharpDevelopProjectProperties.Count > 1) {
				// Ignore. Currently do not handle properties defined inside
				// property groups with conditions (e.g. OutputPath)
			} else if (!sharpDevelopProjectProperties.Any()) {
				AddPropertyToSharpDevelopProject(msbuildProjectProperty);
			} else if (HasMSBuildProjectPropertyBeenUpdated(msbuildProjectProperty, sharpDevelopProjectProperties.First())) {
				UpdatePropertyInSharpDevelopProject(msbuildProjectProperty);
			}
		}
		
		List<ProjectPropertyElement> FindSharpDevelopProjectProperties(ProjectPropertyElement msbuildProjectProperty)
		{
			return sharpDevelopProject
				.MSBuildProjectFile
				.Properties
				.Where(property => String.Equals(property.Name, msbuildProjectProperty.Name, StringComparison.OrdinalIgnoreCase))
				.ToList();
		}
		
		void AddPropertyToSharpDevelopProject(ProjectPropertyElement msbuildProjectProperty)
		{
			SetPropertyInSharpDevelopProject(msbuildProjectProperty);
			result.AddPropertyAdded(msbuildProjectProperty.Name);
		}
		
		void SetPropertyInSharpDevelopProject(ProjectPropertyElement msbuildProjectProperty)
		{
			sharpDevelopProject.SetProperty(msbuildProjectProperty.Name, msbuildProjectProperty.Value);
		}
		
		bool HasMSBuildProjectPropertyBeenUpdated(ProjectPropertyElement msbuildProjectProperty, ProjectPropertyElement sharpDevelopProjectProperty)
		{
			return msbuildProjectProperty.Value != sharpDevelopProjectProperty.Value;
		}
		
		void UpdatePropertyInSharpDevelopProject(ProjectPropertyElement msbuildProjectProperty)
		{
			SetPropertyInSharpDevelopProject(msbuildProjectProperty);
			result.AddPropertyUpdated(msbuildProjectProperty.Name);
		}
	}
}
