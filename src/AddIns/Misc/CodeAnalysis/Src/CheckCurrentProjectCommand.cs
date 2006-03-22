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
	public class CheckCurrentProjectCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			IProject p = ProjectService.CurrentProject;
			if (p == null) return;
			RebuildProject build = new RebuildProject(p);
			build.AdditionalProperties.Add("RunCodeAnalysis", "true");
			build.Run();
		}
	}
}
