// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Util;
using Microsoft.Win32.SafeHandles;

namespace ICSharpCode.GitAddIn
{
	public class GitVersionProvider : IDocumentVersionProvider
	{
		#region PInvoke
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool CreateProcess(
			string lpApplicationName,
			string lpCommandLine,
			IntPtr lpProcessAttributes,
			IntPtr lpThreadAttributes,
			[MarshalAs(UnmanagedType.Bool)] bool bInheritHandles,
			uint dwCreationFlags,
			IntPtr lpEnvironment,
			string lpCurrentDirectory,
			[In] ref StartupInfo lpStartupInfo,
			out PROCESS_INFORMATION lpProcessInformation
		);
		
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetStdHandle(int nStdHandle);
		
		const uint STARTF_USESTDHANDLES = 0x00000100;
		
		const int STD_INPUT_HANDLE  = -10;
		const int STD_OUTPUT_HANDLE = -11;
		const int STD_ERROR_HANDLE  = -12;
		
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		struct StartupInfo
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
			public IntPtr hStdInput;
			public SafePipeHandle hStdOutput;
			public IntPtr hStdError;
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public int dwProcessId;
			public int dwThreadId;
		}
		#endregion
		
		public Stream OpenBaseVersion(string fileName)
		{
			if (!Git.IsInWorkingCopy(fileName))
				return null;
			
			string git = Git.FindGit();
			if (git == null)
				return null;
			
			return OpenOutput(git, fileName, GetBlobHash(git, fileName));
		}
		
		internal static string GetBlobHash(string gitExe, string fileName)
		{
			if (!File.Exists(fileName))
				return null;
			
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = Path.GetDirectoryName(fileName);
			runner.Start(gitExe, "ls-tree HEAD " + Path.GetFileName(fileName));
			
			string blobHash = null;
			runner.OutputLineReceived += delegate(object sender, LineReceivedEventArgs e) {
				string[] parts = e.Line.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length >= 3) {
					if (parts[2].Length == 40)
						blobHash = parts[2];
				}
			};
			
			runner.WaitForExit();
			return blobHash;
		}
		
		Stream OpenOutput(string gitExe, string fileName, string blobHash)
		{
			if (!File.Exists(fileName))
				return null;
			if (blobHash == null)
				return null;
			
			AnonymousPipeServerStream pipe = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
			
			StartupInfo startupInfo = new GitVersionProvider.StartupInfo();
			startupInfo.dwFlags = STARTF_USESTDHANDLES;
			startupInfo.hStdOutput = pipe.ClientSafePipeHandle;
			startupInfo.hStdInput = GetStdHandle(STD_INPUT_HANDLE);
			startupInfo.hStdError = GetStdHandle(STD_ERROR_HANDLE);
			startupInfo.cb = 16;
			
			PROCESS_INFORMATION procInfo;
			
			string commandLine = "\"" + gitExe + "\" cat-file blob " + blobHash;
			string workingDir = Path.GetDirectoryName(fileName);
			Debug.WriteLine(workingDir + "> " + commandLine);
			const uint CREATE_NO_WINDOW = 0x08000000;
			if (!CreateProcess(null, commandLine,
			                   IntPtr.Zero, IntPtr.Zero, true, CREATE_NO_WINDOW, IntPtr.Zero, workingDir, ref startupInfo,
			                   out procInfo)) {
				pipe.DisposeLocalCopyOfClientHandle();
				pipe.Close();
				return null;
			}
			
			pipe.DisposeLocalCopyOfClientHandle();
			
			return pipe;
		}
		
		public IDisposable WatchBaseVersionChanges(string fileName, EventHandler callback)
		{
			if (!File.Exists(fileName))
				return null;
			if (!Git.IsInWorkingCopy(fileName))
				return null;
			
			string git = Git.FindGit();
			if (git == null)
				return null;
			
			return new BaseVersionChangeWatcher(fileName, GetBlobHash(git, fileName), callback);
		}
	}
	
	class BaseVersionChangeWatcher : IDisposable
	{
		EventHandler callback;
		string fileName, hash;
		RepoChangeWatcher watcher;
		
		public BaseVersionChangeWatcher(string fileName, string hash, EventHandler callback)
		{
			string root = Git.FindWorkingCopyRoot(fileName);
			if (root == null)
				throw new InvalidOperationException(fileName + " must be under version control!");
			
			this.callback = callback;
			this.fileName = fileName;
			this.hash = hash;
			
			watcher = RepoChangeWatcher.AddWatch(Path.Combine(root, ".git"), HandleChanges);
		}
		
		void HandleChanges()
		{
			string newHash = GitVersionProvider.GetBlobHash(Git.FindGit(), fileName);
			if (newHash != hash) {
				LoggingService.Info(fileName + " was changed!");
				callback(this, EventArgs.Empty);
			}
			this.hash = newHash;
		}
		
		public void Dispose()
		{
			watcher.ReleaseWatch(HandleChanges);
		}
	}
}
