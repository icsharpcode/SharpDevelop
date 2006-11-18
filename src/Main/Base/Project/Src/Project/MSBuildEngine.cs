// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using Microsoft.Build.BuildEngine;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.Project
{
	// let Project refer to the class, not to the namespace
	using Project = Microsoft.Build.BuildEngine.Project;
	
	/// <summary>
	/// Class responsible for building a project using MSBuild.
	/// Is called by MSBuildProject.
	/// </summary>
	public sealed class MSBuildEngine
	{
		/// <summary>
		/// Gets a list of the task names that cause a "Compiling ..." log message.
		/// You can add items to this list by putting strings into
		/// "/SharpDevelop/MSBuildEngine/CompileTaskNames".
		/// </summary>
		public static readonly ICollection<string> CompileTaskNames;
		
		/// <summary>
		/// Gets a list where addins can add additional properties for use in MsBuild.
		/// </summary>
		public static readonly SortedList<string, string> MSBuildProperties;
		
		/// <summary>
		/// Gets a list of additional target files that are automatically loaded into all projects.
		/// You can add items into this list by putting strings into
		/// "/SharpDevelop/MSBuildEngine/AdditionalTargetFiles"
		/// </summary>
		public static readonly List<string> AdditionalTargetFiles;
		
		/// <summary>
		/// Gets a list of additional MSBuild loggers.
		/// You can register your loggers by putting them into
		/// "/SharpDevelop/MSBuildEngine/AdditionalLoggers"
		/// </summary>
		public static readonly List<IMSBuildAdditionalLogger> AdditionalMSBuildLoggers;
		
		static MSBuildEngine()
		{
			CompileTaskNames = new Set<string>(
				AddInTree.BuildItems<string>("/SharpDevelop/MSBuildEngine/CompileTaskNames", null, false),
				StringComparer.OrdinalIgnoreCase
			);
			AdditionalTargetFiles = AddInTree.BuildItems<string>("/SharpDevelop/MSBuildEngine/AdditionalTargetFiles", null, false);
			AdditionalMSBuildLoggers = AddInTree.BuildItems<IMSBuildAdditionalLogger>("/SharpDevelop/MSBuildEngine/AdditionalLoggers", null, false);
			
			MSBuildProperties = new SortedList<string, string>();
			MSBuildProperties.Add("SharpDevelopBinPath", Path.GetDirectoryName(typeof(MSBuildEngine).Assembly.Location));
		}
		
		#region Properties
		MessageViewCategory messageView;
		
		/// <summary>
		/// The <see cref="MessageViewCategory"/> the output is written to.
		/// </summary>
		public MessageViewCategory MessageView {
			get {
				return messageView;
			}
			set {
				messageView = value;
			}
		}
		
		string configuration;
		
		/// <summary>
		/// The configuration of the solution or project that should be builded.
		/// Use null to build the default configuration.
		/// </summary>
		public string Configuration {
			get {
				return configuration;
			}
			set {
				configuration = value;
			}
		}
		
		string platform;
		
		/// <summary>
		/// The platform of the solution or project that should be builded.
		/// Use null to build the default platform.
		/// </summary>
		public string Platform {
			get {
				return platform;
			}
			set {
				platform = value;
			}
		}
		#endregion
		
		volatile static bool isRunning = false;
		
		public void Run(Solution solution, IProject project, BuildOptions options)
		{
			if (isRunning) {
				BuildResults results = new BuildResults();
				results.Result = BuildResultCode.MSBuildAlreadyRunning;
				results.Add(new BuildError(null, ResourceService.GetString("MainWindow.CompilerMessages.MSBuildAlreadyRunning")));
				if (options.Callback != null) {
					options.Callback(results);
				}
			} else {
				isRunning = true;
				Thread thread = new Thread(new BuildRun(solution, project, options, this).RunMainBuild);
				thread.Name = "MSBuildEngine main worker";
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
			}
		}
		
		internal sealed class BuildRun
		{
			Solution solution;
			IProject project;
			BuildOptions options;
			MSBuildEngine parentEngine;
			internal BuildResults currentResults = new BuildResults();
			List<ProjectToBuild> projectsToBuild = new List<ProjectToBuild>();
			
			public BuildRun(Solution solution, IProject project, BuildOptions options, MSBuildEngine parentEngine)
			{
				this.solution = solution;
				this.project = project;
				this.options = options;
				this.parentEngine = parentEngine;
			}
			
			#region Main Build
			[STAThread]
			internal void RunMainBuild()
			{
				try {
					PrepareBuild();
				} catch (Exception ex) {
					MessageService.ShowError(ex);
				}
				StartWorkerBuild();
			}
			
			void Finish()
			{
				LoggingService.Debug("MSBuild finished");
				MSBuildEngine.isRunning = false;
				if (currentResults.Result == BuildResultCode.None) {
					currentResults.Result = BuildResultCode.Success;
				}
				if (currentResults.Result == BuildResultCode.Success) {
					parentEngine.MessageView.AppendLine("${res:MainWindow.CompilerMessages.BuildFinished}");
					StatusBarService.SetMessage("${res:MainWindow.CompilerMessages.BuildFinished}");
				} else {
					parentEngine.MessageView.AppendLine("${res:MainWindow.CompilerMessages.BuildFailed}");
					StatusBarService.SetMessage("${res:MainWindow.CompilerMessages.BuildFailed}");
				}
				if (options.Callback != null) {
					WorkbenchSingleton.MainForm.BeginInvoke(options.Callback, currentResults);
				}
			}
			
			void PrepareBuild()
			{
				parentEngine.MessageView.AppendLine("${res:MainWindow.CompilerMessages.BuildStarted}");
				
				if (project == null) {
					LoggingService.Debug("Parsing solution file " + solution.FileName);
					
					Engine engine = CreateEngine();
					if (parentEngine.Configuration != null) {
						engine.GlobalProperties.SetProperty("Configuration", parentEngine.Configuration);
					}
					if (parentEngine.Platform != null) {
						engine.GlobalProperties.SetProperty("Platform", parentEngine.Platform);
					}
					Project solutionProject = LoadProject(engine, solution.FileName);
					if (solutionProject == null) {
						Finish();
						return;
					}
					if (!ParseSolution(solutionProject)) {
						Finish();
						return;
					}
				} else {
					if (ParseMSBuildProject(project) == null) {
						Finish();
						return;
					}
				}
				SortProjectsToBuild();
			}
			#endregion
			
			#region Worker build
			int workerCount;
			int maxWorkerCount;
			
			/// <summary>
			/// Runs the first worker on this thread and creates new threads if required
			/// </summary>
			void StartWorkerBuild()
			{
				workerCount = 1;
				// one build worker thread is maximum - MSBuild sets the working directory
				// and thus cannot run multiple times in the same process
				maxWorkerCount = 1;
				RunWorkerBuild();
			}
			
			// Reuse worker objects: improves performance because MSBuild can cache Xml documents
			Queue<MSBuildEngineWorker> unusedWorkers = new Queue<MSBuildEngineWorker>();
			
			/// <summary>
			/// Runs one worker thread
			/// </summary>
			[STAThread]
			void RunWorkerBuild()
			{
				LoggingService.Debug("Build Worker thread started");
				MSBuildEngineWorker worker = null;
				try {
					lock (projectsToBuild) {
						if (unusedWorkers.Count > 0)
							worker = unusedWorkers.Dequeue();
					}
					if (worker == null) {
						worker = new MSBuildEngineWorker(parentEngine, this);
					}
					while (RunWorkerInternal(worker));
				} catch (Exception ex) {
					MessageService.ShowError(ex);
				} finally {
					bool wasLastWorker;
					lock (projectsToBuild) {
						workerCount--;
						wasLastWorker = workerCount == 0;
						if (worker != null) {
							unusedWorkers.Enqueue(worker);
						}
					}
					LoggingService.Debug("Build Worker thread finished");
					if (wasLastWorker) {
						Finish();
					}
				}
			}
			
			int lastUniqueWorkerID;
			
			/// <summary>
			/// Find available work and run it on the specified worker.
			/// </summary>
			bool RunWorkerInternal(MSBuildEngineWorker worker)
			{
				ProjectToBuild nextFreeProject = null;
				lock (projectsToBuild) {
					foreach (ProjectToBuild ptb in projectsToBuild) {
						if (ptb.buildStarted == false && ptb.DependenciesSatisfied()) {
							if (nextFreeProject == null) {
								nextFreeProject = ptb;
								
								// all workers busy, don't look if there is more work available
								if (workerCount == maxWorkerCount)
									break;
							} else {
								// free workers available + additional work available:
								// start a new worker
								LoggingService.Debug("Starting a new worker");
								workerCount++;
								Thread thread = new Thread(RunWorkerBuild);
								thread.Name = "MSBuildEngine worker " + (++lastUniqueWorkerID);
								thread.SetApartmentState(ApartmentState.STA);
								thread.Start();
								
								// start at most one additional worker, the new worker can
								// start more threads if desired
								break;
							}
						}
					}
					if (nextFreeProject == null) {
						// nothing to do for this worker thread
						return false;
					}
					// now build nextFreeProject
					nextFreeProject.buildStarted = true;
				} // end lock
				
				StatusBarService.SetMessage("${res:MainWindow.CompilerMessages.BuildVerb} " + Path.GetFileNameWithoutExtension(nextFreeProject.file) + "...");
				
				// run the build:
				if (worker.Build(nextFreeProject)) {
					// build successful: mark it as finished
					lock (projectsToBuild) {
						nextFreeProject.buildFinished = true;
					}
				}
				return true;
			}
			#endregion
			
			#region Managing the output lock
			/// <summary>
			/// Queue of output text to write when lock is released.
			/// 
			/// Also serves as object to ensure access to outputLockIsAquired is thread-safe.
			/// </summary>
			Queue<string> queuedOutputText = new Queue<string>();
			volatile bool outputLockIsAquired;
			
			internal bool TryAquireOutputLock()
			{
				lock (queuedOutputText) {
					if (outputLockIsAquired) {
						return false;
					} else {
						outputLockIsAquired = true;
						return true;
					}
				}
			}
			
			internal void ReleaseOutputLock()
			{
				lock (queuedOutputText) {
					outputLockIsAquired = false;
					while (queuedOutputText.Count > 0) {
						parentEngine.MessageView.AppendText(queuedOutputText.Dequeue());
					}
				}
			}
			
			internal void EnqueueTextForAppendWhenOutputLockIsReleased(string text)
			{
				lock (queuedOutputText) {
					if (outputLockIsAquired) {
						queuedOutputText.Enqueue(text);
					} else {
						parentEngine.MessageView.AppendText(text);
					}
				}
			}
			#endregion
			
			#region CreateEngine / LoadProject
			internal Engine CreateEngine()
			{
				Engine engine = MSBuildInternals.CreateEngine();
				foreach (KeyValuePair<string, string> entry in MSBuildProperties) {
					engine.GlobalProperties.SetProperty(entry.Key, entry.Value);
				}
				if (options.AdditionalProperties != null) {
					foreach (KeyValuePair<string, string> entry in options.AdditionalProperties) {
						engine.GlobalProperties.SetProperty(entry.Key, entry.Value);
					}
				}
				engine.GlobalProperties.SetProperty("SolutionDir", EnsureBackslash(solution.Directory));
				engine.GlobalProperties.SetProperty("SolutionExt", ".sln");
				engine.GlobalProperties.SetProperty("SolutionFileName", Path.GetFileName(solution.FileName));
				engine.GlobalProperties.SetProperty("SolutionPath", solution.FileName);
				
				engine.GlobalProperties.SetProperty("BuildingInsideVisualStudio", "true");
				
				return engine;
			}
			
			static string EnsureBackslash(string path)
			{
				if (path.EndsWith("\\"))
					return path;
				else
					return path + "\\";
			}
			
			internal Project LoadProject(Engine engine, string fileName)
			{
				Project project = engine.CreateNewProject();
				try {
					project.Load(fileName);
					
					// When we set BuildingInsideVisualStudio, MSBuild tries to build all projects
					// every time because in Visual Studio, the host compiler does the change detection
					// We override the property '_ComputeNonExistentFileProperty' which is responsible
					// for recompiling each time - our _ComputeNonExistentFileProperty does nothing,
					// which re-enables the MSBuild's usual change detection
					project.Targets.AddNewTarget("_ComputeNonExistentFileProperty");
					
					return project;
				} catch (ArgumentException ex) {
					currentResults.Result = BuildResultCode.BuildFileError;
					currentResults.Add(new BuildError("", ex.Message));
				} catch (InvalidProjectFileException ex) {
					currentResults.Result = BuildResultCode.BuildFileError;
					currentResults.Add(new BuildError(ex.ProjectFile, ex.LineNumber, ex.ColumnNumber, ex.ErrorCode, ex.Message));
				}
				return null;
			}
			#endregion
			
			#region ParseSolution
			bool ParseSolution(Project solution)
			{
				// get the build target to call
				Target mainBuildTarget = solution.Targets[options.Target.TargetName];
				if (mainBuildTarget == null) {
					currentResults.Result = BuildResultCode.BuildFileError;
					currentResults.Add(new BuildError(this.solution.FileName, "Target '" + options.Target + "' not supported by solution."));
					return false;
				}
				// example of mainBuildTarget:
				//  <Target Name="Build" Condition="'$(CurrentSolutionConfigurationContents)' != ''">
				//    <CallTarget Targets="Main\ICSharpCode_SharpDevelop;Main\ICSharpCode_Core;Main\StartUp;Tools" RunEachTargetSeparately="true" />
				//  </Target>
				List<BuildTask> mainBuildTargetTasks = Linq.ToList(Linq.CastTo<BuildTask>(mainBuildTarget));
				if (mainBuildTargetTasks.Count != 1
				    || mainBuildTargetTasks[0].Name != "CallTarget")
				{
					return InvalidTarget(mainBuildTarget);
				}
				
				List<Target> solutionTargets = new List<Target>();
				foreach (string solutionTargetName in mainBuildTargetTasks[0].GetParameterValue("Targets").Split(';'))
				{
					Target target = solution.Targets[solutionTargetName];
					if (target != null) {
						solutionTargets.Add(target);
					}
				}
				
				// dictionary for fast lookup of ProjectToBuild elements
				Dictionary<string, ProjectToBuild> projectsToBuildDict = new Dictionary<string, ProjectToBuild>();
				
				// now look through targets that took like this:
				//  <Target Name="Main\ICSharpCode_Core" Condition="'$(CurrentSolutionConfigurationContents)' != ''">
				//    <MSBuild Projects="Main\Core\Project\ICSharpCode.Core.csproj" Properties="Configuration=Debug; Platform=AnyCPU; BuildingSolutionFile=true; CurrentSolutionConfigurationContents=$(CurrentSolutionConfigurationContents); SolutionDir=$(SolutionDir); SolutionExt=$(SolutionExt); SolutionFileName=$(SolutionFileName); SolutionName=$(SolutionName); SolutionPath=$(SolutionPath)" Condition=" ('$(Configuration)' == 'Debug') and ('$(Platform)' == 'Any CPU') " />
				//    <MSBuild Projects="Main\Core\Project\ICSharpCode.Core.csproj" Properties="Configuration=Release; Platform=AnyCPU; BuildingSolutionFile=true; CurrentSolutionConfigurationContents=$(CurrentSolutionConfigurationContents); SolutionDir=$(SolutionDir); SolutionExt=$(SolutionExt); SolutionFileName=$(SolutionFileName); SolutionName=$(SolutionName); SolutionPath=$(SolutionPath)" Condition=" ('$(Configuration)' == 'Release') and ('$(Platform)' == 'Any CPU') " />
				//  </Target>
				// and add those targets to the "projectsToBuild" list.
				foreach (Target target in solutionTargets) {
					List<BuildTask> tasks = Linq.ToList(Linq.CastTo<BuildTask>(target));
					if (tasks.Count == 0) {
						return InvalidTarget(target);
					}
					
					// find task to run when this target is executed
					BuildTask bestTask = null;
					foreach (BuildTask task in tasks) {
						if (task.Name != "MSBuild") {
							return InvalidTarget(target);
						}
						if (MSBuildInternals.EvaluateCondition(solution, task.Condition)) {
							bestTask = task;
						}
					}
					if (bestTask == null) {
						LoggingService.Warn("No matching condition for solution target " + target.Name);
						bestTask = tasks[0];
					}
					
					// create projectToBuild entry and add it to list and dictionary
					string projectFileName = Path.Combine(this.solution.Directory, bestTask.GetParameterValue("Projects"));
					ProjectToBuild projectToBuild = new ProjectToBuild(Path.GetFullPath(projectFileName),
					                                                   bestTask.GetParameterValue("Targets"));
					
					// get project configuration and platform from properties section
					string propertiesString = bestTask.GetParameterValue("Properties");
					Match match = Regex.Match(propertiesString, @"\bConfiguration=([^;]+);");
					if (match.Success) {
						projectToBuild.configuration = match.Groups[1].Value;
					} else {
						projectToBuild.configuration = parentEngine.Configuration;
					}
					match = Regex.Match(propertiesString, @"\bPlatform=([^;]+);");
					if (match.Success) {
						projectToBuild.platform = match.Groups[1].Value;
					} else {
						projectToBuild.platform = parentEngine.Platform;
						if (projectToBuild.platform == "Any CPU") {
							projectToBuild.platform = "AnyCPU";
						}
					}
					
					projectsToBuild.Add(projectToBuild);
					projectsToBuildDict[target.Name] = projectToBuild;
				}
				
				// now create dependencies between projectsToBuild
				foreach (Target target in solutionTargets) {
					ProjectToBuild p1;
					if (!projectsToBuildDict.TryGetValue(target.Name, out p1))
						continue;
					foreach (string dependency in target.DependsOnTargets.Split(';')) {
						ProjectToBuild p2;
						if (!projectsToBuildDict.TryGetValue(dependency, out p2))
							continue;
						p1.dependencies.Add(p2);
					}
				}
				return true;
			}
			
			/// <summary>
			/// Adds an error message that the specified target is invalid and returns false.
			/// </summary>
			bool InvalidTarget(Target target)
			{
				currentResults.Result = BuildResultCode.BuildFileError;
				currentResults.Add(new BuildError(this.solution.FileName, "Solution target '" + target.Name + "' is invalid."));
				return false;
			}
			#endregion
			
			#region ParseMSBuildProject
			Dictionary<IProject, ProjectToBuild> parseMSBuildProjectProjectsToBuildDict = new Dictionary<IProject, ProjectToBuild>();
			
			/// <summary>
			/// Adds a ProjectToBuild item for the project and it's project references.
			/// Returns the added item, or null if an error occured.
			/// </summary>
			ProjectToBuild ParseMSBuildProject(IProject project)
			{
				ProjectToBuild ptb;
				if (parseMSBuildProjectProjectsToBuildDict.TryGetValue(project, out ptb)) {
					// only add each project once, reuse existing ProjectToBuild
					return ptb;
				}
				ptb = new ProjectToBuild(project.FileName, options.Target.TargetName);
				ptb.configuration = parentEngine.Configuration;
				ptb.platform = parentEngine.Platform;
				
				projectsToBuild.Add(ptb);
				parseMSBuildProjectProjectsToBuildDict[project] = ptb;
				
				foreach (ProjectItem item in project.GetItemsOfType(ItemType.ProjectReference)) {
					ProjectReferenceProjectItem prpi = item as ProjectReferenceProjectItem;
					if (prpi != null && prpi.ReferencedProject != null) {
						ProjectToBuild referencedProject = ParseMSBuildProject(prpi.ReferencedProject);
						if (referencedProject == null)
							return null;
						ptb.dependencies.Add(referencedProject);
					}
				}
				
				return ptb;
			}
			#endregion
			
			#region SortProjectsToBuild
			/// <summary>
			/// Recursively count dependencies and sort projects (most important first).
			/// This decreases the number of waiting workers on multi-processor builds
			/// </summary>
			void SortProjectsToBuild()
			{
				// count:
				try {
					foreach (ProjectToBuild ptb in projectsToBuild) {
						projectsToBuild.ForEach(delegate(ProjectToBuild p) { p.visitFlag = 0; });
						ptb.dependencies.ForEach(IncrementRequiredByCount);
					}
				} catch (DependencyCycleException) {
					currentResults.Add(new BuildError(null, "Dependency cycle detected, cannot build!"));
					return;
				}
				// sort by requiredByCount, decreasing
				projectsToBuild.Sort(delegate (ProjectToBuild a, ProjectToBuild b) {
				                     	return -a.requiredByCount.CompareTo(b.requiredByCount);
				                     });
			}
			
			/// <summary>
			/// Recursively increment requiredByCount on ptb and all its dependencies
			/// </summary>
			static void IncrementRequiredByCount(ProjectToBuild ptb)
			{
				if (ptb.visitFlag == 1) {
					return;
				}
				if (ptb.visitFlag == -1) {
					throw new DependencyCycleException();
				}
				ptb.visitFlag = -1;
				ptb.requiredByCount++;
				ptb.dependencies.ForEach(IncrementRequiredByCount);
				ptb.visitFlag = 1;
			}
			
			class DependencyCycleException : Exception {}
			#endregion
		}
		
		/// <summary>
		/// node used for project dependency graph
		/// </summary>
		internal class ProjectToBuild
		{
			// information required to build the project
			internal string file;
			internal string targets;
			internal string configuration, platform;
			
			internal List<ProjectToBuild> dependencies = new List<ProjectToBuild>();
			
			internal bool DependenciesSatisfied()
			{
				return dependencies.TrueForAll(delegate(ProjectToBuild p) { return p.buildFinished; });
			}
			
			/// <summary>
			/// Number of projects that are directly or indirectly dependent on this project.
			/// Used in SortProjectsToBuild step.
			/// </summary>
			internal int requiredByCount;
			
			/// <summary>
			/// Mark already visited nodes. 0 = not visited, -1 = visiting, 1 = visited
			/// Used in SortProjectsToBuild step.
			/// </summary>
			internal int visitFlag;
			
			// build status. Three possible values:
			//   buildStarted=buildFinished=false       => build not yet started
			//   buildStarted=true, buildFinished=false => build running
			//   buildStarted=buildFinished=true        => build finished
			internal bool buildStarted;
			internal bool buildFinished;
			
			public ProjectToBuild(string file, string targets)
			{
				this.file = file;
				this.targets = targets;
			}
		}
	}
}
