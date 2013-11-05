// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Specifies options for building a single project.
	/// </summary>
	public class ProjectBuildOptions
	{
		BuildTarget target;
		IDictionary<string, string> properties = new SortedList<string, string>(MSBuildInternals.PropertyNameComparer);
		
		public BuildTarget Target {
			get { return target; }
		}
		
		public IDictionary<string, string> Properties {
			get { return properties; }
		}
		
		public ProjectBuildOptions(BuildTarget target)
		{
			this.target = target;
		}
		
		/// <summary>
		/// Specifies the project configuration used for the build.
		/// </summary>
		public string Configuration { get; set; }
		
		/// <summary>
		/// Specifies the project platform used for the build.
		/// </summary>
		public string Platform { get; set; }
		
		/// <summary>
		/// Gets/Sets the verbosity of build output.
		/// </summary>
		public BuildOutputVerbosity BuildOutputVerbosity { get; set; }
	}
	

}
