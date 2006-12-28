// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Event arguments specifying a component as parameter.
	/// </summary>
	public class ComponentEventArgs : EventArgs
	{
		readonly object _component;

		/// <summary>
		/// Creates a new ComponentEventArgs instance.
		/// </summary>
		public ComponentEventArgs(object component)
		{
			_component = component;
		}
		
		/// <summary>
		/// The component affected by the event.
		/// </summary>
		public object Component {
			get { return _component; }
		}
	}
	
	/// <summary>
	/// Event arguments specifying a component as parameter.
	/// </summary>
	public class ComponentCollectionEventArgs : EventArgs
	{
		readonly ICollection<object> _components;

		/// <summary>
		/// Creates a new ComponentCollectionEventArgs instance.
		/// </summary>
		public ComponentCollectionEventArgs(ICollection<object> components)
		{
			_components = components;
		}
		
		/// <summary>
		/// The components affected by the event.
		/// </summary>
		public ICollection<object> Components {
			get { return _components; }
		}
	}
}
