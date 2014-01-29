// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Templates
{
	public class ProjectTemplateOptions
	{
		public DirectoryName ProjectBasePath { get; set; }
		public ISolution Solution { get; set; }
		public ISolutionFolder SolutionFolder { get; set; }
		public string ProjectName { get; set; }
		public TargetFramework TargetFramework { get; set; }
		
		internal string SolutionName { get { return Solution.Name; } }
	}
}
