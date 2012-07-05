//
// CompositeFormatStringParserTests.cs
//
// Author:
//       Simon Lindgren <simon.n.lindgren@gmail.com>
//
// Copyright (c) 2012 Simon Lindgren
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using NUnit.Framework;
using System.Linq;
using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Utils
{
	[TestFixture]
	public class CompositeFormatStringParserTests
	{

		IList<IFormatStringSegment> ParseTest(string format, params IFormatStringSegment[] expectedFormatSegments)
		{
			var parser = new CompositeFormatStringParser();
			var actualFormatSegments = parser.Parse(format).ToList();

			Console.WriteLine("Expected format segments:");
			foreach (var item in expectedFormatSegments) {
				Console.WriteLine(item.ToString());
			}
			Console.WriteLine("Actual format segments:");
			foreach (var item in actualFormatSegments) {
				Console.WriteLine(item.ToString());
			}

			Assert.AreEqual(expectedFormatSegments, actualFormatSegments);
			return actualFormatSegments;
		}

		static IList<IFormatStringError> SegmentTest(int count, IFormatStringSegment segment)
		{
			var errors = segment.Errors.ToList();
			Assert.AreEqual(count, errors.Count, "Too many or too few errors.");
			return errors;
		}

		static void ErrorTest(IFormatStringError error, string originalText, string replacementText, int startLocation, int endLocation)
		{
			Assert.AreEqual(originalText, error.OriginalText, "OriginalText is incorrect.");
			Assert.AreEqual(replacementText, error.SuggestedReplacementText, "SuggestedReplacementText is incorrect.");
			Assert.AreEqual(startLocation, error.StartLocation, "StartLocation is incorrect.");
			Assert.AreEqual(endLocation, error.EndLocation, "EndLocation is incorrect.");
		}

		[Test]
		public void Index()
		{
			ParseTest("{0}", new FormatItem(0) { StartLocation = 0, EndLocation = 3 });
		}
		
		[Test]
		public void PositiveAlignment()
		{
			ParseTest("{0,4}", new FormatItem(0, 4) { StartLocation = 0, EndLocation = 5 });
		}
		
		[Test]
		public void NegativeAlignment()
		{
			ParseTest("{0,-4}", new FormatItem(0, -4) { StartLocation = 0, EndLocation = 6 });
		}
		
		[Test]
		public void AlignmentWhiteSpace()
		{
			ParseTest("{0, -4}", new FormatItem(0, -4) { StartLocation = 0, EndLocation = 7 });
		}
		
		[Test]
		public void SubFormatString()
		{
			ParseTest("{0:aaaa}", new FormatItem(0, null, "aaaa") { StartLocation = 0, EndLocation = 8 });
		}
		
		[Test]
		public void CompleteFormatItem()
		{
			ParseTest("{0, -45:aaaa}", new FormatItem(0, -45, "aaaa") { StartLocation = 0, EndLocation = 13 });
		}
		
		[Test]
		public void MultipleCompleteFormatItems()
		{
			ParseTest("{0, -45:aaaa}{3, 67:bbbb}",
			          new FormatItem(0, -45, "aaaa") { StartLocation = 0, EndLocation = 13 },
			          new FormatItem(3, 67, "bbbb") { StartLocation = 13, EndLocation = 25 });
		}
		
		[Test]
		public void BraceEscape()
		{
			ParseTest("{{}}", new TextSegment("{}"));
		}

		[Test]
		public void TextSegment()
		{
			ParseTest("Some Text", new TextSegment("Some Text"));
		}

		[Test]
		public void SingleCharacterTextSegment()
		{
			ParseTest("A", new TextSegment("A"));
		}
		
		[Test]
		public void FormatStringWithPrefixText()
		{
			ParseTest("Some Text {0}",
			          new TextSegment("Some Text "),
			          new FormatItem(0) { StartLocation = 10, EndLocation = 13 });
		}
		
		[Test]
		public void FormatStringWithPostfixText()
		{
			ParseTest("{0} Some Text",
			          new FormatItem(0)  { StartLocation = 0, EndLocation = 3 },
					  new TextSegment(" Some Text", 3));
		}
		
		[Test]
		public void FormatStringWithEscapableBracesInSubFormatString()
		{
			ParseTest("A weird string: {0:{{}}}",
			          new TextSegment("A weird string: "),
			          new FormatItem(0, null, "{}") { StartLocation = 16, EndLocation = 24 });
		}
		
		[Test]
		public void EmptySubFormatString()
		{
			ParseTest("{0:}", new FormatItem(0, null, "") { StartLocation = 0, EndLocation = 4 });
		}
		
		[Test]
		public void EndsAfterOpenBrace()
		{
			var segments = ParseTest("{", new TextSegment("{"));
			var errors = SegmentTest(1, segments.First());
			ErrorTest(errors[0], "{", "{{", 0, 1);
		}
		
		[Test]
		public void UnescapedLoneEndingBrace()
		{
			var segments = ParseTest("Some text {", new TextSegment("Some text {"));
			var errors = SegmentTest(1, segments.First());
			ErrorTest(errors[0], "{", "{{", 10, 11);
		}
		
		[Test]
		public void EndAfterIndex()
		{
			var segments = ParseTest("Some text {0",
			                         new TextSegment("Some text "),
			                         new FormatItem(0) { StartLocation = 10, EndLocation = 12 });
			var errors = SegmentTest(1, segments.Skip(1).First());
			ErrorTest(errors[0], "", "}", 12, 12);
		}
		
		[Test]
		public void EndAfterComma()
		{
			var segments = ParseTest("Some text {0,",
			                         new TextSegment("Some text "),
			                         new FormatItem(0) { StartLocation = 10, EndLocation = 13 });
			var errors = SegmentTest(1, segments.Skip(1).First());
			ErrorTest(errors[0], ",", "}", 12, 13);
		}
		
		[Test]
		public void EndAfterCommaAndSpaces()
		{
			var segments = ParseTest("Some text {0,   ",
			                         new TextSegment("Some text "),
			                         new FormatItem(0) { StartLocation = 10, EndLocation = 16 });
			var errors = SegmentTest(1, segments.Skip(1).First());
			ErrorTest(errors[0], ",   ", "}", 12, 16);
		}
		
		[Test]
		public void EndAfterAlignment()
		{
			var segments = ParseTest("Some text {0, -34",
			                         new TextSegment("Some text "),
			                         new FormatItem(0, -34) { StartLocation = 10, EndLocation = 17 });
			var errors = SegmentTest(1, segments.Skip(1).First());
			ErrorTest(errors[0], "", "}", 17, 17);
		}
		
		[Test]
		public void EndAfterColon()
		{
			var segments = ParseTest("Some text {0:",
			                         new TextSegment("Some text "),
			                         new FormatItem(0, null, "") { StartLocation = 10, EndLocation = 13 });
			var errors = SegmentTest(1, segments.Skip(1).First());
			ErrorTest(errors[0], "", "}", 13, 13);
		}
		
		[Test]
		public void EndAfterSubFormatString()
		{
			var segments = ParseTest("Some text {0: asdf",
			                         new TextSegment("Some text "),
			                         new FormatItem(0, null, " asdf") { StartLocation = 10, EndLocation = 18 });
			var errors = SegmentTest(1, segments.Skip(1).First());
			ErrorTest(errors[0], "", "}", 18, 18);
		}
		
		[Test]
		public void MissingIndex()
		{
			var segments = ParseTest("Some text {}",
			                         new TextSegment("Some text "),
			                         new FormatItem(0) { StartLocation = 10, EndLocation = 12 });
			var errors = SegmentTest(1, segments.Skip(1).First());
			ErrorTest(errors[0], "", "0", 11, 11);
		}
		
		[Test]
		public void MissingAlignment()
		{
			var segments = ParseTest("Some text {0,}",
			                         new TextSegment("Some text "),
			                         new FormatItem(0, 0) { StartLocation = 10, EndLocation = 14 });
			var errors = SegmentTest(1, segments.Skip(1).First());
			ErrorTest(errors[0], "", "0", 13, 13);
		}
		
		[Test]
		public void MissingEveryThing()
		{
			var segments = ParseTest("{,:", new FormatItem(0, 0, "") { StartLocation = 0, EndLocation = 3 });
			var errors = SegmentTest(3, segments.First());
			ErrorTest(errors[0], "", "0", 1, 1);
			ErrorTest(errors[1], "", "0", 2, 2);
			ErrorTest(errors[2], "", "}", 3, 3);
		}
	}
}

