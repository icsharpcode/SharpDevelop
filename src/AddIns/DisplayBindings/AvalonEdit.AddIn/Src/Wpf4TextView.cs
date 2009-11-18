// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Adds WPF4 text clarity support to TextView.
	/// </summary>
	public class Wpf4TextView : TextView
	{
		protected override TextFormatter CreateTextFormatter()
		{
			return TextFormatter.Create(TextOptions.GetTextFormattingMode(this));
		}
		
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == TextOptions.TextFormattingModeProperty) {
				RecreateTextFormatter();
			}
		}
	}
	
	public class Wpf4TextArea : TextArea
	{
		public Wpf4TextArea() : base(new Wpf4TextView())
		{
		}
	}
}
