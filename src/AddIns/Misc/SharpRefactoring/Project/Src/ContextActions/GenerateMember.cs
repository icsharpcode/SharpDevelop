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
using System.Windows;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of GenerateMember.
	/// </summary>
	public class GenerateMemberProvider : ContextActionsProvider
	{
		public override IEnumerable<IContextAction> GetAvailableActions(EditorContext context)
		{
			if (string.IsNullOrEmpty(context.CurrentExpression.Expression)) {
				yield break;
			}
			if (context.CurrentExpression.Region != null && 
			    context.CurrentExpression.Region.EndLine > context.CurrentExpression.Region.BeginLine) {
				// do not yield the action for 2-line expressions like this, which are actually 2 different expressions
				//   variable.(*caret*)
				//   CallFooMethod();
				// this check is not correct for this case because it does not yield the action when it should:
				//   variable.Foo((*caret*)
				//                123);
				yield break;
			}
			var generateCodeAction = GenerateCode.GetContextAction(context);
			if (generateCodeAction != null)
				yield return generateCodeAction;
		}
	}
}
