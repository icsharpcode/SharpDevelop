// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.VBNetBinding
{
	public class VBNetCompletionBinding : ICodeCompletionBinding
	{
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			switch (ch) {
				case ' ':
					if (CodeCompletionOptions.KeywordCompletionEnabled) {
						string word = editor.GetWordBeforeCaret();
						if (!string.IsNullOrEmpty(word)) {
							if (HandleKeyword(editor, word))
								return CodeCompletionKeyPressResult.Completed;
						}
					}
					break;
				default:
					
					break;
			}
			
			return CodeCompletionKeyPressResult.None;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			return false;
		}
		
		bool HandleKeyword(ITextEditor editor, string word)
		{
			return false;
		}
	}
}
