// 
// ExtractMethodAction.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
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
using ICSharpCode.NRefactory.Semantics;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.Analysis;
using System.Threading;
using ICSharpCode.NRefactory.TypeSystem;
using System.Threading.Tasks;
using Mono.CSharp;

namespace ICSharpCode.NRefactory.CSharp.Refactoring.ExtractMethod
{
	[ContextAction("Extract method", Description = "Creates a new method out of selected text.")]
	public class ExtractMethodAction : CodeActionProvider
	{
		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			if (!context.IsSomethingSelected)
				yield break;
			var selected = new List<AstNode>(context.GetSelectedNodes());
			if (selected.Count == 0)
				yield break;
			
			if (selected.Count == 1 && selected [0] is Expression) {
				var codeAction = CreateFromExpression(context, (Expression)selected [0]);
				if (codeAction == null)
					yield break;
				yield return codeAction;
			}

			foreach (var node in selected) {
				if (!(node is Statement) && !(node is Comment) && !(node is NewLineNode) && !(node is PreProcessorDirective))
					yield break;
				if (node.DescendantNodesAndSelf().Any(n => n is YieldBreakStatement || n is YieldReturnStatement))
					yield break;
			}
			var action = CreateFromStatements (context, new List<AstNode> (selected));
			if (action != null)
				yield return action;
		}
		
		CodeAction CreateFromExpression(RefactoringContext context, Expression expression)
		{
			var resolveResult = context.Resolve(expression);
			if (resolveResult.IsError)
				return null;
			
			return new CodeAction(context.TranslateString("Extract method"), script => {
				string methodName = "NewMethod";
				var method = new MethodDeclaration {
					ReturnType = context.CreateShortType(resolveResult.Type),
					Name = methodName,
					Body = new BlockStatement {
						new ReturnStatement(expression.Clone())
					}
				};
				if (!StaticVisitor.UsesNotStaticMember(context, expression))
					method.Modifiers |= Modifiers.Static;

				var usedVariables = VariableLookupVisitor.Analyze(context, expression);
				
				var inExtractedRegion = new VariableUsageAnalyzation (context, usedVariables);

				usedVariables.Sort ((l, r) => l.Region.Begin.CompareTo (r.Region.Begin));
				var target = new IdentifierExpression(methodName);
				var invocation = new InvocationExpression(target);
				foreach (var variable in usedVariables) {
					Expression argumentExpression = new IdentifierExpression(variable.Name); 
					
					var mod = ParameterModifier.None;
					if (inExtractedRegion.GetStatus (variable) == VariableState.Changed) {
						mod = ParameterModifier.Ref;
						argumentExpression = new DirectionExpression(FieldDirection.Ref, argumentExpression);
					}
					
					method.Parameters.Add(new ParameterDeclaration(context.CreateShortType(variable.Type), variable.Name, mod));
					invocation.Arguments.Add(argumentExpression);
				}

				script
					.InsertWithCursor(context.TranslateString("Extract method"), Script.InsertPosition.Before, method)
					.ContinueScript (delegate {
						script.Replace(expression, invocation);
						script.Link(target, method.NameToken);
					});
			}, expression);
		}
		
		CodeAction CreateFromStatements(RefactoringContext context, List<AstNode> statements)
		{
			if (!(statements [0].Parent is Statement))
				return null;
			
			return new CodeAction(context.TranslateString("Extract method"), script => {
				string methodName = "NewMethod";
				var method = new MethodDeclaration() {
					ReturnType = new PrimitiveType("void"),
					Name = methodName,
					Body = new BlockStatement()
				};
				bool usesNonStaticMember = false;
				foreach (var node in statements) {
					usesNonStaticMember |= StaticVisitor.UsesNotStaticMember(context, node);
					if (node is Statement) {
						method.Body.Add((Statement)node.Clone());
					} else {
						method.Body.AddChildUnsafe (node.Clone (), node.Role);
					}
				}
				if (!usesNonStaticMember)
					method.Modifiers |= Modifiers.Static;
				
				var target = new IdentifierExpression(methodName);
				var invocation = new InvocationExpression(target);
				
				var usedVariables = VariableLookupVisitor.Analyze(context, statements);
				
				var inExtractedRegion = new VariableUsageAnalyzation (context, usedVariables);
				var lastStatement = statements [statements.Count - 1];
				
				var stmt = statements [0].GetParent<BlockStatement>();
				while (stmt.GetParent<BlockStatement> () != null) {
					stmt = stmt.GetParent<BlockStatement>();
				}
				
				inExtractedRegion.SetAnalyzedRange(statements [0], lastStatement);
				stmt.AcceptVisitor (inExtractedRegion);
				
				var beforeExtractedRegion = new VariableUsageAnalyzation (context, usedVariables);
				beforeExtractedRegion.SetAnalyzedRange(statements [0].Parent, statements [0], true, false);
				stmt.AcceptVisitor (beforeExtractedRegion);
				
				var afterExtractedRegion = new VariableUsageAnalyzation (context, usedVariables);
				afterExtractedRegion.SetAnalyzedRange(lastStatement, stmt.Statements.Last(), false, true);
				stmt.AcceptVisitor (afterExtractedRegion);
				usedVariables.Sort ((l, r) => l.Region.Begin.CompareTo (r.Region.Begin));

				IVariable generatedReturnVariable = null;
				foreach (var variable in usedVariables) {
					if ((variable is IParameter) || beforeExtractedRegion.Has (variable) || !afterExtractedRegion.Has (variable))
						continue;
					generatedReturnVariable = variable;
					method.ReturnType = context.CreateShortType (variable.Type);
					method.Body.Add (new ReturnStatement (new IdentifierExpression (variable.Name)));
					break;
				}

				int parameterOutCount = 0;
				foreach (var variable in usedVariables) {
					if (!(variable is IParameter) && !beforeExtractedRegion.Has (variable) && !afterExtractedRegion.Has (variable))
						continue;
					if (variable == generatedReturnVariable)
						continue;
					Expression argumentExpression = new IdentifierExpression(variable.Name); 
					
					ParameterModifier mod = ParameterModifier.None;
					if (inExtractedRegion.GetStatus (variable) == VariableState.Changed) {
						if (beforeExtractedRegion.GetStatus (variable) == VariableState.None) {
							mod = ParameterModifier.Out;
							argumentExpression = new DirectionExpression(FieldDirection.Out, argumentExpression);
							parameterOutCount++;
						} else {
							mod = ParameterModifier.Ref;
							argumentExpression = new DirectionExpression(FieldDirection.Ref, argumentExpression);
						}
					}
					
					method.Parameters.Add(new ParameterDeclaration(context.CreateShortType(variable.Type), variable.Name, mod));
					invocation.Arguments.Add(argumentExpression);
				}

				ParameterDeclaration parameterToTransform = null;
				bool transformParameterToReturn = method.ReturnType is PrimitiveType && 
				                                  ((PrimitiveType)method.ReturnType).Keyword == "void" &&
				                                  parameterOutCount == 1;
				if(transformParameterToReturn) {
					parameterToTransform = method.Parameters.First(p => p.ParameterModifier == ParameterModifier.Out);
					parameterToTransform.Remove();
					var argumentExpression = invocation.Arguments.OfType<DirectionExpression>().First(a => a.FieldDirection == FieldDirection.Out);
					argumentExpression.Remove();
					method.ReturnType = parameterToTransform.Type.Clone();
					var argumentDecl = new VariableDeclarationStatement(parameterToTransform.Type.Clone(),parameterToTransform.Name);
					method.Body.InsertChildBefore(method.Body.First(),argumentDecl,BlockStatement.StatementRole);
					method.Body.Add(new ReturnStatement (new IdentifierExpression (parameterToTransform.Name)));
				}

				script
					.InsertWithCursor(context.TranslateString("Extract method"), Script.InsertPosition.Before, method)
					.ContinueScript(delegate {
						foreach (var node in statements.Skip (1)) {
							if (node is NewLineNode)
								continue;
							script.Remove(node);
						}
						foreach (var variable in usedVariables) {
							if ((variable is IParameter) || beforeExtractedRegion.Has (variable) || !afterExtractedRegion.Has (variable))
								continue;
							if (variable == generatedReturnVariable)
								continue;
							script.InsertBefore (statements [0], new VariableDeclarationStatement (context.CreateShortType(variable.Type), variable.Name));
						}
						Statement invocationStatement;

						if (generatedReturnVariable != null) {
							invocationStatement = new VariableDeclarationStatement (new SimpleType ("var"), generatedReturnVariable.Name, invocation);
						} else if(transformParameterToReturn) {
							invocationStatement = new AssignmentExpression(new IdentifierExpression(parameterToTransform.Name), invocation);
						} else {
							invocationStatement = invocation;
						}

						script.Replace(statements [0], invocationStatement);


						script.Link(target, method.NameToken);
					});
			}, statements.First ().StartLocation, statements.Last ().EndLocation);
		}
	}
}
