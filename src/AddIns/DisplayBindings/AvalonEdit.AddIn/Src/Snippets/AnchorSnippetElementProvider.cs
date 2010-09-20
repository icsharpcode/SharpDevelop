// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.AvalonEdit.AddIn.Snippets
{
	public class AnchorSnippetElementProvider : ISnippetElementProvider
	{
		public SnippetElement GetElement(SnippetInfo snippetInfo)
		{
			int typeSeparator = snippetInfo.Tag.IndexOf(':');
			
			if (typeSeparator > 0) {
				string type = snippetInfo.Tag.Substring(0, typeSeparator);
				string name = snippetInfo.Tag.Substring(typeSeparator + 1);
				
				if ("anchor".Equals(type, StringComparison.OrdinalIgnoreCase))
					return new SnippetAnchorElement(name);
			}
			
			return null;
		}
	}
}
