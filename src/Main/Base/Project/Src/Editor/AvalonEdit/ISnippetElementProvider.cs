// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
