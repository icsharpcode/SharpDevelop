// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop.Gui;
using System.Text;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Supports building a project with dependencies or the whole solution.
	/// Creates a build graph and then builds it using topological sort.
	/// Supports building multiple projects in parallel.
	/// 
	/// This class is not related to MSBuild: it simply "builds" a project by calling IBuildable.StartBuild.
	/// </summary>
	public sealed class BuildEngine
	{
		#region Building in the SharpDevelop GUI
		static CancellableProgressMonitor guiBuildProgressMonitor;
		
		/// <summary>
		/// Starts to run a build inside the SharpDevelop GUI.
		/// Only one build can run inside the GUI at one time.
		/// </summary>
		/// <param name="project">The project/solution to build.</param>
		/// <param name="options">The build options.</param>
		public static void BuildInGui(IBuildable project, BuildOptions options)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (options == null)
				throw new ArgumentNullException("options");
			WorkbenchSingleton.AssertMainThread();
			if (guiBuildProgressMonitor != null) {
				BuildResults results = new BuildResults();
				results.Add(new BuildError(null, Core.ResourceService.GetString("MainWindow.CompilerMessages.MSBuildAlreadyRunning")));
				results.Result = BuildResultCode.MSBuildAlreadyRunning;
				if (options.Callback != null) {
					options.Callback(results);
				}
			} else {
				guiBuildProgressMonitor = new CancellableProgressMonitor(StatusBarService.CreateProgressMonitor());
				Gui.WorkbenchSingleton.Workbench.GetPad(typeof(Gui.CompilerMessageView)).BringPadToFront();
				GuiBuildStarted.RaiseEvent(null, new BuildEventArgs(project, options));
				StartBuild(project, options,
				           new MessageViewSink(TaskService.BuildMessageViewCategory),
				           guiBuildProgressMonitor);
			}
		}
		
		// TODO: in 4.0, replace ProjectService.BuildStarted/BuildFinished with these events
		public static event EventHandler<BuildEventArgs> GuiBuildStarted;
		public static event EventHandler<BuildEventArgs> GuiBuildFinished;
		
		/// <summary>
		/// Gets whether there is a build currently running inside the SharpDevelop GUI.
		/// </summary>
		public static bool IsGuiBuildRunning {
			get {
				WorkbenchSingleton.AssertMainThread();
				return guiBuildProgressMonitor != null;
			}
		}
		
		/// <summary>
		/// Cancels the build currently running inside the SharpDevelop GUI.
		/// This method does nothing if no build is running.
		/// </summary>
		public static void CancelGuiBuild()
		{
			WorkbenchSingleton.AssertMainThread();
			if (guiBuildProgressMonitor != null) {
				guiBuildProgressMonitor.Cancel();
			}
		}
		
		sealed class MessageViewSink : IBuildFeedbackSink
		{
			Gui.MessageViewCategory messageView;
			
			public MessageViewSink(ICSharpCode.SharpDevelop.Gui.MessageViewCategory messageView)
			{
				this.messageView = messageView;
			}
			
			public void ReportError(BuildError error)
			{
			}
			
			public void ReportMessage(string message)
			{
				messageView.AppendLine(ICSharpCode.Core.StringParser.Parse(message));
			}
			
			public void Done(bool success)
			{
				throw new InvalidOperationException("The Done(IBuildable,BuildOptions,BuildResults) method should be called instead.");
			}
			
			public void Done(IBuildable buildable, BuildOptions options, BuildResults results)
			{
				WorkbenchSingleton.SafeThreadAsyncCall(
					delegate {
						guiBuildProgressMonitor = null;
						GuiBuildFinished.RaiseEvent(null, new BuildEventArgs(buildable, options, results));
					});
			}
		}
		
		sealed class CancellableProgressMonitor : IProgressMonitor
		{
			IProgressMonitor baseProgressMonitor;
			
			public CancellableProgressMonitor(IProgressMonitor baseProgressMonitor)
			{
				this.baseProgressMonitor = baseProgressMonitor;
			}
			
			readonly object lockObject = new object();
			bool isCancelAllowed;
			bool isCancelled;
			
			public bool IsCancelAllowed {
				get { return isCancelAllowed; }
			}
			
			public bool IsCancelled {
				get { return isCancelled; }
			}
			
			public void Cancel()
			{
				EventHandler eh = null;
				lock (lockObject) {
					if (isCancelAllowed && !isCancelled) {
						ICSharpCode.Core.LoggingService.Info("Cancel build");
						isCancelled = true;
						eh = Cancelled;
					}
				}
				if (eh != null)
					eh(this, EventArgs.Empty);
			}
			
			public event EventHandler Cancelled;
			
			public void BeginTask(string name, int totalWork, bool allowCancel)
			{
				baseProgressMonitor.BeginTask(name, totalWork, allowCancel);
				lock (lockObject) {
					isCancelAllowed = allowCancel;
				}
			}
			
			public void Done()
			{
				lock (lockObject) {
					isCancelAllowed = false;
				}
				baseProgressMonitor.Done();
			}
			
			public int WorkDone {
				get { return baseProgressMonitor.WorkDone; }
				set { baseProgressMonitor.WorkDone = value; }
			}
			
			public string TaskName {
				get { return baseProgressMonitor.TaskName; }
				set { baseProgressMonitor.TaskName = value; }
			}
			
			public bool ShowingDialog {
				get { return baseProgressMonitor.ShowingDialog; }
				set { baseProgressMonitor.ShowingDialog = value; }
			}
		}
		#endregion
		
		#region StartBuild
		/// <summary>
		/// Starts to run a build.
		/// </summary>
		/// <param name="project">The project/solution to build</param>
		/// <param name="options">The build options that should be used</param>
		/// <param name="realtimeBuildFeedbackSink">The build feedback sink that receives the build output.
		/// The output is nearly sent "as it comes in": sometimes output must wait because the BuildEngine
		/// will ensure that output from two projects building in parallel isn't interleaved.</param>
		/// <param name="progressMonitor">The progress monitor that receives build progress.</param>
		public static void StartBuild(IBuildable project, BuildOptions options, IBuildFeedbackSink realtimeBuildFeedbackSink, IProgressMonitor progressMonitor)
		{
			if (project == null)
				throw new ArgumentNullException("solution");
			if (options == null)
				throw new ArgumentNullException("options");
			
			Solution solution = project.ParentSolution;
			if (solution == null)
				throw new ArgumentException("project.ParentSolution must not be null", "project");
			
			if (string.IsNullOrEmpty(options.SolutionConfiguration))
				options.SolutionConfiguration = solution.Preferences.ActiveConfiguration;
			if (string.IsNullOrEmpty(options.SolutionPlatform))
				options.SolutionPlatform = solution.Preferences.ActivePlatform;
			
			BuildEngine engine = new BuildEngine(options, project);
			engine.buildStart = DateTime.Now;
			engine.combinedBuildFeedbackSink = realtimeBuildFeedbackSink;
			engine.progressMonitor = progressMonitor;
			try {
				engine.rootNode = engine.CreateBuildGraph(project);
			} catch (CyclicDependencyException ex) {
				if (ex.Project1 != null && ex.Project2 != null)
					engine.results.Add(new BuildError(null, "Cyclic dependency between " + ex.Project1.Name + " and " + ex.Project2.Name));
				else
					engine.results.Add(new BuildError(null, "Cyclic dependency"));
				engine.results.Result = BuildResultCode.BuildFileError;
				engine.ReportDone();
				return;
			}
			
			engine.workersToStart = options.ParallelProjectCount;
			if (engine.workersToStart < 1)
				engine.workersToStart = 1;
			
			if (progressMonitor != null) {
				progressMonitor.Cancelled += engine.BuildCancelled;
				progressMonitor.BeginTask("", engine.nodeDict.Count, true);
			}
			
			engine.ReportMessageInternal("${res:MainWindow.CompilerMessages.BuildStarted}");
			engine.StartBuildProjects();
			engine.UpdateProgressTaskName();
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
			internal List<string> unreportedMessageList;
			
			public BuildNode(BuildEngine engine, IBuildable project)
			{
				this.engine = engine;
				this.project = project;
			}
			
			public void DoStartBuild(object state)
			{
				project.StartBuild(options, this);
			}
			
			public void ReportError(BuildError error)
			{
				engine.ReportError(this, error);
			}
			
			public void ReportMessage(string message)
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
			ICSharpCode.Core.LoggingService.Info("Finished building " + node.project.Name);
			lock (this) {
				if (node.buildFinished) {
					throw new InvalidOperationException("This node already finished building, do not call IBuildFeedbackSink.Done() multiple times!");
				}
				runningWorkers--;
				node.buildFinished = true;
				node.hasErrors = !success;
				
				projectsCurrentlyBuilding.Remove(node);
				results.AddBuiltProject(node.project);
				if (progressMonitor != null) {
					progressMonitor.WorkDone += 1;
				}
				
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
				
				ReportMessageInternal("${res:MainWindow.CompilerMessages.BuildCancelled}");
			} else if (rootNode.hasErrors) {
				results.Result = BuildResultCode.Error;
				
				ReportMessageInternal("${res:MainWindow.CompilerMessages.BuildFailed}" + buildTime);
			} else {
				results.Result = BuildResultCode.Success;
				
				ReportMessageInternal("${res:MainWindow.CompilerMessages.BuildFinished}" + buildTime);
			}
			if (progressMonitor != null) {
				progressMonitor.Done();
			}
			ReportDone();
		}
		
		/// <summary>
		/// Notifies the callback hooks that the build is done.
		/// </summary>
		void ReportDone()
		{
			if (combinedBuildFeedbackSink != null) {
				if (combinedBuildFeedbackSink is MessageViewSink) {
					// Special case GUI-builds so that they have more information available:
					// (non-GUI builds can get the same information from the options.Callback,
					// but the GUI cannot use the callback because the build options are set by
					// the code triggering the build)
					((MessageViewSink)combinedBuildFeedbackSink).Done(rootProject, options, results);
				} else {
					combinedBuildFeedbackSink.Done(results.Result == BuildResultCode.Success);
				}
			}
			if (options.Callback != null) {
				Gui.WorkbenchSingleton.SafeThreadAsyncCall(delegate { options.Callback(results); });
			}
		}
		#endregion
		
		#region Cancel build
		bool buildIsCancelled;
		
		void BuildCancelled(object sender, EventArgs e)
		{
			lock (this) {
				if (buildIsDone)
					return;
				buildIsCancelled = true;
			}
			results.Add(new BuildError(null, "Build was cancelled."));
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
			ReportMessage(source, error.ToString());
			if (combinedBuildFeedbackSink != null) {
				combinedBuildFeedbackSink.ReportError(error);
			}
		}
		
		void ReportMessage(BuildNode source, string message)
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
						source.unreportedMessageList = new List<string>();
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
			List<string> messagesToReport = null;
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
				messagesToReport.ForEach(ReportMessageInternal);
			}
			if (newNodeWithOutputLockAlreadyFinishedBuilding) {
				// if the node already finished building before it got the output lock, we need
				// to release the output lock again here
				LogBuildFinished(nodeWithOutputLock);
			}
		}
		
		void ReportMessageInternal(string message)
		{
			if (combinedBuildFeedbackSink != null)
				combinedBuildFeedbackSink.ReportMessage(message);
		}
		
		void UpdateProgressTaskName()
		{
			lock (this) {
				if (progressMonitor != null) {
					progressMonitor.TaskName = "${res:MainWindow.CompilerMessages.BuildVerb} "
						+ projectsCurrentlyBuilding.Select(n => n.project.Name).Join(", ")
						+ "...";
				}
			}
		}
		#endregion
	}
}
