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
using System.Diagnostics;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// Options for use with <see cref="OpenedFile.GetModel"/>
	/// </summary>
	[Flags]
	public enum GetModelOptions
	{
		None = 0,
		/// <summary>
		/// Return stale models without reloading.
		/// </summary>
		AllowStale = 1,
		/// <summary>
		/// Do not load any models:
		/// Returns null if the model is not already loaded, or if it is stale and the AllowStale option isn't in use.
		/// </summary>
		DoNotLoad = 2,
		/// <summary>
		/// Allows showing modal dialogs during the GetModel() call.
		/// For example, the previous model might ask the user details on how to save.
		/// </summary>
		AllowUserInteraction = 4,
	}
	
	/// <summary>
	/// Option that control how <see cref="OpenedFile.ReplaceModel"/> handles the dirty flag.
	/// </summary>
	public enum ReplaceModelMode
	{
		/// <summary>
		/// The new model is marked as dirty; and any other models are marked as stale.
		/// </summary>
		SetAsDirty,
		/// <summary>
		/// The new model is marked as valid; the status of any other models is unchanged.
		/// </summary>
		SetAsValid,
		/// <summary>
		/// The new model is marked as dirty or valid (depending on whether the OpenedFile was previously dirty),
		/// the previously dirty model (if any) is marked as stale, and any other models are unchanged.
		/// This mode is intended for use in <see cref="IFileModelProvider{T}.Save"/> implementations.
		/// </summary>
		TransferDirty
	}
	
	/// <summary>
	/// Represents an opened file.
	/// </summary>
	/// <remarks>
	/// This class uses reference counting: the <c>AddReference()</c> and <c>ReleaseReference()</c>
	/// methods are used to track an explicit reference count.
	/// When the last reference is released, the opened file will be disposed.
	/// View contents must maintain a reference to their opened files
	/// (this usually happens via the AbstractViewContent.Files collection).
	/// </remarks>
	public class OpenedFile : ICanBeDirty
	{
		abstract class ModelEntry
		{
			public bool IsStale;
			
			public abstract object Provider { get; }
			
			public abstract void Save(OpenedFile file, FileSaveOptions options);
			public abstract void SaveCopyAs(OpenedFile file, FileName outputFileName, FileSaveOptions options);
			public abstract void NotifyRename(OpenedFile file, FileName oldName, FileName newName);
			public abstract void NotifyStale(OpenedFile file);
			public abstract void NotifyLoaded(OpenedFile file);
			public abstract void NotifyUnloaded(OpenedFile file);
			public abstract bool NeedsSaveForLoadInto<T>(IFileModelProvider<T> modelProvider) where T : class;
		}
		class ModelEntry<T> : ModelEntry where T : class
		{
			readonly IFileModelProvider<T> provider;
			public T Model;
			
			public ModelEntry(IFileModelProvider<T> provider, T model)
			{
				Debug.Assert(provider != null);
				Debug.Assert(model != null);
				this.provider = provider;
				this.Model = model;
			}
			
			public override object Provider { get { return provider; } }
			
			public override void Save(OpenedFile file, FileSaveOptions options)
			{
				provider.Save(file, Model, options);
			}
			
			public override void SaveCopyAs(OpenedFile file, FileName outputFileName, FileSaveOptions options)
			{
				provider.SaveCopyAs(file, Model, outputFileName, options);
			}
			
			public override void NotifyRename(OpenedFile file, FileName oldName, FileName newName)
			{
				provider.NotifyRename(file, Model, oldName, newName);
			}
			
			public override void NotifyStale(OpenedFile file)
			{
				provider.NotifyStale(file, Model);
			}
			
			public override void NotifyLoaded(OpenedFile file)
			{
				provider.NotifyLoaded(file, Model);
			}
			
			public override void NotifyUnloaded(OpenedFile file)
			{
				provider.NotifyUnloaded(file, Model);
			}
			
			public override bool NeedsSaveForLoadInto<U>(IFileModelProvider<U> modelProvider)
			{
				return modelProvider.CanLoadFrom(provider);
			}
		}
		
		readonly List<ModelEntry> entries = new List<ModelEntry>();
		ModelEntry dirtyEntry;
		bool preventLoading;
		
		#region IsDirty implementation
		bool isDirty;
		public event EventHandler IsDirtyChanged;
		
		/// <summary>
		/// Gets/sets if the file is has unsaved changes.
		/// </summary>
		public bool IsDirty {
			get { return isDirty; }
			private set {
				if (isDirty != value) {
					isDirty = value;
					
					if (IsDirtyChanged != null) {
						IsDirtyChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		#endregion
		
		#region FileName
		/// <summary>
		/// Gets if the file is untitled. Untitled files show a "Save as" dialog when they are saved.
		/// </summary>
		public bool IsUntitled {
			get {
				return fileName.ToString().StartsWith("untitled:", StringComparison.Ordinal);
			}
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
		
		/// <summary>
		/// Occurs when the file name has changed.
		/// </summary>
		public event EventHandler FileNameChanged;
		
		protected virtual void ChangeFileName(FileName newValue)
		{
			SD.MainThread.VerifyAccess();
			
			FileName oldName = fileName;
			fileName = newValue;
			
			foreach (var entry in entries)
				entry.NotifyRename(this, oldName, newValue);
			if (FileNameChanged != null) {
				FileNameChanged(this, EventArgs.Empty);
			}
		}
		#endregion
		
		#region ReloadFromDisk
		/// <summary>
		/// This method sets all models to 'stale', causing the file to be re-loaded from disk
		/// on the next GetModel() call.
		/// </summary>
		/// <exception cref="InvalidOperationException">The file is untitled.</exception>
		public void ReloadFromDisk()
		{
			CheckDisposed();
			if (IsUntitled)
				throw new InvalidOperationException("Cannot reload an untitled file from disk.");
			// First set all entries to stale, then call NotifyStale().
			foreach (var entry in entries)
				entry.IsStale = true;
			foreach (var entry in entries)
				entry.NotifyStale(this);
			this.IsDirty = false;
		}
		#endregion
		
		#region SaveToDisk
		/// <summary>
		/// Saves the file to disk.
		/// </summary>
		/// <remarks>If the file is saved successfully, the dirty flag will be cleared (the dirty model becomes valid instead).</remarks>
		/// <exception cref="InvalidOperationException">The file is untitled.</exception>
		public void SaveToDisk(FileSaveOptions options)
		{
			CheckDisposed();
			if (IsUntitled)
				throw new InvalidOperationException("Cannot save an untitled file to disk.");
			SaveToDisk(this.FileName, options);
		}
		
		/// <summary>
		/// Changes the file name, and saves the file to disk.
		/// </summary>
		/// <remarks>If the file is saved successfully, the dirty flag will be cleared (the dirty model becomes valid instead).</remarks>
		public virtual void SaveToDisk(FileName fileName, FileSaveOptions options)
		{
			CheckDisposed();
			
			bool safeSaving = SD.FileService.SaveUsingTemporaryFile && SD.FileSystem.FileExists(fileName);
			FileName saveAs = safeSaving ? FileName.Create(fileName + ".bak") : fileName;
			SaveCopyTo(saveAs, options);
			if (safeSaving) {
				DateTime creationTime = File.GetCreationTimeUtc(fileName);
				File.Delete(fileName);
				try {
					File.Move(saveAs, fileName);
				} catch (UnauthorizedAccessException) {
					// sometime File.Move raise exception (TortoiseSVN, Anti-vir ?)
					// try again after short delay
					System.Threading.Thread.Sleep(250);
					File.Move(saveAs, fileName);
				}
				File.SetCreationTimeUtc(fileName, creationTime);
			}
			
			dirtyEntry = null;
			this.FileName = fileName;
			this.IsDirty = false;
		}
		
		/// <summary>
		/// Saves a copy of the file to disk. Does not change the name of the OpenedFile to the specified file name, and does not reset the dirty flag.
		/// </summary>
		public virtual void SaveCopyAs(FileName fileName, FileSaveOptions options)
		{
			CheckDisposed();
			SaveCopyTo(fileName, options);
		}
		
		void SaveCopyTo(FileName outputFileName, FileSaveOptions options)
		{
			preventLoading = true;
			try {
				var entry = PickValidEntry();
				if (entry != null) {
					entry.SaveCopyAs(this, outputFileName, options);
				} else if (outputFileName != this.FileName) {
					SD.FileSystem.CopyFile(this.FileName, outputFileName, true);
				}
			} finally {
				preventLoading = false;
			}
		}
		
		#endregion
		
		#region GetModel
		ModelEntry<T> GetEntry<T>(IFileModelProvider<T> modelProvider) where T : class
		{
			CheckDisposed();
			foreach (var entry in entries) {
				if (object.Equals(entry.Provider, modelProvider))
					return (ModelEntry<T>)entry;
			}
			return null;
		}
		
		/// <summary>
		/// Retrieves a file model, loading it if necessary.
		/// </summary>
		/// <param name="modelProvider">The model provider for the desired model type. Built-in model providers can be found in the <see cref="FileModels"/> class.</param>
		/// <param name="options">Options that control how</param>
		/// <returns>The model instance, or possibly <c>null</c> if <c>GetModelOptions.DoNotLoad</c> is in use.</returns>
		/// <exception cref="System.IO.IOException">Error loading the file.</exception>
		/// <exception cref="FormatException">Cannot construct the model because the underyling data is in an invalid format.</exception>
		public T GetModel<T>(IFileModelProvider<T> modelProvider, GetModelOptions options = GetModelOptions.None) where T : class
		{
			if (modelProvider == null)
				throw new ArgumentNullException("modelProvider");
			if (preventLoading && (options & GetModelOptions.DoNotLoad) == 0)
				throw new InvalidOperationException("GetModel() operations that potentially load models are not permitted at this point. (to retrieve existing models, use the DoNotLoad option)");
			ModelEntry<T> entry = GetEntry(modelProvider);
			// Return existing model if possible:
			if (entry != null && ((options & GetModelOptions.AllowStale) != 0 || !entry.IsStale))
				return entry.Model;
			// If we aren't allowed to load, just return null:
			if ((options & GetModelOptions.DoNotLoad) != 0)
				return null;
			Debug.Assert(!preventLoading);
			preventLoading = true;
			try {
				// Before we can load the requested model, save the dirty model (if necessary):
				while (dirtyEntry != null && dirtyEntry.NeedsSaveForLoadInto(modelProvider)) {
					var saveOptions = FileSaveOptions.SaveForGetModel;
					if ((options & GetModelOptions.AllowUserInteraction) != 0)
						saveOptions |= FileSaveOptions.AllowUserInteraction;
					dirtyEntry.Save(this, saveOptions);
					// re-fetch entry because it's possible that it was created/replaced/unloaded
					entry = GetEntry(modelProvider);
					// if the entry was made valid by the save operation, return it directly
					if (entry != null && !entry.IsStale)
						return entry.Model;
				}
			} finally {
				preventLoading = false;
			}
			// Load the model. Note that we do allow (and expect) recursive loads at this point.
			T model = modelProvider.Load(this);
			// re-fetch entry because it's possible that it was created/replaced/unloaded (normally this shouldn't happen, but let's be on the safe side)
			entry = GetEntry(modelProvider);
			if (entry == null) {
				// No entry for the model provider exists; we need to create a new one.
				entry = new ModelEntry<T>(modelProvider, model);
				entries.Add(entry);
			} else if (entry.Model == model) {
				// The existing stale model was reused
				entry.IsStale = false;
			} else {
				// The model is being replaced
				entry.NotifyUnloaded(this);
				entry.Model = model;
				entry.IsStale = false;
			}
			return model;
		}
		#endregion
		
		#region MakeDirty
		ModelEntry PickValidEntry()
		{
			if (dirtyEntry != null)
				return dirtyEntry; // prefer dirty entry
			foreach (var entry in entries) {
				if (!entry.IsStale) {
					return entry;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Takes a valid model and marks it as dirty.
		/// If no valid model exists, this method has no effect.
		/// If multiple valid models exist, one is picked at random.
		/// </summary>
		/// <remarks>
		/// This method is used when SharpDevelop detects that the file was changed externally;
		/// but the user does not want to reload the file. 
		/// </remarks>
		public void MakeDirty()
		{
			var entry = PickValidEntry();
			if (entry != null) {
				dirtyEntry = entry;
				this.IsDirty = true;
			}
		}
		
		/// <summary>
		/// Sets the model associated with the specified model provider to be dirty.
		/// All other models are marked as stale. If another model was previously dirty, those earlier changes will be lost.
		/// </summary>
		/// <remarks>
		/// This method is usually called by the model provider. In a well-designed model, any change to the model
		/// should result in the model provider calling MakeDirty() automatically.
		/// </remarks>
		public void MakeDirty<T>(IFileModelProvider<T> modelProvider) where T : class
		{
			if (modelProvider == null)
				throw new ArgumentNullException("modelProvider");
			var entry = GetEntry(modelProvider);
			if (entry == null)
				throw new ArgumentException("There is no model loaded for the specified model provider.");
			entry.IsStale = false;
			dirtyEntry = entry;
			MarkAllAsStaleExcept(entry);
			this.IsDirty = true;
		}
		
		void MarkAllAsStaleExcept(ModelEntry entry)
		{
			foreach (var otherEntry in entries) {
				if (otherEntry != entry) {
					otherEntry.IsStale = true;
				}
			}
			// Raise events after all state is updated:
			foreach (var otherEntry in entries) {
				if (otherEntry != entry) {
					otherEntry.NotifyStale(this);
				}
			}
		}
		#endregion
		
		#region UnloadModel
		/// <summary>
		/// Unloads the model associated with the specified model provider.
		/// Unloading the dirty model will cause changes to be lost.
		/// </summary>
		public void UnloadModel<T>(IFileModelProvider<T> modelProvider) where T : class
		{
			if (modelProvider == null)
				throw new ArgumentNullException("modelProvider");
			var entry = GetEntry(modelProvider);
			if (entry != null) {
				if (dirtyEntry == entry) {
					dirtyEntry = null;
				}
				entries.Remove(entry);
				entry.NotifyUnloaded(this);
			}
		}
		#endregion
		
		#region ReplaceModel
		/// <summary>
		/// Replaces the model associated with the specified model provider with a different instance.
		/// </summary>
		/// <param name="modelProvider">The model provider for the model type.</param>
		/// <param name="model">The new model instance.</param>
		/// <param name="mode">Specifies how the dirty flag is handled during the replacement.
		/// By default, the new model is marked as dirty and all other models are marked as stale.
		/// In <see cref="IFileModelProvider{T}.Save"/> implementations, you should use <see cref="ReplaceModelMode.TransferDirty"/> instead.</param>
		public void ReplaceModel<T>(IFileModelProvider<T> modelProvider, T model, ReplaceModelMode mode = ReplaceModelMode.SetAsDirty) where T : class
		{
			if (modelProvider == null)
				throw new ArgumentNullException("modelProvider");
			var entry = GetEntry(modelProvider);
			if (entry == null) {
				entry = new ModelEntry<T>(modelProvider, model);
			} else {
				if (entry.Model != model) {
					entry.NotifyUnloaded(this);
					entry.Model = model;
					entry.NotifyLoaded(this);
				}
				entry.IsStale = false;
			}
			switch (mode) {
				case ReplaceModelMode.SetAsDirty:
					dirtyEntry = entry;
					MarkAllAsStaleExcept(entry);
					this.IsDirty = true;
					break;
				case ReplaceModelMode.SetAsValid:
					if (dirtyEntry == entry) {
						dirtyEntry = null;
						this.IsDirty = false;
					}
					break;
				case ReplaceModelMode.TransferDirty:
					if (dirtyEntry != null)
						dirtyEntry = entry;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		#endregion
		
		#region Reference Counting
		int referenceCount = 1;
		
		void CheckDisposed()
		{
			if (referenceCount <= 0)
				throw new ObjectDisposedException("OpenedFile");
		}
		
		/// <summary>
		/// Gets the reference count of the OpenedFile.
		/// </summary>
		public int ReferenceCount {
			get { return referenceCount; }
		}
		
		public void AddReference()
		{
			CheckDisposed();
			referenceCount++;
		}
		
		public void ReleaseReference()
		{
			CheckDisposed();
			if (--referenceCount == 0) {
				UnloadFile();
			}
		}
		
		/// <summary>
		/// Unloads the file, this method is called once after the reference count has reached zero.
		/// </summary>
		protected virtual void UnloadFile()
		{
			Debug.Assert(referenceCount == 0);
			foreach (var entry in entries) {
				entry.NotifyUnloaded(this);
			}
			// Free memory consumed by models even if the OpenedFile is leaked somewhere
			entries.Clear();
			dirtyEntry = null;
		}
		#endregion
	}
	
	/*
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
		
		
		/// <summary>
		/// Use this method to save the file to disk using a new name.
		/// </summary>
		public void SaveToDisk(FileName newFileName)
		{
			this.FileName = newFileName;
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
			
			bool success = false;
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
				success = true;
			} finally {
				// Only in case of exceptions:
				// (try-finally with bool is better than try-catch-rethrow because it causes the debugger to stop
				// at the original error location, not at the rethrow)
				if (!success) {
					view.Dispose();
				}
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
				return new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
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
			bool safeSaving = SD.FileService.SaveUsingTemporaryFile && File.Exists(FileName);
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
			IMementoCapable mementoCapable = viewContent.GetService<IMementoCapable>();
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
	*/
}
