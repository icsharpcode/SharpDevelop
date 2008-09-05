// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using System.Xml;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class WpfPrimaryDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return Path.GetExtension(fileName).Equals(".xaml", StringComparison.OrdinalIgnoreCase);
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new WpfViewContent(file);
		}
	}
	
	public class WpfSecondaryDisplayBinding : ISecondaryDisplayBinding
	{
		public bool ReattachWhenParserServiceIsReady {
			get {
				return false;
			}
		}
		
		public bool CanAttachTo(IViewContent content)
		{
			if (Path.GetExtension(content.PrimaryFileName).Equals(".xaml", StringComparison.OrdinalIgnoreCase)) {
				IEditable editable = content as IEditable;
				if (editable != null) {
					try {
						XmlTextReader r = new XmlTextReader(new StringReader(editable.Text));
						r.XmlResolver = null;
						r.WhitespaceHandling = WhitespaceHandling.None;
						while (r.NodeType != XmlNodeType.Element && r.Read());
						
						if (r.LocalName == "Window" || r.LocalName == "UserControl")
							return true;
					} catch (XmlException) {
						return false;
					}
				}
			}
			return false;
		}
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			return new IViewContent[] { new WpfViewContent(viewContent.PrimaryFile) };
		}
	}
}
