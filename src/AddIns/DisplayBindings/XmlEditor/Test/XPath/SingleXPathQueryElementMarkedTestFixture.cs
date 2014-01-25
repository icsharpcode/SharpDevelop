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
			XPathNodeTextMarker.RemoveMarkers(doc);
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
