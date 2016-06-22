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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

using System.Threading.Tasks;
using ICSharpCode.Core;
using Microsoft.Win32.SafeHandles;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Util;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop
{
	[Flags]
	public enum ProcessCreationFlags
	{
		None = 0,
		/// <summary>
		/// Creates a new console instead of inheriting the parent console.
		/// </summary>
		CreateNewConsole = 0x00000010,
		/// <summary>
		/// Launches a console application without a console window.
		/// </summary>
		CreateNoWindow = 0x08000000
	}
	
	public interface IProcessRunner : IDisposable
	{
		Task<int> RunInOutputPadAsync(IOutputCategory outputCategory, string program, params string[] arguments);
		string WorkingDirectory { get; set; }
		ProcessCreationFlags CreationFlags { get; set; }
		IDictionary<string, string> EnvironmentVariables { get; }
		bool RedirectStandardOutput { get; set; }
		bool RedirectStandardError { get; set; }
		bool RedirectStandardOutputAndErrorToSingleStream { get; set; }
		
		void Start(string program, params string[] arguments);
		void StartCommandLine(string commandLine);
		void Kill();
		Task WaitForExitAsync();
		
		Stream StandardOutput { get; }
		Stream StandardError { get; }
		StreamReader OpenStandardOutputReader();
		StreamReader OpenStandardErrorReader();
	}
	
	/// <summary>
	/// Class for starting external processes.
	/// Very similar to System.Diagnostics.Process, but supports binary stdout/stderr (not only text),
	/// and allows using the same pipe for both stdout and stderr.
	/// 
	/// Also, implements an interface to support mocking in unit tests.
	/// </summary>
	public class ProcessRunner : IProcessRunner, IDisposable
	{
		public static Encoding OemEncoding {
			get {
				return NativeMethods.OemEncoding;
			}
		}
		
		#region SafeProcessHandle
		[SecurityCritical]
		sealed class SafeProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			// this private ctor is required for SafeHandle implementations
			SafeProcessHandle() : base(true)
			{
			}
			
			internal SafeProcessHandle(IntPtr handle) : base(true)
			{
				base.SetHandle(handle);
			}
			
			[SecurityCritical]
			protected override bool ReleaseHandle()
			{
				return NativeMethods.CloseHandle(handle);
			}
		}
		#endregion
		
		#region Native structures
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		protected struct STARTUPINFO
		{
			public uint cb;
			public string lpReserved;
			public string lpDesktop;
			public string lpTitle;
			public uint dwX;
			public uint dwY;
			public uint dwXSize;
			public uint dwYSize;
			public uint dwXCountChars;
			public uint dwYCountChars;
			public uint dwFillAttribute;
			public uint dwFlags;
			public short wShowWindow;
			public short cbReserved2;
			public IntPtr lpReserved2;
			public SafePipeHandle hStdInput;
			public SafePipeHandle hStdOutput;
			public SafePipeHandle hStdError;
		}
		
		[StructLayout(LayoutKind.Sequential)]
		protected struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public int dwProcessId;
			public int dwThreadId;
		}
		#endregion
		
		#region Native methods
		[DllImport("kernel32.dll", EntryPoint = "CreateProcess", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool NativeCreateProcess(
			string lpApplicationName,
			StringBuilder lpCommandLine,
			IntPtr lpProcessAttributes,
			IntPtr lpThreadAttributes,
			[MarshalAs(UnmanagedType.Bool)] bool bInheritHandles,
			uint dwCreationFlags,
			string lpEnvironment,
			string lpCurrentDirectory,
			[In] ref STARTUPINFO lpStartupInfo,
			out PROCESS_INFORMATION lpProcessInformation
		);
		
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool TerminateProcess(SafeProcessHandle processHandle, int exitCode);
		
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetExitCodeProcess(SafeProcessHandle processHandle, out int exitCode);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetStdHandle(int nStdHandle);
		
		[DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		static extern unsafe char** CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);
		
		[DllImport("kernel32.dll")]
		static extern IntPtr LocalFree(IntPtr hMem);
		#endregion
		
		#region CommandLine <-> Argument Array
		/// <summary>
		/// Decodes a command line into an array of arguments according to the CommandLineToArgvW rules.
		/// </summary>
		/// <remarks>
		/// Command line parsing rules:
		/// - 2n backslashes followed by a quotation mark produce n backslashes, and the quotation mark is considered to be the end of the argument.
		/// - (2n) + 1 backslashes followed by a quotation mark again produce n backslashes followed by a quotation mark.
		/// - n backslashes not followed by a quotation mark simply produce n backslashes.
		/// </remarks>
		public static unsafe string[] CommandLineToArgumentArray(string commandLine)
		{
			if (string.IsNullOrEmpty(commandLine))
				return new string[0];
			int numberOfArgs;
			char** arr = CommandLineToArgvW(commandLine, out numberOfArgs);
			if (arr == null)
				throw new Win32Exception();
			try {
				string[] result = new string[numberOfArgs];
				for (int i = 0; i < numberOfArgs; i++) {
					result[i] = new string(arr[i]);
				}
				return result;
			} finally {
				// Free memory obtained by CommandLineToArgW.
				LocalFree(new IntPtr(arr));
			}
		}
		
		static readonly char[] charsNeedingQuoting = { ' ', '\t', '\n', '\v', '"' };
		
		/// <summary>
		/// Escapes a set of arguments according to the CommandLineToArgvW rules.
		/// </summary>
		/// <remarks>
		/// Command line parsing rules:
		/// - 2n backslashes followed by a quotation mark produce n backslashes, and the quotation mark is considered to be the end of the argument.
		/// - (2n) + 1 backslashes followed by a quotation mark again produce n backslashes followed by a quotation mark.
		/// - n backslashes not followed by a quotation mark simply produce n backslashes.
		/// </remarks>
		public static string ArgumentArrayToCommandLine(params string[] arguments)
		{
			if (arguments == null)
				return null;
			StringBuilder b = new StringBuilder();
			for (int i = 0; i < arguments.Length; i++) {
				if (i > 0)
					b.Append(' ');
				AppendArgument(b, arguments[i]);
			}
			return b.ToString();
		}
		
		static void AppendArgument(StringBuilder b, string arg)
		{
			if (arg.Length > 0 && arg.IndexOfAny(charsNeedingQuoting) < 0) {
				b.Append(arg);
			} else {
				b.Append('"');
				for (int j = 0; ; j++) {
					int backslashCount = 0;
					while (j < arg.Length && arg[j] == '\\') {
						backslashCount++;
						j++;
					}
					if (j == arg.Length) {
						b.Append('\\', backslashCount * 2);
						break;
					} else if (arg[j] == '"') {
						b.Append('\\', backslashCount * 2 + 1);
						b.Append('"');
					} else {
						b.Append('\\', backslashCount);
						b.Append(arg[j]);
					}
				}
				b.Append('"');
			}
		}
		#endregion
		
		#region RunInOutputPad
		public async Task<int> RunInOutputPadAsync(IOutputCategory outputCategory, string program, params string[] arguments)
		{
			RedirectStandardOutputAndErrorToSingleStream = true;
			Start(program, arguments);
			StringBuilder printedCommandLine = new StringBuilder();
			if (WorkingDirectory != null) {
				printedCommandLine.Append(WorkingDirectory);
				printedCommandLine.Append("> ");
			}
			printedCommandLine.Append(CommandLine);
			outputCategory.AppendLine(printedCommandLine.ToString());
			
			using (TextReader reader = OpenStandardOutputReader()) {
				await reader.CopyToAsync(new MessageViewCategoryTextWriter(outputCategory));
			}
			await WaitForExitAsync();
			outputCategory.AppendLine(StringParser.Parse("${res:XML.MainMenu.ToolMenu.ExternalTools.ExitedWithCode} " + this.ExitCode));
			return this.ExitCode;
		}
		#endregion
		
		#region Start Info Properties
		/// <summary>
		/// Gets or sets the process's working directory.
		/// </summary>
		public string WorkingDirectory { get; set; }
		
		ProcessCreationFlags creationFlags = ProcessCreationFlags.CreateNoWindow;
		
		public ProcessCreationFlags CreationFlags {
			get { return creationFlags; }
			set { creationFlags = value; }
		}
		
		IDictionary<string, string> environmentVariables;
		
		public IDictionary<string, string> EnvironmentVariables {
			get {
				if (environmentVariables == null) {
					environmentVariables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
					foreach (DictionaryEntry e in Environment.GetEnvironmentVariables()) {
						environmentVariables.Add((string)e.Key, (string)e.Value);
					}
				}
				return environmentVariables;
			}
		}
		
		public string CommandLine { get; private set; }
		
		public bool RedirectStandardOutput { get; set; }
		public bool RedirectStandardError { get; set; }
		
		/// <summary>
		/// Gets whether to use a single stream for both stdout and stderr.
		/// </summary>
		public bool RedirectStandardOutputAndErrorToSingleStream { get; set; }
		#endregion
		
		#region Start
		bool wasStarted;
		SafeProcessHandle safeProcessHandle;
		
		public void Start(string program, params string[] arguments)
		{
			StringBuilder commandLine = new StringBuilder();
			AppendArgument(commandLine, program);
			if (arguments != null) {
				for (int i = 0; i < arguments.Length; i++) {
					commandLine.Append(' ');
					AppendArgument(commandLine, arguments[i]);
				}
			}
			StartCommandLine(commandLine.ToString());
		}
		
		public void StartCommandLine(string commandLine)
		{
			lock (lockObj) {
				if (wasStarted)
					throw new InvalidOperationException();
				DoStart(commandLine);
			}
		}
		
		protected virtual void DoStart(string commandLine)
		{
			this.CommandLine = commandLine;
			
			const uint STARTF_USESTDHANDLES = 0x00000100;
			
			const int STD_INPUT_HANDLE  = -10;
			const int STD_OUTPUT_HANDLE = -11;
			const int STD_ERROR_HANDLE  = -12;
			
			const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;
			
			STARTUPINFO startupInfo = new STARTUPINFO();
			startupInfo.cb = (uint)Marshal.SizeOf(typeof(STARTUPINFO));
			startupInfo.dwFlags = STARTF_USESTDHANDLES;
			
			// Create pipes
			startupInfo.hStdInput = new SafePipeHandle(GetStdHandle(STD_INPUT_HANDLE), ownsHandle: false);
			if (RedirectStandardOutput || RedirectStandardOutputAndErrorToSingleStream) {
				standardOutput = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
				startupInfo.hStdOutput = standardOutput.ClientSafePipeHandle;
			} else {
				startupInfo.hStdOutput = new SafePipeHandle(GetStdHandle(STD_OUTPUT_HANDLE), ownsHandle: false);
			}
			if (RedirectStandardOutputAndErrorToSingleStream) {
				standardError = standardOutput;
				startupInfo.hStdError = standardError.ClientSafePipeHandle;
			} else if (RedirectStandardError) {
				standardError = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
				startupInfo.hStdError = standardError.ClientSafePipeHandle;
			} else {
				startupInfo.hStdError = new SafePipeHandle(GetStdHandle(STD_ERROR_HANDLE), ownsHandle: false);
			}
			
			uint flags = (uint)this.CreationFlags;
			
			string environmentBlock = null;
			if (environmentVariables != null) {
				environmentBlock = BuildEnvironmentBlock(environmentVariables);
				flags |= CREATE_UNICODE_ENVIRONMENT;
			}
			
			PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();
			try {
				CreateProcess(null, new StringBuilder(commandLine), IntPtr.Zero, IntPtr.Zero, true, flags, environmentBlock, WorkingDirectory, ref startupInfo, out processInfo);
				wasStarted = true;
			} finally {
				if (processInfo.hProcess != IntPtr.Zero && processInfo.hProcess != new IntPtr(-1)) {
					safeProcessHandle = new SafeProcessHandle(processInfo.hProcess);
				}
				if (processInfo.hThread != IntPtr.Zero && processInfo.hThread != new IntPtr(-1)) {
					NativeMethods.CloseHandle(processInfo.hThread);
				}
				// Dispose the client side handles of the pipe.
				// They got copied into the new process, we don't need our local copies anymore.
				startupInfo.hStdInput.Dispose();
				startupInfo.hStdOutput.Dispose();
				startupInfo.hStdError.Dispose();
				if (!wasStarted) {
					// In case of error, dispose the server side of the pipes as well
					if (standardOutput != null) {
						standardOutput.Dispose();
						standardOutput = null;
					}
					if (standardError != null) {
						standardError.Dispose();
						standardError = null;
					}
				}
			}
			//StartStreamCopyAfterProcessCreation();
		}
		
		static string BuildEnvironmentBlock(IEnumerable<KeyValuePair<string, string>> environment)
		{
			StringBuilder b = new StringBuilder();
			foreach (var pair in environment.OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase)) {
				b.Append(pair.Key);
				b.Append('=');
				b.Append(pair.Value);
				b.Append('\0');
			}
			b.Append('\0');
			return b.ToString();
		}
		
		protected virtual void CreateProcess(
			string lpApplicationName,
			StringBuilder lpCommandLine,
			IntPtr lpProcessAttributes,
			IntPtr lpThreadAttributes,
			bool bInheritHandles,
			uint dwCreationFlags,
			string lpEnvironment,
			string lpCurrentDirectory,
			ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation)
		{
			if (!NativeCreateProcess(lpApplicationName, lpCommandLine, lpProcessAttributes, lpThreadAttributes, bInheritHandles, dwCreationFlags,
			                         lpEnvironment, lpCurrentDirectory, ref lpStartupInfo, out lpProcessInformation)) {
				throw new Win32Exception();
			}
		}
		#endregion
		
		public void Dispose()
		{
			if (safeProcessHandle != null)
				safeProcessHandle.Dispose();
			if (standardOutput != null)
				standardOutput.Dispose();
			if (standardError != null)
				standardError.Dispose();
		}
		
		#region HasExited / ExitCode / Kill
		public bool HasExited {
			get { return WaitForExit(0); }
		}
		
		/// <summary>
		/// Gets the process exit code.
		/// </summary>
		public int ExitCode {
			get {
				if (!WaitForExit(0))
					throw new InvalidOperationException("Process has not yet exited");
				return exitCode; // WaitForExit has the side effect of setting exitCode
			}
		}
		
		/// <summary>
		/// Sends the kill signal to the process.
		/// Does not wait for the process to complete to exit after being killed.
		/// </summary>
		public void Kill()
		{
			if (!wasStarted)
				throw new InvalidOperationException("Process was not started");
			if (!TerminateProcess(safeProcessHandle, -1)) {
				int err = Marshal.GetLastWin32Error();
				// If TerminateProcess fails, maybe it's because the process has already exited.
				if (!WaitForExit(0))
					throw new Win32Exception(err);
			}
		}
		#endregion
		
		#region WaitForExit
		sealed class ProcessWaitHandle : WaitHandle
		{
			public ProcessWaitHandle(SafeProcessHandle processHandle)
			{
				var currentProcess = new HandleRef(this, NativeMethods.GetCurrentProcess());
				SafeWaitHandle safeWaitHandle;
				if (!NativeMethods.DuplicateHandle(currentProcess, processHandle, currentProcess, out safeWaitHandle, 0, false, NativeMethods.DUPLICATE_SAME_ACCESS)) {
					throw new Win32Exception();
				}
				base.SafeWaitHandle = safeWaitHandle;
			}
		}
		
		bool hasExited;
		int exitCode;
		
		public void WaitForExit()
		{
			WaitForExit(Timeout.Infinite);
		}
		
		public bool WaitForExit(int millisecondsTimeout)
		{
			if (hasExited)
				return true;
			if (!wasStarted)
				throw new InvalidOperationException("Process was not yet started");
			if (safeProcessHandle.IsClosed)
				throw new ObjectDisposedException("ProcessRunner");
			using (var waitHandle = new ProcessWaitHandle(safeProcessHandle)) {
				if (waitHandle.WaitOne(millisecondsTimeout, false)) {
					if (!GetExitCodeProcess(safeProcessHandle, out exitCode))
						throw new Win32Exception();
					// Wait until the output is processed
//					if (standardOutputTask != null)
//						standardOutputTask.Wait();
//					if (standardErrorTask != null)
//						standardErrorTask.Wait();
					hasExited = true;
				}
			}
			return hasExited;
		}
		
		readonly object lockObj = new object();
		TaskCompletionSource<object> waitForExitTCS;
		ProcessWaitHandle waitForExitAsyncWaitHandle;
		RegisteredWaitHandle waitForExitAsyncRegisteredWaitHandle;
		
		/// <summary>
		/// Asynchronously waits for the process to exit.
		/// </summary>
		public Task WaitForExitAsync()
		{
			if (hasExited)
				return Task.FromResult(true);
			if (!wasStarted)
				throw new InvalidOperationException("Process was not yet started");
			if (safeProcessHandle.IsClosed)
				throw new ObjectDisposedException("ProcessRunner");
			lock (lockObj) {
				if (waitForExitTCS == null) {
					waitForExitTCS = new TaskCompletionSource<object>();
					waitForExitAsyncWaitHandle = new ProcessWaitHandle(safeProcessHandle);
					waitForExitAsyncRegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject(waitForExitAsyncWaitHandle, WaitForExitAsyncCallback, null, -1, true);
				}
				return waitForExitTCS.Task;
			}
		}
		
		void WaitForExitAsyncCallback(object context, bool wasSignaled)
		{
			waitForExitAsyncRegisteredWaitHandle.Unregister(null);
			waitForExitAsyncRegisteredWaitHandle = null;
			waitForExitAsyncWaitHandle.Close();
			waitForExitAsyncWaitHandle = null;
			// Wait until the output is processed
//			if (standardOutputTask != null)
//				await standardOutputTask;
//			if (standardErrorTask != null)
//				await standardErrorTask;
			waitForExitTCS.SetResult(null);
		}
		#endregion
		
		#region StandardOutput/StandardError
		AnonymousPipeServerStream standardOutput;
		AnonymousPipeServerStream standardError;
		
		public Stream StandardOutput {
			get {
				if (standardOutput == null)
					throw new InvalidOperationException(wasStarted ? "stdout was not redirected" : "Process not yet started");
				return standardOutput;
			}
		}
		
		public Stream StandardError {
			get {
				if (standardError == null)
					throw new InvalidOperationException(wasStarted ? "stderr was not redirected" : "Process not yet started");
				return standardError;
			}
		}
		
		/// <summary>
		/// Opens a text reader around the standard output.
		/// </summary>
		public StreamReader OpenStandardOutputReader()
		{
			return new StreamReader(this.StandardOutput, OemEncoding);
		}
		
		/// <summary>
		/// Opens a text reader around the standard error.
		/// </summary>
		public StreamReader OpenStandardErrorReader()
		{
			return new StreamReader(this.StandardError, OemEncoding);
		}
		#endregion
	}
}
