// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd
{
	/// <summary>
	/// A color in an Xshd file.
	/// </summary>
	public class XshdColor : XshdElement
	{
		/// <summary>
		/// Gets/sets the name.
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// Gets/sets the foreground brush.
		/// </summary>
		public HighlightingBrush Foreground { get; set; }
		
		/// <summary>
		/// Gets/sets the font weight.
		/// </summary>
		public FontWeight? FontWeight { get; set; }
		
		/// <summary>
		/// Gets/sets the font style.
		/// </summary>
		public FontStyle? FontStyle { get; set; }
		
		/// <inheritdoc/>
		public override object AcceptVisitor(IXshdVisitor visitor)
		{
			return visitor.VisitColor(this);
		}
	}
}
