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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	sealed class BuildService : IBuildService
	{
		readonly BuildModifiedProjectsOnlyService buildModifiedProjectsOnly;
		
		public BuildService()
		{
			this.buildModifiedProjectsOnly = new BuildModifiedProjectsOnlyService(this, SD.ProjectService);
		}
		
		public event EventHandler<BuildEventArgs> BuildStarted;
		public event EventHandler<BuildEventArgs> BuildFinished;
		
		CancellationTokenSource guiBuildCancellation;
		
		public bool IsBuilding {
			get { return guiBuildCancellation != null; }
		}
		
		public void CancelBuild()
		{
			if (guiBuildCancellation != null)
				guiBuildCancellation.Cancel();
		}
		
		public async Task<BuildResults> BuildAsync(IEnumerable<IProject> projects, BuildOptions options)
		{
			if (projects == null)
				throw new ArgumentNullException("projects");
			if (options == null)
				throw new ArgumentNullException("options");
			SD.MainThread.VerifyAccess();
			if (guiBuildCancellation != null) {
				BuildResults results = new BuildResults();
				SD.StatusBar.SetMessage(ResourceService.GetString("MainWindow.CompilerMessages.MSBuildAlreadyRunning"));
				BuildError error = new BuildError(null, ResourceService.GetString("MainWindow.CompilerMessages.MSBuildAlreadyRunning"));
				results.Add(error);
				TaskService.Add(new SDTask(error));
				results.Result = BuildResultCode.MSBuildAlreadyRunning;
				return results;
			}
			var projectsList = projects.ToList();
			guiBuildCancellation = new CancellationTokenSource();
			try {
				using (var progressMonitor = SD.StatusBar.CreateProgressMonitor(guiBuildCancellation.Token)) {
					if (BuildStarted != null)
						BuildStarted(this, new BuildEventArgs(projectsList, options));
					
					var trackedFeature = SD.AnalyticsMonitor.TrackFeature("ICSharpCode.SharpDevelop.Project.BuildEngine.Build");
					SD.StatusBar.SetMessage(StringParser.Parse("${res:MainWindow.CompilerMessages.BuildVerb}..."));
					IBuildable buildable;
					if (projectsList.Count == 1)
						buildable = projectsList[0];
					else
						buildable = new MultipleProjectBuildable(projectsList);
					
					buildable = buildModifiedProjectsOnly.WrapBuildable(buildable, options.BuildDetection);
					
					var sink = new UIBuildFeedbackSink(SD.OutputPad.BuildCategory, SD.StatusBar);
					// Actually run the build:
					var results = await BuildEngine.BuildAsync(buildable, options, sink, progressMonitor);
					
					string message;
					if (results.Result == BuildResultCode.Cancelled) {
						message = "${res:MainWindow.CompilerMessages.BuildCancelled}";
					} else {
						if (results.Result == BuildResultCode.Success)
							message = "${res:MainWindow.CompilerMessages.BuildFinished}";
						else
							message = "${res:MainWindow.CompilerMessages.BuildFailed}";
						
						if (results.ErrorCount > 0)
							message += " " + results.ErrorCount + " error(s)";
						if (results.WarningCount > 0)
							message += " " + results.WarningCount + " warning(s)";
					}
					SD.StatusBar.SetMessage(message);
					trackedFeature.EndTracking();
					if (BuildFinished != null)
						BuildFinished(this, new BuildEventArgs(projectsList, options, results));
					
					return results;
				}
			} finally {
				guiBuildCancellation = null;
			}
		}
		
		public Task<BuildResults> BuildAsync(IProject project, BuildOptions options)
		{
			if (project != null)
				return BuildAsync(new[] { project }, options);
			else
				return Task.FromResult(new BuildResults { Result = BuildResultCode.Error });
		}
		
		public Task<BuildResults> BuildAsync(ISolution solution, BuildOptions options)
		{
			if (solution != null) {
				var solutionConfiguration = new ConfigurationAndPlatform(options.SolutionConfiguration ?? solution.ActiveConfiguration.Configuration,
			                                                         options.SolutionPlatform ?? solution.ActiveConfiguration.Platform);
				return BuildAsync(solution.Projects.Where(p => p.ConfigurationMapping.IsBuildEnabled(solutionConfiguration)), options);
			} else {
				return Task.FromResult(new BuildResults { Result = BuildResultCode.Error });
			}
		}
		
		public Task<BuildResults> BuildInBackgroundAsync(IBuildable buildable, BuildOptions options, IBuildFeedbackSink buildFeedbackSink, IProgressMonitor progressMonitor)
		{
			return BuildEngine.BuildAsync(buildable, options, buildFeedbackSink, progressMonitor);
		}
	}
}
