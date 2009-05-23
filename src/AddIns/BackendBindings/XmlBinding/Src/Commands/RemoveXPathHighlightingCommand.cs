// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1966 $</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	public class RemoveXPathHighlightingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			XPathQueryPad pad = XPathQueryPad.Instance;
			if (pad != null) {
				pad.RemoveXPathHighlighting();
			}
		}
	}
}
