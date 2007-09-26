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
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using Microsoft.Build.BuildEngine;
using Microsoft.Build.Framework;
using ICSharpCode.SharpDevelop.BuildWorker;

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
		
		public static void Build(IProject project, ProjectBuildOptions options, IBuildFeedbackSink feedbackSink)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (options == null)
				throw new ArgumentNullException("options");
			if (feedbackSink == null)
				throw new ArgumentNullException("feedbackSink");
			
			MSBuildEngine engine = new MSBuildEngine(project, options, feedbackSink);
			engine.StartBuild();
		}
		
		IProject project;
		ProjectBuildOptions options;
		IBuildFeedbackSink feedbackSink;
		
		private MSBuildEngine(IProject project, ProjectBuildOptions options, IBuildFeedbackSink feedbackSink)
		{
			this.project = project;
			this.options = options;
			this.feedbackSink = feedbackSink;
		}
		
		void StartBuild()
		{
			WorkerManager.ShowError = MessageService.ShowError;
			BuildWorker.BuildSettings settings = new BuildWorker.BuildSettings();
			SharpDevelopLogger logger = new SharpDevelopLogger(this);
			settings.Logger.Add(logger);
			foreach (IMSBuildAdditionalLogger loggerProvider in MSBuildEngine.AdditionalMSBuildLoggers) {
				settings.Logger.Add(loggerProvider.CreateLogger(this));
			}
			
			BuildJob job = new BuildJob();
			job.ProjectFileName = project.FileName;
			
			BuildPropertyGroup pg = new BuildPropertyGroup();
			MSBuildBasedProject.InitializeMSBuildProjectProperties(pg);
			foreach (BuildProperty p in pg) {
				job.Properties[p.Name] = p.FinalValue;
			}
			
			Solution solution = project.ParentSolution;
			job.Properties["SolutionDir"] = EnsureBackslash(solution.Directory);
			job.Properties["SolutionExt"] = ".sln";
			job.Properties["SolutionFileName"] = Path.GetFileName(solution.FileName);
			job.Properties["SolutionPath"] = solution.FileName;
			
			foreach (var pair in options.Properties) {
				job.Properties[pair.Key] = pair.Value;
			}
			job.Properties["Configuration"] = options.Configuration;
			if (options.Platform == "Any CPU")
				job.Properties["Platform"] = "AnyCPU";
			else
				job.Properties["Platform"] = options.Platform;
			job.Target = options.Target.TargetName;
			
			bool buildInProcess = false;
			if (Interlocked.CompareExchange(ref isBuildingInProcess, 1, 0) == 0) {
				buildInProcess = true;
			}
			
			if (buildInProcess) {
				settings.BuildDoneCallback = delegate(bool success) {
					if (Interlocked.Exchange(ref isBuildingInProcess, 0) != 1) {
						MessageService.ShowError("isBuildingInProcess should have been 1!");
					}
					logger.FlushCurrentError();
					feedbackSink.Done(success);
				};
				
				Thread thread = new Thread(new ThreadStart(
					delegate {
						lock (MSBuildInternals.InProcessMSBuildLock) {
							WorkerManager.RunBuildInProcess(job, settings);
						}
					}));
				thread.Name = "InProcess build thread";
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
			} else {
				settings.BuildDoneCallback = delegate(bool success) {
					logger.FlushCurrentError();
					feedbackSink.Done(success);
				};
				
				WorkerManager.StartBuild(job, settings);
			}
		}
		
		static int isBuildingInProcess;
		
		static string EnsureBackslash(string path)
		{
			if (path.EndsWith("\\"))
				return path;
			else
				return path + "\\";
		}
		
		BuildError currentErrorOrWarning;
		
		/// <summary>
		/// Gets the last build error/warning created by the default
		/// SharpDevelop logger.
		/// </summary>
		public BuildError CurrentErrorOrWarning {
			get {
				return currentErrorOrWarning;
			}
		}
		
		Stack<string> projectFiles = new Stack<string>();
		
		/// <summary>
		/// Gets the name of the currently building project file.
		/// </summary>
		public string CurrentProjectFile {
			get {
				if (projectFiles.Count == 0)
					return null;
				else
					return projectFiles.Peek();
			}
		}
		
		public void OutputText(string message)
		{
			feedbackSink.ReportMessage(message);
		}
		
		public void ReportError(BuildError error)
		{
			feedbackSink.ReportError(error);
		}
		
		class SharpDevelopLogger : ILogger
		{
			MSBuildEngine worker;
			
			public SharpDevelopLogger(MSBuildEngine engine)
			{
				this.worker = engine;
			}
			
			void AppendText(string text)
			{
				worker.OutputText(text);
			}
			
			internal void FlushCurrentError()
			{
				if (worker.currentErrorOrWarning != null) {
					worker.ReportError(worker.currentErrorOrWarning);
					worker.currentErrorOrWarning = null;
				}
			}
			
			void OnProjectStarted(object sender, ProjectStartedEventArgs e)
			{
				worker.projectFiles.Push(e.ProjectFile);
			}
			
			void OnProjectFinished(object sender, ProjectFinishedEventArgs e)
			{
				FlushCurrentError();
				worker.projectFiles.Pop();
			}
			
			string activeTaskName;
			
			void OnTaskStarted(object sender, TaskStartedEventArgs e)
			{
				activeTaskName = e.TaskName;
				if (MSBuildEngine.CompileTaskNames.Contains(e.TaskName.ToLowerInvariant())) {
					AppendText("${res:MainWindow.CompilerMessages.CompileVerb} " + Path.GetFileNameWithoutExtension(e.ProjectFile));
				}
			}
			
			void OnTaskFinished(object sender, TaskFinishedEventArgs e)
			{
				FlushCurrentError();
			}
			
			void OnError(object sender, BuildErrorEventArgs e)
			{
				AppendError(e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message, false);
			}
			
			void OnWarning(object sender, BuildWarningEventArgs e)
			{
				AppendError(e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message, true);
			}
			
			void AppendError(string file, int lineNumber, int columnNumber, string code, string message, bool isWarning)
			{
				if (string.Equals(file, activeTaskName, StringComparison.InvariantCultureIgnoreCase)) {
					file = "";
				} else if (FileUtility.IsValidPath(file)) {
					bool isShortFileName = file == Path.GetFileNameWithoutExtension(file);
					if (worker.CurrentProjectFile != null) {
						file = Path.Combine(Path.GetDirectoryName(worker.CurrentProjectFile), file);
					}
					if (isShortFileName && !File.Exists(file)) {
						file = "";
					}
				}
				FlushCurrentError();
				BuildError error = new BuildError(file, lineNumber, columnNumber, code, message);
				error.IsWarning = isWarning;
				worker.currentErrorOrWarning = error;
			}
			
			#region ILogger interface implementation
			public LoggerVerbosity Verbosity { get; set; }
			public string Parameters { get; set; }
			
			public void Initialize(IEventSource eventSource)
			{
				eventSource.ProjectStarted  += new ProjectStartedEventHandler(OnProjectStarted);
				eventSource.ProjectFinished += new ProjectFinishedEventHandler(OnProjectFinished);
				eventSource.TaskStarted     += new TaskStartedEventHandler(OnTaskStarted);
				eventSource.TaskFinished    += new TaskFinishedEventHandler(OnTaskFinished);
				
				eventSource.ErrorRaised     += new BuildErrorEventHandler(OnError);
				eventSource.WarningRaised   += new BuildWarningEventHandler(OnWarning);
			}
			
			public void Shutdown()
			{
				
			}
			#endregion
		}
	}
}
