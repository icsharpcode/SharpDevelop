// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
