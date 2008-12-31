using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SharpDevelop.XamlDesigner.Palette
{
	public class PaletteItemCollection : ObservableCollection<PaletteItem>
	{
		public PaletteItemCollection(PaletteAssembly parent)
		{
			this.parent = parent;
		}

		PaletteAssembly parent;

		protected override void InsertItem(int index, PaletteItem item)
		{
			item.SetParent(parent);
			base.InsertItem(index, item);			
		}
	}
}
