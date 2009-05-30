// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using System;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Validates the xml in the xml editor against the known schemas.
	/// </summary>
	public class ValidateXmlCommand : AbstractMenuCommand
	{
		/// <summary>
		/// Validate the xml.
		/// </summary>
		public override void Run()
		{
			// Find active XmlView.
			XmlView properties = XmlView.ForView(WorkbenchSingleton.Workbench.ActiveViewContent);
			if (properties != null) {
				// Validate the xml.
				properties.ValidateXml();
			}
		}
	}
}
