// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Shell.Flavor
{
	public class FlavoredProject : MarshalByRefObject, IVsAggregatableProject, IVsHierarchy
	{
		MSBuildBasedProject project;
		
		public FlavoredProject(MSBuildBasedProject project)
		{
			this.project = project;
		}
		
		public int GetAggregateProjectTypeGuids(out string projTypeGuids)
		{
			projTypeGuids = GetProjectTypeGuidsFromProject();
			if (projTypeGuids == null) {
				projTypeGuids = GetProjectTypeGuidsBasedOnProjectFileExtension();
			}
			return VsConstants.S_OK;
		}
		
		string GetProjectTypeGuidsFromProject()
		{
			return project.GetUnevalatedProperty("ProjectTypeGuids");
		}
		
		string GetProjectTypeGuidsBasedOnProjectFileExtension()
		{
			var projectType = ProjectType.GetProjectType(project);
			if (projectType == ProjectType.CSharp) {
				return ProjectTypeGuids.CSharp.ToString();
			} else if (projectType == ProjectType.VB) {
				return ProjectTypeGuids.VB.ToString();
			}
			return String.Empty;
		}
	}
}
