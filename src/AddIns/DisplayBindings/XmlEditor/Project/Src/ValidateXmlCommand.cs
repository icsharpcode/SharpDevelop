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
			XmlView xmlView = XmlView.ActiveXmlView;
			if (xmlView != null) {
				// Validate the xml.
				xmlView.ValidateXml();
			}
		}
	}
}
