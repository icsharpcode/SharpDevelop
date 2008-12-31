using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpDevelop.XamlDesigner.Dom.UndoSystem
{
	class PropertyAction : UndoAction
	{
		public PropertyAction(DesignProperty property, DesignItem newValue)
		{
			this.property = property;
			this.oldValue = property.Value;
			this.newValue = newValue;
		}

		DesignProperty property;
		DesignItem oldValue;		
		DesignItem newValue;

		public override void Undo()
		{
			property.SetValueCore(oldValue);
		}

		public override void Redo()
		{
			property.SetValueCore(newValue);
		}

		public override IEnumerable<DesignItem> GetAffectedItems()
		{
			yield return property.ParentItem;
		}

		public bool TryMergeWith(PropertyAction other)
		{
			if (property == other.property) {
				newValue = other.newValue;
				return true;
			}
			return false;
		}
	}
}
