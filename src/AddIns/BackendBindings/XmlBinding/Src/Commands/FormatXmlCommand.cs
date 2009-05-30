// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: -1 $</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using System;
using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Pretty prints the xml.
	/// </summary>
	public class FormatXmlCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			// Find active XmlView.
			XmlView properties = XmlView.ForViewContent(WorkbenchSingleton.Workbench.ActiveViewContent);
			
			if (properties != null) {
				properties.FormatXml();
			}
		}
	}
}
