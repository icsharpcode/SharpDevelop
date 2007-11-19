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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	internal sealed class FileChangeWatcher //: IDisposable
	{
		/*FileSystemWatcher watcher;
		bool wasChangedExternally = false;
		string fileName;
		AbstractViewContent viewContent;
		
		public FileChangeWatcher(AbstractViewContent viewContent)
		{
			this.viewContent = viewContent;
			WorkbenchSingleton.MainForm.Activated += GotFocusEvent;
		}
		 */
		
		public static bool DetectExternalChangesOption {
			get {
				return PropertyService.Get("SharpDevelop.FileChangeWatcher.DetectExternalChanges", true);
			}
			set {
				PropertyService.Set("SharpDevelop.FileChangeWatcher.DetectExternalChanges", value);
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
		
		/*
		public void Dispose()
		{
			WorkbenchSingleton.MainForm.Activated -= GotFocusEvent;
			if (watcher != null) {
				watcher.Dispose();
			}
		}
		
		public void Disable()
		{
			if (watcher != null) {
				watcher.EnableRaisingEvents = false;
			}
		}
		
		public void SetWatcher(string fileName)
		{
			this.fileName = fileName;
			if (DetectExternalChangesOption == false)
				return;
			try {
				if (this.watcher == null) {
					this.watcher = new FileSystemWatcher();
					this.watcher.SynchronizingObject = WorkbenchSingleton.MainForm;
					this.watcher.Changed += new FileSystemEventHandler(this.OnFileChangedEvent);
				} else {
					this.watcher.EnableRaisingEvents = false;
				}
				this.watcher.Path = Path.GetDirectoryName(fileName);
				this.watcher.Filter = Path.GetFileName(fileName);
				this.watcher.NotifyFilter = NotifyFilters.LastWrite;
				this.watcher.EnableRaisingEvents = true;
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
				if (ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench.IsActiveWindow) {
					// delay showing message a bit, prevents showing two messages
					// when the file changes twice in quick succession
					WorkbenchSingleton.SafeThreadAsyncCall(GotFocusEvent, this, EventArgs.Empty);
				}
			}
		}
		
		void GotFocusEvent(object sender, EventArgs e)
		{
			if (wasChangedExternally) {
				wasChangedExternally = false;
				
				string message = StringParser.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorDisplayBinding.FileAlteredMessage}", new string[,] {{"File", Path.GetFullPath(fileName)}});
				if ((AutoLoadExternalChangesOption && viewContent.IsDirty == false)
				    || MessageBox.Show(message,
				                       StringParser.Parse("${res:MainWindow.DialogName}"),
				                       MessageBoxButtons.YesNo,
				                       MessageBoxIcon.Question) == DialogResult.Yes)
				{
					if (File.Exists(fileName)) {
						viewContent.Load(fileName);
					}
				} else {
					viewContent.IsDirty = true;
				}
			}
		}
		 */
	}
}
