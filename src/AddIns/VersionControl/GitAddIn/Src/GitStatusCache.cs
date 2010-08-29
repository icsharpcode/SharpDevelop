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
			lock (statusSetDict) {
				GitStatusSet statusSet;
				foreach (var pair in statusSetDict) {
					if (FileUtility.IsEqualFileName(pair.Key, wcRoot))
						return pair.Value;
				}
				
				statusSet = new GitStatusSet();
				GitGetFiles(wcRoot, statusSet);
				GitGetStatus(wcRoot, statusSet);
				statusSetDict.Add(new KeyValuePair<string, GitStatusSet>(wcRoot, statusSet));
				return statusSet;
			}
		}
		
		static void GitGetFiles(string wcRoot, GitStatusSet statusSet)
		{
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = wcRoot;
			runner.LogStandardOutputAndError = false;
			runner.OutputLineReceived += delegate(object sender, LineReceivedEventArgs e) {
				if (!string.IsNullOrEmpty(e.Line)) {
					statusSet.AddEntry(e.Line, GitStatus.OK);
				}
			};
			
			string command = "git ls-files";
			bool hasErrors = false;
			runner.ErrorLineReceived += delegate(object sender, LineReceivedEventArgs e) {
				if (!hasErrors) {
					hasErrors = true;
					GitMessageView.AppendLine(runner.WorkingDirectory + "> " + command);
				}
				GitMessageView.AppendLine(e.Line);
			};
			runner.Start("cmd", "/c " + command);
			runner.WaitForExit();
		}
		
		static void GitGetStatus(string wcRoot, GitStatusSet statusSet)
		{
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = wcRoot;
			runner.LogStandardOutputAndError = false;
			runner.OutputLineReceived += delegate(object sender, LineReceivedEventArgs e) {
				if (!string.IsNullOrEmpty(e.Line)) {
					Match m = statusParseRegex.Match(e.Line);
					if (m.Success) {
						statusSet.AddEntry(m.Groups[2].Value, StatusFromText(m.Groups[1].Value));
					}
				}
			};
			string command = "git status -a --untracked-files=no";
			bool hasErrors = false;
			runner.ErrorLineReceived += delegate(object sender, LineReceivedEventArgs e) {
				if (!hasErrors) {
					hasErrors = true;
					GitMessageView.AppendLine(runner.WorkingDirectory + "> " + command);
				}
				GitMessageView.AppendLine(e.Line);
			};
			runner.Start("cmd", "/c " + command);
			runner.WaitForExit();
		}
		
		static GitStatus StatusFromText(string text)
		{
			switch (text) {
				case "deleted":
					return GitStatus.Deleted;
				case "modified":
					return GitStatus.Modified;
				case "new file":
					return GitStatus.Added;
				default:
					throw new NotSupportedException();
			}
		}
		
		static readonly Regex statusParseRegex = new Regex(@"#\s+(deleted|modified|new file):\s+(\S.*)$");
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
