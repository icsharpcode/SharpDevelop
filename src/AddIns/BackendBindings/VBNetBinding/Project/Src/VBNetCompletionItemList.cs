// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.VBNetBinding
{
	public class VBNetCompletionItemList : NRefactoryCompletionItemList
	{
		public ITextEditor Editor { get; set; }
		
		public ICompletionListWindow Window { get; set; }
		
		public override CompletionItemListKeyResult ProcessInput(char key)
		{
			if (key == '?' && string.IsNullOrWhiteSpace(Editor.Document.GetText(Window.StartOffset, Window.EndOffset - Window.StartOffset)))
				return CompletionItemListKeyResult.NormalKey;
			
			if (key == '@' && Window.StartOffset > 0 && Editor.Document.GetCharAt(Window.StartOffset - 1) == '.')
				return CompletionItemListKeyResult.NormalKey;
			
			return base.ProcessInput(key);
		}
		
		public override void Complete(CompletionContext context, ICompletionItem item)
		{
			base.Complete(context, item);
			
			if (item.Text == "..") {
				VBNetCompletionBinding.Instance.CtrlSpace(context.Editor);
			}
		}
	}
}
