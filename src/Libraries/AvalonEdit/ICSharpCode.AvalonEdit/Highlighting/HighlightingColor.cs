// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// A highlighting color is a set of font properties and foreground and background color.
	/// </summary>
	public class HighlightingColor
	{
		/// <summary>
		/// Gets/Sets the name of the color.
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// Gets/sets the font weight. Null if the highlighting color does not change the font weight.
		/// </summary>
		public FontWeight? FontWeight { get; set; }
		
		/// <summary>
		/// Gets/sets the font style. Null if the highlighting color does not change the font style.
		/// </summary>
		public FontStyle? FontStyle { get; set; }
		
		/// <summary>
		/// Gets/sets the foreground color applied by the highlighting.
		/// </summary>
		public HighlightingBrush Foreground { get; set; }
		
		/// <summary>
		/// Gets CSS code for the color.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "CSS usually uses lowercase, and all possible values are English-only")]
		public virtual string ToCss()
		{
			StringBuilder b = new StringBuilder();
			if (Foreground != null) {
				Color? c = Foreground.GetColor(null);
				if (c != null) {
					b.AppendFormat(CultureInfo.InvariantCulture, "color: #{0:x2}{1:x2}{2:x2}; ", c.Value.R, c.Value.G, c.Value.B);
				}
			}
			if (FontWeight != null) {
				b.Append("font-weight: ");
				b.Append(FontWeight.Value.ToString().ToLowerInvariant());
				b.Append("; ");
			}
			if (FontStyle != null) {
				b.Append("font-style: ");
				b.Append(FontStyle.Value.ToString().ToLowerInvariant());
				b.Append("; ");
			}
			return b.ToString();
		}
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return "[" + GetType() + " " + (string.IsNullOrEmpty(this.Name) ? ToCss() : this.Name) + "]";
		}
	}
}
