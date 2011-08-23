// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SharpDevelopProjectSystemFactory : ISharpDevelopProjectSystemFactory
	{
		public IProjectSystem CreateProjectSystem(MSBuildBasedProject project)
		{
			return new SharpDevelopProjectSystem(project);
		}
	}
}
