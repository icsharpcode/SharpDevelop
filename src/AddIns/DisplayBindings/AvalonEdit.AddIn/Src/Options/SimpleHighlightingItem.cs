// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.AvalonEdit.AddIn.Options
{
	sealed class SimpleHighlightingItem : IHighlightingItem
	{
		readonly Action<TextArea> onShowExample;
		
		public SimpleHighlightingItem(string name, Action<TextArea> onShowExample)
		{
			this.Name = name;
			this.onShowExample = onShowExample;
		}
		
		public event PropertyChangedEventHandler PropertyChanged { add {} remove {} }
		
		public string Name { get; private set; }
		public bool Bold { get; set; }
		public bool Italic { get; set; }
		
		public Color Foreground { get; set; }
		public bool UseDefaultForeground { get; set; }
		public Color Background { get; set; }
		public bool UseDefaultBackground { get; set; }
		
		public bool CanUseDefaultColors { get; set; }
		
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
		
		public IHighlightingDefinition ParentDefinition { get; set; }
		
		public void Reset()
		{
		}
		
		public void ShowExample(TextArea exampleTextArea)
		{
			if (onShowExample != null)
				onShowExample(exampleTextArea);
		}
	}
}
