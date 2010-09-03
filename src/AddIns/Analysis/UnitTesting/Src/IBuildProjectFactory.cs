// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.UnitTesting
{
	public interface IBuildProjectFactory
	{
		BuildProject CreateBuildProjectBeforeTestRun(IEnumerable<IProject> projects);
	}
}
