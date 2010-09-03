// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.AvalonEdit.Document;
using System;

namespace ICSharpCode.SharpDevelop.Editor.AvalonEdit
{
	public class AvalonEditTextSourceAdapter : ITextBuffer
	{
		internal readonly ITextSource textSource;
		
		public AvalonEditTextSourceAdapter(ITextSource textSource)
		{
			if (textSource == null)
				throw new ArgumentNullException("textSource");
			this.textSource = textSource;
		}
		
		public virtual ITextBufferVersion Version {
			get { return null; }
		}
		
		/// <summary>
		/// Creates an immutable snapshot of this text buffer.
		/// </summary>
		public virtual ITextBuffer CreateSnapshot()
		{
			return new AvalonEditTextSourceAdapter(textSource.CreateSnapshot());
		}
		
		/// <summary>
		/// Creates an immutable snapshot of a part of this text buffer.
		/// Unlike all other methods in this interface, this method is thread-safe.
		/// </summary>
		public ITextBuffer CreateSnapshot(int offset, int length)
		{
			return new AvalonEditTextSourceAdapter(textSource.CreateSnapshot(offset, length));
		}
		
		/// <summary>
		/// Creates a new TextReader to read from this text buffer.
		/// </summary>
		public System.IO.TextReader CreateReader()
		{
			return textSource.CreateReader();
		}
		
		/// <summary>
		/// Creates a new TextReader to read from this text buffer.
		/// </summary>
		public System.IO.TextReader CreateReader(int offset, int length)
		{
			return textSource.CreateSnapshot(offset, length).CreateReader();
		}
		
		public int TextLength {
			get { return textSource.TextLength; }
		}
		
		public string Text {
			get { return textSource.Text; }
		}
		
		/// <summary>
		/// Is raised when the Text property changes.
		/// </summary>
		public event EventHandler TextChanged {
			add { textSource.TextChanged += value; }
			remove { textSource.TextChanged -= value; }
		}
		
		public char GetCharAt(int offset)
		{
			return textSource.GetCharAt(offset);
		}
		
		public string GetText(int offset, int length)
		{
			return textSource.GetText(offset, length);
		}
	}
}
