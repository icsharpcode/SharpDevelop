using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SharpDevelop.XamlDesigner.Palette
{
	public class PaletteAssemblyCollection : ObservableCollection<PaletteAssembly>
	{
		public PaletteAssemblyCollection(PaletteData parent)
		{
			this.parent = parent;
		}

		PaletteData parent;

		protected override void InsertItem(int index, PaletteAssembly item)
		{
			item.SetParent(parent);
			base.InsertItem(index, item);			
		}
	}
}
