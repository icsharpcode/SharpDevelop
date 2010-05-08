// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matt Everson" email="ti.just.me@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace MattEverson.SourceAnalysis
{
	public class CheckCurrentProjectCommand : BuildProject
	{
		public override void StartBuild()
		{
			BuildOptions options = new BuildOptions(BuildTarget.Rebuild, CallbackMethod);
			options.TargetForDependencies = BuildTarget.Build;
			options.ProjectAdditionalProperties["RunSourceAnalysis"] = "true";
			BuildEngine.BuildInGui(this.ProjectToBuild, options);
		}
	}
}
