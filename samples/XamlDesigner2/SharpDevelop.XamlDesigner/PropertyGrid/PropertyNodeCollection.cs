using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpDevelop.XamlDesigner.PropertyGrid
{
	/// <summary>
	/// A SortedObservableCollection{PropertyNode, string} that sorts by the PropertyNode's Name.
	/// </summary>
	public class PropertyNodeCollection : SortedObservableCollection<PropertyNode, string>
	{
		/// <summary>
		/// Creates a new PropertyNodeCollection instance.
		/// </summary>
		public PropertyNodeCollection()
			: base(n => n.MemberId.Name)
		{
		}
	}
}
