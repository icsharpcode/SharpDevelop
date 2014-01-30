// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Search;

namespace SearchAndReplace
{
	/// <summary>
	/// Represents a search result.
	/// </summary>
	public sealed class SearchResultNode : SearchNode
	{
		SearchResultMatch result;
		PermanentAnchor anchor;
		
		public SearchResultNode(SearchResultMatch result)
		{
			this.result = result;
			this.anchor = new PermanentAnchor(result.FileName, result.StartLocation.Line, result.StartLocation.Column);
			anchor.SurviveDeletion = true;
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
		
		public FileName FileName {
			get { return anchor.FileName; }
		}
		
		protected override object CreateText()
		{
			var location = anchor.Location;
			
			LoggingService.Debug("Creating text for search result (" + location.Line + ", " + location.Column + ") ");
			
			TextBlock textBlock = new TextBlock();
			textBlock.FontFamily = new FontFamily(SD.EditorControlService.GlobalOptions.FontFamily);
			if (result.DefaultTextColor != null && !IsSelected) {
				if (result.DefaultTextColor.Background != null)
					textBlock.Background = result.DefaultTextColor.Background.GetBrush(null);
				if (result.DefaultTextColor.Foreground != null)
					textBlock.Foreground = result.DefaultTextColor.Foreground.GetBrush(null);
			}

			textBlock.Inlines.Add("(" + location.Line + ", " + location.Column + ")\t");
			
			RichText displayText = result.DisplayText;
			if (displayText != null) {
				if (IsSelected) {
					RichTextModel model = displayText.ToRichTextModel();
					model.SetForeground(0, displayText.Length, null);
					model.SetBackground(0, displayText.Length, null);
					displayText = new RichText(displayText.Text, model);
				}
				textBlock.Inlines.AddRange(displayText.CreateRuns());
			}
			
			if (showFileName) {
				textBlock.Inlines.Add(
					new Run {
						Text = StringParser.Parse("\t${res:MainWindow.Windows.SearchResultPanel.In} ")
							+ Path.GetFileName(anchor.FileName) + "(" + Path.GetDirectoryName(anchor.FileName) +")",
						FontStyle = FontStyles.Italic
					});
			}
			return textBlock;
		}
		
		protected override void OnPropertyChanged(string propertyName)
		{
			base.OnPropertyChanged(propertyName);
			if (propertyName == "IsSelected") {
				InvalidateText();
			}
		}
		
		public override void ActivateItem()
		{
			FileService.JumpToFilePosition(anchor.FileName, anchor.Location.Line, anchor.Location.Column);
		}
	}
}
