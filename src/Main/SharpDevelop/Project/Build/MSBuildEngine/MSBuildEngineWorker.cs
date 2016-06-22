// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.BuildWorker;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.Project
{
	class MSBuildEngineWorker : IMSBuildLoggerContext
	{
		readonly IMSBuildEngine parentBuildEngine;
		readonly FileName projectFileName;
		readonly IProject project;
		readonly SolutionFormatVersion projectMinimumSolutionVersion;
		ProjectBuildOptions options;
		IBuildFeedbackSink feedbackSink;
		List<string> additionalTargetFiles;
		
		internal MSBuildEngineWorker(IMSBuildEngine parentBuildEngine, IProject project, ProjectBuildOptions options, IBuildFeedbackSink feedbackSink, List<string> additionalTargetFiles)
		{
			this.parentBuildEngine = parentBuildEngine;
			this.project = project;
			this.projectFileName = project.FileName;
			this.projectMinimumSolutionVersion = project.MinimumSolutionVersion;
			this.options = options;
			this.feedbackSink = feedbackSink;
			this.additionalTargetFiles = additionalTargetFiles;
		}
		
		const EventTypes ControllableEvents = EventTypes.Message | EventTypes.TargetStarted | EventTypes.TargetFinished
			| EventTypes.TaskStarted | EventTypes.TaskFinished | EventTypes.Unknown;
		
		public IProject Project {
			get { return project; }
		}
		public FileName ProjectFileName {
			get { return projectFileName; }
		}
		
		public bool ReportMessageEvents { get; set; }
		public bool ReportTargetStartedEvents { get; set; }
		public bool ReportTargetFinishedEvents { get; set; }
		public bool ReportAllTaskStartedEvents { get; set; }
		public bool ReportAllTaskFinishedEvents { get; set; }
		public bool ReportUnknownEvents { get; set; }
		
		HashSet<string> interestingTasks = new HashSet<string>();
		string temporaryFileName;
		
		public ISet<string> InterestingTasks {
			get { return interestingTasks; }
		}
		
		readonly EventSource eventSource = new EventSource();
		List<ILogger> loggers = new List<ILogger>();
		IMSBuildChainedLoggerFilter loggerChain;
		
		internal Task<bool> RunBuildAsync(CancellationToken cancellationToken)
		{
			Dictionary<string, string> globalProperties = new Dictionary<string, string>();
			globalProperties.AddRange(SD.MSBuildEngine.GlobalBuildProperties);
			
			foreach (KeyValuePair<string, string> pair in options.Properties) {
				LoggingService.Debug("Setting property " + pair.Key + " to '" + pair.Value + "'");
				globalProperties[pair.Key] = pair.Value;
			}
			globalProperties["Configuration"] = options.Configuration;
			if (options.Platform == "Any CPU")
				globalProperties["Platform"] = "AnyCPU";
			else
				globalProperties["Platform"] = options.Platform;
			
			InterestingTasks.AddRange(parentBuildEngine.CompileTaskNames);
			
			loggers.Add(new SharpDevelopLogger(this));
			if (options.BuildOutputVerbosity == BuildOutputVerbosity.Diagnostic) {
				this.ReportMessageEvents = true;
				this.ReportAllTaskFinishedEvents = true;
				this.ReportAllTaskStartedEvents = true;
				this.ReportTargetFinishedEvents = true;
				this.ReportTargetStartedEvents = true;
				this.ReportUnknownEvents = true;
				loggers.Add(new SDConsoleLogger(feedbackSink, LoggerVerbosity.Diagnostic));
				globalProperties["MSBuildTargetsVerbose"] = "true";
			}
			//loggers.Add(new BuildLogFileLogger(project.FileName + ".log", LoggerVerbosity.Diagnostic));
			foreach (IMSBuildAdditionalLogger loggerProvider in parentBuildEngine.AdditionalMSBuildLoggers) {
				loggers.Add(loggerProvider.CreateLogger(this));
			}
			
			loggerChain = new EndOfChain(this);
			foreach (IMSBuildLoggerFilter loggerFilter in parentBuildEngine.MSBuildLoggerFilters) {
				loggerChain = loggerFilter.CreateFilter(this, loggerChain) ?? loggerChain;
			}
			
			WriteAdditionalTargetsToTempFile(globalProperties);
			
			BuildJob job = new BuildJob();
			job.ProjectFileName = projectFileName;
			job.Target = options.Target.TargetName;
			
			// First remove the flags for the controllable events.
			job.EventMask = EventTypes.All & ~ControllableEvents;
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
			foreach (var pair in globalProperties) {
				job.Properties.Add(pair.Key, pair.Value);
			}
			
			foreach (ILogger logger in loggers) {
				logger.Initialize(eventSource);
			}
			
			tcs = new TaskCompletionSource<bool>();
			if (projectMinimumSolutionVersion <= SolutionFormatVersion.VS2008) {
				if (DotnetDetection.IsDotnet35SP1Installed()) {
					BuildWorkerManager.MSBuild35.RunBuildJob(job, loggerChain, OnDone, cancellationToken);
				} else {
					loggerChain.HandleError(new BuildError(job.ProjectFileName, ".NET 3.5 SP1 is required to build this project."));
					tcs.SetResult(false);
				}
			} else {
				if (DotnetDetection.IsBuildTools2015Installed()) {
					BuildWorkerManager.MSBuild140.RunBuildJob(job, loggerChain, OnDone, cancellationToken);
				} else if (DotnetDetection.IsBuildTools2013Installed()) {
					BuildWorkerManager.MSBuild120.RunBuildJob(job, loggerChain, OnDone, cancellationToken);
				} else {
					BuildWorkerManager.MSBuild40.RunBuildJob(job, loggerChain, OnDone, cancellationToken);
				}
			}
			return tcs.Task;
		}
		
		TaskCompletionSource<bool> tcs;
		
		void OnDone(bool success)
		{
			foreach (ILogger logger in loggers) {
				logger.Shutdown();
			}
			tcs.SetResult(success);
		}
		
		void WriteAdditionalTargetsToTempFile(Dictionary<string, string> globalProperties)
		{
			// Using projects with in-memory modifications doesn't work with parallel build.
			// As a work-around, we'll write our modifications to a file and force MSBuild to include that file using a custom property.
			temporaryFileName = Path.GetTempFileName();
			using (XmlWriter w = new XmlTextWriter(temporaryFileName, Encoding.UTF8)) {
				const string xmlNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
				w.WriteStartElement("Project", xmlNamespace);
				
				foreach (string import in additionalTargetFiles) {
					w.WriteStartElement("Import", xmlNamespace);
					w.WriteAttributeString("Project", MSBuildInternals.Escape(import));
					w.WriteEndElement();
				}
				
				if (globalProperties.ContainsKey("BuildingInsideVisualStudio")) {
					// When we set BuildingInsideVisualStudio, MSBuild skips its own change detection
					// because in Visual Studio, the host compiler does the change detection.
					// We override the target '_ComputeNonExistentFileProperty' which is responsible
					// for recompiling each time - our _ComputeNonExistentFileProperty does nothing,
					// which re-enables the MSBuild's usual change detection.
					w.WriteStartElement("Target", xmlNamespace);
					w.WriteAttributeString("Name", "_ComputeNonExistentFileProperty");
					w.WriteEndElement();
				}
				
				// 'MsTestToolsTargets' is preferred because it's at the end of the MSBuild 3.5 and 4.0 target file,
				// but on MSBuild 2.0 we need to fall back to 'CodeAnalysisTargets'.
				string hijackedProperty = "MsTestToolsTargets";
				if (projectMinimumSolutionVersion == SolutionFormatVersion.VS2005)
					hijackedProperty = "CodeAnalysisTargets";
				
				// because we'll replace the hijackedProperty, manually write the corresponding include
				if (globalProperties.ContainsKey(hijackedProperty)) {
					// global properties passed to MSBuild are not be evaluated (and are not escaped),
					// so we need to escape them for writing them into an MSBuild file
					w.WriteStartElement("Import", xmlNamespace);
					w.WriteAttributeString("Project", MSBuildInternals.Escape(globalProperties[hijackedProperty]));
					w.WriteEndElement();
				}
				w.WriteEndElement();
				
				// inject our imports at the end of 'Microsoft.Common.Targets' by replacing the hijackedProperty.
				globalProperties[hijackedProperty] = temporaryFileName;
			}
			
			#if DEBUG
			LoggingService.Debug(File.ReadAllText(temporaryFileName));
			#endif
		}
		
		public void OutputTextLine(string message)
		{
			feedbackSink.ReportMessage(message);
		}
		
		public void ReportError(BuildError error)
		{
			feedbackSink.ReportError(error);
		}
		
		sealed class EndOfChain : IMSBuildChainedLoggerFilter
		{
			readonly MSBuildEngineWorker engine;
			
			public EndOfChain(MSBuildEngineWorker engine)
			{
				this.engine = engine;
			}
			
			public void HandleError(BuildError error)
			{
				engine.ReportError(error);
			}
			
			public void HandleBuildEvent(Microsoft.Build.Framework.BuildEventArgs e)
			{
				engine.eventSource.ForwardEvent(e);
			}
		}
		
		sealed class SharpDevelopLogger : ILogger
		{
			readonly MSBuildEngineWorker engine;
			
			public SharpDevelopLogger(MSBuildEngineWorker engine)
			{
				this.engine = engine;
			}
			
			string activeTaskName;
			
			void OnTaskStarted(object sender, TaskStartedEventArgs e)
			{
				activeTaskName = e.TaskName;
				if (engine.parentBuildEngine.CompileTaskNames.Contains(e.TaskName)) {
					engine.OutputTextLine(StringParser.Parse("${res:MainWindow.CompilerMessages.CompileVerb} " + Path.GetFileNameWithoutExtension(e.ProjectFile)));
				}
			}
			
			void OnError(object sender, BuildErrorEventArgs e)
			{
				AppendError(e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message, e.ProjectFile, e.Subcategory, e.HelpKeyword, false);
			}
			
			void OnWarning(object sender, BuildWarningEventArgs e)
			{
				AppendError(e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message, e.ProjectFile, e.Subcategory, e.HelpKeyword, true);
			}
			
			void AppendError(string file, int lineNumber, int columnNumber, string code, string message, string projectFile, string subcategory, string helpKeyword, bool isWarning)
			{
				if (string.Equals(file, activeTaskName, StringComparison.OrdinalIgnoreCase)) {
					file = "";
				} else if (FileUtility.IsValidPath(file)) {
					bool isShortFileName = file == Path.GetFileNameWithoutExtension(file);
					if (engine.ProjectFileName != null) {
						file = Path.Combine(Path.GetDirectoryName(engine.ProjectFileName), file);
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
				BuildError error = new BuildError(file, lineNumber, columnNumber, code, message);
				error.IsWarning = isWarning;
				error.Subcategory = subcategory;
				error.HelpKeyword = helpKeyword;
				error.ParentProject = engine.Project;
				engine.loggerChain.HandleError(error);
			}
			
			#region ILogger interface implementation
			public LoggerVerbosity Verbosity { get; set; }
			public string Parameters { get; set; }
			
			public void Initialize(IEventSource eventSource)
			{
				eventSource.TaskStarted     += OnTaskStarted;
				
				eventSource.ErrorRaised     += OnError;
				eventSource.WarningRaised   += OnWarning;
			}
			
			public void Shutdown()
			{
				if (engine.temporaryFileName != null) {
					File.Delete(engine.temporaryFileName);
					engine.temporaryFileName = null;
				}
			}
			#endregion
		}
	}
}
