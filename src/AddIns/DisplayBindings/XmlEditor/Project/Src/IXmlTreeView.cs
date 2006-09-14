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
	public interface IXmlTreeView
	{
		/// <summary>
		/// Displays an error message indicating that the xml tree view
		/// could not display the xml since the xml is not well formed.
		/// </summary>
		/// <param name="ex">The exception that occurred when the xml
		/// was loaded.</param>
		void ShowXmlIsNotWellFormedMessage(XmlException ex);
		
		/// <summary>
		/// Gets or sets whether this view needs saving.
		/// </summary>
		bool IsDirty {get; set;}
		
		/// <summary>
		/// Gets or sets the xml document element.
		/// </summary>
		XmlElement DocumentElement {get; set;}
		
		/// <summary>
		/// Gets the xml element selected.
		/// </summary>
		XmlElement SelectedElement {get;}
		
		/// <summary>
		/// Shows the attributes for the selected xml element in the view.
		/// </summary>
		void ShowAttributes(XmlAttributeCollection attributes);
		
		/// <summary>
		/// Removes the attributes from the view.
		/// </summary>
		void ClearAttributes();
		
		/// <summary>
		/// Shows the xml element text content.
		/// </summary>
		void ShowTextContent(string text);
		
		/// <summary>
		/// Gets the text content currently on display.
		/// </summary>
		string TextContent {get;}
		
		/// <summary>
		/// Gets the xml element text node.
		/// </summary>
		XmlText SelectedTextNode {get;}

	}
}
