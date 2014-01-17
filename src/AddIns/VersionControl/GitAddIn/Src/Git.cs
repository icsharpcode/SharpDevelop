// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

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
		
		public static Task AddAsync(string fileName)
		{
			string wcRoot = FindWorkingCopyRoot(fileName);
			if (wcRoot == null)
				return Task.FromResult(false);
			return RunGitAsync(wcRoot, "add", AdaptFileName(wcRoot, fileName));
		}
		
		public static Task RemoveAsync(string fileName, bool indexOnly)
		{
			string wcRoot = FindWorkingCopyRoot(fileName);
			if (wcRoot == null)
				return Task.FromResult(false);
			if (indexOnly)
				return RunGitAsync(wcRoot, "rm", "--cached", AdaptFileName(wcRoot, fileName));
			else
				return RunGitAsync(wcRoot, "rm", AdaptFileName(wcRoot, fileName));
		}
		
		public static string AdaptFileName(string wcRoot, string fileName)
		{
			string relFileName = FileUtility.GetRelativePath(wcRoot, fileName);
			return relFileName.Replace('\\', '/');
		}
		
		public static Task<int> RunGitAsync(string workingDir, params string[] arguments)
		{
			string git = FindGit();
			if (git == null)
				return Task.FromResult(9009);
			ProcessRunner p = new ProcessRunner();
			p.WorkingDirectory = workingDir;
			return p.RunInOutputPadAsync(GitMessageView.Category, git, arguments);
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
			string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86, Environment.SpecialFolderOption.DoNotVerify);
			string gitExe = Path.Combine(programFiles, @"git\bin\git.exe");
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
