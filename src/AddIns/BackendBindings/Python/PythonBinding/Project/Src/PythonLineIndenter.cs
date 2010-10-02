// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.PythonBinding
{
	public class PythonLineIndenter : LineIndenter
	{
		public PythonLineIndenter(ITextEditor editor, IDocumentLine line)
			: base(editor, line)
		{
		}

		protected override bool ShouldIncreaseLineIndent()
		{
			return PreviousLine.EndsWith(":");
		}
		
		protected override bool ShouldDecreaseLineIndent()
		{
			if (PreviousLine == "pass") {
				return true;
			} else if (PreviousLineIsReturnStatement()) {
				return true;
			} else if (PreviousLineIsRaiseStatement()) {
				return true;
			} else if (PreviousLine == "break") {
				return true;
			}
			return false;
		}
		
		bool PreviousLineIsReturnStatement()
		{
			return (PreviousLine == "return") || PreviousLine.StartsWith("return ");
		}
		
		bool PreviousLineIsRaiseStatement()
		{
			return (PreviousLine == "raise") || PreviousLine.StartsWith("raise ");
		}
	}
}
