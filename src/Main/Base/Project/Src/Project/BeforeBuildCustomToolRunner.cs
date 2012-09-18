// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Project
{
	public class BeforeBuildCustomToolRunner
	{
		public BeforeBuildCustomToolRunner()
		{
			ProjectService.BuildStarted += ProjectBuildStarted;
		}

		void ProjectBuildStarted(object sender, BuildEventArgs e)
		{
			var projectItems = new BeforeBuildCustomToolProjectItems(e.Buildable);
			RunCustomTool(projectItems.GetProjectItems());
		}
		
		void RunCustomTool(IEnumerable<FileProjectItem> projectItems)
		{
			foreach (FileProjectItem projectItem in projectItems) {
				CustomToolsService.RunCustomTool(projectItem, false);
			}
		}
	}
}
