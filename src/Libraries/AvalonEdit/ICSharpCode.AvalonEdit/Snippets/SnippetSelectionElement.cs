// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
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
		public int Indentation { get; set; }
		
		/// <inheritdoc/>
		public override void Insert(InsertionContext context)
		{
			StringBuilder tabString = new StringBuilder();
			
			for (int i = 0; i < Indentation; i++) {
				tabString.Append(context.Tab);
			}
			
			string indent = tabString.ToString();
			
			string text = context.SelectedText.TrimStart(' ', '\t');
			
			text = text.Replace(context.LineTerminator,
			                             context.LineTerminator + indent);
			
			context.Document.Insert(context.InsertionPosition, text);
			context.InsertionPosition += text.Length;
			
			if (string.IsNullOrEmpty(context.SelectedText))
				SnippetCaretElement.SetCaret(context);
		}
	}
}
