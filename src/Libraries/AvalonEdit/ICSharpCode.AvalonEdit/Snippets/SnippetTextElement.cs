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
	/// Represents a text element in a snippet.
	/// </summary>
	[Serializable]
	public class SnippetTextElement : SnippetElement
	{
		string text;
		
		/// <summary>
		/// The text to be inserted.
		/// </summary>
		public string Text {
			get { return text; }
			set {
				CheckBeforeMutation();
				text = value;
			}
		}
		
		/// <inheritdoc/>
		public override void Insert(InsertionContext context)
		{
			if (text != null)
				context.InsertText(text);
		}
	}
}
