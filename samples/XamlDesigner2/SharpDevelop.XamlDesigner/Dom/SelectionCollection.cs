using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;

namespace SharpDevelop.XamlDesigner.Dom
{
	public class SelectionCollection : ICollection<DesignItem>, INotifyCollectionChanged
	{
		HashSet<DesignItem> selection = new HashSet<DesignItem>();
		DesignItem[] oldSelection;

		public event DesignSelectionChangedHandler Changed;
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public void Add(DesignItem item)
		{
			BeginChange();
			AddCore(item);
			EndChange();
		}

		public bool Remove(DesignItem item)
		{
			BeginChange();
			var result = RemoveCore(item);
			EndChange();
			return result;
		}

		public void Clear()
		{
			BeginChangeAndClear();
			EndChange();
		}

		public void Auto(DesignItem item)
		{
			Auto(item.AsArrayOrDefault());
		}

		public void Auto(IEnumerable<DesignItem> items)
		{
			if (items == null) {
				Clear();
			}
			else {
				if (Keyboard.IsKeyDown(Key.LeftCtrl)) {
					BeginChange();
					foreach (var item in items) {
						ToggleCore(item);
					}
				}
				else {
					BeginChangeAndClear();
					foreach (var item in items) {
						AddCore(item);
					}
				}
				EndChange();
			}
		}

		public void Set(DesignItem item)
		{
			Set(item.AsArrayOrDefault());
		}

		public void Set(IEnumerable<DesignItem> items)
		{
			BeginChangeAndClear();
			if (items != null) {
				foreach (var item in items) {
					AddCore(item);
				}
			}
			EndChange();
		}

		internal void AddWhenPropertyChanged(DesignItem item)
		{
			if (oldSelection == null) {
				Add(item);
			}
		}

		internal void RemoveWhenPropertyChanged(DesignItem item)
		{
			if (oldSelection == null) {
				Remove(item);
			}
		}

		public bool Contains(DesignItem item)
		{
			return selection.Contains(item);
		}

		public void CopyTo(DesignItem[] array, int arrayIndex)
		{
			selection.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return selection.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		IEnumerator<DesignItem> IEnumerable<DesignItem>.GetEnumerator()
		{
			return selection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return selection.GetEnumerator();
		}

		void AddCore(DesignItem item)
		{
			if (item != null) {
				selection.Add(item);
				item.IsSelected = true;
			}
		}

		bool RemoveCore(DesignItem item)
		{
			if (item != null) {
				item.IsSelected = false;
				return selection.Remove(item);
			}
			return false;
		}

		void ToggleCore(DesignItem item)
		{
			if (item != null) {
				if (item.IsSelected) {
					RemoveCore(item);
				}
				else {
					AddCore(item);
				}
			}
		}

		void BeginChange()
		{
			oldSelection = selection.ToArray();
		}

		void BeginChangeAndClear()
		{
			BeginChange();
			foreach (var item in oldSelection) {
				RemoveCore(item);
			}
		}

		void EndChange()
		{
			var e = new DesignSelectionChangedEventArgs() {
				OldItems = oldSelection,
				NewItems = selection.ToArray()
			};
			oldSelection = null;
			if (Changed != null && !selection.SequenceEqual(e.OldItems)) {
				Changed(this, e);
			}
			if (CollectionChanged != null) {
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Reset));
			}
		}
	}
}
