using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpDevelop.XamlDesigner.PropertyGrid
{
	class PropertyMenuModel : ViewModel
	{
		public PropertyMenuModel(PropertyNode node)
		{
			PropertyNode = node;

			AddCommand("Reset", Reset);
			AddCommand("Binding", Binding);
		}

		public PropertyNode PropertyNode { get; private set; }

		//public LocalResources

		public void Reset()
		{
		}

		public void Binding()
		{
			BindingDialog.Show();
		}
	}
}
