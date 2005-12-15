// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
			ClickedWindowIsForm = 2,
			FileUntitled        = 4
		}
		
		OpenFileTabState internalState = OpenFileTabState.Nothing;

		public System.Enum InternalState {
			get {
				return internalState;
			}
		}
		#endregion

		TabControl   viewTabControl = null;
		IViewContent content;
		
		string myUntitledTitle     = null;
		
		public string Title {
			get {
				return Text;
			}
			set {
				Text = value;
				OnTitleChanged(null);
			}
		}
		
		public IBaseViewContent ActiveViewContent {
			get {
				if (viewTabControl != null) {
					int selectedIndex = 0;
					if (viewTabControl.InvokeRequired) {
						// the window might have been disposed just here, invoke on the
						// Workbench instead
						selectedIndex = (int)WorkbenchSingleton.SafeThreadCall(this, "GetSelectedIndex");
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
			base.Dispose(disposing);
			
			if (disposing) {
				if (content != null)
					DetachContent();
				if (this.TabPageContextMenu != null) {
					this.TabPageContextMenu.Dispose();
					this.TabPageContextMenu = null;
				}
			}
		}
		
		public SdiWorkspaceWindow(IViewContent content)
		{
			this.content = content;
			
			content.WorkbenchWindow = this;
			
			content.TitleNameChanged += new EventHandler(SetTitleEvent);
			content.DirtyChanged     += new EventHandler(SetTitleEvent);
			
			SetTitleEvent(null, null);
			
			this.DockableAreas = WeifenLuo.WinFormsUI.DockAreas.Document;
			this.DockPadding.All = 2;

			SetTitleEvent(this, EventArgs.Empty);
			this.TabPageContextMenu  = MenuService.CreateContextMenu(this, contextMenuPath);
			InitControls();
		}
		
		internal void InitControls()
		{
			if (content.SecondaryViewContents.Count > 0) {
				viewTabControl		  = new TabControl();
				viewTabControl.Alignment = TabAlignment.Bottom;
				viewTabControl.Dock = DockStyle.Fill;
				viewTabControl.SelectedIndexChanged += new EventHandler(viewTabControlIndexChanged);
				
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
		
		void LeaveTabPage(object sender, EventArgs e)
		{
			OnWindowDeselected(EventArgs.Empty);
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
			internalState = OpenFileTabState.Nothing;
			
			if (content == null) {
				return;
			}
			SetToolTipText();
			string newTitle = "";
			if (content.TitleName == null) {
				myUntitledTitle = Path.GetFileNameWithoutExtension(content.UntitledName);
//				if (myUntitledTitle == null) {
//					string baseName
//					int    number    = 1;
//					bool   found     = true;
//					while (found) {
//						found = false;
//						foreach (IViewContent windowContent in WorkbenchSingleton.Workbench.ViewContentCollection) {
//							string title = windowContent.WorkbenchWindow.Title;
//							if (title.EndsWith("*") || title.EndsWith("+")) {
//								title = title.Substring(0, title.Length - 1);
//							}
//							if (title == baseName + number) {
//								found = true;
//								++number;
//								break;
//							}
//						}
//					}
//					myUntitledTitle = baseName + number;
//				}
				newTitle = myUntitledTitle;
				internalState |= OpenFileTabState.FileUntitled;
			} else {
				newTitle = content.TitleName;
			}
			
			if (content.IsDirty) {
				internalState |= OpenFileTabState.FileDirty;
				newTitle += "*";
			} else if (content.IsReadOnly) {
				newTitle += "+";
			}
			
			if (newTitle != Title) {
				Text = newTitle;
			}
		}
		
		public void DetachContent()
		{
			content.TitleNameChanged -= new EventHandler(SetTitleEvent);
			content.DirtyChanged     -= new EventHandler(SetTitleEvent);
			content = null;
			
			if (viewTabControl != null) {
				foreach (TabPage page in viewTabControl.TabPages) {
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
				
				DialogResult dr = MessageBox.Show(
				                                  ResourceService.GetString("MainWindow.SaveChangesMessage"),
				                                  ResourceService.GetString("MainWindow.SaveChangesMessageHeader") + " " + Title + " ?",
				                                  MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
		
		int oldIndex = 0;
		void viewTabControlIndexChanged(object sender, EventArgs e)
		{
			if (oldIndex >= 0) {
				IBaseViewContent secondaryViewContent = GetSubViewContent(oldIndex);
				if (secondaryViewContent != null) {
					secondaryViewContent.Deselected();
				}
			}
			
			if (viewTabControl.SelectedIndex >= 0) {
				IBaseViewContent secondaryViewContent = GetSubViewContent(viewTabControl.SelectedIndex);
				if (secondaryViewContent != null) {
					secondaryViewContent.SwitchedTo();
					secondaryViewContent.Selected();
				}
			}
			oldIndex = viewTabControl.SelectedIndex;
			WorkbenchSingleton.Workbench.WorkbenchLayout.OnActiveWorkbenchWindowChanged(EventArgs.Empty);
			ActiveViewContent.Control.Focus();
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
