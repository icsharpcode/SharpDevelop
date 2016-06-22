// 
// TypeScriptCompletionItemProvider.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.TypeScriptBinding.Hosting;

namespace ICSharpCode.TypeScriptBinding
{
	public class TypeScriptCompletionItemProvider
	{
		TypeScriptContext context;
		bool memberCompletion;
		
		public TypeScriptCompletionItemProvider(TypeScriptContext context)
		{
			this.context = context;
		}
		
		public bool ShowCompletion(ITextEditor editor, bool memberCompletion)
		{
			this.memberCompletion = memberCompletion;
			ICompletionItemList list = GenerateCompletionList(editor);
			if (list.Items.Any()) {
				editor.ShowCompletionWindow(list);
				return true;
			}
			return false;
		}
		
		public ICompletionItemList GenerateCompletionList(ITextEditor editor)
		{
			CompletionInfo result = context.GetCompletionItems(
				editor.FileName,
				editor.Caret.Offset,
				editor.Document.Text,
				memberCompletion);
			
			var itemList = new DefaultCompletionItemList();
			if (result != null) {
				
				var completionDetailsProvider = new CompletionEntryDetailsProvider(
					context,
					editor.FileName,
					editor.Caret.Offset);
				
				itemList.Items.AddRange(result.entries.Select(entry => new TypeScriptCompletionItem(entry, completionDetailsProvider)));
				itemList.SortItems();
			}
			return itemList;
		}
	}
}
