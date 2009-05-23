// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Manages parallel access to MSBuild.
	/// 
	/// MSBuild only allows a single BuildManager to build at one time, and loggers
	/// are specified globally for that BuildManager.
	/// This class allows 
	/// </summary>
	public static class ParallelMSBuildManager
	{
		#region Manage StartBuild/EndBuild of BuildManager.DefaultBuildManager
		static readonly object enableDisableLock = new object();
		static int enableCount;
		
		static void EnableBuildEngine()
		{
			lock (enableDisableLock) {
				if (enableCount == 0) {
					BuildParameters parameters = new BuildParameters();
					parameters.Loggers = new ILogger[] { new CentralLogger() };
					BuildManager.DefaultBuildManager.BeginBuild(parameters);		
				}
				enableCount++;
			}
		}
		
		static void DisableBuildEngine()
		{
			lock (enableDisableLock) {
				enableCount--;
				if (enableCount == 0) {
					BuildManager.DefaultBuildManager.EndBuild();
					BuildManager.DefaultBuildManager.ResetCaches();
				}
			}
		}
		#endregion
		
		static readonly Dictionary<int, EventSource> submissionEventSourceMapping = new Dictionary<int, EventSource>();
		
		/// <summary>
		/// Starts building.
		/// </summary>
		/// <param name="requestData">The requested build.</param>
		/// <param name="logger">The logger that received build output.</param>
		/// <param name="callback">Callback that is run when the build is complete</param>
		/// <returns>The build submission that was started.</returns>
		public static BuildSubmission StartBuild(BuildRequestData requestData, ILogger logger, BuildSubmissionCompleteCallback callback)
		{
			EnableBuildEngine();
			try {
				BuildSubmission submission = BuildManager.DefaultBuildManager.PendBuildRequest(requestData);
				EventSource eventSource = new EventSource();
				if (logger != null)
					logger.Initialize(eventSource);
				lock (submissionEventSourceMapping) {
					submissionEventSourceMapping.Add(submission.SubmissionId, eventSource);
				}
				// TODO: when will the logger be shut down?
				submission.ExecuteAsync(OnComplete, callback);
				return submission;
			} catch (Exception ex) {
				LoggingService.Warn("Got exception starting build (exception will be rethrown)", ex);
				DisableBuildEngine();
				throw;
			}
		}
		
		static void OnComplete(BuildSubmission submission)
		{
			DisableBuildEngine();
			
			lock (submissionEventSourceMapping) {
				submissionEventSourceMapping.Remove(submission.SubmissionId);
			}
			BuildSubmissionCompleteCallback callback = submission.AsyncContext as BuildSubmissionCompleteCallback;
			if (callback != null)
				callback(submission);
		}
		
		sealed class CentralLogger : INodeLogger, IEventRedirector
		{	
			public void Initialize(IEventSource eventSource, int nodeCount)
			{
				Initialize(eventSource);
			}
			
			public void Initialize(IEventSource eventSource)
			{
				eventSource.AnyEventRaised += (sender, e) => ForwardEvent(e);
			}
			
			public void Shutdown()
			{
			}
			
			public string Parameters { get; set; }
			public LoggerVerbosity Verbosity { get; set; }
			
			public void ForwardEvent(Microsoft.Build.Framework.BuildEventArgs e)
			{
				if (e.BuildEventContext == null) {
					if (e is BuildStartedEventArgs || e is BuildFinishedEventArgs) {
						// these two don't have context set, so we cannot forward them
						// this isn't a problem because we know ourselves when we start/stop a build
						return;
					} else {
						throw new InvalidOperationException("BuildEventContext is null on " + e.ToString());
					}
				}
				IEventRedirector redirector;
				lock (submissionEventSourceMapping) {
					redirector = submissionEventSourceMapping[e.BuildEventContext.SubmissionId];
				}
				redirector.ForwardEvent(e);
			}
		}
	}
}
