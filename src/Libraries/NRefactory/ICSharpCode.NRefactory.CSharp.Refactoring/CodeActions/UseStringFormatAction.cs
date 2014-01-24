//
// UseStringFormatAction.cs
//
// Author:
//      Mansheng Yang <lightyang0@gmail.com>
//
// Copyright (c) 2012 Mansheng Yang <lightyang0@gmail.com>
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
using System.Collections.Generic;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Use string.Format()",
	                Description = "Convert concatenation of strings and objects to string.Format()")]
	public class UseStringFormatAction : CodeActionProvider
	{
		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			// NOTE: @, multiple occurance
			var node = context.GetNode();
			while (node != null && !IsStringConcatenation(context, node as BinaryOperatorExpression))
				node = node.Parent;

			if (node == null)
				yield break;

			var expr = (BinaryOperatorExpression)node;
			var parent = expr.Parent as BinaryOperatorExpression;
			while (parent != null && parent.Operator == BinaryOperatorType.Add) {
				expr = parent;
				parent = expr.Parent as BinaryOperatorExpression;
			}

			yield return new CodeAction(context.TranslateString("Use 'string.Format()'"),
			                             script => {
				var stringType = new PrimitiveType("string");
				var formatInvocation = stringType.Invoke("Format");
				var formatLiteral = new PrimitiveExpression("");
				var counter = 0;
				var arguments = new List<Expression>();

				formatInvocation.Arguments.Add(formatLiteral);
				var concatItems = GetConcatItems(context, expr);
				bool hasVerbatimStrings = false;
				bool hasNonVerbatimStrings = false;

				foreach (var item in concatItems) {
					if (IsStringLiteral(item)) {
						var stringLiteral = (PrimitiveExpression)item;
						if (stringLiteral.LiteralValue[0] == '@') {
							hasVerbatimStrings = true;
						} else {
							hasNonVerbatimStrings = true;
						}
					}
				}
					
				var format = new StringBuilder();
				var verbatim = hasVerbatimStrings && hasNonVerbatimStrings;
				format.Append('"');
				foreach (var item in concatItems) {
					if (IsStringLiteral(item)) {
						var stringLiteral = (PrimitiveExpression)item;

						string rawLiteral;
						if (hasVerbatimStrings && hasNonVerbatimStrings) {
							rawLiteral = stringLiteral.Value.ToString().Replace("\"", "\"\"");
						} else {
							if (stringLiteral.LiteralValue[0] == '@') {
								verbatim = true;
								rawLiteral = stringLiteral.LiteralValue.Substring(2, stringLiteral.LiteralValue.Length - 3);
							} else {
								rawLiteral = stringLiteral.LiteralValue.Substring(1, stringLiteral.LiteralValue.Length - 2);
							}
						}
						format.Append(QuoteBraces(rawLiteral));
					} else {
						Expression myItem = RemoveUnnecessaryToString(item);
						string itemFormatStr = DetermineItemFormatString(ref myItem);
							
						var index = IndexOf(arguments, myItem);
						if (index == -1) {
							// new item
							formatInvocation.Arguments.Add(myItem.Clone());
							arguments.Add(item);
							format.Append("{" + counter++ + itemFormatStr + "}");
						} else {
							// existing item
							format.Append("{" + index + itemFormatStr + "}");
						}
					}
				}
				format.Append('"');
				if (verbatim)
					format.Insert(0, '@');
				formatLiteral.SetValue(format.ToString(), format.ToString());
				if (arguments.Count > 0)
					script.Replace(expr, formatInvocation);
				else
					script.Replace(expr, formatLiteral);
			}, node);
		}

		Expression RemoveUnnecessaryToString(Expression myItem)
		{
			if (myItem is InvocationExpression) {
				InvocationExpression invocation = (InvocationExpression)myItem;
				if (invocation.Target is MemberReferenceExpression &&
					((MemberReferenceExpression)invocation.Target).MemberName.Equals("ToString") &&
					invocation.Arguments.Count == 0) {
					myItem = invocation.Target.FirstChild as Expression;
				}
                
			}
			return myItem;
		}

		string QuoteBraces(string rawLiteral)
		{
			if (!rawLiteral.Contains("{") && !rawLiteral.Contains("}"))
				return rawLiteral;

			StringBuilder quoted = new StringBuilder();
			for (int i = 0; i < rawLiteral.Length; i++) {
				char c = rawLiteral[i];
				if (c == '{')
					quoted.Append("{{");
				else if (c == '}')
					quoted.Append("}}");
				else
					quoted.Append(c);
			}
			return quoted.ToString();
		}

		static string DetermineItemFormatString(ref Expression myItem)
		{
			if (myItem is InvocationExpression) {
				InvocationExpression invocation = myItem as InvocationExpression;
				if (invocation.Target is MemberReferenceExpression &&
					((MemberReferenceExpression)invocation.Target).MemberName.Equals("ToString") &&
					invocation.Arguments.Count == 1) {
					IEnumerator<Expression> i = invocation.Arguments.GetEnumerator();
					i.MoveNext();
					Expression arg = i.Current;
					if (IsStringLiteral(arg) && invocation.Target.FirstChild is Expression) {
						string formatStr = ((PrimitiveExpression)arg).Value as string;
						myItem = invocation.Target.FirstChild as Expression; // invocation target identifier is first child
						return ":" + formatStr;
					}
				}
			}
            
			return "";
		}

		static int IndexOf(IList<Expression> arguments, Expression item)
		{
			for (int i = 0; i < arguments.Count; i++) {
				if (item.Match(arguments[i]).Success)
					return i;
			}
			return -1;
		}

		static IEnumerable<Expression> GetConcatItems(RefactoringContext context, BinaryOperatorExpression expr)
		{
			var leftExpr = expr.Left as BinaryOperatorExpression;
			if (IsStringConcatenation(context, leftExpr)) {
				foreach (var item in GetConcatItems (context, leftExpr))
					yield return item;
			} else {
				yield return expr.Left;
			}

			var rightExpr = expr.Right as BinaryOperatorExpression;
			if (IsStringConcatenation(context, rightExpr)) {
				foreach (var item in GetConcatItems (context, rightExpr))
					yield return item;
			} else {
				yield return expr.Right;
			}
		}

		static bool IsStringConcatenation(RefactoringContext context, BinaryOperatorExpression expr)
		{
			if (expr == null || expr.Operator != BinaryOperatorType.Add)
				return false;
			var typeDef = context.Resolve(expr).Type.GetDefinition();
			return typeDef != null && typeDef.KnownTypeCode == KnownTypeCode.String;
		}

		static bool IsStringLiteral(AstNode node)
		{
			var expr = node as PrimitiveExpression;
			return expr != null && expr.Value is string;
		}
	}
}
