// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project
{
	public class BuildEventArgs : EventArgs
	{
		/// <summary>
		/// The projects to be built.
		/// </summary>
		public readonly IReadOnlyList<IProject> Projects;
		
		/// <summary>
		/// The build options.
		/// </summary>
		public readonly BuildOptions Options;
		
		/// <summary>
		/// Gets the build results.
		/// This property is null for build started events.
		/// </summary>
		public readonly BuildResults Results;
		
		public BuildEventArgs(IReadOnlyList<IProject> projects, BuildOptions options)
			: this(projects, options, null)
		{
		}
		
		public BuildEventArgs(IReadOnlyList<IProject> projects, BuildOptions options, BuildResults results)
		{
			if (projects == null)
				throw new ArgumentNullException("projects");
			if (options == null)
				throw new ArgumentNullException("options");
			this.Projects = projects;
			this.Options = options;
			this.Results = results;
		}
	}
}
