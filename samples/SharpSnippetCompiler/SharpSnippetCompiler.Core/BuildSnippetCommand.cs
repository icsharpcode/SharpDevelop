// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
// </file>

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.SharpSnippetCompiler.Core
{
	public class BuildSnippetCommand : BuildProject
	{
		public BuildSnippetCommand(IProject project)
			: base(project)
		{
		}
		
		public override bool CanRunBuild {
			get { return true; }
		}
	}
}
