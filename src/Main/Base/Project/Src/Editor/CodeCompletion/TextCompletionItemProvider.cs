// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// Data provider for code completion.
	/// </summary>
	public class TextCompletionItemProvider : AbstractCompletionItemProvider
	{
		string[] texts;
		
		public TextCompletionItemProvider(params string[] texts)
		{
			if (texts == null)
				throw new ArgumentNullException("texts");
			this.texts = texts;
		}
		
		public override ICompletionItemList GenerateCompletionList(ITextEditor editor)
		{
			DefaultCompletionItemList list = new DefaultCompletionItemList();
			foreach (string text in this.texts) {
				list.Items.Add(new DefaultCompletionItem(text) { Image = ClassBrowserIconService.GotoArrow });
			}
			list.SortItems();
			return list;
		}
	}
}
