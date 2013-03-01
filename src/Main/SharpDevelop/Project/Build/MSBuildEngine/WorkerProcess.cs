// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Manages a worker process that communicates with the host using a bidirectional named pipe.
	/// </summary>
	sealed class WorkerProcess : IDisposable
	{
		readonly AnonymousPipeServerStream hostToWorkerPipe = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
		readonly AnonymousPipeServerStream workerToHostPipe = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
		readonly Process process = new Process();
		readonly Thread readerThread;
		int id;
		volatile bool isDisposed;
		readonly Action<string, BinaryReader> dataArrived;
		
		public WorkerProcess(Action<string, BinaryReader> dataArrived)
		{
			this.dataArrived = dataArrived;
			readerThread = new Thread(ReaderThread);
			readerThread.IsBackground = true;
		}
		
		public void Start(ProcessStartInfo info)
		{
			if (info == null)
				throw new ArgumentNullException("info");
			process.StartInfo = info;
			
			Debug.WriteLine("WorkerProcess starting...");
			string oldArguments = info.Arguments;
			try {
				info.Arguments += " " + hostToWorkerPipe.GetClientHandleAsString() + " " + workerToHostPipe.GetClientHandleAsString();
				if (!process.Start())
					throw new InvalidOperationException("Process.Start() failed");
			} finally {
				info.Arguments = oldArguments;
				// client handles were inherited into started process, dispose our copy
				hostToWorkerPipe.DisposeLocalCopyOfClientHandle();
				workerToHostPipe.DisposeLocalCopyOfClientHandle();
			}
			id = process.Id;
			readerThread.Start();
			this.Writer = new BinaryWriter(hostToWorkerPipe);
			Debug.WriteLine("WorkerProcess " + id + " started");
		}
		
		void ReaderThread()
		{
			try {
				using (BinaryReader reader = new BinaryReader(workerToHostPipe)) {
					while (!isDisposed) {
						string text;
						try {
							text = reader.ReadString();
						} catch (EndOfStreamException) {
							Core.LoggingService.Debug("Cannot read from WorkerProcess " + id + ": end of stream");
							break;
						}
						dataArrived(text, reader);
					}
				}
				Core.LoggingService.Debug("Stopped reading from WorkerProcess " + id + ".");
				if (ProcessExited != null)
					ProcessExited(this, EventArgs.Empty);
			} catch (Exception ex) {
				Core.MessageService.ShowException(ex);
			} finally {
				Core.LoggingService.Debug("End of reader thread on WorkerProcess " + id + ".");
			}
		}
		
		public event EventHandler ProcessExited;
		
		public BinaryWriter Writer { get; private set; }
		
		public void Kill()
		{
			try {
				if (!process.HasExited) {
					process.Kill();
				}
			} catch (InvalidOperationException) {
				// may occur when the worker process crashes
			}
		}
		
		public void Dispose()
		{
			isDisposed = true;
			Core.LoggingService.Debug("Telling worker process to exit");
			hostToWorkerPipe.Dispose(); // send "end-of-stream" to worker
			if (Thread.CurrentThread != readerThread) {
				// don't deadlock when disposing from within ReaderThread event
				Core.LoggingService.Debug("Waiting for thread-join");
				if (!readerThread.Join(3000)) {
					Core.LoggingService.Debug("Thread-join failed. Killing worker process.");
					Kill();
					Core.LoggingService.Debug("Waiting for thread-join");
					readerThread.Join();
				}
				Core.LoggingService.Debug("Joined!");
			}
			workerToHostPipe.Dispose();
			process.Dispose();
		}
	}
}
