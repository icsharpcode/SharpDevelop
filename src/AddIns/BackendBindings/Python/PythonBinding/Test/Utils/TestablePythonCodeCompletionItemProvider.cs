// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace PythonBinding.Tests.Utils
{
	public class TestablePythonCodeCompletionItemProvider : PythonCodeCompletionItemProvider
	{
		public DefaultCompletionItemList CallBaseCreateCompletionItemList()
		{
			return base.CreateCompletionItemList();
		}
	}
}
