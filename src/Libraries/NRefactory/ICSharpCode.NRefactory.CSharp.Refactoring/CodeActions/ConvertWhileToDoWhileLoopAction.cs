// 
// ConvertWhileToDoWhileLoopAction.cs
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
using System.Threading;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// Converts a while loop to a do...while loop.
	/// For instance: while (foo) {} becomes do { } while (foo);
	/// </summary>
	[ContextAction("Convert while loop to do...while", Description = "Convert while loop to do...while (changing semantics)")]
	public class ConvertWhileToDoWhileLoopAction : CodeActionProvider
	{
		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var whileLoop = context.GetNode<WhileStatement>();
			if (whileLoop == null || !whileLoop.WhileToken.Contains(context.Location)) {
				yield break;
			}

			yield return new CodeAction(context.TranslateString("Convert to do...while loop"),
			                            script => ApplyAction(script, whileLoop),
			                            whileLoop.WhileToken);
		}

		void ApplyAction(Script script, WhileStatement statement) {
			var doWhile = new DoWhileStatement {
				Condition = statement.Condition.Clone(),
				EmbeddedStatement = statement.EmbeddedStatement.Clone()
			};

			script.Replace(statement, doWhile);
		}
	}
}

