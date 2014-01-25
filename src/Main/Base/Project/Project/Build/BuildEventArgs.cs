// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
