// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// The background task that initializes the projects in the solution.
	/// </summary>
	static class LoadSolutionProjects
	{
		static JobQueue jobs;
		
		internal static void Initialize()
		{
			jobs = new JobQueue();
		}
		
		static volatile bool isThreadRunning;
		
		/// <summary>
		/// Gets whether the LoadSolutionProjects thread is currently running.
		/// </summary>
		public static bool IsThreadRunning {
			get {
				return isThreadRunning;
			}
		}
		
		/// <summary>
		/// Occurs when the 'load solution projects' thread has finished.
		/// This event is not raised when the 'load solution projects' is aborted because the solution was closed.
		/// This event is raised on the main thread.
		/// </summary>
		public static event EventHandler ThreadEnded = delegate {};
		
		static void RaiseThreadEnded(Solution solution)
		{
			Gui.WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					// only raise the event if the solution is still open
					if (solution == ProjectService.OpenSolution) {
						isThreadRunning = false;
						ThreadEnded(null, EventArgs.Empty);
					}
				});
		}
		
		#region Reparse projects
		// list of projects waiting to reparse references
		static List<ParseProjectContent> reParse1 = new List<ParseProjectContent>();
		
		// list of projects waiting to reparse code
		static List<ParseProjectContent> reParse2 = new List<ParseProjectContent>();
		
		public static void Reparse(IProject project, bool initReferences, bool parseCode)
		{
			if (jobs == null)
				return; // do nothing if service wasn't initialized (e.g. some unit tests)
			ParseProjectContent pc = ParserService.GetProjectContent(project) as ParseProjectContent;
			if (pc != null) {
				if (initReferences) {
					lock (reParse1) {
						if (!reParse1.Contains(pc)) {
							LoggingService.Debug("Enqueue for reinitializing references: " + project);
							reParse1.Add(pc);
							jobs.AddJob(new JobTask(pm => ReInitializeReferences(pc, pm),
							                        GetLoadReferenceTaskTitle(project.Name),
							                        10
							                       ));
						}
					}
				}
				if (parseCode) {
					lock (reParse2) {
						if (!reParse2.Contains(pc)) {
							LoggingService.Debug("Enqueue for reparsing code: " + project);
							reParse2.Add(pc);
							jobs.AddJob(new JobTask(pm => ReparseCode(pc, pm),
							                        GetParseTaskTitle(project.Name),
							                        pc.GetInitializationWorkAmount()
							                       ));
						}
					}
				}
				jobs.StartRunningIfRequired();
			}
		}
		
		static void ReInitializeReferences(ParseProjectContent pc, IProgressMonitor progressMonitor)
		{
			lock (reParse1) {
				reParse1.Remove(pc);
			}
			pc.ReInitialize1(progressMonitor);
		}
		
		static void ReparseCode(ParseProjectContent pc, IProgressMonitor progressMonitor)
		{
			lock (reParse2) {
				reParse2.Remove(pc);
			}
			pc.ReInitialize2(progressMonitor);
		}
		#endregion
		
		// do not use an event for this because a solution might be loaded before ParserService
		// is initialized
		internal static void OnSolutionLoaded(List<ParseProjectContent> createdContents)
		{
			WorkbenchSingleton.DebugAssertMainThread();
			Debug.Assert(jobs != null);
			
			Solution openedSolution = ProjectService.OpenSolution;
			isThreadRunning = true;
			
			WorkbenchSingleton.SafeThreadAsyncCall(ProjectService.ParserServiceCreatedProjectContents);
			
			for (int i = 0; i < createdContents.Count; i++) {
				ParseProjectContent pc = createdContents[i];
				jobs.AddJob(new JobTask(pc.Initialize1,
				                        GetLoadReferenceTaskTitle(pc.ProjectName),
				                        10));
			}
			for (int i = 0; i < createdContents.Count; i++) {
				ParseProjectContent pc = createdContents[i];
				jobs.AddJob(new JobTask(pc.Initialize2,
				                        GetParseTaskTitle(pc.ProjectName),
				                        pc.GetInitializationWorkAmount()));
			}
			jobs.AddJob(new JobTask(ct => RaiseThreadEnded(openedSolution), "", 0));
			jobs.StartRunningIfRequired();
		}
		
		static string GetLoadReferenceTaskTitle(string projectName)
		{
			return "Loading references for "  + projectName + "...";
		}
		
		static string GetParseTaskTitle(string projectName)
		{
			return StringParser.Parse("${res:ICSharpCode.SharpDevelop.Internal.ParserService.Parsing} ")  + projectName + "...";
		}
		
		internal static void InitNewProject(ParseProjectContent pc)
		{
			jobs.AddJob(new JobTask(pc.Initialize1,
			                        GetLoadReferenceTaskTitle(pc.ProjectName),
			                        10));
			jobs.AddJob(new JobTask(pc.Initialize2,
			                        GetParseTaskTitle(pc.ProjectName),
			                        pc.GetInitializationWorkAmount()));
			jobs.StartRunningIfRequired();
		}
		
		internal static void OnSolutionClosed()
		{
			jobs.Clear();
			lock (reParse1) {
				reParse1.Clear();
			}
			lock (reParse2) {
				reParse2.Clear();
			}
		}
		
		sealed class JobQueue
		{
			readonly object lockObj = new object();
			readonly Queue<JobTask> actions = new Queue<JobTask>();
			CancellationTokenSource cancellationSource = new CancellationTokenSource();
			IProgressMonitor progressMonitor;
			bool threadIsRunning;
			double totalWork;
			double workDone;
			
			public void AddJob(JobTask task)
			{
				if (task == null)
					throw new ArgumentNullException("task");
				lock (lockObj) {
					this.totalWork += task.cost;
					this.actions.Enqueue(task);
				}
			}
			
			public void StartRunningIfRequired()
			{
				lock (lockObj) {
					if (!this.threadIsRunning && this.actions.Count > 0) {
						this.threadIsRunning = true;
						
						progressMonitor = WorkbenchSingleton.StatusBar.CreateProgressMonitor(cancellationSource.Token);
						progressMonitor.TaskName = this.actions.Peek().name;
						
						Thread thread = new Thread(new ThreadStart(RunThread));
						thread.Name = "LoadSolutionProjects";
						thread.Priority = ThreadPriority.BelowNormal;
						thread.IsBackground = true;
						thread.Start();
					}
				}
			}
			
			void RunThread()
			{
				while (true) {
					JobTask task;
					// copy fields from this class into local variables to ensure thread-safety
					// with concurrent Clear() calls
					double totalWork, workDone;
					IProgressMonitor progressMonitor;
					lock (lockObj) {
						// enqueued null: quit thread and restart (used for cancellation)
						if (actions.Count == 0 || this.actions.Peek() == null) {
							this.threadIsRunning = false;
							this.progressMonitor.Dispose();
							this.progressMonitor = null;
							// restart if necessary:
							if (actions.Count > 0) {
								actions.Dequeue(); // dequeue the null
								WorkbenchSingleton.SafeThreadAsyncCall(StartRunningIfRequired);
							}
							return;
						}
						task = this.actions.Dequeue();
						totalWork = this.totalWork;
						workDone = this.workDone;
						progressMonitor = this.progressMonitor;
					}
					progressMonitor.Progress = workDone / totalWork;
					progressMonitor.TaskName = task.name;
					try {
						using (IProgressMonitor subTask = progressMonitor.CreateSubTask(task.cost / totalWork)) {
							task.Run(subTask);
						}
						lock (lockObj) {
							this.workDone += task.cost;
						}
					} catch (OperationCanceledException) {
						// ignore cancellation
					} catch (Exception ex) {
						MessageService.ShowException(ex, "Error on LoadSolutionProjects thread");
					}
				}
			}
			
			public void Clear()
			{
				lock (lockObj) {
					cancellationSource.Cancel();
					actions.Clear();
					cancellationSource = new CancellationTokenSource();
					this.totalWork = 0;
					this.workDone = 0;
					// progress monitor gets disposed when the worker thread exits
					if (threadIsRunning) {
						actions.Enqueue(null); // force worker thread to restart using a new progress monitor
						// This is necessary so that actions enqueued after the Clear() call get executed using
						// the fresh CancellationToken.
					}
				}
			}
		}
		
		sealed class JobTask
		{
			readonly Action<IProgressMonitor> action;
			internal readonly string name;
			internal readonly double cost;
			
			public JobTask(Action<IProgressMonitor> action, string name, double cost)
			{
				if (action == null)
					throw new ArgumentNullException("action");
				this.action = action;
				this.name = name;
				this.cost = cost;
			}
			
			public void Run(IProgressMonitor progressMonitor)
			{
				action(progressMonitor);
			}
		}
	}
}
