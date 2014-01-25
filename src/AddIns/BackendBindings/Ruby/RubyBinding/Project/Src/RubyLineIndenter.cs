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
using System.Collections.Generic;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.RubyBinding
{
	public class RubyLineIndenter : LineIndenter
	{
		List<string> decreaseLineIndentStatements = new List<string>();
		List<string> decreaseLineIndentStartsWithStatements = new List<string>();

		List<string> increaseLineIndentStatements = new List<string>();
		List<string> increaseLineIndentStartsWithStatements = new List<string>();
		List<string> increaseLineIndentEndsWithStatements = new List<string>();
		List<string> increaseLineIndentContainsStatements = new List<string>();

		public RubyLineIndenter(ITextEditor editor, IDocumentLine line)
			: base(editor, line)
		{
			CreateDecreaseLineIndentStatements();
			CreateDecreaseLineIndentStartsWithStatements();
			CreateIncreaseLineIndentStatements();
			CreateIncreaseLineIndentStartsWithStatements();
			CreateIncreaseLineIndentEndsWithStatements();
			CreateIncreaseLineIndentContainsStatements();
		}
		
		void CreateDecreaseLineIndentStatements()
		{
			decreaseLineIndentStatements.Add("break");
			decreaseLineIndentStatements.Add("return");
			decreaseLineIndentStatements.Add("raise");
		}
		
		void CreateDecreaseLineIndentStartsWithStatements()
		{
			decreaseLineIndentStartsWithStatements.Add("return ");
			decreaseLineIndentStartsWithStatements.Add("raise ");
		}
		
		void CreateIncreaseLineIndentStatements()
		{
			increaseLineIndentStatements.Add("else");
			increaseLineIndentStatements.Add("begin");
			increaseLineIndentStatements.Add("rescue");
			increaseLineIndentStatements.Add("ensure");
		}
		
		void CreateIncreaseLineIndentStartsWithStatements()
		{
			increaseLineIndentStartsWithStatements.Add("if ");
			increaseLineIndentStartsWithStatements.Add("def ");
			increaseLineIndentStartsWithStatements.Add("class ");
			increaseLineIndentStartsWithStatements.Add("while ");
			increaseLineIndentStartsWithStatements.Add("elsif ");
			increaseLineIndentStartsWithStatements.Add("loop ");
			increaseLineIndentStartsWithStatements.Add("unless ");
			increaseLineIndentStartsWithStatements.Add("until ");
			increaseLineIndentStartsWithStatements.Add("for ");
			increaseLineIndentStartsWithStatements.Add("rescue ");			
			increaseLineIndentStartsWithStatements.Add("module ");
			increaseLineIndentStartsWithStatements.Add("when ");
			increaseLineIndentStartsWithStatements.Add("case ");
		}
		
		void CreateIncreaseLineIndentEndsWithStatements()
		{
			increaseLineIndentEndsWithStatements.Add(" then");
			increaseLineIndentEndsWithStatements.Add(" do");
			increaseLineIndentEndsWithStatements.Add(" {");
		}
		
		void CreateIncreaseLineIndentContainsStatements()
		{
			increaseLineIndentContainsStatements.Add(" case ");
		}
		
		protected override bool ShouldIncreaseLineIndent()
		{
			if (increaseLineIndentStatements.Contains(PreviousLine)) {
				return true;
			}
			if (PreviousLineStartsWith(increaseLineIndentStartsWithStatements)) {
				return true;
			}
			if (PreviousLineContains(increaseLineIndentContainsStatements)) {
				return true;
			}
			return PreviousLineEndsWith(increaseLineIndentEndsWithStatements);
		}
		
		protected override bool ShouldDecreaseLineIndent()
		{
			if (decreaseLineIndentStatements.Contains(PreviousLine)) {
				return true;
			}
			return PreviousLineStartsWith(decreaseLineIndentStartsWithStatements);
		}
		
		bool PreviousLineStartsWith(List<string> items)
		{
			foreach (string item in items) {
				if (PreviousLine.StartsWith(item)) {
					return true;
				}
			}
			return false;
		}
		
		bool PreviousLineEndsWith(List<string> items)
		{
			foreach (string item in items) {
				if (PreviousLine.EndsWith(item)) {
					return true;
				}
			}
			return false;
		}
		
		bool PreviousLineContains(List<string> items)
		{
			foreach (string item in items) {
				if (PreviousLine.Contains(item)) {
					return true;
				}
			}
			return false;
		}
	}
}
