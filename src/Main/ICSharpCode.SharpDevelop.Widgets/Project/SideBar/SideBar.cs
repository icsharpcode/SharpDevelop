// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.SideBar
{
	public interface ISideTabItemFactory
	{
		SideTabItem CreateSideTabItem(string name);
		SideTabItem CreateSideTabItem(string name, object tag);
		SideTabItem CreateSideTabItem(string name, object tag, Bitmap bitmap);
	}
	
	public class DefaultSideTabItemFactory : ISideTabItemFactory
	{
		public SideTabItem CreateSideTabItem(string name)
		{
			return new SideTabItem(name);
		}
		
		public SideTabItem CreateSideTabItem(string name, object tag)
		{
			return new SideTabItem(name, tag);
		}
		public SideTabItem CreateSideTabItem(string name, object tag, Bitmap bitmap)
		{
			return new SideTabItem(name, tag, bitmap);
		}
		
	}
	
	public interface ISideTabFactory
	{
		SideTab CreateSideTab(SideBarControl sideBar, string name);
	}
	
	public class DefaultSideTabFactory : ISideTabFactory
	{
		public SideTab CreateSideTab(SideBarControl sideBar, string name)
		{
			return new SideTab(sideBar, name);
		}
	}
	
	public class SideBarControl : UserControl
	{
		SideTabCollection    sideTabs;
		SideTab           activeTab = null;
		
		[CLSCompliant(false)]
		protected SideTabContent sideTabContent = new SideTabContent();
		
		SideTab           renameTab     = null;
		SideTabItem     renameTabItem = null;
		TextBox           renameTextBox = new TextBox();
		
		ScrollBar         scrollBar = new VScrollBar();
		
		Point mousePosition;
		bool  doAddTab = false;
		
		public bool DoAddTab {
			get {
				return doAddTab;
			}
			set {
				doAddTab = value;
			}
		}
		
		ISideTabFactory     sideTabFactory     = new DefaultSideTabFactory();
		ISideTabItemFactory sideTabItemFactory = new DefaultSideTabItemFactory();
		
		public ISideTabItemFactory SideTabItemFactory {
			get {
				return sideTabItemFactory;
			}
			set {
				sideTabItemFactory = value;
			}
		}
		
		public ISideTabFactory SideTabFactory {
			get {
				return sideTabFactory;
			}
			set {
				sideTabFactory = value;
			}
		}
		
		public SideTabCollection Tabs {
			get {
				return sideTabs;
			}
		}
		
		public SideTab ActiveTab {
			get {
				return activeTab;
			}
			set {
				if (activeTab != value) {
					if (activeTab != null) {
						activeTab.ScrollIndex = scrollBar.Value;
					}
					activeTab = value;
					if (activeTab != null) {
						scrollBar.SmallChange = 1;
						scrollBar.LargeChange = sideTabContent.Height / activeTab.ItemHeight;
						scrollBar.Maximum  = activeTab.Items.Count;
						scrollBar.Value    = activeTab.ScrollIndex;
					}
				}
				Refresh();
			}
		}
		
		protected override void OnResize(System.EventArgs e)
		{
			base.OnResize(e);
			if (activeTab != null) {
				scrollBar.LargeChange = sideTabContent.Height / activeTab.ItemHeight;
			}
		}
		
		public SideBarControl()
		{
			ResizeRedraw = true;
			AllowDrop = true;
			
			sideTabs = new SideTabCollection(this);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.CacheText, true);
			
			//		itemTabMenu.Popup += new EventHandler(ItemContextMenuPopup);
			
			renameTextBox.Visible = false;
			renameTextBox.BorderStyle = BorderStyle.None;
			
			Controls.Add(renameTextBox);
			
			scrollBar.Scroll += new ScrollEventHandler(ScrollBarScrolled);
			Controls.Add(scrollBar);
			
			sideTabContent.SideBar = this;
			Controls.Add(sideTabContent);
		}
		
		protected void ExitRenameMode()
		{
			if (renameTab != null) {
				renameTextBox.Visible = false;
				renameTab = null;
				doAddTab = false;
			} else if (renameTabItem != null) {
				renameTextBox.Visible = false;
				renameTabItem = null;
			}
		}
		
		public void EnsureVisible(SideTabItem item)
		{
			int index = activeTab.Items.IndexOf(item);
			if (index != -1) {
				if (index < scrollBar.Value) {
					scrollBar.Value = Math.Max(scrollBar.Minimum, Math.Min(scrollBar.Maximum, index));
					ScrollBarScrolled(null, null);
				} else if (index > scrollBar.Value + (sideTabContent.Height - 15) / 20) {
					scrollBar.Value = Math.Max(scrollBar.Minimum, Math.Min(scrollBar.Maximum, index - (sideTabContent.Height - 15) / 20));
					ScrollBarScrolled(null, null);
				}
			}
		}
		
		protected override bool ProcessCmdKey(ref Message msg,Keys keyData)
		{
			int index;
			if (base.ProcessCmdKey(ref msg, keyData)) {
				return true;
			}
			bool isInRenameMode = renameTab != null || renameTabItem != null;
			
			switch (keyData) {
				case Keys.Home:
					if (activeTab.Items.Count > 0 && !isInRenameMode) {
						activeTab.ChoosedItem = activeTab.Items[0];
						EnsureVisible(activeTab.ChoosedItem);
						Refresh();
					}
					break;
				case Keys.End:
					if (activeTab.Items.Count > 0 && !isInRenameMode) {
						activeTab.ChoosedItem = activeTab.Items[activeTab.Items.Count - 1];
						EnsureVisible(activeTab.ChoosedItem);
						Refresh();
					}
					break;
				case Keys.PageUp:
					if (activeTab.Items.Count > 0 && !isInRenameMode) {
						index = Math.Max(0, activeTab.Items.IndexOf(activeTab.ChoosedItem) - scrollBar.LargeChange);
						activeTab.ChoosedItem = activeTab.Items[index];
						EnsureVisible(activeTab.ChoosedItem);
						Refresh();
					}
					break;
				case Keys.PageDown:
					if (activeTab.Items.Count > 0 && !isInRenameMode) {
						index = Math.Min(activeTab.Items.Count - 1, activeTab.Items.IndexOf(activeTab.ChoosedItem) + scrollBar.LargeChange);
						activeTab.ChoosedItem = activeTab.Items[index];
						EnsureVisible(activeTab.ChoosedItem);
						Refresh();
					}
					break;
				case Keys.Down:
					if (activeTab.Items.Count > 0 && !isInRenameMode) {
						if (activeTab.ChoosedItem != null) {
							activeTab.ChoosedItem = activeTab.Items[Math.Min(activeTab.Items.Count - 1, activeTab.Items.IndexOf(activeTab.ChoosedItem) + 1)];
						} else {
							activeTab.ChoosedItem = activeTab.Items[0];
						}
						activeTab.SelectedItem = null;
						EnsureVisible(activeTab.ChoosedItem);
						Refresh();
					}
					return true;
				case Keys.Up:
					if (activeTab.Items.Count > 0 && !isInRenameMode) {
						if (activeTab.ChoosedItem != null) {
							activeTab.ChoosedItem = activeTab.Items[Math.Max(0, activeTab.Items.IndexOf(activeTab.ChoosedItem) - 1)];
						} else {
							activeTab.ChoosedItem = activeTab.Items[0];
						}
						activeTab.SelectedItem = null;
						EnsureVisible(activeTab.ChoosedItem);
						Refresh();
					}
					return true;
				case Keys.Control | Keys.Up:
					ActiveTab = Tabs[Math.Max(0, Tabs.IndexOf(ActiveTab) - 1)];
					Refresh();
					return true;
				case Keys.Control | Keys.Down:
					ActiveTab = Tabs[Math.Min(Tabs.Count - 1, Tabs.IndexOf(ActiveTab) + 1)];
					Refresh();
					return true;
				case Keys.Return:
					if (renameTab != null) {
						renameTab.Name = renameTextBox.Text;
						ExitRenameMode();
					} else if (renameTabItem != null) {
						renameTabItem.Name = renameTextBox.Text;
						ExitRenameMode();
					}
					return true;
				case Keys.Escape:
					if (renameTab != null) {
						if (doAddTab) {
							Tabs.RemoveAt(Tabs.Count - 1);
							renameTab = null;
							renameTextBox.Visible = false;
							doAddTab  = false;
							Refresh();
						} else {
							ExitRenameMode();
						}
					} else if (renameTabItem != null) {
						ExitRenameMode();
					}
					return true;
			}
			return false;
		}
		
		public void StartRenamingOf(SideTabItem item)
		{
			EnsureVisible(item);
			renameTabItem = item;
			
			Point   location = activeTab.GetLocation(item);
			location.X += Bounds.X + 5 + sideTabContent.Location.X + 16;
			location.Y += Bounds.Y + 3 + sideTabContent.Location.Y - scrollBar.Value * 20;
			renameTextBox.Location = location;
			
			renameTextBox.Width    = Width - 10;
			renameTextBox.Height   = Font.Height - 2;
			renameTextBox.Text     = item.Name;
			renameTextBox.Visible  = true;
			renameTextBox.Focus();
		}
		
		
		public void StartRenamingOf(SideTab tab)
		{
			int index = Tabs.IndexOf(tab);
			renameTab        = Tabs[index];
			Point   location = GetLocation(renameTab);
			location.X += 3;
			location.Y += 1;
			renameTextBox.Location = location;
			renameTextBox.Width    = Width - 10;
			renameTextBox.Height   = Font.Height - 2;
			renameTextBox.Text     = renameTab.Name;
			renameTextBox.Visible  = true;
			renameTextBox.Focus();
		}
		
		void ItemContextMenuPopup(object sender, EventArgs e)
		{
			activeTab.ChoosedItem = activeTab.SelectedItem;
			Refresh();
		}
		
		public Point GetLocation(SideTab whichTab)
		{
			int i = 0;
			
			int lastUpperY = 0;
			
			for (; i < sideTabs.Count; ++i) {
				SideTab tab = sideTabs[i];
				
				int yPos = i * (Font.Height + 4 + 1);
				if (tab == whichTab) {
					return new Point(0, yPos);
				}
				lastUpperY = yPos + Font.Height + 4;
				if (tab == activeTab) {
					break;
				}
			}
			
			int bottom = Height;
			
			for (int j = sideTabs.Count - 1; j > i; --j) {
				SideTab tab = sideTabs[j];
				
				int yPos = Height - (-j + sideTabs.Count ) * (Font.Height + 4 + 1);
				
				if (yPos < lastUpperY + (Font.Height + 4 + 1))
					break;
				
				bottom = yPos;
				if (tab == whichTab) {
					return new Point(0, yPos);
				}
			}
			
			return new Point(-1, -1);
		}
		
		public SideTab GetTabAt(int x, int y)
		{
			int lastUpperY = 0;
			int i = 0;
			for (; i < sideTabs.Count; ++i) {
				SideTab tab = sideTabs[i];
				
				int yPos = i * (Font.Height + 4 + 1);
				
				lastUpperY = yPos + Font.Height + 4;
				
				if (y >= yPos && y <= lastUpperY)
					return tab;
				if (tab == activeTab) {
					break;
				}
			}
			
			for (int j = sideTabs.Count - 1; j > i; --j) {
				SideTab tab = sideTabs[j];
				
				int yPos = Height - (-j + sideTabs.Count ) * (Font.Height + 4 + 1);
				
				if (yPos < lastUpperY)
					break;
				if (y >= yPos && y <= yPos + Font.Height + 4)
					return tab;
			}
			return null;
		}
		
		public int GetTabIndexAt(int x, int y)
		{
			int lastUpperY = 0;
			int i = 0;
			for (; i < sideTabs.Count; ++i) {
				SideTab tab = sideTabs[i];
				
				int yPos = i * (Font.Height + 4 + 1);
				
				lastUpperY = yPos + Font.Height + 4;
				
				if (y >= yPos && y <= lastUpperY)
					return i;
				if (tab == activeTab)
					break;
			}
			
			for (int j = sideTabs.Count - 1; j > i; --j) {
				SideTab tab = sideTabs[j];
				
				int yPos = Height - (-j + sideTabs.Count ) * (Font.Height + 4 + 1);
				
				if (yPos < lastUpperY + (Font.Height + 4 + 1))
					break;
				if (y >= yPos && y <= yPos + Font.Height + 4)
					return j;
			}
			return -1;
		}
		
		static DragDropEffects GetDragDropEffect(DragEventArgs e)
		{
			if ((e.AllowedEffect & DragDropEffects.Move) > 0 &&
			    (e.AllowedEffect & DragDropEffects.Copy) > 0) {
				return (e.KeyState & 8) > 0 ? DragDropEffects.Copy : DragDropEffects.Move;
			} else if ((e.AllowedEffect & DragDropEffects.Move) > 0) {
				return DragDropEffects.Move;
			} else if ((e.AllowedEffect & DragDropEffects.Copy) > 0) {
				return DragDropEffects.Copy;
			}
			return DragDropEffects.None;
		}
		
		protected override void OnDragEnter(DragEventArgs e)
		{
			ExitRenameMode();
			
			base.OnDragEnter(e);
			
			Point p = PointToClient(new Point(e.X, e.Y));
			
			if (e.Data.GetDataPresent(typeof(SideTabItem))) {
				e.Effect = (e.KeyState & 8) > 0 ? DragDropEffects.Copy : DragDropEffects.Move;
			} else if (e.Data.GetDataPresent(typeof(SideTab))) {
				SideTab tab = (SideTab)e.Data.GetData(typeof(SideTab));
				if (Tabs.Contains(tab)) {
					Tabs.DragOverTab = tab;
					e.Effect = GetDragDropEffect(e);
				} else {
					e.Effect = DragDropEffects.None;
				}
			} else if (e.Data.GetDataPresent(typeof(string))) {
				e.Effect = GetDragDropEffect(e);
			} else {
				e.Effect = DragDropEffects.None;
			}
		}
		
		protected override void OnDragLeave(EventArgs e)
		{
			
			base.OnDragLeave(e);
			Tabs.DragOverTab = null;
			ClearDraggings(activeTab);
			Refresh();
		}
		
		protected override void OnDragDrop(DragEventArgs e)
		{
			base.OnDragDrop(e);
			
			Point p = PointToClient(new Point(e.X, e.Y));
			if (e.Data.GetDataPresent(typeof(SideTabItem))) {
				
				SideTabItem draggedItem = (SideTabItem)e.Data.GetData(typeof(SideTabItem));
				
				// drag tabitem into other sideTab
				SideTab tab = GetTabAt(p.X, p.Y);
				if (tab != null) {
					if (tab == Tabs.DragOverTab && tab.CanDragDrop) {
						Tabs.DragOverTab.SideTabStatus = SideTabStatus.Normal;
						draggedItem.SideTabItemStatus = SideTabItemStatus.Normal;
						switch (e.Effect) {
							case DragDropEffects.Move:
								if (Tabs.DragOverTab != activeTab) {
									activeTab.Items.Remove(draggedItem);
									Tabs.DragOverTab.Items.Add(draggedItem);
								}
								break;
							case DragDropEffects.Copy:
								SideTabItem newItem = draggedItem.Clone();
								Tabs.DragOverTab.Items.Add(newItem);
								break;
						}
						Tabs.DragOverTab = null;
						Refresh();
					}
				}
			} else if (e.Data.GetDataPresent(typeof(string))) {
				if (Tabs.DragOverTab != null) {
					string str = (string)e.Data.GetData(typeof(string));
					Tabs.DragOverTab.Items.Add("Text:" + str.Trim(), str);
				}
				Tabs.DragOverTab = null;
				Refresh();
			} else {
				Tabs.DragOverTab = null;
				Refresh();
			}
		}
		
		void ClearDraggings(SideTab tab)
		{
			foreach (SideTabItem item in tab.Items) {
				if (item.SideTabItemStatus == SideTabItemStatus.Drag) {
					item.SideTabItemStatus = SideTabItemStatus.Normal;
				}
			}
		}
		
		protected override void OnDragOver(DragEventArgs e)
		{
			ExitRenameMode();
			base.OnDragOver(e);
			
			Point p = PointToClient(new Point(e.X, e.Y));
			if (e.Data.GetDataPresent(typeof(SideTabItem))) {
				ClearDraggings(activeTab);
				SideTab tab = GetTabAt(p.X, p.Y);
				if (tab != null && tab != Tabs.DragOverTab) {
					if (tab.CanDragDrop) {
						Tabs.DragOverTab = tab;
					} else {
						Tabs.DragOverTab = null;
					}
					Refresh();
				}
				if (Tabs.DragOverTab != null && Tabs.DragOverTab.CanDragDrop) {
					e.Effect = GetDragDropEffect(e);
				} else {
					e.Effect = DragDropEffects.None;
				}
			} else if (e.Data.GetDataPresent(typeof(string))) {
				SideTab oldTab = Tabs.DragOverTab;
				if (activeTabMemberArea.Contains(p.X, p.Y)) {
					Tabs.DragOverTab = activeTab;
				} else {
					Tabs.DragOverTab = GetTabAt(p.X, p.Y);
				}
				if (oldTab != Tabs.DragOverTab) {
					Refresh();
				}
			} else if (e.Data.GetDataPresent(typeof(SideTab))) {
				int tabIndex = GetTabIndexAt(p.X, p.Y);
				if (tabIndex != -1) {
					SideTab tab = Tabs.DragOverTab;
					Tabs.Remove(tab);
					Tabs.Insert(tabIndex, tab);
					Refresh();
				}
				e.Effect = DragDropEffects.Move;
			}
		}
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			mousePosition = new Point(e.X, e.Y);
			if (e.Button == MouseButtons.Left) {
				int tab = -1;
				for (int i = 0; i < sideTabs.Count; ++i) {
					if (sideTabs[i].SideTabStatus == SideTabStatus.Selected) {
						tab = i;
						break;
					}
				}
				
				if (tab != -1) {
					if (IsDragStarted(mouseDownPosition, e.Location)) {
						Tabs.DragOverTab = Tabs[tab];
						DoDragDrop(Tabs.DragOverTab, DragDropEffects.All);
					}
					Refresh();
				}
			}
		}
		
		internal static bool IsDragStarted(Point mouseDownPos, Point mouseMovePos)
		{
			Size dragSize = SystemInformation.DragSize;
			if (dragSize.Width < 3) dragSize.Width = 3;
			if (dragSize.Height < 3) dragSize.Height = 3;
			mouseDownPos.Offset(-dragSize.Width / 2, -dragSize.Width / 2);
			Rectangle r = new Rectangle(mouseDownPos, dragSize);
			return !r.Contains(mouseMovePos);
		}
		
		//
		//	protected override void OnLostFocus(EventArgs e)
		//	{
		//		base.OnLostFocus(e);
		//		ExitRenameMode();
		//		Refresh();
		//	}
		
		MouseWheelHandler mouseWheelHandler = new MouseWheelHandler();
		
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if (scrollBar.Visible) {
				mouseWheelHandler.Scroll(scrollBar, e);
				ScrollBarScrolled(null, null);
			}
		}
		
		Point mouseDownPosition;
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button == MouseButtons.Left) {
				mouseDownPosition = e.Location;
				
				SideTab tab = GetTabAt(e.X, e.Y);
				if (tab != null) {
					mouseDownTab = tab;
					tab.SideTabStatus = SideTabStatus.Selected;
					Refresh();
				}
			}
		}
		SideTab mouseDownTab = null;
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (mouseDownTab != null) {
				ActiveTab    = mouseDownTab;
				mouseDownTab.SideTabStatus = SideTabStatus.Normal;
				mouseDownTab = null;
			}
			
			ExitRenameMode();
			Refresh();
			base.OnMouseUp(e);
		}
		
		Rectangle activeTabMemberArea;
		
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			int i = 0;
			
			int lastUpperY = 0;
			
			for (; i < sideTabs.Count; ++i) {
				SideTab tab = sideTabs[i];
				
				int yPos = i * (Font.Height + 4 + 1);
				tab.DrawTabHeader(g, Font, new Point(0, yPos), Width);
				lastUpperY = yPos + Font.Height + 4;
				
				if (tab == activeTab) {
					break;
				}
			}
			
			int bottom = Height;
			
			for (int j = sideTabs.Count - 1; j > i; --j) {
				SideTab tab = sideTabs[j];
				
				int yPos = Height - (-j + sideTabs.Count ) * (Font.Height + 4 + 1);
				
				if (yPos < lastUpperY + (Font.Height + 4 + 1))
					break;
				
				bottom = yPos;
				tab.DrawTabHeader(g, Font, new Point(0, yPos), Width);
			}
			
			if (activeTab != null) {
				bool b = scrollBar.Maximum > (bottom - lastUpperY) / 20 || scrollBar.Value != 0;
				scrollBar.Visible  = b;
				activeTabMemberArea = new Rectangle(0, lastUpperY,
				                                    Width - (scrollBar.Visible ? (SystemInformation.VerticalScrollBarWidth) : 0)  - 4, bottom - lastUpperY);
				sideTabContent.Bounds  = activeTabMemberArea;
				scrollBar.Location     = new Point(Width - SystemInformation.VerticalScrollBarWidth - 4,
				                                   lastUpperY);
				scrollBar.Width    = SystemInformation.VerticalScrollBarWidth;
				scrollBar.Height   = activeTabMemberArea.Height;
			}
		}
		
		
		void ScrollBarScrolled(object sender, ScrollEventArgs e)
		{
			activeTab.ScrollIndex = scrollBar.Value;
			sideTabContent.Refresh();
		}
		
		protected virtual object StartItemDrag(SideTabItem draggedItem)
		{
			SpecialDataObject dataObject = new SpecialDataObject();
			dataObject.SetData(draggedItem.Tag);
			dataObject.SetData(draggedItem);
			return dataObject;
		}
		
		protected class SideTabContent : UserControl
		{
			SideBarControl sideBar = null;
			Point   mousePosition;
			
			public SideBarControl SideBar {
				get {
					return sideBar;
				}
				set {
					sideBar = value;
				}
			}
			
			public SideTabContent()
			{
				ResizeRedraw = true;
				AllowDrop = true;
				
				SetStyle(ControlStyles.UserPaint, true);
				SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
				SetStyle(ControlStyles.AllPaintingInWmPaint, true);
				SetStyle(ControlStyles.CacheText, true);
			}
			
			
			////////////////////////////////////////////////////////////////////////////////
			
			protected override void OnPaint(PaintEventArgs e)
			{
				if (sideBar != null && sideBar.activeTab != null) {
					sideBar.activeTab.DrawTabContent(e.Graphics, Font, new Rectangle(0, 0, Width, Height));
				}
			}
			
			////////////////////////////////////////////////////////////////////////////
			// Drag and Drop
			protected override void OnDragEnter(DragEventArgs e)
			{
				base.OnDragEnter(e);
				sideBar.ExitRenameMode();
				if (sideBar.activeTab != null && sideBar.activeTab.CanDragDrop) {
					if (e.Data.GetDataPresent(typeof(string)) || e.Data.GetDataPresent(typeof(SideTabItem))) {
						e.Effect = GetDragDropEffect(e);
					} else {
						e.Effect = DragDropEffects.None;
					}
				} else {
					e.Effect = DragDropEffects.None;
				}
			}
			
			protected override void OnDragLeave(EventArgs e)
			{
				base.OnDragLeave(e);
				if (sideBar.activeTab != null) {
					sideBar.Tabs.DragOverTab = null;
					sideBar.ClearDraggings(sideBar.activeTab);
					Refresh();
				}
			}
			
			protected override void OnDragDrop(DragEventArgs e)
			{
				base.OnDragDrop(e);
				
				Point p = PointToClient(new Point(e.X, e.Y));
				if (e.Data.GetDataPresent(typeof(SideTabItem))) {
					SideTabItem draggedItem = (SideTabItem)e.Data.GetData(typeof(SideTabItem));
					switch (e.Effect) {
						case DragDropEffects.Move:
							SideTabItem item = sideBar.activeTab.GetItemAt(p.X, p.Y);
							
							if (item != sideBar.activeTab.ChoosedItem) {
								int idx = sideBar.activeTab.Items.DraggedIndex;
								if (idx != -1) {
									sideBar.activeTab.Items.Remove(draggedItem);
									sideBar.activeTab.Items.Insert(idx, draggedItem);
								}
							}
							break;
						case DragDropEffects.Copy:
							SideTabItem newItem = draggedItem.Clone();
							newItem.SideTabItemStatus = SideTabItemStatus.Normal;
							sideBar.activeTab.Items.Add(newItem);
							break;
					}
					sideBar.ClearDraggings(sideBar.activeTab);
					sideBar.Tabs.DragOverTab = null;
					sideBar.Refresh();
				} else if (e.Data.GetDataPresent(typeof(string))) {
					if (sideBar.Tabs.DragOverTab != null) {
						string str = (string)e.Data.GetData(typeof(string));
						sideBar.Tabs.DragOverTab.Items.Add("Text:" + str.Trim(), str);
					}
					sideBar.Tabs.DragOverTab = null;
					Refresh();
				} else {
					sideBar.Tabs.DragOverTab = null;
					sideBar.Refresh();
				}
			}
			
			void ClearDraggings(SideTab tab)
			{
				foreach (SideTabItem item in tab.Items) {
					if (item.SideTabItemStatus == SideTabItemStatus.Drag) {
						item.SideTabItemStatus = SideTabItemStatus.Normal;
					}
				}
			}
			
			protected override void OnDragOver(DragEventArgs e)
			{
				base.OnDragOver(e);
				sideBar.ExitRenameMode();
				Point p = PointToClient(new Point(e.X, e.Y));
				
				if (e.Data.GetDataPresent(typeof(SideTabItem))) {
					// drag move item inside the activeTabMembarArea
					if (sideBar.activeTab.CanDragDrop) {
						SideTabItem item = sideBar.activeTab.GetItemAt(p.X, p.Y);
						if (item == null) {
							sideBar.ClearDraggings(sideBar.activeTab);
							sideBar.Refresh();
						} else
							if (item != sideBar.activeTab.ChoosedItem) {
							if (item.SideTabItemStatus != SideTabItemStatus.Drag) {
								sideBar.ClearDraggings(sideBar.activeTab);
								item.SideTabItemStatus = SideTabItemStatus.Drag;
								sideBar.Tabs.DragOverTab       = sideBar.activeTab;
								sideBar.Refresh();
							}
						} else {
							sideBar.ClearDraggings(sideBar.activeTab);
							sideBar.activeTab.SideTabStatus = SideTabStatus.Dragged;
							sideBar.Refresh();
						}
						
						e.Effect = GetDragDropEffect(e);
					} else {
						e.Effect = DragDropEffects.None;
					}
				} else if (e.Data.GetDataPresent(typeof(string))) {
					if (sideBar.activeTab != sideBar.Tabs.DragOverTab) {
						sideBar.Tabs.DragOverTab = sideBar.activeTab;
						sideBar.Refresh();
					}
				}
				
				//			else if (e.Data.GetDataPresent(typeof(SideTab))) {
				//				int tabIndex = GetTabIndexAt(p.X, p.Y);
				//				if (tabIndex != -1) {
				//					SideTab tab = Tabs.DragOverTab;
				//					Tabs.Remove(tab);
				//					Tabs.Insert(tabIndex, tab);
				//					Refresh();
				//				}
				//				e.Effect = DragDropEffects.Move;
				//			}
			}
			
			
			////////////////////////////////////////////////////////////////////////////
			// Mouse Handling
			protected override void OnMouseLeave(EventArgs e)
			{
				base.OnMouseLeave(e);
				if (sideBar.activeTab != null) {
					sideBar.activeTab.SelectedItem = null;
				}
				Refresh();
			}
			
			Point mouseDownPos;
			
			protected override void OnMouseMove(MouseEventArgs e)
			{
				base.OnMouseMove(e);
				if (sideBar.activeTab == null) return;
				if (e.Button == MouseButtons.Left) {
					SideTabItem item = sideBar.activeTab.GetItemAt(e.X, e.Y);
					
					if (item != null) {
						if (IsDragStarted(mouseDownPos, e.Location)) {
							sideBar.Tabs.DragOverTab = sideBar.activeTab;
							
							DoDragDrop(sideBar.StartItemDrag(item), sideBar.activeTab.CanDragDrop ? DragDropEffects.All : (DragDropEffects.Copy | DragDropEffects.None));
						}
						Refresh();
					}
				} else {
					SideTabItem oldItem = sideBar.activeTab.SelectedItem;
					sideBar.activeTab.SelectedItem = null;
					mousePosition = new Point(e.X, e.Y);
					SideTabItem item = sideBar.activeTab.GetItemAt(e.X, e.Y);
					
					if (item != null) {
						sideBar.activeTab.SelectedItem = item;
					}
					
					if (oldItem != sideBar.activeTab.SelectedItem) {
						sideBar.Refresh();
					}
				}
			}
			
			protected override void OnMouseDown(MouseEventArgs e)
			{
				base.OnMouseDown(e);
				if (e.Button == MouseButtons.Left && sideBar.activeTab != null) {
					mouseDownPos = e.Location;
					sideBar.activeTab.ChoosedItem = sideBar.activeTab.SelectedItem;
				}
				Refresh();
			}
			
			protected override void OnMouseUp(MouseEventArgs e)
			{
				if (sideBar != null) {
					sideBar.ExitRenameMode();
					Refresh();
				}
				base.OnMouseUp(e);
			}
		}
		
		public class SideTabCollection : ICollection<SideTab>, IEnumerable<SideTab>
		{
			List<SideTab> list = new List<SideTab>();
			SideTab dragOverTab;
			SideBarControl sideBar;
			
			public SideTab this[int index] {
				get {
					return list[index];
				}
				set {
					list[index] = value;
				}
			}
			
			public SideTab DragOverTab {
				get {
					return dragOverTab;
				}
				set {
					if (dragOverTab != null) {
						dragOverTab.SideTabStatus = SideTabStatus.Normal;
					}
					dragOverTab = value;
					if (dragOverTab != null) {
						dragOverTab.SideTabStatus = SideTabStatus.Dragged;
					}
				}
			}
			public SideTabCollection(SideBarControl sideBar)
			{
				this.sideBar = sideBar;
			}
			public int Count {
				get {
					return list.Count;
				}
			}
			
			public virtual bool IsSynchronized {
				get {
					return false;
				}
			}
			
			public virtual object SyncRoot {
				get {
					return this;
				}
			}
			
			public virtual void Add(SideTab item)
			{
				list.Add(item);
			}
			
			public virtual SideTab Add(string name)
			{
				SideTab tab = sideBar.SideTabFactory.CreateSideTab(sideBar, name);
				Add(tab);
				return tab;
			}
			
			public virtual void Clear()
			{
				list.Clear();
			}
			
			public bool Contains(SideTab item)
			{
				return list.Contains(item);
			}
			
			public IEnumerator<SideTab> GetEnumerator()
			{
				return list.GetEnumerator();
			}
			
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return list.GetEnumerator();
			}
			
			public int IndexOf(SideTab item)
			{
				return list.IndexOf(item);
			}
			
			public void CopyTo(Array dest, int index)
			{
				list.CopyTo((SideTab[])dest, index);
			}
			
			public virtual SideTab Insert(int index, SideTab item)
			{
				list.Insert(index, item);
				return item;
			}
			
			public virtual SideTab Insert(int index, string name)
			{
				return Insert(index, sideBar.SideTabFactory.CreateSideTab(sideBar, name));
			}
			
			public bool Remove(SideTab item)
			{
				if (item == sideBar.ActiveTab) {
					int index =  IndexOf(item);
					if (index > 0) {
						sideBar.ActiveTab = this[index - 1];
					} else if (index < Count - 1) {
						sideBar.ActiveTab = this[index + 1];
					} else {
						sideBar.ActiveTab = null;
					}
				}
				return list.Remove(item);
			}
			
			public virtual void RemoveAt(int index)
			{
				list.RemoveAt(index);
			}
			
			public bool IsReadOnly {
				get {
					return false;
				}
			}
			
			public void CopyTo(SideTab[] array, int arrayIndex)
			{
				list.CopyTo(array, arrayIndex);
			}
		}
	}
	
	public class SpecialDataObject : System.Windows.Forms.IDataObject
	{
		List<object> dataObjects = new List<object>();
		public object GetData(string format)
		{
			return GetData(format, true);
		}
		
		public object GetData(System.Type format)
		{
			foreach (object o in dataObjects) {
				if (o.GetType() == format) {
					return o;
				}
			}
			return null;
		}
		
		public object GetData(string str, bool autoConvert)
		{
			foreach (object o in dataObjects) {
				if (o == null) {
					continue;
				}
				Type type = o.GetType();
				string typeStr = type.ToString();
				if (typeStr == str) {
					return o;
				}
				
				if (typeStr == "ICSharpCode.SharpDevelop.Gui.SharpDevelopSideTabItem" && str == "TimeSprint.Alexandria.UI.SideBar.AxSideTabItem") {
					return o;
				}
				
				if (type.BaseType != null) {
					typeStr = type.BaseType.ToString();
					if (typeStr == str) {
						return o;
					}
				}
			}
			return null;
		}
		
		public bool GetDataPresent(string format)
		{
			return GetDataPresent(format, true);
		}
		
		public bool GetDataPresent(System.Type format)
		{
			return GetData(format) != null;
		}
		
		public bool GetDataPresent(string format, bool autoConvert)
		{
			return GetData(format, autoConvert) != null;
		}
		
		public string[] GetFormats()
		{
			return new string[0];
		}
		public string[] GetFormats(bool autoConvert)
		{
			return new string[0];
		}
		
		public void SetData(object data)
		{
			dataObjects.Add(data);
		}
		
		public void SetData(string format, object data)
		{
			
		}
		public void SetData(System.Type format, object data)
		{
			
		}
		public void SetData(string format, bool autoConvert, object data)
		{
			
		}
	}
}
