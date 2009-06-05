// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
