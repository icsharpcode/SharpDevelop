// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
