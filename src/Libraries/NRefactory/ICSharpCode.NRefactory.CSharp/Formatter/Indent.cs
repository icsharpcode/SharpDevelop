// 
// Indent.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.CSharp
{
	public enum IndentType {
		Block,
		DoubleBlock,
		Continuation,
		Label,
		Empty
	}

	public class Indent
	{
		readonly Stack<IndentType> indentStack = new Stack<IndentType> ();
		readonly TextEditorOptions options;
		
		int curIndent;

		public int CurIndent {
			get {
				return curIndent;
			}
		}

		public Indent(TextEditorOptions options)
		{
			this.options = options;
			Reset();
		}

		Indent(TextEditorOptions options, Stack<IndentType> indentStack, int curIndent) : this(options)
		{
			this.indentStack = indentStack;
			this.curIndent = curIndent;
		}

		public Indent Clone()
		{
			var result = new Indent(options, new Stack<IndentType> (indentStack), curIndent);
			result.indentString = indentString;
			return result;
		}

		public void Reset()
		{
			curIndent = 0;
			indentString = "";
			indentStack.Clear();
		}

		public void Push(IndentType type)
		{
			indentStack.Push(type);
			curIndent += GetIndent(type);
			Update();
		}

		public void Pop()
		{
			curIndent -= GetIndent(indentStack.Pop());
			Update();
		}

		public int Count {
			get {
				return indentStack.Count;
			}
		}

		public IndentType Peek ()
		{
			return indentStack.Peek();
		}

		int GetIndent(IndentType indentType)
		{
			switch (indentType) {
				case IndentType.Block:
					return options.IndentSize;
				case IndentType.DoubleBlock:
					return options.IndentSize * 2;
				case IndentType.Continuation:
					return options.ContinuationIndent;
				case IndentType.Label:
					return options.LabelIndent;
				case IndentType.Empty:
					return 0;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		void Update()
		{
			if (options.TabsToSpaces) {
				indentString = new string(' ', curIndent);
				return;
			}
			indentString = new string('\t', curIndent / options.TabSize) + new string(' ', curIndent % options.TabSize) + new string (' ', ExtraSpaces);
		}

		int extraSpaces;

		public int ExtraSpaces {
			get {
				return extraSpaces;
			}
			set {
				if (value < 0)
					throw new ArgumentOutOfRangeException ("ExtraSpaces >= 0 but was " + value);
				extraSpaces = value;
				Update();
			}
		}

		string indentString;
		public string IndentString {
			get {
				return indentString;
			}
		}

		public override string ToString()
		{
			return string.Format("[Indent: curIndent={0}]", curIndent);
		}
	}
}
