// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	public class XPathQueryPad : AbstractPadContent
	{
		public const string XPathQueryControlProperties = "XPathQueryControl.Options";
		
		XPathQueryControl xpathQueryControl;
		bool disposed;
		static XPathQueryPad instance;
		
		public XPathQueryPad()
		{
			xpathQueryControl = new XPathQueryControl();
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += ActiveViewContentChanged;
			Properties properties = PropertyService.Get(XPathQueryControlProperties, new Properties());
			xpathQueryControl.SetMemento(properties);
			instance = this;
		}
		
		public static XPathQueryPad Instance {
			get { return instance; }
		}
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad.
		/// </summary>
		public override object Control {
			get { return xpathQueryControl; }
		}
		
		public override void Dispose()
		{
			if (!disposed) {
				disposed = true;
				WorkbenchSingleton.Workbench.ActiveViewContentChanged -= ActiveViewContentChanged;
				Properties properties = xpathQueryControl.CreateMemento();
				PropertyService.Set(XPathQueryControlProperties, properties);
				xpathQueryControl.Dispose();
			}
		}
		
		void ActiveViewContentChanged(object source, EventArgs e)
		{
			xpathQueryControl.ActiveWindowChanged();
		}
	}
}
