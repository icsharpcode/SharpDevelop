// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace SharpRefactoring
{
	/// <summary>
	/// Registers refactoring:switchbody snippet tag.
	/// (path /SharpDevelop/ViewContent/AvalonEdit/SnippetElementProviders)
	/// </summary>
	public class SwitchSnippetProvider : ISnippetElementProvider
	{
		public SnippetElement GetElement(SnippetInfo snippetInfo)
		{
			if ("refactoring:switchbody".Equals(snippetInfo.Tag, StringComparison.OrdinalIgnoreCase))
				return new SwitchBodySnippetElement();
			
			return null;
		}
	}
}
