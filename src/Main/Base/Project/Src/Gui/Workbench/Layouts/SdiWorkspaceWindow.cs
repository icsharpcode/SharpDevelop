// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core;
using WeifenLuo.WinFormsUI;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class SdiWorkspaceWindow : DockContent, IWorkbenchWindow, IOwnerState
	{
		readonly static string contextMenuPath = "/SharpDevelop/Workbench/OpenFileTab/ContextMenu";
		
		#region IOwnerState
		[Flags]
		public enum OpenFileTabState {
			Nothing             = 0,
			FileDirty           = 1,
			FileReadOnly        = 2,
			FileUntitled        = 4
		}
		
		public System.Enum InternalState {
			get {
				OpenFileTabState state = OpenFileTabState.Nothing;
				if (content != null) {
					if (content.IsDirty)    state |= OpenFileTabState.FileDirty;
					if (content.IsReadOnly) state |= OpenFileTabState.FileReadOnly;
					if (content.IsUntitled) state |= OpenFileTabState.FileUntitled;
				}
				return state;
			}
		}
		#endregion

		TabControl   viewTabControl = null;
		IViewContent content;
		
		public string Title {
			get {
				return Text;
			}
			set {
				Text = value;
				OnTitleChanged(EventArgs.Empty);
			}
		}
		
		public IBaseViewContent ActiveViewContent {
			get {
				if (viewTabControl != null) {
					int selectedIndex = 0;
					if (WorkbenchSingleton.InvokeRequired) {
						// the window might have been disposed just here, invoke on the
						// Workbench instead
						selectedIndex = WorkbenchSingleton.SafeThreadFunction<int>(GetSelectedIndex);
					} else {
						selectedIndex = GetSelectedIndex();
					}
					
					return GetSubViewContent(selectedIndex);
				}
				return content;
			}
		}
		
		int GetSelectedIndex()
		{
			return viewTabControl.SelectedIndex;
		}
		
		protected override Size DefaultSize {
			get {
				return Size.Empty;
			}
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
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				ParserService.LoadSolutionProjectsThreadEnded -= LoadSolutionProjectsThreadEndedEvent;
				if (content != null)
					DetachContent();
				if (this.TabPageContextMenu != null) {
					this.TabPageContextMenu.Dispose();
					this.TabPageContextMenu = null;
				}
			}
			// DetachContent must be called before the controls are disposed
			base.Dispose(disposing);
		}
		
		public SdiWorkspaceWindow(IViewContent content)
		{
			this.content = content;
			
			content.WorkbenchWindow = this;
			
			content.TitleNameChanged += new EventHandler(SetTitleEvent);
			content.DirtyChanged     += new EventHandler(SetTitleEvent);
			
			this.DockableAreas = WeifenLuo.WinFormsUI.DockAreas.Document;
			this.DockPadding.All = 2;

			SetTitleEvent(this, EventArgs.Empty);
			this.TabPageContextMenuStrip = MenuService.CreateContextMenu(this, contextMenuPath);
			InitControls();
			
			ParserService.LoadSolutionProjectsThreadEnded += LoadSolutionProjectsThreadEndedEvent;
		}
		
		private void CreateViewTabControl()
		{
			viewTabControl = new TabControl();
			viewTabControl.GotFocus += delegate {
				TabPage page = viewTabControl.TabPages[viewTabControl.TabIndex];
				if (page.Controls.Count == 1 && !page.ContainsFocus) page.Controls[0].Focus();
			};
			viewTabControl.Alignment = TabAlignment.Bottom;
			viewTabControl.Dock = DockStyle.Fill;
			viewTabControl.Selected += viewTabControlSelected;
			viewTabControl.Deselecting += viewTabControlDeselecting;
			viewTabControl.Deselected += viewTabControlDeselected;
		}
		
		internal void InitControls()
		{
			if (content.SecondaryViewContents.Count > 0) {
				CreateViewTabControl();
				AttachSecondaryViewContent(content);
				foreach (ISecondaryViewContent subContent in content.SecondaryViewContents) {
					AttachSecondaryViewContent(subContent);
				}
				Controls.Add(viewTabControl);
			} else {
				content.Control.Dock = DockStyle.Fill;
				Controls.Add(content.Control);
			}
		}
		
		private void AttachSecondaryViewContent(IBaseViewContent viewContent)
		{
			viewContent.WorkbenchWindow = this;
			TabPage newPage = new TabPage(StringParser.Parse(viewContent.TabPageText));
			newPage.Tag = viewContent;
			viewContent.Control.Dock = DockStyle.Fill;
			newPage.Controls.Add(viewContent.Control);
			viewTabControl.TabPages.Add(newPage);
		}
		
		/// <summary>
		/// Ensures that all possible secondary view contents are attached.
		/// This is primarily used to add the FormsDesigner view content for files
		/// containing partial classes after the designer file has been parsed if
		/// the view content has been created too early on startup.
		/// </summary>
		void RefreshSecondaryViewContents()
		{
			if (content == null) {
				return;
			}
			int oldSvcCount = content.SecondaryViewContents.Count;
			DisplayBindingService.AttachSubWindows(content, true);
			if (content.SecondaryViewContents.Count > oldSvcCount) {
				LoggingService.Debug("Attaching new secondary view contents to '"+this.Title+"'");
				if (viewTabControl == null) {
					// The tab control needs to be created first.
					Controls.Remove(content.Control);
					CreateViewTabControl();
					AttachSecondaryViewContent(content);
					Controls.Add(viewTabControl);
				}
				foreach (ISecondaryViewContent svc in content.SecondaryViewContents) {
					if (svc.WorkbenchWindow == null) {
						AttachSecondaryViewContent(svc);
					}
				}
			}
		}
		
		void LoadSolutionProjectsThreadEndedEvent(object sender, EventArgs e)
		{
			// do not invoke on this: it's possible that "this" is disposed while this method is executing
			WorkbenchSingleton.SafeThreadAsyncCall(this.RefreshSecondaryViewContents);
		}
		
		public IViewContent ViewContent {
			get {
				return content;
			}
		}
		
		void SetToolTipText()
		{
			if (content != null) {
				try {
					if (content.FileName != null && content.FileName.Length > 0) {
						base.ToolTipText = Path.GetFullPath(content.FileName);
					} else {
						base.ToolTipText = null;
					}
				} catch (Exception) {
					base.ToolTipText = content.FileName;
				}
			} else {
				base.ToolTipText = null;
			}
		}
		
		public void SetTitleEvent(object sender, EventArgs e)
		{
			if (content == null) {
				return;
			}
			SetToolTipText();
			string newTitle;
			if (content.TitleName == null) {
				newTitle = Path.GetFileName(content.UntitledName);
			} else {
				newTitle = content.TitleName;
			}
			
			if (content.IsDirty) {
				newTitle += "*";
			} else if (content.IsReadOnly) {
				newTitle += "+";
			}
			
			if (newTitle != Title) {
				Title = newTitle;
			}
		}
		
		public void DetachContent()
		{
			content.TitleNameChanged -= new EventHandler(SetTitleEvent);
			content.DirtyChanged     -= new EventHandler(SetTitleEvent);
			content = null;
			
			if (viewTabControl != null) {
				foreach (TabPage page in viewTabControl.TabPages) {
					if (viewTabControl.SelectedTab == page && page.Tag is IBaseViewContent) {
						((IBaseViewContent)page.Tag).Deselecting();
					}
					page.Controls.Clear();
					if (viewTabControl.SelectedTab == page && page.Tag is IBaseViewContent) {
						((IBaseViewContent)page.Tag).Deselected();
					}
				}
				viewTabControl.Dispose();
				viewTabControl = null;
			}
			Controls.Clear();
		}
		
		public bool CloseWindow(bool force)
		{
			if (!force && ViewContent != null && ViewContent.IsDirty) {
				DialogResult dr = MessageBox.Show(ResourceService.GetString("MainWindow.SaveChangesMessage"),
				                                  ResourceService.GetString("MainWindow.SaveChangesMessageHeader") + " " + Title + " ?",
				                                  MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
				                                  MessageBoxDefaultButton.Button1,
				                                  RightToLeftConverter.IsRightToLeft ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : 0);
				switch (dr) {
					case DialogResult.Yes:
						if (content.FileName == null) {
							while (true) {
								new ICSharpCode.SharpDevelop.Commands.SaveFileAs().Run();
								if (ViewContent.IsDirty) {
									
									if (MessageService.AskQuestion("${res:MainWindow.DiscardChangesMessage}")) {
										break;
									}
								} else {
									break;
								}
							}
							
						} else {
							
							FileUtility.ObservedSave(new FileOperationDelegate(ViewContent.Save), ViewContent.FileName , FileErrorPolicy.ProvideAlternative);
						}
						break;
					case DialogResult.No:
						break;
					case DialogResult.Cancel:
						return false;
				}
			}

			OnCloseEvent(null);
			Dispose();
			return true;
		}
		
		IBaseViewContent GetSubViewContent(int index)
		{
			if (index == 0)
				return content;
			else
				return content.SecondaryViewContents[index - 1];
		}
		
		void viewTabControlSelected(object sender, TabControlEventArgs e)
		{
			if (e.Action == TabControlAction.Selected && e.TabPageIndex >= 0) {
				IBaseViewContent secondaryViewContent = GetSubViewContent(e.TabPageIndex);
				if (secondaryViewContent != null) {
					secondaryViewContent.SwitchedTo();
					secondaryViewContent.Selected();
				}
			}
			WorkbenchSingleton.Workbench.WorkbenchLayout.OnActiveWorkbenchWindowChanged(EventArgs.Empty);
			ActiveViewContent.Control.Focus();
		}
		
		void viewTabControlDeselecting(object sender, TabControlCancelEventArgs e)
		{
			if (e.Action == TabControlAction.Deselecting && e.TabPageIndex >= 0) {
				IBaseViewContent secondaryViewContent = GetSubViewContent(e.TabPageIndex);
				if (secondaryViewContent != null) {
					secondaryViewContent.Deselecting();
				}
			}
		}

		void viewTabControlDeselected(object sender, TabControlEventArgs e)
		{
			if (e.Action == TabControlAction.Deselected && e.TabPageIndex >= 0) {
				IBaseViewContent secondaryViewContent = GetSubViewContent(e.TabPageIndex);
				if (secondaryViewContent != null) {
					secondaryViewContent.Deselected();
				}
			}
		}

		public virtual void RedrawContent()
		{
			if (viewTabControl != null) {
				for (int i = 0; i < viewTabControl.TabPages.Count; ++i) {
					TabPage tabPage = viewTabControl.TabPages[i];
					tabPage.Text = StringParser.Parse(GetSubViewContent(i).TabPageText);
				}
			}
		}
		
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
			WorkbenchSingleton.Workbench.WorkbenchLayout.OnActiveWorkbenchWindowChanged(EventArgs.Empty);
		}
		
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = !CloseWindow(false);
		}
		
		protected virtual void OnCloseEvent(EventArgs e)
		{
			OnWindowDeselected(e);
			if (CloseEvent != null) {
				CloseEvent(this, e);
			}
		}
		
		public virtual void OnWindowSelected(EventArgs e)
		{
			if (WindowSelected != null) {
				WindowSelected(this, e);
			}
		}
		public virtual void OnWindowDeselected(EventArgs e)
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
