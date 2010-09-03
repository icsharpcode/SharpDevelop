// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace Hornung.ResourceToolkit.CodeCompletion
{
	/// <summary>
	/// Provides code completion data for the ${res tag.
	/// </summary>
	public sealed class ICSharpCodeCoreTagCompletionItemList : DefaultCompletionItemList
	{
		public ICSharpCodeCoreTagCompletionItemList(ITextEditor editor)
			: base()
		{
			var item = new DefaultCompletionItem("{res") { Image = ClassBrowserIconService.Keyword };
			this.Items.Add(item);
			this.SuggestedItem = item;
		}
		
		public override CompletionItemListKeyResult ProcessInput(char key)
		{
			if (key == ':' || key == '}' || Char.IsWhiteSpace(key)) {
				return CompletionItemListKeyResult.InsertionKey;
			} else {
				return CompletionItemListKeyResult.NormalKey;
			}
		}
	}
}
