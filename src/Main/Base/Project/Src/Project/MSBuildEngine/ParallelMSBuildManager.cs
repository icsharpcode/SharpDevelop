// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Build.Logging;
using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Manages parallel access to MSBuild.
	/// 
	/// MSBuild only allows a single BuildManager to build at one time, and loggers
	/// are specified globally for that BuildManager.
	/// 
	/// This class allows to run multiple indepent builds concurrently. Logging events
	/// will be forwarded to the appropriate logger.
	/// 
	/// Note: due to MSBuild limitations, all projects being built must be from the same ProjectCollection.
	/// SharpDevelop simply uses the predefined ProjectCollection.GlobalProjectCollection.
	/// Code accessing that collection (even if indirectly through MSBuild) should lock on
	/// MSBuildInternals.GlobalProjectCollectionLock.
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
					parameters.Loggers = new ILogger[] {
						new CentralLogger(),
						#if DEBUG
						new ConsoleLogger(LoggerVerbosity.Normal),
						#endif
					};
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
				RunningBuild build = new RunningBuild(eventSource, logger, callback);
				submission.ExecuteAsync(build.OnComplete, null);
				return submission;
			} catch (Exception ex) {
				LoggingService.Warn("Got exception starting build (exception will be rethrown)", ex);
				DisableBuildEngine();
				throw;
			}
		}
		
		sealed class RunningBuild
		{
			ILogger logger;
			BuildSubmissionCompleteCallback callback;
			EventSource eventSource;
			
			public RunningBuild(EventSource eventSource, ILogger logger, BuildSubmissionCompleteCallback callback)
			{
				this.eventSource = eventSource;
				this.logger = logger;
				this.callback = callback;
			}
			
			internal void OnComplete(BuildSubmission submission)
			{
				DisableBuildEngine();
				
				lock (submissionEventSourceMapping) {
					submissionEventSourceMapping.Remove(submission.SubmissionId);
				}
				if (submission.BuildResult.Exception != null) {
					LoggingService.Error(submission.BuildResult.Exception);
					eventSource.ForwardEvent(new BuildErrorEventArgs(null, null, null, 0, 0, 0, 0, submission.BuildResult.Exception.ToString(), null, null));
				}
				if (logger != null)
					logger.Shutdown();
				if (callback != null)
					callback(submission);
			}
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
				try {
					if (e.BuildEventContext == null) {
						if (e is BuildStartedEventArgs || e is BuildFinishedEventArgs) {
							// these two don't have context set, so we cannot forward them
							// this isn't a problem because we know ourselves when we start/stop a build
							return;
						} else {
							throw new InvalidOperationException("BuildEventContext is null on " + e.ToString());
						}
					}
					EventSource redirector;
					lock (submissionEventSourceMapping) {
						if (!submissionEventSourceMapping.TryGetValue(e.BuildEventContext.SubmissionId, out redirector)) {
							LoggingService.Warn("Could not deliver build event: " + e + ":\n" + e.Message);
						}
					}
					if (redirector != null)
						redirector.ForwardEvent(e);
				} catch (Exception ex) {
					MessageService.ShowError(ex, "Error in build logger");
				}
			}
		}
	}
}
