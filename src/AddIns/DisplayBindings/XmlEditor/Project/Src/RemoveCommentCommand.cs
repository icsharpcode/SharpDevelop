// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Removes the currently selected comment node from the XML tree.
	/// </summary>
	public class RemoveCommentCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			XmlTreeViewContainerControl view = Owner as XmlTreeViewContainerControl;
			if (view != null) {
				view.RemoveComment();
			}
		}
	}
}
