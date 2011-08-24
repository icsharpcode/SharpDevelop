// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	internal sealed class FileChangeWatcher : IDisposable
	{
		public static bool DetectExternalChangesOption {
			get {
				return PropertyService.Get("SharpDevelop.FileChangeWatcher.DetectExternalChanges", true);
			}
			set {
				WorkbenchSingleton.AssertMainThread();
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
			WorkbenchSingleton.AssertMainThread();
			globalDisableCount++;
			foreach (FileChangeWatcher w in activeWatchers)
				w.SetWatcher();
			Project.ProjectChangeWatcher.OnAllChangeWatchersDisabledChanged();
		}
		
		public static void EnableAllChangeWatchers()
		{
			WorkbenchSingleton.AssertMainThread();
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
			WorkbenchSingleton.MainWindow.Activated += MainForm_Activated;
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
			WorkbenchSingleton.AssertMainThread();
			activeWatchers.Remove(this);
			if (file != null) {
				WorkbenchSingleton.MainWindow.Activated -= MainForm_Activated;
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
			WorkbenchSingleton.AssertMainThread();
			
			if (watcher != null) {
				watcher.EnableRaisingEvents = false;
			}
			
			if (!enabled)
				return;
			if (globalDisableCount > 0)
				return;
			if (DetectExternalChangesOption == false)
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
		
		void OnFileChangedEvent(object sender, FileSystemEventArgs e)
		{
			if (file == null)
				return;
			LoggingService.Debug("File " + file.FileName + " was changed externally: " + e.ChangeType);
			if (!wasChangedExternally) {
				wasChangedExternally = true;
				if (WorkbenchSingleton.Workbench.IsActiveWindow) {
					// delay reloading message a bit, prevents showing two messages
					// when the file changes twice in quick succession; and prevents
					// trying to reload the file while it is still being written
					WorkbenchSingleton.CallLater(
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
				
				string message = StringParser.Parse(
					"${res:ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorDisplayBinding.FileAlteredMessage}",
					new StringTagPair("File", Path.GetFullPath(fileName))
				);
				if ((AutoLoadExternalChangesOption && file.IsDirty == false)
				    || MessageService.AskQuestion(message, StringParser.Parse("${res:MainWindow.DialogName}")))
				{
					if (File.Exists(fileName)) {
						file.ReloadFromDisk();
					}
				} else {
					file.MakeDirty();
				}
			}
		}
	}
}
