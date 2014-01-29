// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Parser
{
	/// <summary>
	/// Assembly searcher using a project's reference list to resolve assemblies.
	/// </summary>
	public class ProjectAssemblyReferenceSearcher : IAssemblySearcher
	{
		IProject project;

		public ProjectAssemblyReferenceSearcher(IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
		}
		
		public ICSharpCode.Core.FileName FindAssembly(DomAssemblyName fullName)
		{
			// Try to find assembly among solution projects
			IProjectService projectService = SD.GetRequiredService<IProjectService>();
			
			ProjectItem projectItem =
				project.Items.FirstOrDefault(
					item => {
						if (item.ItemType == ItemType.COMReference) {
							// Special handling for COM references: Their assembly names are prefixed with "Interop."
							return fullName.ShortName == "Interop." + item.Include;
						}
						if ((item.ItemType == ItemType.ProjectReference) && (item is ProjectReferenceProjectItem)) {
							// Special handling for project references: Compare with project name instead of file name
							return (((ProjectReferenceProjectItem) item).ProjectName == fullName.ShortName);
						}
						return (item.ItemType == ItemType.Reference) && (item.Include == fullName.ShortName);
					});
			
			if (projectItem != null) {
				if (projectItem is ProjectReferenceProjectItem) {
					// This is a project reference, so FileName delivers the project file instead of assembly binary
					IProject refProject = ((ProjectReferenceProjectItem) projectItem).ReferencedProject;
					if (refProject != null) {
						return refProject.OutputAssemblyFullPath;
					}
				} else {
					return projectItem.FileName;
				}
			}
			
			return null;
		}
	}
}
