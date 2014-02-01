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
