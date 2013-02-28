// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Evaluation;
using Rhino.Mocks;

namespace TextTemplating.Tests.Helpers
{
	public static class ProjectHelper
	{
		public static TestableProject CreateProject()
		{
			var info = new ProjectCreateInformation();
			info.Solution = MockRepository.GenerateStub<ISolution>();
			info.Solution.Stub(s => s.MSBuildProjectCollection).Return(new ProjectCollection());
			info.ConfigurationMapping = MockRepository.GenerateStub<IConfigurationMapping>();
			info.OutputProjectFileName = FileName.Create(@"d:\projects\MyProject\MyProject.csproj");
			info.ProjectName = "MyProject";
			return new TestableProject(info);
		}
	}
}
