using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	class PlaceHolderItem: VariableListItem
	{
		public override bool IsValid {
			get {
				return false;
			}
		}
	}
}
