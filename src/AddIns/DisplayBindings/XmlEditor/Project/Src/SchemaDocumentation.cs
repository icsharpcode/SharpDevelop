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
using System.Xml.Schema;

namespace ICSharpCode.XmlEditor
{
	public class SchemaDocumentation
	{
		XmlSchemaAnnotation annotation;
		StringBuilder documentation = new StringBuilder();
		StringBuilder documentationWithoutWhitespace = new StringBuilder();
		
		public SchemaDocumentation(XmlSchemaAnnotation annotation)
		{
			this.annotation = annotation;
			if (annotation != null) {
				ReadDocumentationFromAnnotation(annotation.Items);
			}
		}
		
		void ReadDocumentationFromAnnotation(XmlSchemaObjectCollection annotationItems)
		{
			foreach (XmlSchemaObject schemaObject in annotationItems) {
				XmlSchemaDocumentation schemaDocumentation = schemaObject as XmlSchemaDocumentation;
				if (schemaDocumentation != null) {
					ReadSchemaDocumentationFromMarkup(schemaDocumentation.Markup);
				}
			}
			RemoveWhitespaceFromDocumentation();
		}
		
		void ReadSchemaDocumentationFromMarkup(XmlNode[] markup)
		{
			foreach (XmlNode node in markup) {
				XmlText textNode = node as XmlText;
				AppendTextToDocumentation(textNode);
			}
		}
		
		void AppendTextToDocumentation(XmlText textNode)
		{
			if (textNode != null) {
				if (textNode.Data != null) {
					documentation.Append(textNode.Data);
				}
			}
		}
		
		void RemoveWhitespaceFromDocumentation()
		{
			string[] lines = documentation.ToString().Split('\n');
			RemoveWhitespaceFromLines(lines);
		}
		
		void RemoveWhitespaceFromLines(string[] lines)
		{
			foreach (string line in lines) {
				string lineWithoutWhitespace = line.Trim();
				documentationWithoutWhitespace.AppendLine(lineWithoutWhitespace);
			}
		}
		
		public override string ToString()
		{
			return documentationWithoutWhitespace.ToString().Trim();
		}
	}
}
