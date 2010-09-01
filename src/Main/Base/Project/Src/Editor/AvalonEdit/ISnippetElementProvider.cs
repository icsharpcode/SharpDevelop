// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Snippets;

namespace ICSharpCode.SharpDevelop.Editor.AvalonEdit
{
	/// <summary>
	/// Used in "/SharpDevelop/ViewContent/AvalonEdit/SnippetElementProviders" to allow AddIns to provide custom snippet elements.
	/// </summary>
	public interface ISnippetElementProvider
	{
		SnippetElement GetElement(SnippetInfo snippetInfo);
	}
	
	public class SnippetInfo
	{
		public readonly string Tag;
		public readonly string SnippetText;
		public readonly int Position;
		
		public SnippetInfo(string tag, string snippetText, int position)
		{
			this.Tag = tag;
			this.SnippetText = snippetText;
			this.Position = position;
		}
	}
}
