// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.CppBinding
{
	/// <summary>
	/// Formatting strategy for C++ (only implements insertion of comment tags).
	/// </summary>
	public class CppFormattingStrategy : DefaultFormattingStrategy
	{
		public CppFormattingStrategy()
		{
		}
		
		public override void SurroundSelectionWithComment(ITextEditor editor)
		{
			SurroundSelectionWithSingleLineComment(editor, "//");
		}
	}
}
