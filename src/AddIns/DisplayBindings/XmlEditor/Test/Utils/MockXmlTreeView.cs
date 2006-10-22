// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		XmlElement documentElement;
		XmlElement selectedElement;
		List<XmlAttribute> attributesDisplayed = new List<XmlAttribute>();
		string textContentDisplayed;
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
		
		public MockXmlTreeView()
		{
		}
		
		public void ShowXmlIsNotWellFormedMessage(XmlException ex)
		{
			notWellFormedMessageDisplayed = true;
			notWellFormedException = ex;
		}
		
		public bool IsDirty {
			get {
				return dirty;
			}
			set {
				dirty = value;
			}
		}
		
		public XmlElement DocumentElement {
			get {
				return documentElement;
			}
			set {
				documentElement = value;
			}
		}
		
		public XmlElement SelectedElement {
			get {
				return selectedElement;
			}
			set {
				selectedElement = value;
			}
		}
		
		public XmlText SelectedTextNode {
			get {
				return selectedTextNode;
			}
			set {
				selectedTextNode = value;
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

		public string TextContent {
			get {
				return textContentDisplayed;
			}
		}
		
		public List<XmlAttribute> AttributesDisplayed {
			get {
				return attributesDisplayed;
			}
		}
		
		public string TextContentDisplayed {
			get {
				return textContentDisplayed;
			}
			set {
				textContentDisplayed = value;
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
	}
}
