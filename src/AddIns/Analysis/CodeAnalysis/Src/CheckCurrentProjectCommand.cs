// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.CodeAnalysis
{
	public class CheckCurrentProjectCommand : BuildProject
	{
		public override void StartBuild()
		{
			BuildOptions options = new BuildOptions(BuildTarget.Rebuild, CallbackMethod);
			options.TargetForDependencies = BuildTarget.Build;
			options.ProjectAdditionalProperties["RunCodeAnalysis"] = "true";
			BuildEngine.BuildInGui(this.ProjectToBuild, options);
		}
	}
}
