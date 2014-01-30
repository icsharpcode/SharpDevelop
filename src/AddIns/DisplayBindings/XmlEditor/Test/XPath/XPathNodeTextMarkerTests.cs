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
using System.ComponentModel.Design;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Tests.Utils;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.XPath
{
	[TestFixture]
	public class XPathNodeTextMarkerTests
	{
		/// <summary>
		/// Tests that XPathNodeMatch with an empty string value is not marked since
		/// the MarkerStrategy cannot use a TextMarker with a length of 0.
		/// </summary>
		[Test]
		public void EmptyCommentNode()
		{
			string xml = "<!----><root/>";
			XPathQuery query = new XPathQuery(xml);
			XPathNodeMatch[] nodes = query.FindNodes("//comment()");
			
			ServiceContainer container = new ServiceContainer();
			container.AddService(typeof(MockTextMarkerService), new MockTextMarkerService());
			
			IDocument doc = new ICSharpCode.AvalonEdit.Document.TextDocument() { ServiceProvider = container };
			doc.Text = xml;
			XPathNodeTextMarker xpathNodeMarker = new XPathNodeTextMarker(doc);
			xpathNodeMarker.AddMarkers(nodes);
			
			ITextMarkerService service = doc.GetService(typeof(MockTextMarkerService)) as ITextMarkerService;
			List<ITextMarker> markers = new List<ITextMarker>(service.TextMarkers);
			
			Assert.AreEqual(0, markers.Count);
			Assert.AreEqual(1, nodes.Length);
		}
		
		/// <summary>
		/// Note that the XPathDocument.SelectNodes call returns a bad XPathNode set
		/// back. It finds a namespace node at 0, 0, even though it uses one based
		/// line information, it should really return false from HasLineInfo, but it
		/// does not. In our XPathNodeMatch we return false from HasLineInfo.
		/// </summary>
		[Test]
		public void NamespaceQuery()
		{
			string xml = 
				"<?xml version='1.0'?>\r\n" +
				"<Xml1></Xml1>";
			XPathQuery query = new XPathQuery(xml);
			XPathNodeMatch[] nodes = query.FindNodes("//namespace::*");
			
			ServiceContainer container = new ServiceContainer();
			container.AddService(typeof(MockTextMarkerService), new MockTextMarkerService());
			
			IDocument doc = new ICSharpCode.AvalonEdit.Document.TextDocument() { ServiceProvider = container };
			doc.Text = xml;
			XPathNodeTextMarker xpathNodeMarker = new XPathNodeTextMarker(doc);
			xpathNodeMarker.AddMarkers(nodes);
			
			ITextMarkerService service = doc.GetService(typeof(MockTextMarkerService)) as ITextMarkerService;
			List<ITextMarker> markers = new List<ITextMarker>(service.TextMarkers);
			
			Assert.AreEqual(0, markers.Count);
			Assert.AreEqual(1, nodes.Length);
		}
	}
}
