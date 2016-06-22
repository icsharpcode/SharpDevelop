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
using System.IO;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class HtmlReader
	{
		CharacterReader reader;
		HtmlNode currentNode = new HtmlNode();
		
		public HtmlReader(string html)
			: this(new StringReader(html))
		{
		}
		
		public HtmlReader(TextReader reader)
			: this(new CharacterReader(reader))
		{
		}
		
		public HtmlReader(CharacterReader reader)
		{
			this.reader = reader;
			this.Line = 1;
		}
		
		public string Value {
			get { return currentNode.Value; }
		}
		
		public int Offset { get; private set; }
		public int Length { get; private set; }
		public int Line { get; private set; }
		
		public int EndOffset {
			get { return Offset + Length; }
		}
		
		public bool IsEmptyElement { get; private set; }
		public bool IsEndElement { get; private set; }
		
		public bool IsStartElement {
			get { return !IsEndElement; }
		}
		
		public bool Read()
		{
			while (ReadNextCharacter()) {
				if (!IsHtml()) {
					// Skip character
				} else if (IsElementStartCharacter()) {
					currentNode = new HtmlNode();
					IsEndElement = reader.IsNextCharacterForwardSlash();
					IsEmptyElement = false;
					Offset = reader.CurrentCharacterOffset;
				} else if (IsElementEndCharacter()) {
					Length = reader.NextCharacterOffset - Offset;
					return true;
				} else if (reader.IsForwardSlash()) {
					IsEmptyElement = !IsEndElement;
				} else if (IsElementNameCharacter()) {
					currentNode.Append(reader.CurrentCharacter);
				} else if (reader.IsDoubleQuote()) {
					ReadDoubleQuotedString();
				} else if (reader.IsSingleQuote()) {
					ReadSingleQuotedString();
				}
			}
			return false;
		}
		
		protected virtual bool IsHtml()
		{
			return true;
		}
		
		bool ReadNextCharacter()
		{
			bool result = reader.Read();
			UpdateLineCountIfNewLineCharacterRead();
			return result;
		}
		
		void UpdateLineCountIfNewLineCharacterRead()
		{
			if (reader.IsLineFeed()) {
				Line++;
			}
		}
		
		bool IsElementStartCharacter()
		{
			return reader.IsLessThanSign();
		}
		
		bool IsElementEndCharacter()
		{
			return reader.IsGreaterThanSign();
		}
		
		bool IsElementNameCharacter()
		{
			return reader.IsLetterOrDigit() || reader.IsSpace() || reader.IsColon();
		}
		
		void ReadDoubleQuotedString()
		{
			ReadUntil(() => IsLastDoubleQuoteOfString());
		}
		
		void ReadUntil(Func<bool> match)
		{
			while (ReadNextCharacter()) {
				if (match()) {
					return;
				}
			}
		}
		
		bool IsLastDoubleQuoteOfString()
		{
			if (reader.IsDoubleQuote()) {
				return IsHtml();
			}
			return false;
		}
		
		void ReadSingleQuotedString()
		{
			ReadUntil(() => reader.IsSingleQuote());
		}
	}
}
