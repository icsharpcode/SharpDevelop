// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		/// Displays an error message.
		/// </summary>
		void ShowErrorMessage(string message);
		
		/// <summary>
		/// Gets or sets whether this view needs saving.
		/// </summary>
		bool IsDirty {get; set;}
		
		/// <summary>
		/// Gets or sets the xml document.
		/// </summary>
		XmlDocument Document {get; set;}
		
		/// <summary>
		/// Gets the selected node in the tree.
		/// </summary>
		XmlNode SelectedNode {get;}
		
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
		/// Gets or sets the text content currently on display. The
		/// text content will not be displayed unless 
		/// ShowTextContent has been called.
		/// </summary>
		string TextContent {get; set;}
		
		/// <summary>
		/// Gets the xml element text node.
		/// </summary>
		XmlText SelectedTextNode {get;}

		/// <summary>
		/// Shows the add attribute dialog and allows the user
		/// to select a new attribute to be added to the selected
		/// xml element.
		/// </summary>
		/// <param name="attributes">The list of attributes to
		/// be displayed to the user.</param>
		/// <returns>The attributes selected; otherwise an empty 
		/// collection.</returns>
		string[] SelectNewAttributes(string[] attributes);
		
		/// <summary>
		/// Gets the name of the selected attribute.
		/// </summary>
		string SelectedAttribute {get;}
		
		/// <summary>
		/// Shows the add element dialog and allows the user
		/// to select a new element to be added to the selected
		/// xml element, either added as a child, inserted before
		/// or after.
		/// </summary>
		/// <param name="attributes">The list of elements to
		/// be displayed to the user.</param>
		/// <returns>The attributes elements; otherwise an empty 
		/// collection.</returns>
		string[] SelectNewElements(string[] elements);
		
		/// <summary>
		/// Appends the child element to the currently selected element.
		/// </summary>
		void AppendChildElement(XmlElement element);
		
		/// <summary>
		/// Inserts the specified element before the currently selected
		/// element.
		/// </summary>
		void InsertElementBefore(XmlElement element);
		
		/// <summary>
		/// Inserts the specified element after the currently selected
		/// element.
		/// </summary>
		void InsertElementAfter(XmlElement element);
		
		/// <summary>
		/// Removes the specified element from the tree.
		/// </summary>
		void RemoveElement(XmlElement element);
		
		/// <summary>
		/// Informs the view that the specified node has been selected
		/// to be cut from the tree. The view can then update its display
		/// to inform the user that the node has been cut.
		/// </summary>
		void ShowCut(XmlNode node);
		
		/// <summary>
		/// Informs the view that the visual indication of the cut should
		/// be cleared.
		/// </summary>
		void HideCut(XmlNode node);
		
		/// <summary>
		/// Appends a new child text node to the currently selected
		/// element.
		/// </summary>
		void AppendChildTextNode(XmlText textNode);
		
		/// <summary>
		/// Inserts a new child text node before the currently
		/// selected node.
		/// </summary>
		void InsertTextNodeBefore(XmlText textNode);
		
		/// <summary>
		/// Inserts a new child text node after the currently
		/// selected node.
		/// </summary>
		void InsertTextNodeAfter(XmlText textNode);
		
		/// <summary>
		/// Removes the currently selected text node.
		/// </summary>
		void RemoveTextNode(XmlText textNode);
		
		/// <summary>
		/// Informs the xml tree view that the text node
		/// has changed and the corresponding tree node
		/// needs to be updated.
		/// </summary>
		void UpdateTextNode(XmlText textNode);
		
		/// <summary>
		/// Gets the currently selected comment node.
		/// </summary>
		XmlComment SelectedComment {get;}
		
		/// <summary>
		/// Informs the xml tree view that the comment node
		/// has changed and the corresponding tree node
		/// needs to be updated.
		/// </summary>
		void UpdateComment(XmlComment comment);
		
		/// <summary>
		/// Appends a new child comment to the currently selected
		/// element.
		/// </summary>
		void AppendChildComment(XmlComment comment);
		
		/// <summary>
		/// Removes the specified comment node from the view.
		/// </summary>
		void RemoveComment(XmlComment comment);
		
		/// <summary>
		/// Inserts the comment before the currently selected node.
		/// </summary>
		void InsertCommentBefore(XmlComment comment);
		
		/// <summary>
		/// Inserts the comment after the currently selected node.
		/// </summary>
		void InsertCommentAfter(XmlComment comment);
	}
}
