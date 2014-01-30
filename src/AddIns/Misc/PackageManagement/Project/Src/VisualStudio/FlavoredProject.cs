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
