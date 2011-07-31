// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.GitAddIn
{
	/// <summary>
	/// Description of Git.
	/// </summary>
	public class Git
	{
		public static bool IsInWorkingCopy(string fileName)
		{
			return FindWorkingCopyRoot(fileName) != null;
		}
		
		public static string FindWorkingCopyRoot(string fileName)
		{
			try {
				if (!Path.IsPathRooted(fileName))
					return null;
			} catch (ArgumentException) {
				return null;
			}
			if (!Directory.Exists(fileName))
				fileName = Path.GetDirectoryName(fileName);
			DirectoryInfo info = new DirectoryInfo(fileName);
			while (info != null) {
				if (Directory.Exists(Path.Combine(info.FullName, ".git")))
					return info.FullName;
				info = info.Parent;
			}
			return null;
		}
		
		public static void Add(string fileName, Action<int> callback)
		{
			string wcRoot = FindWorkingCopyRoot(fileName);
			if (wcRoot == null)
				return;
			RunGit(wcRoot, "add " + AdaptFileName(wcRoot, fileName), callback);
		}
		
		public static void Remove(string fileName, bool indexOnly, Action<int> callback)
		{
			string wcRoot = FindWorkingCopyRoot(fileName);
			if (wcRoot == null)
				return;
			if (indexOnly)
				RunGit(wcRoot, "rm --cached " + AdaptFileName(wcRoot, fileName), callback);
			else
				RunGit(wcRoot, "rm " + AdaptFileName(wcRoot, fileName), callback);
		}
		
		public static string AdaptFileName(string wcRoot, string fileName)
		{
			return '"' + AdaptFileNameNoQuotes(wcRoot, fileName) + '"';
		}
		
		public static string AdaptFileNameNoQuotes(string wcRoot, string fileName)
		{
			string relFileName = FileUtility.GetRelativePath(wcRoot, fileName);
			return relFileName.Replace('\\', '/');
		}
		
		public static void RunGit(string workingDir, string arguments, Action<int> finished)
		{
			GitMessageView.AppendLine(workingDir + "> git " + arguments);
			string git = FindGit();
			if (git == null) {
				GitMessageView.AppendLine("Could not find git.exe");
				return;
			}
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = workingDir;
			runner.LogStandardOutputAndError = false;
			runner.OutputLineReceived += (sender, e) => GitMessageView.AppendLine(e.Line);
			runner.ErrorLineReceived  += (sender, e) => GitMessageView.AppendLine(e.Line);
			runner.ProcessExited += delegate {
				GitMessageView.AppendLine("Done. (exit code " + runner.ExitCode + ")");
				
				if (finished != null)
					finished(runner.ExitCode);
			};
			runner.Start(git, arguments);
		}
		
		/// <summary>
		/// Finds 'git.exe'
		/// </summary>
		public static string FindGit()
		{
			string[] paths = Environment.GetEnvironmentVariable("PATH").Split(';');
			foreach (string path in paths) {
				try {
					string exe = Path.Combine(path, "git.exe");
					if (File.Exists(exe))
						return exe;
					string cmd = Path.Combine(path, "git.cmd");
					if (File.Exists(cmd)) {
						exe = Path.Combine(path, "..\\bin\\git.exe");
						if (File.Exists(exe))
							return exe;
					}
				} catch (ArgumentException) {
					// ignore invalid entries in PATH
				}
			}
			string gitExe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86, Environment.SpecialFolderOption.DoNotVerify), "bin\\git.exe");
			if (File.Exists(gitExe))
				return gitExe;
			return null;
		}
		
		/*
		/// <summary>
		/// Runs git and returns the output if successful (exit code 0).
		/// If not successful, returns null and displays output to message view.
		/// </summary>
		public static string RunGit(string workingDir, string arguments)
		{
			return RunGit(workingDir, arguments, false);
		}
		
		public static string RunGit(string workingDir, string arguments, bool ignoreNothingToCommitError)
		{
			using (AsynchronousWaitDialog dlg = AsynchronousWaitDialog.ShowWaitDialog("git " + arguments, true)) {
				ProcessRunner runner = new ProcessRunner();
				dlg.Cancelled += delegate {
					runner.Kill();
				};
				runner.WorkingDirectory = workingDir;
				string git = FindGit();
				if (git == null) ...
				runner.Start(git, arguments);
				runner.WaitForExit();
				if (runner.ExitCode == 0) {
					return runner.StandardOutput;
				} else {
					GitMessageView.Category.ClearText();
					GitMessageView.AppendLine("$ git " + arguments);
					GitMessageView.AppendLine(runner.StandardOutput);
					GitMessageView.AppendLine(runner.StandardError);
					GitMessageView.AppendLine("Failed with exit code " + runner.ExitCode);
					return null;
				}
			}
		}
		 */
	}
}
