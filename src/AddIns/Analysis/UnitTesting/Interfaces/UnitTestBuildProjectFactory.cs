// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestBuildProjectFactory : IBuildProjectFactory
	{
		public BuildProject CreateBuildProjectBeforeTestRun(IEnumerable<IProject> projects)
		{
			return new BuildProjectBeforeExecute(new MultipleProjectBuildable(projects));
		}
	}
}
