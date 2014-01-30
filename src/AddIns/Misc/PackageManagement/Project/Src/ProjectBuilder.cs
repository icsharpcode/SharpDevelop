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
using System.Linq;
using System.Threading;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.PackageManagement
{
	public class ProjectBuilder : IProjectBuilder
	{
		ManualResetEvent buildCompleteEvent = new ManualResetEvent(false);
		TimeSpan DefaultBuildTimeout = new TimeSpan(0, 5, 0);
		
		public ProjectBuilder()
		{
		}
		
		public BuildResults BuildResults { get; private set; }
		
		public void Build(IProject project)
		{
			var build = new BuildProject(project);
			build.BuildComplete += BuildComplete;
			buildCompleteEvent.Reset();
			SD.MainThread.InvokeAsyncAndForget(() => build.Run());
			if (buildCompleteEvent.WaitOne(DefaultBuildTimeout)) {
				BuildResults = build.LastBuildResults;
			} else {
				BuildResults = GetBuildTimeoutResult();
			}
			build.BuildComplete -= BuildComplete;
		}
		
		BuildResults GetBuildTimeoutResult()
		{
			var results = new BuildResults { Result = BuildResultCode.Error };
			results.Add(new BuildError(String.Empty, "Timed out waiting for build to complete."));
			return results;
		}
		
		void BuildComplete(object sender, EventArgs e)
		{
			buildCompleteEvent.Set();
		}
	}
}
