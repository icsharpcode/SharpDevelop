// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestFramework : ITestFramework
	{
		public bool IsTestProject(IProject project)
		{
			if (project == null)
				return false;
			
			foreach (ProjectItem item in project.Items) {
				if (item.IsMSTestAssemblyReference()) {
					return true;
				}
			}
			return false;
		}
		
		public ITestProject CreateTestProject(ITestSolution parentSolution, IProject project)
		{
			return new MSTestProject(project);
		}
	}
}