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
		const string CompileTaskNamesPath = "/SharpDevelop/MSBuildEngine/CompileTaskNames";
		const string AdditionalTargetFilesPath = "/SharpDevelop/MSBuildEngine/AdditionalTargetFiles";
		const string AdditionalLoggersPath = "/SharpDevelop/MSBuildEngine/AdditionalLoggers";
		internal const string AdditionalPropertiesPath = "/SharpDevelop/MSBuildEngine/AdditionalProperties";
		
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
				AddInTree.BuildItems<string>(CompileTaskNamesPath, null, false),
				StringComparer.OrdinalIgnoreCase
			);
			AdditionalTargetFiles = AddInTree.BuildItems<string>(AdditionalTargetFilesPath, null, false);
			AdditionalMSBuildLoggers = AddInTree.BuildItems<IMSBuildAdditionalLogger>(AdditionalLoggersPath, null, false);
			
			MSBuildProperties = new SortedList<string, string>();
			MSBuildProperties.Add("SharpDevelopBinPath", Path.GetDirectoryName(typeof(MSBuildEngine).Assembly.Location));
			MSBuildProperties.Add("BuildingInsideVisualStudio", "true");
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
				if (MSBuildEngine.isRunning) {
					StartWorkerBuild();
				}
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
				
				// first parse the solution file into a MSBuild project
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
				
				// get the projects that should be built
				IEnumerable<IProject> projectsToBuildWithoutDependencies;
				if (project == null)
					projectsToBuildWithoutDependencies = solution.Projects;
				else
					projectsToBuildWithoutDependencies = GetAllReferencedProjects(project);
				
				projectsToBuild = Linq.ToList(Linq.Select<IProject, ProjectToBuild>(
					projectsToBuildWithoutDependencies,
					p => new ProjectToBuild(p.FileName, options.Target.TargetName)
				));
				
				Dictionary<string, ProjectToBuild> projectDict = new Dictionary<string, ProjectToBuild>(StringComparer.InvariantCultureIgnoreCase);
				foreach (ProjectToBuild ptb in projectsToBuild) {
					projectDict[ptb.file] = ptb;
				}
				
				// use the information from the parsed solution file to determine build level, configuration and
				// platform of the projects to build
				
				for (int level = 0;; level++) {
					BuildItemGroup group = solutionProject.GetEvaluatedItemsByName("BuildLevel" + level);
					if (group.Count == 0)
						break;
					foreach (BuildItem item in group) {
						string path = FileUtility.GetAbsolutePath(solution.Directory, item.Include);
						ProjectToBuild ptb;
						if (projectDict.TryGetValue(path, out ptb)) {
							ptb.level = level;
							ptb.configuration = item.GetEvaluatedMetadata("Configuration");
							ptb.platform = item.GetEvaluatedMetadata("Platform");
						} else {
							parentEngine.MessageView.AppendLine("Cannot build project file: " + path + " (project is not loaded by SharpDevelop)");
						}
					}
				}
				
				projectsToBuild.Sort(
					delegate(ProjectToBuild ptb1, ProjectToBuild ptb2) {
						if (ptb1.level == ptb2.level) {
							return Path.GetFileName(ptb1.file).CompareTo(Path.GetFileName(ptb2.file));
						} else {
							return ptb1.level.CompareTo(ptb2.level);
						}
					});
				
				bool noProjectsToBuild = true;
				for (int i = 0; i < projectsToBuild.Count; i++) {
					if (projectsToBuild[i].level < 0) {
						parentEngine.MessageView.AppendLine("Skipping " + Path.GetFileName(projectsToBuild[i].file));
					} else {
						noProjectsToBuild = false;
						break;
					}
				}
				if (noProjectsToBuild) {
					parentEngine.MessageView.AppendLine("There are no projects to build.");
					Finish();
				}
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
			int currentBuildLevel;
			
			/// <summary>
			/// Find available work and run it on the specified worker.
			/// </summary>
			bool RunWorkerInternal(MSBuildEngineWorker worker)
			{
				ProjectToBuild nextFreeProject = null;
				lock (projectsToBuild) {
					foreach (ProjectToBuild ptb in projectsToBuild) {
						if (ptb.buildStarted == false && ptb.level == currentBuildLevel) {
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
						// are all projects on the current level done?
						bool allDone = true;
						foreach (ProjectToBuild ptb in projectsToBuild) {
							if (ptb.level == currentBuildLevel) {
								if (!ptb.buildFinished) {
									allDone = false;
									break;
								}
							}
						}
						if (allDone) {
							currentBuildLevel++;
							if (currentBuildLevel > projectsToBuild[projectsToBuild.Count - 1].level) {
								return false;
							}
							return true;
						} else {
							// nothing to do for this worker thread
							return false;
						}
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
				
				MSBuildBasedProject.InitializeMSBuildProjectProperties(engine.GlobalProperties);
				
				if (options.AdditionalProperties != null) {
					foreach (KeyValuePair<string, string> entry in options.AdditionalProperties) {
						engine.GlobalProperties.SetProperty(entry.Key, entry.Value);
					}
				}
				engine.GlobalProperties.SetProperty("SolutionDir", EnsureBackslash(solution.Directory));
				engine.GlobalProperties.SetProperty("SolutionExt", ".sln");
				engine.GlobalProperties.SetProperty("SolutionFileName", Path.GetFileName(solution.FileName));
				engine.GlobalProperties.SetProperty("SolutionPath", solution.FileName);
				
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
			
			#region GetAllReferencedProjects
			/// <summary>
			/// Gets all projects referenced by the project.
			/// </summary>
			static ICollection<IProject> GetAllReferencedProjects(IProject project)
			{
				HashSet<IProject> referencedProjects = new HashSet<IProject>();
				Stack<IProject> stack = new Stack<IProject>();
				referencedProjects.Add(project);
				stack.Push(project);
				
				while (stack.Count != 0) {
					project = stack.Pop();
					foreach (ProjectItem item in project.GetItemsOfType(ItemType.ProjectReference)) {
						ProjectReferenceProjectItem prpi = item as ProjectReferenceProjectItem;
						if (prpi != null) {
							if (referencedProjects.Add(prpi.ReferencedProject)) {
								stack.Push(prpi.ReferencedProject);
							}
						}
					}
				}
				
				return referencedProjects;
			}
			#endregion
			
		}
		
		internal sealed class ProjectToBuild
		{
			internal string file;
			internal int level = -1;
			internal string configuration;
			internal string platform;
			internal string targets;
			
			// build status. Three possible values:
			//   buildStarted=buildFinished=false       => build not yet started
			//   buildStarted=true, buildFinished=false => build running
			//   buildStarted=buildFinished=true        => build finished
			internal bool buildStarted;
			internal bool buildFinished;
			
			internal ProjectToBuild(string file, string targets)
			{
				this.file = file;
				this.targets = targets;
			}
		}
	}
}
