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
using System.Threading;

namespace ICSharpCode.SharpDevelop.Interprocess
{
	/// <summary>
	/// Used in the worker process to refer to the host.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	sealed class HostProcess
	{
		readonly AnonymousPipeClientStream hostToWorkerPipe;
		readonly AnonymousPipeClientStream workerToHostPipe;
		
		public HostProcess(string pipe1, string pipe2)
		{
			hostToWorkerPipe = new AnonymousPipeClientStream(PipeDirection.In, pipe1);
			workerToHostPipe = new AnonymousPipeClientStream(PipeDirection.Out, pipe2);
			this.Writer = new BinaryWriter(workerToHostPipe);
		}
		
		public BinaryWriter Writer { get; private set; }
		
		public void Run(Action<string, BinaryReader> dataReceived)
		{
			using (BinaryReader reader = new BinaryReader(hostToWorkerPipe)) {
				while (true) {
					string command;
					try {
						command = reader.ReadString();
					} catch (EndOfStreamException) {
						break;
					}
					dataReceived(command, reader);
				}
			}
		}
	}
}
