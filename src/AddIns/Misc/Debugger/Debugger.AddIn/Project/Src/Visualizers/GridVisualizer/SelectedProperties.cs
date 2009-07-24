// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	/// Remembers which properties are selected to display in the Grid visualizer.
	/// </summary>
	public class SelectedProperties : IEnumerable<SelectedProperty>
	{
		private List<SelectedProperty> properties = new List<SelectedProperty>();

		public SelectedProperties(IEnumerable<string> propertyNames)
		{
			foreach (var propName in propertyNames)
			{
				properties.Add(new SelectedProperty { Name = propName, IsSelected = true });
			}
		}

		#region IEnumerable<PropertySelected> Members

		public IEnumerator<SelectedProperty> GetEnumerator()
		{
			return ((IEnumerable<SelectedProperty>)properties).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((System.Collections.IEnumerable)properties).GetEnumerator();
		}

		#endregion
	}
	
	/// <summary>
	/// Property name + IsSelected.
	/// </summary>
	public class SelectedProperty
	{
		public event EventHandler SelectedChanged;

		public string Name { get; set; }

		private bool isSelected = true;
		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				isSelected = value;
				if (SelectedChanged != null)
					SelectedChanged(this, EventArgs.Empty);
			}
		}
	}
}
