// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;
using ICSharpCode.SharpDevelop.BuildWorker.Interprocess;
using Microsoft.Build.Framework;
using Microsoft.Build.BuildEngine;

// activate this define to see the build worker window
//#define WORKERDEBUG

namespace ICSharpCode.SharpDevelop.BuildWorker
{
	class Program
	{
		static HostProcess host;
		BuildJob currentJob;
		bool requestCancellation;
		
		internal static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_CurrentDomain_UnhandledException);
			
			if (args.Length == 2 && args[0] == "worker") {
				try {
					host = new HostProcess(new Program());
					host.WorkerProcessMain(args[1]);
				} catch (Exception ex) {
					ShowMessageBox(ex.ToString());
				}
			} else {
				Program.Log("ICSharpCode.SharpDevelop.BuildWorker.exe is used to compile " +
				            "MSBuild projects inside SharpDevelop.");
				Program.Log("If you want to compile projects on the command line, use " +
				            "MSBuild.exe (part of the .NET Framework)");
			}
		}
		
		static void AppDomain_CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			ShowMessageBox(e.ExceptionObject.ToString());
		}
		
		internal static ProcessStartInfo CreateStartInfo()
		{
			ProcessStartInfo info = new ProcessStartInfo(typeof(Program).Assembly.Location);
			info.WorkingDirectory = Path.GetDirectoryName(info.FileName);
			info.Arguments = "worker";
			#if RELEASE || !WORKERDEBUG
			info.UseShellExecute = false;
			info.CreateNoWindow = true;
			#endif
			return info;
		}
		
		internal static void ShowMessageBox(string text)
		{
			System.Windows.Forms.MessageBox.Show(text, "SharpDevelop Build Worker Process");
		}
		
		[Conditional("DEBUG")]
		internal static void Log(string text)
		{
			#if WORKERDEBUG
			DateTime now = DateTime.Now;
			Console.WriteLine(now.ToString() + "," + now.Millisecond.ToString("d3") + " " + text);
			#endif
		}
		
		// Called with CallMethodOnWorker
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
			#if DEBUG && WORKERDEBUG
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
		public void CancelBuild()
		{
			lock (this) {
				requestCancellation = true;
			}
		}
		
		Engine engine;
		
		void RunThread()
		{
			Program.Log("In build thread");
			bool success = false;
			try {
				if (engine == null) {
					engine = CreateEngine();
				}
				success = DoBuild();
			} catch (Exception ex) {
				host.CallMethodOnHost("ReportException", ex);
			} finally {
				Program.Log("BuildDone");
				
				#if DEBUG && WORKERDEBUG
				Console.Title = "BuildWorker - no job";
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
			
			return engine;
		}
		
		InProcessLogger inProcessLogger;
		
		internal void BuildInProcess(BuildSettings settings, BuildJob job)
		{
			lock (this) {
				if (currentJob != null)
					throw new InvalidOperationException("Already running a job");
				currentJob = job;
				requestCancellation = false;
				inProcessLogger = new InProcessLogger();
			}
			bool success = false;
			try {
				if (engine == null) {
					engine = new Engine(ToolsetDefinitionLocations.Registry
					                    | ToolsetDefinitionLocations.ConfigurationFile);
				}
				engine.UnregisterAllLoggers();
				engine.RegisterLogger(inProcessLogger);
				foreach (ILogger logger in settings.Logger) {
					logger.Initialize(inProcessLogger.EventSource);
				}
				success = DoBuild();
				foreach (ILogger logger in settings.Logger) {
					logger.Shutdown();
				}
				engine.UnregisterAllLoggers();
			} catch (Exception ex) {
				if (WorkerManager.ShowError != null)
					WorkerManager.ShowError(ex);
				else
					Program.ShowMessageBox(ex.ToString());
			} finally {
				lock (this) {
					currentJob = null;
				}
				if (settings.BuildDoneCallback != null)
					settings.BuildDoneCallback(success);
			}
		}
		
		bool DoBuild()
		{
			engine.GlobalProperties.Clear();
			foreach (KeyValuePair<string, string> pair in currentJob.Properties) {
				engine.GlobalProperties.SetProperty(pair.Key, pair.Value);
			}
			
			Project project = LoadProject(engine, currentJob.ProjectFileName);
			if (project == null)
				return false;
			
			if (string.IsNullOrEmpty(currentJob.Target)) {
				return engine.BuildProject(project);
			} else {
				return engine.BuildProject(project, currentJob.Target.Split(';'));
			}
		}
		
		Project LoadProject(Engine engine, string fileName)
		{
			Project project = engine.CreateNewProject();
			try {
				project.Load(fileName);
				
				// When we set BuildingInsideVisualStudio, MSBuild tries to build all projects
				// every time because in Visual Studio, the host compiler does the change detection
				// We override the property '_ComputeNonExistentFileProperty' which is responsible
				// for recompiling each time - our _ComputeNonExistentFileProperty does nothing,
				// which re-enables the MSBuild's usual change detection
				project.Targets.AddNewTarget("_ComputeNonExistentFileProperty");
				
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
			if (host != null) {
				HostReportEvent(e);
			} else {
				// enable error reporting for in-process builds
				InProcessLogger logger = inProcessLogger;
				if (logger != null) {
					logger.EventSource.RaiseEvent(e);
				}
			}
		}
		
		void HostReportEvent(BuildEventArgs e)
		{
			host.CallMethodOnHost("ReportEvent", e);
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
				eventSource.AnyEventRaised += OnAnyEventRaised;
			}
			
			public void Shutdown()
			{
				eventSource.AnyEventRaised -= OnAnyEventRaised;
			}
			
			void OnAnyEventRaised(object sender, BuildEventArgs e)
			{
				if (program.requestCancellation)
					throw new BuildCancelException();
				program.HostReportEvent(e);
			}
		}
		
		sealed class InProcessLogger : ILogger
		{
			public readonly EventSource EventSource = new EventSource();
			
			public LoggerVerbosity Verbosity { get; set; }
			
			public string Parameters { get; set; }
			
			IEventSource realEventSource;
			
			public void Initialize(IEventSource eventSource)
			{
				this.realEventSource = eventSource;
				this.realEventSource.AnyEventRaised += OnAnyEventRaised;
			}
			
			public void Shutdown()
			{
				this.realEventSource.AnyEventRaised -= OnAnyEventRaised;
			}
			
			void OnAnyEventRaised(object sender, BuildEventArgs e)
			{
				this.EventSource.RaiseEvent(e);
			}
		}
		
		[Serializable]
		sealed class BuildCancelException : Exception
		{
		}
	}
}