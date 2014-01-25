// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
				int startLine = editor.Document.GetLineByOffset(editor.SelectionStart).LineNumber;
				editor.SelectedText = DocumentUtilities.NormalizeNewLines(signature, editor.Document, startLine);
				int endLine = editor.Document.GetLineByOffset(editor.SelectionStart + editor.SelectionLength).LineNumber;
				editor.Language.FormattingStrategy.IndentLines(editor, startLine, endLine);
			}
		}
	}
}
