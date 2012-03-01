// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.BuildWorker;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Class responsible for building a project using MSBuild.
	/// Is called by MSBuildProject.
	/// </summary>
	public sealed class MSBuildEngine
	{
		const string CompileTaskNamesPath = "/SharpDevelop/MSBuildEngine/CompileTaskNames";
		const string AdditionalTargetFilesPath = "/SharpDevelop/MSBuildEngine/AdditionalTargetFiles";
		const string AdditionalLoggersPath = "/SharpDevelop/MSBuildEngine/AdditionalLoggers";
		const string LoggerFiltersPath = "/SharpDevelop/MSBuildEngine/LoggerFilters";
		internal const string AdditionalPropertiesPath = "/SharpDevelop/MSBuildEngine/AdditionalProperties";
		
		/// <summary>
		/// Gets a list of the task names that cause a "Compiling ..." log message.
		/// You can add items to this list by putting strings into
		/// "/SharpDevelop/MSBuildEngine/CompileTaskNames".
		/// </summary>
		public static readonly ICollection<string> CompileTaskNames;
		
		/// <summary>
		/// Gets a list where addins can add additional properties for use in MSBuild.
		/// </summary>
		/// <remarks>
		/// Please use the AddIn Tree path "/SharpDevelop/MSBuildEngine/AdditionalProperties"
		/// instead of this list.
		/// </remarks>
		public static readonly IDictionary<string, string> MSBuildProperties = new SortedList<string, string> {
			{ "SharpDevelopBinPath", SharpDevelopBinPath },
			// 'BuildingSolutionFile' tells MSBuild that we took care of building a project's dependencies
			// before trying to build the project itself. This speeds up compilation because it prevents MSBuild from
			// repeatedly looking if a project needs to be rebuilt.
			{ "BuildingSolutionFile", "true" },
			// BuildingSolutionFile does not work in MSBuild 4.0 anymore, but BuildingInsideVisualStudio
			// can be used to get the same effect.
			{ "BuildingInsideVisualStudio", "true" }
		};
		
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
		
		/// <summary>
		/// Gets a list of MSBuild logger filter.
		/// You can register your loggers by putting them into
		/// "/SharpDevelop/MSBuildEngine/LoggerFilters"
		/// </summary>
		public static readonly IList<IMSBuildLoggerFilter> MSBuildLoggerFilters;
		
		public static string SharpDevelopBinPath {
			get {
				return Path.GetDirectoryName(typeof(MSBuildEngine).Assembly.Location);
			}
		}
		
		static MSBuildEngine()
		{
			CompileTaskNames = new SortedSet<string>(
				AddInTree.BuildItems<string>(CompileTaskNamesPath, null, false),
				StringComparer.OrdinalIgnoreCase
			);
			AdditionalTargetFiles = AddInTree.BuildItems<string>(AdditionalTargetFilesPath, null, false);
			AdditionalMSBuildLoggers = AddInTree.BuildItems<IMSBuildAdditionalLogger>(AdditionalLoggersPath, null, false);
			MSBuildLoggerFilters = AddInTree.BuildItems<IMSBuildLoggerFilter>(LoggerFiltersPath, null, false);
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
			engine.additionalTargetFiles = additionalTargetFiles.ToList();
			if (project.MinimumSolutionVersion >= Solution.SolutionVersionVS2010) {
				engine.additionalTargetFiles.Add(Path.Combine(Path.GetDirectoryName(typeof(MSBuildEngine).Assembly.Location), "SharpDevelop.TargetingPack.targets"));
			}
			engine.StartBuild();
		}
		
		readonly string projectFileName;
		readonly int projectMinimumSolutionVersion;
		ProjectBuildOptions options;
		IBuildFeedbackSink feedbackSink;
		List<string> additionalTargetFiles;
		
		private MSBuildEngine(IProject project, ProjectBuildOptions options, IBuildFeedbackSink feedbackSink)
		{
			this.projectFileName = project.FileName;
			this.projectMinimumSolutionVersion = project.MinimumSolutionVersion;
			this.options = options;
			this.feedbackSink = feedbackSink;
		}
		
		const EventTypes ControllableEvents = EventTypes.Message | EventTypes.TargetStarted | EventTypes.TargetFinished
			| EventTypes.TaskStarted | EventTypes.TaskFinished | EventTypes.Unknown;
		
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
		
		/// <summary>
		/// Gets the name of the project file being compiled by this engine.
		/// </summary>
		public string ProjectFileName {
			get { return projectFileName; }
		}
		
		/// <summary>
		/// Gets the minimum solution version (VS version) required to open the project.
		/// </summary>
		public int ProjectMinimumSolutionVersion {
			get { return projectMinimumSolutionVersion; }
		}
		
		HashSet<string> interestingTasks = new HashSet<string>();
		string temporaryFileName;
		
		/// <summary>
		/// The list of task names for which TaskStarted and TaskFinished events should be
		/// made available to loggers.
		/// Logger AddIns should add entries in their CreateLogger method.
		/// </summary>
		public ICollection<string> InterestingTasks {
			get { return interestingTasks; }
		}
		
		readonly EventSource eventSource = new EventSource();
		List<ILogger> loggers = new List<ILogger>();
		IMSBuildChainedLoggerFilter loggerChain;
		
		void StartBuild()
		{
			Dictionary<string, string> globalProperties = new Dictionary<string, string>();
			MSBuildBasedProject.InitializeMSBuildProjectProperties(globalProperties);
			
			foreach (KeyValuePair<string, string> pair in options.Properties) {
				LoggingService.Debug("Setting property " + pair.Key + " to '" + pair.Value + "'");
				globalProperties[pair.Key] = pair.Value;
			}
			globalProperties["Configuration"] = options.Configuration;
			if (options.Platform == "Any CPU")
				globalProperties["Platform"] = "AnyCPU";
			else
				globalProperties["Platform"] = options.Platform;
			
			InterestingTasks.AddRange(MSBuildEngine.CompileTaskNames);
			
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
			foreach (IMSBuildAdditionalLogger loggerProvider in MSBuildEngine.AdditionalMSBuildLoggers) {
				loggers.Add(loggerProvider.CreateLogger(this));
			}
			
			loggerChain = new EndOfChain(this);
			foreach (IMSBuildLoggerFilter loggerFilter in MSBuildEngine.MSBuildLoggerFilters) {
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
			
			if (projectMinimumSolutionVersion <= Solution.SolutionVersionVS2008) {
				if (DotnetDetection.IsDotnet35SP1Installed()) {
					BuildWorkerManager.MSBuild35.RunBuildJob(job, loggerChain, OnDone, feedbackSink.ProgressMonitor.CancellationToken);
				} else {
					loggerChain.HandleError(new BuildError(job.ProjectFileName, ".NET 3.5 SP1 is required to build this project."));
					OnDone(false);
				}
			} else {
				BuildWorkerManager.MSBuild40.RunBuildJob(job, loggerChain, OnDone, feedbackSink.ProgressMonitor.CancellationToken);
			}
		}
		
		void OnDone(bool success)
		{
			foreach (ILogger logger in loggers) {
				logger.Shutdown();
			}
			feedbackSink.Done(success);
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
				if (projectMinimumSolutionVersion == Solution.SolutionVersionVS2005)
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
			readonly MSBuildEngine engine;
			
			public EndOfChain(MSBuildEngine engine)
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
			MSBuildEngine engine;
			
			public SharpDevelopLogger(MSBuildEngine engine)
			{
				this.engine = engine;
			}
			
			string activeTaskName;
			
			void OnTaskStarted(object sender, TaskStartedEventArgs e)
			{
				activeTaskName = e.TaskName;
				if (MSBuildEngine.CompileTaskNames.Contains(e.TaskName.ToLowerInvariant())) {
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
