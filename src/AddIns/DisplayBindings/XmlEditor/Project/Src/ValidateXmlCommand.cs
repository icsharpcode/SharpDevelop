// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using System;
using ICSharpCode.Core;

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
			XmlView properties = XmlView.ForViewContent(WorkbenchSingleton.Workbench.ActiveViewContent);
			if (properties != null) {
				// Validate the xml.
				properties.ValidateXml();
			}
		}
	}
}
