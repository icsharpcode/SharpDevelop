// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.BuildEngine;
using ICSharpCode.SharpDevelop.Gui;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public sealed class MSBuildEngineWorker
	{
		MSBuildEngine parentEngine;
		MSBuildEngine.BuildRun buildRun;
		Engine engine;
		SharpDevelopLogger logger;
		
		internal MSBuildEngineWorker(MSBuildEngine parentEngine, MSBuildEngine.BuildRun buildRun)
		{
			this.parentEngine = parentEngine;
			this.buildRun = buildRun;
			engine = buildRun.CreateEngine();
			
			logger = new SharpDevelopLogger(this);
			engine.RegisterLogger(logger);
			foreach (IMSBuildAdditionalLogger loggerProvider in MSBuildEngine.AdditionalMSBuildLoggers) {
				engine.RegisterLogger(loggerProvider.CreateLogger(this));
			}
		}
		
		internal bool Build(MSBuildEngine.ProjectToBuild ptb)
		{
			LoggingService.Debug("Run MSBuild on " + ptb.file);
			
			if (!string.IsNullOrEmpty(ptb.configuration)) {
				engine.GlobalProperties.SetProperty("Configuration", ptb.configuration);
			}
			if (!string.IsNullOrEmpty(ptb.platform)) {
				engine.GlobalProperties.SetProperty("Platform", ptb.platform);
			}
			
			Microsoft.Build.BuildEngine.Project project = buildRun.LoadProject(engine, ptb.file);
			if (project == null) {
				LoggingService.Debug("Error loading " + ptb.file);
				return false;
			}
			foreach (string additionalTargetFile in MSBuildEngine.AdditionalTargetFiles) {
				project.AddNewImport(additionalTargetFile, null);
			}
			
			bool success;
			if (string.IsNullOrEmpty(ptb.targets)) {
				success = engine.BuildProject(project);
			} else {
				success = engine.BuildProject(project, ptb.targets.Split(';'));
			}
			
			logger.FlushCurrentError();
			ReleaseOutput();
			
			LoggingService.Debug("MSBuild on " + ptb.file + " finished " + (success ? "successfully" : "with error"));
			return success;
		}
		
		bool outputAcquired;
		StringBuilder cachedOutput;
		
		public void OutputText(string text)
		{
			if (outputAcquired == false && cachedOutput == null) {
				outputAcquired = buildRun.TryAquireOutputLock();
				if (!outputAcquired) {
					cachedOutput = new StringBuilder();
				}
			}
			if (outputAcquired) {
				parentEngine.MessageView.AppendText(text);
			} else {
				cachedOutput.Append(text);
			}
		}
		
		void ReleaseOutput()
		{
			if (cachedOutput != null) {
				buildRun.EnqueueTextForAppendWhenOutputLockIsReleased(cachedOutput.ToString());
				cachedOutput = null;
			}
			if (outputAcquired) {
				buildRun.ReleaseOutputLock();
			}
		}
		
		#region CurrentBuild properties
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
		#endregion
		
		class SharpDevelopLogger : ILogger
		{
			MSBuildEngineWorker worker;
			BuildResults results;
			
			public SharpDevelopLogger(MSBuildEngineWorker worker)
			{
				this.worker = worker;
				this.results = worker.buildRun.currentResults;
			}
			
			void AppendText(string text)
			{
				worker.OutputText(text + "\r\n");
			}
			
			internal void FlushCurrentError()
			{
				if (worker.currentErrorOrWarning != null) {
					AppendText(worker.currentErrorOrWarning.ToString());
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
				} else if (FileUtility.IsValidFileName(file)) {
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
				results.Add(error);
				worker.currentErrorOrWarning = error;
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
