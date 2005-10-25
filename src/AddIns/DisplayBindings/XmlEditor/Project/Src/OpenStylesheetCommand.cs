// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Windows.Forms;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Opens the stylesheet associated with the active XML document.
	/// </summary>
	public class OpenStylesheetCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			XmlView xmlView = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as XmlView;
			if (xmlView != null) {
				if (xmlView.StylesheetFileName != null) {
					try {
						FileService.OpenFile(xmlView.StylesheetFileName);
					} catch (Exception ex) {
						MessageService.ShowError(ex);
					}
				}
			}
		}
	}
}
