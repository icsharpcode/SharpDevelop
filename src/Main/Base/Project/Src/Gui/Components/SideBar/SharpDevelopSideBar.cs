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
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class SharpDevelopSideTabItemFactory : ISideTabItemFactory
	{
		public SideTabItem CreateSideTabItem(string name)
		{
			return new SharpDevelopSideTabItem(name);
		}
		
		public SideTabItem CreateSideTabItem(string name, object tag)
		{
			return new SharpDevelopSideTabItem(name, tag);
		}
		
		public SideTabItem CreateSideTabItem(string name, object tag, Bitmap bitmap)
		{
			return new SharpDevelopSideTabItem(name, tag, bitmap);
		}
	}
	
	public class SharpDevelopSideBar : SideBarControl, IOwnerState
	{
		protected string contextMenuPath        = "/SharpDevelop/Workbench/SharpDevelopSideBar/ContextMenu";
		protected string sideTabContextMenuPath = "/SharpDevelop/Workbench/SharpDevelopSideBar/SideTab/ContextMenu";
		
		[Flags]
		public enum SidebarState {
			Nothing       = 0,
			CanMoveUp     = 1,
			CanMoveDown   = 2,
			TabCanBeDeleted = 4,
			CanMoveItemUp = 8,
			CanMoveItemDown = 16,
			CanBeRenamed = 32
		}
		
		protected SidebarState internalState = SidebarState.TabCanBeDeleted;

		public System.Enum InternalState {
			get {
				return internalState;
			}
		}
		
		public SharpDevelopSideBar()
		{
			SideTabItemFactory = new SharpDevelopSideTabItemFactory();
			
			MouseUp                     += new MouseEventHandler(SetContextMenu);
			sideTabContent.MouseUp += new MouseEventHandler(SetItemContextMenu);
		}
		
		public void DeleteSideTab(SideTab tab)
		{
			if (tab == null) {
				return;
			}
			
			Tabs.Remove(tab);
			OnSideTabDeleted(tab);
		}
		
		////////////////////////////////////////////////////////////////////////////
		// Tab Context Menu
		
		void SetDeletedState(SideTabItem item)
		{
			if (item != null) {
				SetDeletedState(item.CanBeDeleted);
			} else {
				SetDeletedState(false);
			}
		}
		
		void SetDeletedState(bool canBeDeleted)
		{
			if (canBeDeleted) {
				internalState |= SidebarState.TabCanBeDeleted;
			} else {
				internalState = internalState & ~SidebarState.TabCanBeDeleted;
			}
		}

		
		void SetRenameState(SideTabItem item)
		{
			if (item != null) {
				SetRenameState(item.CanBeRenamed);
			} else {
				SetRenameState(false);
			}
		}
		
		void SetRenameState(bool canBeRenamed)
		{
			if (canBeRenamed) {
				internalState |= SidebarState.CanBeRenamed;
			} else {
				internalState = internalState & ~SidebarState.CanBeRenamed;
			}
		}
		
		void SetContextMenu(object sender, MouseEventArgs e)
		{
			ExitRenameMode();
			
			int index = GetTabIndexAt(e.X, e.Y);
			if (index >= 0) {
				SideTab tab = Tabs[index];
				
				SetDeletedState(tab.CanBeDeleted);
				SetRenameState(tab.CanBeRenamed);
				
				if (index > 0) {
					internalState |= SidebarState.CanMoveUp;
				} else {
					internalState = internalState & ~SidebarState.CanMoveUp;
				}
				
				if (index < Tabs.Count - 1) {
					internalState |= SidebarState.CanMoveDown;
				} else {
					internalState = internalState & ~(SidebarState.CanMoveDown);
				}
				Tabs.DragOverTab = tab;
				Refresh();
				Tabs.DragOverTab = null;
			}
			
			if (e.Button == MouseButtons.Right) {
				SD.WinForms.MenuService.ShowContextMenu(this, contextMenuPath, this, e.X, e.Y);
			}
		}
		
		void SetItemContextMenu(object sender, MouseEventArgs e)
		{
			ExitRenameMode();
			if (e.Button == MouseButtons.Right) {
				int index = Tabs.IndexOf(ActiveTab);
				
				if (index > 0) {
					internalState |= SidebarState.CanMoveUp;
				} else {
					internalState = internalState & ~SidebarState.CanMoveUp;
				}
				
				if (index < Tabs.Count - 1) {
					internalState |= SidebarState.CanMoveDown;
				} else {
					internalState = internalState & ~(SidebarState.CanMoveDown);
				}
				
				Tabs.DragOverTab = ActiveTab;
				Refresh();
				Tabs.DragOverTab = null;
			}
			
			if (e.Button == MouseButtons.Right) {
				// set moveup/down states correctly
				SetDeletedState(ActiveTab.SelectedItem);
				SetRenameState(ActiveTab.SelectedItem);
				
				int index = ActiveTab.Items.IndexOf(ActiveTab.SelectedItem);
				if (index > 0) {
					internalState |= SidebarState.CanMoveItemUp;
				} else {
					internalState = internalState & ~(SidebarState.CanMoveItemUp);
				}
				
				if (index < ActiveTab.Items.Count - 1) {
					internalState |= SidebarState.CanMoveItemDown;
				} else {
					internalState = internalState & ~(SidebarState.CanMoveItemDown);
				}
				
				SD.WinForms.MenuService.ShowContextMenu(this, sideTabContextMenuPath, sideTabContent, e.X, e.Y);
			}
		}
		
		void OnSideTabDeleted(SideTab tab)
		{
			if (SideTabDeleted != null) {
				SideTabDeleted(this, new SideTabEventArgs(tab));
			}
		}
		
		public event SideTabEventHandler SideTabDeleted;
		
		
		public Point SideBarMousePosition { get; private set; }
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			this.SideBarMousePosition = new Point(e.X, e.Y);
		}
	}
}
