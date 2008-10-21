using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ICSharpCode.Xaml
{
	public class NodeCollection<T> : Collection<T> where T : XamlNode
	{
		public NodeCollection(XamlNode parent)
		{
			this.parent = parent;
		}

		XamlNode parent;

		protected override void InsertItem(int index, T item)
		{
			base.InsertItem(index, item);
			item.ParentNode = parent;
			if (item.Document != null) {
				var e = new DocumentChangedEventArgs() { NewNode = item };
				item.Document.RaiseDocumentChanged(e);
			}
		}

		protected override void RemoveItem(int index)
		{
			var item = this[index];
			base.RemoveItem(index);
			if (item.Document != null) {
				var e = new DocumentChangedEventArgs() { OldNode = item, OldParent = item.ParentNode };
				item.ParentNode = null;
				item.Document.RaiseDocumentChanged(e);
			}
			else {
				item.ParentNode = null;
			}
		}

		protected override void SetItem(int index, T item)
		{
			var oldItem = this[index];
			base.SetItem(index, item);
			if (item.Document != null) {
				var e = new DocumentChangedEventArgs() {
					OldNode = oldItem,
					OldParent = oldItem.ParentNode,
					NewNode = item
				};
				oldItem.ParentNode = null;
				item.ParentNode = parent;
				item.Document.RaiseDocumentChanged(e);
			}
			else {
				oldItem.ParentNode = null;
			}
		}

		protected override void ClearItems()
		{
			while (Count > 0) {
				RemoveItem(Count - 1);
			}
		}
	}
}
