// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2498 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Xml;
using System.Xml.XPath;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
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
			
			ServiceContainer container = new ServiceContainer();
			container.AddService(typeof(ITextMarkerService), new MockTextMarkerService());

			
			AvalonEditDocumentAdapter doc = new AvalonEditDocumentAdapter(container);
			doc.Text = xml;
			XPathNodeTextMarker.AddMarkers(doc, nodes);
			
			ITextMarkerService service = doc.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			
			List<ITextMarker> markers = new List<ITextMarker>();
			foreach (ITextMarker marker in service.TextMarkers) {
				markers.Add(marker);
			}
			
			// Remove markers.
			XPathNodeTextMarker.RemoveMarkers(doc);
			List<ITextMarker> markersAfterRemove = new List<ITextMarker>();
			foreach (ITextMarker markerAfterRemove in service.TextMarkers) {
				markersAfterRemove.Add(markerAfterRemove);
			}

			ITextMarker xpathNodeTextMarker = markers[0];
			Assert.AreEqual(1, markers.Count, "markers.Count");
			Assert.AreEqual(1, xpathNodeTextMarker.StartOffset, "startoffset");
			Assert.AreEqual(4, xpathNodeTextMarker.Length, "length");
			Assert.AreEqual(0, markersAfterRemove.Count, "afterremove.count");
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
			
			ServiceContainer container = new ServiceContainer();
			container.AddService(typeof(MockTextMarkerService), new MockTextMarkerService());
			
			AvalonEditDocumentAdapter doc = new AvalonEditDocumentAdapter(container);
			doc.Text = xml;
			XPathNodeTextMarker.AddMarkers(doc, nodes);
			
			ITextMarkerService service = doc.GetService(typeof(MockTextMarkerService)) as ITextMarkerService;
			
			List<ITextMarker> markers = new List<ITextMarker>();
			foreach (ITextMarker marker in service.TextMarkers) {
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
			
			ServiceContainer container = new ServiceContainer();
			container.AddService(typeof(MockTextMarkerService), new MockTextMarkerService());
			
			AvalonEditDocumentAdapter doc = new AvalonEditDocumentAdapter(container);
			doc.Text = xml;
			XPathNodeTextMarker.AddMarkers(doc, nodes);
			
			ITextMarkerService service = doc.GetService(typeof(MockTextMarkerService)) as ITextMarkerService;
			
			List<ITextMarker> markers = new List<ITextMarker>();
			foreach (ITextMarker marker in service.TextMarkers) {
				markers.Add(marker);
			}
			Assert.AreEqual(0, markers.Count);
			Assert.AreEqual(1, nodes.Length);
		}
	}
}
