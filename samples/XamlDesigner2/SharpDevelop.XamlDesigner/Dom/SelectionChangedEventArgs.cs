using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpDevelop.XamlDesigner.Dom
{
	public class DesignSelectionChangedEventArgs
	{
		public DesignItem[] OldItems;
		public DesignItem[] NewItems;
	}

	public delegate void DesignSelectionChangedHandler(object sender, DesignSelectionChangedEventArgs e);
}
