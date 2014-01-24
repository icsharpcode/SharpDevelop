// 
// GenerateSwitchLabels.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2011 Mike Krüger <mkrueger@novell.com>
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
using ICSharpCode.NRefactory.TypeSystem;
using System.Threading;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Generate switch labels", Description = "Creates switch lables for enumerations.")]
	public class GenerateSwitchLabelsAction : CodeActionProvider
	{
		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var switchStatement = context.GetNode<SwitchStatement> ();

			if (switchStatement == null || !switchStatement.SwitchToken.Contains(context.Location))
				yield break;
			var result = context.Resolve(switchStatement.Expression);
			if (result.Type.Kind != TypeKind.Enum)
				yield break;

			if (switchStatement.SwitchSections.Count == 0) {
				yield return new CodeAction(context.TranslateString("Create switch labels"), script => {
					var type = result.Type;
					var newSwitch = (SwitchStatement)switchStatement.Clone();
					
					var target = context.CreateShortType(result.Type);
					foreach (var field in type.GetFields ()) {
						if (field.IsSynthetic || !field.IsConst)
							continue;
						newSwitch.SwitchSections.Add(new SwitchSection() {
							CaseLabels = {
								new CaseLabel(target.Clone().Member(field.Name))
							},
							Statements = {
								new BreakStatement()
							}
						});
					}
					
					newSwitch.SwitchSections.Add(new SwitchSection() {
						CaseLabels = {
							new CaseLabel()
						},
						Statements = {
							new ThrowStatement(new ObjectCreateExpression(context.CreateShortType("System", "ArgumentOutOfRangeException")))
						}
					});
					
					script.Replace(switchStatement, newSwitch);
				}, switchStatement);
			} else {
				var missingFields = new List<IField>();
				foreach (var field in result.Type.GetFields ()) {
					if (field.IsSynthetic || !field.IsConst)
						continue;
					if (!IsHandled(context, switchStatement, field)) {
						missingFields.Add(field); 
					}
				}
				if (missingFields.Count == 0)
					yield break;
				yield return new CodeAction(context.TranslateString("Create missing switch labels"), script => {
					var type = result.Type;
					//var newSwitch = (SwitchStatement)switchStatement.Clone();
					var insertNode = (AstNode)switchStatement.SwitchSections.LastOrDefault(s => !s.CaseLabels.Any(label => label.Expression.IsNull)) ?? switchStatement.LBraceToken;

					var target = context.CreateShortType(result.Type);
					foreach (var field in missingFields) {
						script.InsertAfter(insertNode, new SwitchSection() {
							CaseLabels =  {
								new CaseLabel(target.Clone().Member(field.Name))
							},
							Statements =  {
								new BreakStatement()
							}
						});
					}
				}, switchStatement);
			}
		}

		static bool IsHandled(RefactoringContext context, SwitchStatement switchStatement, IField field)
		{
			foreach (var sect in switchStatement.SwitchSections) {
				foreach (var caseLabel in sect.CaseLabels) {
					var resolveCase = context.Resolve(caseLabel.Expression) as MemberResolveResult;
					if (resolveCase == null)
						continue;
					if (field == resolveCase.Member)
						return true;
				}
			}
			return false;
		}
	}
}
