// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.BuildWorker;
using ICSharpCode.SharpDevelop.Gui;
using Microsoft.Build.Construction;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using MSBuild = Microsoft.Build.Evaluation;

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
			{ "BuildingSolutionFile", "true" }
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
		}
		
		public static void StartBuild(IProject project, ThreadSafeServiceContainer serviceContainer, ProjectBuildOptions options, IBuildFeedbackSink feedbackSink, IEnumerable<string> additionalTargetFiles)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (serviceContainer == null)
				throw new ArgumentNullException("serviceContainer");
			if (options == null)
				throw new ArgumentNullException("options");
			if (feedbackSink == null)
				throw new ArgumentNullException("feedbackSink");
			if (additionalTargetFiles == null)
				throw new ArgumentNullException("additionalTargetFiles");
			
			MSBuildEngine engine = new MSBuildEngine(project, options, feedbackSink);
			engine.additionalTargetFiles = additionalTargetFiles;
			engine.serviceContainer = serviceContainer;
			engine.StartBuild();
		}
		
		IProject project;
		ProjectBuildOptions options;
		IBuildFeedbackSink feedbackSink;
		IEnumerable<string> additionalTargetFiles;
		ThreadSafeServiceContainer serviceContainer;
		
		private MSBuildEngine(IProject project, ProjectBuildOptions options, IBuildFeedbackSink feedbackSink)
		{
			this.project = project;
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
		
		List<string> interestingTasks = new List<string>();
		string temporaryFileName;
		
		/// <summary>
		/// The list of task names for which TaskStarted and TaskFinished events should be
		/// made available to loggers.
		/// Logger AddIns should add entries in their CreateLogger method.
		/// </summary>
		public ICollection<string> InterestingTasks {
			get { return interestingTasks; }
		}
		
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
			
			List<ILogger> loggers = new List<ILogger> {
				new SharpDevelopLogger(this),
				//new BuildLogFileLogger(project.FileName + ".log", LoggerVerbosity.Diagnostic)
			};
			foreach (IMSBuildAdditionalLogger loggerProvider in MSBuildEngine.AdditionalMSBuildLoggers) {
				loggers.Add(loggerProvider.CreateLogger(this));
			}
			WriteAdditionalTargetsToTempFile(globalProperties);
			
			BuildJob job = new BuildJob();
			job.ProjectFileName = project.FileName;
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
			
			if (project.MinimumSolutionVersion <= Solution.SolutionVersionVS2008) {
				BuildWorkerManager.MSBuild35.RunBuildJob(job, loggers, feedbackSink);
			} else {
				BuildWorkerManager.MSBuild40.RunBuildJob(job, loggers, feedbackSink);
			}
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
				if (project.MinimumSolutionVersion == Solution.SolutionVersionVS2005)
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
				// it's possible that MSBuild raises ProjectFinished without a matching
				// ProjectStarted - e.g. if an additional import is missing
				if (worker.projectFiles.Count > 0)
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
			
			public void Initialize(IEventSource eventSource)
			{
				eventSource.ProjectStarted  += OnProjectStarted;
				eventSource.ProjectFinished += OnProjectFinished;
				eventSource.TaskStarted     += OnTaskStarted;
				eventSource.TaskFinished    += OnTaskFinished;
				
				eventSource.ErrorRaised     += OnError;
				eventSource.WarningRaised   += OnWarning;
			}
			
			public void Shutdown()
			{
				FlushCurrentError();
				if (worker.temporaryFileName != null) {
					File.Delete(worker.temporaryFileName);
					worker.temporaryFileName = null;
				}
			}
			#endregion
		}
	}
}
