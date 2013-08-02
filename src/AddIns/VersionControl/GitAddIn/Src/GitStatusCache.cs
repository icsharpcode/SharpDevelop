// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.GitAddIn
{
	public enum GitStatus
	{
		None,
		Added,
		Modified,
		Deleted,
		OK,
		Ignored
	}
	
	public static class GitStatusCache
	{
		static object getStatusLock = new object();
		static List<KeyValuePair<string, GitStatusSet>> statusSetDict = new List<KeyValuePair<string, GitStatusSet>>();
		
		public static void ClearCachedStatus(string fileName)
		{
			lock (statusSetDict) {
				for (int i = 0; i < statusSetDict.Count; i++) {
					if (FileUtility.IsBaseDirectory(statusSetDict[i].Key, fileName)) {
						statusSetDict.RemoveAt(i--);
					}
				}
			}
		}
		
		public static GitStatus GetFileStatus(string fileName)
		{
			string wcroot = Git.FindWorkingCopyRoot(fileName);
			if (wcroot == null)
				return GitStatus.None;
			GitStatusSet gss = GetStatusSet(wcroot);
			return gss.GetStatus(Git.AdaptFileNameNoQuotes(wcroot, fileName));
		}
		
		public static GitStatusSet GetStatusSet(string wcRoot)
		{
			// Prevent multiple GetStatusSet calls from running in parallel
			lock (getStatusLock) {
				GitStatusSet statusSet;
				// Don't hold statusSetDict during the whole operation; we don't want
				// to slow down other threads calling ClearCachedStatus()
				lock (statusSetDict) {
					foreach (var pair in statusSetDict) {
						if (FileUtility.IsEqualFileName(pair.Key, wcRoot))
							return pair.Value;
					}
				}
				
				statusSet = new GitStatusSet();
				GitGetFiles(wcRoot, statusSet);
				GitGetStatus(wcRoot, statusSet);
				lock (statusSetDict) {
					statusSetDict.Add(new KeyValuePair<string, GitStatusSet>(wcRoot, statusSet));
				}
				return statusSet;
			}
		}
		
		static void GitGetFiles(string wcRoot, GitStatusSet statusSet)
		{
			string git = Git.FindGit();
			if (git == null)
				return;
			
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = wcRoot;
			runner.LogStandardOutputAndError = false;
			runner.OutputLineReceived += delegate(object sender, LineReceivedEventArgs e) {
				if (!string.IsNullOrEmpty(e.Line)) {
					statusSet.AddEntry(e.Line, GitStatus.OK);
				}
			};
			
			string command = "ls-files";
			bool hasErrors = false;
			runner.ErrorLineReceived += delegate(object sender, LineReceivedEventArgs e) {
				if (!hasErrors) {
					hasErrors = true;
					GitMessageView.AppendLine(runner.WorkingDirectory + "> git " + command);
				}
				GitMessageView.AppendLine(e.Line);
			};
			runner.Start(git, command);
			runner.WaitForExit();
		}
		
		static void GitGetStatus(string wcRoot, GitStatusSet statusSet)
		{
			string git = Git.FindGit();
			if (git == null)
				return;
			
			string command = "status --porcelain --untracked-files=no";
			bool hasErrors = false;
			
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = wcRoot;
			runner.LogStandardOutputAndError = false;
			runner.OutputLineReceived += delegate(object sender, LineReceivedEventArgs e) {
				if (!string.IsNullOrEmpty(e.Line)) {
					Match m = statusParseRegex.Match(e.Line);
					if (m.Success) {
						statusSet.AddEntry(m.Groups[2].Value, StatusFromText(m.Groups[1].Value));
					} else {
						if (!hasErrors) {
							// in front of first output line, print the command line we invoked
							hasErrors = true;
							GitMessageView.AppendLine(runner.WorkingDirectory + "> git " + command);
						}
						GitMessageView.AppendLine("unknown output: " + e.Line);
					}
				}
			};
			runner.ErrorLineReceived += delegate(object sender, LineReceivedEventArgs e) {
				if (!hasErrors) {
					hasErrors = true;
					GitMessageView.AppendLine(runner.WorkingDirectory + "> git " + command);
				}
				GitMessageView.AppendLine(e.Line);
			};
			runner.Start(git, command);
			runner.WaitForExit();
		}
		
		static GitStatus StatusFromText(string text)
		{
			if (text.Contains("A"))
				return GitStatus.Added;
			else if (text.Contains("D"))
				return GitStatus.Deleted;
			else if (text.Contains("M"))
				return GitStatus.Modified;
			else
				return GitStatus.None;
		}
		
		static readonly Regex statusParseRegex = new Regex(@"^([DMA ][DMA ])\s(\S.*)$");
	}
	
	public class GitStatusSet
	{
		Dictionary<string, GitStatusSet> entries;
		GitStatus status = GitStatus.OK;
		
		public GitStatus AddEntry(string path, GitStatus status)
		{
			if (string.IsNullOrEmpty(path) || path == ".") {
				this.status = status;
				return status;
			}
			if (entries == null)
				entries = new Dictionary<string, GitStatusSet>();
			string entry;
			string subpath;
			int pos = path.IndexOf('/');
			if (pos < 0) {
				entry = path;
				subpath = null;
			} else {
				entry = path.Substring(0, pos);
				subpath = path.Substring(pos + 1);
			}
			GitStatusSet subset;
			if (!entries.TryGetValue(entry, out subset))
				entries[entry] = subset = new GitStatusSet();
			status = subset.AddEntry(subpath, status);
			if (status == GitStatus.Added || status == GitStatus.Deleted || status == GitStatus.Modified) {
				this.status = GitStatus.Modified;
			}
			return this.status;
		}
		
		public GitStatus GetStatus(string path)
		{
			if (string.IsNullOrEmpty(path) || path == ".")
				return status;
			if (entries == null)
				return GitStatus.None;
			string entry;
			string subpath;
			int pos = path.IndexOf('/');
			if (pos < 0) {
				entry = path;
				subpath = null;
			} else {
				entry = path.Substring(0, pos);
				subpath = path.Substring(pos + 1);
			}
			GitStatusSet subset;
			if (!entries.TryGetValue(entry, out subset))
				return GitStatus.None;
			else
				return subset.GetStatus(subpath);
		}
	}
}
