// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Displays the resulting output from an XSL transform.
	/// </summary>
	public class XslOutputView : XmlView
	{
		public XslOutputView()
		{
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
		
		public override bool IsDirty {
			get {
				return false;
			}
			set {
			}
		}
		
		public override string TitleName {
			get {
				return "XSLT Output";
			}
			set {
			}
		}
	}
}
