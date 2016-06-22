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
