// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

// activate this define to see the build worker window
//#define WORKERDEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;

using ICSharpCode.SharpDevelop.BuildWorker.Interprocess;
using Microsoft.Build.BuildEngine;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.BuildWorker
{
	class Program
	{
		static HostProcess host;
		BuildJob currentJob;
		bool requestCancellation;
		
		[STAThread]
		internal static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_CurrentDomain_UnhandledException);
			
			if (args.Length == 3 && args[0] == "worker") {
				try {
					host = new HostProcess(new Program());
					host.WorkerProcessMain(args[1], args[2]);
				} catch (Exception ex) {
					ShowMessageBox(ex.ToString());
				}
			} else {
				Console.WriteLine("ICSharpCode.SharpDevelop.BuildWorker.exe is used to compile " +
				                  "MSBuild projects inside SharpDevelop.");
				Console.WriteLine("If you want to compile projects on the command line, use " +
				                  "MSBuild.exe (part of the .NET Framework)");
			}
		}
		
		static void AppDomain_CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			ShowMessageBox(e.ExceptionObject.ToString());
		}
		
		#if RELEASE && WORKERDEBUG
		#error WORKERDEBUG must not be defined if RELEASE is defined
		#endif
		
		internal static ProcessStartInfo CreateStartInfo()
		{
			ProcessStartInfo info = new ProcessStartInfo(typeof(Program).Assembly.Location);
			info.WorkingDirectory = Path.GetDirectoryName(info.FileName);
			info.Arguments = "worker";
			info.UseShellExecute = false;
			#if RELEASE || !WORKERDEBUG
			info.CreateNoWindow = true;
			#endif
			return info;
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
		internal static void ShowMessageBox(string text)
		{
			System.Windows.Forms.MessageBox.Show(text, "SharpDevelop Build Worker Process");
		}
		
		[Conditional("DEBUG")]
		internal static void Log(string text)
		{
			Debug.WriteLine(text);
			#if WORKERDEBUG
			DateTime now = DateTime.Now;
			Console.WriteLine(now.ToString() + "," + now.Millisecond.ToString("d3") + " " + text);
			#endif
		}
		
		// Called with CallMethodOnWorker
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public void StartBuild(BuildJob job)
		{
			if (job == null)
				throw new ArgumentNullException("job");
			lock (this) {
				if (currentJob != null)
					throw new InvalidOperationException("Already running a job");
				currentJob = job;
				requestCancellation = false;
			}
			#if WORKERDEBUG
			Console.Title = "BuildWorker - " + Path.GetFileName(job.ProjectFileName);
			#endif
			Program.Log("Got job:");
			Program.Log(job.ToString());
			Program.Log("Start build thread");
			Thread thread = new Thread(RunThread);
			thread.Name = "Build thread";
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}
		
		// Called with CallMethodOnWorker
		//[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		// TODO: make use of CancelBuild
		public void CancelBuild()
		{
			lock (this) {
				requestCancellation = true;
			}
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		void RunThread()
		{
			Program.Log("In build thread");
			bool success = false;
			try {
				success = DoBuild();
			} catch (Exception ex) {
				host.CallMethodOnHost("ReportException", ex.ToString());
			} finally {
				Program.Log("BuildDone");
				
				#if WORKERDEBUG
				Console.Title = "BuildWorker - no job";
				DisplayEventCounts();
				#endif
				
				lock (this) {
					currentJob = null;
				}
				// in the moment we call BuildDone, we can get the next job
				host.CallMethodOnHost("BuildDone", success);
			}
		}
		
		Engine CreateEngine()
		{
			Engine engine = new Engine(ToolsetDefinitionLocations.Registry
			                           | ToolsetDefinitionLocations.ConfigurationFile);
			
			engine.RegisterLogger(new ForwardingLogger(this));
			//engine.RegisterLogger(new ConsoleLogger(LoggerVerbosity.Diagnostic));
			
			return engine;
		}
		
		EventSource hostEventSource;
		
		internal void BuildInProcess(BuildSettings settings, BuildJob job)
		{
			lock (this) {
				if (currentJob != null)
					throw new InvalidOperationException("Already running a job");
				currentJob = job;
				requestCancellation = false;
				hostEventSource = new EventSource();
			}
			job.CancelCallback = delegate {
				lock (this) {
					if (currentJob == job) {
						requestCancellation = true;
					}
				}
			};
			bool success = false;
			try {
				foreach (ILogger logger in settings.Logger) {
					logger.Initialize(hostEventSource);
				}
				success = DoBuild();
				foreach (ILogger logger in settings.Logger) {
					logger.Shutdown();
				}
			} finally {
				lock (this) {
					currentJob = null;
				}
				if (settings.BuildDoneCallback != null)
					settings.BuildDoneCallback(success);
			}
		}
		
		Engine engine;
		Dictionary<string, string> lastJobProperties;
		
		// Fix for SD2-1533 - Project configurations get confused
		// Whenever the global properties change, we have to create a new Engine
		// to ensure MSBuild doesn't cache old paths
		bool GlobalPropertiesChanged(Dictionary<string, string> newJobProperties)
		{
			Debug.Assert(newJobProperties != null);
			if (lastJobProperties == null || lastJobProperties.Count != newJobProperties.Count) {
				Log("Recreating engine: Number of build properties changed");
				return true;
			}
			foreach (KeyValuePair<string, string> pair in lastJobProperties) {
				string val;
				if (!newJobProperties.TryGetValue(pair.Key, out val)) {
					Log("Recreating engine: Build property removed: " + pair.Key);
					return true;
				}
				if (val != pair.Value) {
					Log("Recreating engine: Build property changed: " + pair.Key);
					return true;
				}
			}
			return false;
		}
		
		bool DoBuild()
		{
			if (currentJob.IntPtrSize != IntPtr.Size)
				throw new ApplicationException("Incompatible IntPtr.Size between host and worker");
			
			if (engine == null || GlobalPropertiesChanged(currentJob.Properties)) {
				engine = CreateEngine();
				lastJobProperties = currentJob.Properties;
				engine.GlobalProperties.Clear();
				foreach (KeyValuePair<string, string> pair in currentJob.Properties) {
					engine.GlobalProperties.SetProperty(pair.Key, pair.Value);
				}
			}
			
			Log("Loading " + currentJob.ProjectFileName);
			Project project = LoadProject(engine, currentJob.ProjectFileName);
			if (project == null)
				return false;
			
			if (string.IsNullOrEmpty(currentJob.Target)) {
				Log("Building default target in " + currentJob.ProjectFileName);
				return engine.BuildProject(project);
			} else {
				Log("Building target '" + currentJob.Target + "' in " + currentJob.ProjectFileName);
				return engine.BuildProject(project, currentJob.Target.Split(';'));
			}
		}
		
		Project LoadProject(Engine engine, string fileName)
		{
			Project project = engine.CreateNewProject();
			try {
				project.Load(fileName);
				
				/* No longer necessary as we stopped using BuildingInsideVisualStudio
				// When we set BuildingInsideVisualStudio, MSBuild tries to build all projects
				// every time because in Visual Studio, the host compiler does the change detection
				// We override the property '_ComputeNonExistentFileProperty' which is responsible
				// for recompiling each time - our _ComputeNonExistentFileProperty does nothing,
				// which re-enables the MSBuild's usual change detection
				project.Targets.AddNewTarget("_ComputeNonExistentFileProperty");
				*/
				
				foreach (string additionalImport in currentJob.AdditionalImports) {
					project.AddNewImport(additionalImport, null);
				}
				
				return project;
			} catch (ArgumentException ex) {
				ReportError(ex.Message);
			} catch (InvalidProjectFileException ex) {
				ReportError(new BuildErrorEventArgs(ex.ErrorSubcategory, ex.ErrorCode, ex.ProjectFile,
				                                    ex.LineNumber, ex.ColumnNumber, ex.EndLineNumber, ex.EndColumnNumber,
				                                    ex.BaseMessage, ex.HelpKeyword, ex.Source));
			}
			return null;
		}
		
		void ReportError(string message)
		{
			ReportError(new BuildErrorEventArgs(null, null, null, -1, -1, -1, -1,
			                                    message, null, "SharpDevelopBuildWorker"));
		}
		
		void ReportError(BuildErrorEventArgs e)
		{
			HostReportEvent(e);
		}
		
		void HostReportEvent(BuildEventArgs e)
		{
			if (host != null) {
				host.CallMethodOnHost("ReportEvent", e);
			} else {
				// enable error reporting for in-process builds
				EventSource eventSource = hostEventSource;
				if (eventSource != null) {
					eventSource.RaiseEvent(e);
				}
			}
		}
		
		sealed class ForwardingLogger : ILogger
		{
			Program program;
			
			public ForwardingLogger(Program program)
			{
				this.program = program;
			}
			
			IEventSource eventSource;
			
			public LoggerVerbosity Verbosity { get; set; }
			public string Parameters { get; set; }
			
			public void Initialize(IEventSource eventSource)
			{
				this.eventSource = eventSource;
				EventTypes eventMask = program.currentJob.EventMask;
				if ((eventMask & EventTypes.Message) != 0)
					eventSource.MessageRaised += OnEvent;
				if ((eventMask & EventTypes.Error) != 0)
					eventSource.ErrorRaised += OnEvent;
				if ((eventMask & EventTypes.Warning) != 0)
					eventSource.WarningRaised += OnEvent;
				if ((eventMask & EventTypes.BuildStarted) != 0)
					eventSource.BuildStarted += OnEvent;
				if ((eventMask & EventTypes.BuildFinished) != 0)
					eventSource.BuildFinished += OnEvent;
				if ((eventMask & EventTypes.ProjectStarted) != 0)
					eventSource.ProjectStarted += OnEvent;
				if ((eventMask & EventTypes.ProjectFinished) != 0)
					eventSource.ProjectFinished += OnEvent;
				if ((eventMask & EventTypes.TargetStarted) != 0)
					eventSource.TargetStarted += OnEvent;
				if ((eventMask & EventTypes.TargetFinished) != 0)
					eventSource.TargetFinished += OnEvent;
				if ((eventMask & EventTypes.TaskStarted) != 0)
					eventSource.TaskStarted += OnEvent;
				else
					eventSource.TaskStarted += OnTaskStarted;
				if ((eventMask & EventTypes.TaskFinished) != 0)
					eventSource.TaskFinished += OnEvent;
				else
					eventSource.TaskFinished += OnTaskFinished;
				if ((eventMask & EventTypes.Custom) != 0)
					eventSource.CustomEventRaised += OnEvent;
				if ((eventMask & EventTypes.Unknown) != 0)
					eventSource.AnyEventRaised += OnUnknownEventRaised;
				if (eventMask != EventTypes.All)
					eventSource.AnyEventRaised += OnAnyEvent;
				
				#if WORKERDEBUG
				eventSource.AnyEventRaised += CountEvent;
				#endif
			}
			
			public void Shutdown()
			{
				EventTypes eventMask = program.currentJob.EventMask;
				if ((eventMask & EventTypes.Message) != 0)
					eventSource.MessageRaised -= OnEvent;
				if ((eventMask & EventTypes.Error) != 0)
					eventSource.ErrorRaised -= OnEvent;
				if ((eventMask & EventTypes.Warning) != 0)
					eventSource.WarningRaised -= OnEvent;
				if ((eventMask & EventTypes.BuildStarted) != 0)
					eventSource.BuildStarted -= OnEvent;
				if ((eventMask & EventTypes.BuildFinished) != 0)
					eventSource.BuildFinished -= OnEvent;
				if ((eventMask & EventTypes.ProjectStarted) != 0)
					eventSource.ProjectStarted -= OnEvent;
				if ((eventMask & EventTypes.ProjectFinished) != 0)
					eventSource.ProjectFinished -= OnEvent;
				if ((eventMask & EventTypes.TargetStarted) != 0)
					eventSource.TargetStarted -= OnEvent;
				if ((eventMask & EventTypes.TargetFinished) != 0)
					eventSource.TargetFinished -= OnEvent;
				if ((eventMask & EventTypes.TaskStarted) != 0)
					eventSource.TaskStarted -= OnEvent;
				else
					eventSource.TaskStarted -= OnTaskStarted;
				if ((eventMask & EventTypes.TaskFinished) != 0)
					eventSource.TaskFinished -= OnEvent;
				else
					eventSource.TaskFinished -= OnTaskFinished;
				if ((eventMask & EventTypes.Custom) != 0)
					eventSource.CustomEventRaised -= OnEvent;
				if ((eventMask & EventTypes.Unknown) != 0)
					eventSource.AnyEventRaised -= OnUnknownEventRaised;
				if (eventMask != EventTypes.All)
					eventSource.AnyEventRaised -= OnAnyEvent;
				
				#if WORKERDEBUG
				eventSource.AnyEventRaised -= CountEvent;
				#endif
			}
			
			// registered for AnyEventRaised to support build cancellation.
			// is not registered if all events should be forwarded, in that case, OnEvent
			// already handles build cancellation
			void OnAnyEvent(object sender, BuildEventArgs e)
			{
				if (program.requestCancellation)
					throw new BuildCancelException();
			}
			
			// used for all events that should be forwarded
			void OnEvent(object sender, BuildEventArgs e)
			{
				if (program.requestCancellation)
					throw new BuildCancelException();
				program.HostReportEvent(e);
			}
			
			// registered for AnyEventRaised to forward unknown events
			void OnUnknownEventRaised(object sender, BuildEventArgs e)
			{
				if (EventSource.GetEventType(e) == EventTypes.Unknown)
					OnEvent(sender, e);
			}
			
			// registered when only specific tasks should be forwarded
			void OnTaskStarted(object sender, TaskStartedEventArgs e)
			{
				if (program.currentJob.InterestingTaskNames.Contains(e.TaskName))
					OnEvent(sender, e);
			}
			
			// registered when only specific tasks should be forwarded
			void OnTaskFinished(object sender, TaskFinishedEventArgs e)
			{
				if (program.currentJob.InterestingTaskNames.Contains(e.TaskName))
					OnEvent(sender, e);
			}
			
			
			#if WORKERDEBUG
			void CountEvent(object sender, BuildEventArgs e)
			{
				Program.CountEvent(EventSource.GetEventType(e));
			}
			#endif
		}
		
		#if WORKERDEBUG
		static Dictionary<EventTypes, int> eventCounts = new Dictionary<EventTypes, int>();
		
		static void CountEvent(EventTypes e)
		{
			if (eventCounts.ContainsKey(e))
				eventCounts[e] += 1;
			else
				eventCounts[e] = 1;
		}
		
		static void DisplayEventCounts()
		{
			foreach (var pair in eventCounts) {
				Console.WriteLine("    " + pair.Key.ToString() + ": " + pair.Value.ToString());
			}
		}
		#endif
		
		[Serializable]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]
		sealed class BuildCancelException : Exception
		{
			public BuildCancelException()
			{
			}
			
			BuildCancelException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}
		}
	}
}
