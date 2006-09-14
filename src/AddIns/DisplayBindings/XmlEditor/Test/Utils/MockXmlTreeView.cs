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
	}
}
