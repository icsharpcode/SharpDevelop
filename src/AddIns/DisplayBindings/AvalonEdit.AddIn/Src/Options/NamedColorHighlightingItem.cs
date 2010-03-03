// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace ICSharpCode.AvalonEdit.AddIn.Options
{
	sealed class NamedColorHighlightingItem : IHighlightingItem
	{
		readonly XshdColor color;
		
		public NamedColorHighlightingItem(XshdColor color)
		{
			if (color == null)
				throw new ArgumentNullException("color");
			
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
					return Colors.Black;
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
					return Colors.White;
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
		
		public void ShowExample(ICSharpCode.AvalonEdit.Rendering.TextView exampleTextView)
		{
			throw new NotImplementedException();
		}
		
		event System.ComponentModel.PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add {} remove {} }
	}
}
