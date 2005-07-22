// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public enum SideTabStatus {
		Normal,
		Selected,
		Dragged
	}
	
	public class AxSideTab
	{
		string    name;
		bool      canDragDrop  = true;
		bool      canBeDeleted = true;
		bool      isClipboardRing = false;
		SideTabItemCollection items = new SideTabItemCollection();
		SideTabStatus sideTabStatus;
		AxSideTabItem   selectedItem = null;
		AxSideTabItem   choosedItem  = null;
		
		ImageList largeImageList = null;
		ImageList smallImageList = null;
		int       scrollIndex    = 0;
		
		public bool Hidden = false;
		
		public bool IsClipboardRing {
			get {
				return isClipboardRing;
			}
			set {
				isClipboardRing = value;
			}
		}
		
		public int ScrollIndex {
			get {
				return scrollIndex;
			}
			set {
				scrollIndex = value;
			}
		}
		
		public ImageList LargeImageList {
			get {
				return largeImageList;
			}
			set {
				largeImageList = value;
			}
		}
		
		public ImageList SmallImageList {
			get {
				return smallImageList;
			}
			set {
				smallImageList = value;
			}
		}
		
		public SideTabStatus SideTabStatus {
			get {
				return sideTabStatus;
			}
			
			set {
				sideTabStatus = value;
			}
		}
		
		public bool   CanBeDeleted {
			get {
				return canBeDeleted;
			}
			set {
				canBeDeleted = value;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public SideTabItemCollection Items  {
			get {
				return items;
			}
		}
		
		public bool CanDragDrop {
			get {
				return canDragDrop;
			}
			set {
				canDragDrop = value;
			}
		}
		
		bool canSaved = true;
		
		public bool CanSaved {
			get {
				return canSaved;
			}
			set {
				canSaved = value;
			}
		}
		
		public AxSideTabItem SelectedItem {
			get {
				return selectedItem;
			}
			set {
				if (selectedItem != null && selectedItem != choosedItem) {
					selectedItem.SideTabItemStatus = SideTabItemStatus.Normal;
				}
				selectedItem = value;
				if (selectedItem != null && selectedItem != choosedItem) {
					selectedItem.SideTabItemStatus = SideTabItemStatus.Selected;
				}
			}
		}
		
		protected  void OnChoosedItemChanged(EventArgs e)
		{
			if (ChoosedItemChanged != null) {
				ChoosedItemChanged(this, e);
			}
		}
		public event EventHandler ChoosedItemChanged;
		
		public AxSideTabItem ChoosedItem {
			get {
				return choosedItem;
			}
			set {
				if (choosedItem != null) {
					choosedItem.SideTabItemStatus = SideTabItemStatus.Normal;
				}
				choosedItem = value;
				if (choosedItem != null) {
					choosedItem.SideTabItemStatus = SideTabItemStatus.Choosed;
				}
				OnChoosedItemChanged(null);
			}
		}
		
		public ISideTabItemFactory SideTabItemFactory {
			get {
				return items.SideTabItemFactory;
			}
			set {
				items.SideTabItemFactory = value;
			}
		}
		
		protected AxSideTab()
		{
		}
		
		public AxSideTab(ISideTabItemFactory sideTabItemFactory)
		{
			SideTabItemFactory = sideTabItemFactory;
		}
		
		public AxSideTab(AxSideBar sideBar, string name) : this(sideBar.SideTabItemFactory)
		{
			this.name = name;
		}
		
		public AxSideTab(string name)
		{
			this.name = name;
		}
		
		
		public bool ScrollDownButtonActivated {
			get {
				return scrollIndex > 0;
			}
		}
		
		public bool ScrollUpButtonActivated {
			get {
				return true;
			}
		}
		
		public void DrawTabHeader(Graphics g, Font font, Point pos, int width)
		{
			switch (sideTabStatus) {
				case SideTabStatus.Normal:
					ControlPaint.DrawBorder3D(g, new Rectangle(0, pos.Y, width - 4, font.Height + 4), Border3DStyle.RaisedInner);
					g.DrawString(StringParser.Parse(name), font, SystemBrushes.ControlText, new RectangleF(1, pos.Y + 1, width - 5, font.Height + 1));
					
					break;
				case SideTabStatus.Selected:
					ControlPaint.DrawBorder3D(g, new Rectangle(0, pos.Y, width - 4, font.Height + 4), Border3DStyle.Sunken);
					g.DrawString(StringParser.Parse(name), font, SystemBrushes.ControlText, new RectangleF(1 + 1, pos.Y + 2, width - 5, font.Height + 2));
					break;
				case SideTabStatus.Dragged:
					Rectangle r = new Rectangle(0, pos.Y, width - 4, font.Height + 4);
					ControlPaint.DrawBorder3D(g, r, Border3DStyle.RaisedInner);
					r.X += 2;
					r.Y += 1;
					r.Width  -= 4;
					r.Height -= 2;
					
					g.FillRectangle(SystemBrushes.ControlDarkDark, r);
					
					g.DrawString(StringParser.Parse(name), font, SystemBrushes.HighlightText, new RectangleF(1 + 1, pos.Y + 2, width - 5, font.Height + 2));
					break;
			}
		}
		
		public int Height {
			get {
				return Items.Count * 20;
			}
		}
		
		public Point GetLocation(AxSideTabItem whichItem)
		{
			for (int i = 0; i < Items.Count; ++i) {
				AxSideTabItem item = (AxSideTabItem)Items[i];
				if (item == whichItem) {
					return new Point(0, i * 20);
				}
			}
			return new Point(-1, -1);
		}
		
		public AxSideTabItem GetItemAt(int x, int y)
		{
			int index = ScrollIndex + y / 20;
			return (index >= 0 && index < Items.Count) ? (AxSideTabItem)Items[index] : null;
		}
		
		public AxSideTabItem GetItemAt(Point pos)
		{
			return GetItemAt(pos.X, pos.Y);
		}
		public int ItemHeight {
			get {
				return 20;
			}
		}
		public void DrawTabContent(Graphics g, Font f, Rectangle rectangle)
		{
			for (int i = 0; i + ScrollIndex < Items.Count; ++i) {
				AxSideTabItem item = (AxSideTabItem)Items[ScrollIndex + i];
				if (rectangle.Height < i * ItemHeight) {
					break;
				}
				item.DrawItem(g, f, new Rectangle(rectangle.X,
				                                  rectangle.Y + i * ItemHeight,
				                                  rectangle.Width,
				                                  ItemHeight));
			}
		}
		
		public class SideTabItemCollection : ICollection, IEnumerable
		{
			ArrayList list = new ArrayList();
			ISideTabItemFactory sideTabItemFactory = new DefaultSideTabItemFactory();
			
			public ISideTabItemFactory SideTabItemFactory {
				get {
					return sideTabItemFactory;
				}
				set {
					sideTabItemFactory = value;
				}
			}
			
			public SideTabItemCollection()
			{
			}
			
			public AxSideTabItem this[int index] {
				get {
					return (AxSideTabItem)list[index];
				}
				set {
					list[index] = value;
				}
			}
			
			public int DraggedIndex {
				get {
					for (int i = 0; i < Count; ++i) {
						if (this[i].SideTabItemStatus == SideTabItemStatus.Drag)
							return i;
					}
					return -1;
				}
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
			
			public virtual AxSideTabItem Add(AxSideTabItem item)
			{
				list.Add(item);
				return item;
			}
			
			public virtual AxSideTabItem Add(string name, object content)
			{
				return Add(name, content, -1);
			}
			
			public virtual AxSideTabItem Add(string name, object content, int imageIndex)
			{
				AxSideTabItem item = sideTabItemFactory.CreateSideTabItem(name, imageIndex);
				item.Tag = content;
				return Add(item);
			}
			
			public virtual void Clear()
			{
				list.Clear();
			}
			
			public bool Contains(AxSideTabItem item)
			{
				return list.Contains(item);
			}
			
			public IEnumerator GetEnumerator()
			{
				return list.GetEnumerator();
			}
			
			public int IndexOf(AxSideTabItem item)
			{
				return list.IndexOf(item);
			}
			
			public void CopyTo(Array dest, int index)
			{
				list.CopyTo(dest, index);
			}
			
			public virtual AxSideTabItem Insert(int index, AxSideTabItem item)
			{
				list.Insert(index, item);
				return item;
			}
			
			public virtual AxSideTabItem Insert(int index, string name, object content)
			{
				return Insert(index, name, content, -1);
			}
			
			public virtual AxSideTabItem Insert(int index, string name, object content, int imageIndex)
			{
				AxSideTabItem item = sideTabItemFactory.CreateSideTabItem(name, imageIndex);
				item.Tag = content;
				return Insert(index, item);
			}
			
			public virtual void Remove(AxSideTabItem item)
			{
				list.Remove(item);
			}
			
			public virtual void RemoveAt(int index)
			{
				list.RemoveAt(index);
			}
		}
	}
}
