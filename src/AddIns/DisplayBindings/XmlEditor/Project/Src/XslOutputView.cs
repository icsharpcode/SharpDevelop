// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Displays the resulting output from an XSL transform.
	/// </summary>
	public class XslOutputView : XmlView
	{
		public XslOutputView()
		{
			TitleName = StringParser.Parse("${res:ICSharpCode.XmlEditor.XslOutputView.Title}");
			TextEditorControl.FileName = String.Empty;
		}
		
		public static XslOutputView Instance {
			get {
				foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
					if (content is XslOutputView) {
						LoggingService.Debug("XslOutputView instance exists.");
						LoggingService.Debug("XslOutputView.IsDisposed=" + content.Control.IsDisposed.ToString());
						return (XslOutputView)content;
					}
				}
				return null;
			}
		}
	}
}
