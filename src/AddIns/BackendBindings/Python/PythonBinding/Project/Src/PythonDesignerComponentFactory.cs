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
			if (component is ListView) {
				return new PythonListViewComponent(component);
			}
			return new PythonDesignerComponent(component);
		}
		
		public static PythonDesignerRootComponent CreateDesignerRootComponent(IComponent component)
		{
			return new PythonDesignerRootComponent(component);
		}
	}
}
