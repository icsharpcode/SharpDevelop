// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.WixBinding;
using NUnit.Framework;

namespace WixBinding.Tests.Document
{
	[TestFixture]
	public class WixDocumentLineSegmentEqualsTests
	{
		[Test]
		public void SegmentsWithSameOffsetAndLengthAreEqual()
		{
			WixDocumentLineSegment lhs = new WixDocumentLineSegment(4, 5);
			WixDocumentLineSegment rhs = new WixDocumentLineSegment(4, 5);
			Assert.IsTrue(lhs.Equals(rhs));
		}
		
		[Test]
		public void SegmentsWithSameOffsetAndDifferentLengthAreNotEqual()
		{
			WixDocumentLineSegment lhs = new WixDocumentLineSegment(4, 10);
			WixDocumentLineSegment rhs = new WixDocumentLineSegment(4, 5);
			Assert.IsFalse(lhs.Equals(rhs));
		}
		
		[Test]
		public void SegmentsWithSameLengthAndDifferentOffsetAreNotEqual()
		{
			WixDocumentLineSegment lhs = new WixDocumentLineSegment(3, 5);
			WixDocumentLineSegment rhs = new WixDocumentLineSegment(4, 5);
			Assert.IsFalse(lhs.Equals(rhs));
		}
		
		[Test]
		public void NullSegmentIsNotEqualToSegment()
		{
			WixDocumentLineSegment lhs = new WixDocumentLineSegment(1, 4);
			Assert.IsFalse(lhs.Equals(null));
		}
	}
}
