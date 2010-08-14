// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
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