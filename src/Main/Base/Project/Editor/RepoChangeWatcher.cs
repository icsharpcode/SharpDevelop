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
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// A helper class that can be used to register an action to be called
	/// when the contents of a directory change.
	/// The action is called only once if there were multiple changes,
	/// and the call may wait until SharpDevelop gets the focus.
	/// If multiple actions are registered for the same directory, the underlying
	/// FileSystemWatcher will be reused.
	/// This class is usually used to watch a .svn or .git directory.
	/// </summary>
	public class RepoChangeWatcher
	{
		static readonly Dictionary<string, RepoChangeWatcher> watchers
			= new Dictionary<string, RepoChangeWatcher>(StringComparer.OrdinalIgnoreCase);
		
		Action actions;
		FileSystemWatcher watcher;
		
		RepoChangeWatcher(string repositoryRoot)
		{
			this.watcher = new FileSystemWatcher(repositoryRoot);
			
			watcher.SynchronizingObject = SD.MainThread.SynchronizingObject;
			
			SD.Workbench.MainWindow.Activated += MainWindowActivated;
			
			watcher.Created += FileChanged;
			watcher.Deleted += FileChanged;
			watcher.Changed += FileChanged;
			watcher.Renamed += FileChanged;
			
	//			watcher.IncludeSubdirectories = true;
			watcher.EnableRaisingEvents = true;
		}
	
		void MainWindowActivated(object sender, EventArgs e)
		{
			if (alreadyCalled) {
				alreadyCalled = false;
				// thread-safety: copy delegate reference into local variable
				var actions = this.actions;
				if (actions != null)
					actions();
			}
		}
		
		bool alreadyCalled;
		
		void FileChanged(object sender, FileSystemEventArgs e)
		{
			if (!alreadyCalled) {
				if (e.Name.EndsWith(".lock", StringComparison.OrdinalIgnoreCase))
					return;
				alreadyCalled = true;
				LoggingService.Info(e.Name + " changed!" + e.ChangeType);
				if (SD.Workbench.IsActiveWindow) {
					SD.MainThread.CallLater(
						TimeSpan.FromSeconds(2),
						() => { MainWindowActivated(this, EventArgs.Empty); }
					);
				}
			}
		}
		
		public static RepoChangeWatcher AddWatch(string repositoryRoot, Action action)
		{
			RepoChangeWatcher watcher;
			lock (watchers) {
				if (!watchers.TryGetValue(repositoryRoot, out watcher)) {
					watcher = new RepoChangeWatcher(repositoryRoot);
					watchers.Add(repositoryRoot, watcher);
				}
				
				watcher.actions += action;
			}
			return watcher;
		}
		
		bool disposed;
		
		public void ReleaseWatch(Action action)
		{
			lock (watchers) {
				actions -= action;
				if (actions == null && !disposed) {
					SD.Workbench.MainWindow.Activated -= MainWindowActivated;
					watchers.Remove(watcher.Path);
					this.watcher.Dispose();
					disposed = true;
				}
			}
		}
	}
}
