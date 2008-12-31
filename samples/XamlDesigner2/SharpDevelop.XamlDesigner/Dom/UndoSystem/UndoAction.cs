using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpDevelop.XamlDesigner.Dom.UndoSystem
{
	public abstract class UndoAction
	{
		public abstract void Undo();
		public abstract void Redo();

		public virtual IEnumerable<DesignItem> GetAffectedItems()
		{
			yield break;
		}
	}
}
