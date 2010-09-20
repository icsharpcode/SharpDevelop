// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.PythonBinding
{
	public class PythonCompletionItemList : DefaultCompletionItemList
	{
		public override CompletionItemListKeyResult ProcessInput(char key)
		{
			if (key == '*') {
				return ProcessAsterisk();
			}
			return base.ProcessInput(key);
		}
		
		CompletionItemListKeyResult ProcessAsterisk()
		{
			InsertSpace = false;
			return CompletionItemListKeyResult.NormalKey;
		}
	}
}
