// Copyright (c) 2008-2009 Daniel Grunwald
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
			runner.Start("cmd", "/c git " + arguments);
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
				runner.Start("git", arguments);
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
