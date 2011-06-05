// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT license (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.Editor
{
	/// <summary>
	/// Implements the ITextBuffer interface using a string.
	/// </summary>
	[Serializable]
	public class StringTextBuffer : ITextBuffer
	{
		readonly string text;
		
		/// <summary>
		/// Creates a new StringTextBuffer with the given text.
		/// </summary>
		public StringTextBuffer(string text)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			this.text = text;
		}
		
		event EventHandler ITextBuffer.TextChanged { add {} remove {} }
		
		ITextBufferVersion ITextBuffer.Version {
			get { return null; }
		}
		
		/// <inheritdoc/>
		public int TextLength {
			get { return text.Length; }
		}
		
		/// <inheritdoc/>
		public string Text {
			get { return text; }
		}
		
		/// <inheritdoc/>
		public ITextBuffer CreateSnapshot()
		{
			return this; // StringTextBuffer is immutable
		}
		
		/// <inheritdoc/>
		public ITextBuffer CreateSnapshot(int offset, int length)
		{
			return new StringTextBuffer(text.Substring(offset, length));
		}
		
		/// <inheritdoc/>
		public TextReader CreateReader()
		{
			return new StringReader(text);
		}
		
		/// <inheritdoc/>
		public TextReader CreateReader(int offset, int length)
		{
			return new StringReader(text.Substring(offset, length));
		}
		
		/// <inheritdoc/>
		public char GetCharAt(int offset)
		{
			return text[offset];
		}
		
		/// <inheritdoc/>
		public string GetText(int offset, int length)
		{
			return text.Substring(offset, length);
		}
		
		/// <inheritdoc/>
		public int IndexOfAny(char[] anyOf, int startIndex, int count)
		{
			return text.IndexOfAny(anyOf, startIndex, count);
		}
	}
}
