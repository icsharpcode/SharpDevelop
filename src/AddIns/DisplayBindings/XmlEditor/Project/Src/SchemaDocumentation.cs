// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
