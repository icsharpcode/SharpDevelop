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
		/// <param name="context">The construction context. context can be null!</param>
		public abstract Brush GetBrush(ITextRunConstructionContext context);
		
		/// <summary>
		/// Gets the color of the brush.
		/// </summary>
		/// <param name="context">The construction context. context can be null!</param>
		public virtual Color? GetColor(ITextRunConstructionContext context)
		{
			SolidColorBrush scb = GetBrush(context) as SolidColorBrush;
			if (scb != null)
				return scb.Color;
			else
				return null;
		}
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
			return brush.ToString();
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
			if (context != null && context.TextView != null)
				return (Brush)context.TextView.FindResource(resourceKey);
			else
				return (Brush)Application.Current.FindResource(resourceKey);
		}
		
		public override string ToString()
		{
			return name;
		}
	}
}
