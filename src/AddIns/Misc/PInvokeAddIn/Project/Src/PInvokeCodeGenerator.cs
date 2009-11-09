// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.PInvokeAddIn
{
	/// <summary>
	/// Generates PInvoke signature code in the SharpDevelop text editor.
	/// </summary>
	public class PInvokeCodeGenerator
	{
		/// <summary>
		/// Inserts the PInvoke signature at the current cursor position.
		/// </summary>
		/// <param name="textArea">The text editor.</param>
		/// <param name="signature">A PInvoke signature string.</param>
		public void Generate(ITextEditor editor, string signature)
		{
			using (editor.Document.OpenUndoGroup()) {
				int startLine = editor.Document.GetLineForOffset(editor.SelectionStart).LineNumber;
				editor.SelectedText = DocumentUtilitites.NormalizeNewLines(signature, editor.Document, startLine);
				int endLine = editor.Document.GetLineForOffset(editor.SelectionStart + editor.SelectionLength).LineNumber;
				editor.Language.FormattingStrategy.IndentLines(editor, startLine, endLine);
			}
		}
	}
}
