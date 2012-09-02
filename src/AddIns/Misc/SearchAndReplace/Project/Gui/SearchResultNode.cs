// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			if (result.DefaultTextColor != null && !IsSelected) {
				if (result.DefaultTextColor.Background != null)
					textBlock.Background = result.DefaultTextColor.Background.GetBrush(null);
				if (result.DefaultTextColor.Foreground != null)
					textBlock.Foreground = result.DefaultTextColor.Foreground.GetBrush(null);
			}
			textBlock.FontFamily = new FontFamily(EditorControlService.GlobalOptions.FontFamily);

			textBlock.Inlines.Add("(" + location.Line + ", " + location.Column + ")\t");
			
			string displayText = result.DisplayText;
			if (displayText != null) {
				textBlock.Inlines.Add(displayText);
			} else if (result.Builder != null) {
				HighlightedInlineBuilder builder = result.Builder;
				if (IsSelected) {
					builder = builder.Clone();
					builder.SetForeground(0, builder.Text.Length, null);
					builder.SetBackground(0, builder.Text.Length, null);
				}
				textBlock.Inlines.AddRange(builder.CreateRuns());
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
