// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.BuildWorker.Interprocess;

namespace ICSharpCode.SharpDevelop.BuildWorker
{
	/// <summary>
	/// Manages the list of running child worker processes.
	/// </summary>
	public static class WorkerManager
	{
		/// <summary>
		/// Delegate executed on the host when an exception occurs.
		/// </summary>
		public static Action<Exception> ShowError { get; set; }
		
		const int MaxWorkerProcessCount = 16;
		
		static readonly object lockObject = new object();
		static Queue<BuildRun> outstandingBuildRuns = new Queue<BuildRun>();
		static int workerProcessCount;
		static List<WorkerProcessHost> freeWorkerProcesses = new List<WorkerProcessHost>();
		
		static WorkerProcessHost DequeueFreeWorkerProcess()
		{
			WorkerProcessHost h = freeWorkerProcesses[freeWorkerProcesses.Count - 1];
			freeWorkerProcesses.RemoveAt(freeWorkerProcesses.Count - 1);
			return h;
		}
		
		public static void StartBuild(BuildJob job, BuildSettings settings)
		{
			if (job == null)
				throw new ArgumentNullException("job");
			if (settings == null)
				throw new ArgumentNullException("settings");
			
			BuildRun buildRun = new BuildRun(job, settings);
			lock (lockObject) {
				if (freeWorkerProcesses.Count > 0) {
					DequeueFreeWorkerProcess().StartBuild(buildRun);
				} else {
					outstandingBuildRuns.Enqueue(buildRun);
					if (workerProcessCount < MaxWorkerProcessCount) {
						workerProcessCount++;
						(new WorkerProcessHost()).Start();
					}
				}
			}
		}
		
		static Program inProcessBuildWorker;
		
		public static void RunBuildInProcess(BuildJob job, BuildSettings settings)
		{
			if (job == null)
				throw new ArgumentNullException("job");
			if (settings == null)
				throw new ArgumentNullException("settings");
			lock (lockObject) {
				if (inProcessBuildWorker == null)
					inProcessBuildWorker = new Program();
			}
			inProcessBuildWorker.BuildInProcess(settings, job);
		}
		
		static readonly object timerLock = new object();
		static Timer lastBuildDoneTimer;
		
		static void SetLastBuildDoneTimer()
		{
			lock (timerLock) {
				if (lastBuildDoneTimer != null) {
					lastBuildDoneTimer.Dispose();
					lastBuildDoneTimer = null;
				}
				lastBuildDoneTimer = new Timer(LastBuildDoneTimerCallback, null, 60000, 20000);
			}
		}
		
		static void ClearLastBuildDoneTimer()
		{
			lock (timerLock) {
				if (lastBuildDoneTimer != null) {
					lastBuildDoneTimer.Dispose();
					lastBuildDoneTimer = null;
				}
			}
		}
		
		static void LastBuildDoneTimerCallback(object state)
		{
			lock (lockObject) {
				if (freeWorkerProcesses.Count > 0) {
					Debug.WriteLine("WorkerManager: shutting down free worker");
					DequeueFreeWorkerProcess().Shutdown();
				} else {
					ClearLastBuildDoneTimer();
				}
			}
		}
		
		sealed class BuildRun
		{
			internal BuildJob job;
			internal BuildSettings settings;
			EventSource eventSource = new EventSource();
			
			public BuildRun(BuildJob job, BuildSettings settings)
			{
				this.job = job;
				this.settings = settings;
				foreach (ILogger logger in settings.Logger) {
					logger.Initialize(eventSource);
				}
			}
			
			public void RaiseError(string message)
			{
				Debug.WriteLine(message);
				RaiseEvent(new BuildErrorEventArgs(null, null, null, -1, -1, -1, -1, message, null, "SharpDevelopBuildWorkerManager"));
			}
			
			public void RaiseEvent(BuildEventArgs e)
			{
				eventSource.RaiseEvent(e);
			}
			
			public void Done(bool success)
			{
				SetLastBuildDoneTimer();
				try {
					foreach (ILogger logger in settings.Logger) {
						logger.Shutdown();
					}
				} finally {
					if (settings.BuildDoneCallback != null)
						settings.BuildDoneCallback(success);
				}
			}
		}
		
		sealed class WorkerProcessHost : IHostObject
		{
			WorkerProcess process;
			
			internal void Start()
			{
				process = new WorkerProcess(this);
				process.Ready += OnReady;
				process.WorkerLost += OnWorkerLost;
				
				process.Start(Program.CreateStartInfo());
			}
			
			BuildRun currentBuildRun;
			
			// runs in lock(lockObject)
			internal void StartBuild(BuildRun nextBuildRun)
			{
				Debug.Assert(currentBuildRun == null);
				currentBuildRun = nextBuildRun;
				process.CallMethodOnWorker("StartBuild", currentBuildRun.job);
			}
			
			void OnReady(object sender, EventArgs e)
			{
				BuildRun nextBuildRun = null;
				lock (lockObject) {
					if (outstandingBuildRuns.Count > 0)
						nextBuildRun = outstandingBuildRuns.Dequeue();
					else
						freeWorkerProcesses.Add(this);
				}
				if (nextBuildRun != null) {
					StartBuild(nextBuildRun);
				}
			}
			
			void OnWorkerLost(object sender, EventArgs e)
			{
				BuildRun buildRun;
				lock (lockObject) {
					workerProcessCount--;
					freeWorkerProcesses.Remove(this);
					if (workerProcessCount == 0 && currentBuildRun == null) {
						// error starting worker => we must
						// cancel all outstanding build runs to prevent them from waiting
						// for a worker becoming ready when all workers are dead
						while (workerProcessCount == 0 && outstandingBuildRuns.Count > 0) {
							BuildRun r = outstandingBuildRuns.Dequeue();
							Monitor.Exit(lockObject);
							r.RaiseError("Error starting worker process.");
							r.Done(false);
							Monitor.Enter(lockObject);
						}
					}
					buildRun = Interlocked.Exchange(ref currentBuildRun, null);
				}
				if (buildRun != null) {
					buildRun.RaiseError("Worker process lost during build");
					buildRun.Done(false);
				}
			}
			
			internal void Shutdown()
			{
				process.Shutdown();
			}
			
			// Called with CallMethodOnHost
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			public void ReportEvent(BuildEventArgs e)
			{
				BuildRun buildRun = currentBuildRun;
				if (buildRun != null) {
					buildRun.RaiseEvent(e);
				}
			}
			
			// Called with CallMethodOnHost
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			public void BuildDone(bool success)
			{
				BuildRun buildRun;
				lock (lockObject) {
					buildRun = Interlocked.Exchange(ref currentBuildRun, null);
				}
				if (buildRun != null) {
					// OnReady must be called before buildRun.Done - the callback
					// might trigger another build, and if this worker process
					// isn't marked as ready, a new process will be created even
					// though this one could do the work.
					OnReady(null, null);
					buildRun.Done(success);
				}
			}
			
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
			public void ReportException(string exceptionText)
			{
				// shutdown worker if it produced an exception
				try {
					process.Shutdown();
				} catch {}
				
				if (ShowError != null)
					ShowError(new Exception(exceptionText));
				else
					Program.ShowMessageBox(exceptionText);
			}
		}
	}
	
	public sealed class BuildSettings
	{
		List<ILogger> logger = new List<ILogger>();
		public Action<bool> BuildDoneCallback { get; set; }
		
		public ICollection<ILogger> Logger {
			get { return logger; }
		}
	}
}
