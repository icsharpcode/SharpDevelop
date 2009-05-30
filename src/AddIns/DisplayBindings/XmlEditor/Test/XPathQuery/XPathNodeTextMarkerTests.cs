// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2498 $</version>
// </file>

using ICSharpCode.TextEditor.Document;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.XPathQuery
{
	[TestFixture]
	public class XPathNodeTextMarkerTests
	{		
		[Test]
		public void OneNodeMarked()
		{
			string xml = "<root><foo/></root>";
			XPathNodeMatch[] nodes = XmlView.SelectNodes(xml, "//root");
			
			IDocument doc = MockDocument.Create();
			doc.TextContent = xml;
			MarkerStrategy markerStrategy = new MarkerStrategy(doc);
			XPathNodeTextMarker.AddMarkers(markerStrategy, nodes);
			
			List<TextMarker> markers = new List<TextMarker>();
			foreach (TextMarker marker in markerStrategy.TextMarker) {
				markers.Add(marker);
			}
			
			// Remove markers.
			XPathNodeTextMarker.RemoveMarkers(markerStrategy);
			List<TextMarker> markersAfterRemove = new List<TextMarker>();
			foreach (TextMarker markerAfterRemove in markerStrategy.TextMarker) {
				markers.Add(markerAfterRemove);
			}

			XPathNodeTextMarker xpathNodeTextMarker = (XPathNodeTextMarker)markers[0];
			Assert.AreEqual(1, markers.Count);
			Assert.AreEqual(1, xpathNodeTextMarker.Offset);
			Assert.AreEqual(4, xpathNodeTextMarker.Length);
			Assert.AreEqual(TextMarkerType.SolidBlock, xpathNodeTextMarker.TextMarkerType);
			Assert.AreEqual(0, markersAfterRemove.Count);
			Assert.AreEqual(XPathNodeTextMarker.MarkerBackColor, xpathNodeTextMarker.Color);
		}
		
		/// <summary>
		/// Tests that XPathNodeMatch with an empty string value are not marked since
		/// the MarkerStrategy cannot use a TextMarker with a length of 0.
		/// </summary>
		[Test]
		public void EmptyCommentNode()
		{
			string xml = "<!----><root/>";
			XPathNodeMatch[] nodes = XmlView.SelectNodes(xml, "//comment()");
			
			IDocument doc = MockDocument.Create();
			doc.TextContent = xml;
			MarkerStrategy markerStrategy = new MarkerStrategy(doc);
			XPathNodeTextMarker.AddMarkers(markerStrategy, nodes);
			
			List<TextMarker> markers = new List<TextMarker>();
			foreach (TextMarker marker in markerStrategy.TextMarker) {
				markers.Add(marker);
			}
			
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
			string xml = "<?xml version='1.0'?>\r\n" +
				"<Xml1></Xml1>";
			XPathNodeMatch[] nodes = XmlView.SelectNodes(xml, "//namespace::*");
			
			IDocument doc = MockDocument.Create();
			doc.TextContent = xml;
			MarkerStrategy markerStrategy = new MarkerStrategy(doc);
			XPathNodeTextMarker.AddMarkers(markerStrategy, nodes);
			
			List<TextMarker> markers = new List<TextMarker>();
			foreach (TextMarker marker in markerStrategy.TextMarker) {
				markers.Add(marker);
			}
			Assert.AreEqual(0, markers.Count);
			Assert.AreEqual(1, nodes.Length);
		}
	}
}
