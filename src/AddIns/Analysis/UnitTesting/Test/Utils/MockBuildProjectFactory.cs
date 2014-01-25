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
