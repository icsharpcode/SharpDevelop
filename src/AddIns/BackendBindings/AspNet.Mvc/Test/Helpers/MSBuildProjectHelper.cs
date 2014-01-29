// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using Rhino.Mocks;

namespace AspNet.Mvc.Tests.Helpers
{
	public static class MSBuildProjectHelper
	{
		public static MSBuildBasedProject CreateCSharpProject()
		{
			var createInfo = new ProjectCreateInformation(FakeSolution.Create(), new FileName(@"d:\projects\MyProject\MyProject.csproj"));
			return new MSBuildBasedProject(createInfo);
		}
	}
}
