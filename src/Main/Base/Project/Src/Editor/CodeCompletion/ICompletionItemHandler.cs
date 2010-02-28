// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	public interface ICompletionItemHandler
	{
		void Insert(CompletionContext context);
		bool Handles(ICompletionItem item);
	}
}
