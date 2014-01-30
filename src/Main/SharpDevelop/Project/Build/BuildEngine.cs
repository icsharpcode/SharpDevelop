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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Supports building a project with dependencies or the whole solution.
	/// Creates a build graph and then builds it using topological sort.
	/// Supports building multiple projects in parallel.
	/// 
	/// This class is not related to MSBuild: it simply "builds" a project by calling IBuildable.StartBuild.
	/// </summary>
	sealed class BuildEngine
	{
		#region StartBuild
		/// <summary>
		/// Starts to run a build.
		/// </summary>
		/// <param name="project">The project/solution to build</param>
		/// <param name="options">The build options that should be used</param>
		/// <param name="buildFeedbackSink">The build feedback sink that receives the build output.
		/// The output is nearly sent "as it comes in": sometimes output must wait because the BuildEngine
		/// will ensure that output from two projects building in parallel isn't interleaved.</param>
		/// <param name="progressMonitor">The progress monitor that receives build progress. The monitor will be disposed
		/// when the build completes.</param>
		public static Task<BuildResults> BuildAsync(IBuildable project, BuildOptions options, IBuildFeedbackSink buildFeedbackSink, IProgressMonitor progressMonitor)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (options == null)
				throw new ArgumentNullException("options");
			
			BuildEngine engine = new BuildEngine(options, project);
			engine.buildStart = DateTime.Now;
			engine.combinedBuildFeedbackSink = buildFeedbackSink;
			engine.progressMonitor = progressMonitor;
			try {
				engine.rootNode = engine.CreateBuildGraph(project);
			} catch (CyclicDependencyException ex) {
				BuildError error;
				if (ex.Project1 != null && ex.Project2 != null)
					error = new BuildError(null, "Cyclic dependency between " + ex.Project1.Name + " and " + ex.Project2.Name);
				else
					error = new BuildError(null, "Cyclic dependency");
				engine.results.Add(error);
				if (engine.combinedBuildFeedbackSink != null) {
					engine.combinedBuildFeedbackSink.ReportError(error);
					engine.combinedBuildFeedbackSink.ReportMessage(error.ToRichText());
				}
				
				engine.results.Result = BuildResultCode.BuildFileError;
				engine.ReportDone();
				return engine.tcs.Task;
			}
			
			engine.workersToStart = options.ParallelProjectCount;
			if (engine.workersToStart < 1)
				engine.workersToStart = 1;
			
			engine.cancellationRegistration = engine.progressMonitor.CancellationToken.Register(engine.BuildCancelled);
			
			engine.ReportMessageLine("${res:MainWindow.CompilerMessages.BuildStarted}");
			engine.StartBuildProjects();
			engine.UpdateProgressTaskName();
			return engine.tcs.Task;
		}
		#endregion
		
		#region inner class BuildNode
		sealed class BuildNode : IBuildFeedbackSink
		{
			readonly BuildEngine engine;
			internal readonly IBuildable project;
			/// <summary>The build options used for this node. Might be null.</summary>
			internal ProjectBuildOptions options;
			internal BuildNode[] dependencies;
			/// <summary>specifies whether the node has been constructed completely (all dependencies initialized)</summary>
			internal bool nodeComplete;
			/// <summary>specifies whether this node has started building</summary>
			internal bool buildStarted;
			/// <summary>specifies whether this node has finished building</summary>
			internal bool buildFinished;
			
			/// <summary>specifies whether the node produces build errors</summary>
			internal bool hasErrors;
			
			/// <summary>The number of dependencies missing until this node can be built</summary>
			internal int outstandingDependencies;
			
			/// <summary>The list of nodes that directly depend on this node</summary>
			internal List<BuildNode> dependentOnThis = new List<BuildNode>();
			
			/// <summary>Progress monitor just for this node, used only while the node is being built</summary>
			internal IProgressMonitor perNodeProgressMonitor;
			
			int totalDependentOnThisCount = -1;
			
			/// <summary>Gets the number of nodes that depend on this node
			/// directly or indirectly</summary>
			internal int TotalDependentOnThisCount {
				get {
					if (totalDependentOnThisCount >= 0)
						return totalDependentOnThisCount;
					
					// make a depth-first search to determine the size of the tree of
					// projects that depends on this project:
					HashSet<BuildNode> visitedNodes = new HashSet<BuildNode>();
					VisitDependentOnThis(visitedNodes, this);
					totalDependentOnThisCount = visitedNodes.Count;
					return totalDependentOnThisCount;
				}
			}
			
			static void VisitDependentOnThis(HashSet<BuildNode> visitedNodes, BuildNode node)
			{
				if (visitedNodes.Add(node)) {
					foreach (BuildNode n in node.dependentOnThis) {
						VisitDependentOnThis(visitedNodes, n);
					}
				}
			}
			
			/// <summary>The list of messages that were not reported because another node held the
			/// output lock</summary>
			internal List<RichText> unreportedMessageList;
			
			public BuildNode(BuildEngine engine, IBuildable project)
			{
				this.engine = engine;
				this.project = project;
			}
			
			public async void DoStartBuild(object state)
			{
				string name = string.Empty;
				try {
					name = project.Name;
					bool success = await project.BuildAsync(options, this, perNodeProgressMonitor).ConfigureAwait(false);
					Done(success);
				} catch (ObjectDisposedException) {
					// Handle ObjectDisposedException that occurs when trying to build a project that was unloaded.
					ReportError(new BuildError(null, "The project '" + name + "' was unloaded."));
					Done(false);
				}
			}
			
			public void ReportError(BuildError error)
			{
				TransformBuildError(error);
				if (error.IsWarning) {
					if (perNodeProgressMonitor.Status != OperationStatus.Error)
						perNodeProgressMonitor.Status = OperationStatus.Warning;
				} else {
					perNodeProgressMonitor.Status = OperationStatus.Error;
				}
				engine.ReportError(this, error);
			}

			void TransformBuildError(BuildError error)
			{
				if (error.IsWarning) {
					// treat "MSB3274: The primary reference "{0}" could not be resolved because it was 
					// built against the "{1}" framework. This is a higher version than the currently 
					// targeted framework "{2}"." as error.
					if ("MSB3274".Equals(error.ErrorCode, StringComparison.OrdinalIgnoreCase)) {
						error.IsWarning = false;
						return;
					}
					// treat "MSB3275: The primary reference "{0}" could not be resolved because it has
					// an indirect dependency on the assembly "{1}" which was built against the "{2}"
					// framework. This is a higher version than the currently targeted framework "{3}"."
					// as error.
					if ("MSB3275".Equals(error.ErrorCode, StringComparison.OrdinalIgnoreCase)) {
						error.IsWarning = false;
						return;
					}
				}
			}
			
			public void ReportMessage(RichText message)
			{
				engine.ReportMessage(this, message);
			}
			
			public void Done(bool success)
			{
				engine.OnBuildFinished(this, success);
			}
		}
		#endregion
		
		#region BuildEngine fields and constructor
		readonly Dictionary<IBuildable, BuildNode> nodeDict = new Dictionary<IBuildable, BuildNode>();
		readonly BuildOptions options;
		IProgressMonitor progressMonitor;
		CancellationTokenRegistration cancellationRegistration;
		readonly TaskCompletionSource<BuildResults> tcs = new TaskCompletionSource<BuildResults>();
		BuildNode rootNode;
		readonly IBuildable rootProject;
		readonly BuildResults results = new BuildResults();
		DateTime buildStart;
		
		readonly List<BuildNode> projectsCurrentlyBuilding = new List<BuildNode>();
		readonly List<BuildNode> projectsReadyForBuildStart = new List<BuildNode>();
		int workersToStart, runningWorkers;
		
		private BuildEngine(BuildOptions options, IBuildable rootProject)
		{
			this.options = options;
			this.rootProject = rootProject;
		}
		#endregion
		
		#region CreateBuildGraph
		BuildNode CreateBuildGraph(IBuildable project)
		{
			BuildNode node;
			if (nodeDict.TryGetValue(project, out node)) {
				return node;
			}
			node = new BuildNode(this, project);
			nodeDict[project] = node;
			InitializeProjectOptions(node);
			InitializeDependencies(node);
			
			node.nodeComplete = true;
			return node;
		}
		
		void InitializeProjectOptions(BuildNode node)
		{
			IBuildable project = node.project;
			// Create options for building the project
			node.options = project.CreateProjectBuildOptions(options, project == rootProject);
		}
		
		void InitializeDependencies(BuildNode node)
		{
			// Initialize dependencies
			if (options.BuildDependentProjects) {
				var dependencies = node.project.GetBuildDependencies(node.options);
				if (dependencies != null) {
					node.dependencies = dependencies
						.Select<IBuildable, BuildNode>(CreateBuildGraph)
						.Distinct().ToArray();
				}
			}
			if (node.dependencies == null) {
				node.dependencies = new BuildNode[0];
			}
			node.outstandingDependencies = node.dependencies.Length;
			foreach (BuildNode d in node.dependencies) {
				if (!d.nodeComplete)
					throw new CyclicDependencyException(node.project, d.project);
				d.dependentOnThis.Add(node);
			}
			if (node.outstandingDependencies == 0) {
				projectsReadyForBuildStart.Add(node);
			}
		}
		
		[Serializable]
		sealed class CyclicDependencyException : Exception
		{
			public IBuildable Project1, Project2;
			
			public CyclicDependencyException()
			{
			}
			
			public CyclicDependencyException(IBuildable project1, IBuildable project2)
			{
				this.Project1 = project1;
				this.Project2 = project2;
			}
		}
		#endregion
		
		#region Building
		void StartBuildProjects()
		{
			lock (this) {
				while (workersToStart > 0) {
					if (buildIsCancelled || projectsReadyForBuildStart.Count == 0) {
						if (runningWorkers == 0) {
							BuildDone();
						}
						return;
					}
					
					workersToStart--;
					
					int projectToStartIndex = 0;
					for (int i = 1; i < projectsReadyForBuildStart.Count; i++) {
						if (CompareBuildOrder(projectsReadyForBuildStart[i],
						                      projectsReadyForBuildStart[projectToStartIndex]) < 0)
						{
							projectToStartIndex = i;
						}
					}
					BuildNode node = projectsReadyForBuildStart[projectToStartIndex];
					projectsReadyForBuildStart.RemoveAt(projectToStartIndex);
					node.perNodeProgressMonitor = progressMonitor.CreateSubTask(1.0 / nodeDict.Count);
					node.buildStarted = true;
					
					bool hasDependencyErrors = false;
					foreach (BuildNode n in node.dependencies) {
						if (!n.buildFinished)
							throw new Exception("Trying to build project with unfinished dependencies");
						hasDependencyErrors |= n.hasErrors;
					}
					
					ICSharpCode.Core.LoggingService.Info("Start building " + node.project.Name);
					runningWorkers++;
					projectsCurrentlyBuilding.Add(node);
					if (hasDependencyErrors) {
						ICSharpCode.Core.LoggingService.Debug("Skipped building " + node.project.Name + " (errors in dependencies)");
						node.hasErrors = true;
						node.Done(false);
					} else {
						// do not run "DoStartBuild" inside lock - run it async on the thread pool
						System.Threading.ThreadPool.QueueUserWorkItem(node.DoStartBuild);
					}
				}
			}
		}
		
		static int CompareBuildOrder(BuildNode a, BuildNode b)
		{
			// When the build graph looks like this:
			//    1   2
			//     \ / \
			//      3   4
			//     / \
			//    5   6
			// And we have 2 build workers, determining the build order alphabetically only would result
			// in this order:
			//  4, 5
			//  6
			//  3
			//  1, 2
			// However, we can build faster if we build 5+6 first:
			//  5, 6
			//  3, 4
			//  1, 2
			//
			// As heuristic, we first build the projects that have the most projects dependent on it.
			int r = -(a.TotalDependentOnThisCount.CompareTo(b.TotalDependentOnThisCount));
			if (r == 0)
				r = string.Compare(a.project.Name, b.project.Name, StringComparison.CurrentCultureIgnoreCase);
			return r;
		}
		
		void OnBuildFinished(BuildNode node, bool success)
		{
			ICSharpCode.Core.LoggingService.Info("Finished building " + node.project.Name + ", success=" + success);
			node.perNodeProgressMonitor.Progress = 1;
			node.perNodeProgressMonitor.Dispose();
			lock (this) {
				if (node.buildFinished) {
					throw new InvalidOperationException("This node already finished building, do not call IBuildFeedbackSink.Done() multiple times!");
				}
				runningWorkers--;
				node.buildFinished = true;
				node.hasErrors = !success;
				
				projectsCurrentlyBuilding.Remove(node);
				
				foreach (BuildNode n in node.dependentOnThis) {
					n.outstandingDependencies--;
					if (n.outstandingDependencies == 0)
						projectsReadyForBuildStart.Add(n);
				}
				workersToStart++;
			}
			LogBuildFinished(node);
			StartBuildProjects();
			UpdateProgressTaskName();
		}
		
		bool buildIsDone;
		
		void BuildDone()
		{
			lock (this) {
				if (buildIsDone)
					return;
				buildIsDone = true;
			}
			if (!buildIsCancelled) {
				foreach (BuildNode n in nodeDict.Values) {
					if (!n.buildFinished) {
						throw new Exception("All workers done, but a project did not finish building");
					}
				}
			}
			string buildTime = " (" + (DateTime.Now - buildStart).ToString() + ")";
			
			if (buildIsCancelled) {
				results.Result = BuildResultCode.Cancelled;
				
				ReportMessageLine("${res:MainWindow.CompilerMessages.BuildCancelled}");
			} else if (rootNode.hasErrors) {
				results.Result = BuildResultCode.Error;
				
				ReportMessageLine("${res:MainWindow.CompilerMessages.BuildFailed}" + buildTime);
			} else {
				results.Result = BuildResultCode.Success;
				
				ReportMessageLine("${res:MainWindow.CompilerMessages.BuildFinished}" + buildTime);
			}
			cancellationRegistration.Dispose();
			ReportDone();
		}
		
		/// <summary>
		/// Notifies the callback hooks that the build is done.
		/// </summary>
		void ReportDone()
		{
			tcs.SetResult(results);
		}
		#endregion
		
		#region Cancel build
		bool buildIsCancelled;
		
		void BuildCancelled()
		{
			lock (this) {
				if (buildIsDone)
					return;
				buildIsCancelled = true;
			}
			results.Add(new BuildError(null, StringParser.Parse("${res:MainWindow.CompilerMessages.BuildCancelled}")));
		}
		#endregion
		
		#region Logging
		IBuildFeedbackSink combinedBuildFeedbackSink;
		/// <summary>
		/// The node that currently may output messages.
		/// To avoid confusing the user in parallel builds, only one project may output messages;
		/// the messages from other projects will be enqueued and written to the output as soon
		/// as the project holding the lock finishes building.
		/// </summary>
		BuildNode nodeWithOutputLock;
		Queue<BuildNode> nodesWaitingForOutputLock = new Queue<BuildNode>();
		
		void ReportError(BuildNode source, BuildError error)
		{
			if (!error.IsWarning)
				source.hasErrors = true;
			results.Add(error);
			ReportMessage(source, error.ToRichText());
			if (combinedBuildFeedbackSink != null) {
				combinedBuildFeedbackSink.ReportError(error);
			}
		}
		
		void ReportMessage(BuildNode source, RichText message)
		{
			bool hasOutputLock;
			lock (this) {
				if (nodeWithOutputLock == null) {
					nodeWithOutputLock = source;
				}
				hasOutputLock = nodeWithOutputLock == source;
				if (!hasOutputLock) {
					if (source.unreportedMessageList == null) {
						nodesWaitingForOutputLock.Enqueue(source);
						source.unreportedMessageList = new List<RichText>();
					}
					source.unreportedMessageList.Add(message);
				}
			}
			if (hasOutputLock) {
				ReportMessageInternal(message);
			}
		}
		
		void LogBuildFinished(BuildNode node)
		{
			List<RichText> messagesToReport = null;
			bool newNodeWithOutputLockAlreadyFinishedBuilding = false;
			lock (this) {
				if (node == nodeWithOutputLock) {
					if (nodesWaitingForOutputLock.Count > 0) {
						nodeWithOutputLock = nodesWaitingForOutputLock.Dequeue();
						messagesToReport = nodeWithOutputLock.unreportedMessageList;
						nodeWithOutputLock.unreportedMessageList = null;
						newNodeWithOutputLockAlreadyFinishedBuilding = nodeWithOutputLock.buildFinished;
					} else {
						nodeWithOutputLock = null;
					}
				}
			}
			if (messagesToReport != null) {
				// we can report these messages outside the lock:
				// while they swap order with messages currently coming in (ReportMessage),
				// this shouldn't be a problem as nodes should not report messages after they finish building.
				messagesToReport.ForEach(ReportMessageInternal);
			}
			if (newNodeWithOutputLockAlreadyFinishedBuilding) {
				// if the node already finished building before it got the output lock, we need
				// to release the output lock again here
				LogBuildFinished(nodeWithOutputLock);
			}
		}
		
		void ReportMessageLine(string message)
		{
			ReportMessageInternal(new RichText(StringParser.Parse(message)));
		}
		
		void ReportMessageInternal(RichText message)
		{
			if (combinedBuildFeedbackSink != null)
				combinedBuildFeedbackSink.ReportMessage(message);
		}
		
		void UpdateProgressTaskName()
		{
			lock (this) {
				progressMonitor.TaskName = StringParser.Parse("${res:MainWindow.CompilerMessages.BuildVerb} ")
					+ string.Join(", ", projectsCurrentlyBuilding.Select(n => n.project.Name))
					+ "...";
			}
		}
		#endregion
	}
}
