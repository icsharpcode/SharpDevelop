using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpDevelop.XamlDesigner.Dom.UndoSystem
{
	class RemoveAction : UndoAction
	{
		public RemoveAction(DesignItemCollection collection, DesignItem item)
		{
			this.collection = collection;
			this.item = item;
			this.index = collection.IndexOf(item);
		}

		DesignItemCollection collection;
		DesignItem item;
		int index;

		public override void Undo()
		{
			collection.Insert(index, item);
		}

		public override void Redo()
		{
			collection.RemoveAt(index);
		}

		public override IEnumerable<DesignItem> GetAffectedItems()
		{
			yield return item;
		}
	}
}
