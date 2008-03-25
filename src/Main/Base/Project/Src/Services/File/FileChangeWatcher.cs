// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
		
		static Set<FileChangeWatcher> activeWatchers = new Set<FileChangeWatcher>();
		
		FileSystemWatcher watcher;
		bool wasChangedExternally = false;
		OpenedFile file;
		
		public FileChangeWatcher(OpenedFile file)
		{
			if (file == null)
				throw new ArgumentNullException("file");
			WorkbenchSingleton.AssertMainThread();
			this.file = file;
			WorkbenchSingleton.MainForm.Activated += MainForm_Activated;
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
				WorkbenchSingleton.MainForm.Activated -= MainForm_Activated;
				file.FileNameChanged -= file_FileNameChanged;
				file = null;
			}
			if (watcher != null) {
				watcher.Dispose();
				watcher = null;
			}
		}
		
		void SetWatcher()
		{
			if (watcher != null) {
				watcher.EnableRaisingEvents = false;
			}
			
			if (DetectExternalChangesOption == false)
				return;
			
			string fileName = file.FileName;
			if (FileUtility.IsUrl(fileName))
				return;
			if (!Path.IsPathRooted(fileName))
				return;
			
			try {
				if (watcher == null) {
					watcher = new FileSystemWatcher();
					watcher.SynchronizingObject = WorkbenchSingleton.MainForm;
					watcher.Changed += new FileSystemEventHandler(this.OnFileChangedEvent);
				}
				watcher.Path = Path.GetDirectoryName(fileName);
				watcher.Filter = Path.GetFileName(fileName);
				watcher.NotifyFilter = NotifyFilters.LastWrite;
				watcher.EnableRaisingEvents = true;
			} catch (PlatformNotSupportedException) {
				if (watcher != null) {
					watcher.Dispose();
				}
				watcher = null;
			}
		}
		
		void OnFileChangedEvent(object sender, FileSystemEventArgs e)
		{
			if (e.ChangeType != WatcherChangeTypes.Deleted) {
				wasChangedExternally = true;
				if (WorkbenchSingleton.Workbench.IsActiveWindow) {
					// delay showing message a bit, prevents showing two messages
					// when the file changes twice in quick succession
					WorkbenchSingleton.SafeThreadAsyncCall(MainForm_Activated, this, EventArgs.Empty);
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
				
				string message = StringParser.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorDisplayBinding.FileAlteredMessage}", new string[,] {{"File", Path.GetFullPath(fileName)}});
				if ((AutoLoadExternalChangesOption && file.IsDirty == false)
				    || MessageBox.Show(message,
				                       StringParser.Parse("${res:MainWindow.DialogName}"),
				                       MessageBoxButtons.YesNo,
				                       MessageBoxIcon.Question) == DialogResult.Yes)
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
