// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// A TextWriter implementation that directly inserts into a document.
	/// </summary>
	public class DocumentTextWriter : TextWriter
	{
		readonly IDocument document;
		int insertionOffset;
		
		/// <summary>
		/// Creates a new DocumentTextWriter that inserts into document, starting at insertionOffset.
		/// </summary>
		public DocumentTextWriter(IDocument document, int insertionOffset)
		{
			this.insertionOffset = insertionOffset;
			if (document == null)
				throw new ArgumentNullException("document");
			this.document = document;
			var line = document.GetLineByOffset(insertionOffset);
			if (line.DelimiterLength == 0)
				line = line.PreviousLine;
			if (line != null)
				this.NewLine = document.GetText(line.EndOffset, line.DelimiterLength);
		}
		
		/// <summary>
		/// Gets/Sets the current insertion offset.
		/// </summary>
		public int InsertionOffset {
			get { return insertionOffset; }
			set { insertionOffset = value; }
		}
		
		/// <inheritdoc/>
		public override void Write(char value)
		{
			document.Insert(insertionOffset, value.ToString());
			insertionOffset++;
		}
		
		/// <inheritdoc/>
		public override void Write(char[] buffer, int index, int count)
		{
			document.Insert(insertionOffset, new string(buffer, index, count));
			insertionOffset += count;
		}
		
		/// <inheritdoc/>
		public override void Write(string value)
		{
			document.Insert(insertionOffset, value);
			insertionOffset += value.Length;
		}
		
		/// <inheritdoc/>
		public override Encoding Encoding {
			get { return Encoding.UTF8; }
		}
	}
}
