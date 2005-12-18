// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class SharpDevelopSideTabItemFactory : ISideTabItemFactory
	{
		public AxSideTabItem CreateSideTabItem(string name)
		{
			return new SharpDevelopSideTabItem(name);
		}
		
		public AxSideTabItem CreateSideTabItem(string name, object tag)
		{
			return new SharpDevelopSideTabItem(name, tag);
		}
		
		public AxSideTabItem CreateSideTabItem(string name, object tag, Bitmap bitmap)
		{
			return new SharpDevelopSideTabItem(name, tag, bitmap);
		}
	}
	
	public class SharpDevelopSideBar : AxSideBar, IOwnerState
	{
		readonly static string contextMenuPath        = "/SharpDevelop/Workbench/SharpDevelopSideBar/ContextMenu";
		readonly static string sideTabContextMenuPath = "/SharpDevelop/Workbench/SharpDevelopSideBar/SideTab/ContextMenu";
		
		Point mousePosition;
		Point itemMousePosition;
		public AxSideTab ClipboardRing = null;
		
		public Point ItemMousePosition {
			get {
				return itemMousePosition;
			}
		}
		public Point SideBarMousePosition {
			get {
				return mousePosition;
			}
		}
		
		[Flags]
		public enum SidebarState {
			Nothing       = 0,
			CanMoveUp     = 1,
			CanMoveDown   = 2,
			TabCanBeDeleted = 4,
			CanMoveItemUp = 8,
			CanMoveItemDown = 16
		}
		
		protected SidebarState internalState = SidebarState.TabCanBeDeleted;

		public System.Enum InternalState {
			get {
				return internalState;
			}
		}
		
		Hashtable   standardTabs = new Hashtable();
		
		public static SharpDevelopSideBar SideBar;
		
		public SharpDevelopSideBar(XmlElement el) : this()
		{
			SetOptions(el);
		}
		
		public SharpDevelopSideBar()
		{
			SideBar = this;
			
			SideTabItemFactory = new SharpDevelopSideTabItemFactory();
			
			MouseUp                  += new MouseEventHandler(SetContextMenu);
			sideTabContent.MouseUp   += new MouseEventHandler(SetItemContextMenu);
			
			/*
			deleteMenuHeaderItem = new IconMenuItem(mainwindow, ResourceService.GetString("SideBarComponent.ContextMenu.DeleteTab"), new EventHandler(DeleteTabHeader));
			moveUpMenuHeaderItem = new IconMenuItem(mainwindow, ResourceService.GetString("SideBarComponent.ContextMenu.MoveTabUp"), new EventHandler(MoveTabUp));
			moveDownMenuHeaderItem = new IconMenuItem(mainwindow, ResourceService.GetString("SideBarComponent.ContextMenu.MoveTabDown"), new EventHandler(MoveTabDown));
			
			moveItemTabUpMenuHeaderItem = new IconMenuItem(mainwindow, ResourceService.GetString("SideBarComponent.ContextMenu.MoveTabUp"), new EventHandler(MoveActiveTabUp));
			moveItemTabDownMenuHeaderItem = new IconMenuItem(mainwindow, ResourceService.GetString("SideBarComponent.ContextMenu.MoveTabDown"), new EventHandler(MoveActiveTabDown));
			
			ContextMenu = new ContextMenu(new MenuItem[] {
				deleteMenuHeaderItem,
				new IconMenuItem(mainwindow, ResourceService.GetString("SideBarComponent.ContextMenu.RenameTab"), new EventHandler(RenameTabHeader)),
				new IconMenuItem(mainwindow, "-"),
				new IconMenuItem(mainwindow, ResourceService.GetString("SideBarComponent.ContextMenu.AddTab"), new EventHandler(AddTabHeader)),
				new IconMenuItem(mainwindow, "-"),
				moveUpMenuHeaderItem,
				moveDownMenuHeaderItem
			});
			
			
			
			sideTabContent.MouseMove += new MouseEventHandler(MoveItem);
			
			sideTabContent.ContextMenu = new ContextMenu(new MenuItem[] {
					new IconMenuItem(mainwindow, ResourceService.GetString("SideBarComponent.ContextMenu.RenameTabItem"), new EventHandler(RenameTabItem)),
					new IconMenuItem(mainwindow, ResourceService.GetString("SideBarComponent.ContextMenu.DeleteTabItem"), new EventHandler(DeleteTabItem)),
					new IconMenuItem(mainwindow, "-"),
					moveItemTabUpMenuHeaderItem,
					moveItemTabDownMenuHeaderItem
			});
			*/
			
			foreach (TextTemplate template in TextTemplate.TextTemplates) {
				AxSideTab tab = new AxSideTab(this, template.Name);
				tab.CanSaved  = false;
				foreach (TextTemplate.Entry entry in template.Entries)  {
					tab.Items.Add(SideTabItemFactory.CreateSideTabItem(entry.Display, entry.Value));
				}
				tab.CanBeDeleted = tab.CanDragDrop = false;
				standardTabs[tab] = true;
				Tabs.Add(tab);
			}
			sideTabContent.DoubleClick += new EventHandler(MyDoubleClick);
		}
		
		public void MyDoubleClick(object sender, EventArgs e)
		{
//			if (mainWindow.ActiveContentWindow == null) {
//				return;
//			}
//			string text = ActiveTab.SelectedItem.Tag.ToString();
//			
//			mainWindow.ActiveContentWindow.IEditable.ClipboardHandler.Delete(this, null);
//			
//			TextAreaControl sharptextarea = (TextAreaControl)mainWindow.ActiveContentWindow.ISdEditable;
//			
//			int curLineNr     = sharptextarea.Document.GetLineNumberForOffset(sharptextarea.Document.Caret.Offset);
//			sharptextarea.Document.Insert(sharptextarea.Document.Caret.Offset, text);
//			
//			sharptextarea.Document.Caret.Offset += text.Length;
//			
//			if (curLineNr != sharptextarea.Document.GetLineNumberForOffset(sharptextarea.Document.Caret.Offset)) {
//				sharptextarea.UpdateToEnd(curLineNr);
//			} else {
//				sharptextarea.UpdateLines(curLineNr, curLineNr);
//			}
		}
		
		public void PutInClipboardRing(string text)
		{
			foreach (AxSideTab tab in Tabs) {
				if (tab.IsClipboardRing) {
					tab.Items.Add("Text:" + text.Trim(), text);
					if (tab.Items.Count > 20) {
						tab.Items.RemoveAt(0);
					}
					return;
				}
			}
			System.Diagnostics.Debug.Assert(false, "Can't find clipboard ring side tab category");
		}
		
		////////////////////////////////////////////////////////////////////////////
		// Tab Context Menu
		
		void SetDeletedState(AxSideTab tab)
		{
			if (tab.CanBeDeleted) {
				internalState |= SidebarState.TabCanBeDeleted;
			} else {
				internalState = internalState & ~SidebarState.TabCanBeDeleted;
			}
		}
		
		void SetContextMenu(object sender, MouseEventArgs e)
		{
			ExitRenameMode();
			
			int index = GetTabIndexAt(e.X, e.Y);
			if (index >= 0) {
				AxSideTab tab = Tabs[index];
				
				SetDeletedState(tab);
				
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
				MenuService.ShowContextMenu(this, contextMenuPath, this, e.X, e.Y);
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
				SetDeletedState(ActiveTab);
				
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
				
				MenuService.ShowContextMenu(this, sideTabContextMenuPath, sideTabContent, e.X, e.Y);
			}
		}
		
		void MoveItem(object sender, MouseEventArgs e)
		{
			itemMousePosition = new Point(e.X, e.Y);
		}
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			mousePosition = new Point(e.X, e.Y);
		}
		
		void SetOptions(XmlElement el)
		{
			foreach (XmlElement sideTabEl in el.ChildNodes) {
				AxSideTab tab = new AxSideTab(this, sideTabEl.GetAttribute("text"));
				if (tab.Name == el.GetAttribute("activetab")) {
					ActiveTab = tab;
				} else {
					if (ActiveTab == null) {
						ActiveTab = tab;
					}
				}
				
				foreach (XmlElement sideTabItemEl in sideTabEl.ChildNodes) {
					tab.Items.Add(SideTabItemFactory.CreateSideTabItem(sideTabItemEl.GetAttribute("text"), 
					                                                   sideTabItemEl.GetAttribute("value")));
				}
				
				if (sideTabEl.GetAttribute("clipboardring") == "true") {
					tab.CanBeDeleted = false;
					tab.CanDragDrop  = false;
					tab.Name            = "${res:SharpDevelop.SideBar.ClipboardRing}";
					tab.IsClipboardRing = true;
				}
				Tabs.Add(tab);
			}
		}
		public XmlElement ToXmlElement(XmlDocument doc)
		{
			if (doc == null) {
				throw new ArgumentNullException("doc");
			}
			XmlElement el = doc.CreateElement("SideBar");
			el.SetAttribute("activetab", ActiveTab.Name);
			
			foreach (AxSideTab tab in Tabs) {
				if (tab.CanSaved && standardTabs[tab] == null) {
					XmlElement child = doc.CreateElement("SideTab");
					
					if (tab.IsClipboardRing) {
						child.SetAttribute("clipboardring", "true");
					}
					
					child.SetAttribute("text", tab.Name);
					
					foreach (AxSideTabItem item in tab.Items) {
						XmlElement itemChild = doc.CreateElement("SideTabItem");
						
						itemChild.SetAttribute("text",  item.Name);
						itemChild.SetAttribute("value", item.Tag.ToString());
						
						child.AppendChild(itemChild);
					}
					el.AppendChild(child);
				}
			}
			
			return el;
		}
		
	}
}
