// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Search;

namespace SearchAndReplace
{
	/// <summary>
	/// Represents a search result.
	/// </summary>
	sealed class SearchResultNode : SearchNode
	{
		SearchResultMatch result;
		int lineNumber, column;
		string resultText;
		
		public SearchResultNode(SearchResultMatch result)
		{
			this.result = result;
			
			IDocument document = result.CreateDocument();
			IDocumentLine matchedLine = document.GetLineForOffset(result.Offset);
			lineNumber = matchedLine.LineNumber;
			column = result.Offset - matchedLine.Offset + 1;
			resultText = matchedLine.Text;
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
		
		static readonly FontFamily resultLineFamily = new FontFamily("Consolas, Courier New");
		
		protected override object CreateText()
		{
			string displayText = result.DisplayText;
			if (displayText != null)
				return displayText;
			
			TextBlock textBlock = new TextBlock();
			textBlock.Inlines.Add("(" + lineNumber + ", " + column + ")\t");
			
			Span resultLineSpan = new Span();
			resultLineSpan.FontFamily = resultLineFamily;
			resultLineSpan.Inlines.Add(resultText);
			textBlock.Inlines.Add(resultLineSpan);
			
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
			FileService.JumpToFilePosition(result.FileName, lineNumber, column);
		}
	}
}
