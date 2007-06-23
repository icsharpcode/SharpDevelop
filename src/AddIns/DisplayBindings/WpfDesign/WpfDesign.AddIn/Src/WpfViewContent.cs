// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using ICSharpCode.WpfDesign.Designer;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// Description of WpfViewContent.
	/// </summary>
	public class WpfViewContent : AbstractViewContentHandlingLoadErrors
	{
		ElementHost wpfHost;
		DesignSurface designer;
		
		public WpfViewContent(OpenedFile file) : base(file)
		{
		}
		
		protected override void LoadInternal(OpenedFile file, System.IO.Stream stream)
		{
			if (designer == null) {
				// initialize designer on first load
				wpfHost = new ElementHost();
				designer = new DesignSurface();
				wpfHost.Child = designer;
				this.UserControl = wpfHost;
			}
			using (XmlTextReader r = new XmlTextReader(stream)) {
				designer.LoadDesigner(r);
			}
		}
		
		protected override void SaveInternal(OpenedFile file, System.IO.Stream stream)
		{
			using (XmlTextWriter xmlWriter = new XmlTextWriter(stream, Encoding.UTF8)) {
				xmlWriter.Formatting = Formatting.Indented;
				designer.SaveDesigner(xmlWriter);
			}
		}
	}
}
