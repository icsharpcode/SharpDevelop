// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Core;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using System.Threading;

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
	/// </summary>
	public sealed class ParallelMSBuildManager : IDisposable
	{
		/// <summary>
		/// MSBuild only allows a single build to run at one time - so if multiple ParallelMSBuildManager
		/// are present, we synchronize them using this global lock.
		/// </summary>
		static readonly object globalBuildEngineLock = new object();
		
		static bool globalBuildEngineLockTaken;
		
		static void EnterGlobalBuildEngineLock()
		{
			lock (globalBuildEngineLock) {
				while (globalBuildEngineLockTaken)
					Monitor.Wait(globalBuildEngineLock);
				globalBuildEngineLockTaken = true;
			}
		}
		
		static void LeaveGlobalBuildEngineLock()
		{
			lock (globalBuildEngineLock) {
				Debug.Assert(globalBuildEngineLockTaken);
				globalBuildEngineLockTaken = false;
				Monitor.Pulse(globalBuildEngineLock);
			}
		}
		
		public ProjectCollection ProjectCollection { get; private set; }
		
		/// <summary>
		/// Creates a new ParallelMSBuildManager for the project collection.
		/// WARNING: only a single ParallelMSBuildManager can build at a time, others will wait.
		/// Ensure you dispose the ParallelMSBuildManager when you are done with it, otherwise all future
		/// ParallelMSBuildManagers will deadlock!
		/// </summary>
		public ParallelMSBuildManager(ProjectCollection projectCollection)
		{
			if (projectCollection == null)
				throw new ArgumentNullException("projectCollection");
			this.ProjectCollection = projectCollection;
		}
		
		#region Manage StartBuild/EndBuild
		readonly object enableDisableLock = new object();
		bool buildIsRunning;
		
		void StartupBuildEngine()
		{
			lock (enableDisableLock) {
				if (buildIsRunning)
					return;
				buildIsRunning = true;
				
				LoggingService.Info("ParallelMSBuildManager: waiting for start permission...");
				EnterGlobalBuildEngineLock();
				
				LoggingService.Info("ParallelMSBuildManager: got start permisson, starting...");
				BuildParameters parameters = new BuildParameters(this.ProjectCollection);
				parameters.Loggers = new ILogger[] {
					new CentralLogger(this),
					#if DEBUG
					new ConsoleLogger(LoggerVerbosity.Normal),
					#endif
				};
				parameters.EnableNodeReuse = false;
				// parallel build seems to break in-memory modifications of the project (additionalTargetFiles+_ComputeNonExistentFileProperty),
				// so we keep it disabled for the moment.
				parameters.MaxNodeCount = BuildOptions.DefaultParallelProjectCount;
				BuildManager.DefaultBuildManager.BeginBuild(parameters);
			}
		}
		
		
		/// <summary>
		/// Shuts down the build engine and allows other managers to build.
		/// </summary>
		public void Dispose()
		{
			lock (enableDisableLock) {
				if (!buildIsRunning)
					return;
				buildIsRunning = false;
				try {
					LoggingService.Info("ParallelMSBuildManager shutting down...");
					BuildManager.DefaultBuildManager.EndBuild();
					BuildManager.DefaultBuildManager.ResetCaches();
					LoggingService.Info("ParallelMSBuildManager shut down!");
				} finally {
					LeaveGlobalBuildEngineLock();
				}
			}
		}
		#endregion
		
		readonly Dictionary<int, EventSource> submissionEventSourceMapping = new Dictionary<int, EventSource>();
		
		/// <summary>
		/// Starts building.
		/// This method blocks if another ParallelMSBuildManager is currently building.
		/// However, it doe
		/// </summary>
		/// <param name="requestData">The requested build.</param>
		/// <param name="logger">The logger that received build output.</param>
		/// <param name="callback">Callback that is run when the build is complete</param>
		/// <returns>The build submission that was started.</returns>
		public BuildSubmission StartBuild(BuildRequestData requestData, IEnumerable<ILogger> loggers, BuildSubmissionCompleteCallback callback)
		{
			if (requestData == null)
				throw new ArgumentNullException("requestData");
			ILogger[] loggersArray;
			if (loggers == null)
				loggersArray = new ILogger[0];
			else
				loggersArray = loggers.ToArray(); // iterate through logger enumerable once
			
			StartupBuildEngine();
			BuildSubmission submission = BuildManager.DefaultBuildManager.PendBuildRequest(requestData);
			EventSource eventSource = new EventSource();
			foreach (ILogger logger in loggersArray) {
				if (logger != null)
					logger.Initialize(eventSource);
			}
			lock (submissionEventSourceMapping) {
				submissionEventSourceMapping.Add(submission.SubmissionId, eventSource);
			}
			RunningBuild build = new RunningBuild(this, eventSource, loggersArray, callback);
			submission.ExecuteAsync(build.OnComplete, null);
			return submission;
		}
		
		sealed class RunningBuild
		{
			ParallelMSBuildManager manager;
			ILogger[] loggers;
			BuildSubmissionCompleteCallback callback;
			EventSource eventSource;
			
			public RunningBuild(ParallelMSBuildManager manager, EventSource eventSource, ILogger[] loggers, BuildSubmissionCompleteCallback callback)
			{
				this.manager = manager;
				this.eventSource = eventSource;
				this.loggers = loggers;
				this.callback = callback;
			}
			
			internal void OnComplete(BuildSubmission submission)
			{
				lock (manager.submissionEventSourceMapping) {
					manager.submissionEventSourceMapping.Remove(submission.SubmissionId);
				}
				if (submission.BuildResult.Exception != null) {
					LoggingService.Error(submission.BuildResult.Exception);
					eventSource.ForwardEvent(new BuildErrorEventArgs(null, null, null, 0, 0, 0, 0, submission.BuildResult.Exception.ToString(), null, null));
				}
				foreach (ILogger logger in loggers) {
					if (logger != null)
						logger.Shutdown();
				}
				if (callback != null)
					callback(submission);
			}
		}
		
		sealed class CentralLogger : INodeLogger, IEventRedirector
		{
			readonly ParallelMSBuildManager parentManager;
			
			public CentralLogger(ParallelMSBuildManager parentManager)
			{
				this.parentManager = parentManager;
			}
			
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
					lock (parentManager.submissionEventSourceMapping) {
						if (!parentManager.submissionEventSourceMapping.TryGetValue(e.BuildEventContext.SubmissionId, out redirector)) {
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
