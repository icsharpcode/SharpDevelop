// 
// RedundantAssignmentIssue.cs
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
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Redundant assignment",
	                  Description = "Value assigned to a variable or parameter is not used in all execution path.",
	                  Category = IssueCategories.CodeQualityIssues,
	                  Severity = Severity.Warning)]
	public class RedundantAssignmentIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			var unit = context.RootNode as SyntaxTree;
			if (unit == null)
				return null;
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantAssignmentIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
			}

			public override void VisitParameterDeclaration(ParameterDeclaration parameterDeclaration)
			{
				base.VisitParameterDeclaration(parameterDeclaration);
				if (parameterDeclaration.ParameterModifier == ParameterModifier.Out ||
					parameterDeclaration.ParameterModifier == ParameterModifier.Ref)
					return;

				var resolveResult = ctx.Resolve(parameterDeclaration) as LocalResolveResult;
				BlockStatement rootStatement = null;
				if (parameterDeclaration.Parent is MethodDeclaration) {
					rootStatement = ((MethodDeclaration)parameterDeclaration.Parent).Body;
				} else if (parameterDeclaration.Parent is AnonymousMethodExpression) {
					rootStatement = ((AnonymousMethodExpression)parameterDeclaration.Parent).Body;
				} else if (parameterDeclaration.Parent is LambdaExpression) {
					rootStatement = ((LambdaExpression)parameterDeclaration.Parent).Body as BlockStatement;
				}
				CollectIssues(parameterDeclaration, rootStatement, resolveResult);
			}

			public override void VisitVariableInitializer(VariableInitializer variableInitializer)
			{
				base.VisitVariableInitializer(variableInitializer);
				if (!inUsingStatementResourceAcquisition) {
					var resolveResult = ctx.Resolve(variableInitializer) as LocalResolveResult;
					CollectIssues(variableInitializer, variableInitializer.GetParent<BlockStatement>(), resolveResult);
				}
			}

			bool inUsingStatementResourceAcquisition;

			public override void VisitUsingStatement(UsingStatement usingStatement)
			{
				inUsingStatementResourceAcquisition = true;
				usingStatement.ResourceAcquisition.AcceptVisitor(this);
				inUsingStatementResourceAcquisition = false;
				usingStatement.EmbeddedStatement.AcceptVisitor(this);
			}

			void CollectIssues(AstNode variableDecl, BlockStatement rootStatement, LocalResolveResult resolveResult)
			{
				if (rootStatement == null || resolveResult == null)
					return;

				var references = new HashSet<AstNode>();
				var refStatements = new HashSet<Statement>();
				var usedInLambda = false;
				var results = ctx.FindReferences(rootStatement, resolveResult.Variable);
				foreach (var result in results) {
					var node = result.Node;
					if (node == variableDecl)
						continue;

					var parent = node.Parent;
					while (!(parent == null || parent is Statement || parent is LambdaExpression || parent is QueryExpression))
						parent = parent.Parent;
					if (parent == null)
						continue;

					var statement = parent as Statement;
					if (statement != null) {
						references.Add(node);
						refStatements.Add(statement);
					}

					while (parent != null && parent != rootStatement) {
						if (parent is LambdaExpression || parent is AnonymousMethodExpression || parent is QueryExpression) {
							usedInLambda = true;
							break;
						}
						parent = parent.Parent;
					}
					if (usedInLambda) {
						break;
					}
				}

				// stop analyzing if the variable is used in any lambda expression or anonymous method
				if (usedInLambda)
					return;

				var startNode = new VariableReferenceGraphBuilder(ctx).Build(rootStatement, references, refStatements, ctx);
				var variableInitializer = variableDecl as VariableInitializer;
				if (variableInitializer != null && !variableInitializer.Initializer.IsNull)
					startNode.References.Insert(0, variableInitializer);

				ProcessNodes(startNode);
			}

			class SearchInvocationsVisitor : DepthFirstAstVisitor
			{
				bool foundInvocations;

				public bool ContainsInvocations(AstNode node)
				{
					foundInvocations = false;
					node.AcceptVisitor(this);
					return foundInvocations;
				}

				protected override void VisitChildren(AstNode node)
				{
					AstNode next;
					for (var child = node.FirstChild; child != null && !foundInvocations; child = next) {
						next = child.NextSibling;
						child.AcceptVisitor(this);
					}
				}

				public override void VisitInvocationExpression(InvocationExpression invocationExpression)
				{
					foundInvocations = true;
				}
			}

			private class SearchRefOrOutVisitor : DepthFirstAstVisitor
			{
				private bool foundInvocations;
				private string _varName;

				public bool ContainsRefOrOut(VariableInitializer variableInitializer)
				{
					var node = variableInitializer.Parent.Parent;
					foundInvocations = false;
					_varName = variableInitializer.Name;
					node.AcceptVisitor(this);
					return foundInvocations;
				}

				protected override void VisitChildren(AstNode node)
				{
					AstNode next;
					for (var child = node.FirstChild; child != null && !foundInvocations; child = next) {
						next = child.NextSibling;
						child.AcceptVisitor(this);
					}
				}

				public override void VisitInvocationExpression(InvocationExpression methodDeclaration)
				{
					if (foundInvocations)
						return;
					if (methodDeclaration.Arguments.Count == 0)
						return;
					foreach (var argument in methodDeclaration.Arguments) {
						var directionExpression = argument as DirectionExpression;
						if (directionExpression == null)
							continue;

						if (directionExpression.FieldDirection != FieldDirection.Out && directionExpression.FieldDirection != FieldDirection.Ref)
							continue;
						var idExpression = (directionExpression.Expression) as IdentifierExpression;
						if (idExpression == null)
							continue;
						foundInvocations = (idExpression.Identifier == _varName);

						foundInvocations = true;
					}
				}
			}

			class SearchAssignmentForVarVisitor : DepthFirstAstVisitor
			{
				bool _foundInvocations;
				private VariableInitializer _variableInitializer;

				public bool ContainsLaterAssignments(VariableInitializer variableInitializer)
				{
					_foundInvocations = false;
					_variableInitializer = variableInitializer;
					variableInitializer.Parent.Parent.AcceptVisitor(this);
					return _foundInvocations;
				}

				protected override void VisitChildren(AstNode node)
				{
					AstNode next;
					for (var child = node.FirstChild; child != null && !_foundInvocations; child = next) {
						next = child.NextSibling;
						child.AcceptVisitor(this);
					}
				}

				public override void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
				{
					if (_foundInvocations)
						return;
					base.VisitAssignmentExpression(assignmentExpression);
					if (assignmentExpression.Left.ToString() == _variableInitializer.Name
						&& assignmentExpression.StartLocation > _variableInitializer.StartLocation) {
						_foundInvocations = true;
					}
				}
			}

			bool ContainsOtherAssignments(AstNode parent)
			{
				int count = 0;
				foreach (var child in parent.Children.OfType<VariableInitializer>()) {
					count++;
					if (count > 1)
						return true;
				}
				return false;
			}

			void AddIssue(AstNode node)
			{
				var issueDescription = ctx.TranslateString("Assignment is redundant");
				var actionDescription = ctx.TranslateString("Remove redundant assignment");

				var variableInitializer = node as VariableInitializer;
				if (variableInitializer != null) {
					var containsInvocations =
						new SearchInvocationsVisitor().ContainsInvocations(variableInitializer.Initializer);

					var varDecl = node.Parent as VariableDeclarationStatement;

					var isDeclareStatement = varDecl != null;
					var isUsingVar = isDeclareStatement && varDecl.Type.IsVar();

					var expressionType = ctx.Resolve(node).Type;

					var containsLaterAssignments = false;
					if (isDeclareStatement) {
						//if it is used later, the redundant removal should remove the assignment 
						//but not the variable
						containsLaterAssignments = 
							new SearchAssignmentForVarVisitor().ContainsLaterAssignments(variableInitializer);
					}

					AstNode grayOutNode;
					var containsRefOrOut = new SearchRefOrOutVisitor().ContainsRefOrOut(variableInitializer);
					if (containsInvocations && isDeclareStatement) {
						grayOutNode = variableInitializer.AssignToken;
					} else {
						if (isDeclareStatement && !containsRefOrOut && !containsLaterAssignments && !ContainsOtherAssignments(variableInitializer.Parent)) {
							grayOutNode = variableInitializer.Parent;
						} else {
							grayOutNode = variableInitializer.Initializer;
						}
					}

					AddIssue(new CodeIssue(grayOutNode, issueDescription, actionDescription, script => {
						var variableNode = (VariableInitializer)node;
						if (containsInvocations && isDeclareStatement) {
							//add the column ';' that will be removed after the next line replacement
							var expression = (Statement)variableNode.Initializer.Clone();
							if (containsLaterAssignments && varDecl != null) {
								var clonedDefinition = (VariableDeclarationStatement)varDecl.Clone();

								var shortExpressionType = CreateShortType(ctx, expressionType, node);
								clonedDefinition.Type = shortExpressionType;
								var variableNodeClone = clonedDefinition.GetVariable(variableNode.Name);
								variableNodeClone.Initializer = null;
								script.InsertBefore(node.Parent, clonedDefinition);
							}
							script.Replace(node.Parent, expression);
							return;
						}
						if (isDeclareStatement && !containsRefOrOut && !containsLaterAssignments&& !ContainsOtherAssignments(variableInitializer.Parent)) {
							script.Remove(node.Parent);
							return;
						}
						var replacement = (VariableInitializer)variableNode.Clone();
						replacement.Initializer = Expression.Null;
						if (isUsingVar) {
							var shortExpressionType = CreateShortType(ctx, expressionType, node);
							script.Replace(varDecl.Type, shortExpressionType);
						}
						script.Replace(node, replacement);
					}) { IssueMarker = IssueMarker.GrayOut });
				}

				var assignmentExpr = node.Parent as AssignmentExpression;
				if (assignmentExpr == null)
					return;
				if (assignmentExpr.Parent is ExpressionStatement) {
					AddIssue(new CodeIssue(assignmentExpr.Parent, issueDescription, actionDescription, script => script.Remove(assignmentExpr.Parent)) { IssueMarker = IssueMarker.GrayOut });
				} else {
					AddIssue(new CodeIssue(assignmentExpr.Left.StartLocation, assignmentExpr.OperatorToken.EndLocation, issueDescription, actionDescription,
						script => script.Replace(assignmentExpr, assignmentExpr.Right.Clone())) { IssueMarker = IssueMarker.GrayOut });
				}
			}

			private static AstType CreateShortType(BaseRefactoringContext refactoringContext, IType expressionType, AstNode node)
			{

				var csResolver = refactoringContext.Resolver.GetResolverStateBefore(node);
				var builder = new TypeSystemAstBuilder(csResolver);
				return builder.ConvertType(expressionType);
			}

			static bool IsAssignment(AstNode node)
			{
				if (node is VariableInitializer)
					return true;

				var assignmentExpr = node.Parent as AssignmentExpression;
				if (assignmentExpr != null)
					return assignmentExpr.Left == node && assignmentExpr.Operator == AssignmentOperatorType.Assign;

				var direction = node.Parent as DirectionExpression;
				if (direction != null)
					return direction.FieldDirection == FieldDirection.Out && direction.Expression == node;

				return false;
			}

			static bool IsInsideTryOrCatchBlock(AstNode node)
			{
				var tryCatchStatement = node.GetParent<TryCatchStatement>();
				if (tryCatchStatement != null) {
					if (tryCatchStatement.TryBlock.Contains(node.StartLocation))
						return true;
					foreach (var catchBlock in tryCatchStatement.CatchClauses)
						if (catchBlock.Body.Contains(node.StartLocation))
							return true;
				}
				return false;
			}

			enum NodeState
			{
				None,
				UsageReachable,
				UsageUnreachable,
				Processing,
			}

			void ProcessNodes(VariableReferenceNode startNode)
			{
				// node state of a node indicates whether it is possible for an upstream node to reach any usage via
				// the node
				var nodeStates = new Dictionary<VariableReferenceNode, NodeState>();
				var assignments = new List<VariableReferenceNode>();

				// dfs to preprocess all nodes and find nodes which end with assignment
				var stack = new Stack<VariableReferenceNode>();
				stack.Push(startNode);
				while (stack.Count > 0) {
					var node = stack.Pop();
					if (node == null)
						continue;
					if (node.References.Count > 0) {
						nodeStates [node] = IsAssignment(node.References [0]) ?
							NodeState.UsageUnreachable : NodeState.UsageReachable;
					} else {
						nodeStates [node] = NodeState.None;
					}

					// find indices of all assignments in node.References
					var assignmentIndices = new List<int>();
					for (int i = 0; i < node.References.Count; i++) {
						if (IsAssignment(node.References [i]))
							assignmentIndices.Add(i);
					}
					// for two consecutive assignments, the first one is redundant
					for (int i = 0; i < assignmentIndices.Count - 1; i++) {
						var index1 = assignmentIndices [i];
						var index2 = assignmentIndices [i + 1];
						if (index1 + 1 == index2)
							AddIssue(node.References [index1]);
					}
					// if the node ends with an assignment, add it to assignments so as to check if it is redundant
					// later
					if (assignmentIndices.Count > 0 &&
						assignmentIndices [assignmentIndices.Count - 1] == node.References.Count - 1)
						assignments.Add(node);

					foreach (var nextNode in node.NextNodes) {
						if (!nodeStates.ContainsKey(nextNode))
							stack.Push(nextNode);
					}
				}

				foreach (var node in assignments) {
					// we do not analyze an assignment inside a try block as it can jump to any catch block or finally block
					if (IsInsideTryOrCatchBlock(node.References [0]))
						continue;
					ProcessNode(node, true, nodeStates);
				}
			}

			void ProcessNode(VariableReferenceNode node, bool addIssue,
			                 IDictionary<VariableReferenceNode, NodeState> nodeStates)
			{
				if (nodeStates [node] == NodeState.None)
					nodeStates [node] = NodeState.Processing;

				bool? reachable = false;
				foreach (var nextNode in node.NextNodes) {
					if (nodeStates [nextNode] == NodeState.None)
						ProcessNode(nextNode, false, nodeStates);

					if (nodeStates [nextNode] == NodeState.UsageReachable) {
						reachable = true;
						break;
					}
					// downstream nodes are not fully processed (e.g. due to loop), there is no enough info to
					// determine the node state
					if (nodeStates [nextNode] != NodeState.UsageUnreachable)
						reachable = null;
				}

				// add issue if it is not possible to reach any usage via NextNodes
				if (addIssue && reachable == false)
					AddIssue(node.References [node.References.Count - 1]);

				if (nodeStates [node] != NodeState.Processing)
					return;

				switch (reachable) {
					case null:
						nodeStates [node] = NodeState.None;
						break;
					case true:
						nodeStates [node] = NodeState.UsageReachable;
						break;
					case false:
						nodeStates [node] = NodeState.UsageUnreachable;
						break;
				}
			}
		}
	}
}