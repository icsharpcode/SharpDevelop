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
using System.IO;
using System.Xml;
using System.Xml.XPath;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XmlEditor
{
	public class XPathQuery
	{
		string xml;
		XmlNamespaceCollection namespaces;
		
		public XPathQuery(string xml)
			:  this(xml, new XmlNamespaceCollection())
		{
		}
		
		public XPathQuery(ITextEditor textEditor, XmlNamespaceCollection namespaces)
			: this(textEditor.Document.Text, namespaces)
		{
		}
		
		public XPathQuery(string xml, XmlNamespaceCollection namespaces)
		{
			this.xml = xml;
			this.namespaces = namespaces;
		}
		
		/// <summary>
		/// Finds the xml nodes that match the specified xpath.
		/// </summary>
		/// <returns>An array of XPathNodeMatch items. These include line number
		/// and line position information aswell as the node found.</returns>
		public XPathNodeMatch[] FindNodes(string xpath)
		{
			XmlTextReader xmlReader = new XmlTextReader(new StringReader(xml));
			xmlReader.XmlResolver = null;
			XPathDocument doc = new XPathDocument(xmlReader);
			XPathNavigator navigator = doc.CreateNavigator();
			
			// Add namespaces.
			XmlNamespaceManager namespaceManager = new XmlNamespaceManager(navigator.NameTable);
			foreach (XmlNamespace xmlNamespace in namespaces) {
				namespaceManager.AddNamespace(xmlNamespace.Prefix, xmlNamespace.Name);
			}
			
			// Run the xpath query.
			XPathNodeIterator iterator = navigator.Select(xpath, namespaceManager);
			
			List<XPathNodeMatch> nodes = new List<XPathNodeMatch>();
			while (iterator.MoveNext()) {
				nodes.Add(new XPathNodeMatch(iterator.Current));
			}
			return nodes.ToArray();
		}
	}
}
