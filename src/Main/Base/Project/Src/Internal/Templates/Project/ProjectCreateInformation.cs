// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class holds all information the language binding need to create
	/// a predefined project for their language, if no project template for a
	/// specific language is avaiable, the language binding shouldn't care about
	/// this stuff.
	/// </summary>
	public class ProjectCreateInformation
	{
		internal List<IProject> createdProjects = new List<IProject>();
		
		public ProjectCreateInformation()
			: this(new IProject[0])
		{
		}
		
		public ProjectCreateInformation(IEnumerable<IProject> projects)
		{
			Platform = "AnyCPU";
			createdProjects.AddRange(projects);
		}		
		
		public ReadOnlyCollection<IProject> CreatedProjects {
			get { return createdProjects.AsReadOnly(); }
		}
		
		public string OutputProjectFileName { get; set; }
		public string Platform { get; set; }
		public string ProjectName { get; set; }
		public string SolutionName { get; set; }
		public string RootNamespace { get; set; }
		public string SolutionPath { get; set; }
		public string ProjectBasePath { get; set; }
		public TargetFramework TargetFramework { get; set; }
		public Solution Solution { get; set; }
	}
}
