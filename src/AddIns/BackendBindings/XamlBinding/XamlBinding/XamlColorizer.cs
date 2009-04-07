// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Gui;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XamlBinding
{
	public class XamlColorizer : ColorizingTransformer
	{
		static readonly XamlColorizerSettings defaultSettings = new XamlColorizerSettings();
		XamlColorizerSettings settings = defaultSettings;
		static readonly char[] punctuationItems = {'"' }; // , '<', '>', '.', '=', '{', '}'
		
		public AvalonEditViewContent Content { get; set; }
		
		public XamlColorizer(AvalonEditViewContent content)
		{
			this.Content = content;
		}
		
		protected override void Colorize(ITextRunConstructionContext context)
		{
			DocumentLine line = context.VisualLine.FirstDocumentLine;
			
			while (line != null) {
				if (!line.IsDeleted) {
					int index = 0;
					do {
						index = line.Text.IndexOfAny(punctuationItems, index);
						if (index > -1) {
							int col = context.VisualLine.GetVisualColumn(index);
							ChangeVisualElements(index, index + 1, ColorizePunctuation);
							index++;
						}
					} while (index > -1);
				}
				
				line = line.NextLine;
			}
		}
		
		void ColorizePunctuation(VisualLineElement element)
		{
			element.TextRunProperties.SetForegroundBrush(settings.PunctuationForegroundBrush);
			element.TextRunProperties.SetBackgroundBrush(settings.PunctuationBackgroundBrush);
		}
	}
	
	public class XamlColorizerSettings
	{
		public Brush PunctuationForegroundBrush { get; set; }
		public Brush PunctuationBackgroundBrush { get; set; }
		
		public XamlColorizerSettings()
		{
			this.PunctuationBackgroundBrush = Brushes.Transparent;
			this.PunctuationForegroundBrush = Brushes.Red;
		}
	}
}
