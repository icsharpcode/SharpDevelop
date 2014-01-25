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

using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// Description of IndentSelection
	/// </summary>
	public class IndentSelection : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
			if (editor == null)
				return;
			
			int beginLine = 1;
			int endLine = editor.Document.LineCount;
			
			if (editor.SelectionLength != 0) {
				beginLine = editor.Document.GetLineByOffset(editor.SelectionStart).LineNumber;
				endLine = editor.Document.GetLineByOffset(editor.SelectionStart + editor.SelectionLength).LineNumber;
			}
			
			using (editor.Document.OpenUndoGroup())
				editor.Language.FormattingStrategy.IndentLines(editor, beginLine, endLine);
		}
	}
}
