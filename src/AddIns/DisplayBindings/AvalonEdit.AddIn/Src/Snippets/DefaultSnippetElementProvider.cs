// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>
using System;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.AvalonEdit.AddIn.Snippets
{
	/// <summary>
	/// Description of DefaultSnippetElementProvider.
	/// </summary>
	public class DefaultSnippetElementProvider : ISnippetElementProvider
	{
		public SnippetElement GetElement(SnippetInfo snippetInfo)
		{
			if ("Selection".Equals(snippetInfo.Tag, StringComparison.OrdinalIgnoreCase))
				return new SnippetSelectionElement() { Indentation = GetWhitespaceBefore(snippetInfo.SnippetText, snippetInfo.Position).Length };
			if ("Caret".Equals(snippetInfo.Tag, StringComparison.OrdinalIgnoreCase))
				return new SnippetCaretElement();
			
			return null;
		}
		
		static string GetWhitespaceBefore(string snippetText, int offset)
		{
			int start = snippetText.LastIndexOfAny(new[] { '\r', '\n' }, offset) + 1;
			return snippetText.Substring(start, offset - start);
		}
	}
}
