// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Document
{
	[TestFixture]
	public class RegionToOffsetTests
	{
		[Test]
		public void SingleLine()
		{
			DomRegion region = new DomRegion(0, 0, 0, 5);
			DocumentFactory factory = new DocumentFactory();
			IDocument document = factory.CreateDocument();
			document.TextContent = "1234567890";
			ISegment segment = WixDocument.ConvertRegionToSegment(document, region);
			
			WixDocumentLineSegment expectedSegment = new WixDocumentLineSegment(0, 6);
			
			Assert.AreEqual(expectedSegment, segment);
		}
		
		[Test]
		public void TwoLines()
		{
			DomRegion region = new DomRegion(0, 1, 1, 0);
			DocumentFactory factory = new DocumentFactory();
			IDocument document = factory.CreateDocument();
			document.TextContent = "1234567890\r\n1234567890";
			ISegment segment = WixDocument.ConvertRegionToSegment(document, region);
			
			WixDocumentLineSegment expectedSegment = new WixDocumentLineSegment(1, 12);
			
			Assert.AreEqual(expectedSegment, segment);
		}
		
		[Test]
		public void ThreeLines()
		{
			DomRegion region = new DomRegion(0, 2, 2, 1);
			DocumentFactory factory = new DocumentFactory();
			IDocument document = factory.CreateDocument();
			document.TextContent = "1234567890\r\n1234567890\r\n1234567890";
			ISegment segment = WixDocument.ConvertRegionToSegment(document, region);
			
			WixDocumentLineSegment expectedSegment = new WixDocumentLineSegment(2, 24);
			
			Assert.AreEqual(expectedSegment, segment);
		}
		
		[Test]
		public void ThreeLinesWithoutCarriageReturn()
		{
			DomRegion region = new DomRegion(0, 2, 2, 1);
			DocumentFactory factory = new DocumentFactory();
			IDocument document = factory.CreateDocument();
			document.TextContent = "1234567890\n1234567890\n1234567890";
			ISegment segment = WixDocument.ConvertRegionToSegment(document, region);
			
			WixDocumentLineSegment expectedSegment = new WixDocumentLineSegment(2, 22);
			
			Assert.AreEqual(expectedSegment, segment);
		}
		
		[Test]
		public void BeginLineOnSecondLine()
		{
			DomRegion region = new DomRegion(1, 0, 2, 0);
			DocumentFactory factory = new DocumentFactory();
			IDocument document = factory.CreateDocument();
			document.TextContent = "1234567890\r\n1234567890\r\n1234567890";
			ISegment segment = WixDocument.ConvertRegionToSegment(document, region);
			
			WixDocumentLineSegment expectedSegment = new WixDocumentLineSegment(12, 13);
			
			Assert.AreEqual(expectedSegment, segment);
		}
	}
}
