// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Document.Tests
{
	[TestFixture]
	public class TextSegmentTreeTest
	{
		class TestTextSegment : TextSegment
		{
			internal int ExpectedOffset, ExpectedLength;
			
			public TestTextSegment(int expectedOffset, int expectedLength)
			{
				this.ExpectedOffset = expectedOffset;
				this.ExpectedLength = expectedLength;
				this.StartOffset = expectedOffset;
				this.Length = expectedLength;
			}
		}
		
		static readonly string documentText = new string(' ', 1000);
		
		TextDocument document;
		TextSegmentCollection<TestTextSegment> tree;
		List<TestTextSegment> expectedSegments;
		
		[SetUp]
		public void SetUp()
		{
			document = new TextDocument();
			document.Text = documentText;
			tree = new TextSegmentCollection<TestTextSegment>(document);
			expectedSegments = new List<TestTextSegment>();
		}
		
		[Test]
		public void FindInEmptyTree()
		{
			Assert.AreSame(null, tree.FindFirstSegmentWithStartAfter(0));
			Assert.AreEqual(0, tree.FindSegmentsContaining(0).Count);
			Assert.AreEqual(0, tree.FindOverlappingSegments(10, 20).Count);
		}
		
		[Test]
		public void FindFirstSegmentWithStartAfter()
		{
			var s1 = new TestTextSegment(5, 10);
			var s2 = new TestTextSegment(10, 10);
			tree.Add(s1);
			tree.Add(s2);
			Assert.AreSame(s1, tree.FindFirstSegmentWithStartAfter(-100));
			Assert.AreSame(s1, tree.FindFirstSegmentWithStartAfter(0));
			Assert.AreSame(s1, tree.FindFirstSegmentWithStartAfter(4));
			Assert.AreSame(s1, tree.FindFirstSegmentWithStartAfter(5));
			Assert.AreSame(s2, tree.FindFirstSegmentWithStartAfter(6));
			Assert.AreSame(s2, tree.FindFirstSegmentWithStartAfter(9));
			Assert.AreSame(s2, tree.FindFirstSegmentWithStartAfter(10));
			Assert.AreSame(null, tree.FindFirstSegmentWithStartAfter(11));
			Assert.AreSame(null, tree.FindFirstSegmentWithStartAfter(100));
		}
		
		TestTextSegment AddSegment(int offset, int length)
		{
//			Console.WriteLine("Add " + offset + ", " + length);
			TestTextSegment s = new TestTextSegment(offset, length);
			tree.Add(s);
			expectedSegments.Add(s);
			return s;
		}
		
		void RemoveSegment(TestTextSegment s)
		{
//			Console.WriteLine("Remove " + s);
			expectedSegments.Remove(s);
			tree.Remove(s);
		}
		
		void TestRetrieval(int offset, int length)
		{
			HashSet<TestTextSegment> actual = new HashSet<TestTextSegment>(tree.FindOverlappingSegments(offset, length));
			HashSet<TestTextSegment> expected = new HashSet<TestTextSegment>();
			foreach (TestTextSegment e in expectedSegments) {
				if (e.ExpectedOffset + e.ExpectedLength < offset)
					continue;
				if (e.ExpectedOffset > offset + length)
					continue;
				expected.Add(e);
			}
			Assert.IsTrue(actual.IsSubsetOf(expected));
			Assert.IsTrue(expected.IsSubsetOf(actual));
		}
		
		void CheckSegments()
		{
			Assert.AreEqual(expectedSegments.Count, tree.Count);
			foreach (TestTextSegment s in expectedSegments) {
				Assert.AreEqual(s.ExpectedOffset, s.StartOffset);
				Assert.AreEqual(s.ExpectedLength, s.Length);
			}
		}
		
		[Test]
		public void AddSegments()
		{
			TestTextSegment s1 = AddSegment(10, 20);
			TestTextSegment s2 = AddSegment(15, 10);
			CheckSegments();
		}
		
		Random rnd;
		
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			int seed = Environment.TickCount;
			Console.WriteLine("TextSegmentTreeTest Seed: " + seed);
			rnd = new Random(seed);
		}
		
		[Test]
		public void RandomizedNoDocumentChanges()
		{
			for (int i = 0; i < 1000; i++) {
//				Console.WriteLine(tree.GetTreeAsString());
//				Console.WriteLine("Iteration " + i);
				
				switch (rnd.Next(3)) {
					case 0:
						AddSegment(rnd.Next(500), rnd.Next(30));
						break;
					case 1:
						AddSegment(rnd.Next(500), rnd.Next(300));
						break;
					case 2:
						if (tree.Count > 0) {
							RemoveSegment(expectedSegments[rnd.Next(tree.Count)]);
						}
						break;
				}
				CheckSegments();
			}
		}
		
		[Test]
		public void RandomizedClose()
		{
			// Lots of segments in a short document. Tests how the tree copes with multiple identical segments.
			for (int i = 0; i < 1000; i++) {
				switch (rnd.Next(3)) {
					case 0:
						AddSegment(rnd.Next(20), rnd.Next(10));
						break;
					case 1:
						AddSegment(rnd.Next(20), rnd.Next(20));
						break;
					case 2:
						if (tree.Count > 0) {
							RemoveSegment(expectedSegments[rnd.Next(tree.Count)]);
						}
						break;
				}
				CheckSegments();
			}
		}
		
		[Test]
		public void RandomizedRetrievalTest()
		{
			for (int i = 0; i < 1000; i++) {
				AddSegment(rnd.Next(500), rnd.Next(300));
			}
			CheckSegments();
			for (int i = 0; i < 1000; i++) {
				TestRetrieval(rnd.Next(1000) - 100, rnd.Next(500));
			}
		}
		
		// TODO: insertion/removal tests
	}
}
