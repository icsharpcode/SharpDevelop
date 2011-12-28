// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
