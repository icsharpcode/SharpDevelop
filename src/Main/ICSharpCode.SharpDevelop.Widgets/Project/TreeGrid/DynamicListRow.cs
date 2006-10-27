// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;

namespace ICSharpCode.SharpDevelop.Widgets.TreeGrid
{
	public class DynamicListRow
	{
		int height = 16;
		
		public int Height {
			get {
				return height;
			}
			set {
				if (value < 2)
					throw new ArgumentOutOfRangeException("value", value, "value must be at least 2");
				if (height != value) {
					height = value;
					OnHeightChanged(EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler HeightChanged;
		
		protected virtual void OnHeightChanged(EventArgs e)
		{
			if (HeightChanged != null) {
				HeightChanged(this, e);
			}
		}
		
		public event EventHandler<DynamicListEventArgs> Shown;
		public event EventHandler<DynamicListEventArgs> Hidden;
		
		protected virtual void OnShown(DynamicListEventArgs e)
		{
			if (Shown != null) {
				Shown(this, e);
			}
		}
		
		protected virtual void OnHidden(DynamicListEventArgs e)
		{
			if (Hidden != null) {
				Hidden(this, e);
			}
		}
		
		internal void NotifyListVisibilityChange(DynamicList list, bool visible)
		{
			if (visible)
				OnShown(new DynamicListEventArgs(list));
			else
				OnHidden(new DynamicListEventArgs(list));
		}
		
		/// <summary>
		/// Fired when any item has changed.
		/// </summary>
		public event EventHandler ItemChanged;
		
		protected virtual void OnItemChanged(EventArgs e)
		{
			if (ItemChanged != null) {
				ItemChanged(this, e);
			}
		}
		
		internal void RaiseItemChanged(DynamicListItem item)
		{
			OnItemChanged(EventArgs.Empty);
		}
		
		DynamicListItem[] items = new DynamicListItem[10];
		
		public DynamicListItem this[int columnIndex] {
			[DebuggerStepThrough]
			get {
				if (columnIndex < 0)
					throw new ArgumentOutOfRangeException("columnIndex", columnIndex, "columnIndex must be >= 0");
				if (columnIndex > DynamicList.MaxColumnCount)
					throw new ArgumentOutOfRangeException("columnIndex", columnIndex, "columnIndex must be <= " + DynamicList.MaxColumnCount);
				if (columnIndex >= items.Length) {
					Array.Resize(ref items, columnIndex * 2 + 1);
				}
				DynamicListItem item = items[columnIndex];
				if (item == null) {
					items[columnIndex] = item = CreateItem();
				}
				return item;
			}
		}
		
		protected virtual DynamicListItem CreateItem()
		{
			return new DynamicListItem(this);
		}
	}
}
