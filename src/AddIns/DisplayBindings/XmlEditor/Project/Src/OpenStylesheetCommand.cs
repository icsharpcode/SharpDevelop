// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Opens the stylesheet associated with the active XML document.
	/// </summary>
	public class OpenStylesheetCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			XmlView xmlView = XmlView.ActiveXmlView;
			if (xmlView != null && xmlView.StylesheetFileName != null) {
				try {
					FileService.OpenFile(xmlView.StylesheetFileName);
				} catch (Exception ex) {
					MessageService.ShowError(ex);
				}
			}
		}
	}
}
