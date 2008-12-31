using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpDevelop.XamlDesigner.Dom.UndoSystem
{
	class InsertAction : UndoAction
	{
		public InsertAction(DesignItemCollection collection, DesignItem item, int index)
		{
			this.collection = collection;
			this.item = item;
			this.index = index;
		}

		DesignItemCollection collection;
		DesignItem item;
		int index;

		public override void Undo()
		{
			collection.RemoveAt(index);
		}

		public override void Redo()
		{
			collection.Insert(index, item);
		}

		public override IEnumerable<DesignItem> GetAffectedItems()
		{
			yield return item;
		}
	}
}
