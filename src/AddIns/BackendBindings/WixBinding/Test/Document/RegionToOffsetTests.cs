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
