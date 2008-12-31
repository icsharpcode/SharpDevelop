using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpDevelop.XamlDesigner.Dom.UndoSystem
{
	class UndoTransaction : UndoAction
	{
		public List<UndoAction> Actions = new List<UndoAction>();

		public override IEnumerable<DesignItem> GetAffectedItems()
		{
			foreach (var action in Actions) {
				foreach (var item in action.GetAffectedItems()) {
					yield return item;
				}
			}
		}

		public override void Undo()
		{
			foreach (var action in Enumerable.Reverse(Actions)) {
				action.Undo();
			}
		}

		public override void Redo()
		{
			foreach (var action in Actions) {
				action.Redo();
			}
		}
	}
}
