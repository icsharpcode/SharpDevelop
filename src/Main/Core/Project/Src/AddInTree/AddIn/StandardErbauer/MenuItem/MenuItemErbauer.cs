using System;
using System.Windows.Forms;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of ClassErbauer.
	/// </summary>
	public class MenuItemErbauer : IErbauer
	{
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			string type = codon.Properties.Contains("type") ? codon.Properties["type"] : "Item";
			
			switch (type) {
				case "Separator":
					return new MenuSeparator(codon, caller);
				case "CheckBox":
					return new MenuCheckBox(codon, caller);
				case "Item":
					return new MenuCommand(codon, caller);
				case "Menu":
					return new Menu(codon, caller, subItems);
				case "Builder":
					return codon.AddIn.CreateObject(codon.Properties["class"]);
				default:
					throw new System.NotSupportedException("unsupported menu item type : " + type);
			}
		}
	}
}
