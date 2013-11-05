// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Evaluation;
using Rhino.Mocks;

namespace TextTemplating.Tests.Helpers
{
	public static class ProjectHelper
	{
		public static TestableProject CreateProject()
		{
			var info = new ProjectCreateInformation(MockRepository.GenerateStub<ISolution>(), FileName.Create(@"d:\projects\MyProject\MyProject.csproj"));
			info.Solution.Stub(s => s.MSBuildProjectCollection).Return(new ProjectCollection());
			return new TestableProject(info);
		}
	}
}
