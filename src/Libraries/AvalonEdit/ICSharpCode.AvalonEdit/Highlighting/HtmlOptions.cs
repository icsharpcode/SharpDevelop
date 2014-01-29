// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Net;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// Holds options for converting text to HTML.
	/// </summary>
	public class HtmlOptions
	{
		/// <summary>
		/// Creates a default HtmlOptions instance.
		/// </summary>
		public HtmlOptions()
		{
			this.TabSize = 4;
		}
		
		/// <summary>
		/// Creates a new HtmlOptions instance that copies applicable options from the <see cref="TextEditorOptions"/>.
		/// </summary>
		public HtmlOptions(TextEditorOptions options) : this()
		{
			if (options == null)
				throw new ArgumentNullException("options");
			this.TabSize = options.IndentationSize;
		}
		
		/// <summary>
		/// The amount of spaces a tab gets converted to.
		/// </summary>
		public int TabSize { get; set; }
		
		/// <summary>
		/// Writes the HTML attribute for the style to the text writer.
		/// </summary>
		public virtual void WriteStyleAttributeForColor(TextWriter writer, HighlightingColor color)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			if (color == null)
				throw new ArgumentNullException("color");
			writer.Write(" style=\"");
			WebUtility.HtmlEncode(color.ToCss(), writer);
			writer.Write('"');
		}
		
		/// <summary>
		/// Gets whether the color needs to be written out to HTML.
		/// </summary>
		public virtual bool ColorNeedsSpanForStyling(HighlightingColor color)
		{
			if (color == null)
				throw new ArgumentNullException("color");
			return !string.IsNullOrEmpty(color.ToCss());
		}
	}
}
