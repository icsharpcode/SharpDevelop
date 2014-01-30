//
// ConvertDoWhileToWhileLoopAction.cs
//
// Author:
//       Luís Reis <luiscubal@gmail.com>
//
// Copyright (c) 2013 Luís Reis
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
using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// Convert do...while to while. For instance, { do x++; while (Foo(x)); } becomes { while(Foo(x)) x++; }.
	/// Note that this action will often change the semantics of the code.
	/// </summary>
	[ContextAction("Convert do...while to while.", Description = "Converts a do...while to a while loop (changing semantics).")]
	public class ConvertDoWhileToWhileLoopAction : CodeActionProvider
	{
		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var node = context.GetNode<DoWhileStatement>();
			if (node == null)
				yield break;

			var target = node.DoToken;
			if (!target.Contains(context.Location)) {
				target = node.WhileToken;
				if (!target.Contains(context.Location)) {
					yield break;
				}
			}

			yield return new CodeAction(context.TranslateString("Convert to while loop"),
			                            script => ConvertToWhileLoop(script, node),
			                            target);

		}

		static void ConvertToWhileLoop(Script script, DoWhileStatement originalStatement)
		{
			script.Replace(originalStatement, new WhileStatement {
				Condition = originalStatement.Condition.Clone(),
				EmbeddedStatement = originalStatement.EmbeddedStatement.Clone()
			});
		}
	}
}

