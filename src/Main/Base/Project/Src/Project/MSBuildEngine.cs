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

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using Microsoft.Build.BuildEngine;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.Project
{
	public delegate void MSBuildEngineCallback(BuildResults results);
	
	/// <summary>
	/// Class responsible for building a project using MSBuild.
	/// Is called by MSBuildProject.
	/// </summary>
	public class MSBuildEngine
	{
		/// <summary>
		/// Gets a list of the task names that cause a "Compiling ..." log message.
		/// You can add items to this list by putting strings into
		/// "/SharpDevelop/MSBuildEngine/CompileTaskNames".
		/// </summary>
		public static readonly List<string> CompileTaskNames;
		
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
			CompileTaskNames = AddInTree.BuildItems<string>("/SharpDevelop/MSBuildEngine/CompileTaskNames", null, false);
			for (int i = 0; i < CompileTaskNames.Count; i++) {
				CompileTaskNames[i] = CompileTaskNames[i].ToLowerInvariant();
			}
			AdditionalTargetFiles = AddInTree.BuildItems<string>("/SharpDevelop/MSBuildEngine/AdditionalTargetFiles", null, false);
			AdditionalMSBuildLoggers = AddInTree.BuildItems<IMSBuildAdditionalLogger>("/SharpDevelop/MSBuildEngine/AdditionalLoggers", null, false);
			
			MSBuildProperties = new SortedList<string, string>();
			MSBuildProperties.Add("SharpDevelopBinPath", Path.GetDirectoryName(typeof(MSBuildEngine).Assembly.Location));
		}
		
		#region relocated from ICSharpCode.SharpDevelop.Project.Commands.Build in BuildCommands.cs
		public static int LastErrorCount;
		public static int LastWarningCount;
		
		public static void ShowResults(BuildResults results)
		{
			if (results != null) {
				LastErrorCount = 0;
				LastWarningCount = 0;
				TaskService.InUpdate = true;
				foreach (BuildError error in results.Errors) {
					TaskService.Add(new Task(error));
					if (error.IsWarning)
						LastWarningCount++;
					else
						LastErrorCount++;
				}
				TaskService.InUpdate = false;
				if (results.Errors.Count > 0 && ErrorListPad.ShowAfterBuild) {
					WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
				}
			}
		}
		
		/// <summary>
		/// Notifies the user that #develp's internal MSBuildEngine
		/// implementation only supports compiling solutions and projects;
		/// it does not allow compiling individual files.
		/// </summary>
		/// <remarks>Adds a message to the <see cref="TaskService"/> and
		/// shows the <see cref="ErrorListPad"/>.</remarks>
		public static void AddNoSingleFileCompilationError()
		{
			LastErrorCount = 1;
			LastWarningCount = 0;
			TaskService.Add(new Task(null, StringParser.Parse("${res:BackendBindings.ExecutionManager.NoSingleFileCompilation}"), 0, 0, TaskType.Error));
			WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
		}
		#endregion
		
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
		
		SortedList<string, string> additionalProperties = new SortedList<string, string>();
		
		public IDictionary<string, string> AdditionalProperties {
			get {
				return additionalProperties;
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
		
		BuildResults currentResults;
		
		public BuildResults CurrentResults {
			get {
				return currentResults;
			}
		}
		
		public void Run(string buildFile, MSBuildEngineCallback callback)
		{
			Run(buildFile, null, callback);
		}
		
		volatile static bool isRunning = false;
		
		public void Run(string buildFile, string[] targets, MSBuildEngineCallback callback)
		{
			if (isRunning) {
				BuildResults results = new BuildResults();
				results.Result = BuildResultCode.MSBuildAlreadyRunning;
				results.Errors.Add(new BuildError(null, 0, 0, null, ResourceService.GetString("MainWindow.CompilerMessages.MSBuildAlreadyRunning")));
				callback(results);
			} else {
				isRunning = true;
				Thread thread = new Thread(new ThreadStarter(buildFile, targets, this, callback).Run);
				thread.Name = "MSBuildEngine";
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
			}
		}
		
		class ThreadStarter
		{
			string buildFile;
			string[] targets;
			MSBuildEngine engine;
			MSBuildEngineCallback callback;
			
			public ThreadStarter(string buildFile, string[] targets, MSBuildEngine engine, MSBuildEngineCallback callback)
			{
				engine.currentResults = new BuildResults();
				this.buildFile = buildFile;
				this.targets = targets;
				this.engine = engine;
				this.callback = callback;
			}
			
			[STAThread]
			public void Run()
			{
				BuildResults results = this.engine.currentResults;
				LoggingService.Debug("Run MSBuild on " + buildFile);
				
				Engine engine = new Engine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory());
				if (this.engine.Configuration != null) {
					engine.GlobalProperties.SetProperty("Configuration", this.engine.Configuration);
				}
				if (this.engine.Platform != null) {
					engine.GlobalProperties.SetProperty("Platform", this.engine.Platform);
				}
				foreach (KeyValuePair<string, string> entry in MSBuildProperties) {
					engine.GlobalProperties.SetProperty(entry.Key, entry.Value);
				}
				foreach (KeyValuePair<string, string> entry in this.engine.additionalProperties) {
					engine.GlobalProperties.SetProperty(entry.Key, entry.Value);
				}
				
				SharpDevelopLogger logger = new SharpDevelopLogger(this.engine);
				engine.RegisterLogger(logger);
				foreach (IMSBuildAdditionalLogger loggerProvider in MSBuildEngine.AdditionalMSBuildLoggers) {
					engine.RegisterLogger(loggerProvider.CreateLogger(this.engine));
				}
				
				Microsoft.Build.BuildEngine.Project project = engine.CreateNewProject();
				try {
					project.Load(buildFile);
					
					foreach (string targetFile in AdditionalTargetFiles) {
						project.AddNewImport(targetFile, null);
					}
					
					if (engine.BuildProject(project, targets))
						results.Result = BuildResultCode.Success;
					else
						results.Result = BuildResultCode.Error;
				} catch (ArgumentException ex) {
					results.Result = BuildResultCode.BuildFileError;
					results.Errors.Add(new BuildError(null, -1, -1, "", ex.Message));
				} catch (InvalidProjectFileException ex) {
					results.Result = BuildResultCode.BuildFileError;
					results.Errors.Add(new BuildError(ex.ProjectFile, ex.LineNumber, ex.ColumnNumber, ex.ErrorCode, ex.Message));
				}
				
				logger.FlushCurrentError();
				
				LoggingService.Debug("MSBuild finished");
				MSBuildEngine.isRunning = false;
				if (callback != null) {
					WorkbenchSingleton.MainForm.BeginInvoke(callback, results);
				}
				engine.UnloadAllProjects();
				this.engine.currentResults = null;
			}
		}
		
		class SharpDevelopLogger : ILogger
		{
			MSBuildEngine engine;
			BuildResults results;
			
			public SharpDevelopLogger(MSBuildEngine engine)
			{
				this.engine = engine;
				this.results = engine.currentResults;
			}
			
			void AppendText(string text)
			{
				engine.MessageView.AppendText(text + "\r\n");
			}
			
			internal void FlushCurrentError()
			{
				if (engine.currentErrorOrWarning != null) {
					AppendText(engine.currentErrorOrWarning.ToString());
					engine.currentErrorOrWarning = null;
				}
			}
			
			void OnBuildStarted(object sender, BuildStartedEventArgs e)
			{
				AppendText("${res:MainWindow.CompilerMessages.BuildStarted}");
			}
			
			void OnBuildFinished(object sender, BuildFinishedEventArgs e)
			{
				if (e.Succeeded) {
					AppendText("${res:MainWindow.CompilerMessages.BuildFinished}");
					StatusBarService.SetMessage("${res:MainWindow.CompilerMessages.BuildFinished}");
				} else {
					AppendText("${res:MainWindow.CompilerMessages.BuildFailed}");
					StatusBarService.SetMessage("${res:MainWindow.CompilerMessages.BuildFailed}");
				}
			}
			
			void OnProjectStarted(object sender, ProjectStartedEventArgs e)
			{
				engine.projectFiles.Push(e.ProjectFile);
				StatusBarService.SetMessage("${res:MainWindow.CompilerMessages.BuildVerb} " + Path.GetFileNameWithoutExtension(e.ProjectFile) + "...");
			}
			
			void OnProjectFinished(object sender, ProjectFinishedEventArgs e)
			{
				FlushCurrentError();
				engine.projectFiles.Pop();
				if (engine.projectFiles.Count > 0) {
					StatusBarService.SetMessage("${res:MainWindow.CompilerMessages.BuildVerb} " + Path.GetFileNameWithoutExtension(engine.CurrentProjectFile) + "...");
				}
			}
			
			string activeTaskName;
			
			void OnTaskStarted(object sender, TaskStartedEventArgs e)
			{
				activeTaskName = e.TaskName;
				if (CompileTaskNames.Contains(e.TaskName.ToLowerInvariant())) {
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
				} else if (FileUtility.IsValidFileName(file)) {
					bool isShortFileName = file == Path.GetFileNameWithoutExtension(file);
					if (engine.CurrentProjectFile != null) {
						file = Path.Combine(Path.GetDirectoryName(engine.CurrentProjectFile), file);
					}
					if (isShortFileName && !File.Exists(file)) {
						file = "";
					}
				}
				FlushCurrentError();
				BuildError error = new BuildError(file, lineNumber, columnNumber, code, message);
				error.IsWarning = isWarning;
				results.Errors.Add(error);
				engine.currentErrorOrWarning = error;
			}
			
			#region ILogger interface implementation
			LoggerVerbosity verbosity = LoggerVerbosity.Minimal;
			
			public LoggerVerbosity Verbosity {
				get {
					return verbosity;
				}
				set {
					verbosity = value;
				}
			}
			
			string parameters;
			
			public string Parameters {
				get {
					return parameters;
				}
				set {
					parameters = value;
				}
			}
			
			public void Initialize(IEventSource eventSource)
			{
				eventSource.BuildStarted    += new BuildStartedEventHandler(OnBuildStarted);
				eventSource.BuildFinished   += new BuildFinishedEventHandler(OnBuildFinished);
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
