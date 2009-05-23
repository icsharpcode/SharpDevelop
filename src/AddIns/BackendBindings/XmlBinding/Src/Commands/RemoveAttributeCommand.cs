// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Removes the selected attribute from the xml document being 
	/// displayed in the XML tree.
	/// </summary>
	public class RemoveAttributeCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			XmlTreeViewContainerControl view = Owner as XmlTreeViewContainerControl;
			if (view != null) {
				view.RemoveAttribute();
			}
		}
	}
}
