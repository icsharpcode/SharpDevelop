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

using System.Windows.Threading;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project.Commands;
using ICSharpCode.SharpDevelop.Workbench;

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
		DateTime lastWriteTime;

		public ProjectChangeWatcher(string fileName)
		{
			this.fileName = fileName;
			
			SD.MainThread.VerifyAccess();
			activeWatchers.Add(this);
			UpdateLastWriteTime();
			
			SD.Workbench.MainWindow.Activated += MainFormActivated;
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
		
		void UpdateLastWriteTime()
		{
			// Save current last write time attribute
			FileInfo fileInfo = new FileInfo(fileName);
			if (fileInfo.Exists) {
				lastWriteTime = fileInfo.LastWriteTimeUtc;
			}
		}
		
		bool LastWriteTimeHasChanged()
		{
			// Save current last write time attribute
			FileInfo fileInfo = new FileInfo(fileName);
			if (fileInfo.Exists) {
				return lastWriteTime != fileInfo.LastWriteTimeUtc;
			}

			return true; // File might have been renamed/deleted?
		}

		void SetWatcher()
		{
			SD.MainThread.VerifyAccess();

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
					watcher.SynchronizingObject = SD.MainThread.SynchronizingObject;
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
			// Ignore this event, if LastWriteTime has changed to same value as before (= no real change in file)
			if ((e.ChangeType == WatcherChangeTypes.Changed) && !LastWriteTimeHasChanged()) {
				LoggingService.DebugFormatted("Attributes of project file {0} have been set externally ({1}), but no relevant changes detected.", e.Name, e.ChangeType);
				return;
			}
			
			LoggingService.DebugFormatted("Project file {0} was changed externally: {1}", e.Name, e.ChangeType);
			UpdateLastWriteTime();
			if (!wasChangedExternally) {
				wasChangedExternally = true;
				if (SD.Workbench.IsActiveWindow) {
					// delay reloading message a bit, prevents showing two messages
					// when the file changes twice in quick succession; and prevents
					// trying to reload the file while it is still being written
					SD.MainThread.CallLater(TimeSpan.FromSeconds(0.5), delegate { MainFormActivated(); });
				}
			}
		}

		static bool showingMessageBox;
		
		static void MainFormActivated(object sender, EventArgs e)
		{
			// delay the event so that we don't interrupt the user if he's trying to close SharpDevelop
			SD.MainThread.InvokeAsyncAndForget(MainFormActivated, DispatcherPriority.Background);
		}
		
		static void MainFormActivated()
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
						SD.ProjectService.OpenSolutionOrProject(ProjectService.OpenSolution.FileName);
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
			SD.MainThread.VerifyAccess();
			if (!disposed) {
				SD.Workbench.MainWindow.Activated -= MainFormActivated;
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
