// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;

using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using Microsoft.Win32.SafeHandles;

namespace ICSharpCode.GitAddIn
{
	public class GitVersionProvider : IDocumentVersionProvider
	{
		public async Task<Stream> OpenBaseVersionAsync(FileName fileName)
		{
			if (!Git.IsInWorkingCopy(fileName))
				return null;
			
			string git = Git.FindGit();
			if (git == null)
				return null;
			
			return OpenOutput(git, fileName, await GetBlobHashAsync(git, fileName).ConfigureAwait(false));
		}
		
		internal static async Task<string> GetBlobHashAsync(string gitExe, FileName fileName)
		{
			if (!File.Exists(fileName))
				return null;
			
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = fileName.GetParentDirectory();
			runner.RedirectStandardOutput = true;
			runner.Start(gitExe, "ls-tree", "HEAD", fileName.GetFileName());
			using (var reader = runner.OpenStandardOutputReader()) {
				string firstLine = await reader.ReadLineAsync().ConfigureAwait(false);
				if (firstLine != null) {
					string[] parts = firstLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
					if (parts.Length >= 3) {
						if (parts[2].Length == 40)
							return parts[2];
					}
				}
			}
			return null;
		}
		
		Stream OpenOutput(string gitExe, FileName fileName, string blobHash)
		{
			if (blobHash == null)
				return null;
			if (!File.Exists(fileName))
				return null;
			
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = fileName.GetParentDirectory();
			runner.RedirectStandardOutput = true;
			runner.Start(gitExe, "cat-file", "blob", blobHash);
			return runner.StandardOutput;
		}
		
		public IDisposable WatchBaseVersionChanges(FileName fileName, EventHandler callback)
		{
			if (!File.Exists(fileName))
				return null;
			if (!Git.IsInWorkingCopy(fileName))
				return null;
			
			string git = Git.FindGit();
			if (git == null)
				return null;
			
			return new BaseVersionChangeWatcher(fileName, GetBlobHashAsync(git, fileName).Result, callback);
		}
	}
	
	class BaseVersionChangeWatcher : IDisposable
	{
		EventHandler callback;
		FileName fileName;
		string hash;
		RepoChangeWatcher watcher;
		
		public BaseVersionChangeWatcher(FileName fileName, string hash, EventHandler callback)
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
			string newHash = GitVersionProvider.GetBlobHashAsync(Git.FindGit(), fileName).Result;
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
