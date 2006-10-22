// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Adds a new attribute to the XML Tree's attribute property grid.
	/// </summary>
	public class AddAttributeCommand : AbstractMenuCommand
	{		
		public override void Run()
		{
			XmlTreeViewContainerControl view = Owner as XmlTreeViewContainerControl;
			if (view != null) {
				view.AddAttribute();
			}
		}
	}
}
