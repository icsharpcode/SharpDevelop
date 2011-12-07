// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class SimpleWebFormsHtmlReader
	{
		CharacterReader reader;
		HtmlNode currentNode = new HtmlNode();
		
		public SimpleWebFormsHtmlReader(string html)
			: this(new StringReader(html))
		{
		}
		
		public SimpleWebFormsHtmlReader(TextReader reader)
			: this(new CharacterReader(reader))
		{
		}
		
		public SimpleWebFormsHtmlReader(CharacterReader reader)
		{
			this.reader = reader;
		}
		
		public string Value {
			get { return currentNode.Value; }
		}
		
		public int Offset { get; private set; }
		public int Length { get; private set; }
		
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
				if (IsElementStartCharacter()) {
					currentNode = new HtmlNode();
					IsEndElement = reader.IsNextCharacterForwardSlash();
					IsEmptyElement = false;
					Offset = reader.PreviousOffset;
				} else if (IsElementEndCharacter()) {
					Length = reader.Offset - Offset;
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
		
		bool ReadNextCharacter()
		{
			return reader.Read();
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
			return reader.IsLetterOrDigit() || reader.IsSpace();
		}
		
		void ReadDoubleQuotedString()
		{
			ReadUntil(() => reader.IsDoubleQuote());
		}
		
		void ReadUntil(Func<bool> match)
		{
			while (ReadNextCharacter()) {
				if (match()) {
					return;
				}
			}
		}
		
		void ReadSingleQuotedString()
		{
			ReadUntil(() => reader.IsSingleQuote());
		}
	}
}
