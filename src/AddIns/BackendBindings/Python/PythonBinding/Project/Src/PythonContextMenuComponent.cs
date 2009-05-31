// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;

namespace ICSharpCode.PythonBinding
{
	public class PythonContextMenuComponent : PythonDesignerComponent
	{
		public PythonContextMenuComponent(PythonDesignerComponent parent, IComponent component) 
			: base(parent, component)
		{
		}
		
		/// <summary>
		/// Always ignore the OwnerItem property. This is set if the context menu is open and displayed in
		/// the designer when the user switches to the source tab. This method works around the problem by 
		/// ignoring the OwnerItem property when generating the form designer code.
		/// </summary>
		protected override bool IgnoreProperty(PropertyDescriptor property)
		{
			return property.Name == "OwnerItem";
		}
	}
}
