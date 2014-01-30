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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Parser
{
	/// <summary>
	/// The background task that initializes the projects in the solution.
	/// </summary>
	sealed class LoadSolutionProjects : ILoadSolutionProjectsThread
	{
		readonly JobQueue jobs;
		
		public LoadSolutionProjects()
		{
			jobs = new JobQueue(this);
		}
		
		/// <inheritdoc/>
		public bool IsRunning { get; private set; }
		
		/// <inheritdoc/>
		public event EventHandler Started = delegate {};
		
		/// <inheritdoc/>
		public event EventHandler Finished = delegate {};
		
		Stopwatch threadRunningTime;
		
		void RaiseThreadStarted()
		{
			threadRunningTime = Stopwatch.StartNew();
			SD.MainThread.InvokeAsyncAndForget(delegate {
				IsRunning = true;
				Started(this, EventArgs.Empty);
			});
		}
		
		void RaiseThreadEnded()
		{
			if (threadRunningTime != null)
				LoggingService.Debug("LoadSolutionProjectsThread finished after " + threadRunningTime.Elapsed);
			SD.MainThread.InvokeAsyncAndForget(delegate {
				IsRunning = false;
				Finished(this, EventArgs.Empty);
			});
		}
		
		static string GetLoadReferenceTaskTitle(string projectName)
		{
			return "Loading references for "  + projectName + "...";
		}
		
		static string GetParseTaskTitle(string projectName)
		{
			return StringParser.Parse("${res:ICSharpCode.SharpDevelop.Internal.ParserService.Parsing} ")  + projectName + "...";
		}
		
		/// <summary>
		/// Adds a new task to the job queue, and starts the LoadSolutionProjects thread (if its not already running).
		/// </summary>
		/// <param name="action">The action to run. Parameter: a nested progress monitor for the action.</param>
		/// <param name="name">Name of the action - shown in the status bar</param>
		/// <param name="cost">Cost of the action</param>
		public void AddJob(Action<IProgressMonitor> action, string name, double cost)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			if (!(cost > 0))
				cost = 1; // avoid 0-cost tasks (division by zero)
			
			jobs.AddJob(new JobTask(action, name, cost));
			// Start the thread with a bit delay so that the SD UI gets responsive first,
			// and so that the total cost is known for showing the progress bar.
			SD.MainThread.InvokeAsyncAndForget(jobs.StartRunningIfRequired, DispatcherPriority.Background);
		}
		
		public void CancelAllJobs()
		{
			jobs.Clear();
		}
		
		sealed class JobQueue
		{
			readonly LoadSolutionProjects loadSolutionProjects;
			readonly object lockObj = new object();
			readonly Queue<JobTask> actions = new Queue<JobTask>();
			CancellationTokenSource cancellationSource = new CancellationTokenSource();
			IProgressMonitor progressMonitor;
			bool threadIsRunning;
			double totalWork;
			double workDone;
			
			public JobQueue(LoadSolutionProjects loadSolutionProjects)
			{
				this.loadSolutionProjects = loadSolutionProjects;
			}
			
			public void AddJob(JobTask task)
			{
				if (task == null)
					throw new ArgumentNullException("task");
				lock (lockObj) {
					bool wasRunning = this.threadIsRunning || this.actions.Count > 0;
					this.totalWork += task.cost;
					this.actions.Enqueue(task);
					if (!wasRunning)
						loadSolutionProjects.RaiseThreadStarted();
				}
			}
			
			public bool IsThreadRunningOrWaitingToStart {
				get {
					lock (lockObj) {
						return this.threadIsRunning || this.actions.Count > 0;
					}
				}
			}
			
			public void StartRunningIfRequired()
			{
				lock (lockObj) {
					if (!this.threadIsRunning && this.actions.Count > 0) {
						this.threadIsRunning = true;
						
						progressMonitor = SD.StatusBar.CreateProgressMonitor(cancellationSource.Token);
						progressMonitor.TaskName = this.actions.Peek().name;
						
						Thread thread = new Thread(new ThreadStart(RunThread));
						thread.Name = "LoadSolutionProjects";
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
								if (actions.Count > 0)
									SD.MainThread.InvokeAsyncAndForget(StartRunningIfRequired);
								else
									loadSolutionProjects.RaiseThreadEnded();
							} else {
								loadSolutionProjects.RaiseThreadEnded();
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
							subTask.CancellationToken.ThrowIfCancellationRequested();
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
