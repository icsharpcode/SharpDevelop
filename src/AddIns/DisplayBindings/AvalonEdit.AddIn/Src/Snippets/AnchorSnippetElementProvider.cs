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
