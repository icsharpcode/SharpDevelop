// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Snippets
{
	/// <summary>
	/// Inserts the previously selected text at the selection marker.
	/// </summary>
	[Serializable]
	public class SnippetSelectionElement : SnippetElement
	{
		/// <inheritdoc/>
		public override void Insert(InsertionContext context)
		{
			context.InsertText(context.SelectedText);
			if (string.IsNullOrEmpty(context.SelectedText))
				SnippetCaretElement.SetCaret(context);
		}
	}
}
