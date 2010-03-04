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
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.AddIn.Options
{
	/// <summary>
	/// Represents an item visible in the highlighting editor.
	/// </summary>
	public interface IHighlightingItem : INotifyPropertyChanged
	{
		string Name { get; }
		
		/// <summary>
		/// Gets/Sets whether the element uses bold font.
		/// </summary>
		bool Bold { get; set; }
		
		/// <summary>
		/// Gets/Sets whether the element uses italic font.
		/// </summary>
		bool Italic { get; set; }
		
		Color Foreground { get; set; }
		bool UseDefaultForeground { get; set; }
		Color Background { get; set; }
		bool UseDefaultBackground { get; set; }
		
		bool CanUseDefaultColors { get; }
		bool CanSetForeground { get; }
		bool CanSetBackground { get; }
		bool CanSetFont { get; }
		
		bool IsCustomized { get; }
		void Reset();
		
		/// <summary>
		/// Parent definition to use in conjunction with the example.
		/// </summary>
		IHighlightingDefinition ParentDefinition { get; }
		
		/// <summary>
		/// Shows an example
		/// </summary>
		void ShowExample(TextArea exampleTextArea);
	}
}
