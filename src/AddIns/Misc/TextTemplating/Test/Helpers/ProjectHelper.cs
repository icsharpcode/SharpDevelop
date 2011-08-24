// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace TextTemplating.Tests.Helpers
{
	public static class ProjectHelper
	{
		public static TestableProject CreateProject()
		{
			var info = new ProjectCreateInformation();
			info.Solution = new Solution(new MockProjectChangeWatcher());
			info.OutputProjectFileName = @"d:\projects\MyProject\MyProject.csproj";
			info.ProjectName = "MyProject";
			return new TestableProject(info);
		}
	}
}
