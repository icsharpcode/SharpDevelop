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
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.NRefactory.Xml
{
	/// <summary>
	/// XML tag soup parser that .
	/// </summary>
	public class TagSoupParser
	{
		/// <summary>
		/// Generate syntax error when seeing entity reference other then the built-in ones
		/// </summary>
		public bool UnknownEntityReferenceIsError { get; set; }
		
		/// <summary>
		/// Parses a document.
		/// </summary>
		/// <returns>Parsed tag soup.</returns>
		public IList<AXmlObject> Parse(ITextSource textSource)
		{
			if (textSource == null)
				throw new ArgumentNullException("textSource");
			var reader = new TagReader(this, textSource);
			var internalObjects = reader.ReadAllObjects();
			var publicObjects = new AXmlObject[internalObjects.Length];
			int pos = 0;
			for (int i = 0; i < internalObjects.Length; i++) {
				publicObjects[i] = internalObjects[i].CreatePublicObject(null, pos);
				pos += internalObjects[i].Length;
			}
			return Array.AsReadOnly(publicObjects);
		}
		
		/// <summary>
		/// Parses a document incrementally.
		/// </summary>
		/// <param name="oldParserState">The parser state from a previous call to ParseIncremental(). Use null for the first call.</param>
		/// <param name="newTextSource">The text source for the new document version.</param>
		/// <param name="newParserState">Out: the new parser state, pass this to the next ParseIncremental() call.</param>
		/// <returns>Parsed tag soup.</returns>
		public IList<AXmlObject> ParseIncremental(IncrementalParserState oldParserState, ITextSource newTextSource, out IncrementalParserState newParserState)
		{
			if (newTextSource == null)
				throw new ArgumentNullException("newTextSource");
			// TODO: incremental parser
			newParserState = null;
			return Parse(newTextSource);
		}
	}
}
