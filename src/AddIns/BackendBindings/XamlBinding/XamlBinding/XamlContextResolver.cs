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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Collects data needed to generate completion and insigth info.
	/// </summary>
	public class XamlContextResolver
	{
		public static XamlContext ResolveContext(FileName fileName, string text, int offset)
		{
			return ResolveContext(fileName, new StringTextSource(text), offset);
		}
		
		public static XamlContext ResolveContext(FileName fileName, ITextSource fileContent, int offset)
		{
			XamlFullParseInformation info = SD.ParserService.Parse(fileName, fileContent) as XamlFullParseInformation;
			
			if (info == null)
				throw new Exception("need full parse info!");
			
			AXmlDocument document = info.Document;
			AXmlObject currentData = document.GetChildAtOffset(offset);
			
			string attributeName = string.Empty, attributeValue = string.Empty;
			AttributeValue value = null;
			bool isRoot = false;
			bool wasAXmlElement = false;
			int offsetFromValueStart = -1;
			
			List<AXmlElement> ancestors = new List<AXmlElement>();
			Dictionary<string, XamlNamespace> xmlns = new Dictionary<string, XamlNamespace>();
			List<string> ignored = new List<string>();
			string xamlNamespacePrefix = string.Empty;
			
			var item = currentData;
			
			while (item != document) {
				if (item is AXmlElement) {
					AXmlElement element = item as AXmlElement;
					ancestors.Add(element);
					foreach (var attr in element.Attributes) {
						if (attr.IsNamespaceDeclaration) {
							string prefix = (attr.Name == "xmlns") ? "" : attr.LocalName;
							if (!xmlns.ContainsKey(prefix))
								xmlns.Add(prefix, new XamlNamespace(attr.Value));
						}
						
						if (attr.LocalName == "Ignorable" && attr.Namespace == XamlConst.MarkupCompatibilityNamespace)
							ignored.AddRange(attr.Value.Split(' ', '\t'));
						
						if (string.IsNullOrEmpty(xamlNamespacePrefix) && attr.Value == XamlConst.XamlNamespace)
							xamlNamespacePrefix = attr.LocalName;
					}
					
					if (!wasAXmlElement && item.Parent is AXmlDocument)
						isRoot = true;
					
					wasAXmlElement = true;
				}
				
				item = item.Parent;
			}
			
			XamlContextDescription description = XamlContextDescription.None;
			
			AXmlElement active = null;
			AXmlElement parent = null;
			
			if (currentData is AXmlAttribute) {
				AXmlAttribute a = currentData as AXmlAttribute;
				int valueStartOffset = a.ValueSegment.Offset + 1;
				attributeName = a.Name;
				attributeValue = a.Value;
				value = MarkupExtensionParser.ParseValue(attributeValue);
				
				if (offset >= valueStartOffset && (offset < a.EndOffset // if length is < 2 one quote is missing
				                                   || (a.ValueSegment.Length <= 1 && offset <= a.EndOffset))) {
					offsetFromValueStart = offset - valueStartOffset;
					
					description = XamlContextDescription.InAttributeValue;
					
					if (value != null && !value.IsString)
						description = XamlContextDescription.InMarkupExtension;
					if (attributeValue.StartsWith("{}", StringComparison.Ordinal) && attributeValue.Length > 2)
						description = XamlContextDescription.InAttributeValue;
				} else
					description = XamlContextDescription.InTag;
				active = a.ParentElement;
			} else if (currentData is AXmlTag) {
				AXmlTag tag = currentData as AXmlTag;
				if (tag.IsStartOrEmptyTag || tag.IsEndTag) {
					if (tag.NameSegment.EndOffset < offset)
						description = XamlContextDescription.InTag;
					else
						description = XamlContextDescription.AtTag;
				} else if (tag.IsComment)
					description = XamlContextDescription.InComment;
				else if (tag.IsCData)
					description = XamlContextDescription.InCData;
				active = tag.Parent as AXmlElement;
			}
			
			if (active != ancestors.FirstOrDefault())
				parent = ancestors.FirstOrDefault();
			else
				parent = ancestors.Skip(1).FirstOrDefault();
			
			if (active == null)
				active = parent;
			
			var xAttribute = currentData as AXmlAttribute;
			
			var context = new XamlContext() {
				Description         = description,
				ActiveElement       = active,
				ParentElement       = parent,
				Ancestors           = ancestors.AsReadOnly(),
				Attribute           = xAttribute,
				InRoot              = isRoot,
				AttributeValue      = value,
				RawAttributeValue   = attributeValue,
				ValueStartOffset    = offsetFromValueStart,
				XmlnsDefinitions    = xmlns,
				ParseInformation    = info,
				IgnoredXmlns        = ignored.AsReadOnly(),
				XamlNamespacePrefix = xamlNamespacePrefix
			};
			
			return context;
		}
		
		public static XamlCompletionContext ResolveCompletionContext(ITextEditor editor, char typedValue)
		{
			var binding = editor.GetService(typeof(XamlTextEditorExtension)) as XamlTextEditorExtension;
			
			if (binding == null)
				throw new InvalidOperationException("Can only use ResolveCompletionContext with a XamlTextEditorExtension.");
			
			var context = new XamlCompletionContext(ResolveContext(editor.FileName, editor.Document, editor.Caret.Offset)) {
				PressedKey = typedValue,
				Editor = editor
			};
			
			return context;
		}
		
		public static XamlCompletionContext ResolveCompletionContext(ITextEditor editor, char typedValue, int offset)
		{
			var binding = editor.GetService(typeof(XamlTextEditorExtension)) as XamlTextEditorExtension;
			
			if (binding == null)
				throw new InvalidOperationException("Can only use ResolveCompletionContext with a XamlTextEditorExtension.");
			
			var context = new XamlCompletionContext(ResolveContext(editor.FileName, editor.Document, offset)) {
				PressedKey = typedValue,
				Editor = editor
			};
			
			return context;
		}
	}
}
