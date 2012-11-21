// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSharpCode.PackageManagement.VisualStudio
{
	public class VsSolution : MarshalByRefObject, IVsSolution
	{
		IPackageManagementProjectService projectService;
		
		public VsSolution()
			: this(new PackageManagementProjectService())
		{
		}
		
		public VsSolution(IPackageManagementProjectService projectService)
		{
			this.projectService = projectService;
		}
		
		public int GetProjectOfUniqueName(string uniqueName, out IVsHierarchy hierarchy)
		{
			hierarchy = null;
			MSBuildBasedProject project = FindProject(uniqueName);
			if (project != null) {
				hierarchy = new FlavoredProject(project);
				return VsConstants.S_OK;
			}
			return VsConstants.E_FAIL;
		}
		
		MSBuildBasedProject FindProject(string uniqueName)
		{
			return projectService
				.GetOpenProjects()
				.SingleOrDefault(project => ProjectUniqueNameMatches(project, uniqueName)) as MSBuildBasedProject;
		}
		
		bool ProjectUniqueNameMatches(IProject msbuildProject, string uniqueName)
		{
			var project = new Project(msbuildProject as MSBuildBasedProject);
			return IsCaseInsensitiveMatch(project.UniqueName, uniqueName);
		}
		
		bool IsCaseInsensitiveMatch(string a, string b)
		{
			return String.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}
	}
}
