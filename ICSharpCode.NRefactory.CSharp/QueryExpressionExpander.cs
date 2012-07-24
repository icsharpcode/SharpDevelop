using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.CSharp {
	public class QueryExpressionExpansionResult {
		public AstNode AstNode { get; private set; }

		/// <summary>
		/// Maps original range variables to some node in the new tree that represents them.
		/// </summary>
		public IDictionary<Identifier, Identifier> RangeVariables { get; private set; }

		/// <summary>
		/// Maps clauses to method calls. The keys will always be either a <see cref="QueryClause"/> or a <see cref="QueryOrdering"/>
		/// </summary>
		public IDictionary<AstNode, Expression> Expressions { get; private set; }

		public QueryExpressionExpansionResult(AstNode astNode, IDictionary<Identifier, Identifier> rangeVariables, IDictionary<AstNode, Expression> expressions) {
			AstNode = astNode;
			RangeVariables = rangeVariables;
			Expressions = expressions;
		}
	}

	public class QueryExpressionExpander {
		class Visitor : IAstVisitor<AstNode> {
			int currentTransparentParameter;
			const string TransparentParameterNameTemplate = "<>x{0}";

			AstNode Default(AstNode node) {
				List<AstNode> newChildren = null;
				
				int i = 0;
				foreach (var child in node.Children) {
					var newChild = child.AcceptVisitor(this);
					if (newChild != null) {
						newChildren = newChildren ?? Enumerable.Repeat((AstNode)null, i).ToList();
						newChildren.Add(newChild);
					}
					else if (newChildren != null) {
						newChildren.Add(null);
					}
					i++;
				}

				if (newChildren == null)
					return null;

				var result = node.Clone();

				i = 0;
				foreach (var children in result.Children) {
					if (newChildren[i] != null)
						children.ReplaceWith(newChildren[i]);
					i++;
				}

				return result;
			}

			Expression MakeNestedMemberAccess(Expression target, IEnumerable<string> members) {
				return members.Aggregate(target, (current, m) => current.Member(m));
			}

			Expression VisitNested(Expression node, ParameterDeclaration transparentParameter) {
				var oldRangeVariableSubstitutions = activeRangeVariableSubstitutions;
				try {
					if (transparentParameter != null && currentTransparentType.Count > 1) {
						activeRangeVariableSubstitutions = new Dictionary<string, Expression>(activeRangeVariableSubstitutions);
						foreach (var t in currentTransparentType)
							activeRangeVariableSubstitutions[t.Item1.Name] = MakeNestedMemberAccess(new IdentifierExpression(transparentParameter.Name), t.Item2);
					}
					var result = node.AcceptVisitor(this);
					return (Expression)(result ?? node.Clone());
				}
				finally {
					activeRangeVariableSubstitutions = oldRangeVariableSubstitutions;
				}
			}

			QueryClause GetNextQueryClause(QueryClause clause) {
				for (AstNode node = clause.NextSibling; node != null; node = node.NextSibling) {
					if (node.Role == QueryExpression.ClauseRole)
						return (QueryClause)node;
				}
				return null;
			}

			public IDictionary<Identifier, Identifier> rangeVariables = new Dictionary<Identifier, Identifier>();
			public IDictionary<AstNode, Expression> expressions = new Dictionary<AstNode, Expression>();

			Dictionary<string, Expression> activeRangeVariableSubstitutions = new Dictionary<string, Expression>();
			List<Tuple<Identifier, List<string>>> currentTransparentType = new List<Tuple<Identifier, List<string>>>();
			Expression currentResult;
			bool eatSelect;

			void MapExpression(AstNode orig, Expression newExpr) {
				Debug.Assert(orig is QueryClause || orig is QueryOrdering);
				expressions[orig] = newExpr;
			}

			ParameterDeclaration CreateParameterForCurrentRangeVariable() {
				var param = new ParameterDeclaration();

				if (currentTransparentType.Count == 1) {
					var clonedRangeVariable = (Identifier)currentTransparentType[0].Item1.Clone();
					if (!rangeVariables.ContainsKey(currentTransparentType[0].Item1))
						rangeVariables[currentTransparentType[0].Item1] = clonedRangeVariable;
					param.AddChild(clonedRangeVariable, Roles.Identifier);
				}
				else {
					param.AddChild(Identifier.Create(string.Format(CultureInfo.InvariantCulture, TransparentParameterNameTemplate, currentTransparentParameter++)), Roles.Identifier);
				}
				return param;
			}

			LambdaExpression CreateLambda(IList<ParameterDeclaration> parameters, Expression body) {
				var result = new LambdaExpression();
				if (parameters.Count > 1)
					result.AddChild(new CSharpTokenNode(TextLocation.Empty), Roles.LPar);
				result.AddChild(parameters[0], Roles.Parameter);
				for (int i = 1; i < parameters.Count; i++) {
					result.AddChild(new CSharpTokenNode(TextLocation.Empty), Roles.Comma);
					result.AddChild(parameters[i], Roles.Parameter);
				}
				if (parameters.Count > 1)
					result.AddChild(new CSharpTokenNode(TextLocation.Empty), Roles.RPar);
				result.AddChild(body, LambdaExpression.BodyRole);

				return result;
			}

			ParameterDeclaration CreateParameter(Identifier identifier) {
				var result = new ParameterDeclaration();
				result.AddChild(identifier, Roles.Identifier);
				return result;
			}

			Expression AddMemberToCurrentTransparentType(ParameterDeclaration param, Identifier name, Expression value, bool namedExpression) {
				Expression newAssignment = VisitNested(value, param);
				if (namedExpression) {
					newAssignment = new NamedExpression(name.Name, VisitNested(value, param));
					if (!rangeVariables.ContainsKey(name) )
						rangeVariables[name] = ((NamedExpression)newAssignment).NameToken;
				}

				foreach (var t in currentTransparentType)
					t.Item2.Insert(0, param.Name);

				currentTransparentType.Add(Tuple.Create(name, new List<string> { name.Name }));
				return new AnonymousTypeCreateExpression(new[] { new IdentifierExpression(param.Name), newAssignment });
			}

			void AddFirstMemberToCurrentTransparentType(Identifier identifier) {
				Debug.Assert(currentTransparentType.Count == 0);
				currentTransparentType.Add(Tuple.Create(identifier, new List<string>()));
			}

			AstNode IAstVisitor<AstNode>.VisitQueryExpression(QueryExpression queryExpression) {
				var oldTransparentType = currentTransparentType;
				var oldResult = currentResult;
				var oldEatSelect = eatSelect;
				try {
					currentTransparentType = new List<Tuple<Identifier, List<string>>>();
					currentResult = null;
					eatSelect = false;

					foreach (var clause in queryExpression.Clauses) {
						var result = (Expression)clause.AcceptVisitor(this);
						MapExpression(clause, result ?? currentResult);
						currentResult = result;
					}

					return currentResult; 
				}
				finally {
					currentTransparentType = oldTransparentType;
					currentResult = oldResult;
					eatSelect = oldEatSelect;
				}
			}

			AstNode IAstVisitor<AstNode>.VisitQueryContinuationClause(QueryContinuationClause queryContinuationClause) {
				var prev = VisitNested(queryContinuationClause.PrecedingQuery, null);
				AddFirstMemberToCurrentTransparentType(queryContinuationClause.IdentifierToken);
				return prev;
			}

			AstNode IAstVisitor<AstNode>.VisitQueryFromClause(QueryFromClause queryFromClause) {
				if (currentResult == null) {
					AddFirstMemberToCurrentTransparentType(queryFromClause.IdentifierToken);
					if (queryFromClause.Type.IsNull) {
						return VisitNested(queryFromClause.Expression, null);
					}
					else {
						return VisitNested(queryFromClause.Expression, null).Invoke("Cast", new[] { queryFromClause.Type.Clone() }, new Expression[0]);
					}
				}
				else {
					var innerSelectorParam = CreateParameterForCurrentRangeVariable();
					var innerSelector = CreateLambda(new[] { innerSelectorParam }, VisitNested(queryFromClause.Expression, innerSelectorParam));

					var clonedIdentifier = (Identifier)queryFromClause.IdentifierToken.Clone();

					var resultParam = CreateParameterForCurrentRangeVariable();
					Expression body;
					// Second from clause - SelectMany
					var select = GetNextQueryClause(queryFromClause) as QuerySelectClause;
					if (select != null) {
						body = VisitNested(select.Expression, resultParam);
						eatSelect = true;
					}
					else {
						body = AddMemberToCurrentTransparentType(resultParam, queryFromClause.IdentifierToken, new IdentifierExpression(queryFromClause.Identifier), false);
					}

					var resultSelector = CreateLambda(new[] { resultParam, CreateParameter(clonedIdentifier) }, body);
					rangeVariables[queryFromClause.IdentifierToken] = clonedIdentifier;

					return currentResult.Invoke("SelectMany", innerSelector, resultSelector);
				}
			}

			AstNode IAstVisitor<AstNode>.VisitQueryLetClause(QueryLetClause queryLetClause) {
				var param = CreateParameterForCurrentRangeVariable();
				var body = AddMemberToCurrentTransparentType(param, queryLetClause.IdentifierToken, queryLetClause.Expression, true);
				var lambda = CreateLambda(new[] { param }, body);

				return currentResult.Invoke("Select", lambda);
			}

			AstNode IAstVisitor<AstNode>.VisitQueryWhereClause(QueryWhereClause queryWhereClause) {
				var param = CreateParameterForCurrentRangeVariable();
				return currentResult.Invoke("Where", CreateLambda(new[] { param }, VisitNested(queryWhereClause.Condition, param)));
			}

			AstNode IAstVisitor<AstNode>.VisitQueryJoinClause(QueryJoinClause queryJoinClause) {
				Expression resultSelectorBody = null;
				var inExpression = VisitNested(queryJoinClause.InExpression, null);
				var key1SelectorFirstParam = CreateParameterForCurrentRangeVariable();
				var key1Selector = CreateLambda(new[] { key1SelectorFirstParam }, VisitNested(queryJoinClause.OnExpression, key1SelectorFirstParam));
				var key2Param = Identifier.Create(queryJoinClause.JoinIdentifier);
				var key2Selector = CreateLambda(new[] { CreateParameter(key2Param) }, VisitNested(queryJoinClause.EqualsExpression, null));

				var resultSelectorFirstParam = CreateParameterForCurrentRangeVariable();

				var select = GetNextQueryClause(queryJoinClause) as QuerySelectClause;
				if (select != null) {
					resultSelectorBody = VisitNested(select.Expression, resultSelectorFirstParam);
					eatSelect = true;
				}

				if (queryJoinClause.IntoKeyword.IsNull) {
					// Normal join
					if (resultSelectorBody == null)
						resultSelectorBody = AddMemberToCurrentTransparentType(resultSelectorFirstParam, queryJoinClause.JoinIdentifierToken, new IdentifierExpression(queryJoinClause.JoinIdentifier), false);

					var resultSelector = CreateLambda(new[] { resultSelectorFirstParam, CreateParameter(Identifier.Create(queryJoinClause.JoinIdentifier)) }, resultSelectorBody);
					rangeVariables[queryJoinClause.JoinIdentifierToken] = key2Param;
					return currentResult.Invoke("Join", inExpression, key1Selector, key2Selector, resultSelector);
				}
				else {
					// Group join
					if (resultSelectorBody == null)
						resultSelectorBody = AddMemberToCurrentTransparentType(resultSelectorFirstParam, queryJoinClause.IntoIdentifierToken, new IdentifierExpression(queryJoinClause.IntoIdentifier), false);

					var intoParam = Identifier.Create(queryJoinClause.IntoIdentifier);
					var resultSelector = CreateLambda(new[] { resultSelectorFirstParam, CreateParameter(intoParam) }, resultSelectorBody);
					rangeVariables[queryJoinClause.IntoIdentifierToken] = intoParam;

					return currentResult.Invoke("GroupJoin", inExpression, key1Selector, key2Selector, resultSelector);
				}
			}

			AstNode IAstVisitor<AstNode>.VisitQueryOrderClause(QueryOrderClause queryOrderClause) {
				var current = currentResult;
				bool first = true;
				foreach (var o in queryOrderClause.Orderings) {
					string methodName = first ? (o.Direction == QueryOrderingDirection.Descending ? "OrderByDescending" : "OrderBy")
					                          : (o.Direction == QueryOrderingDirection.Descending ? "ThenByDescending" : "ThenBy");

					var param = CreateParameterForCurrentRangeVariable();
					current = current.Invoke(methodName, CreateLambda(new[] { param }, VisitNested(o.Expression, param)));
					MapExpression(o, current);
					first = false;
				}
				return current;
			}

			AstNode IAstVisitor<AstNode>.VisitQueryOrdering(QueryOrdering queryOrdering) {
				return null;
			}

			AstNode IAstVisitor<AstNode>.VisitQuerySelectClause(QuerySelectClause querySelectClause) {
				if (eatSelect) {
					eatSelect = false;
					return currentResult;
				}
				else if (currentTransparentType.Count == 1 && ((QueryExpression)querySelectClause.Parent).Clauses.Count > 2 && querySelectClause.Expression is IdentifierExpression && ((IdentifierExpression)querySelectClause.Expression).Identifier == currentTransparentType[0].Item1.Name) {
					// A simple query that ends with a trivial select should be removed.
					return currentResult;
				}

				var param = CreateParameterForCurrentRangeVariable();
				var lambda = CreateLambda(new[] { param }, VisitNested(querySelectClause.Expression, param));
				return currentResult.Invoke("Select", lambda);
			}

			AstNode IAstVisitor<AstNode>.VisitQueryGroupClause(QueryGroupClause queryGroupClause) {
				var param = CreateParameterForCurrentRangeVariable();
				var keyLambda = CreateLambda(new[] { param }, VisitNested(queryGroupClause.Key, param));

				if (currentTransparentType.Count == 1 && queryGroupClause.Projection is IdentifierExpression && ((IdentifierExpression)queryGroupClause.Projection).Identifier == currentTransparentType[0].Item1.Name) {
					// We are grouping by the single active range variable, so we can use the single argument form of GroupBy
					return currentResult.Invoke("GroupBy", keyLambda);
				}
				else {
					var projectionParam = CreateParameterForCurrentRangeVariable();
					var projectionLambda = CreateLambda(new[] { projectionParam }, VisitNested(queryGroupClause.Projection, projectionParam));
					return currentResult.Invoke("GroupBy", keyLambda, projectionLambda);
				}
			}

			AstNode IAstVisitor<AstNode>.VisitIdentifierExpression(IdentifierExpression identifierExpression) {
				Expression subst;
				activeRangeVariableSubstitutions.TryGetValue(identifierExpression.Identifier, out subst);
				return subst != null ? subst.Clone() : null;
			}

#region Uninteresting methods
			AstNode IAstVisitor<AstNode>.VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression) {
				return Default(anonymousMethodExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitUndocumentedExpression(UndocumentedExpression undocumentedExpression) {
				return Default(undocumentedExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression) {
				return Default(arrayCreateExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression) {
				return Default(arrayInitializerExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitAsExpression(AsExpression asExpression) {
				return Default(asExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitAssignmentExpression(AssignmentExpression assignmentExpression) {
				return Default(assignmentExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression) {
				return Default(baseReferenceExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression) {
				return Default(binaryOperatorExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitCastExpression(CastExpression castExpression) {
				return Default(castExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitCheckedExpression(CheckedExpression checkedExpression) {
				return Default(checkedExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitConditionalExpression(ConditionalExpression conditionalExpression) {
				return Default(conditionalExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression) {
				return Default(defaultValueExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitDirectionExpression(DirectionExpression directionExpression) {
				return Default(directionExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitIndexerExpression(IndexerExpression indexerExpression) {
				return Default(indexerExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitInvocationExpression(InvocationExpression invocationExpression) {
				return Default(invocationExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitIsExpression(IsExpression isExpression) {
				return Default(isExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitLambdaExpression(LambdaExpression lambdaExpression) {
				return Default(lambdaExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression) {
				return Default(memberReferenceExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression) {
				return Default(namedArgumentExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitNamedExpression(NamedExpression namedExpression) {
				return Default(namedExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitNullReferenceExpression(NullReferenceExpression nullReferenceExpression) {
				return Default(nullReferenceExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression) {
				return Default(objectCreateExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitAnonymousTypeCreateExpression(AnonymousTypeCreateExpression anonymousTypeCreateExpression) {
				return Default(anonymousTypeCreateExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression) {
				return Default(parenthesizedExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression) {
				return Default(pointerReferenceExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitPrimitiveExpression(PrimitiveExpression primitiveExpression) {
				return Default(primitiveExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitSizeOfExpression(SizeOfExpression sizeOfExpression) {
				return Default(sizeOfExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitStackAllocExpression(StackAllocExpression stackAllocExpression) {
				return Default(stackAllocExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression) {
				return Default(thisReferenceExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitTypeOfExpression(TypeOfExpression typeOfExpression) {
				return Default(typeOfExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression) {
				return Default(typeReferenceExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression) {
				return Default(unaryOperatorExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitUncheckedExpression(UncheckedExpression uncheckedExpression) {
				return Default(uncheckedExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitEmptyExpression(EmptyExpression emptyExpression) {
				return Default(emptyExpression);
			}

			AstNode IAstVisitor<AstNode>.VisitAttribute(Attribute attribute) {
				return Default(attribute);
			}

			AstNode IAstVisitor<AstNode>.VisitAttributeSection(AttributeSection attributeSection) {
				return Default(attributeSection);
			}

			AstNode IAstVisitor<AstNode>.VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration) {
				return Default(delegateDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration) {
				return Default(namespaceDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitTypeDeclaration(TypeDeclaration typeDeclaration) {
				return Default(typeDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitUsingAliasDeclaration(UsingAliasDeclaration usingAliasDeclaration) {
				return Default(usingAliasDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitUsingDeclaration(UsingDeclaration usingDeclaration) {
				return Default(usingDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitExternAliasDeclaration(ExternAliasDeclaration externAliasDeclaration) {
				return Default(externAliasDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitBlockStatement(BlockStatement blockStatement) {
				return Default(blockStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitBreakStatement(BreakStatement breakStatement) {
				return Default(breakStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitCheckedStatement(CheckedStatement checkedStatement) {
				return Default(checkedStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitContinueStatement(ContinueStatement continueStatement) {
				return Default(continueStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitDoWhileStatement(DoWhileStatement doWhileStatement) {
				return Default(doWhileStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitEmptyStatement(EmptyStatement emptyStatement) {
				return Default(emptyStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitExpressionStatement(ExpressionStatement expressionStatement) {
				return Default(expressionStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitFixedStatement(FixedStatement fixedStatement) {
				return Default(fixedStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitForeachStatement(ForeachStatement foreachStatement) {
				return Default(foreachStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitForStatement(ForStatement forStatement) {
				return Default(forStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement) {
				return Default(gotoCaseStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitGotoDefaultStatement(GotoDefaultStatement gotoDefaultStatement) {
				return Default(gotoDefaultStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitGotoStatement(GotoStatement gotoStatement) {
				return Default(gotoStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitIfElseStatement(IfElseStatement ifElseStatement) {
				return Default(ifElseStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitLabelStatement(LabelStatement labelStatement) {
				return Default(labelStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitLockStatement(LockStatement lockStatement) {
				return Default(lockStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitReturnStatement(ReturnStatement returnStatement) {
				return Default(returnStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitSwitchStatement(SwitchStatement switchStatement) {
				return Default(switchStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitSwitchSection(SwitchSection switchSection) {
				return Default(switchSection);
			}

			AstNode IAstVisitor<AstNode>.VisitCaseLabel(CaseLabel caseLabel) {
				return Default(caseLabel);
			}

			AstNode IAstVisitor<AstNode>.VisitThrowStatement(ThrowStatement throwStatement) {
				return Default(throwStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitTryCatchStatement(TryCatchStatement tryCatchStatement) {
				return Default(tryCatchStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitCatchClause(CatchClause catchClause) {
				return Default(catchClause);
			}

			AstNode IAstVisitor<AstNode>.VisitUncheckedStatement(UncheckedStatement uncheckedStatement) {
				return Default(uncheckedStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitUnsafeStatement(UnsafeStatement unsafeStatement) {
				return Default(unsafeStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitUsingStatement(UsingStatement usingStatement) {
				return Default(usingStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement) {
				return Default(variableDeclarationStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitWhileStatement(WhileStatement whileStatement) {
				return Default(whileStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitYieldBreakStatement(YieldBreakStatement yieldBreakStatement) {
				return Default(yieldBreakStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitYieldReturnStatement(YieldReturnStatement yieldReturnStatement) {
				return Default(yieldReturnStatement);
			}

			AstNode IAstVisitor<AstNode>.VisitAccessor(Accessor accessor) {
				return Default(accessor);
			}

			AstNode IAstVisitor<AstNode>.VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration) {
				return Default(constructorDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitConstructorInitializer(ConstructorInitializer constructorInitializer) {
				return Default(constructorInitializer);
			}

			AstNode IAstVisitor<AstNode>.VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration) {
				return Default(destructorDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration) {
				return Default(enumMemberDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitEventDeclaration(EventDeclaration eventDeclaration) {
				return Default(eventDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitCustomEventDeclaration(CustomEventDeclaration customEventDeclaration) {
				return Default(customEventDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitFieldDeclaration(FieldDeclaration fieldDeclaration) {
				return Default(fieldDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration) {
				return Default(indexerDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitMethodDeclaration(MethodDeclaration methodDeclaration) {
				return Default(methodDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration) {
				return Default(operatorDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitParameterDeclaration(ParameterDeclaration parameterDeclaration) {
				return Default(parameterDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration) {
				return Default(propertyDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitVariableInitializer(VariableInitializer variableInitializer) {
				return Default(variableInitializer);
			}

			AstNode IAstVisitor<AstNode>.VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration) {
				return Default(fixedFieldDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitFixedVariableInitializer(FixedVariableInitializer fixedVariableInitializer) {
				return Default(fixedVariableInitializer);
			}

			AstNode IAstVisitor<AstNode>.VisitCompilationUnit(CompilationUnit compilationUnit) {
				return Default(compilationUnit);
			}

			AstNode IAstVisitor<AstNode>.VisitSimpleType(SimpleType simpleType) {
				return Default(simpleType);
			}

			AstNode IAstVisitor<AstNode>.VisitMemberType(MemberType memberType) {
				return Default(memberType);
			}

			AstNode IAstVisitor<AstNode>.VisitComposedType(ComposedType composedType) {
				return Default(composedType);
			}

			AstNode IAstVisitor<AstNode>.VisitArraySpecifier(ArraySpecifier arraySpecifier) {
				return Default(arraySpecifier);
			}

			AstNode IAstVisitor<AstNode>.VisitPrimitiveType(PrimitiveType primitiveType) {
				return Default(primitiveType);
			}

			AstNode IAstVisitor<AstNode>.VisitComment(Comment comment) {
				return Default(comment);
			}

			AstNode IAstVisitor<AstNode>.VisitWhitespace(WhitespaceNode whitespaceNode) {
				return Default(whitespaceNode);
			}

			AstNode IAstVisitor<AstNode>.VisitText(TextNode textNode) {
				return Default(textNode);
			}

			AstNode IAstVisitor<AstNode>.VisitNewLine(NewLineNode newLineNode) {
				return Default(newLineNode);
			}

			AstNode IAstVisitor<AstNode>.VisitPreProcessorDirective(PreProcessorDirective preProcessorDirective) {
				return Default(preProcessorDirective);
			}

			AstNode IAstVisitor<AstNode>.VisitDocumentationReference(DocumentationReference documentationReference) {
				return Default(documentationReference);
			}

			AstNode IAstVisitor<AstNode>.VisitTypeParameterDeclaration(TypeParameterDeclaration typeParameterDeclaration) {
				return Default(typeParameterDeclaration);
			}

			AstNode IAstVisitor<AstNode>.VisitConstraint(Constraint constraint) {
				return Default(constraint);
			}

			AstNode IAstVisitor<AstNode>.VisitCSharpTokenNode(CSharpTokenNode cSharpTokenNode) {
				return Default(cSharpTokenNode);
			}

			AstNode IAstVisitor<AstNode>.VisitIdentifier(Identifier identifier) {
				return Default(identifier);
			}

			AstNode IAstVisitor<AstNode>.VisitPatternPlaceholder(AstNode placeholder, Pattern pattern) {
				return Default(pattern);
			}
#endregion
		}

		/// <summary>
		/// Expands all occurances of query patterns in the specified node. Returns a clone of the node with all query patterns expanded, or null if there was no query pattern to expand.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public QueryExpressionExpansionResult ExpandQueryPattern(AstNode node) {
			var visitor = new Visitor();
			var astNode = node.AcceptVisitor(visitor);
			if (astNode != null) {
				astNode.Freeze();
				return new QueryExpressionExpansionResult(astNode, visitor.rangeVariables, visitor.expressions);
			}
			else {
				return null;
			}
		}
	}
}
