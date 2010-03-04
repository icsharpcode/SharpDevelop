// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Applies a set of customizations to syntax highlighting.
	/// </summary>
	public class CustomizableHighlightingColorizer : HighlightingColorizer
	{
		readonly IEnumerable<CustomizedHighlightingColor> customizations;
		
		public CustomizableHighlightingColorizer(HighlightingRuleSet ruleSet, IEnumerable<CustomizedHighlightingColor> customizations)
			: base(ruleSet)
		{
			if (customizations == null)
				throw new ArgumentNullException("customizations");
			this.customizations = customizations;
		}
		
		protected override IHighlighter CreateHighlighter(TextView textView, TextDocument document)
		{
			return new CustomizingHighlighter(customizations, base.CreateHighlighter(textView, document));
		}
		
		sealed class CustomizingHighlighter : IHighlighter
		{
			readonly IEnumerable<CustomizedHighlightingColor> customizations;
			readonly IHighlighter baseHighlighter;
			
			public CustomizingHighlighter(IEnumerable<CustomizedHighlightingColor> customizations, IHighlighter baseHighlighter)
			{
				Debug.Assert(customizations != null);
				this.customizations = customizations;
				this.baseHighlighter = baseHighlighter;
			}

			public TextDocument Document {
				get { return baseHighlighter.Document; }
			}
			
			public ICSharpCode.AvalonEdit.Utils.ImmutableStack<HighlightingSpan> GetSpanStack(int lineNumber)
			{
				return baseHighlighter.GetSpanStack(lineNumber);
			}
			
			public HighlightedLine HighlightLine(int lineNumber)
			{
				HighlightedLine line = baseHighlighter.HighlightLine(lineNumber);
				foreach (HighlightedSection section in line.Sections) {
					section.Color = CustomizeColor(section.Color);
				}
				return line;
			}
			
			HighlightingColor CustomizeColor(HighlightingColor color)
			{
				if (color == null || color.Name == null)
					return color;
				foreach (CustomizedHighlightingColor customization in customizations) {
					if (customization.Name == color.Name) {
						return new HighlightingColor {
							Name = color.Name,
							Background = CreateBrush(customization.Background),
							Foreground = CreateBrush(customization.Foreground),
							FontWeight = customization.Bold ? FontWeights.Bold : FontWeights.Normal,
							FontStyle = customization.Italic ? FontStyles.Italic : FontStyles.Normal
						};
					}
				}
				return color;
			}
			
			static HighlightingBrush CreateBrush(Color? color)
			{
				if (color == null)
					return null;
				else
					return new CustomizedBrush(color.Value);
			}
		}
		
		sealed class CustomizedBrush : HighlightingBrush
		{
			readonly SolidColorBrush brush;
			
			public CustomizedBrush(Color color)
			{
				brush = new SolidColorBrush(color);
				brush.Freeze();
			}
			
			public override Brush GetBrush(ITextRunConstructionContext context)
			{
				return brush;
			}
			
			public override string ToString()
			{
				return brush.ToString();
			}
		}
	}
}
