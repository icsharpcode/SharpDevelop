// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor
{
	public interface IDocumentVersionProvider
	{
		/// <summary>
		/// Provides the BASE-Version for a file. This can be either the file saved
		/// to disk or a base version provided by any VCS.
		/// </summary>
		Stream OpenBaseVersion(string fileName);
		
		IDisposable WatchBaseVersionChanges(string fileName, EventHandler callback);
	}
	
	public class VersioningServices
	{
		public static readonly VersioningServices Instance = new VersioningServices();
		
		List<IDocumentVersionProvider> baseVersionProviders;
		
		public List<IDocumentVersionProvider> DocumentVersionProviders {
			get {
				if (baseVersionProviders == null)
					baseVersionProviders = AddInTree.BuildItems<IDocumentVersionProvider>("/Workspace/DocumentVersionProviders", this, false);
				
				return baseVersionProviders;
			}
		}
	}
	
	public class RepoChangeWatcher
	{
		static readonly Dictionary<string, RepoChangeWatcher> watchers
			= new Dictionary<string, RepoChangeWatcher>(StringComparer.OrdinalIgnoreCase);
		
		Action actions;
		FileSystemWatcher watcher;
		
		RepoChangeWatcher(string repositoryRoot)
		{
			this.watcher = new FileSystemWatcher(repositoryRoot);
			
			if (WorkbenchSingleton.Workbench != null)
				watcher.SynchronizingObject = WorkbenchSingleton.Workbench.SynchronizingObject;
			
			WorkbenchSingleton.MainWindow.Activated += MainWindowActivated;
			
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
				alreadyCalled = true;
				LoggingService.Info(e.Name + " changed!" + e.ChangeType);
				if (WorkbenchSingleton.Workbench.IsActiveWindow) {
					WorkbenchSingleton.CallLater(
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
					WorkbenchSingleton.MainWindow.Activated -= MainWindowActivated;
					watchers.Remove(watcher.Path);
					this.watcher.Dispose();
					disposed = true;
				}
			}
		}
	}
}
