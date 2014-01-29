// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Templates
{
	/// <summary>
	/// Represents the result of creating new projects using a project template.
	/// </summary>
	public class ProjectTemplateResult
	{
		readonly ProjectTemplateOptions options;
		readonly IList<IProject> newProjects = new List<IProject>();
		
		public ProjectTemplateResult(ProjectTemplateOptions options)
		{
			if (options == null)
				throw new ArgumentNullException("options");
			this.options = options;
		}
		
		public ProjectTemplateOptions Options {
			get { return options; }
		}
		
		public IList<IProject> NewProjects {
			get { return newProjects; }
		}
	}
}
