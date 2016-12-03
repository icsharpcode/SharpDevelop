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
			lock (sharpDevelopProject.SyncRoot) {
				return sharpDevelopProject
					.MSBuildProjectFile
					.Properties
					.Where(property => String.Equals(property.Name, msbuildProjectProperty.Name, StringComparison.OrdinalIgnoreCase))
					.ToList();
			}
		}
		
		void AddPropertyToSharpDevelopProject(ProjectPropertyElement msbuildProjectProperty)
		{
			SetPropertyInSharpDevelopProject(msbuildProjectProperty);
			result.AddPropertyAdded(msbuildProjectProperty.Name);
		}
		
		void SetPropertyInSharpDevelopProject(ProjectPropertyElement msbuildProjectProperty)
		{
			sharpDevelopProject.SetProperty(msbuildProjectProperty.Name, msbuildProjectProperty.Value, treatPropertyValueAsLiteral: false);
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
