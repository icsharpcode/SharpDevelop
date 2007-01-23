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
	/// <summary>
	/// Represents an opened file.
	/// </summary>
	public sealed class OpenedFile : ICanBeDirty
	{
		bool isConnectedWithFileService = true;
		string fileName;
		
		IViewContent currentView;
		bool isUntitled;
		List<IViewContent> registeredViews = new List<IViewContent>();
		
		/// <summary>
		/// holds unsaved file content in memory when view containing the file was closed but no other view
		/// activated
		/// </summary>
		byte[] fileData;
		
		internal OpenedFile(string fileName)
		{
			this.fileName = fileName;
			isUntitled = false;
		}
		
		internal OpenedFile(byte[] fileData)
		{
			this.fileName = null;
			this.fileData = fileData;
			isUntitled = true;
			MakeDirty();
		}
		
		/// <summary>
		/// Creates a dummy opened file instance. Use for unit tests only!
		/// Do not call UnregisterView or CloseIfAllViewsClosed on the dummy opened file!!!
		/// </summary>
		public static OpenedFile CreateDummyOpenedFile(string name, bool isUntitled)
		{
			if (isUntitled) {
				OpenedFile f = new OpenedFile(new byte[0]);
				f.isConnectedWithFileService = false;
				f.FileName = name;
				return f;
			} else {
				OpenedFile f = new OpenedFile(name);
				f.isConnectedWithFileService = false;
				return f;
			}
		}
		
		public bool IsUntitled {
			get { return isUntitled; }
		}
		
		public string FileName {
			get { return fileName; }
			set {
				if (fileName == value) return;
				
				value = FileUtility.NormalizePath(value);
				
				if (fileName != value) {
					if (isConnectedWithFileService) {
						FileService.OpenedFileFileNameChange(this, fileName, value);
					}
					fileName = value;
					
					if (FileNameChanged != null) {
						FileNameChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		public event EventHandler FileNameChanged;
		
		/// <summary>
		/// Gets the list of view contents registered with this opened file.
		/// </summary>
		public IList<IViewContent> RegisteredViewContents {
			get { return registeredViews.AsReadOnly(); }
		}
		
		/// <summary>
		/// Gets the view content that currently edits this file.
		/// If there are multiple view contents registered, this returns the view content that was last
		/// active. The property might return null even if view contents are registered if the last active
		/// content was closed. In that case, the file is stored in-memory and loaded when one of the
		/// registered view contents becomes active.
		/// </summary>
		public IViewContent CurrentView {
			get { return currentView; }
		}
		
		/// <summary>
		/// Opens the file for reading.
		/// </summary>
		public Stream OpenRead()
		{
			if (fileData != null) {
				return new MemoryStream(fileData, false);
			} else {
				return new FileStream(fileName, FileMode.Open, FileAccess.Read);
			}
		}
		
		public void SaveToDisk(string newFileName)
		{
			this.FileName = newFileName;
			isUntitled = false;
			SaveToDisk();
		}
		
		public void SaveToDisk()
		{
			if (isUntitled)
				throw new InvalidOperationException("Cannot save an untitled file to disk!");
			
			/*
			 * TODO: Reimplement "safe saving"
			if (document.TextEditorProperties.CreateBackupCopy) {
				try {
					if (File.Exists(fileName)) {
						string backupName = fileName + ".bak";
						File.Copy(fileName, backupName, true);
					}
				} catch (Exception) {
	//
	//				MessageService.ShowError(e, "Can not create backup copy of " + fileName);
				}
			}
			 */
			using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
				if (currentView != null) {
					SaveCurrentViewToStream(fs);
				} else {
					fs.Write(fileData, 0, fileData.Length);
				}
			}
			IsDirty = false;
		}
		
//		/// <summary>
//		/// Called before saving the current view. This event is raised both when saving to disk and to memory (for switching between views).
//		/// </summary>
//		public event EventHandler SavingCurrentView;
//
//		/// <summary>
//		/// Called after saving the current view. This event is raised both when saving to disk and to memory (for switching between views).
//		/// </summary>
//		public event EventHandler SavedCurrentView;
		
		void SaveCurrentViewToStream(Stream stream)
		{
//			if (SavingCurrentView != null)
//				SavingCurrentView(this, EventArgs.Empty);
			currentView.Save(this, stream);
//			if (SavedCurrentView != null)
//				SavedCurrentView(this, EventArgs.Empty);
		}
		
		void SaveCurrentView()
		{
			using (MemoryStream memoryStream = new MemoryStream()) {
				SaveCurrentViewToStream(memoryStream);
				fileData = memoryStream.ToArray();
			}
		}
		
		public void RegisterView(IViewContent view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			Debug.Assert(!registeredViews.Contains(view));
			
			registeredViews.Add(view);
			
			view.ViewActivated += SwitchedToView;
			#if DEBUG
			view.Disposed += ViewDisposed;
			#endif
		}
		
		public void UnregisterView(IViewContent view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			Debug.Assert(registeredViews.Contains(view));
			
			view.ViewActivated -= SwitchedToView;
			#if DEBUG
			view.Disposed -= ViewDisposed;
			#endif
			
			registeredViews.Remove(view);
			if (registeredViews.Count > 0) {
				if (currentView == view) {
					SaveCurrentView();
					currentView = null;
				}
			} else {
				// all views to the file were closed
				if (isConnectedWithFileService) {
					FileService.OpenedFileClosed(this);
				}
			}
		}
		
		internal void CloseIfAllViewsClosed()
		{
			if (registeredViews.Count == 0) {
				if (isConnectedWithFileService) {
					FileService.OpenedFileClosed(this);
				}
			}
		}
		
		#if DEBUG
		void ViewDisposed(object sender, EventArgs e)
		{
			Debug.Fail("View was disposed while still registered with OpenedFile!");
		}
		#endif
		
		/// <summary>
		/// Forces initialization of the specified view.
		/// </summary>
		public void ForceInitializeView(IViewContent view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			Debug.Assert(registeredViews.Contains(view));
			
			if (currentView != view) {
				if (currentView == null) {
					SwitchedToView(view, EventArgs.Empty);
				} else {
					try {
						inLoadOperation = true;
						using (Stream sourceStream = OpenRead()) {
							view.Load(this, sourceStream);
						}
					} finally {
						inLoadOperation = false;
					}
				}
			}
		}
		
		void SwitchedToView(object sender, EventArgs e)
		{
			IViewContent newView = (IViewContent)sender;
			if (currentView != null) {
				if (newView.SupportsSwitchToThisWithoutSaveLoad(this, currentView)
				    || currentView.SupportsSwitchFromThisWithoutSaveLoad(this, newView))
				{
					// switch without Save/Load
					currentView.SwitchFromThisWithoutSaveLoad(this, newView);
					newView.SwitchToThisWithoutSaveLoad(this, currentView);
					
					currentView = newView;
					return;
				}
			}
			if (currentView != null) {
				SaveCurrentView();
			}
			try {
				inLoadOperation = true;
				using (Stream sourceStream = OpenRead()) {
					currentView = newView;
					fileData = null;
					newView.Load(this, sourceStream);
				}
			} finally {
				inLoadOperation = false;
			}
		}
		
		public void ReloadFromDisk()
		{
			fileData = null;
			if (currentView != null) {
				try {
					inLoadOperation = true;
					using (Stream sourceStream = OpenRead()) {
						currentView.Load(this, sourceStream);
					}
				} finally {
					inLoadOperation = false;
				}
			}
		}
		
		/*
		internal void ChangeView(IViewContent newView)
		{
			if (currentView != null && currentView != newView) {
				SaveCurrentView();
			}
			using (Stream sourceStream = OpenRead()) {
				currentView = newView;
				fileData = null;
				newView.Load(this, sourceStream);
			}
		}
		 */
		
		bool isDirty;
		public event EventHandler IsDirtyChanged;
		
		public bool IsDirty {
			get { return isDirty;}
			set {
				if (isDirty != value) {
					isDirty = value;
					
					if (IsDirtyChanged != null) {
						IsDirtyChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		bool inLoadOperation;
		
		/// <summary>
		/// Marks the file as dirty if it currently is not in a load operation.
		/// </summary>
		public void MakeDirty()
		{
			if (!inLoadOperation) {
				this.IsDirty = true;
			}
		}
		
		#region FileChangeWatcher
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
		#endregion
	}
}
