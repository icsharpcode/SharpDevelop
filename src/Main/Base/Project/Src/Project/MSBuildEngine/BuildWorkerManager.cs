// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.BuildWorker;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.Project
{
	sealed class BuildWorkerManager
	{
		readonly List<BuildWorker> freeWorkers = new List<BuildWorker>();
		readonly string workerProcessName;
		
		public static readonly BuildWorkerManager MSBuild40 = new BuildWorkerManager("ICSharpCode.SharpDevelop.BuildWorker40.exe");
		public static readonly BuildWorkerManager MSBuild35 = new BuildWorkerManager("ICSharpCode.SharpDevelop.BuildWorker35.exe");
		
		private BuildWorkerManager(string workerProcessName)
		{
			this.workerProcessName = workerProcessName;
		}
		
		public void RunBuildJob(BuildJob job, IEnumerable<ILogger> loggers, IBuildFeedbackSink reportWhenDone)
		{
			BuildWorker worker = GetFreeWorker();
			worker.RunJob(job, loggers, reportWhenDone);
		}
		
		BuildWorker GetFreeWorker()
		{
			lock (freeWorkers) {
				if (freeWorkers.Count != 0) {
					BuildWorker w = freeWorkers[freeWorkers.Count - 1];
					freeWorkers.RemoveAt(freeWorkers.Count - 1);
					w.MarkAsInUse();
					return w;
				}
			}
			return new BuildWorker(this);
		}
		
		sealed class BuildWorker
		{
			readonly BuildWorkerManager parentManager;
			readonly WorkerProcess process;
			Timer timer;
			bool isFree;
			
			public BuildWorker(BuildWorkerManager parentManager)
			{
				this.parentManager = parentManager;
				string sdbin = Path.GetDirectoryName(typeof(BuildWorker).Assembly.Location);
				ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(sdbin, parentManager.workerProcessName), "worker");
				startInfo.WorkingDirectory = sdbin;
				startInfo.UseShellExecute = false;
				startInfo.CreateNoWindow = true;
				process = new WorkerProcess(DataArrived);
				process.ProcessExited += process_ProcessExited;
				process.Start(startInfo);
			}

			EventSource source;
			IEnumerable<ILogger> loggers;
			IBuildFeedbackSink reportWhenDone;
			CancellationTokenRegistration cancellationRegistration;
			
			public void RunJob(BuildJob job, IEnumerable<ILogger> loggers, IBuildFeedbackSink reportWhenDone)
			{
				this.source = new EventSource();
				this.loggers = loggers;
				this.reportWhenDone = reportWhenDone;
				foreach (var logger in loggers) {
					logger.Initialize(source);
				}
				try {
					process.Writer.Write("StartBuild");
					job.WriteTo(process.Writer);
					this.cancellationRegistration = reportWhenDone.ProgressMonitor.CancellationToken.Register(OnCancel);
				} catch (IOException ex) {
					// "Pipe is broken"
					source.ForwardEvent(new BuildErrorEventArgs(null, null, null, 0, 0, 0, 0, "Error talking to build worker: " + ex.Message, null, null));
					BuildDone(false);
				}
			}
			
			void OnCancel()
			{
				LoggingService.Debug("Cancel Build Worker");
				process.Writer.Write("Cancel");
			}
			
			void DataArrived(string command, BinaryReader reader)
			{
				LoggingService.Debug("Received command " + command);
				switch (command) {
					case "ReportEvent":
						source.ForwardEvent(EventSource.DecodeEvent(reader));
						break;
					case "BuildDone":
						bool success = reader.ReadBoolean();
						BuildDone(success);
						MarkAsFree();
						break;
					case "ReportException":
						string ex = reader.ReadString();
						MessageService.ShowException(new Exception("Exception from build worker"), ex);
						break;
					default:
						throw new NotSupportedException(command);
				}
			}

			void BuildDone(bool success)
			{
				lock (this) {
					if (reportWhenDone == null)
						return;
					cancellationRegistration.Dispose();
					foreach (var logger in loggers) {
						logger.Shutdown();
					}
					reportWhenDone.Done(success);
					reportWhenDone = null;
				}
			}
			
			void MarkAsFree()
			{
				lock (parentManager.freeWorkers) {
					this.isFree = true;
					parentManager.freeWorkers.Add(this);
					timer = new Timer(timer_Elapsed, null, 10000, 0);
				}
			}
			
			void timer_Elapsed(object state)
			{
				lock (parentManager.freeWorkers) {
					if (!isFree)
						return; // worker is already being used again
					parentManager.freeWorkers.Remove(this);
					MarkAsInUse();
				}
				process.Dispose();
			}
			
			void process_ProcessExited(object sender, EventArgs e)
			{
				bool reportBuildError = false;
				lock (parentManager.freeWorkers) {
					if (isFree) {
						parentManager.freeWorkers.Remove(this);
						MarkAsInUse();
					} else {
						reportBuildError = true;
					}
				}
				if (reportBuildError) {
					source.ForwardEvent(new BuildErrorEventArgs(null, null, null, 0, 0, 0, 0, "Build worker process exited unexpectedly.", null, null));
					BuildDone(false);
				}
				process.Dispose();
			}
			
			public void MarkAsInUse()
			{
				Debug.Assert(this.isFree);
				this.isFree = false;
				timer.Dispose();
			}
		}
	}
}
