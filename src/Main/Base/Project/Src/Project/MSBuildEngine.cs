// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using Microsoft.Build.Framework;
using Microsoft.Build.BuildEngine;

namespace ICSharpCode.SharpDevelop.Project
{
	public delegate void MSBuildEngineCallback(CompilerResults results);
	
	/// <summary>
	/// Class responsible for building a project using MSBuild.
	/// Is called by MSBuildProject.
	/// </summary>
	public class MSBuildEngine
	{
		/// <summary>
		/// Gets a list of the task names that cause a "Compiling ..." log message.
		/// The contents of the list can be changed by addins.
		/// All names must be in lower case!
		/// </summary>
		public static readonly List<string> CompileTaskNames;
		
		/// <summary>
		/// Gets a list where addins can add additional properties for use in MsBuild.
		/// </summary>
		public static readonly SortedList<string, string> MSBuildProperties;
		
		static MSBuildEngine()
		{
			CompileTaskNames = new List<string>(new string[] {"csc", "vbc", "ilasm"});
			MSBuildProperties = new SortedList<string, string>();
			MSBuildProperties.Add("SharpDevelopBinPath", Path.GetDirectoryName(typeof(MSBuildEngine).Assembly.Location));
		}

		#region relocated from ICSharpCode.SharpDevelop.Project.Commands.Build in BuildCommands.cs
		public static int LastErrorCount;
		public static int LastWarningCount;
		
		public static void ShowResults(CompilerResults results)
		{
			if (results != null) {
				LastErrorCount = 0;
				LastWarningCount = 0;
				TaskService.InUpdate = true;
				foreach (CompilerError error in results.Errors) {
					TaskService.Add(new Task(error));
					if (error.IsWarning)
						LastWarningCount++;
					else
						LastErrorCount++;
				}
				TaskService.InUpdate = false;
				if (results.Errors.Count > 0) {
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
		
		public void Run(string buildFile, MSBuildEngineCallback callback)
		{
			Run(buildFile, null, callback);
		}
		
		volatile static bool isRunning = false;
		
		public void Run(string buildFile, string[] targets, MSBuildEngineCallback callback)
		{
			if (isRunning) {
				CompilerResults results = new CompilerResults(null);
				results.Errors.Add(new CompilerError(null, 0, 0, null, "MSBuild is already running!"));
				callback(results);
			} else {
				isRunning = true;
				Thread thread = new Thread(new ThreadStarter(buildFile, targets, this, callback).Run);
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
			}
		}
		
		class ThreadStarter
		{
			CompilerResults results = new CompilerResults(null);
			string buildFile;
			string[] targets;
			MSBuildEngine engine;
			MSBuildEngineCallback callback;
			
			public ThreadStarter(string buildFile, string[] targets, MSBuildEngine engine, MSBuildEngineCallback callback)
			{
				this.buildFile = buildFile;
				this.targets = targets;
				this.engine = engine;
				this.callback = callback;
			}
			
			[STAThread]
			public void Run()
			{
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
				
				SharpDevelopLogger logger = new SharpDevelopLogger(this.engine, results);
				engine.RegisterLogger(logger);
				
				Microsoft.Build.BuildEngine.Project project = engine.CreateNewProject();
				project.Load(buildFile);
				engine.BuildProject(project, targets);
				
				LoggingService.Debug("MSBuild finished");
				MSBuildEngine.isRunning = false;
				if (callback != null) {
					WorkbenchSingleton.MainForm.BeginInvoke(callback, results);
				}
			}
		}
		
		class SharpDevelopLogger : ILogger
		{
			MSBuildEngine engine;
			CompilerResults results;
			
			public SharpDevelopLogger(MSBuildEngine engine, CompilerResults results)
			{
				this.engine = engine;
				this.results = results;
			}
			
			void AppendText(string text)
			{
				engine.MessageView.AppendText(text + "\r\n");
			}
			
			void OnBuildStarted(object sender, BuildStartedEventArgs e)
			{
				AppendText("Build started.");
			}
			
			void OnBuildFinished(object sender, BuildFinishedEventArgs e)
			{
				if (e.Succeeded) {
					AppendText("Build finished successfully.");
					StatusBarService.SetMessage("Build finished successfully.");
				} else {
					AppendText("Build failed.");
					StatusBarService.SetMessage("Build failed.");
				}
			}
			
			Stack<string> projectFiles = new Stack<string>();
			
			void OnProjectStarted(object sender, ProjectStartedEventArgs e)
			{
				projectFiles.Push(e.ProjectFile);
				StatusBarService.SetMessage("Building " + Path.GetFileNameWithoutExtension(e.ProjectFile) + "...");
			}
			
			void OnProjectFinished(object sender, ProjectFinishedEventArgs e)
			{
				projectFiles.Pop();
				if (projectFiles.Count > 0) {
					StatusBarService.SetMessage("Building " + Path.GetFileNameWithoutExtension(projectFiles.Peek()) + "...");
				}
			}
			
			void OnTargetStarted(object sender, TargetStartedEventArgs e)
			{
				// do not display
			}
			
			void OnTargetFinished(object sender, TargetFinishedEventArgs e)
			{
				// do not display
			}
			
			string activeTaskName;
			
			void OnTaskStarted(object sender, TaskStartedEventArgs e)
			{
				activeTaskName = e.TaskName;
				if (CompileTaskNames.Contains(e.TaskName.ToLowerInvariant())) {
					AppendText("Compiling " + Path.GetFileNameWithoutExtension(e.ProjectFile));
				}
			}
			
			void OnTaskFinished(object sender, TaskFinishedEventArgs e)
			{
				// do not display
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
				} else {
					if (projectFiles.Count > 0) {
						file = Path.Combine(Path.GetDirectoryName(projectFiles.Peek()), file);
					}
				}
				CompilerError error = new CompilerError(file, lineNumber, columnNumber, code, message);
				error.IsWarning = isWarning;
				AppendText(error.ToString());
				results.Errors.Add(error);
			}
			
			void OnMessage(object sender, BuildMessageEventArgs e)
			{
				//if (e.Importance == MessageImportance.High)
				//	AppendText(e.Message);
			}
			
			void OnCustomEvent(object sender, CustomBuildEventArgs e)
			{
				//AppendText(e.Message);
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
				eventSource.TargetStarted   += new TargetStartedEventHandler(OnTargetStarted);
				eventSource.TargetFinished  += new TargetFinishedEventHandler(OnTargetFinished);
				eventSource.TaskStarted     += new TaskStartedEventHandler(OnTaskStarted);
				eventSource.TaskFinished    += new TaskFinishedEventHandler(OnTaskFinished);
				
				eventSource.ErrorRaised     += new BuildErrorEventHandler(OnError);
				eventSource.WarningRaised   += new BuildWarningEventHandler(OnWarning);
				eventSource.MessageRaised   += new BuildMessageEventHandler(OnMessage);
				eventSource.CustomEventRaised += new CustomBuildEventHandler(OnCustomEvent);
			}
			
			public void Shutdown()
			{
				
			}
			#endregion
		}
	}
}
