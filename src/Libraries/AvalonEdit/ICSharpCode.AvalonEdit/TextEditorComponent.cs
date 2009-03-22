// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.ComponentModel;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit
{
	/// <summary>
	/// Represents a text editor control (<see cref="TextEditor"/>, <see cref="TextArea"/>
	/// or <see cref="Gui.TextView"/>).
	/// </summary>
	public interface ITextEditorComponent
	{
		/// <summary>
		/// Gets the document being edited.
		/// </summary>
		TextDocument Document { get; }
		
		/// <summary>
		/// Occurs when the Document property changes (when the text editor is connected to another
		/// document - not when the document content changes).
		/// </summary>
		event EventHandler DocumentChanged;
		
		/// <summary>
		/// Gets the options of the text editor.
		/// </summary>
		TextEditorOptions Options { get; }
		
		/// <summary>
		/// Occurs when the Options property changes, or when an option inside the current option list
		/// changes.
		/// </summary>
		event PropertyChangedEventHandler OptionChanged;
	}
}
