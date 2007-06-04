// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Util
{
	/// <summary>
	/// Runs a process that sends output to standard output and to
	/// standard error.
	/// </summary>
	public class ProcessRunner : IDisposable
	{
		Process process;
		string standardOutput = String.Empty;
		string workingDirectory = String.Empty;
		OutputReader standardOutputReader;
		OutputReader standardErrorReader;

		/// <summary>
		/// Triggered when the process has exited.
		/// </summary>
		public event EventHandler ProcessExited;
		
		/// <summary>
		/// Triggered when a line of text is read from the standard output.
		/// </summary>
		public event LineReceivedEventHandler OutputLineReceived;
		
		/// <summary>
		/// Triggered when a line of text is read from the standard error.
		/// </summary>
		public event LineReceivedEventHandler ErrorLineReceived;
		
		/// <summary>
		/// Creates a new instance of the <see cref="ProcessRunner"/>.
		/// </summary>
		public ProcessRunner()
		{
		}
		
		/// <summary>
		/// Gets or sets the process's working directory.
		/// </summary>
		public string WorkingDirectory {
			get {
				return workingDirectory;
			}
			
			set {
				workingDirectory = value;
			}
		}

		/// <summary>
		/// Gets the standard output returned from the process.
		/// </summary>
		public string StandardOutput {
			get {
				string output = String.Empty;
				if (standardOutputReader != null) {
					output = standardOutputReader.Output;
				}
				return output;
			}
		}
		
		/// <summary>
		/// Gets the standard error output returned from the process.
		/// </summary>
		public string StandardError {
			get {
				string output = String.Empty;
				if (standardErrorReader != null) {
					output = standardErrorReader.Output;
				}
				return output;
			}
		}
		
		/// <summary>
		/// Releases resources held by the <see cref="ProcessRunner"/>
		/// </summary>
		public void Dispose()
		{
		}
		
		/// <summary>
		/// Gets the process exit code.
		/// </summary>
		public int ExitCode {
			get {	
				int exitCode = 0;
				if (process != null) {
					exitCode = process.ExitCode;
				}
				return exitCode;
			}
		}
		
		/// <summary>
		/// Waits for the process to exit.
		/// </summary>
		public void WaitForExit()
		{
			WaitForExit(Int32.MaxValue);
		}
		
		/// <summary>
		/// Waits for the process to exit.
		/// </summary>
		/// <param name="timeout">A timeout in milliseconds.</param>
		/// <returns><see langword="true"/> if the associated process has 
		/// exited; otherwise, <see langword="false"/></returns>
		public bool WaitForExit(int timeout)
		{
			if (process == null) {
				throw new ProcessRunnerException(StringParser.Parse("${res:ICSharpCode.NAntAddIn.ProcessRunner.NoProcessRunningErrorText}"));
			}
			
			bool exited = process.WaitForExit(timeout);
			
			if (exited) {
				standardOutputReader.WaitForFinish();
				standardErrorReader.WaitForFinish();
			}
			
			return exited;
		}
		
		public bool IsRunning {
			get {
				bool isRunning = false;
				
				if (process != null) {
					isRunning = !process.HasExited;
				}
				
				return isRunning;
			}
		}
		
		/// <summary>
		/// Starts the process.
		/// </summary>
		/// <param name="command">The process filename.</param>
		/// <param name="arguments">The command line arguments to
		/// pass to the command.</param>
		public void Start(string command, string arguments)
		{	
			process = new Process();
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.FileName = command;
			process.StartInfo.WorkingDirectory = workingDirectory;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.Arguments = arguments;
			
			if (ProcessExited != null) {
				process.EnableRaisingEvents = true;
				process.Exited += OnProcessExited;
			}

			bool started = false;
			try {
				process.Start();
				started = true;
			} finally {
				if (!started) {
					process.Exited -= OnProcessExited;			
					process = null;
				}
			}
				
			standardOutputReader = new OutputReader(process.StandardOutput);
			if (OutputLineReceived != null) {
				standardOutputReader.LineReceived += new LineReceivedEventHandler(OnOutputLineReceived);
			}
			
			standardOutputReader.Start();
			
			standardErrorReader = new OutputReader(process.StandardError);
			if (ErrorLineReceived != null) {
				standardErrorReader.LineReceived += new LineReceivedEventHandler(OnErrorLineReceived);
			}
			
			standardErrorReader.Start();
		}
		
		/// <summary>
		/// Starts the process.
		/// </summary>
		/// <param name="command">The process filename.</param>
		public void Start(string command)
		{
			Start(command, String.Empty);
		}
		
		/// <summary>
		/// Kills the running process.
		/// </summary>
		public void Kill()
		{
			if (process != null) {
				if (!process.HasExited) {
					process.Kill();
					process.Close();
					process.Dispose();
					process = null;
					standardOutputReader.WaitForFinish();
					standardErrorReader.WaitForFinish();
				} else {
					process = null;
				}
			}
		}
		
		/// <summary>
		/// Raises the <see cref="ProcessExited"/> event.
		/// </summary>
		protected void OnProcessExited(object sender, EventArgs e)
		{
			if (ProcessExited != null) {
				
				standardOutputReader.WaitForFinish();
				standardErrorReader.WaitForFinish();
				
				ProcessExited(this, e);
			}
		}
		
		/// <summary>
		/// Raises the <see cref="OutputLineReceived"/> event.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The line received event arguments.</param>
		protected void OnOutputLineReceived(object sender, LineReceivedEventArgs e)
		{
			if (OutputLineReceived != null) {
				OutputLineReceived(this, e);
			}
		}
		
		/// <summary>
		/// Raises the <see cref="ErrorLineReceived"/> event.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The line received event arguments.</param>
		protected void OnErrorLineReceived(object sender, LineReceivedEventArgs e)
		{
			if (ErrorLineReceived != null) {
				ErrorLineReceived(this, e);
			}
		}
	}
}
