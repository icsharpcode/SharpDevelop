//
// AutoAsyncIssue.cs
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
using System;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Analysis;
using ICSharpCode.NRefactory.PatternMatching;

using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Refactoring;


namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Old-style asynchronous function can be converted to C# 5 async",
	                  Description = "Detects usage of old-style TaskCompletionSource/ContinueWith and suggests using async/await instead",
	                  Category = IssueCategories.Opportunities,
	                  Severity = Severity.Hint)]
	public class AutoAsyncIssue : GatherVisitorCodeIssueProvider
	{
		static readonly ReturnStatement ReturnTaskCompletionSourcePattern = new ReturnStatement {
			Expression = new MemberReferenceExpression {
				Target = new IdentifierExpression(Pattern.AnyString).WithName("target"),
				MemberName = "Task"
			}
		};

		sealed class OriginalNodeAnnotation
		{
			internal readonly AstNode sourceNode;

			internal OriginalNodeAnnotation(AstNode sourceNode) {
				this.sourceNode = sourceNode;
			}
		}

		static void AddOriginalNodeAnnotations(AstNode currentFunction)
		{
			foreach (var nodeToAnnotate in currentFunction
			         .DescendantNodesAndSelf(MayHaveChildrenToAnnotate)
			         .Where(ShouldAnnotate)) {
				nodeToAnnotate.AddAnnotation(new OriginalNodeAnnotation(nodeToAnnotate));
			}
		}

		static void RemoveOriginalNodeAnnotations(AstNode currentFunction)
		{
			foreach (var nodeToAnnotate in currentFunction
			         .DescendantNodesAndSelf(MayHaveChildrenToAnnotate)
			         .Where(ShouldAnnotate)) {
				nodeToAnnotate.RemoveAnnotations<OriginalNodeAnnotation>();
			}
		}

		static bool MayHaveChildrenToAnnotate(AstNode node)
		{
			return node is Statement ||
				node is Expression ||
				node is MethodDeclaration;
		}

		static bool ShouldAnnotate(AstNode node)
		{
			return node is InvocationExpression;
		}

		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			if (!context.Supports(new Version(5, 0))) {
				//Old C# version -- async/await are not available
				return null;
			}
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<AutoAsyncIssue>
		{
			internal GatherVisitor(BaseRefactoringContext ctx)
			: base(ctx) {}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				AddIssueFor(methodDeclaration);
				base.VisitMethodDeclaration(methodDeclaration);
			}

			public override void VisitLambdaExpression(LambdaExpression lambdaExpression)
			{
				AddIssueFor(lambdaExpression);
				base.VisitLambdaExpression(lambdaExpression);
			}

			public override void VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression)
			{
				AddIssueFor(anonymousMethodExpression);
				base.VisitAnonymousMethodExpression(anonymousMethodExpression);
			}

			void AddIssueFor(AstNode currentFunction)
			{
				if (IsAsync(currentFunction))
					return;

				//Only suggest modifying functions that return void, Task or Task<T>.
				IType returnType = GetReturnType(ctx, currentFunction);
				if (returnType == null) {
					return;
				}

				bool isVoid = false;
				IType resultType = null;
				switch (returnType.FullName) {
					case "System.Void":
						isVoid = true;
						break;
					case "System.Threading.Tasks.Task":
						resultType = returnType.IsParameterized ? returnType.TypeArguments.FirstOrDefault() : null;
						break;
					default:
						return;
				}

				var functionBody = currentFunction.GetChildByRole(Roles.Body);
				var statements = GetStatements(functionBody).ToList();
				var returnStatements = statements.OfType<ReturnStatement>().ToList();

				var invocations = new List<InvocationExpression>();
				var nextInChain = new Dictionary<InvocationExpression, InvocationExpression>();
				foreach (var invocation in currentFunction.Descendants.OfType<InvocationExpression>()) {
					if (invocation.Arguments.Count != 1)
						continue;

					var lambdaOrDelegate = invocation.Arguments.Single();
					Statement lambdaBody;
					if (lambdaOrDelegate is LambdaExpression) {
						lambdaBody = lambdaOrDelegate.GetChildByRole(LambdaExpression.BodyRole) as BlockStatement;
						if (lambdaBody == null) {
							continue;
						}
					} else if (lambdaOrDelegate is AnonymousMethodExpression) {
						lambdaBody = lambdaOrDelegate.GetChildByRole(Roles.Body);
					} else {
						continue;
					}

					var resolveResult = ctx.Resolve(invocation) as MemberResolveResult;
					if (resolveResult == null) {
						continue;
					}
					if (resolveResult.Member.FullName != "System.Threading.Tasks.Task.ContinueWith")
						continue;

					var parentExpression = invocation.Parent as Expression;
					if (parentExpression != null) {
						var mreParent = parentExpression as MemberReferenceExpression;
						if (mreParent == null || mreParent.MemberName != "ContinueWith") {
							continue;
						}

						var parentInvocation = mreParent.Parent as InvocationExpression;
						if (parentInvocation == null || parentInvocation.Arguments.Count != 1) {
							continue;
						}

						nextInChain[invocation] = parentInvocation;
					}

					invocations.Add(invocation);
				}

				if (isVoid && invocations.Count == 0) {
					//Prevent functions like void Foo() {} from being accepted
					return;
				}

				string taskCompletionSourceIdentifier = null;
				InvocationExpression returnedContinuation = null;

				if (isVoid) {
					if (returnStatements.Any())
						return;
				} else if (!isVoid) {
					if (returnStatements.Count() != 1)
						return;

					var returnStatement = returnStatements.Single();
					if (functionBody.Statements.Last() != returnStatement)
						return;

					var match = ReturnTaskCompletionSourcePattern.Match(returnStatement);
					if (match.Success) {
						var taskCompletionSource = match.Get<IdentifierExpression>("target").Single();
						var taskCompletionSourceResolveResult = ctx.Resolve(taskCompletionSource);

						//Make sure the TaskCompletionSource is a local variable
						if (!(taskCompletionSourceResolveResult is LocalResolveResult) ||
						    taskCompletionSourceResolveResult.Type.FullName != "System.Threading.Tasks.TaskCompletionSource") {

							return;
						}
						taskCompletionSourceIdentifier = taskCompletionSource.Identifier;

						var cfgBuilder = new ControlFlowGraphBuilder();
						var cachedControlFlowGraphs = new Dictionary<BlockStatement, IList<ControlFlowNode>>();

						//Make sure there are no unsupported uses of the task completion source
						foreach (var identifier in functionBody.Descendants.OfType<Identifier>()) {
							if (identifier.Name != taskCompletionSourceIdentifier)
								continue;

							var statement = identifier.GetParent<Statement>();
							var variableStatement = statement as VariableDeclarationStatement;
							if (variableStatement != null) {
								if (functionBody.Statements.First() != variableStatement || variableStatement.Variables.Count != 1) {
									//This may actually be valid, but it would add even more complexity to this action
									return;
								}
								var initializer = variableStatement.Variables.First().Initializer as ObjectCreateExpression;
								if (initializer == null || initializer.Arguments.Count != 0 || !initializer.Initializer.IsNull) {
									return;
								}

								var constructedType = ctx.ResolveType(initializer.Type);
								if (constructedType.FullName != "System.Threading.Tasks.TaskCompletionSource") {
									return;
								}

								continue;
							}

							if (statement == returnStatement)
								continue;

							if (identifier.Parent is MemberReferenceExpression) {
								//Right side of the member.
								//We don't care about this case since it's not a reference to the variable.
								continue;
							}

							//The method's taskCompletionSource can only be used on the left side of a member
							//reference expression (specifically tcs.SetResult).
							var identifierExpressionParent = identifier.Parent as IdentifierExpression;
							if (identifierExpressionParent == null) {
								return;
							}
							var memberReferenceExpression = identifierExpressionParent.Parent as MemberReferenceExpression;
							if (memberReferenceExpression == null) {
								return;
							}

							if (memberReferenceExpression.MemberName != "SetResult") {
								//Aside from the final return statement, the only member of task completion source
								//that can be used is SetResult.
								//Perhaps future versions could also include SetException and SetCancelled.
								return;
							}

							//We found a SetResult -- we will now find out if it is in a proper context
							AstNode node = memberReferenceExpression;
							for (;;) {
								node = node.Parent;

								if (node == null) {
									//Abort since this is unexpected (it should never happen)
									return;
								}

								if (node is MethodDeclaration) {
									//Ok -- tcs.SetResult is in method declaration
									break;
								}

								if (node is LambdaExpression || node is AnonymousMethodExpression) {
									//It's time to verify if the lambda is supported
									var lambdaParent = node.Parent as InvocationExpression;
									if (lambdaParent == null || !invocations.Contains(lambdaParent)) {
										return;
									}
									break;
								}
							}

							var containingContinueWith = node.Parent as InvocationExpression;
							if (containingContinueWith != null) {
								if (nextInChain.ContainsKey(containingContinueWith)) {
									//Unsupported: ContinueWith has a SetResult
									//but it's not the last in the chain
									return;
								}
							}

							var containingFunctionBlock = node is LambdaExpression ? (BlockStatement)node.GetChildByRole(LambdaExpression.BodyRole) : node.GetChildByRole(Roles.Body);

							//Finally, tcs.SetResult must be at the end of its method
							IList<ControlFlowNode> nodes;
							if (!cachedControlFlowGraphs.TryGetValue(containingFunctionBlock, out nodes)) {
								nodes = cfgBuilder.BuildControlFlowGraph(containingFunctionBlock, ctx.CancellationToken);
								cachedControlFlowGraphs[containingFunctionBlock] = nodes;
							}

							var setResultNode = nodes.FirstOrDefault(candidateNode => candidateNode.PreviousStatement == statement);
							if (setResultNode != null && HasReachableNonReturnNodes(setResultNode)) {
								//The only allowed outgoing nodes are return statements
								return;
							}
						}
					} else {
						//Not TaskCompletionSource-based
						//Perhaps it is return Task.ContinueWith(foo);

						if (!invocations.Any()) {
							return;
						}

						var outerMostInvocations = new List<InvocationExpression>();
						InvocationExpression currentInvocation = invocations.First();
						do {
							outerMostInvocations.Add(currentInvocation);
						} while (nextInChain.TryGetValue(currentInvocation, out currentInvocation));

						var lastInvocation = outerMostInvocations.Last();
						if (returnStatement.Expression != lastInvocation) {
							return;
						}

						//Found return <1>.ContinueWith(<2>);
						returnedContinuation = lastInvocation;
					}
				}

				//We do not support "return expr" in continuations
				//The only exception is when the outer method returns that continuation.
				invocations.RemoveAll(invocation => invocation != returnedContinuation &&
				                      invocation.Arguments.First().Children.OfType<Statement>().First().DescendantNodesAndSelf(node => node is Statement).OfType<ReturnStatement>().Any(returnStatement => !returnStatement.Expression.IsNull));

				AddIssue(new CodeIssue(GetFunctionToken(currentFunction),
				         ctx.TranslateString("Function can be converted to C# 5-style async function"),
				         ctx.TranslateString("Convert to C# 5-style async function"),
				                            script => {
					AddOriginalNodeAnnotations(currentFunction);
					var newFunction = currentFunction.Clone();
					RemoveOriginalNodeAnnotations(currentFunction);

					//Set async
					var lambda = newFunction as LambdaExpression;
					if (lambda != null)
						lambda.IsAsync = true;
					var anonymousMethod = newFunction as AnonymousMethodExpression;
					if (anonymousMethod != null)
						anonymousMethod.IsAsync = true;
					var methodDeclaration = newFunction as MethodDeclaration;
					if (methodDeclaration != null)
						methodDeclaration.Modifiers |= Modifiers.Async;

					TransformBody(invocations, isVoid, resultType != null, returnedContinuation, taskCompletionSourceIdentifier, newFunction.GetChildByRole(Roles.Body));

					script.Replace(currentFunction, newFunction);
					}));
			}

			void TransformBody(List<InvocationExpression> validInvocations, bool isVoid, bool isParameterizedTask, InvocationExpression returnedContinuation, string taskCompletionSourceIdentifier, BlockStatement blockStatement)
			{
				if (!isVoid) {
					if (returnedContinuation == null) {
						//Is TaskCompletionSource-based
						blockStatement.Statements.First().Remove(); //Remove task completion source declaration
						blockStatement.Statements.Last().Remove(); //Remove final return
					}

					//We use ToList() because we will be modifying the original collection
					foreach (var expressionStatement in blockStatement.Descendants.OfType<ExpressionStatement>().ToList()) {
						var invocationExpression = expressionStatement.Expression as InvocationExpression;
						if (invocationExpression == null || invocationExpression.Arguments.Count != 1)
							continue;

						var target = invocationExpression.Target as MemberReferenceExpression;
						if (target == null || target.MemberName != "SetResult") {
							continue;
						}
						var targetExpression = target.Target as IdentifierExpression;
						if (targetExpression == null || targetExpression.Identifier != taskCompletionSourceIdentifier) {
							continue;
						}

						var returnedExpression = invocationExpression.Arguments.Single();
						returnedExpression.Remove();

						var originalInvocation = (InvocationExpression) invocationExpression.Annotation<OriginalNodeAnnotation>().sourceNode;
						var originalReturnedExpression = originalInvocation.Arguments.Single();
						var argumentType = ctx.Resolve(originalReturnedExpression).Type;

						if (!isParameterizedTask) {
							var parent = expressionStatement.Parent;
							var resultIdentifier = CreateVariableName(blockStatement, "result");

							var blockParent = parent as BlockStatement;
							var resultDeclarationType = argumentType == SpecialType.NullType ? new PrimitiveType("object") : CreateShortType(originalInvocation, argumentType);
							var declaration = new VariableDeclarationStatement(resultDeclarationType, resultIdentifier, returnedExpression);
							if (blockParent == null) {
								var newStatement = new BlockStatement();
								newStatement.Add(declaration);
								newStatement.Add(new ReturnStatement());
								expressionStatement.ReplaceWith(newStatement);
							} else {
								blockParent.Statements.InsertAfter(expressionStatement, new ReturnStatement());
								expressionStatement.ReplaceWith(declaration);
							}
						} else {
							var newStatement = new ReturnStatement(returnedExpression);
							expressionStatement.ReplaceWith(newStatement);
						}
					}
				}

				//Find all instances of ContinueWith to replace and associated 
				var continuations = new List<Tuple<InvocationExpression, InvocationExpression, string>>();
				foreach (var invocation in blockStatement.Descendants.OfType<InvocationExpression>()) {
					if (invocation.Arguments.Count != 1)
						continue;

					var originalInvocation = (InvocationExpression) invocation.Annotation<OriginalNodeAnnotation>().sourceNode;
					if (!validInvocations.Contains(originalInvocation))
						continue;

					var lambda = invocation.Arguments.Single();

					string associatedTaskName = null;

					var lambdaParameters = lambda.GetChildrenByRole(Roles.Parameter).Select(p => p.Name).ToList();
					var lambdaTaskParameterName = lambdaParameters.FirstOrDefault();
					if (lambdaTaskParameterName != null) {
						associatedTaskName = lambdaTaskParameterName;
					}

					continuations.Add(Tuple.Create(invocation, originalInvocation, associatedTaskName));
				}

				foreach (var continuationTuple in continuations) {
					string taskName = continuationTuple.Item3 ?? "task";
					string effectiveTaskName = CreateVariableName(blockStatement, taskName);
					string resultName = CreateVariableName(blockStatement, taskName + "Result");
					var continuation = continuationTuple.Item1;
					var originalInvocation = continuationTuple.Item2;

					var target = continuation.Target.GetChildByRole(Roles.TargetExpression).Detach();
					var awaitedExpression = new UnaryOperatorExpression(UnaryOperatorType.Await, target);
					var replacements = new List<Statement>();
					var lambdaExpression = originalInvocation.Arguments.First();
					var continuationLambdaResolveResult = (LambdaResolveResult) ctx.Resolve(lambdaExpression);

					if (!continuationLambdaResolveResult.HasParameterList)
					{
						//Lambda has no parameter, so creating a variable for the argument is not needed
						// (since you can't use an argument that doesn't exist).
						replacements.Add(new ExpressionStatement(awaitedExpression));
					} else {
						//Lambda has a parameter, which can either be a Task or a Task<T>.

						var lambdaParameter = continuationLambdaResolveResult.Parameters[0];
						bool isTaskIdentifierUsed = lambdaExpression.Descendants.OfType<IdentifierExpression>().Any(identifier => {
							if (identifier.Identifier != lambdaParameter.Name)
								return false;
							var identifierMre = identifier.Parent as MemberReferenceExpression;
							return identifierMre == null || identifierMre.MemberName != "Result";
						});

						var precedentTaskType = lambdaParameter.Type;

						//We might need to separate the task creation and awaiting
						if (isTaskIdentifierUsed) {
							//Create new task variable
							var taskExpression = awaitedExpression.Expression;
							taskExpression.Detach();
							replacements.Add(new VariableDeclarationStatement(CreateShortType(lambdaExpression, precedentTaskType),
							                                                  effectiveTaskName,
							                                                  taskExpression));
							awaitedExpression.Expression = new IdentifierExpression(effectiveTaskName);
						}

						if (precedentTaskType.IsParameterized) {
							//precedent is Task<T>
							var precedentResultType = precedentTaskType.TypeArguments.First();
							replacements.Add(new VariableDeclarationStatement(CreateShortType(originalInvocation, precedentResultType), resultName, awaitedExpression));
						} else {
							//precedent is Task
							replacements.Add(awaitedExpression);
						}
					}

					var parentStatement = continuation.GetParent<Statement>();
					var grandParentStatement = parentStatement.Parent;
					var block = grandParentStatement as BlockStatement;
					if (block == null) {
						block = new BlockStatement();
						block.Statements.AddRange(replacements);
						parentStatement.ReplaceWith(block);
					} else {
						foreach (var replacement in replacements) {
							block.Statements.InsertBefore(parentStatement, replacement);
						}
						parentStatement.Remove();
					}

					var lambdaOrDelegate = continuation.Arguments.Single();
					Statement lambdaContent;
					if (lambdaOrDelegate is LambdaExpression) {
						lambdaContent = (Statement)lambdaOrDelegate.GetChildByRole(LambdaExpression.BodyRole);
					} else {
						lambdaContent = lambdaOrDelegate.GetChildByRole(Roles.Body);
					}

					foreach (var identifierExpression in lambdaContent.Descendants.OfType<IdentifierExpression>()) {
						if (continuationTuple.Item3 != identifierExpression.Identifier) {
							continue;
						}

						var memberReference = identifierExpression.Parent as MemberReferenceExpression;

						if (memberReference == null || memberReference.MemberName != "Result") {
							identifierExpression.ReplaceWith(new IdentifierExpression(effectiveTaskName));
							continue;
						}

						memberReference.ReplaceWith(new IdentifierExpression(resultName));
					}

					if (lambdaContent is BlockStatement) {
						Statement previousStatement = replacements.Last();
						foreach (var statementInContinuation in lambdaContent.GetChildrenByRole(BlockStatement.StatementRole)) {
							statementInContinuation.Detach();
							block.Statements.InsertAfter(previousStatement, statementInContinuation);
							previousStatement = statementInContinuation;
						}
					} else {
						lambdaContent.Detach();
						block.Statements.InsertAfter(replacements.Last(), lambdaContent);
					}
				}
			}

			AstType CreateShortType(AstNode node, IType type)
			{
				return ctx.CreateTypeSystemAstBuilder(node).ConvertType(type);
			}
		}
		
		static string CreateVariableName(AstNode currentRootNode, string proposedName) {
			var identifiers = currentRootNode.Descendants.OfType<Identifier>()
				.Select(identifier => identifier.Name).Where(identifier => identifier.StartsWith(proposedName, StringComparison.InvariantCulture)).ToList();
			for (int i = 0;; ++i) {
				string name = proposedName + (i == 0 ? string.Empty : i.ToString());
				if (!identifiers.Contains(name)) {
					return name;
				}
			}
		}

		static bool HasReachableNonReturnNodes(ControlFlowNode firstNode)
		{
			var visitedNodes = new List<ControlFlowNode>();
			var nodesToVisit = new HashSet<ControlFlowNode>();
			nodesToVisit.Add(firstNode);

			while (nodesToVisit.Any()) {
				var node = nodesToVisit.First();
				nodesToVisit.Remove(node);
				visitedNodes.Add(node);

				if (node.Type == ControlFlowNodeType.LoopCondition)
					return true;

				var nextStatement = node.NextStatement;
				if (nextStatement != null) {
					if (!(nextStatement is ReturnStatement ||
					    nextStatement is GotoStatement ||
					    nextStatement is GotoCaseStatement ||
					    nextStatement is GotoDefaultStatement ||
					    nextStatement is ContinueStatement ||
					    nextStatement is BreakStatement)) {

						return true;
					}
				}
			}

			return false;
		}

		static IType GetReturnType(BaseRefactoringContext context, AstNode currentFunction)
		{
			var resolveResult = context.Resolve(currentFunction);
			return resolveResult.IsError ? null : resolveResult.Type;
		}

		static IEnumerable<Statement> GetStatements(Statement statement)
		{
			return statement.DescendantNodesAndSelf(stmt => stmt is Statement).OfType<Statement>();
		}

		static AstNode GetFunctionToken(AstNode currentFunction)
		{
			return (AstNode)currentFunction.GetChildByRole(Roles.Identifier) ??
			currentFunction.GetChildByRole(LambdaExpression.ArrowRole) ??
			currentFunction.GetChildByRole(AnonymousMethodExpression.DelegateKeywordRole);
		}

		static bool IsAsync(AstNode currentFunction)
		{
			var method = currentFunction as MethodDeclaration;
			if (method != null) {
				return method.HasModifier(Modifiers.Async);
			}
			return !currentFunction.GetChildByRole(LambdaExpression.AsyncModifierRole).IsNull;
		}
	}
}

