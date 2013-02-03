// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.WixBinding;
using NUnit.Framework;

namespace WixBinding.Tests.Document
{
	[TestFixture]
	public class RegionToOffsetTests
	{
		TextDocument document;
		
		[SetUp]
		public void Init()
		{
			document = new TextDocument();
		}
		
		[Test]
		public void SingleLineRegionConvertedToSegment()
		{
			DomRegion region = new DomRegion(1, 1, 1, 6);
			document.Text = "1234567890";
			WixDocumentLineSegment segment = WixDocumentLineSegment.ConvertRegionToSegment(document, region);
			
			WixDocumentLineSegment expectedSegment = new WixDocumentLineSegment(0, 6);
			
			Assert.AreEqual(expectedSegment, segment);
		}
		
		[Test]
		public void TwoLineRegionConvertedToSegment()
		{
			DomRegion region = new DomRegion(1, 2, 2, 1);
			document.Text = "1234567890\r\n1234567890";
			WixDocumentLineSegment segment = WixDocumentLineSegment.ConvertRegionToSegment(document, region);
			
			WixDocumentLineSegment expectedSegment = new WixDocumentLineSegment(1, 12);
			
			Assert.AreEqual(expectedSegment, segment);
		}
		
		[Test]
		public void ThreeLineRegionConvertedToSegment()
		{
			DomRegion region = new DomRegion(1, 3, 3, 2);
			document.Text = "1234567890\r\n1234567890\r\n1234567890";
			WixDocumentLineSegment segment = WixDocumentLineSegment.ConvertRegionToSegment(document, region);
			
			WixDocumentLineSegment expectedSegment = new WixDocumentLineSegment(2, 24);
			
			Assert.AreEqual(expectedSegment, segment);
		}
		
		[Test]
		public void ThreeLineRegionWithoutCarriageReturnConvertedToSegment()
		{
			DomRegion region = new DomRegion(1, 3, 3, 2);
			document.Text = "1234567890\n1234567890\n1234567890";
			WixDocumentLineSegment segment = WixDocumentLineSegment.ConvertRegionToSegment(document, region);
			
			WixDocumentLineSegment expectedSegment = new WixDocumentLineSegment(2, 22);
			
			Assert.AreEqual(expectedSegment, segment);
		}
		
		[Test]
		public void RegionWithBeginLineOnSecondLineConvertedToSegment()
		{
			DomRegion region = new DomRegion(2, 1, 3, 1);
			document.Text = "1234567890\r\n1234567890\r\n1234567890";
			WixDocumentLineSegment segment = WixDocumentLineSegment.ConvertRegionToSegment(document, region);
			
			WixDocumentLineSegment expectedSegment = new WixDocumentLineSegment(12, 13);
			
			Assert.AreEqual(expectedSegment, segment);
		}
	}
}
