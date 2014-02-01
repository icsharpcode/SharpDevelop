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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
		
		public void RunBuildJob(BuildJob job, IMSBuildChainedLoggerFilter loggerChain, Action<bool> reportWhenDone, CancellationToken cancellationToken)
		{
			if (job == null)
				throw new ArgumentNullException("job");
			if (loggerChain == null)
				throw new ArgumentNullException("loggerChain");
			BuildWorker worker = GetFreeWorker();
			worker.RunJob(job, loggerChain, reportWhenDone, cancellationToken);
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

			IMSBuildChainedLoggerFilter loggerChain;
			Action<bool> reportWhenDone;
			CancellationTokenRegistration cancellationRegistration;
			
			public void RunJob(BuildJob job, IMSBuildChainedLoggerFilter loggerChain, Action<bool> reportWhenDone, CancellationToken cancellationToken)
			{
				Debug.Assert(loggerChain != null);
				this.loggerChain = loggerChain;
				this.reportWhenDone = reportWhenDone;
				try {
					process.Writer.Write("StartBuild");
					job.WriteTo(process.Writer);
					this.cancellationRegistration = cancellationToken.Register(OnCancel);
				} catch (IOException ex) {
					// "Pipe is broken"
					loggerChain.HandleError(new BuildError(null, 0, 0, null, "Error talking to build worker: " + ex.Message));
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
						loggerChain.HandleBuildEvent(EventSource.DecodeEvent(reader));
						break;
					case "BuildDone":
						bool success = reader.ReadBoolean();
						BuildDone(success);
						MarkAsFree();
						break;
					case "ReportException":
						string ex = reader.ReadString();
						MessageService.ShowException(new BuildWorkerException(), ex);
						break;
					default:
						throw new NotSupportedException(command);
				}
			}
			
			void BuildDone(bool success)
			{
				lock (this) {
					cancellationRegistration.Dispose();
					if (reportWhenDone == null)
						return;
					reportWhenDone(success);
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
						reportBuildError = (reportWhenDone != null); // only if not done
					}
				}
				if (reportBuildError) {
					loggerChain.HandleError(new BuildError(null, 0, 0, null, "Build worker process exited unexpectedly."));
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
		
		sealed class BuildWorkerException : Exception
		{
			public BuildWorkerException() : base("Exception from build worker")
			{
			}
		}
	}
}
