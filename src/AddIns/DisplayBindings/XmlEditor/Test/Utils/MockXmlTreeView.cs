// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.XmlEditor;
using System;
using System.Collections.Generic;
using System.Xml;

namespace XmlEditor.Tests.Utils
{
	public class MockXmlTreeView : IXmlTreeView
	{
		bool notWellFormedMessageDisplayed;
		XmlException notWellFormedException;
		XmlDocument document;
		XmlElement selectedElement;
		List<XmlAttribute> attributesDisplayed = new List<XmlAttribute>();
		string textContentDisplayed;
		bool showTextContentCalled;
		XmlText selectedTextNode;
		bool dirty;
		bool selectNewAttributesCalled = false;
		List<string> selectNewAttributesList = new List<string>();
		List<string> selectedNewAttributesToReturn = new List<string>();
		string selectedAttribute = null;
		bool selectNewElementsCalled = false;
		List<string> selectedNewElementsToReturn = new List<string>();
		List<string> selectNewElementsList = new List<string>();
		List<XmlElement> childElementsAdded = new List<XmlElement>();
		List<XmlElement> elementsInsertedBefore = new List<XmlElement>();
		List<XmlElement> elementsInsertedAfter = new List<XmlElement>();
		List<XmlElement> elementsRemoved = new List<XmlElement>();
		List<XmlText> childTextNodesAdded = new List<XmlText>();
		List<XmlText> textNodesInsertedBefore = new List<XmlText>();
		List<XmlText> textNodesInsertedAfter = new List<XmlText>();
		List<XmlText> textNodesRemoved = new List<XmlText>();
		List<XmlText> textNodesUpdated = new List<XmlText>();
		XmlComment selectedCommentNode;
		List<XmlComment> commentNodesUpdated = new List<XmlComment>();
		List<XmlComment> childCommentNodesAdded = new List<XmlComment>();
		List<XmlComment> commentNodesRemoved = new List<XmlComment>();
		List<XmlComment> commentNodesInsertedBefore = new List<XmlComment>();
		List<XmlComment> commentNodesInsertedAfter = new List<XmlComment>();
		XmlNode selectedNode;
		List<XmlNode> cutNodes = new List<XmlNode>();
		List<XmlNode> hiddenCutNodes = new List<XmlNode>();
			
		public MockXmlTreeView()
		{
		}
		
		public void ShowXmlIsNotWellFormedMessage(XmlException ex)
		{
			notWellFormedMessageDisplayed = true;
			notWellFormedException = ex;
		}
		
		public void ShowErrorMessage(string message)
		{
		}
		
		public bool IsDirty {
			get {
				return dirty;
			}
			set {
				dirty = value;
			}
		}
		
		public XmlDocument Document {
			get {
				return document;
			}
			set {
				document = value;
			}
		}
		
		public XmlElement SelectedElement {
			get {
				return selectedElement;
			}
			set {
				selectedElement = value;
				selectedNode = value;
			}
		}
		
		public XmlText SelectedTextNode {
			get {
				return selectedTextNode;
			}
			set {
				selectedTextNode = value;
				selectedNode = value;
			}
		}
		
		public XmlComment SelectedComment {
			get {
				return selectedCommentNode;
			}
			set {
				selectedCommentNode = value;
				selectedNode = value;
			}
		}
		
		public XmlNode SelectedNode {
			get {
				return selectedNode;
			}
			set {
				selectedNode = value;
			}
		}

		public void ShowAttributes(XmlAttributeCollection attributes)
		{
			attributesDisplayed = new List<XmlAttribute>();
			foreach (XmlAttribute attribute in attributes) {
				attributesDisplayed.Add(attribute);
			}
		}
		
		public void ClearAttributes()
		{
			attributesDisplayed.Clear();
		}
		
		public void ShowTextContent(string text)
		{
			showTextContentCalled = true;
			textContentDisplayed = text;
		}
				
		public string[] SelectNewAttributes(string[] attributes)
		{
			selectNewAttributesCalled = true;
			selectNewAttributesList.AddRange(attributes);
			return selectedNewAttributesToReturn.ToArray();
		}
		
		public string SelectedAttribute {
			get {
				return selectedAttribute;
			}
			set {
				selectedAttribute = value;
			}
		}
		
		public string[] SelectNewElements(string[] elements)
		{
			selectNewElementsCalled = true;
			selectNewElementsList.AddRange(elements);
			return selectedNewElementsToReturn.ToArray();
		}
		
		public void AppendChildElement(XmlElement element)
		{
			childElementsAdded.Add(element);
		}
		
		public void InsertElementBefore(XmlElement element)
		{
			elementsInsertedBefore.Add(element);
		}
		
		public void InsertElementAfter(XmlElement element)
		{
			elementsInsertedAfter.Add(element);
		}
		
		public void RemoveElement(XmlElement element)
		{
			elementsRemoved.Add(element);
		}

		public void AppendChildTextNode(XmlText textNode)
		{
			childTextNodesAdded.Add(textNode);
		}
		
		public void InsertTextNodeBefore(XmlText textNode)
		{
			textNodesInsertedBefore.Add(textNode);
		}
		
		public void InsertTextNodeAfter(XmlText textNode)
		{
			textNodesInsertedAfter.Add(textNode);
		}
		
		public void RemoveTextNode(XmlText textNode)
		{
			textNodesRemoved.Add(textNode);
		}
		
		public void UpdateTextNode(XmlText textNode)
		{
			textNodesUpdated.Add(textNode);
		}
		
		public void UpdateComment(XmlComment comment)
		{
			commentNodesUpdated.Add(comment);
		}
		
		public void AppendChildComment(XmlComment comment)
		{
			childCommentNodesAdded.Add(comment);
		}
		
		public void RemoveComment(XmlComment comment)
		{
			commentNodesRemoved.Add(comment);
		}
		
		public void InsertCommentBefore(XmlComment comment)
		{
			commentNodesInsertedBefore.Add(comment);
		}
		
		public void InsertCommentAfter(XmlComment comment)
		{
			commentNodesInsertedAfter.Add(comment);
		}
		
		public void ShowCut(XmlNode node)
		{
			cutNodes.Add(node);
		}
		
		public void HideCut(XmlNode node)
		{
			hiddenCutNodes.Add(node);
		}
		
		public string TextContent {
			get {
				return textContentDisplayed;
			}
			set {
				textContentDisplayed = value;
			}
		}
		
		public List<XmlAttribute> AttributesDisplayed {
			get {
				return attributesDisplayed;
			}
		}
	
		public bool IsXmlNotWellFormedMessageDisplayed {
			get {
				return notWellFormedMessageDisplayed;
			}
		}
		
		public XmlException NotWellFormedExceptionPassed {
			get {
				return notWellFormedException;
			}
		}
		
		/// <summary>
		/// Gets whether the SelectNewAttributes method has been called.
		/// </summary>
		public bool IsSelectNewAttributesCalled {
			get {
				return selectNewAttributesCalled;
			}
			set {
				selectNewAttributesCalled = value;
			}
		}
		
		/// <summary>
		/// Gets the set of attributes that will be returned from the
		/// SelectNewAttributes method.
		/// </summary>
		public List<string> SelectedNewAttributesToReturn {
			get {
				return selectedNewAttributesToReturn;
			}
		}
		
		/// <summary>
		/// Returns the list of attributes passed to the 
		/// SelectNewAttribute method.
		/// </summary>
		public List<string> SelectNewAttributesList {
			get {
				return selectNewAttributesList;
			}
		}
		
		/// <summary>
		/// Gets whether the SelectNewElements method was called. 
		/// </summary>
		public bool IsSelectNewElementsCalled {
			get {
				return selectNewElementsCalled;
			}
			set {
				selectNewElementsCalled = value;
			}
		}
		
		/// <summary>
		/// Gets the set of elements that will be returned from the
		/// SelectNewElements method.
		/// </summary>
		public List<string> SelectedNewElementsToReturn {
			get {
				return selectedNewElementsToReturn;
			}
		}
		
		/// <summary>
		/// Returns the list of elements passed to the 
		/// SelectNewElement method.
		/// </summary>
		public List<string> SelectNewElementsList {
			get {
				return selectNewElementsList;
			}
		}
		
		/// <summary>
		/// Returns the child elements added via the AppendChildElement
		/// method.
		/// </summary>
		public List<XmlElement> ChildElementsAdded {
			get {
				return childElementsAdded;
			}
		}

		/// <summary>
		/// Returns the elements added via the InsertElementBefore
		/// method.
		/// </summary>
		public List<XmlElement> ElementsInsertedBefore {
			get {
				return elementsInsertedBefore;
			}
		}
		
		/// <summary>
		/// Returns the elements added via the InsertElementAfter
		/// method.
		/// </summary>
		public List<XmlElement> ElementsInsertedAfter {
			get {
				return elementsInsertedAfter;
			}
		}
				
		/// <summary>
		/// Returns the elements removed via the RemoveElement
		/// method.
		/// </summary>
		public List<XmlElement> ElementsRemoved {
			get {
				return elementsRemoved;
			}
		}
		
		/// <summary>
		/// Returns the text nodes added via the AppendChildTextNode
		/// method.
		/// </summary>
		public List<XmlText> ChildTextNodesAdded {
			get {
				return childTextNodesAdded;
			}
		}
		
		/// <summary>
		/// Returns the text nodes that were inserted via the
		/// InsertTextNodeBefore method.
		/// </summary>
		public List<XmlText> TextNodesInsertedBefore {
			get {
				return textNodesInsertedBefore;
			}
		}
		
		/// <summary>
		/// Returns the text nodes that were inserted via the
		/// InsertTextNodeAfter method.
		/// </summary>
		public List<XmlText> TextNodesInsertedAfter {
			get {
				return textNodesInsertedAfter;
			}
		}
		
		/// <summary>
		/// Returns the text nodes that were removed via the
		/// RemoveTextNode method.
		/// </summary>
		public List<XmlText> TextNodesRemoved {
			get {
				return textNodesRemoved;
			}
		}
		
		/// <summary>
		/// Returns the text nodes that were updated via the
		/// UpdateTextNode method.
		/// </summary>
		public List<XmlText> TextNodesUpdated {
			get {
				return textNodesUpdated;
			}
		}
		
		/// <summary>
		/// Returns a flag indicating whether the ShowTextContent
		/// method was called.
		/// </summary>
		public bool IsShowTextContentCalled {
			get {
				return showTextContentCalled;
			}
		}
		
		/// <summary>
		/// Returns the comment nodes that were updated via the
		/// UpdateCommentNode method.
		/// </summary>
		public List<XmlComment> CommentNodesUpdated {
			get {
				return commentNodesUpdated;
			}
		}
		
		/// <summary>
		/// Returns the comment nodes that were added via the
		/// AddChildComment method.
		/// </summary>
		public List<XmlComment> ChildCommentNodesAdded {
			get {
				return childCommentNodesAdded;
			}
		}
		
		/// <summary>
		/// Returns the comment nodes that were removed via the
		/// RemoveComment method.
		/// </summary>
		public List<XmlComment> CommentNodesRemoved {
			get {
				return commentNodesRemoved;
			}
		}
				
		/// <summary>
		/// Returns the comment nodes that were inserted via the
		/// InsertCommentBefore method.
		/// </summary>
		public List<XmlComment> CommentNodesInsertedBefore {
			get {
				return commentNodesInsertedBefore;
			}
		}
		
		/// <summary>
		/// Returns the comment nodes that were inserted via the
		/// InsertCommentAfter method.
		/// </summary>
		public List<XmlComment> CommentNodesInsertedAfter {
			get {
				return commentNodesInsertedAfter;
			}
		}
		
		/// <summary>
		/// Returns the nodes that used when informing the view
		/// that a particular node was about to be cut.
		/// </summary>
		public List<XmlNode> CutNodes {
			get {
				return cutNodes;
			}
		}
		
		/// <summary>
		/// Returns the nodes where the cut has been hidden via
		/// the HideCut method.
		/// </summary>
		public List<XmlNode> HiddenCutNodes {
			get {
				return hiddenCutNodes;
			}
		}
	}
}
