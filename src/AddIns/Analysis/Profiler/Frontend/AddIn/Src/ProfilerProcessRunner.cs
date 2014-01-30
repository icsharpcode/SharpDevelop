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
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.Profiler.Controller;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Profiler.AddIn
{
	/// <summary>
	/// Description of ProfilerProcessRunner.
	/// </summary>
	public class ProfilerProcessRunner : IProcessRunner
	{
		ProcessStartInfo psi;
		ProfilerRunner profilerRunner;
		Process runningProcess;
		IProfilingDataWriter writer;
		ProfilerOptions options;
		readonly object lockObj = new object();
		bool wasStarted;
		
		public ProfilerProcessRunner(IProfilingDataWriter writer, ProfilerOptions options)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			if (options == null)
				throw new ArgumentNullException("options");
			this.writer = writer;
			this.options = options;
			this.psi = new ProcessStartInfo();
			wasStarted = false;
		}
		
		public Task<int> RunInOutputPadAsync(IOutputCategory outputCategory, string program, params string[] arguments)
		{
			throw new NotImplementedException();
		}
		
		public void Start(string program, params string[] arguments)
		{
			lock (lockObj) {
				if (wasStarted)
					throw new InvalidOperationException();
				
				profilerRunner = new ProfilerRunner(psi, true, writer);
				profilerRunner.RunFinished += delegate { hasExited = true; };
				runningProcess = profilerRunner.Run();
				wasStarted = true;
			}
		}
		
		public void StartCommandLine(string commandLine)
		{
			string[] args = ProcessRunner.CommandLineToArgumentArray(commandLine);
			Start(args.FirstOrDefault() ?? "", args.Skip(1).ToArray());
		}
		
		public void Kill()
		{
			profilerRunner.Stop();
		}
		
		TaskCompletionSource<object> waitForExitTCS;
		bool hasExited;
		
		public Task WaitForExitAsync()
		{
			if (hasExited)
				return Task.FromResult(true);
			if (!wasStarted)
				throw new InvalidOperationException("Process was not yet started");
			lock (lockObj) {
				if (waitForExitTCS != null) {
					waitForExitTCS = new TaskCompletionSource<object>();
					profilerRunner.RunFinished += delegate { waitForExitTCS.SetResult(null); };
				}
			}
			return waitForExitTCS.Task;
		}
		
		public StreamReader OpenStandardOutputReader()
		{
			return runningProcess.StandardOutput;
		}
		
		public StreamReader OpenStandardErrorReader()
		{
			return runningProcess.StandardError;
		}
		
		public string WorkingDirectory {
			get { return psi.WorkingDirectory; }
			set { psi.WorkingDirectory = value; }
		}
		
		public ProcessCreationFlags CreationFlags { get; set; }
		
		public IDictionary<string, string> EnvironmentVariables { get; set; }

		public bool RedirectStandardOutput { get; set; }

		public bool RedirectStandardError { get; set; }

		public bool RedirectStandardOutputAndErrorToSingleStream { get; set; }

		public Stream StandardOutput {
			get {
				if (runningProcess == null)
					throw new InvalidOperationException("Cannot access StdOut of process, because it is not yet started!");
				return null;//runningProcess.StandardOutput;
			}
		}
		public Stream StandardError {
			get {
				if (runningProcess == null)
					throw new InvalidOperationException("Cannot access StdErr of process, because it is not yet started!");
				return null;//runningProcess.StandardError;
			}
		}
		
		bool disposed = false;
		
		public void Dispose()
		{
			if (!disposed) {
				profilerRunner.Dispose();
				disposed = true;
			}
		}
	}
}
