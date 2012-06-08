// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.SharpDevelop.Project
{
	public sealed class ProjectChangeWatcher : IProjectChangeWatcher
	{
		static readonly HashSet<ProjectChangeWatcher> activeWatchers = new HashSet<ProjectChangeWatcher>();
		
		internal static void OnAllChangeWatchersDisabledChanged()
		{
			foreach (ProjectChangeWatcher watcher in activeWatchers)
				watcher.SetWatcher();
		}
		
		FileSystemWatcher watcher;
		string fileName;
		bool enabled = true;

		public ProjectChangeWatcher(string fileName)
		{
			this.fileName = fileName;
			
			WorkbenchSingleton.AssertMainThread();
			activeWatchers.Add(this);
			
			WorkbenchSingleton.MainWindow.Activated += MainFormActivated;
		}
		
		public void Enable()
		{
			enabled = true;
			SetWatcher();
		}

		public void Disable()
		{
			enabled = false;
			SetWatcher();
		}

		public void Rename(string newFileName)
		{
			fileName = newFileName;
		}

		void SetWatcher()
		{
			WorkbenchSingleton.AssertMainThread();

			if (watcher != null) {
				watcher.EnableRaisingEvents = false;
			}

			if (!enabled || FileChangeWatcher.AllChangeWatchersDisabled)
				return;

			if (string.IsNullOrEmpty(fileName))
				return;
			if (FileUtility.IsUrl(fileName))
				return;
			if (!Path.IsPathRooted(fileName))
				return;

			try {
				if (watcher == null) {
					watcher = new FileSystemWatcher();
					if (WorkbenchSingleton.Workbench != null)
						watcher.SynchronizingObject = WorkbenchSingleton.Workbench.SynchronizingObject;
					watcher.Changed += OnFileChangedEvent;
					watcher.Created += OnFileChangedEvent;
					watcher.Renamed += OnFileChangedEvent;
				}
				watcher.Path = Path.GetDirectoryName(fileName);
				watcher.Filter = Path.GetFileName(fileName);
				watcher.EnableRaisingEvents = true;
			} catch (PlatformNotSupportedException) {
				if (watcher != null) {
					watcher.Dispose();
				}
				watcher = null;
			} catch (FileNotFoundException) {
				// can occur if directory was deleted externally
				if (watcher != null) {
					watcher.Dispose();
				}
				watcher = null;
			} catch (ArgumentException) {
				// can occur if parent directory was deleted externally
				if (watcher != null) {
					watcher.Dispose();
				}
				watcher = null;
			}
		}

		static bool wasChangedExternally;

		void OnFileChangedEvent(object sender, FileSystemEventArgs e)
		{
			LoggingService.Debug("Project file " + e.Name + " was changed externally: {1}" + e.ChangeType);
			if (!wasChangedExternally) {
				wasChangedExternally = true;
				if (WorkbenchSingleton.Workbench.IsActiveWindow) {
					// delay reloading message a bit, prevents showing two messages
					// when the file changes twice in quick succession; and prevents
					// trying to reload the file while it is still being written
					WorkbenchSingleton.CallLater(TimeSpan.FromSeconds(0.5), delegate { MainFormActivated(this, EventArgs.Empty); });
				}
			}
		}

		static bool showingMessageBox;
		
		static void MainFormActivated(object sender, EventArgs e)
		{
			if (wasChangedExternally && !showingMessageBox) {

				if (ProjectService.OpenSolution != null) {
					// Set wasChangedExternally=false only after the dialog is closed,
					// so that additional changes to the project while the dialog is open
					// don't cause it to appear twice.
					
					// The MainFormActivated event occurs when the dialog is closed before
					// we get a change to set wasChangedExternally=false, so we use 'showingMessageBox'
					// to prevent the dialog from appearing infititely.
					showingMessageBox = true;
					int result = MessageService.ShowCustomDialog(MessageService.DefaultMessageBoxTitle, "${res:ICSharpCode.SharpDevelop.Project.SolutionAlteredExternallyMessage}", 0, 1, "${res:ICSharpCode.SharpDevelop.Project.ReloadSolution}", "${res:ICSharpCode.SharpDevelop.Project.KeepOldSolution}", "${res:ICSharpCode.SharpDevelop.Project.CloseSolution}");
					showingMessageBox = false;
					wasChangedExternally = false;
					if (result == 0)
						ProjectService.LoadSolution(ProjectService.OpenSolution.FileName);
					else if (result == 2)
						new CloseSolution().Run();
				} else {
					wasChangedExternally = false;
				}
			}
		}

		bool disposed;

		public void Dispose()
		{
			WorkbenchSingleton.AssertMainThread();
			if (!disposed) {
				WorkbenchSingleton.MainWindow.Activated -= MainFormActivated;
				activeWatchers.Remove(this);
			}
			if (watcher != null) {
				watcher.Dispose();
				watcher = null;
			}
			disposed = true;
		}
	}
}
