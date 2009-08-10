// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Search;

namespace SearchAndReplace
{
	/// <summary>
	/// Represents a search result.
	/// </summary>
	sealed class SearchResultNode : SearchNode
	{
		static readonly FontFamily resultLineFamily = new FontFamily("Consolas, Courier New");
		
		SearchResultMatch result;
		int lineNumber, column;
		HighlightedInlineBuilder inlineBuilder;
		
		public SearchResultNode(SearchResultMatch result)
		{
			this.result = result;
			
			IDocument document = result.CreateDocument();
			var startPosition = result.GetStartPosition(document);
			this.lineNumber = startPosition.Line;
			this.column = startPosition.Column;
			
			if (lineNumber >= 1 && lineNumber <= document.TotalNumberOfLines) {
				IDocumentLine matchedLine = document.GetLine(lineNumber);
				inlineBuilder = new HighlightedInlineBuilder(matchedLine.Text);
				inlineBuilder.SetFontFamily(0, inlineBuilder.Text.Length, resultLineFamily);
				
				DocumentHighlighter highlighter = document.GetService(typeof(DocumentHighlighter)) as DocumentHighlighter;
				TextDocument textDocument = document.GetService(typeof(TextDocument)) as TextDocument;
				if (highlighter != null && textDocument != null) {
					HighlightedLine highlightedLine = highlighter.HighlightLine(textDocument.GetLineByNumber(lineNumber));
					int startOffset = highlightedLine.DocumentLine.Offset;
					// copy only the foreground color
					foreach (HighlightedSection section in highlightedLine.Sections) {
						if (section.Color.Foreground != null) {
							inlineBuilder.SetForeground(section.Offset - startOffset, section.Length, section.Color.Foreground.GetBrush(null));
						}
					}
				}
				
				// now highlight the match in bold
				if (column >= 1) {
					var endPosition = result.GetEndPosition(document);
					if (endPosition.Line == startPosition.Line && endPosition.Column > startPosition.Column) {
						// subtract one from the column to get the offset inside the line's text
						int startOffset = column - 1;
						int endOffset = Math.Min(inlineBuilder.Text.Length, endPosition.Column - 1);
						inlineBuilder.SetFontWeight(startOffset, endOffset - startOffset, FontWeights.Bold);
					}
				}
			}
		}
		
		bool showFileName = true;
		
		public bool ShowFileName {
			get {
				return showFileName;
			}
			set {
				if (showFileName != value) {
					showFileName = value;
					InvalidateText();
				}
			}
		}
		
		protected override object CreateText()
		{
			LoggingService.Debug("Creating text for search result (" + lineNumber + ", " + column + ") ");
			
			TextBlock textBlock = new TextBlock();
			textBlock.Inlines.Add("(" + lineNumber + ", " + column + ")\t");
			
			string displayText = result.DisplayText;
			if (displayText != null) {
				textBlock.Inlines.Add(displayText);
			} else if (inlineBuilder != null) {
				textBlock.Inlines.AddRange(inlineBuilder.CreateRuns());
			}
			
			if (showFileName) {
				textBlock.Inlines.Add(
					new Run {
						Text = StringParser.Parse("\t${res:MainWindow.Windows.SearchResultPanel.In} ")
							+ Path.GetFileName(result.FileName) + "(" + Path.GetDirectoryName(result.FileName) +")",
						FontStyle = FontStyles.Italic
					});
			}
			return textBlock;
		}
		
		public override void ActivateItem()
		{
			ITextEditorProvider provider = FileService.JumpToFilePosition(result.FileName, lineNumber, column) as ITextEditorProvider;
			if (provider != null) {
				ITextMarkerService markerService = provider.TextEditor.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
				if (markerService != null) {
					ITextMarker marker = markerService.Create(provider.TextEditor.Document.PositionToOffset(lineNumber, column), result.Length);
					marker.BackgroundColor = Colors.Orange;
					marker.Tag = this;
					EventHandler remover = null;
					remover = (sender, e) => {
						marker.Delete();
						provider.TextEditor.Caret.PositionChanged -= remover;
					};
					provider.TextEditor.Caret.PositionChanged += remover;
				}
			}
		}
	}
}
