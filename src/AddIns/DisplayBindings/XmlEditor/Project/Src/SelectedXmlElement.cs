// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
