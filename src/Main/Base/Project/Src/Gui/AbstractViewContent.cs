// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Provides a default implementation for the IViewContent interface.
	/// It provides a files collection that, by default, automatically registers the view with the
	/// files added to it.
	/// Several properties have default implementation that depend on the contents of the files collection,
	/// e.g. IsDirty is true when at least one of the files in the collection is dirty.
	/// To support the changed event, this class registers event handlers with the members of the files collection.
	/// 
	/// When used with an empty Files collection, IsViewOnly will return true and this class can be used as a base class
	/// for view contents not using files.
	/// </summary>
	public abstract class AbstractViewContent : IViewContent
	{
		/// <summary>
		/// Create a new AbstractViewContent instance.
		/// </summary>
		protected AbstractViewContent()
		{
			secondaryViewContentCollection = new SecondaryViewContentCollection(this);
			InitFiles();
		}
		
		/// <summary>
		/// Create a new AbstractViewContent instance with the specified primary file.
		/// </summary>
		protected AbstractViewContent(OpenedFile file) : this()
		{
			if (file == null)
				throw new ArgumentNullException("file");
			this.Files.Add(file);
		}
		
		public abstract object Control {
			get;
		}
		
		public virtual object InitiallyFocusedControl {
			get { return null; }
		}
		
		IWorkbenchWindow workbenchWindow;
		
		IWorkbenchWindow IViewContent.WorkbenchWindow {
			get { return workbenchWindow; }
			set {
				if (workbenchWindow != value) {
					workbenchWindow = value;
					OnWorkbenchWindowChanged();
				}
			}
		}
		
		public IWorkbenchWindow WorkbenchWindow {
			get { return workbenchWindow; }
		}
		
		protected virtual void OnWorkbenchWindowChanged()
		{
		}
		
		string tabPageText = "TabPageText";
		
		public event EventHandler TabPageTextChanged;
		
		/// <summary>
		/// Gets/Sets the title of the current tab page.
		/// This value will be passed through the string parser before being displayed.
		/// </summary>
		public string TabPageText {
			get { return tabPageText; }
			set {
				if (tabPageText != value) {
					tabPageText = value;
					
					if (TabPageTextChanged != null) {
						TabPageTextChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		#region Secondary view content support
		sealed class SecondaryViewContentCollection : ICollection<IViewContent>
		{
			readonly AbstractViewContent parent;
			readonly List<IViewContent> list = new List<IViewContent>();
			
			public SecondaryViewContentCollection(AbstractViewContent parent)
			{
				this.parent = parent;
			}
			
			public int Count {
				get { return list.Count; }
			}
			
			public bool IsReadOnly {
				get { return false; }
			}
			
			public void Add(IViewContent item)
			{
				if (item == null) {
					throw new ArgumentNullException("item");
				}
				if (item.WorkbenchWindow != null && item.WorkbenchWindow != parent.WorkbenchWindow) {
					throw new ArgumentException("The view content already is displayed in another workbench window.");
				}
				list.Add(item);
				if (parent.workbenchWindow != null) {
					parent.workbenchWindow.ViewContents.Add(item);
				}
			}
			
			public void Clear()
			{
				if (parent.workbenchWindow != null) {
					foreach (IViewContent vc in list) {
						parent.workbenchWindow.ViewContents.Remove(vc);
					}
				}
				list.Clear();
			}
			
			public bool Contains(IViewContent item)
			{
				return list.Contains(item);
			}
			
			public void CopyTo(IViewContent[] array, int arrayIndex)
			{
				list.CopyTo(array, arrayIndex);
			}
			
			public bool Remove(IViewContent item)
			{
				if (list.Remove(item)) {
					if (parent.workbenchWindow != null) {
						parent.workbenchWindow.ViewContents.Remove(item);
					}
					return true;
				} else {
					return false;
				}
			}
			
			public IEnumerator<IViewContent> GetEnumerator()
			{
				return list.GetEnumerator();
			}
			
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return list.GetEnumerator();
			}
		}
		
		readonly SecondaryViewContentCollection secondaryViewContentCollection;
		
		/// <summary>
		/// Gets the collection that stores the secondary view contents.
		/// </summary>
		public virtual ICollection<IViewContent> SecondaryViewContents {
			get {
				return secondaryViewContentCollection;
			}
		}
		
		/// <summary>
		/// Gets switching without a Save/Load cycle for <paramref name="file"/> is supported
		/// when switching from this view content to <paramref name="newView"/>.
		/// </summary>
		public virtual bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			return newView == this;
		}
		
		/// <summary>
		/// Gets switching without a Save/Load cycle for <paramref name="file"/> is supported
		/// when switching from <paramref name="oldView"/> to this view content.
		/// </summary>
		public virtual bool SupportsSwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			return oldView == this;
		}
		
		/// <summary>
		/// Executes an action before switching from this view content to the new view content.
		/// </summary>
		public virtual void SwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
		}
		
		/// <summary>
		/// Executes an action before switching from the old view content to this view content.
		/// </summary>
		public virtual void SwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
		}
		#endregion
		
		#region Files
		FilesCollection files;
		ReadOnlyCollection<OpenedFile> filesReadonly;
		
		void InitFiles()
		{
			files = new FilesCollection(this);
			filesReadonly = new ReadOnlyCollection<OpenedFile>(files);
		}
		
		protected Collection<OpenedFile> Files {
			get { return files; }
		}
		
		IList<OpenedFile> IViewContent.Files {
			get { return filesReadonly; }
		}
		
		/// <summary>
		/// Gets the primary file being edited. Might return null if no file is edited.
		/// </summary>
		public virtual OpenedFile PrimaryFile {
			get {
				WorkbenchSingleton.AssertMainThread();
				if (files.Count != 0)
					return files[0];
				else
					return null;
			}
		}
		
		/// <summary>
		/// Gets the name of the primary file being edited. Might return null if no file is edited.
		/// </summary>
		public virtual FileName PrimaryFileName {
			get {
				OpenedFile file = PrimaryFile;
				if (file != null)
					return file.FileName;
				else
					return null;
			}
		}
		
		protected bool AutomaticallyRegisterViewOnFiles = true;
		
		void RegisterFileEventHandlers(OpenedFile newItem)
		{
			newItem.FileNameChanged += OnFileNameChanged;
			newItem.IsDirtyChanged += OnIsDirtyChanged;
			if (AutomaticallyRegisterViewOnFiles) {
				newItem.RegisterView(this);
			}
			OnIsDirtyChanged(null, EventArgs.Empty); // re-evaluate this.IsDirty after changing the file collection
		}
		
		void UnregisterFileEventHandlers(OpenedFile oldItem)
		{
			oldItem.FileNameChanged -= OnFileNameChanged;
			oldItem.IsDirtyChanged -= OnIsDirtyChanged;
			if (AutomaticallyRegisterViewOnFiles) {
				oldItem.UnregisterView(this);
			}
			OnIsDirtyChanged(null, EventArgs.Empty); // re-evaluate this.IsDirty after changing the file collection
		}
		
		void OnFileNameChanged(object sender, EventArgs e)
		{
			OnFileNameChanged((OpenedFile)sender);
			if (titleName == null && files.Count > 0 && sender == files[0]) {
				OnTitleNameChanged(EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// Is called when the file name of a file opened in this view content changes.
		/// </summary>
		protected virtual void OnFileNameChanged(OpenedFile file)
		{
		}
		
		private sealed class FilesCollection : Collection<OpenedFile>
		{
			AbstractViewContent parent;
			
			public FilesCollection(AbstractViewContent parent)
			{
				this.parent = parent;
			}
			
			protected override void InsertItem(int index, OpenedFile item)
			{
				base.InsertItem(index, item);
				parent.RegisterFileEventHandlers(item);
			}
			
			protected override void SetItem(int index, OpenedFile item)
			{
				parent.UnregisterFileEventHandlers(this[index]);
				base.SetItem(index, item);
				parent.RegisterFileEventHandlers(item);
			}
			
			protected override void RemoveItem(int index)
			{
				parent.UnregisterFileEventHandlers(this[index]);
				base.RemoveItem(index);
			}
			
			protected override void ClearItems()
			{
				foreach (OpenedFile item in this) {
					parent.UnregisterFileEventHandlers(item);
				}
				base.ClearItems();
			}
		}
		#endregion
		
		#region TitleName
		public event EventHandler TitleNameChanged;
		
		void OnTitleNameChanged(EventArgs e)
		{
			if (TitleNameChanged != null) {
				TitleNameChanged(this, e);
			}
		}
		
		string titleName;
		LanguageDependentExtension titleNameLocalizeExtension;
		
		string IViewContent.TitleName {
			get {
				if (titleName != null) {
					return titleName;
				} else if (files.Count > 0) {
					return Path.GetFileName(files[0].FileName);
				} else {
					return "[Default Title]";
				}
			}
		}
		
		public string TitleName {
			get { return titleName; }
			set {
				if (titleNameLocalizeExtension != null) {
					titleNameLocalizeExtension.PropertyChanged -= OnTitleNameLocalizationChanged;
					titleNameLocalizeExtension = null;
				}
				if (titleName != value) {
					titleName = value;
					OnTitleNameChanged(EventArgs.Empty);
				}
			}
		}
		
		/// <summary>
		/// Sets a localized title that will update automatically when the language changes.
		/// </summary>
		/// <param name="text">The input to the string parser which will localize title.</param>
		protected void SetLocalizedTitle(string text)
		{
			titleNameLocalizeExtension = new StringParseExtension(text) { UsesAccessors = false };
			titleNameLocalizeExtension.PropertyChanged += OnTitleNameLocalizationChanged;
			OnTitleNameLocalizationChanged(null, null);
		}
		
		void OnTitleNameLocalizationChanged(object sender, EventArgs e)
		{
			string value = titleNameLocalizeExtension.Value;
			if (titleName != value) {
				titleName = value;
				OnTitleNameChanged(EventArgs.Empty);
			}
		}
		#endregion


		#region InfoTip
		public event EventHandler InfoTipChanged;

		void OnInfoTipChanged()
		{
			if (InfoTipChanged != null)
			{
				InfoTipChanged(this, EventArgs.Empty);
			}
		}

		string infoTip;
		LanguageDependentExtension infoTipLocalizeExtension;

		string IViewContent.InfoTip {
			get {
				if (infoTip != null)
					return infoTip;
				else if (files.Count > 0)
					return files[0].FileName;
				else
					return null;
			}
		}

		public string InfoTip
		{
			get { return infoTip; }
			set
			{
				if (infoTipLocalizeExtension != null)
				{
					infoTipLocalizeExtension.PropertyChanged -= OnInfoTipLocalizationChanged;
					infoTipLocalizeExtension = null;
				}
				if (infoTip != value)
				{
					infoTip = value;
					OnInfoTipChanged();
				}
			}
		}

		/// <summary>
		/// Sets a localized info tip that will update automatically when the language changes.
		/// </summary>
		/// <param name="text">The input to the string parser which will localize info tip.</param>
		protected void SetLocalizedInfoTip(string text)
		{
			infoTipLocalizeExtension = new StringParseExtension(text) { UsesAccessors = false };
			infoTipLocalizeExtension.PropertyChanged += OnInfoTipLocalizationChanged;
			OnInfoTipLocalizationChanged(null, null);
		}

		void OnInfoTipLocalizationChanged(object sender, EventArgs e)
		{
			string value = infoTipLocalizeExtension.Value;
			if (infoTip != value)
			{
				infoTip = value;
				OnInfoTipChanged();
			}
		}
		#endregion
		
		#region IDisposable
		public event EventHandler Disposed;
		
		bool isDisposed;
		
		public bool IsDisposed {
			get { return isDisposed; }
		}
		
		public virtual void Dispose()
		{
			workbenchWindow = null;
			UnregisterOnActiveViewContentChanged();
			if (AutomaticallyRegisterViewOnFiles) {
				this.Files.Clear();
			}
			isDisposed = true;
			if (Disposed != null) {
				Disposed(this, EventArgs.Empty);
			}
		}
		#endregion
		
		#region IsDirty
		bool IsDirtyInternal {
			get {
				foreach (OpenedFile file in this.Files) {
					if (file.IsDirty)
						return true;
				}
				return false;
			}
		}
		
		bool isDirty;
		
		public virtual bool IsDirty {
			get { return isDirty; }
		}
		
		void OnIsDirtyChanged(object sender, EventArgs e)
		{
			bool newIsDirty = IsDirtyInternal;
			if (newIsDirty != isDirty) {
				isDirty = newIsDirty;
				RaiseIsDirtyChanged();
			}
		}
		
		/// <summary>
		/// Raise the IsDirtyChanged event. Call this method only if you have overridden the IsDirty property
		/// to implement your own handling of IsDirty.
		/// </summary>
		protected void RaiseIsDirtyChanged()
		{
			if (IsDirtyChanged != null)
				IsDirtyChanged(this, EventArgs.Empty);
		}
		
		public event EventHandler IsDirtyChanged;
		#endregion
		
		#region IsActiveViewContent
		EventHandler isActiveViewContentChanged;
		bool registeredOnViewContentChange;
		bool wasActiveViewContent;
		
		/// <summary>
		/// Gets if this view content is the active view content.
		/// </summary>
		protected bool IsActiveViewContent {
			get { return WorkbenchSingleton.Workbench != null && WorkbenchSingleton.Workbench.ActiveViewContent == this; }
		}
		
		/// <summary>
		/// Is raised when the value of the IsActiveViewContent property changes.
		/// </summary>
		protected event EventHandler IsActiveViewContentChanged {
			add {
				if (!registeredOnViewContentChange) {
					// register WorkbenchSingleton.Workbench.ActiveViewContentChanged only on demand
					wasActiveViewContent = IsActiveViewContent;
					// null check is required to support running in unit test mode
					if (WorkbenchSingleton.Workbench != null) {
						WorkbenchSingleton.Workbench.ActiveViewContentChanged += OnActiveViewContentChanged;
					}
					registeredOnViewContentChange = true;
				}
				isActiveViewContentChanged += value;
			}
			remove {
				isActiveViewContentChanged -= value;
			}
		}
		
		void UnregisterOnActiveViewContentChanged()
		{
			if (registeredOnViewContentChange) {
				// null check is required to support running in unit test mode
				if (WorkbenchSingleton.Workbench != null) {
					WorkbenchSingleton.Workbench.ActiveViewContentChanged -= OnActiveViewContentChanged;
				}
				registeredOnViewContentChange = false;
			}
		}
		
		void OnActiveViewContentChanged(object sender, EventArgs e)
		{
			bool isActiveViewContent = IsActiveViewContent;
			if (isActiveViewContent != wasActiveViewContent) {
				wasActiveViewContent = isActiveViewContent;
				if (isActiveViewContentChanged != null)
					isActiveViewContentChanged(this, e);
			}
		}
		#endregion
		
		#region IServiceProvider
		ServiceContainer services = new ServiceContainer();
		
		public object GetService(Type serviceType)
		{
			object obj = services.GetService(serviceType);
			if (obj == null) {
				if (serviceType.IsInstanceOfType(this))
					return this;
			}
			return obj;
		}
		
		public ServiceContainer Services {
			get { return services; }
		}
		
		#endregion
		
		public virtual void Save(OpenedFile file, Stream stream)
		{
		}
		
		public virtual void Load(OpenedFile file, Stream stream)
		{
		}
		
		public virtual INavigationPoint BuildNavPoint()
		{
			return null;
		}
		
		/// <summary>
		/// Gets if the view content is read-only (can be saved only when choosing another file name).
		/// </summary>
		public virtual bool IsReadOnly {
			get { return false; }
		}
		
		/// <summary>
		/// Gets if the view content is view-only (cannot be saved at all).
		/// </summary>
		public virtual bool IsViewOnly {
			get { return Files.Count == 0; }
		}
		
		public virtual bool CloseWithSolution {
			get {
				var fileName = this.PrimaryFileName;
				return fileName == null
					|| Project.ProjectService.OpenSolution.FindProjectContainingFile(fileName) != null;
			}
		}
	}
}
