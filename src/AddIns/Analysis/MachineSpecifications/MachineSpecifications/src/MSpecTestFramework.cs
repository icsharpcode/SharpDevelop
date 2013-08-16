// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	public class MSpecTestFramework : ITestFramework
	{
		public bool IsTestProject(IProject project)
		{
			if (project != null) {
				foreach (ProjectItem item in project.Items) {
					if (IsMSpecAssemblyReference(item))
						return true;
				}
			}
			return false;
		}
		
		public ITestProject CreateTestProject(ITestSolution parentSolution, IProject project)
		{
			return new MSpecTestProject(parentSolution, project);
		}
		
		bool IsMSpecAssemblyReference(ProjectItem projectItem)
		{
			if (projectItem is ReferenceProjectItem) {
				var refProjectItem = projectItem as ReferenceProjectItem;
				string name = refProjectItem.ShortName;
				return MSpecTestProject.MSpecAssemblyName.Equals(name, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
	}
}
