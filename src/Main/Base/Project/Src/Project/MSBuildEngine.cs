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
		/// Gets a list where addins can add additional properties for use in MSBuild.
		/// You can add items to this dictionary by putting strings into
		/// "/SharpDevelop/MSBuildEngine/AdditionalProperties".
		/// </summary>
		public static readonly IDictionary<string, string> MSBuildProperties;
		
		/// <summary>
		/// Gets a list of additional target files that are automatically loaded into all projects.
		/// You can add items into this list by putting strings into
		/// "/SharpDevelop/MSBuildEngine/AdditionalTargetFiles"
		/// </summary>
		public static readonly IList<string> AdditionalTargetFiles;
		
		/// <summary>
		/// Gets a list of additional MSBuild loggers.
		/// You can register your loggers by putting them into
		/// "/SharpDevelop/MSBuildEngine/AdditionalLoggers"
		/// </summary>
		public static readonly IList<IMSBuildAdditionalLogger> AdditionalMSBuildLoggers;
		
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
			// we always need this (even when compiling a single project) to prevent MSBuild from automatically
			// building referenced projects
			MSBuildProperties.Add("BuildingSolutionFile", "true");
		}
		
		
		public static void StartBuild(IProject project, ProjectBuildOptions options, IBuildFeedbackSink feedbackSink, IEnumerable<string> additionalTargetFiles)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (options == null)
				throw new ArgumentNullException("options");
			if (feedbackSink == null)
				throw new ArgumentNullException("feedbackSink");
			if (additionalTargetFiles == null)
				throw new ArgumentNullException("additionalTargetFiles");
			
			MSBuildEngine engine = new MSBuildEngine(project, options, feedbackSink);
			engine.additionalTargetFiles = additionalTargetFiles;
			engine.StartBuild();
		}
		
		IProject project;
		ProjectBuildOptions options;
		IBuildFeedbackSink feedbackSink;
		IEnumerable<string> additionalTargetFiles;
		
		private MSBuildEngine(IProject project, ProjectBuildOptions options, IBuildFeedbackSink feedbackSink)
		{
			this.project = project;
			this.options = options;
			this.feedbackSink = feedbackSink;
		}
		
		const EventTypes ControllableEvents = EventTypes.Message
			| EventTypes.TargetStarted | EventTypes.TargetFinished
			| EventTypes.TaskStarted | EventTypes.TaskFinished
			| EventTypes.Unknown;
		
		/// <summary>
		/// Controls whether messages should be made available to loggers.
		/// Logger AddIns should set this property in their CreateLogger method.
		/// </summary>
		public bool ReportMessageEvents { get; set; }
		
		/// <summary>
		/// Controls whether the TargetStarted event should be made available to loggers.
		/// Logger AddIns should set this property in their CreateLogger method.
		/// </summary>
		public bool ReportTargetStartedEvents { get; set; }
		
		/// <summary>
		/// Controls whether the TargetStarted event should be made available to loggers.
		/// Logger AddIns should set this property in their CreateLogger method.
		/// </summary>
		public bool ReportTargetFinishedEvents { get; set; }
		
		/// <summary>
		/// Controls whether all TaskStarted events should be made available to loggers.
		/// Logger AddIns should set this property in their CreateLogger method.
		/// </summary>
		public bool ReportAllTaskStartedEvents { get; set; }
		
		/// <summary>
		/// Controls whether all TaskFinished events should be made available to loggers.
		/// Logger AddIns should set this property in their CreateLogger method.
		/// </summary>
		public bool ReportAllTaskFinishedEvents { get; set; }
		
		/// <summary>
		/// Controls whether the AnyEventRaised and StatusEventRaised events should
		/// be called for unknown events.
		/// Logger AddIns should set this property in their CreateLogger method.
		/// </summary>
		public bool ReportUnknownEvents { get; set; }
		
		List<string> interestingTasks = new List<string>();
		
		/// <summary>
		/// The list of task names for which TaskStarted and TaskFinished events should be
		/// made available to loggers.
		/// Logger AddIns should add entries in their CreateLogger method.
		/// </summary>
		public ICollection<string> InterestingTasks {
			get { return interestingTasks; }
		}
		
		static bool AllowBuildInProcess {
			get {
				// don't build in-process if building in parallel.
				// workaround for SD2-1485: Build worker occasionally hangs
				return BuildOptions.DefaultParallelProjectCount == 1;
			}
		}
		
		void StartBuild()
		{
			WorkerManager.ShowError = MessageService.ShowError;
			BuildWorker.BuildSettings settings = new BuildWorker.BuildSettings();
			SharpDevelopLogger logger = new SharpDevelopLogger(this);
			settings.Logger.Add(logger);
			
			InterestingTasks.AddRange(MSBuildEngine.CompileTaskNames);
			foreach (IMSBuildAdditionalLogger loggerProvider in MSBuildEngine.AdditionalMSBuildLoggers) {
				settings.Logger.Add(loggerProvider.CreateLogger(this));
			}
			
			BuildJob job = new BuildJob();
			job.IntPtrSize = IntPtr.Size;
			job.ProjectFileName = project.FileName;
			// Never report custom events (those are usually derived EventArgs classes, and SharpDevelop
			// doesn't have the matching assemblies loaded - see SD2-1514).
			// Also, remove the flags for the controllable events.
			job.EventMask = EventTypes.All & ~(ControllableEvents | EventTypes.Custom);
			// Add back active controllable events.
			if (ReportMessageEvents)
				job.EventMask |= EventTypes.Message;
			if (ReportTargetStartedEvents)
				job.EventMask |= EventTypes.TargetStarted;
			if (ReportTargetFinishedEvents)
				job.EventMask |= EventTypes.TargetFinished;
			if (ReportAllTaskStartedEvents)
				job.EventMask |= EventTypes.TaskStarted;
			if (ReportAllTaskFinishedEvents)
				job.EventMask |= EventTypes.TaskFinished;
			if (ReportUnknownEvents)
				job.EventMask |= EventTypes.Unknown;
			
			if (!(ReportAllTaskStartedEvents && ReportAllTaskFinishedEvents)) {
				// just some TaskStarted & TaskFinished events should be reported
				job.InterestingTaskNames.AddRange(InterestingTasks);
			}
			
			job.AdditionalImports.AddRange(additionalTargetFiles);
			
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
			if (AllowBuildInProcess && Interlocked.CompareExchange(ref isBuildingInProcess, 1, 0) == 0) {
				buildInProcess = true;
			}
			LoggingService.Info("Start job (buildInProcess=" + buildInProcess + "): " + job.ToString());
			
			if (buildInProcess) {
				settings.BuildDoneCallback = delegate(bool success) {
					LoggingService.Debug("BuildInProcess: Received BuildDoneCallback");
					if (Interlocked.Exchange(ref isBuildingInProcess, 0) != 1) {
						MessageService.ShowError("isBuildingInProcess should have been 1!");
					}
					logger.FlushCurrentError();
					feedbackSink.Done(success);
				};
				
				Thread thread = new Thread(new ThreadStart(
					delegate {
						LoggingService.Debug("Acquiring InProcessMSBuildLock");
						lock (MSBuildInternals.InProcessMSBuildLock) {
							WorkerManager.RunBuildInProcess(job, settings);
							LoggingService.Debug("Leaving InProcessMSBuildLock");
						}
					}));
				thread.Name = "InProcess build thread " + thread.ManagedThreadId;
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
			} else {
				settings.BuildDoneCallback = delegate(bool success) {
					LoggingService.Debug("BuildOutOfProcess: Received BuildDoneCallback");
					logger.FlushCurrentError();
					feedbackSink.Done(success);
				};
				
				WorkerManager.StartBuild(job, settings);
			}
		}
		
		static int isBuildingInProcess = 0;
		
		static string EnsureBackslash(string path)
		{
			if (path.EndsWith("\\", StringComparison.Ordinal))
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
			
			// TODO: Add XmlDocBloc to MSBuildError.AppendError()
			void AppendError(string file, int lineNumber, int columnNumber, string code, string message, bool isWarning)
			{
				if (string.Equals(file, activeTaskName, StringComparison.OrdinalIgnoreCase)) {
					file = "";
				} else if (FileUtility.IsValidPath(file)) {
					bool isShortFileName = file == Path.GetFileNameWithoutExtension(file);
					if (worker.CurrentProjectFile != null) {
						file = Path.Combine(Path.GetDirectoryName(worker.CurrentProjectFile), file);
					}
					if (isShortFileName && !File.Exists(file)) {
						file = "";
					}
					//TODO: Do we have to check for other SDKs here.
					else if (file.EndsWith(".targets", StringComparison.OrdinalIgnoreCase)
					         && (FileUtility.IsBaseDirectory(FileUtility.NetFrameworkInstallRoot, file)
					             || FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, file)))
					{
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
			
			IEventSource eventSource;
			
			public void Initialize(IEventSource eventSource)
			{
				this.eventSource = eventSource;
				eventSource.ProjectStarted  += OnProjectStarted;
				eventSource.ProjectFinished += OnProjectFinished;
				eventSource.TaskStarted     += OnTaskStarted;
				eventSource.TaskFinished    += OnTaskFinished;
				
				eventSource.ErrorRaised     += OnError;
				eventSource.WarningRaised   += OnWarning;
			}
			
			public void Shutdown()
			{
				eventSource.ProjectStarted  -= OnProjectStarted;
				eventSource.ProjectFinished -= OnProjectFinished;
				eventSource.TaskStarted     -= OnTaskStarted;
				eventSource.TaskFinished    -= OnTaskFinished;
				
				eventSource.ErrorRaised     -= OnError;
				eventSource.WarningRaised   -= OnWarning;
			}
			#endregion
		}
	}
}
