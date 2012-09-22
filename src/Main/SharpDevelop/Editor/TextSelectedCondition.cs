// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor
{
	sealed class TextSelectedCondition : IConditionEvaluator
	{
		public bool IsValid(object owner, Condition condition)
		{
			ITextEditor textEditor = SD.GetActiveViewContentService<ITextEditor>();
			if (textEditor != null) {
				return textEditor.SelectionLength > 0;
			}
			return false;
		}
	}
}
