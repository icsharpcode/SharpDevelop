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
	/// <summary>
	/// Class responsible for building a project using MSBuild.
	/// Is called by MSBuildProject.
	/// </summary>
	public class MSBuildEngine
	{
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
		
		public CompilerResults Run(string buildFile)
		{
			return Run(buildFile, null);
		}
		
		public CompilerResults Run(string buildFile, string[] targets)
		{
			CompilerResults results = new CompilerResults(null);
			Engine engine = new Engine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory());
			SharpDevelopLogger logger = new SharpDevelopLogger(this, results);
			engine.RegisterLogger(logger);
			engine.BuildProjectFile(buildFile, targets, null, null);
			logger.FlushText();
			return results;
		}
		
		class SharpDevelopLogger : ILogger
		{
			MSBuildEngine engine;
			CompilerResults results;
			Thread mainThread;
			
			public SharpDevelopLogger(MSBuildEngine engine, CompilerResults results)
			{
				mainThread = Thread.CurrentThread;
				this.engine = engine;
				this.results = results;
			}
			
			StringBuilder textToWrite = new StringBuilder();
			
			void AppendText(string text)
			{
				lock (textToWrite) {
					textToWrite.AppendLine(text);
					if (Thread.CurrentThread == mainThread) {
						FlushText();
					}
				}
			}
			
			public void FlushText()
			{
				if (engine.MessageView != null)
					engine.MessageView.AppendText(textToWrite.ToString());
				textToWrite.Length = 0;
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
				Application.DoEvents();
			}
			
			void OnProjectFinished(object sender, ProjectFinishedEventArgs e)
			{
				projectFiles.Pop();
				if (projectFiles.Count > 0) {
					StatusBarService.SetMessage("Building " + Path.GetFileNameWithoutExtension(projectFiles.Peek()) + "...");
					Application.DoEvents();
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
			
			void OnTaskStarted(object sender, TaskStartedEventArgs e)
			{
				// TODO: Give addins ability to extend the logger
				switch (e.TaskName.ToLower()) {
					case "csc":
					case "vbc":
						AppendText("Compiling " + Path.GetFileNameWithoutExtension(e.ProjectFile));
						break;
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
				CompilerError error = new CompilerError(file, lineNumber, columnNumber, code, message);
				error.IsWarning = isWarning;
				AppendText(error.ToString());
				if (projectFiles.Count > 0)
					error.FileName = Path.Combine(Path.GetDirectoryName(projectFiles.Peek()), file);
				results.Errors.Add(error);
			}
			
			void OnMessage(object sender, BuildMessageEventArgs e)
			{
				//AppendText(e.Message);
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
