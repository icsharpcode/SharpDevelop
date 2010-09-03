// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
