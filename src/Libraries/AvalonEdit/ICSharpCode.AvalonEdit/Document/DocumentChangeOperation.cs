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
		OffsetChangeMap offsetChangeMap;
		
		public DocumentChangeOperation(TextDocument document, int offset, string removedText, string insertedText, OffsetChangeMap offsetChangeMap)
		{
			this.document = document;
			this.offset = offset;
			this.removedText = removedText;
			this.insertedText = insertedText;
			this.offsetChangeMap = offsetChangeMap;
		}
		
		public void Undo()
		{
			document.Replace(offset, insertedText.Length, removedText, offsetChangeMap != null ? offsetChangeMap.Invert() : null);
		}
		
		public void Redo()
		{
			document.Replace(offset, removedText.Length, insertedText, offsetChangeMap);
		}
	}
}
