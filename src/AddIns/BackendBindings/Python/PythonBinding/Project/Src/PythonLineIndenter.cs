// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
