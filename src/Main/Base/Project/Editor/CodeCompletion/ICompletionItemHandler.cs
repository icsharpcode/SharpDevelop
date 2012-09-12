// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	public interface ICompletionItemHandler
	{
		void Insert(CompletionContext context, ICompletionItem item);
		bool Handles(ICompletionItem item);
	}
}
