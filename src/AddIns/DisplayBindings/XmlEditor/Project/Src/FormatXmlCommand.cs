// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System.IO;
using System.Xml;

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
			XmlView xmlView = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as XmlView;
			if (xmlView != null) {
				xmlView.FormatXml();
			}
		}
	}
}

