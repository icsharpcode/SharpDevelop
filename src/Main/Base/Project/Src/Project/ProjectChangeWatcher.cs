// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public sealed class ProjectChangeWatcher : IDisposable
	{
		FileSystemWatcher watcher;
		string fileName;
		bool enabled = true;
		
		public ProjectChangeWatcher(string fileName)
		{
			this.fileName = fileName;
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
			
			if (!enabled)
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
		
		bool wasChangedExternally;
		
		void OnFileChangedEvent(object sender, FileSystemEventArgs e)
		{
			LoggingService.Debug("Solution was changed externally: " + e.ChangeType);
			if (!wasChangedExternally) {
				wasChangedExternally = true;
				if (WorkbenchSingleton.Workbench.IsActiveWindow) {
					// delay reloading message a bit, prevents showing two messages
					// when the file changes twice in quick succession; and prevents
					// trying to reload the file while it is still being written
					WorkbenchSingleton.CallLater(
						TimeSpan.FromSeconds(0.5),
						delegate { MainFormActivated(this, EventArgs.Empty); } );
				}
			}
		}
		
		void MainFormActivated(object sender, EventArgs e)
		{
			if (wasChangedExternally) {
				wasChangedExternally = false;
				
				if (MessageService.ShowCustomDialog(
					MessageService.DefaultMessageBoxTitle,
					"${res:ICSharpCode.SharpDevelop.Project.SolutionAlteredExternallyMessage}", 0, 1,
					"${res:ICSharpCode.SharpDevelop.Project.ReloadSolution}", "${res:ICSharpCode.SharpDevelop.Project.KeepOldSolution}")
				    == 0)
				{
					ProjectService.LoadSolution(ProjectService.OpenSolution.FileName);
				}
			}
		}
		
		bool disposed;
		
		public void Dispose()
		{
			if (!disposed) {
				WorkbenchSingleton.MainWindow.Activated -= MainFormActivated;
			}
			disposed = true;
		}
	}
}
