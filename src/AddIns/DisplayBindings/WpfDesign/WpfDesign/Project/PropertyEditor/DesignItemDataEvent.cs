// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2667$</version>
// </file>

using System;
using System.Diagnostics;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// wraps a DesignItemDataProperty (with IsEvent=true) for the property editor/grid.
	/// </summary>
	sealed class DesignItemDataEvent : DesignItemDataMember, IPropertyEditorDataEvent
	{
		internal DesignItemDataEvent(DesignItemDataSource ownerDataSource, DesignItemProperty property)
			: base(ownerDataSource, property)
		{
			Debug.Assert(property.IsEvent);
		}
		
		public event EventHandler HandlerNameChanged {
			add { property.ValueChanged += value; }
			remove { property.ValueChanged -= value; }
		}
		
		public string HandlerName {
			get {
				return (string)property.ValueOnInstance;
			}
			set {
				if (string.IsNullOrEmpty(value))
					property.Reset();
				else
					property.SetValue(value);
			}
		}
		
		public void GoToHandler()
		{
			IEventHandlerService ehs = ownerDataSource.Services.GetService<IEventHandlerService>();
			if (ehs != null) {
				ehs.CreateEventHandler(ownerDataSource.DesignItem, property);
			}
		}
	}
}
