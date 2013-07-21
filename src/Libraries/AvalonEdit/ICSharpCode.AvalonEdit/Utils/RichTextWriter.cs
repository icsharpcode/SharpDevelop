// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.AvalonEdit.Utils
{
	/// <summary>
	/// A text writer that supports creating spans of highlighted text.
	/// </summary>
	public abstract class RichTextWriter : TextWriter
	{
		/// <summary>
		/// Gets called by the RichTextWriter base class when a BeginSpan() method
		/// that is not overwritten gets called.
		/// </summary>
		protected abstract void BeginUnhandledSpan();
		
		/// <summary>
		/// Writes the RichText instance.
		/// </summary>
		public void Write(RichText richText)
		{
			Write(richText, 0, richText.Length);
		}
		
		/// <summary>
		/// Writes the RichText instance.
		/// </summary>
		public virtual void Write(RichText richText, int offset, int length)
		{
			foreach (var section in richText.GetHighlightedSections(offset, length)) {
				BeginSpan(section.Color);
				Write(richText.Text.Substring(section.Offset, section.Length));
				EndSpan();
			}
		}
		
		/// <summary>
		/// Begin a colored span.
		/// </summary>
		public virtual void BeginSpan(Color foregroundColor)
		{
			BeginUnhandledSpan();
		}
		
		/// <summary>
		/// Begin a span with modified font weight.
		/// </summary>
		public virtual void BeginSpan(FontWeight fontWeight)
		{
			BeginUnhandledSpan();
		}
		
		/// <summary>
		/// Begin a span with modified font style.
		/// </summary>
		public virtual void BeginSpan(FontStyle fontStyle)
		{
			BeginUnhandledSpan();
		}
		
		/// <summary>
		/// Begin a span with modified font family.
		/// </summary>
		public virtual void BeginSpan(FontFamily fontFamily)
		{
			BeginUnhandledSpan();
		}
		
		/// <summary>
		/// Begin a highlighted span.
		/// </summary>
		public virtual void BeginSpan(Highlighting.HighlightingColor highlightingColor)
		{
			BeginUnhandledSpan();
		}
		
		/// <summary>
		/// Begin a span that links to the specified URI.
		/// </summary>
		public virtual void BeginHyperlinkSpan(Uri uri)
		{
			BeginUnhandledSpan();
		}
		
		/// <summary>
		/// Marks the end of the current span.
		/// </summary>
		public abstract void EndSpan();
		
		/// <summary>
		/// Increases the indentation level.
		/// </summary>
		public abstract void Indent();
		
		/// <summary>
		/// Decreases the indentation level.
		/// </summary>
		public abstract void Unindent();
	}
}
