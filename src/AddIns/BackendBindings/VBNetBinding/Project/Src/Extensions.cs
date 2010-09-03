// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.VBNetBinding
{
	public static class Extensions
	{
		public static T PeekOrDefault<T>(this Stack<T> stack)
		{
			if (stack.Count > 0)
				return stack.Peek();
			
			return default(T);
		}
		
		public static T PopOrDefault<T>(this Stack<T> stack)
		{
			if (stack.Count > 0)
				return stack.Pop();
			
			return default(T);
		}
		
		internal static VBNetCompletionItemList ToVBCCList(this ICompletionItemList list)
		{
			var result = new VBNetCompletionItemList() {
				SuggestedItem = list.SuggestedItem,
				PreselectionLength = list.PreselectionLength
			};
			
			result.Items.AddRange(list.Items);
			
			return result;
		}
	}
}
