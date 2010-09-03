// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

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
		XmlSchemaCompletionCollection schemas;
		XmlNode copiedNode;
		XmlNode cutNode;
		XmlSchemaCompletion defaultSchema;
				
		public XmlTreeEditor(IXmlTreeView view, XmlSchemaCompletionCollection schemas, XmlSchemaCompletion defaultSchema)
		{
			this.view = view;
			this.schemas = schemas;
			this.defaultSchema = defaultSchema;
		}
		
		/// <summary>
		/// Loads the xml into the editor.
		/// </summary>
		public void LoadXml(string xml)
		{
			try {
				document = new XmlDocument();
				document.XmlResolver = null;
				document.LoadXml(xml);
				view.Document = document;
			} catch (XmlException ex) {
				view.ShowXmlIsNotWellFormedMessage(ex);
			} catch (WebException ex) {
				LoggingService.Debug(ex.ToString());
				view.ShowErrorMessage(ex.Message);
			}
		}
		
		/// <summary>
		/// Gets the Xml document being edited.
		/// </summary>
		public XmlDocument Document {
			get { return document; }
		}
		
		/// <summary>
		/// The selected tree node in the view has changed.
		/// </summary>
		public void SelectedNodeChanged()
		{
			XmlElement selectedElement = view.SelectedElement;
			XmlText selectedTextNode = view.SelectedTextNode;
			XmlComment selectedComment = view.SelectedComment;
			if (selectedTextNode != null) {
				view.ClearAttributes();
				view.ShowTextContent(selectedTextNode.InnerText);
			} else if (selectedElement != null) {
				view.TextContent = String.Empty;
				view.ShowAttributes(selectedElement.Attributes);
			} else if (selectedComment != null) {
				view.ClearAttributes();
				view.ShowTextContent(selectedComment.InnerText);
			} else {
				view.ClearAttributes();
				view.TextContent = String.Empty;
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
		/// Adds one or more new attribute to the selected element.
		/// </summary>
		public void AddAttribute()
		{
			XmlElement selectedElement = view.SelectedElement;
			if (selectedElement != null) {
				string[] attributesNames = GetMissingAttributes(selectedElement);
				string[] selectedAttributeNames = view.SelectNewAttributes(attributesNames);
				if (selectedAttributeNames.Length > 0) {
					foreach (string attributeName in selectedAttributeNames) {
						selectedElement.SetAttribute(attributeName, String.Empty);
					}
					view.IsDirty = true;
					view.ShowAttributes(selectedElement.Attributes);
				}
			}
		}
		
		/// <summary>
		/// Removes the selected attribute from the xml document.
		/// </summary>
		public void RemoveAttribute()
		{
			XmlElement selectedElement = view.SelectedElement;
			if (selectedElement != null) {
				string attribute = view.SelectedAttribute;
				if (attribute != null) {
					selectedElement.RemoveAttribute(attribute);
					view.IsDirty = true;
					view.ShowAttributes(selectedElement.Attributes);
				}
			}
		}
		
		/// <summary>
		/// The text content has been changed in the view.
		/// </summary>
		public void TextContentChanged()
		{
			XmlText textNode = view.SelectedTextNode;
			XmlComment comment = view.SelectedComment;
			if (textNode != null) {
				view.IsDirty = true;
				textNode.InnerText = view.TextContent;
				view.UpdateTextNode(textNode);
			} else if (comment != null) {
				view.IsDirty = true;
				comment.InnerText = view.TextContent;
				view.UpdateComment(comment);
			}
		}
		
		/// <summary>
		/// Adds a new child element to the selected element.
		/// </summary>
		public void AppendChildElement()
		{
			XmlElement selectedElement = view.SelectedElement;
			if (selectedElement != null) {
				string[] elementNames = GetChildElements(selectedElement);
				string[] selectedElementNames = view.SelectNewElements(elementNames);
				if (selectedElementNames.Length > 0) {
					view.IsDirty = true;
					foreach (string elementName in selectedElementNames) {
						XmlElement newElement = document.CreateElement(elementName, selectedElement.NamespaceURI);
						AppendChildElement(selectedElement, newElement);
					}
				}
			}
		}
		
		/// <summary>
		/// Inserts an element before the currently selected element.
		/// </summary>
		public void InsertElementBefore()
		{
			XmlElement parentElement = null;
			XmlNode selectedNode = view.SelectedElement;
			if (selectedNode == null) {
				selectedNode = view.SelectedComment;
			}
			if (selectedNode != null) {
				parentElement = selectedNode.ParentNode as XmlElement;
			}
			
			if (parentElement != null) {
				string[] elementNames = GetChildElements(parentElement);
				string[] selectedElementNames = view.SelectNewElements(elementNames);
				if (selectedElementNames.Length > 0) {
					view.IsDirty = true;
					foreach (string elementName in selectedElementNames) {
						XmlElement newElement = document.CreateElement(elementName, parentElement.NamespaceURI);
						parentElement.InsertBefore(newElement, selectedNode);
						view.InsertElementBefore(newElement);
					}
				}
			}
		}
		
		/// <summary>
		/// Inserts an element after the currently selected element.
		/// </summary>
		public void InsertElementAfter()
		{
			XmlElement parentElement = null;
			XmlNode selectedNode = view.SelectedElement;
			if (selectedNode == null) {
				selectedNode = view.SelectedComment;
			}
			if (selectedNode != null) {
				parentElement = selectedNode.ParentNode as XmlElement;
			}
			
			if (parentElement != null) {
				string[] elementNames = GetChildElements(parentElement);
				string[] selectedElementNames = view.SelectNewElements(elementNames);
				if (selectedElementNames.Length > 0) {
					view.IsDirty = true;
					foreach (string elementName in selectedElementNames) {
						XmlElement newElement = document.CreateElement(elementName, parentElement.NamespaceURI);
						parentElement.InsertAfter(newElement, selectedNode);
						view.InsertElementAfter(newElement);
					}
				}
			}
		}
		
		/// <summary>
		/// Adds a child text node to the current selected element.
		/// </summary>
		public void AppendChildTextNode()
		{
			AppendChildTextNode(document.CreateTextNode(string.Empty));
		}
		
		/// <summary>
		/// Inserts a text node before the currently selected node.
		/// </summary>
		public void InsertTextNodeBefore()
		{
			// Get the currently selected text node or element.
			XmlNode selectedNode = GetSelectedCommentOrElementOrTextNode();

			// Insert the text node before the selected node.
			if (selectedNode != null) {
				XmlElement parentElement = selectedNode.ParentNode as XmlElement;
				if (parentElement != null) {
					XmlText textNode = document.CreateTextNode(string.Empty);
					parentElement.InsertBefore(textNode, selectedNode);
					view.IsDirty = true;
					view.InsertTextNodeBefore(textNode);
				}
			}
		}

		/// <summary>
		/// Inserts a text node after the currently selected node.
		/// </summary>
		public void InsertTextNodeAfter()
		{
			// Get the currently selected text node or element.
			XmlNode selectedNode = GetSelectedCommentOrElementOrTextNode();

			// Insert the text node after the selected node.
			if (selectedNode != null) {
				XmlElement parentElement = selectedNode.ParentNode as XmlElement;
				if (parentElement != null) {
					XmlText textNode = document.CreateTextNode(string.Empty);
					parentElement.InsertAfter(textNode, selectedNode);
					view.IsDirty = true;
					view.InsertTextNodeAfter(textNode);
				}
			}
		}
		
		/// <summary>
		/// Adds a child comment node to the current selected element.
		/// </summary>
		public void AppendChildComment()
		{
			XmlComment comment = document.CreateComment(string.Empty);
			AppendChildComment(comment);
		}
		
		/// <summary>
		/// Inserts a comment before the selected node.
		/// </summary>
		public void InsertCommentBefore()
		{
			XmlNode node = GetSelectedCommentOrElementOrTextNode();
			if (node != null) {
				XmlNode parentNode = node.ParentNode;
				XmlComment comment = document.CreateComment(string.Empty);
				parentNode.InsertBefore(comment, node);
				view.IsDirty = true;
				view.InsertCommentBefore(comment);
			}
		}
		
		/// <summary>
		/// Inserts a comment after the selected node.
		/// </summary>
		public void InsertCommentAfter()
		{
			XmlNode node = GetSelectedCommentOrElementOrTextNode();
			if (node != null) {
				XmlNode parentNode = node.ParentNode;
				XmlComment comment = document.CreateComment(string.Empty);
				parentNode.InsertAfter(comment, node);
				view.IsDirty = true;
				view.InsertCommentAfter(comment);
			}
		}
		
		/// <summary>
		/// Deletes the selected tree node from the xml document.
		/// </summary>
		public void Delete()
		{
			XmlNode selectedNode = view.SelectedNode;
			XmlElement selectedElement = selectedNode as XmlElement;
			XmlComment selectedComment = selectedNode as XmlComment;
			XmlText selectedText = selectedNode as XmlText;
			if (selectedElement != null) {
				RemoveElement(selectedElement);
			} else if (selectedComment != null) {
				RemoveComment(selectedComment);
			} else if (selectedText != null) {
				RemoveTextNode(selectedText);
			}
		}
		
		/// <summary>
		/// Copies the selected node.
		/// </summary>
		public void Copy()
		{
			copiedNode = view.SelectedNode;
			if (cutNode != null) {
				view.HideCut(cutNode);
			}
		}
		
		/// <summary>
		/// Pastes the copied or cut node as a child of the selected node.
		/// </summary>
		public void Paste()
		{
			if (IsPasteEnabled) {
				if (copiedNode != null) {
					AppendChildCopy(copiedNode);
				} else {
					CutAndPasteNode(cutNode);
				}
			}
		}
		
		/// <summary>
		/// Cuts the selected node.
		/// </summary>
		public void Cut()
		{
			cutNode = view.SelectedNode;
			if (cutNode != null) {
				view.ShowCut(cutNode);
			}
			copiedNode = null;
		}
		
		/// <summary>
		/// Gets whether the cut method is enabled.
		/// </summary>
		public bool IsCutEnabled {
			get {
				XmlNode selectedNode = view.SelectedNode;
				return selectedNode != null && document.DocumentElement != selectedNode;
			}
		}
		
		/// <summary>
		/// Gets whether the copy method is enabled.
		/// </summary>
		public bool IsCopyEnabled {
			get {
				return view.SelectedNode != null;
			}
		}
		
		/// <summary>
		/// Gets whether the paste method is enabled.
		/// </summary>
		public bool IsPasteEnabled {
			get {
				XmlNode destinationNode = view.SelectedNode;
				if (destinationNode != null) {
					XmlNode sourceNode = copiedNode ?? cutNode;
					if (sourceNode != null) {
						return GetPasteEnabled(sourceNode, destinationNode);
					}
				}
				return false;
			}
		}
		
		/// <summary>
		/// Gets whether the delete method is enabled.
		/// </summary>
		public bool IsDeleteEnabled {
			get { return view.SelectedNode != null; }
		}
		
		/// <summary>
		/// Gets the missing attributes for the specified element based
		/// on its associated schema.
		/// </summary>
		string[] GetMissingAttributes(XmlElement element)
		{
			XmlElementPath elementPath = GetElementPath(element);
			List<string> attributes = new List<string>();
			XmlSchemaCompletion schema = FindSchema(elementPath);
			if (schema != null) {
				XmlCompletionItemCollection completionItems = schema.GetAttributeCompletion(elementPath);
				foreach (XmlCompletionItem item in completionItems) {
					// Ignore existing attributes.
					if (!element.HasAttribute(item.Text)) {
						attributes.Add(item.Text);
					}
				}
			}
			return attributes.ToArray();
		}
		
		/// <summary>
		/// Returns the path to the specified element starting from the
		/// root element.
		/// </summary>
		static XmlElementPath GetElementPath(XmlElement element)
		{
			XmlElementPath path = new XmlElementPath();
			XmlElement parentElement = element;
			while (parentElement != null) {
				QualifiedName name = new QualifiedName(parentElement.LocalName, parentElement.NamespaceURI, parentElement.Prefix);
				path.Elements.Insert(0, name);
				
				// Move to parent element.
				parentElement = parentElement.ParentNode as XmlElement;
			}
			return path;
		}
		
		XmlSchemaCompletion FindSchema(XmlElementPath path)
		{
			XmlSchemaCompletion schema = schemas[path.GetRootNamespace()];
			if ((schema == null) && (defaultSchema != null)) {
				path.SetNamespaceForUnqualifiedNames(defaultSchema.NamespaceUri);
				return defaultSchema;
			}
			return null;
		}
		
		/// <summary>
		/// Returns a list of elements that can be children of the
		/// specified element.
		/// </summary>
		string[] GetChildElements(XmlElement element)
		{
			XmlElementPath elementPath = GetElementPath(element);
			
			List<string> elements = new List<string>();
			XmlSchemaCompletion schema = FindSchema(elementPath);
			if (schema != null) {
				XmlCompletionItemCollection completionItems = schema.GetChildElementCompletion(elementPath);
				foreach (XmlCompletionItem elementCompletionData in completionItems) {
					elements.Add(elementCompletionData.Text);
				}
			}
			return elements.ToArray();
		}
		
		/// <summary>
		/// Gets the current comment or element or text node
		/// from the view.
		/// </summary>
		XmlNode GetSelectedCommentOrElementOrTextNode()
		{
			XmlNode node = view.SelectedComment;
			if (node != null) {
				return node;
			}
			return GetSelectedElementOrTextNode();
		}
		
		/// <summary>
		/// Gets the currently selected element or text node.
		/// </summary>
		XmlNode GetSelectedElementOrTextNode()
		{
			XmlNode node = view.SelectedTextNode;
			if (node != null) {
				return node;
			}
			return view.SelectedElement;
		}
		
		/// <summary>
		/// Appends the specified element as a child to the selected element.
		/// </summary>
		void AppendChildElement(XmlElement element)
		{
			AppendChildElement(view.SelectedElement, element);
		}
		
		/// <summary>
		/// Appends the specified element as a child to the selected element.
		/// </summary>
		void AppendChildElement(XmlElement selectedElement, XmlElement element)
		{
			selectedElement.AppendChild(element);
			view.AppendChildElement(element);
			view.IsDirty = true;
		}
		
		/// <summary>
		/// Removes the specified element from the document.
		/// </summary>
		void RemoveElement(XmlElement element)
		{
			XmlNode parentNode = element.ParentNode;
			parentNode.RemoveChild(element);
			view.IsDirty = true;
			view.RemoveElement(element);
		}
		
		/// <summary>
		/// Removes the specified comment from the document.
		/// </summary>
		void RemoveComment(XmlComment comment)
		{
			XmlNode parentNode = comment.ParentNode;
			parentNode.RemoveChild(comment);
			view.IsDirty = true;
			view.RemoveComment(comment);
		}
		
		/// <summary>
		/// Removes the specified text node from the document.
		/// </summary>
		void RemoveTextNode(XmlText textNode)
		{
			XmlNode parentNode = textNode.ParentNode;
			parentNode.RemoveChild(textNode);
			view.IsDirty = true;
			view.RemoveTextNode(textNode);
		}
		
		/// <summary>
		/// Gets whether the source node can be pasted as a child
		/// node of the destination node.
		/// </summary>
		static bool GetPasteEnabled(XmlNode source, XmlNode destination)
		{
			if (source is XmlElement || source is XmlText || source is XmlComment) {
				return destination is XmlElement;
			}
			return false;
		}
		
		/// <summary>
		/// Takes a copy of the node and appends it to the selected
		/// node.
		/// </summary>
		void AppendChildCopy(XmlNode nodeToCopy)
		{
			if (nodeToCopy is XmlElement) {
				XmlElement copy = (XmlElement)nodeToCopy.CloneNode(true);
				AppendChildElement(copy);
			} else if (nodeToCopy is XmlText) {
				XmlText copy = (XmlText)nodeToCopy.CloneNode(true);
				AppendChildTextNode(copy);
			} else if (nodeToCopy is XmlComment) {
				XmlComment copy = (XmlComment)nodeToCopy.CloneNode(true);
				AppendChildComment(copy);
			}
		}
		
		/// <summary>
		/// Appends the specified text node to the currently selected element.
		/// </summary>
		void AppendChildTextNode(XmlText textNode)
		{
			XmlElement selectedElement = view.SelectedElement;
			if (selectedElement != null) {
				selectedElement.AppendChild(textNode);
				view.IsDirty = true;
				view.AppendChildTextNode(textNode);
			}
		}
		
		/// <summary>
		/// Cuts from the tree and pastes it to the currently selected node.
		/// </summary>
		void CutAndPasteNode(XmlNode node)
		{
			XmlElement cutElement = node as XmlElement;
			XmlText cutTextNode = node as XmlText;
			XmlComment cutCommentNode = node as XmlComment;
			if (cutElement != null) {
				CutAndPasteElement(cutElement);
			} else if (cutTextNode != null) {
				CutAndPasteTextNode(cutTextNode);
			} else if (cutCommentNode != null) {
				CutAndPasteComment(cutCommentNode);
			}
			cutNode = null;
		}
		
		/// <summary>
		/// Cuts the element from the document and pastes it as a child
		/// of the selected element.
		/// </summary>
		void CutAndPasteElement(XmlElement element)
		{
			if (element != view.SelectedElement) {
				view.RemoveElement(element);
				AppendChildElement(element);
			} else {
				// Pasting to the same cut element so just
				// change the tree icon back to the uncut state.
				view.HideCut(element);
			}
		}
		
		/// <summary>
		/// Cuts the text node from the document and appends it as a child
		/// of the currently selected element.
		/// </summary>
		void CutAndPasteTextNode(XmlText text)
		{
			view.RemoveTextNode(text);
			AppendChildTextNode(text);
		}
		
		/// <summary>
		/// Appends the specified comment to the currently selected element.
		/// </summary>
		void AppendChildComment(XmlComment comment)
		{
			XmlElement selectedElement = view.SelectedElement;
			if (selectedElement != null) {
				selectedElement.AppendChild(comment);
				view.IsDirty = true;
				view.AppendChildComment(comment);
			}
		}
		
		/// <summary>
		/// Cuts the comment node from the document and appends it as a child
		/// of the currently selected element.
		/// </summary>
		void CutAndPasteComment(XmlComment comment)
		{
			view.RemoveComment(comment);
			AppendChildComment(comment);
		}
	}
}
