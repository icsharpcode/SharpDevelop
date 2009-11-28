// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Document
{
	[TestFixture]
	public class RegionToOffsetTests
	{
		AvalonEditDocumentAdapter document;
		
		[SetUp]
		public void Init()
		{
			document = new AvalonEditDocumentAdapter();
		}
		
		[Test]
		public void SingleLineRegionConvertedToSegment()
		{
			DomRegion region = new DomRegion(0, 0, 0, 5);
			document.Text = "1234567890";
			WixDocumentLineSegment segment = WixDocumentLineSegment.ConvertRegionToSegment(document, region);
			
			WixDocumentLineSegment expectedSegment = new WixDocumentLineSegment(0, 6);
			
			Assert.AreEqual(expectedSegment, segment);
		}
		
		[Test]
		public void TwoLineRegionConvertedToSegment()
		{
			DomRegion region = new DomRegion(0, 1, 1, 0);
			document.Text = "1234567890\r\n1234567890";
			WixDocumentLineSegment segment = WixDocumentLineSegment.ConvertRegionToSegment(document, region);
			
			WixDocumentLineSegment expectedSegment = new WixDocumentLineSegment(1, 12);
			
			Assert.AreEqual(expectedSegment, segment);
		}
		
		[Test]
		public void ThreeLineRegionConvertedToSegment()
		{
			DomRegion region = new DomRegion(0, 2, 2, 1);
			document.Text = "1234567890\r\n1234567890\r\n1234567890";
			WixDocumentLineSegment segment = WixDocumentLineSegment.ConvertRegionToSegment(document, region);
			
			WixDocumentLineSegment expectedSegment = new WixDocumentLineSegment(2, 24);
			
			Assert.AreEqual(expectedSegment, segment);
		}
		
		[Test]
		public void ThreeLineRegionWithoutCarriageReturnConvertedToSegment()
		{
			DomRegion region = new DomRegion(0, 2, 2, 1);
			document.Text = "1234567890\n1234567890\n1234567890";
			WixDocumentLineSegment segment = WixDocumentLineSegment.ConvertRegionToSegment(document, region);
			
			WixDocumentLineSegment expectedSegment = new WixDocumentLineSegment(2, 22);
			
			Assert.AreEqual(expectedSegment, segment);
		}
		
		[Test]
		public void RegionWithBeginLineOnSecondLineConvertedToSegment()
		{
			DomRegion region = new DomRegion(1, 0, 2, 0);
			document.Text = "1234567890\r\n1234567890\r\n1234567890";
			WixDocumentLineSegment segment = WixDocumentLineSegment.ConvertRegionToSegment(document, region);
			
			WixDocumentLineSegment expectedSegment = new WixDocumentLineSegment(12, 13);
			
			Assert.AreEqual(expectedSegment, segment);
		}
	}
}
