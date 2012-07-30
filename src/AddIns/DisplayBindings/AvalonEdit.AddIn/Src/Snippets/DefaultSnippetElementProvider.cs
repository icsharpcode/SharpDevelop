// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.AvalonEdit.AddIn.Snippets
{
	public class DefaultSnippetElementProvider : ISnippetElementProvider
	{
		public SnippetElement GetElement(SnippetInfo snippetInfo)
		{
			if ("Selection".Equals(snippetInfo.Tag, StringComparison.OrdinalIgnoreCase))
				return new SnippetSelectionElement() { Indentation = GetWhitespaceBefore(snippetInfo.SnippetText, snippetInfo.Position).Length };
			if ("Caret".Equals(snippetInfo.Tag, StringComparison.OrdinalIgnoreCase)) {
				// If a ${Selection} exists, use the ${Caret} only if there is text selected
				// (if no text is selected, ${Selection} will set the caret
				if (snippetInfo.SnippetText.IndexOf("${Selection}", StringComparison.OrdinalIgnoreCase) >= 0)
					return new SnippetCaretElement(setCaretOnlyIfTextIsSelected: true);
				else
					return new SnippetCaretElement();
			}
			
			return null;
		}
		
		static string GetWhitespaceBefore(string snippetText, int offset)
		{
			int start = snippetText.LastIndexOfAny(new[] { '\r', '\n' }, offset) + 1;
			return snippetText.Substring(start, offset - start);
		}
	}
}
