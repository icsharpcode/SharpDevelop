// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XmlEditor
{
	public class SelectedXmlElement
	{
		XmlElementPath path = new XmlElementPath();
		string selectedAttribute = String.Empty;
		string selectedAttributeValue = String.Empty;
		
		public SelectedXmlElement(ITextEditor textEditor)
			: this(textEditor.Document.Text, textEditor.Caret.Offset)
		{
		}
		
		public SelectedXmlElement(string xml, int index)
		{
			FindSelectedElement(xml, index);
			FindSelectedAttribute(xml, index);
			FindSelectedAttributeValue(xml, index);
		}
		
		void FindSelectedElement(string xml, int index)
		{
			path = XmlParser.GetActiveElementStartPathAtIndex(xml, index);
		}
		
		void FindSelectedAttribute(string xml, int index)
		{
			selectedAttribute = XmlParser.GetAttributeNameAtIndex(xml, index);
		}
		
		void FindSelectedAttributeValue(string xml, int index)
		{
			selectedAttributeValue = XmlParser.GetAttributeValueAtIndex(xml, index);
		}
		
		public XmlElementPath Path {
			get { return path; }
		}
		
		public string SelectedAttribute {
			get { return selectedAttribute; }
		}
		
		public bool HasSelectedAttribute {
			get { return selectedAttribute.Length > 0; }
		}
		
		public string SelectedAttributeValue {
			get { return selectedAttributeValue; }
		}
		
		public bool HasSelectedAttributeValue {
			get { return selectedAttributeValue.Length > 0; }
		}
	}
}
