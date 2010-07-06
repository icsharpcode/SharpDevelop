// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestBuildProjectFactory : IBuildProjectFactory
	{
		public BuildProject CreateBuildProjectBeforeTestRun(IEnumerable<IProject> projects)
		{
			return new BuildProjectBeforeExecute(new MultipleProjectBuildable(projects));
		}
	}
}
