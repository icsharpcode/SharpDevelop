// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Snippets
{
	/// <summary>
	/// Sets the caret position after interactive mode has finished.
	/// </summary>
	public class SnippetCaretElement : SnippetElement
	{
		/// <inheritdoc/>
		public override void Insert(InsertionContext context)
		{
			TextAnchor pos = context.Document.CreateAnchor(context.InsertionPosition);
			pos.SurviveDeletion = true;
			context.Deactivated += delegate {
				context.TextArea.Caret.Offset = pos.Offset;
			};
		}
	}
}
