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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn.Options
{
	sealed class NamedColorHighlightingItem : IHighlightingItem
	{
		readonly IHighlightingItem defaultText;
		readonly XshdColor color;
		
		public NamedColorHighlightingItem(IHighlightingItem defaultText, XshdColor color)
		{
			if (defaultText == null)
				throw new ArgumentNullException("defaultText");
			if (color == null)
				throw new ArgumentNullException("color");
			
			this.defaultText = defaultText;
			this.color = color;
		}
		
		public string Name {
			get { return color.Name; }
		}
		
		public bool Bold {
			get {
				return color.FontWeight == FontWeights.Bold;
			}
			set {
				throw new NotSupportedException();
			}
		}
		
		public bool Italic {
			get {
				return color.FontStyle == FontStyles.Italic;
			}
			set {
				throw new NotSupportedException();
			}
		}
		
		public Color Foreground {
			get {
				Color? c = color.Foreground != null ? color.Foreground.GetColor(null) : null;
				if (c != null)
					return c.Value;
				else
					return defaultText.Foreground;
			}
			set {
				throw new NotSupportedException();
			}
		}
		
		public bool UseDefaultForeground {
			get {
				return color.Foreground == null;
			}
			set {
				throw new NotSupportedException();
			}
		}
		
		public Color Background {
			get {
				Color? c = color.Background != null ? color.Background.GetColor(null) : null;
				if (c != null)
					return c.Value;
				else
					return defaultText.Background;
			}
			set {
				throw new NotSupportedException();
			}
		}
		
		public bool UseDefaultBackground {
			get {
				return color.Background == null;
			}
			set {
				throw new NotSupportedException();
			}
		}
		
		public bool CanUseDefaultColors {
			get { return true; }
		}
		
		public bool CanSetForeground {
			get { return false; }
		}
		
		public bool CanSetBackground {
			get { return false; }
		}
		
		public bool CanSetFont {
			get { return false; }
		}
		
		public bool IsCustomized {
			get { return false; }
		}
		
		public void Reset()
		{
		}
		
		public override string ToString()
		{
			return color.Name;
		}
		
		public IHighlightingDefinition ParentDefinition { get; set; }
		
		public void ShowExample(TextArea exampleTextArea)
		{
			string exampleText = StringParser.Parse(color.ExampleText, GetXshdProperties().ToArray());
			int semanticHighlightStart = exampleText.IndexOf("#{#", StringComparison.OrdinalIgnoreCase);
			int semanticHighlightEnd = exampleText.IndexOf("#}#", StringComparison.OrdinalIgnoreCase);
			if (semanticHighlightStart > -1 && semanticHighlightEnd >= semanticHighlightStart + 3) {
				semanticHighlightEnd -= 3;
				exampleText = exampleText.Remove(semanticHighlightStart, 3).Remove(semanticHighlightEnd, 3);
				ITextMarkerService svc = exampleTextArea.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
				exampleTextArea.Document.Text = exampleText;
				if (svc != null) {
					ITextMarker m = svc.Create(semanticHighlightStart, semanticHighlightEnd - semanticHighlightStart);
					m.Tag = (Action<IHighlightingItem, ITextMarker>)(
						(IHighlightingItem item, ITextMarker marker) => {
							marker.BackgroundColor = item.Background;
							marker.ForegroundColor = item.Foreground;
							marker.FontStyle = item.Italic ? FontStyles.Italic : FontStyles.Normal;
							marker.FontWeight = item.Bold ? FontWeights.Bold : FontWeights.Normal;
						});
				}
			} else {
				exampleTextArea.Document.Text = exampleText;
			}
		}
		
		IEnumerable<StringTagPair> GetXshdProperties()
		{
			foreach (var p in ParentDefinition.Properties)
				yield return new StringTagPair(p.Key, p.Value);
		}
		
		event System.ComponentModel.PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add {} remove {} }
	}
}
