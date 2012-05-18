// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public abstract class OpenedFile : ICanBeDirty
	{
		protected IViewContent currentView;
		bool inLoadOperation;
		bool inSaveOperation;
		
		/// <summary>
		/// holds unsaved file content in memory when view containing the file was closed but no other view
		/// activated
		/// </summary>
		byte[] fileData;
		
		#region IsDirty
		bool isDirty;
		public event EventHandler IsDirtyChanged;
		
		/// <summary>
		/// Gets/sets if the file is has unsaved changes.
		/// </summary>
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
		
		/// <summary>
		/// Marks the file as dirty if it currently is not in a load operation.
		/// </summary>
		public virtual void MakeDirty()
		{
			if (!inLoadOperation) {
				this.IsDirty = true;
			}
		}
		#endregion
		
		bool isUntitled;
		
		/// <summary>
		/// Gets if the file is untitled. Untitled files show a "Save as" dialog when they are saved.
		/// </summary>
		public bool IsUntitled {
			get { return isUntitled; }
			protected set { isUntitled = value; }
		}
		
		FileName fileName;
		
		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		public FileName FileName {
			get { return fileName; }
			set {
				if (fileName != value) {
					ChangeFileName(value);
				}
			}
		}
		
		protected virtual void ChangeFileName(FileName newValue)
		{
			WorkbenchSingleton.AssertMainThread();
			
			fileName = newValue;
			
			if (FileNameChanged != null) {
				FileNameChanged(this, EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// Occurs when the file name has changed.
		/// </summary>
		public event EventHandler FileNameChanged;
		
		public abstract event EventHandler FileClosed;
		
		/// <summary>
		/// Use this method to save the file to disk using a new name.
		/// </summary>
		public void SaveToDisk(string newFileName)
		{
			this.FileName = new FileName(newFileName);
			this.IsUntitled = false;
			SaveToDisk();
		}
		
		public abstract void RegisterView(IViewContent view);
		public abstract void UnregisterView(IViewContent view);
		
		public virtual void CloseIfAllViewsClosed()
		{
		}
		
		/// <summary>
		/// Forces initialization of the specified view.
		/// </summary>
		public virtual void ForceInitializeView(IViewContent view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			
			try {
				if (currentView != view) {
					if (currentView == null) {
						SwitchedToView(view);
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
			} catch (Exception) {
				view.Dispose();
				throw;
			}
		}
		
		/// <summary>
		/// Gets the list of view contents registered with this opened file.
		/// </summary>
		public abstract IList<IViewContent> RegisteredViewContents {
			get;
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
		public virtual Stream OpenRead()
		{
			if (fileData != null) {
				return new MemoryStream(fileData, false);
			} else {
				return new FileStream(FileName, FileMode.Open, FileAccess.Read);
			}
		}
		
		/// <summary>
		/// Sets the internally stored data to the specified byte array.
		/// This method should only be used when there is no current view or by the
		/// current view.
		/// </summary>
		/// <remarks>
		/// Use this method to specify the initial file content if you use a OpenedFile instance
		/// for a file that doesn't exist on disk but should be automatically created when a view
		/// with the file is saved, e.g. for .resx files created by the forms designer.
		/// </remarks>
		public virtual void SetData(byte[] fileData)
		{
			if (fileData == null)
				throw new ArgumentNullException("fileData");
			if (inLoadOperation)
				throw new InvalidOperationException("SetData cannot be used while loading");
			if (inSaveOperation)
				throw new InvalidOperationException("SetData cannot be used while saving");
			
			this.fileData = fileData;
		}
		
		/// <summary>
		/// Save the file to disk using the current name.
		/// </summary>
		public virtual void SaveToDisk()
		{
			if (IsUntitled)
				throw new InvalidOperationException("Cannot save an untitled file to disk!");
			
			LoggingService.Debug("Save " + FileName);
			bool safeSaving = FileService.SaveUsingTemporaryFile && File.Exists(FileName);
			string saveAs = safeSaving ? FileName + ".bak" : FileName;
			using (FileStream fs = new FileStream(saveAs, FileMode.Create, FileAccess.Write)) {
				if (currentView != null) {
					SaveCurrentViewToStream(fs);
				} else {
					fs.Write(fileData, 0, fileData.Length);
				}
			}
			if (safeSaving) {
				DateTime creationTime = File.GetCreationTimeUtc(FileName);
				File.Delete(FileName);
				try {
					File.Move(saveAs, FileName);
				} catch (UnauthorizedAccessException) {
					// sometime File.Move raise exception (TortoiseSVN, Anti-vir ?)
					// try again after short delay
					System.Threading.Thread.Sleep(250);
					File.Move(saveAs, FileName);
				}
				File.SetCreationTimeUtc(FileName, creationTime);
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
			inSaveOperation = true;
			try {
				currentView.Save(this, stream);
			} finally {
				inSaveOperation = false;
			}
//			if (SavedCurrentView != null)
//				SavedCurrentView(this, EventArgs.Empty);
		}
		
		protected void SaveCurrentView()
		{
			using (MemoryStream memoryStream = new MemoryStream()) {
				SaveCurrentViewToStream(memoryStream);
				fileData = memoryStream.ToArray();
			}
		}
		
		
		public void SwitchedToView(IViewContent newView)
		{
			if (newView == null)
				throw new ArgumentNullException("newView");
			if (currentView == newView)
				return;
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
				SaveCurrentView();
			}
			try {
				inLoadOperation = true;
				Properties memento = GetMemento(newView);
				using (Stream sourceStream = OpenRead()) {
					IViewContent oldView = currentView;
					bool success = false;
					try {
						currentView = newView;
						// don't reset fileData if the file is untitled, because OpenRead() wouldn't be able to read it otherwise
						if (this.IsUntitled == false)
							fileData = null;
						newView.Load(this, sourceStream);
						success = true;
					} finally {
						// Use finally instead of catch+rethrow so that the debugger
						// breaks at the original crash location.
						if (!success) {
							// stay with old view in case of exceptions
							currentView = oldView;
						}
					}
				}
				RestoreMemento(newView, memento);
			} finally {
				inLoadOperation = false;
			}
		}
		
		public virtual void ReloadFromDisk()
		{
			var r = FileUtility.ObservedLoad(ReloadFromDiskInternal, FileName);
			if (r == FileOperationResult.Failed) {
				if (currentView != null && currentView.WorkbenchWindow != null) {
					currentView.WorkbenchWindow.CloseWindow(true);
				}
			}
		}
		
		void ReloadFromDiskInternal()
		{
			fileData = null;
			if (currentView != null) {
				try {
					inLoadOperation = true;
					Properties memento = GetMemento(currentView);
					using (Stream sourceStream = OpenRead()) {
						currentView.Load(this, sourceStream);
					}
					IsDirty = false;
					RestoreMemento(currentView, memento);
				} finally {
					inLoadOperation = false;
				}
			}
		}
		
		static Properties GetMemento(IViewContent viewContent)
		{
			IMementoCapable mementoCapable = viewContent as IMementoCapable;
			if (mementoCapable == null) {
				return null;
			} else {
				return mementoCapable.CreateMemento();
			}
		}
		
		static void RestoreMemento(IViewContent viewContent, Properties memento)
		{
			if (memento != null) {
				((IMementoCapable)viewContent).SetMemento(memento);
			}
		}
	}
	
	sealed class FileServiceOpenedFile : OpenedFile
	{
		List<IViewContent> registeredViews = new List<IViewContent>();
		FileChangeWatcher fileChangeWatcher;
		
		protected override void ChangeFileName(FileName newValue)
		{
			FileService.OpenedFileFileNameChange(this, this.FileName, newValue);
			base.ChangeFileName(newValue);
		}
		
		internal FileServiceOpenedFile(FileName fileName)
		{
			this.FileName = fileName;
			IsUntitled = false;
			fileChangeWatcher = new FileChangeWatcher(this);
		}
		
		internal FileServiceOpenedFile(byte[] fileData)
		{
			this.FileName = null;
			SetData(fileData);
			IsUntitled = true;
			MakeDirty();
			fileChangeWatcher = new FileChangeWatcher(this);
		}
		
		/// <summary>
		/// Gets the list of view contents registered with this opened file.
		/// </summary>
		public override IList<IViewContent> RegisteredViewContents {
			get { return registeredViews.AsReadOnly(); }
		}
		
		public override void ForceInitializeView(IViewContent view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			if (!registeredViews.Contains(view))
				throw new ArgumentException("registeredViews must contain view");
			
			base.ForceInitializeView(view);
		}
		
		public override void RegisterView(IViewContent view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			if (registeredViews.Contains(view))
				throw new ArgumentException("registeredViews already contains view");
			
			registeredViews.Add(view);
			
			if (WorkbenchSingleton.Workbench != null) {
				WorkbenchSingleton.Workbench.ActiveViewContentChanged += WorkbenchActiveViewContentChanged;
				if (WorkbenchSingleton.Workbench.ActiveViewContent == view) {
					SwitchedToView(view);
				}
			}
			#if DEBUG
			view.Disposed += ViewDisposed;
			#endif
		}
		
		public override void UnregisterView(IViewContent view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			Debug.Assert(registeredViews.Contains(view));
			
			if (WorkbenchSingleton.Workbench != null) {
				WorkbenchSingleton.Workbench.ActiveViewContentChanged -= WorkbenchActiveViewContentChanged;
			}
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
				CloseIfAllViewsClosed();
			}
		}
		
		public override void CloseIfAllViewsClosed()
		{
			if (registeredViews.Count == 0) {
				bool wasDirty = this.IsDirty;
				FileService.OpenedFileClosed(this);
				
				FileClosed.RaiseEvent(this, EventArgs.Empty);
				
				if (fileChangeWatcher != null) {
					fileChangeWatcher.Dispose();
					fileChangeWatcher = null;
				}
				
				if (wasDirty) {
					// We discarded some information when closing the file,
					// so we need to re-parse it.
					if (File.Exists(this.FileName))
						ParserService.BeginParse(this.FileName);
					else
						ParserService.ClearParseInformation(this.FileName);
				}
			}
		}
		
		#if DEBUG
		void ViewDisposed(object sender, EventArgs e)
		{
			Debug.Fail("View was disposed while still registered with OpenedFile!");
		}
		#endif
		
		void WorkbenchActiveViewContentChanged(object sender, EventArgs e)
		{
			IViewContent newView = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (!registeredViews.Contains(newView))
				return;
			
			SwitchedToView(newView);
		}
		
		public override void SaveToDisk()
		{
			try {
				if (fileChangeWatcher != null)
					fileChangeWatcher.Enabled = false;
				base.SaveToDisk();
			} finally {
				if (fileChangeWatcher != null)
					fileChangeWatcher.Enabled = true;
			}
		}
		
		public override event EventHandler FileClosed;
	}
}
