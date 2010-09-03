// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Event arguments specifying a component as parameter.
	/// </summary>
	public class DesignItemEventArgs : EventArgs
	{
		readonly DesignItem _item;

		/// <summary>
		/// Creates a new ComponentEventArgs instance.
		/// </summary>
		public DesignItemEventArgs(DesignItem item)
		{
			_item = item;
		}
		
		/// <summary>
		/// The component affected by the event.
		/// </summary>
		public DesignItem Item {
			get { return _item; }
		}
	}
	
	/// <summary>
	/// Event arguments specifying a component as parameter.
	/// </summary>
	public class DesignItemCollectionEventArgs : EventArgs
	{
		readonly ICollection<DesignItem> _items;

		/// <summary>
		/// Creates a new ComponentCollectionEventArgs instance.
		/// </summary>
		public DesignItemCollectionEventArgs(ICollection<DesignItem> items)
		{
			_items = items;
		}
		
		/// <summary>
		/// The components affected by the event.
		/// </summary>
		public ICollection<DesignItem> Items {
			get { return _items; }
		}
	}
}
