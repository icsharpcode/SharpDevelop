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

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// The service interface for accessing the build engine.
	/// The build engine is responsible for constructing a dependency graph
	/// between the <see cref="IBuildable"/>s, performing a topological sort
	/// and scheduling the actual build.
	/// </summary>
	[SDService]
	public interface IBuildService
	{
		/// <summary>
		/// Builds the specified projects.
		/// If tests are already running, the existing run is cancelled.
		/// </summary>
		/// <remarks>
		/// The build progress will be shown in the SharpDevelop UI;
		/// and the build can be cancelled by the user.
		/// This method can only be used on the main thread.
		/// </remarks>
		Task<BuildResults> BuildAsync(IEnumerable<IProject> projects, BuildOptions options);
		
		Task<BuildResults> BuildAsync(IProject project, BuildOptions options);
		Task<BuildResults> BuildAsync(ISolution solution, BuildOptions options);
		
		/// <summary>
		/// Raised when a build is started.
		/// </summary>
		/// <remarks>This event always occurs on the main thread.</remarks>
		event EventHandler<BuildEventArgs> BuildStarted;
		
		/// <summary>
		/// Raised when a build is finished.
		/// </summary>
		/// <remarks>This event always occurs on the main thread.</remarks>
		event EventHandler<BuildEventArgs> BuildFinished;
		
		/// <summary>
		/// Gets whether a build is currently running.
		/// </summary>
		bool IsBuilding { get; }
		
		/// <summary>
		/// Aborts the current build.
		/// This method has no effect if no build is running.
		/// </summary>
		void CancelBuild();
		
		/// <summary>
		/// Performs a build in the background (not visible in the UI).
		/// </summary>
		/// <param name="buildable">The root buildable</param>
		/// <param name="options">The build options that should be used</param>
		/// <param name="buildFeedbackSink">The build feedback sink that receives the build output.</param>
		/// <param name="progressMonitor">Progress monitor used to report progress about this build.</param>
		/// <remarks>
		/// This method does not set <see cref="IsBuilding"/> and cannot be cancelled using
		/// <see cref="CancelBuild()"/>.
		/// Cancellation is possible using the progress monitor's cancellation token, but
		/// will not cause an <see cref="TaskCanceledException"/> - instead, the build results
		/// will use <c>BuildResultCode.Cancelled</c>.
		/// It does not raise the <see cref="BuildStarted"/>/<see cref="BuildFinished"/> events.
		/// This method is thread-safe, and multiple background builds can run concurrently.
		/// </remarks>
		Task<BuildResults> BuildInBackgroundAsync(IBuildable buildable, BuildOptions options, IBuildFeedbackSink buildFeedbackSink, IProgressMonitor progressMonitor);
	}
}
