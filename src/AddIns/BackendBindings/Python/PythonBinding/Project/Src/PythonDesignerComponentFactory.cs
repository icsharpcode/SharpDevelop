// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ICSharpCode.PythonBinding
{
	public class PythonDesignerComponentFactory
	{
		PythonDesignerComponentFactory()
		{
		}
		
		/// <summary>
		/// Creates a PythonDesignerComponent class for the specified component.
		/// </summary>
		public static PythonDesignerComponent CreateDesignerComponent(IComponent component)
		{
			return CreateDesignerComponent(null, component);
		}
		
		/// <summary>
		/// Creates a PythonDesignerComponent class for the specified component.
		/// </summary>
		public static PythonDesignerComponent CreateDesignerComponent(PythonDesignerComponent parent, IComponent component)
		{
			if (component is ListView) {
				return new PythonListViewComponent(parent, component);
			} else if (component is ContextMenuStrip) {
				return new PythonContextMenuComponent(parent, component);
			}
			return new PythonDesignerComponent(parent, component);
		}
		
		public static PythonDesignerRootComponent CreateDesignerRootComponent(IComponent component)
		{
			return CreateDesignerRootComponent(component, String.Empty);
		}
		
		public static PythonDesignerRootComponent CreateDesignerRootComponent(IComponent component, string rootNamespace)
		{
			return new PythonDesignerRootComponent(component, rootNamespace);
		}
	}
}
