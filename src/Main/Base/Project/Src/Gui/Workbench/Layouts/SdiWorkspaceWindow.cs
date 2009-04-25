// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using WeifenLuo.WinFormsUI.Docking;

namespace ICSharpCode.SharpDevelop.Gui
{
	internal sealed class SdiWorkspaceWindow : DockContent, IWorkbenchWindow, IOwnerState
	{
		readonly static string contextMenuPath = "/SharpDevelop/Workbench/OpenFileTab/ContextMenu";
		
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
		
		public string Title {
			get {
				return Text;
			}
			set {
				Text = value;
				OnTitleChanged(EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// The current view content which is shown inside this window.
		/// </summary>
		public IViewContent ActiveViewContent {
			get {
				Debug.Assert(WorkbenchSingleton.InvokeRequired == false);
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
					throw new ArgumentException("");
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
		}
		
		sealed class ViewContentCollection : Collection<IViewContent>
		{
			readonly SdiWorkspaceWindow window;
			
			internal ViewContentCollection(SdiWorkspaceWindow window)
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
				
				item.Control.Dock = DockStyle.Fill;
				if (Count == 1) {
					window.Controls.Add(item.Control);
				} else {
					if (Count == 2) {
						window.CreateViewTabControl();
						IViewContent oldItem = this[0];
						if (oldItem == item) oldItem = this[1];
						
						TabPage oldPage = new TabPage(StringParser.Parse(oldItem.TabPageText));
						oldPage.Controls.Add(oldItem.Control);
						window.viewTabControl.TabPages.Add(oldPage);
					}
					
					TabPage newPage = new TabPage(StringParser.Parse(item.TabPageText));
					newPage.Controls.Add(item.Control);
					
					// Work around bug in TabControl: TabPages.Insert has no effect if inserting at end
					if (index == window.viewTabControl.TabPages.Count) {
						window.viewTabControl.TabPages.Add(newPage);
					} else {
						window.viewTabControl.TabPages.Insert(index, newPage);
					}
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
						window.Controls.Add(this[0].Control);
					}
				} else {
					window.viewTabControl.TabPages.RemoveAt(index);
				}
				window.UpdateActiveViewContent();
			}
			
			protected override void SetItem(int index, IViewContent item)
			{
				window.UnregisterContent(this[index]);
				
				base.SetItem(index, item);
				
				window.RegisterNewContent(item);
				
				item.Control.Dock = DockStyle.Fill;
				if (Count == 1) {
					window.ClearContent();
					window.Controls.Add(item.Control);
				} else {
					TabPage page = window.viewTabControl.TabPages[index];
					page.Controls.Clear();
					page.Controls.Add(item.Control);
					page.Text = StringParser.Parse(item.TabPageText);
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
			Show();
		}
		
		public SdiWorkspaceWindow()
		{
			viewContents = new ViewContentCollection(this);
			
			this.DockAreas = DockAreas.Document;
			this.DockPadding.All = 2;

			OnTitleNameChanged(this, EventArgs.Empty);
			this.TabPageContextMenuStrip = MenuService.CreateContextMenu(this, contextMenuPath);
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				// DetachContent must be called before the controls are disposed
				this.ViewContents.Clear();
				if (this.TabPageContextMenu != null) {
					this.TabPageContextMenu.Dispose();
					this.TabPageContextMenu = null;
				}
			}
			base.Dispose(disposing);
		}
		
		private void CreateViewTabControl()
		{
			if (viewTabControl == null) {
				this.Controls.Clear();
				
				viewTabControl = new TabControl();
				viewTabControl.GotFocus += delegate {
					TabPage page = viewTabControl.SelectedTab;
					if (page.Controls.Count == 1 && !page.ContainsFocus) page.Controls[0].Focus();
				};
				viewTabControl.Alignment = TabAlignment.Bottom;
				viewTabControl.Dock = DockStyle.Fill;
				this.Controls.Add(viewTabControl);
				
				viewTabControl.SelectedIndexChanged += delegate {
					UpdateActiveViewContent();
				};
			}
		}
		
		void ClearContent()
		{
			this.Controls.Clear();
			if (viewTabControl != null) {
				foreach (TabPage page in viewTabControl.TabPages) {
					page.Controls.Clear();
				}
				viewTabControl.Dispose();
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
			if (content == null && this.ViewContents.Count > 0) {
				// This can happen when the window is inactive and
				// no tab page of the viewTabControl is selected
				// (viewTabControl.SelectedIndex == -1)
				// but we have multiple ViewContents.
				content = this.ViewContents[0];
			}
			if (content != null) {
				base.ToolTipText = content.PrimaryFileName;
				
				string newTitle = content.TitleName;
				
				if (this.IsDirty) {
					newTitle += "*";
				} else if (content.IsReadOnly) {
					newTitle += "+";
				}
				
				if (newTitle != Title) {
					Title = newTitle;
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
		}
		
		void UnregisterContent(IViewContent content)
		{
			content.WorkbenchWindow = null;
			
			content.TabPageTextChanged -= OnTabPageTextChanged;
			content.TitleNameChanged   -= OnTitleNameChanged;
			content.IsDirtyChanged     -= OnIsDirtyChanged;
		}
		
		void OnTabPageTextChanged(object sender, EventArgs e)
		{
			RefreshTabPageTexts();
		}
		
		public bool CloseWindow(bool force)
		{
			bool fileDiscarded = false;
			if (!force && this.IsDirty) {
				DialogResult dr = MessageBox.Show(ResourceService.GetString("MainWindow.SaveChangesMessage"),
				                                  ResourceService.GetString("MainWindow.SaveChangesMessageHeader") + " " + Title + " ?",
				                                  MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
				                                  MessageBoxDefaultButton.Button1,
				                                  RightToLeftConverter.IsRightToLeft ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : 0);
				switch (dr) {
					case DialogResult.Yes:
						foreach (IViewContent vc in this.ViewContents) {
							while (vc.IsDirty) {
								ICSharpCode.SharpDevelop.Commands.SaveFile.Save(vc);
								if (vc.IsDirty) {
									if (MessageService.AskQuestion("${res:MainWindow.DiscardChangesMessage}")) {
										fileDiscarded = true;
										break;
									}
								}
							}
						}
						break;
					case DialogResult.No:
						fileDiscarded = true;
						break;
					case DialogResult.Cancel:
						return false;
				}
			}
			
			// Create list of files to reparse after the window is closed.
			// This is necessary because when changes were discarded,
			// the ParserService still has the information with the changes
			// that are now invalid.
			string[] filesToReparse;
			if (fileDiscarded) {
				filesToReparse = this.ViewContents
					.SelectMany(vc => vc.Files)
					.Select(f => f.FileName)
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.ToArray();
			} else {
				filesToReparse = null;
			}

			OnCloseEvent(null);
			Dispose();
			
			if (filesToReparse != null) {
				// This must happen after Dispose so that the ViewContents
				// are closed and their content is no longer found by
				// the ParserService.
				LoggingService.Debug("SdiWorkspaceWindow closed with discarding changes, enqueueing files for parsing: " + String.Join(", ", filesToReparse));
				foreach (string file in filesToReparse) {
					if (File.Exists(file)) {
						ParserService.EnqueueForParsing(file);
					} else {
						ParserService.ClearParseInformation(file);
					}
				}
			}
			
			return true;
		}
		
		public void RedrawContent()
		{
			RefreshTabPageTexts();
		}
		
		void RefreshTabPageTexts()
		{
			if (viewTabControl != null) {
				for (int i = 0; i < viewTabControl.TabPages.Count; ++i) {
					TabPage tabPage = viewTabControl.TabPages[i];
					tabPage.Text = StringParser.Parse(ViewContents[i].TabPageText);
				}
			}
		}
		
		void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}
		
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = !CloseWindow(false);
		}
		
		void OnCloseEvent(EventArgs e)
		{
			OnWindowDeselected(e);
			if (CloseEvent != null) {
				CloseEvent(this, e);
			}
		}
		
		public void OnWindowSelected(EventArgs e)
		{
			if (WindowSelected != null) {
				WindowSelected(this, e);
			}
		}
		
		public void OnWindowDeselected(EventArgs e)
		{
			if (WindowDeselected != null) {
				WindowDeselected(this, e);
			}
		}
		
		public event EventHandler WindowSelected;
		public event EventHandler WindowDeselected;
		
		public event EventHandler TitleChanged;
		public event EventHandler CloseEvent;
	}
}
