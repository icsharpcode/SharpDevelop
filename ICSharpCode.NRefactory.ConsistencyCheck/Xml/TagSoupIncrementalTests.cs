// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.ConsistencyCheck.Xml;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Xml;

namespace ICSharpCode.NRefactory.ConsistencyCheck
{
	/// <summary>
	/// Tests incremental tag soup parser.
	/// </summary>
	public class TagSoupIncrementalTests
	{
		static Random sharedRnd = new Random();
		
		public static void Run(string fileName)
		{
			Run(new StringTextSource(File.ReadAllText(fileName)));
		}
		
		public static void Run(ITextSource originalXmlFile)
		{
			int seed;
			lock (sharedRnd) {
				seed = sharedRnd.Next();
			}
			Random rnd = new Random(seed);
			
			TagSoupParser parser = new TagSoupParser();
			StringBuilder b = new StringBuilder(originalXmlFile.Text);
			IncrementalParserState parserState = null;
			TestTextSourceVersion version = new TestTextSourceVersion();
			for (int iteration = 0; iteration < 100; iteration++) {
				var textSource = new TextSourceWithVersion(new StringTextSource(b.ToString()), version);
				var incrementalResult = parser.ParseIncremental(parserState, textSource, out parserState);
				var nonIncrementalResult = parser.Parse(textSource);
				CompareResults(incrementalResult, nonIncrementalResult);
				// Randomly mutate the file:
				
				List<TextChangeEventArgs> changes = new List<TextChangeEventArgs>();
				int modifications = rnd.Next(0, 10);
				for (int i = 0; i < modifications; i++) {
					int offset = rnd.Next(0, b.Length);
					int originalOffset = rnd.Next(0, originalXmlFile.TextLength);
					int insertionLength;
					int removalLength;
					switch (rnd.Next(0, 21) / 20) {
						case 0:
							removalLength = 0;
							insertionLength = rnd.Next(0, Math.Min(50, originalXmlFile.TextLength - originalOffset));
							break;
						case 1:
							removalLength = rnd.Next(0, Math.Min(10, b.Length - offset));
							insertionLength = rnd.Next(0, Math.Min(20, originalXmlFile.TextLength - originalOffset));
							break;
						default:
							removalLength = rnd.Next(0, b.Length - offset);
							insertionLength = rnd.Next(0, originalXmlFile.TextLength - originalOffset);
							break;
					}
					string removedText = b.ToString(offset, removalLength);
					b.Remove(offset, removalLength);
					string insertedText = originalXmlFile.GetText(originalOffset, insertionLength);
					b.Insert(offset, insertedText);
					changes.Add(new TextChangeEventArgs(offset, removedText, insertedText));
				}
				version = new TestTextSourceVersion(version, changes);
			}
		}
		
		static void CompareResults(IList<AXmlObject> result1, IList<AXmlObject> result2)
		{
			if (result1.Count != result2.Count)
				throw new InvalidOperationException();
			for (int i = 0; i < result1.Count; i++) {
				CompareResults(result1[i], result2[i]);
			}
		}
		
		static void CompareResults(AXmlObject obj1, AXmlObject obj2)
		{
			if (obj1.GetType() != obj2.GetType())
				throw new InvalidOperationException();
			if (obj1.StartOffset != obj2.StartOffset)
				throw new InvalidOperationException();
			if (obj1.EndOffset != obj2.EndOffset)
				throw new InvalidOperationException();
			
			if (obj1.MySyntaxErrors.Count() != obj2.MySyntaxErrors.Count())
				throw new InvalidOperationException();
			foreach (var pair in obj1.MySyntaxErrors.Zip(obj2.MySyntaxErrors, (a,b) => new { a, b })) {
				if (pair.a.StartOffset != pair.b.StartOffset)
					throw new InvalidOperationException();
				if (pair.a.EndOffset != pair.b.EndOffset)
					throw new InvalidOperationException();
				if (pair.a.Description != pair.b.Description)
					throw new InvalidOperationException();
			}
			
			if (obj1 is AXmlText) {
				var a = (AXmlText)obj1;
				var b = (AXmlText)obj2;
				if (a.ContainsOnlyWhitespace != b.ContainsOnlyWhitespace)
					throw new InvalidOperationException();
				if (a.Value != b.Value)
					throw new InvalidOperationException();
			} else if (obj1 is AXmlTag) {
				var a = (AXmlTag)obj1;
				var b = (AXmlTag)obj2;
				if (a.OpeningBracket != b.OpeningBracket)
					throw new InvalidOperationException();
				if (a.ClosingBracket != b.ClosingBracket)
					throw new InvalidOperationException();
				if (a.Name != b.Name)
					throw new InvalidOperationException();
			} else if (obj1 is AXmlAttribute) {
				var a = (AXmlAttribute)obj1;
				var b = (AXmlAttribute)obj2;
				if (a.Name != b.Name)
					throw new InvalidOperationException();
				if (a.Value != b.Value)
					throw new InvalidOperationException();
			} else {
				throw new NotSupportedException();
			}
			
			CompareResults(obj1.Children, obj2.Children);
		}
	}
}
