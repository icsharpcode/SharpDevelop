// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// The class that is responsible for controlling the editing of the 
	/// Xml tree view.
	/// </summary>
	public class XmlTreeEditor
	{
		IXmlTreeView view;
		XmlDocument document;
		
		public XmlTreeEditor(IXmlTreeView view)
		{
			this.view = view;
		}
		
		/// <summary>
		/// Loads the xml into the editor.
		/// </summary>
		public void LoadXml(string xml)
		{
			try {
				document = new XmlDocument();
				document.LoadXml(xml);
				XmlElement documentElement = document.DocumentElement;
				view.DocumentElement = documentElement;
			} catch (XmlException ex) {
				view.ShowXmlIsNotWellFormedMessage(ex);
			}
		}
		
		/// <summary>
		/// Gets the Xml document being edited.
		/// </summary>
		public XmlDocument Document {
			get {
				return document;
			}
		}
		
		/// <summary>
		/// The selected xml element in the view has changed.
		/// </summary>
		public void SelectedElementChanged()
		{
			XmlElement selectedElement = view.SelectedElement;
			if (selectedElement != null) {
				view.ShowAttributes(selectedElement.Attributes);
			} else {
				view.ClearAttributes();
			}
		}
		
		/// <summary>
		/// The selected xml text node in the view has changed.
		/// </summary>
		public void SelectedTextNodeChanged()
		{
			XmlText selectedTextNode = view.SelectedTextNode;
			if (selectedTextNode != null) {
				view.ShowTextContent(selectedTextNode.InnerText);
			} else {
				view.ShowTextContent(String.Empty);
			}
		}
		
		/// <summary>
		/// The attribute value has changed.
		/// </summary>
		public void AttributeValueChanged()
		{
			view.IsDirty = true;
		}
		
		/// <summary>
		/// The text content has been changed in the view.
		/// </summary>
		public void TextContentChanged()
		{
			XmlText textNode = view.SelectedTextNode;
			if (textNode != null) {
				view.IsDirty = true;
				textNode.Value = view.TextContent;
			}
		}
	}
}
