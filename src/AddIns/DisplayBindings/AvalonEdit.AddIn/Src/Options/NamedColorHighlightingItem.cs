// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			exampleTextArea.Document.Text = StringParser.Parse(color.ExampleText, GetXshdProperties().ToArray());
		}
		
		IEnumerable<StringTagPair> GetXshdProperties()
		{
			IHighlightingDefinition2 def = ParentDefinition as IHighlightingDefinition2;
			if (def == null)
				yield break;
			foreach (var p in def.Properties)
				yield return new StringTagPair(p.Key, p.Value);
		}
		
		event System.ComponentModel.PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add {} remove {} }
	}
}
