// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using AvalonDock;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.SharpDevelop.Gui
{
	sealed class AvalonWorkbenchWindow : DocumentContent, IWorkbenchWindow, IOwnerState
	{
		readonly static string contextMenuPath = "/SharpDevelop/Workbench/OpenFileTab/ContextMenu";
		
		public bool IsDisposed { get { return false; } }
		
		#region IOwnerState
		[Flags]
		public enum OpenFileTabStates {
			Nothing             = 0,
			FileDirty           = 1,
			FileReadOnly        = 2,
			FileUntitled        = 4
		}
		
		public System.Enum InternalState {
			get {
				IViewContent content = this.ActiveViewContent;
				OpenFileTabStates state = OpenFileTabStates.Nothing;
				if (content != null) {
					if (content.IsDirty)
						state |= OpenFileTabStates.FileDirty;
					if (content.IsReadOnly)
						state |= OpenFileTabStates.FileReadOnly;
					if (content.PrimaryFile != null && content.PrimaryFile.IsUntitled)
						state |= OpenFileTabStates.FileUntitled;
				}
				return state;
			}
		}
		#endregion
		
		TabControl viewTabControl;
		
		/// <summary>
		/// The current view content which is shown inside this window.
		/// </summary>
		public IViewContent ActiveViewContent {
			get {
				WorkbenchSingleton.DebugAssertMainThread();
				if (viewTabControl != null && viewTabControl.SelectedIndex >= 0 && viewTabControl.SelectedIndex < ViewContents.Count) {
					return ViewContents[viewTabControl.SelectedIndex];
				} else if (ViewContents.Count == 1) {
					return ViewContents[0];
				} else {
					return null;
				}
			}
			set {
				int pos = ViewContents.IndexOf(value);
				if (pos < 0)
					throw new ArgumentException();
				SwitchView(pos);
			}
		}
		
		public event EventHandler ActiveViewContentChanged;
		
		IViewContent oldActiveViewContent;
		
		void UpdateActiveViewContent()
		{
			UpdateTitle();
			IViewContent newActiveViewContent = this.ActiveViewContent;
			if (oldActiveViewContent != newActiveViewContent && ActiveViewContentChanged != null) {
				ActiveViewContentChanged(this, EventArgs.Empty);
			}
			oldActiveViewContent = newActiveViewContent;
			CommandManager.InvalidateRequerySuggested();
		}
		
		sealed class ViewContentCollection : Collection<IViewContent>
		{
			readonly AvalonWorkbenchWindow window;
			
			internal ViewContentCollection(AvalonWorkbenchWindow window)
			{
				this.window = window;
			}
			
			protected override void ClearItems()
			{
				foreach (IViewContent vc in this) {
					window.UnregisterContent(vc);
				}
				
				base.ClearItems();
				window.ClearContent();
				window.UpdateActiveViewContent();
			}
			
			protected override void InsertItem(int index, IViewContent item)
			{
				base.InsertItem(index, item);
				
				window.RegisterNewContent(item);
				
				if (Count == 1) {
					window.SetContent(item.Content);
				} else {
					if (Count == 2) {
						window.CreateViewTabControl();
						IViewContent oldItem = this[0];
						if (oldItem == item) oldItem = this[1];
						
						TabItem oldPage = new TabItem();
						oldPage.Header = StringParser.Parse(oldItem.TabPageText);
						oldPage.SetContent(oldItem.Content);
						window.viewTabControl.Items.Add(oldPage);
					}
					
					TabItem newPage = new TabItem();
					newPage.Header = StringParser.Parse(item.TabPageText);
					newPage.SetContent(item.Content);
					
					window.viewTabControl.Items.Insert(index, newPage);
				}
				window.UpdateActiveViewContent();
			}
			
			protected override void RemoveItem(int index)
			{
				window.UnregisterContent(this[index]);
				
				base.RemoveItem(index);
				
				if (Count < 2) {
					window.ClearContent();
					if (Count == 1) {
						window.SetContent(this[0].Content);
					}
				} else {
					window.viewTabControl.Items.RemoveAt(index);
				}
				window.UpdateActiveViewContent();
			}
			
			protected override void SetItem(int index, IViewContent item)
			{
				window.UnregisterContent(this[index]);
				
				base.SetItem(index, item);
				
				window.RegisterNewContent(item);
				
				if (Count == 1) {
					window.ClearContent();
					window.SetContent(item.Content);
				} else {
					TabItem page = (TabItem)window.viewTabControl.Items[index];
					page.SetContent(item.Content);
					page.Header = StringParser.Parse(item.TabPageText);
				}
				window.UpdateActiveViewContent();
			}
		}
		
		readonly ViewContentCollection viewContents;
		
		public IList<IViewContent> ViewContents {
			get { return viewContents; }
		}
		
		/// <summary>
		/// Gets whether any contained view content has changed
		/// since the last save/load operation.
		/// </summary>
		public bool IsDirty {
			get { return this.ViewContents.Any(vc => vc.IsDirty); }
		}
		
		public void SwitchView(int viewNumber)
		{
			if (viewTabControl != null) {
				this.viewTabControl.SelectedIndex = viewNumber;
			}
		}
		
		public void SelectWindow()
		{
			dockLayout.DockingManager.Show(this);
		}
		
		AvalonDockLayout dockLayout;
		
		public AvalonWorkbenchWindow(AvalonDockLayout dockLayout)
		{
			if (dockLayout == null)
				throw new ArgumentNullException("dockLayout");
			this.dockLayout = dockLayout;
			viewContents = new ViewContentCollection(this);
			
			OnTitleNameChanged(this, EventArgs.Empty);
			this.ContextMenu = MenuService.CreateContextMenu(this, contextMenuPath);
		}
		
		void Dispose()
		{
			// DetachContent must be called before the controls are disposed
			List<IViewContent> viewContents = this.ViewContents.ToList();
			this.ViewContents.Clear();
			viewContents.ForEach(vc => vc.Dispose());
		}
		
		private void CreateViewTabControl()
		{
			if (viewTabControl == null) {
				viewTabControl = new TabControl();
				viewTabControl.GotFocus += delegate {
					TabItem page = (TabItem)viewTabControl.SelectedItem;
					if (!page.IsFocused) page.Focus();
				};
				viewTabControl.TabStripPlacement = Dock.Bottom;
				this.SetContent(viewTabControl);
				
				viewTabControl.SelectionChanged += delegate {
					UpdateActiveViewContent();
				};
			}
		}
		
		void ClearContent()
		{
			this.Content = null;
			if (viewTabControl != null) {
				foreach (TabItem page in viewTabControl.Items) {
					page.SetContent(null);
				}
				viewTabControl = null;
			}
		}
		
		void OnTitleNameChanged(object sender, EventArgs e)
		{
			if (sender == ActiveViewContent) {
				UpdateTitle();
			}
		}
		
		void OnIsDirtyChanged(object sender, EventArgs e)
		{
			UpdateTitle();
		}
		
		void UpdateTitle()
		{
			IViewContent content = ActiveViewContent;
			if (content != null) {
				base.ToolTip = content.PrimaryFileName;
				
				string newTitle = content.TitleName;
				
				if (this.IsDirty) {
					newTitle += "*";
				} else if (content.IsReadOnly) {
					newTitle += "+";
				}
				
				if (newTitle != Title) {
					Title = newTitle;
					OnTitleChanged(EventArgs.Empty);
				}
			}
		}
		
		void RegisterNewContent(IViewContent content)
		{
			Debug.Assert(content.WorkbenchWindow == null);
			content.WorkbenchWindow = this;
			
			content.TabPageTextChanged += OnTabPageTextChanged;
			content.TitleNameChanged   += OnTitleNameChanged;
			content.IsDirtyChanged     += OnIsDirtyChanged;
			
			this.dockLayout.Workbench.OnViewOpened(new ViewContentEventArgs(content));
		}
		
		void UnregisterContent(IViewContent content)
		{
			content.WorkbenchWindow = null;
			
			content.TabPageTextChanged -= OnTabPageTextChanged;
			content.TitleNameChanged   -= OnTitleNameChanged;
			content.IsDirtyChanged     -= OnIsDirtyChanged;
			
			this.dockLayout.Workbench.OnViewClosed(new ViewContentEventArgs(content));
		}
		
		void OnTabPageTextChanged(object sender, EventArgs e)
		{
			RefreshTabPageTexts();
		}
		
		bool forceClose;
		
		public bool CloseWindow(bool force)
		{
			forceClose = force;
			Close();
			return this.ViewContents.Count == 0;
		}
		
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);
			if (!e.Cancel && !forceClose && this.IsDirty) {
				MessageBoxResult dr = MessageBox.Show(
					ResourceService.GetString("MainWindow.SaveChangesMessage"),
					ResourceService.GetString("MainWindow.SaveChangesMessageHeader") + " " + Title + " ?",
					MessageBoxButton.YesNoCancel, MessageBoxImage.Question,
					MessageBoxResult.Yes);
				switch (dr) {
					case MessageBoxResult.Yes:
						foreach (IViewContent vc in this.ViewContents) {
							if (!vc.IsDirty) continue;
							if (vc.PrimaryFile != null) {
								while (true) {
									vc.Files.ForEach(ICSharpCode.SharpDevelop.Commands.SaveFile.Save);
									if (vc.IsDirty) {
										if (MessageService.AskQuestion("${res:MainWindow.DiscardChangesMessage}")) {
											break;
										}
									} else {
										break;
									}
								}
							}
						}
						break;
					case MessageBoxResult.No:
						break;
					case MessageBoxResult.Cancel:
						e.Cancel = true;
						break;
				}
			}
		}
		
		protected override void OnClosed()
		{
			base.OnClosed();
			Dispose();
			CommandManager.InvalidateRequerySuggested();
		}
		
		public void RedrawContent()
		{
			RefreshTabPageTexts();
		}
		
		void RefreshTabPageTexts()
		{
			if (viewTabControl != null) {
				for (int i = 0; i < viewTabControl.Items.Count; ++i) {
					TabItem tabPage = (TabItem)viewTabControl.Items[i];
					tabPage.Header = StringParser.Parse(ViewContents[i].TabPageText);
				}
			}
		}
		
		void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}
		
		public event EventHandler TitleChanged;
		
		BitmapSource icon;
		
		BitmapSource IWorkbenchWindow.Icon {
			get { return icon; }
			set {
				if (icon != value) {
					icon = value;
					base.Icon = new PixelSnapper(new Image { Source = value });
				}
			}
		}
	}
}
