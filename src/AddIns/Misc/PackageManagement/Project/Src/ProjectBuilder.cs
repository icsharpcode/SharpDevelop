// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Threading;
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
			WorkbenchSingleton.SafeThreadAsyncCall(() => build.Run());
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
