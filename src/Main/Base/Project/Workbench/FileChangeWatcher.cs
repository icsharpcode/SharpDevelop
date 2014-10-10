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

namespace ICSharpCode.SharpDevelop.Workbench
{
	public sealed class FileChangeWatcher : IDisposable
	{
		public static bool DetectExternalChangesOption {
			get {
				return PropertyService.Get("SharpDevelop.FileChangeWatcher.DetectExternalChanges", true);
			}
			set {
				SD.MainThread.VerifyAccess();
				PropertyService.Set("SharpDevelop.FileChangeWatcher.DetectExternalChanges", value);
				foreach (FileChangeWatcher watcher in activeWatchers) {
					watcher.SetWatcher();
				}
			}
		}
		
		public static bool AutoLoadExternalChangesOption {
			get {
				return PropertyService.Get("SharpDevelop.FileChangeWatcher.AutoLoadExternalChanges", true);
			}
			set {
				PropertyService.Set("SharpDevelop.FileChangeWatcher.AutoLoadExternalChanges", value);
			}
		}
		
		static HashSet<FileChangeWatcher> activeWatchers = new HashSet<FileChangeWatcher>();
		
		static int globalDisableCount;
		
		public static bool AllChangeWatchersDisabled {
			get { return globalDisableCount > 0; }
		}
		
		public static void DisableAllChangeWatchers()
		{
			SD.MainThread.VerifyAccess();
			globalDisableCount++;
			foreach (FileChangeWatcher w in activeWatchers)
				w.SetWatcher();
			Project.ProjectChangeWatcher.OnAllChangeWatchersDisabledChanged();
		}
		
		public static void EnableAllChangeWatchers()
		{
			SD.MainThread.VerifyAccess();
			if (globalDisableCount == 0)
				throw new InvalidOperationException();
			globalDisableCount--;
			foreach (FileChangeWatcher w in activeWatchers)
				w.SetWatcher();
			Project.ProjectChangeWatcher.OnAllChangeWatchersDisabledChanged();
		}
		
		FileSystemWatcher watcher;
		bool wasChangedExternally = false;
		OpenedFile file;
		
		public FileChangeWatcher(OpenedFile file)
		{
			if (file == null)
				throw new ArgumentNullException("file");
			this.file = file;
			SD.Workbench.MainWindow.Activated += MainForm_Activated;
			file.FileNameChanged += file_FileNameChanged;
			activeWatchers.Add(this);
			SetWatcher();
		}

		void file_FileNameChanged(object sender, EventArgs e)
		{
			SetWatcher();
		}
		
		public void Dispose()
		{
			SD.MainThread.VerifyAccess();
			activeWatchers.Remove(this);
			if (file != null) {
				SD.Workbench.MainWindow.Activated -= MainForm_Activated;
				file.FileNameChanged -= file_FileNameChanged;
				file = null;
			}
			if (watcher != null) {
				watcher.Dispose();
				watcher = null;
			}
		}
		
		bool enabled = true;
		
		public bool Enabled {
			get { return enabled; }
			set {
				enabled = value;
				SetWatcher();
			}
		}
		
		void SetWatcher()
		{
			SD.MainThread.VerifyAccess();
			
			if (watcher != null) {
				watcher.EnableRaisingEvents = false;
			}
			
			if (!enabled)
				return;
			if (globalDisableCount > 0)
				return;
			if (!DetectExternalChangesOption)
				return;
			
			string fileName = file.FileName;
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
		
		void OnFileChangedEvent(object sender, FileSystemEventArgs e)
		{
			if (file == null)
				return;
			LoggingService.Debug("File " + file.FileName + " was changed externally: " + e.ChangeType);
			if (!wasChangedExternally) {
				wasChangedExternally = true;
				if (SD.Workbench.IsActiveWindow) {
					// delay reloading message a bit, prevents showing two messages
					// when the file changes twice in quick succession; and prevents
					// trying to reload the file while it is still being written
					SD.MainThread.CallLater(
						TimeSpan.FromSeconds(0.5),
						delegate { MainForm_Activated(this, EventArgs.Empty); } );
				}
			}
		}
		
		void MainForm_Activated(object sender, EventArgs e)
		{
			if (wasChangedExternally) {
				wasChangedExternally = false;
				
				if (file == null)
					return;
				
				string fileName = file.FileName;
				if (!File.Exists(fileName))
					return;

				if (AutoLoadExternalChangesOption && !file.IsDirty) {
					if (File.Exists(fileName)) {
						file.ReloadFromDisk();
					}
				} else {
					QueueFileForReloadDialog(file);
				}
			}
		}
		
		#region Reload Queue
		static readonly HashSet<OpenedFile> queue = new HashSet<OpenedFile>();
		static volatile bool currentlyReloading;
		
		static void QueueFileForReloadDialog(OpenedFile file)
		{
			if (file == null)
				throw new ArgumentNullException("file");
			lock (queue) {
				queue.Add(file);
			}
		}
		
		public static void AskForReload()
		{
			if (currentlyReloading) return;
			currentlyReloading = true;
			try {
				lock (queue) {
					foreach (var file in queue) {
						string fileName = file.FileName;
						if (!File.Exists(fileName))
							continue;
						string message = StringParser.Parse(
							"${res:ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorDisplayBinding.FileAlteredMessage}",
							new StringTagPair("File", Path.GetFullPath(fileName))
						);
						if (SD.MessageService.AskQuestion(message, StringParser.Parse("${res:MainWindow.DialogName}"))) {
							if (File.Exists(fileName)) {
								file.ReloadFromDisk();
							}
						} else {
							file.MakeDirty();
						}
					}
					queue.Clear();
				}
			} finally {
				currentlyReloading = false;
			}
		}

		public static void CancelReloadQueue()
		{
			lock (queue) {
				queue.Clear();
			}
		}
		#endregion
	}
}
