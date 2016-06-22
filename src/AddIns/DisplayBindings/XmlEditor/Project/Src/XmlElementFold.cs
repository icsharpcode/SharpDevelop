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
using System.Text;
using System.Xml;

using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.XmlEditor
{
	public class XmlElementFold
	{
		int line = 0;
		int column = 0;
		
		int endLine = 0;
		int endColumn = 0;
		
		string prefix = String.Empty;
		string name = String.Empty;
		string qualifiedElementName = String.Empty;
		string elementDisplayText = String.Empty;
		string elementWithAttributesDisplayText = String.Empty;
		
		public void ReadStart(XmlTextReader reader)
		{
			// Take off 1 from the line position returned 
			// from the xml since it points to the start
			// of the element name and not the beginning 
			// tag.
			column = reader.LinePosition - 1;
			line = reader.LineNumber;
			
			prefix = reader.Prefix;
			name = reader.LocalName;
			
			GetQualifiedElementName();
			GetElementDisplayText(qualifiedElementName);
			GetElementWithAttributesDisplayText(reader);
		}
		
		void GetQualifiedElementName()
		{
			if (prefix.Length > 0) {
				qualifiedElementName = String.Format("{0}:{1}", prefix, name);
			} else {
				qualifiedElementName = name;
			}
		}
		
		void GetElementDisplayText(string qualifiedName)
		{
			elementDisplayText = String.Format("<{0}>", qualifiedName);
		}
		
		/// <summary>
		/// Gets the element's attributes as a string on one line that will
		/// be displayed when the element is folded.
		/// </summary>
		void GetElementWithAttributesDisplayText(XmlTextReader reader)
		{
			string attributesDisplayText = GetAttributesDisplayText(reader);
			if (String.IsNullOrEmpty(attributesDisplayText)) {
				elementWithAttributesDisplayText = elementDisplayText;
			} else {
				elementWithAttributesDisplayText = String.Format("<{0} {1}>", qualifiedElementName, attributesDisplayText);
			}
		}
		
		string GetAttributesDisplayText(XmlTextReader reader)
		{
			StringBuilder text = new StringBuilder();
			
			for (int i = 0; i < reader.AttributeCount; ++i) {
				reader.MoveToAttribute(i);
				
				text.Append(reader.Name);
				text.Append("=");
				text.Append(reader.QuoteChar.ToString());
				text.Append(XmlEncodeAttributeValue(reader.Value, reader.QuoteChar));
				text.Append(reader.QuoteChar.ToString());
				
				// Append a space if this is not the
				// last attribute.
				if (!IsLastAttributeIndex(i, reader)) {
					text.Append(" ");
				}
			}
			
			return text.ToString();
		}
		
		/// <summary>
		/// Xml encode the attribute string since the string returned from
		/// the XmlTextReader is the plain unencoded string and .NET
		/// does not provide us with an xml encode method.
		/// </summary>
		static string XmlEncodeAttributeValue(string attributeValue, char quoteChar)
		{
			StringBuilder encodedValue = new StringBuilder(attributeValue);
			
			encodedValue.Replace("&", "&amp;");
			encodedValue.Replace("<", "&lt;");
			encodedValue.Replace(">", "&gt;");
			
			if (quoteChar == '"') {
				encodedValue.Replace("\"", "&quot;");
			} else {
				encodedValue.Replace("'", "&apos;");
			}
			
			return encodedValue.ToString();
		}
		
		bool IsLastAttributeIndex(int attributeIndex, XmlTextReader reader)
		{
			 return attributeIndex == (reader.AttributeCount - 1);
		}
		
		public void ReadEnd(XmlTextReader reader)
		{
			endLine = reader.LineNumber;
			int columnAfterEndTag = reader.LinePosition + qualifiedElementName.Length + 1;
			endColumn = columnAfterEndTag;
		}
		
		public FoldingRegion CreateFoldingRegion()
		{
			return CreateFoldingRegion(elementDisplayText);
		}
		
		FoldingRegion CreateFoldingRegion(string displayText)
		{
			DomRegion region = new DomRegion(line, column, endLine, endColumn);
			return new FoldingRegion(displayText, region);
		}
		
		public FoldingRegion CreateFoldingRegionWithAttributes()
		{
			return CreateFoldingRegion(elementWithAttributesDisplayText);
		}
		
		public bool IsSingleLine {
			get { return line == endLine; }
		}
	}
}
