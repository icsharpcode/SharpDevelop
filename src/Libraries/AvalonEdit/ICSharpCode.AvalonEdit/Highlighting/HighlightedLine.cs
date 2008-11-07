// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// Represents a highlighted document line.
	/// </summary>
	public class HighlightedLine
	{
		/// <summary>
		/// Creates a new HighlightedLine instance.
		/// </summary>
		public HighlightedLine()
		{
			this.Sections = new List<HighlightedSection>();
		}
		
		/// <summary>
		/// Gets/Sets the document line associated with this HighlightedLine.
		/// </summary>
		public DocumentLine DocumentLine { get; set; }
		
		/// <summary>
		/// Gets the highlighted sections.
		/// The sections are not overlapping, but they may be nested.
		/// In that case, outer sections come in the list before inner sections.
		/// The sections are sorted by start offset.
		/// </summary>
		public IList<HighlightedSection> Sections { get; private set; }
		
		
		sealed class HtmlElement : IComparable<HtmlElement>
		{
			internal readonly int Offset;
			internal readonly int Nesting;
			internal readonly bool IsEnd;
			internal readonly HighlightingColor Color;
			
			public HtmlElement(int offset, int nesting, bool isEnd, HighlightingColor color)
			{
				this.Offset = offset;
				this.Nesting = nesting;
				this.IsEnd = isEnd;
				this.Color = color;
			}
			
			public int CompareTo(HtmlElement other)
			{
				int r = Offset.CompareTo(other.Offset);
				if (r != 0)
					return r;
				if (IsEnd != other.IsEnd) {
					if (IsEnd)
						return -1;
					else
						return 1;
				} else {
					if (IsEnd)
						return -Nesting.CompareTo(other.Nesting);
					else
						return Nesting.CompareTo(other.Nesting);
				}
			}
		}
		
		/// <summary>
		/// Produces HTML code for the line, with &lt;span class="colorName"&gt; tags.
		/// </summary>
		public string ToHtml()
		{
			List<HtmlElement> elements = new List<HtmlElement>();
			for (int i = 0; i < this.Sections.Count; i++) {
				HighlightedSection s = this.Sections[i];
				elements.Add(new HtmlElement(s.Offset, i, false, s.Color));
				elements.Add(new HtmlElement(s.Offset + s.Length, i, true, s.Color));
			}
			elements.Sort();
			
			TextDocument document = DocumentLine.Document;
			StringBuilder b = new StringBuilder();
			int textOffset = DocumentLine.Offset;
			foreach (HtmlElement e in elements) {
				b.Append(document.GetText(textOffset, e.Offset - textOffset));
				textOffset = e.Offset;
				if (e.IsEnd) {
					b.Append("</span>");
				} else {
					b.Append("<span style=\"");
					b.Append(e.Color.ToCss());
					b.Append("\">");
				}
			}
			b.Append(document.GetText(textOffset, DocumentLine.Offset + DocumentLine.Length - textOffset));
			return b.ToString();
		}
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return "[" + GetType().Name + " " + ToHtml() + "]";
		}
	}
}
