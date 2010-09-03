// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

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
			
			AvalonEditDocumentAdapter doc = new AvalonEditDocumentAdapter(new ICSharpCode.AvalonEdit.Document.TextDocument(), container);
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
			
			AvalonEditDocumentAdapter doc = new AvalonEditDocumentAdapter(new ICSharpCode.AvalonEdit.Document.TextDocument(), container);
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
