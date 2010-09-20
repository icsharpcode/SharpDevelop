// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
