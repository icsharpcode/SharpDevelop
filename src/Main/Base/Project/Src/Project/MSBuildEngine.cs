// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
		
		static bool IsRunning = false;
		
		public CompilerResults Run(string buildFile, string[] targets)
		{
			CompilerResults results = new CompilerResults(null);
			if (IsRunning) {
				results.Errors.Add(new CompilerError(null, 0, 0, null, "MsBuild is already running!"));
				return results;
			}
			IsRunning = true;
			try {
				Thread thread = new Thread(new ThreadStarter(results, buildFile, targets, this).Run);
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
				while (!thread.Join(10)) {
					Application.DoEvents();
					Application.DoEvents();
				}
				return results;
			} finally {
				IsRunning = false;
			}
		}
		
		class ThreadStarter
		{
			CompilerResults results;
			string buildFile;
			string[] targets;
			MSBuildEngine engine;
			
			public ThreadStarter(CompilerResults results, string buildFile, string[] targets, MSBuildEngine engine)
			{
				this.results = results;
				this.buildFile = buildFile;
				this.targets = targets;
				this.engine = engine;
			}
			
			[STAThread]
			public void Run()
			{
				LoggingService.Debug("Run MSBuild on " + buildFile);
				
				Engine engine = new Engine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory());
				foreach (KeyValuePair<string, string> entry in MSBuildProperties) {
					engine.GlobalProperties.SetProperty(entry.Key, entry.Value);
				}
				
				SharpDevelopLogger logger = new SharpDevelopLogger(this.engine, results);
				engine.RegisterLogger(logger);
				// IMPORTANT: engine.GlobalProperties must be passed here.
				// The properties must be available for both the scanning and building steps
				engine.BuildProjectFile(buildFile, targets, engine.GlobalProperties);
				
				LoggingService.Debug("MSBuild finished");
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
