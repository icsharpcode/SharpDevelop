using System;
using System.Windows.Forms;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of ClassErbauer.
	/// </summary>
	public class ToolbarItemErbauer : IErbauer
	{
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			string type = codon.Properties.Contains("type") ? codon.Properties["type"] : "Item";
			
			switch (type) {
				case "Separator":
					return new ToolBarSeparator(codon, caller);
				case "CheckBox":
					return new ToolBarCheckBox(codon, caller);
				case "Item":
					return new ToolBarCommand(codon, caller);
				case "ComboBox":
					return new ToolBarComboBox(codon, caller);
				case "DropDownButton":
					return new ToolBarDropDownButton(codon, caller);
				default:
					throw new System.NotSupportedException("unsupported menu item type : " + type);
			}
		}
	}
}
