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
using System.Threading.Tasks;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// A project or solution.
	/// The IBuildable interface members are thread-safe.
	/// </summary>
	public interface IBuildable
	{
		/// <summary>
		/// Gets the list of projects on which this project depends.
		/// This method is thread-safe.
		/// </summary>
		IEnumerable<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions);
		
		/// <summary>
		/// Starts building the project using the specified options.
		/// This member must be implemented thread-safe.
		/// </summary>
		Task<bool> BuildAsync(ProjectBuildOptions options, IBuildFeedbackSink feedbackSink, IProgressMonitor progressMonitor);
		
		/// <summary>
		/// Gets the name of the buildable item.
		/// This property is thread-safe.
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// Creates the project-specific build options.
		/// This member must be implemented thread-safe.
		/// </summary>
		/// <param name="options">The global build options.</param>
		/// <param name="isRootBuildable">Specifies whether this project is the main buildable item.
		/// The root buildable is the buildable for which <see cref="BuildOptions.ProjectTarget"/> and <see cref="BuildOptions.ProjectAdditionalProperties"/> apply.
		/// The dependencies of that root buildable are the non-root buildables.</param>
		/// <returns>The project-specific build options.</returns>
		ProjectBuildOptions CreateProjectBuildOptions(BuildOptions options, bool isRootBuildable);
	}
}
