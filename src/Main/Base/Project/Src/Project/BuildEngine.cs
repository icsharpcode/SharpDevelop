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
	/// 
	/// Supports building multiple projects in parallel.
	/// </summary>
	public sealed class BuildEngine
	{
		#region Building in the SharpDevelop GUI
		static CancellableProgressMonitor guiBuildProgressMonitor;
		
		public static void BuildInGui(IBuildable project, BuildOptions options)
		{
			WorkbenchSingleton.DebugAssertMainThread();
			if (guiBuildProgressMonitor != null) {
				BuildResults results = new BuildResults();
				results.Add(new BuildError(null, Core.ResourceService.GetString("MainWindow.CompilerMessages.MSBuildAlreadyRunning")));
				results.Result = BuildResultCode.MSBuildAlreadyRunning;
				options.Callback(results);
			} else {
				guiBuildProgressMonitor = new CancellableProgressMonitor(StatusBarService.CreateProgressMonitor());
				Gui.WorkbenchSingleton.Workbench.GetPad(typeof(Gui.CompilerMessageView)).BringPadToFront();
				StartBuild(project, options,
				           new MessageViewSink(TaskService.BuildMessageViewCategory),
				           guiBuildProgressMonitor);
			}
		}
		
		public static bool IsGuiBuildRunning {
			get {
				WorkbenchSingleton.DebugAssertMainThread();
				return guiBuildProgressMonitor != null;
			}
		}
		
		public static void CancelGuiBuild()
		{
			WorkbenchSingleton.DebugAssertMainThread();
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
				messageView.AppendLine(message);
			}
			
			public void Done(bool success)
			{
				WorkbenchSingleton.SafeThreadAsyncCall(delegate { guiBuildProgressMonitor = null; });
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
						ICSharpCode.Core.LoggingService.Debug("Cancel build");
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
			engine.configMatchings = solution.GetActiveConfigurationsAndPlatformsForProjects(options.SolutionConfiguration, options.SolutionPlatform);
			try {
				engine.rootNode = engine.CreateBuildGraph(project);
			} catch (CyclicDependencyException ex) {
				if (ex.Project1 != null && ex.Project2 != null)
					engine.results.Add(new BuildError(null, "Cyclic dependency between " + ex.Project1.Name + " and " + ex.Project2.Name));
				else
					engine.results.Add(new BuildError(null, "Cyclic dependency"));
				engine.results.Result = BuildResultCode.BuildFileError;
				realtimeBuildFeedbackSink.Done(false);
				options.Callback(engine.results);
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
		List<Solution.ProjectConfigurationPlatformMatching> configMatchings;
		BuildNode rootNode;
		IBuildable rootProject;
		BuildResults results = new BuildResults();
		DateTime buildStart;
		
		List<BuildNode> projectsCurrentlyBuilding = new List<BuildNode>();
		List<BuildNode> projectsReadyForBuildStart = new List<BuildNode>();
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
			node.options = new ProjectBuildOptions(project == rootProject ? options.ProjectTarget : options.TargetForDependencies);
			// find the project configuration
			foreach (var matching in configMatchings) {
				if (matching.Project == project) {
					node.options.Configuration = matching.Configuration;
					node.options.Platform = matching.Platform;
				}
			}
			if (string.IsNullOrEmpty(node.options.Configuration))
				node.options.Configuration = options.SolutionConfiguration;
			if (string.IsNullOrEmpty(node.options.Platform))
				node.options.Platform = options.SolutionPlatform;
			
			// copy properties to project options
			options.GlobalAdditionalProperties.Foreach(node.options.Properties.Add);
			if (project == rootProject) {
				foreach (var pair in options.ProjectAdditionalProperties) {
					node.options.Properties[pair.Key] = pair.Value;
				}
			}
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
					
					ICSharpCode.Core.LoggingService.Debug("Start building " + node.project.Name);
					runningWorkers++;
					projectsCurrentlyBuilding.Add(node);
					if (hasDependencyErrors) {
						ICSharpCode.Core.LoggingService.Debug("Skipped building " + node.project.Name + " (errors in dependencies)");
						node.hasErrors = true;
						node.Done(false);
					} else {
						node.project.StartBuild(node.options, node);
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
			ICSharpCode.Core.LoggingService.Debug("Finished building " + node.project.Name);
			lock (this) {
				if (node.buildFinished) {
					throw new InvalidOperationException("This node already finished building, do not call IBuildFeedbackSink.Done() multiple times!");
				}
				runningWorkers--;
				node.buildFinished = true;
				node.hasErrors = !success;
				
				projectsCurrentlyBuilding.Remove(node);
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
				
				ReportMessageInternal("Build was cancelled.");
			} else if (rootNode.hasErrors) {
				results.Result = BuildResultCode.Error;
				
				ReportMessageInternal("${res:MainWindow.CompilerMessages.BuildFailed}" + buildTime);
				//StatusBarService.SetMessage("${res:MainWindow.CompilerMessages.BuildFailed}");
			} else {
				results.Result = BuildResultCode.Success;
				
				ReportMessageInternal("${res:MainWindow.CompilerMessages.BuildFinished}" + buildTime);
				//StatusBarService.SetMessage("${res:MainWindow.CompilerMessages.BuildFinished}");
			}
			if (progressMonitor != null) {
				progressMonitor.Done();
			}
			if (combinedBuildFeedbackSink != null) {
				combinedBuildFeedbackSink.Done(results.Result == BuildResultCode.Success);
			}
			if (options.Callback != null) {
				Gui.WorkbenchSingleton.MainForm.BeginInvoke(options.Callback, results);
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
				messagesToReport.Foreach(ReportMessageInternal);
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
					progressMonitor.TaskName = "Building "
						+ projectsCurrentlyBuilding.Select(n => n.project.Name).Join(", ")
						+ "...";
				}
			}
		}
		#endregion
	}
}
