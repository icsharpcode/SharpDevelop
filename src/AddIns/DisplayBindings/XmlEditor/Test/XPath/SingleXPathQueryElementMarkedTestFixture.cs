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
	public class SingleXPathQueryElementMarkedTestFixture
	{
		ITextMarker xpathNodeTextMarker;
		List<ITextMarker> markers;
		List<ITextMarker> markersAfterRemove;
		
		[SetUp]
		public void Init()
		{
			string xml = "<root><foo/></root>";
			XPathQuery query = new XPathQuery(xml);
			XPathNodeMatch[] nodes = query.FindNodes("//root");
			
			IDocument doc = MockTextMarkerService.CreateDocumentWithMockService();
			doc.Text = xml;
			XPathNodeTextMarker xpathNodeMarker = new XPathNodeTextMarker(doc);
			xpathNodeMarker.AddMarkers(nodes);
			
			ITextMarkerService service = doc.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			markers = new List<ITextMarker>(service.TextMarkers);
			
			// Remove markers.
			xpathNodeMarker.RemoveMarkers();
			markersAfterRemove = new List<ITextMarker>(service.TextMarkers);
			
			xpathNodeTextMarker = markers[0];
		}
		
		[Test]
		public void OneTextMarkerAddedForXPathMatch()
		{
			Assert.AreEqual(1, markers.Count);
		}
		
		[Test]
		public void StartOffsetForXPathNodeMarkerIsOne()
		{
			Assert.AreEqual(1, xpathNodeTextMarker.StartOffset);
		}
		
		[Test]
		public void LengthForXpathNodeMarkerIsFour()
		{
			Assert.AreEqual(4, xpathNodeTextMarker.Length);
		}
		
		[Test]
		public void NoTextMarkersAfterXPathNodeMarkerRemoveMarkersCalled()
		{
			Assert.AreEqual(0, markersAfterRemove.Count);
		}
	}
}
