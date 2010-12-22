// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;

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
			
			return OpenOutput(fileName, GetBlobHash(fileName));
		}
		
		string GetBlobHash(string fileName)
		{
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = Path.GetDirectoryName(fileName);
			runner.Start("cmd", "/c git ls-tree HEAD " + Path.GetFileName(fileName));
			runner.WaitForExit();
			
			string output = runner.StandardOutput.Trim();
			string[] parts = output.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
			
			if (parts.Length < 3)
				return null;
			
			return parts[2];
		}
		
		Stream OpenOutput(string fileName, string blobHash)
		{
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
			
			if (!CreateProcess(null, string.Format("cmd /c git cat-file blob {0}", blobHash),
			                   IntPtr.Zero, IntPtr.Zero, true, 0, IntPtr.Zero, Path.GetDirectoryName(fileName), ref startupInfo,
			                   out procInfo)) {
				pipe.DisposeLocalCopyOfClientHandle();
				pipe.Close();
				return null;
			}
			
			pipe.DisposeLocalCopyOfClientHandle();
			
			return pipe;
		}
	}
}
