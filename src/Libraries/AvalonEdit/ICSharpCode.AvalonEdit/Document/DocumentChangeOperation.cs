// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// Describes a change to a TextDocument.
	/// </summary>
	sealed class DocumentChangeOperation : IUndoableOperation
	{
		TextDocument document;
		int offset;
		string removedText;
		string insertedText;
		
		public DocumentChangeOperation(TextDocument document, int offset, string removedText, string insertedText)
		{
			this.document = document;
			this.offset = offset;
			this.removedText = removedText;
			this.insertedText = insertedText;
		}
		
		public void Undo()
		{
			document.Replace(offset, insertedText.Length, removedText);
		}
		
		public void Redo()
		{
			document.Replace(offset, removedText.Length, insertedText);
		}
	}
}
