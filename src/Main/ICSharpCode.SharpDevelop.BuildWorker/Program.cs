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

// activate this define to see the build worker window
//#define WORKERDEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;

using ICSharpCode.SharpDevelop.Interprocess;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.BuildWorker
{
	sealed class Program
	{
		HostProcess host;
		BuildJob currentJob;
		bool requestCancellation;
		bool requestProcessShutdown;
		
		[STAThread]
		internal static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_CurrentDomain_UnhandledException);
			
			if (args.Length == 3 && args[0] == "worker") {
				try {
					Program program = new Program();
					program.host = new HostProcess(args[1], args[2]);
					Thread communicationThread = new Thread(program.RunCommunicationThread);
					communicationThread.Name = "Communication";
					communicationThread.Start();
					program.RunBuildThread();
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
		
		void RunCommunicationThread()
		{
			try {
				host.Run(DataReceived);
			} catch (Exception ex) {
				ShowMessageBox(ex.ToString());
			} finally {
				lock (this) {
					requestProcessShutdown = true;
					Monitor.PulseAll(this);
				}
			}
		}
		
		readonly MSBuildWrapper buildWrapper = new MSBuildWrapper();
		
		void DataReceived(string command, BinaryReader reader)
		{
			// called on communication thread
			switch (command) {
				case "StartBuild":
					StartBuild(BuildJob.ReadFrom(reader));
					break;
				case "Cancel":
					CancelBuild();
					break;
				default:
					throw new InvalidOperationException("Unknown command");
			}
		}
		
		static void AppDomain_CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			ShowMessageBox(e.ExceptionObject.ToString());
		}
		
		#if RELEASE && WORKERDEBUG
		#error WORKERDEBUG must not be defined if RELEASE is defined
		#endif
		
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
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public void StartBuild(BuildJob job)
		{
			// called on communication thread
			if (job == null)
				throw new ArgumentNullException("job");
			Program.Log("Got job:");
			Program.Log(job.ToString());
			lock (this) {
				if (currentJob != null)
					throw new InvalidOperationException("Already running a job");
				currentJob = job;
				requestCancellation = false;
				Monitor.PulseAll(this);
			}
			#if WORKERDEBUG
			Console.Title = "BuildWorker - " + Path.GetFileName(job.ProjectFileName);
			#endif
		}
		
		public void CancelBuild()
		{
			// called on communication thread
			Program.Log("CancelBuild()");
			lock (this) {
				if (!requestCancellation) {
					requestCancellation = true;
					buildWrapper.Cancel();
				}
			}
		}
		
		void RunBuildThread()
		{
			while (true) {
				// Wait for next job, or for shutdown
				lock (this) {
					while (currentJob == null && !requestProcessShutdown)
						Monitor.Wait(this);
					if (requestProcessShutdown)
						break;
				}
				PerformBuild();
			}
		}
		
		void PerformBuild()
		{
			Program.Log("In build thread");
			bool success = false;
			try {
				if (File.Exists(currentJob.ProjectFileName)) {
					success = buildWrapper.DoBuild(currentJob, new ForwardingLogger(this));
				} else {
					success = false;
					HostReportEvent(new BuildErrorEventArgs(null, null, currentJob.ProjectFileName, 0, 0, 0, 0, "Project file '" + Path.GetFileName(currentJob.ProjectFileName) + "' not found", null, null));
				}
			} catch (Exception ex) {
				host.Writer.Write("ReportException");
				host.Writer.Write(ex.ToString());
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
				host.Writer.Write("BuildDone");
				host.Writer.Write(success);
			}
		}
		
		void HostReportEvent(BuildEventArgs e)
		{
			host.Writer.Write("ReportEvent");
			EventSource.EncodeEvent(host.Writer, e);
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
				
				#if WORKERDEBUG
				eventSource.AnyEventRaised -= CountEvent;
				#endif
			}
			
			// used for all events that should be forwarded
			void OnEvent(object sender, BuildEventArgs e)
			{
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
	}
}
