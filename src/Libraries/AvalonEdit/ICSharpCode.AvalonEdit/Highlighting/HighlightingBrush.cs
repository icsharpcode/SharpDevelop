// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Gui;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// A brush used for syntax highlighting. Can retrieve a real brush on-demand.
	/// </summary>
	public abstract class HighlightingBrush
	{
		/// <summary>
		/// Gets the real brush.
		/// </summary>
		public abstract Brush GetBrush(ITextRunConstructionContext context);
	}
	
	/// <summary>
	/// Highlighting brush implementation that takes a frozen brush.
	/// </summary>
	sealed class SimpleHighlightingBrush : HighlightingBrush
	{
		readonly Brush brush;
		
		public SimpleHighlightingBrush(Brush brush)
		{
			brush.Freeze();
			this.brush = brush;
		}
		
		public SimpleHighlightingBrush(Color color) : this(new SolidColorBrush(color)) {}
		
		public override Brush GetBrush(ITextRunConstructionContext context)
		{
			return brush;
		}
		
		public override string ToString()
		{
			SolidColorBrush scb = brush as SolidColorBrush;
			if (scb != null) {
				return scb.Color.ToString();
			} else {
				return brush.ToString();
			}
		}
	}
	
	/// <summary>
	/// HighlightingBrush implementation that finds a brush using a resource.
	/// </summary>
	sealed class ResourceKeyHighlightingBrush : HighlightingBrush
	{
		readonly ResourceKey resourceKey;
		readonly string name;
		
		public ResourceKeyHighlightingBrush(ResourceKey resourceKey, string name)
		{
			this.resourceKey = resourceKey;
			this.name = name;
		}
		
		public override Brush GetBrush(ITextRunConstructionContext context)
		{
			return (Brush)context.TextView.FindResource(resourceKey);
		}
		
		public override string ToString()
		{
			return name;
		}
	}
}
