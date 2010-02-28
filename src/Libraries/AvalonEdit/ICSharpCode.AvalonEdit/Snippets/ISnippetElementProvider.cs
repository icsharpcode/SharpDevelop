// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.Snippets
{
	/// <summary>
	/// Description of ISnippetElementProvider.
	/// </summary>
	public interface ISnippetElementProvider
	{
		SnippetElement GetElement(string tag);
	}
}
