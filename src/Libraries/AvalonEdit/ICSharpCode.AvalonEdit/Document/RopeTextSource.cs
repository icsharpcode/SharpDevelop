// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Editor;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// Implements the ITextSource interface using a rope.
	/// </summary>
	public sealed class RopeTextSource : ITextSource
	{
		readonly Rope<char> rope;
		
		/// <summary>
		/// Creates a new RopeTextSource.
		/// </summary>
		public RopeTextSource(Rope<char> rope)
		{
			if (rope == null)
				throw new ArgumentNullException("rope");
			this.rope = rope;
		}
		
		/// <summary>
		/// Returns a clone of the rope used for this text source.
		/// </summary>
		/// <remarks>
		/// RopeTextSource only publishes a copy of the contained rope to ensure that the underlying rope cannot be modified.
		/// Unless the creator of the RopeTextSource still has a reference on the rope, RopeTextSource is immutable.
		/// </remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification="Not a property because it creates a clone")]
		public Rope<char> GetRope()
		{
			return rope.Clone();
		}
		
		/// <inheritdoc/>
		public string Text {
			get { return rope.ToString(); }
		}
		
		/// <inheritdoc/>
		public int TextLength {
			get { return rope.Length; }
		}
		
		/// <inheritdoc/>
		public char GetCharAt(int offset)
		{
			return rope[offset];
		}
		
		/// <inheritdoc/>
		public string GetText(int offset, int length)
		{
			return rope.ToString(offset, length);
		}
		
		/// <inheritdoc/>
		public TextReader CreateReader()
		{
			return new RopeTextReader(rope);
		}
		
		/// <inheritdoc/>
		public TextReader CreateReader(int offset, int length)
		{
			return new RopeTextReader(rope.GetRange(offset, length));
		}
		
		/// <inheritdoc/>
		public ITextSource CreateSnapshot()
		{
			// we clone the underlying rope because the creator of the RopeTextSource might be modifying it
			return new RopeTextSource(rope.Clone());
		}
		
		/// <inheritdoc/>
		public ITextSource CreateSnapshot(int offset, int length)
		{
			return new RopeTextSource(rope.GetRange(offset, length));
		}
		
		/// <inheritdoc/>
		public int IndexOfAny(char[] anyOf, int startIndex, int count)
		{
			return rope.IndexOfAny(anyOf, startIndex, count);
		}
		
		ITextSourceVersion ITextSource.Version {
			get { return null; }
		}
		
		string ITextSource.GetText(ICSharpCode.Editor.ISegment segment)
		{
			throw new NotImplementedException();
		}
	}
}
