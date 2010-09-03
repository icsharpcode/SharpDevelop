// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockBuildProjectFactory : IBuildProjectFactory
	{
		List<MockBuildProjectBeforeTestRun> buildProjectInstances = new List<MockBuildProjectBeforeTestRun>();
		
		public BuildProject CreateBuildProjectBeforeTestRun(IEnumerable<IProject> projects)
		{
			MockBuildProjectBeforeTestRun buildProject = RemoveBuildProjectInstance();
			if (buildProject != null) {
				buildProject.Projects = projects.ToArray();
			}
			return buildProject;
		}
		
		MockBuildProjectBeforeTestRun RemoveBuildProjectInstance()
		{
			if (buildProjectInstances.Count > 0) {
				MockBuildProjectBeforeTestRun buildProject = buildProjectInstances[0];
				buildProjectInstances.RemoveAt(0);
				return buildProject;
			}
			return null;
		}
		
		public void AddBuildProjectBeforeTestRun(MockBuildProjectBeforeTestRun buildBeforeTestRun)
		{
			buildProjectInstances.Add(buildBeforeTestRun);
		}
	}
}
