// 
// ObservableAstVisitor.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2011 Novell, Inc (http://www.novell.com)
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

namespace ICSharpCode.NRefactory.CSharp
{
	public delegate void ObserveNodeHandler<T> (T node);
	
	public class ObservableAstVisitor<T, S> : IAstVisitor<T, S>
	{
		
		public event ObserveNodeHandler<CompilationUnit> Visited;

		S VisitChildren (AstNode node, T data)
		{
			AstNode next;
			for (var child = node.FirstChild; child != null; child = next) {
				// Store next to allow the loop to continue
				// if the visitor removes/replaces child.
				next = child.NextSibling;
				child.AcceptVisitor (this, data);
			}
			return default (S);
		}
		
		public event ObserveNodeHandler<CompilationUnit> CompilationUnitVisited;

		S IAstVisitor<T, S>.VisitCompilationUnit (CompilationUnit unit, T data)
		{
			var handler = CompilationUnitVisited;
			if (handler != null)
				handler (unit);
			return VisitChildren (unit, data);
		}
		
		public event ObserveNodeHandler<Comment> CommentVisited;

		S IAstVisitor<T, S>.VisitComment (Comment comment, T data)
		{
			var handler = CommentVisited;
			if (handler != null)
				handler (comment);
			return VisitChildren (comment, data);
		}
		
		public event ObserveNodeHandler<Identifier> IdentifierVisited;

		S IAstVisitor<T, S>.VisitIdentifier (Identifier identifier, T data)
		{
			var handler = IdentifierVisited;
			if (handler != null)
				handler (identifier);
			return VisitChildren (identifier, data);
		}
		
		public event ObserveNodeHandler<CSharpTokenNode> CSharpTokenNodeVisited;

		S IAstVisitor<T, S>.VisitCSharpTokenNode (CSharpTokenNode token, T data)
		{
			var handler = CSharpTokenNodeVisited;
			if (handler != null)
				handler (token);
			return VisitChildren (token, data);
		}
		
		public event ObserveNodeHandler<PrimitiveType> PrimitiveTypeVisited;

		S IAstVisitor<T, S>.VisitPrimitiveType (PrimitiveType primitiveType, T data)
		{
			var handler = PrimitiveTypeVisited;
			if (handler != null)
				handler (primitiveType);
			return VisitChildren (primitiveType, data);
		}
		
		public event ObserveNodeHandler<ComposedType> ComposedTypeVisited;

		S IAstVisitor<T, S>.VisitComposedType (ComposedType composedType, T data)
		{
			var handler = ComposedTypeVisited;
			if (handler != null)
				handler (composedType);
			return VisitChildren (composedType, data);
		}
		
		public event ObserveNodeHandler<SimpleType> SimpleTypeVisited;

		S IAstVisitor<T, S>.VisitSimpleType (SimpleType simpleType, T data)
		{
			var handler = SimpleTypeVisited;
			if (handler != null)
				handler (simpleType);
			return VisitChildren (simpleType, data);
		}
		
		public event ObserveNodeHandler<MemberType> MemberTypeVisited;

		S IAstVisitor<T, S>.VisitMemberType (MemberType memberType, T data)
		{
			var handler = MemberTypeVisited;
			if (handler != null)
				handler (memberType);
			return VisitChildren (memberType, data);
		}
		
		public event ObserveNodeHandler<Attribute> AttributeVisited;

		S IAstVisitor<T, S>.VisitAttribute (Attribute attribute, T data)
		{
			var handler = AttributeVisited;
			if (handler != null)
				handler (attribute);
			return VisitChildren (attribute, data);
		}
		
		public event ObserveNodeHandler<AttributeSection> AttributeSectionVisited;

		S IAstVisitor<T, S>.VisitAttributeSection (AttributeSection attributeSection, T data)
		{
			var handler = AttributeSectionVisited;
			if (handler != null)
				handler (attributeSection);
			return VisitChildren (attributeSection, data);
		}
		
		public event ObserveNodeHandler<DelegateDeclaration> DelegateDeclarationVisited;

		S IAstVisitor<T, S>.VisitDelegateDeclaration (DelegateDeclaration delegateDeclaration, T data)
		{
			var handler = DelegateDeclarationVisited;
			if (handler != null)
				handler (delegateDeclaration);
			return VisitChildren (delegateDeclaration, data);
		}
		
		public event ObserveNodeHandler<NamespaceDeclaration> NamespaceDeclarationVisited;

		S IAstVisitor<T, S>.VisitNamespaceDeclaration (NamespaceDeclaration namespaceDeclaration, T data)
		{
			var handler = NamespaceDeclarationVisited;
			if (handler != null)
				handler (namespaceDeclaration);
			return VisitChildren (namespaceDeclaration, data);
		}
		
		public event ObserveNodeHandler<TypeDeclaration> TypeDeclarationVisited;

		S IAstVisitor<T, S>.VisitTypeDeclaration (TypeDeclaration typeDeclaration, T data)
		{
			var handler = TypeDeclarationVisited;
			if (handler != null)
				handler (typeDeclaration);
			return VisitChildren (typeDeclaration, data);
		}
		
		public event ObserveNodeHandler<TypeParameterDeclaration> TypeParameterDeclarationVisited;

		S IAstVisitor<T, S>.VisitTypeParameterDeclaration (TypeParameterDeclaration typeParameterDeclaration, T data)
		{
			var handler = TypeParameterDeclarationVisited;
			if (handler != null)
				handler (typeParameterDeclaration);
			return VisitChildren (typeParameterDeclaration, data);
		}
		
		public event ObserveNodeHandler<EnumMemberDeclaration> EnumMemberDeclarationVisited;

		S IAstVisitor<T, S>.VisitEnumMemberDeclaration (EnumMemberDeclaration enumMemberDeclaration, T data)
		{
			var handler = EnumMemberDeclarationVisited;
			if (handler != null)
				handler (enumMemberDeclaration);
			return VisitChildren (enumMemberDeclaration, data);
		}
		
		public event ObserveNodeHandler<UsingDeclaration> UsingDeclarationVisited;

		S IAstVisitor<T, S>.VisitUsingDeclaration (UsingDeclaration usingDeclaration, T data)
		{
			var handler = UsingDeclarationVisited;
			if (handler != null)
				handler (usingDeclaration);
			return VisitChildren (usingDeclaration, data);
		}
		
		public event ObserveNodeHandler<UsingAliasDeclaration> UsingAliasDeclarationVisited;

		S IAstVisitor<T, S>.VisitUsingAliasDeclaration (UsingAliasDeclaration usingDeclaration, T data)
		{
			var handler = UsingAliasDeclarationVisited;
			if (handler != null)
				handler (usingDeclaration);
			return VisitChildren (usingDeclaration, data);
		}
		
		public event ObserveNodeHandler<ExternAliasDeclaration> ExternAliasDeclarationVisited;

		S IAstVisitor<T, S>.VisitExternAliasDeclaration (ExternAliasDeclaration externAliasDeclaration, T data)
		{
			var handler = ExternAliasDeclarationVisited;
			if (handler != null)
				handler (externAliasDeclaration);
			return VisitChildren (externAliasDeclaration, data);
		}
		
		public event ObserveNodeHandler<ConstructorDeclaration> ConstructorDeclarationVisited;

		S IAstVisitor<T, S>.VisitConstructorDeclaration (ConstructorDeclaration constructorDeclaration, T data)
		{
			var handler = ConstructorDeclarationVisited;
			if (handler != null)
				handler (constructorDeclaration);
			return VisitChildren (constructorDeclaration, data);
		}
		
		public event ObserveNodeHandler<ConstructorInitializer> ConstructorInitializerVisited;

		S IAstVisitor<T, S>.VisitConstructorInitializer (ConstructorInitializer constructorInitializer, T data)
		{
			var handler = ConstructorInitializerVisited;
			if (handler != null)
				handler (constructorInitializer);
			return VisitChildren (constructorInitializer, data);
		}
		
		public event ObserveNodeHandler<DestructorDeclaration> DestructorDeclarationVisited;

		S IAstVisitor<T, S>.VisitDestructorDeclaration (DestructorDeclaration destructorDeclaration, T data)
		{
			var handler = DestructorDeclarationVisited;
			if (handler != null)
				handler (destructorDeclaration);
			return VisitChildren (destructorDeclaration, data);
		}
		
		public event ObserveNodeHandler<EventDeclaration> EventDeclarationVisited;

		S IAstVisitor<T, S>.VisitEventDeclaration (EventDeclaration eventDeclaration, T data)
		{
			var handler = EventDeclarationVisited;
			if (handler != null)
				handler (eventDeclaration);
			return VisitChildren (eventDeclaration, data);
		}
		
		public event ObserveNodeHandler<CustomEventDeclaration> CustomEventDeclarationVisited;

		S IAstVisitor<T, S>.VisitCustomEventDeclaration (CustomEventDeclaration eventDeclaration, T data)
		{
			var handler = CustomEventDeclarationVisited;
			if (handler != null)
				handler (eventDeclaration);
			return VisitChildren (eventDeclaration, data);
		}
		
		public event ObserveNodeHandler<FieldDeclaration> FieldDeclarationVisited;

		S IAstVisitor<T, S>.VisitFieldDeclaration (FieldDeclaration fieldDeclaration, T data)
		{
			var handler = FieldDeclarationVisited;
			if (handler != null)
				handler (fieldDeclaration);
			return VisitChildren (fieldDeclaration, data);
		}
		
		public event ObserveNodeHandler<FixedFieldDeclaration> FixedFieldDeclarationVisited;

		S IAstVisitor<T, S>.VisitFixedFieldDeclaration (FixedFieldDeclaration fixedFieldDeclaration, T data)
		{
			var handler = FixedFieldDeclarationVisited;
			if (handler != null)
				handler (fixedFieldDeclaration);
			return VisitChildren (fixedFieldDeclaration, data);
		}
		
		public event ObserveNodeHandler<FixedVariableInitializer> FixedVariableInitializerVisited;

		S IAstVisitor<T, S>.VisitFixedVariableInitializer (FixedVariableInitializer fixedVariableInitializer, T data)
		{
			var handler = FixedVariableInitializerVisited;
			if (handler != null)
				handler (fixedVariableInitializer);
			return VisitChildren (fixedVariableInitializer, data);
		}
		
		public event ObserveNodeHandler<IndexerDeclaration> IndexerDeclarationVisited;

		S IAstVisitor<T, S>.VisitIndexerDeclaration (IndexerDeclaration indexerDeclaration, T data)
		{
			var handler = IndexerDeclarationVisited;
			if (handler != null)
				handler (indexerDeclaration);
			return VisitChildren (indexerDeclaration, data);
		}
		
		public event ObserveNodeHandler<MethodDeclaration> MethodDeclarationVisited;

		S IAstVisitor<T, S>.VisitMethodDeclaration (MethodDeclaration methodDeclaration, T data)
		{
			var handler = MethodDeclarationVisited;
			if (handler != null)
				handler (methodDeclaration);
			return VisitChildren (methodDeclaration, data);
		}
		
		public event ObserveNodeHandler<OperatorDeclaration> OperatorDeclarationVisited;

		S IAstVisitor<T, S>.VisitOperatorDeclaration (OperatorDeclaration operatorDeclaration, T data)
		{
			var handler = OperatorDeclarationVisited;
			if (handler != null)
				handler (operatorDeclaration);
			return VisitChildren (operatorDeclaration, data);
		}
		
		public event ObserveNodeHandler<PropertyDeclaration> PropertyDeclarationVisited;

		S IAstVisitor<T, S>.VisitPropertyDeclaration (PropertyDeclaration propertyDeclaration, T data)
		{
			var handler = PropertyDeclarationVisited;
			if (handler != null)
				handler (propertyDeclaration);
			return VisitChildren (propertyDeclaration, data);
		}
		
		public event ObserveNodeHandler<Accessor> AccessorVisited;

		S IAstVisitor<T, S>.VisitAccessor (Accessor accessor, T data)
		{
			var handler = AccessorVisited;
			if (handler != null)
				handler (accessor);
			return VisitChildren (accessor, data);
		}
		
		public event ObserveNodeHandler<VariableInitializer> VariableInitializerVisited;

		S IAstVisitor<T, S>.VisitVariableInitializer (VariableInitializer variableInitializer, T data)
		{
			var handler = VariableInitializerVisited;
			if (handler != null)
				handler (variableInitializer);
			return VisitChildren (variableInitializer, data);
		}
		
		public event ObserveNodeHandler<ParameterDeclaration> ParameterDeclarationVisited;

		S IAstVisitor<T, S>.VisitParameterDeclaration (ParameterDeclaration parameterDeclaration, T data)
		{
			var handler = ParameterDeclarationVisited;
			if (handler != null)
				handler (parameterDeclaration);
			return VisitChildren (parameterDeclaration, data);
		}
		
		public event ObserveNodeHandler<Constraint> ConstraintVisited;

		S IAstVisitor<T, S>.VisitConstraint (Constraint constraint, T data)
		{
			var handler = ConstraintVisited;
			if (handler != null)
				handler (constraint);
			return VisitChildren (constraint, data);
		}
		
		public event ObserveNodeHandler<BlockStatement> BlockStatementVisited;

		S IAstVisitor<T, S>.VisitBlockStatement (BlockStatement blockStatement, T data)
		{
			var handler = BlockStatementVisited;
			if (handler != null)
				handler (blockStatement);
			return VisitChildren (blockStatement, data);
		}
		
		public event ObserveNodeHandler<ExpressionStatement> ExpressionStatementVisited;

		S IAstVisitor<T, S>.VisitExpressionStatement (ExpressionStatement expressionStatement, T data)
		{
			var handler = ExpressionStatementVisited;
			if (handler != null)
				handler (expressionStatement);
			return VisitChildren (expressionStatement, data);
		}
		
		public event ObserveNodeHandler<BreakStatement> BreakStatementVisited;

		S IAstVisitor<T, S>.VisitBreakStatement (BreakStatement breakStatement, T data)
		{
			var handler = BreakStatementVisited;
			if (handler != null)
				handler (breakStatement);
			return VisitChildren (breakStatement, data);
		}
		
		public event ObserveNodeHandler<CheckedStatement> CheckedStatementVisited;

		S IAstVisitor<T, S>.VisitCheckedStatement (CheckedStatement checkedStatement, T data)
		{
			var handler = CheckedStatementVisited;
			if (handler != null)
				handler (checkedStatement);
			return VisitChildren (checkedStatement, data);
		}
		
		public event ObserveNodeHandler<ContinueStatement> ContinueStatementVisited;

		S IAstVisitor<T, S>.VisitContinueStatement (ContinueStatement continueStatement, T data)
		{
			var handler = ContinueStatementVisited;
			if (handler != null)
				handler (continueStatement);
			return VisitChildren (continueStatement, data);
		}
		
		public event ObserveNodeHandler<DoWhileStatement> DoWhileStatementVisited;

		S IAstVisitor<T, S>.VisitDoWhileStatement (DoWhileStatement doWhileStatement, T data)
		{
			var handler = DoWhileStatementVisited;
			if (handler != null)
				handler (doWhileStatement);
			return VisitChildren (doWhileStatement, data);
		}
		
		public event ObserveNodeHandler<EmptyStatement> EmptyStatementVisited;

		S IAstVisitor<T, S>.VisitEmptyStatement (EmptyStatement emptyStatement, T data)
		{
			var handler = EmptyStatementVisited;
			if (handler != null)
				handler (emptyStatement);
			return VisitChildren (emptyStatement, data);
		}
		
		public event ObserveNodeHandler<FixedStatement> FixedStatementVisited;

		S IAstVisitor<T, S>.VisitFixedStatement (FixedStatement fixedStatement, T data)
		{
			var handler = FixedStatementVisited;
			if (handler != null)
				handler (fixedStatement);
			return VisitChildren (fixedStatement, data);
		}
		
		public event ObserveNodeHandler<ForeachStatement> ForeachStatementVisited;

		S IAstVisitor<T, S>.VisitForeachStatement (ForeachStatement foreachStatement, T data)
		{
			var handler = ForeachStatementVisited;
			if (handler != null)
				handler (foreachStatement);
			return VisitChildren (foreachStatement, data);
		}
		
		public event ObserveNodeHandler<ForStatement> ForStatementVisited;

		S IAstVisitor<T, S>.VisitForStatement (ForStatement forStatement, T data)
		{
			var handler = ForStatementVisited;
			if (handler != null)
				handler (forStatement);
			return VisitChildren (forStatement, data);
		}
		
		public event ObserveNodeHandler<GotoCaseStatement> GotoCaseStatementVisited;

		S IAstVisitor<T, S>.VisitGotoCaseStatement (GotoCaseStatement gotoCaseStatement, T data)
		{
			var handler = GotoCaseStatementVisited;
			if (handler != null)
				handler (gotoCaseStatement);
			return VisitChildren (gotoCaseStatement, data);
		}
		
		public event ObserveNodeHandler<GotoDefaultStatement> GotoDefaultStatementVisited;

		S IAstVisitor<T, S>.VisitGotoDefaultStatement (GotoDefaultStatement gotoDefaultStatement, T data)
		{
			var handler = GotoDefaultStatementVisited;
			if (handler != null)
				handler (gotoDefaultStatement);
			return VisitChildren (gotoDefaultStatement, data);
		}
		
		public event ObserveNodeHandler<GotoStatement> GotoStatementVisited;

		S IAstVisitor<T, S>.VisitGotoStatement (GotoStatement gotoStatement, T data)
		{
			var handler = GotoStatementVisited;
			if (handler != null)
				handler (gotoStatement);
			return VisitChildren (gotoStatement, data);
		}
		
		public event ObserveNodeHandler<IfElseStatement> IfElseStatementVisited;

		S IAstVisitor<T, S>.VisitIfElseStatement (IfElseStatement ifElseStatement, T data)
		{
			var handler = IfElseStatementVisited;
			if (handler != null)
				handler (ifElseStatement);
			return VisitChildren (ifElseStatement, data);
		}
		
		public event ObserveNodeHandler<LabelStatement> LabelStatementVisited;

		S IAstVisitor<T, S>.VisitLabelStatement (LabelStatement labelStatement, T data)
		{
			var handler = LabelStatementVisited;
			if (handler != null)
				handler (labelStatement);
			return VisitChildren (labelStatement, data);
		}
		
		public event ObserveNodeHandler<LockStatement> LockStatementVisited;

		S IAstVisitor<T, S>.VisitLockStatement (LockStatement lockStatement, T data)
		{
			var handler = LockStatementVisited;
			if (handler != null)
				handler (lockStatement);
			return VisitChildren (lockStatement, data);
		}
		
		public event ObserveNodeHandler<ReturnStatement> ReturnStatementVisited;

		S IAstVisitor<T, S>.VisitReturnStatement (ReturnStatement returnStatement, T data)
		{
			var handler = ReturnStatementVisited;
			if (handler != null)
				handler (returnStatement);
			return VisitChildren (returnStatement, data);
		}
		
		public event ObserveNodeHandler<SwitchStatement> SwitchStatementVisited;

		S IAstVisitor<T, S>.VisitSwitchStatement (SwitchStatement switchStatement, T data)
		{
			var handler = SwitchStatementVisited;
			if (handler != null)
				handler (switchStatement);
			return VisitChildren (switchStatement, data);
		}
		
		public event ObserveNodeHandler<SwitchSection> SwitchSectionVisited;

		S IAstVisitor<T, S>.VisitSwitchSection (SwitchSection switchSection, T data)
		{
			var handler = SwitchSectionVisited;
			if (handler != null)
				handler (switchSection);
			return VisitChildren (switchSection, data);
		}
		
		public event ObserveNodeHandler<CaseLabel> CaseLabelVisited;

		S IAstVisitor<T, S>.VisitCaseLabel (CaseLabel caseLabel, T data)
		{
			var handler = CaseLabelVisited;
			if (handler != null)
				handler (caseLabel);
			return VisitChildren (caseLabel, data);
		}
		
		public event ObserveNodeHandler<ThrowStatement> ThrowStatementVisited;

		S IAstVisitor<T, S>.VisitThrowStatement (ThrowStatement throwStatement, T data)
		{
			var handler = ThrowStatementVisited;
			if (handler != null)
				handler (throwStatement);
			return VisitChildren (throwStatement, data);
		}
		
		public event ObserveNodeHandler<TryCatchStatement> TryCatchStatementVisited;

		S IAstVisitor<T, S>.VisitTryCatchStatement (TryCatchStatement tryCatchStatement, T data)
		{
			var handler = TryCatchStatementVisited;
			if (handler != null)
				handler (tryCatchStatement);
			return VisitChildren (tryCatchStatement, data);
		}
		
		public event ObserveNodeHandler<CatchClause> CatchClauseVisited;

		S IAstVisitor<T, S>.VisitCatchClause (CatchClause catchClause, T data)
		{
			var handler = CatchClauseVisited;
			if (handler != null)
				handler (catchClause);
			return VisitChildren (catchClause, data);
		}
		
		public event ObserveNodeHandler<UncheckedStatement> UncheckedStatementVisited;

		S IAstVisitor<T, S>.VisitUncheckedStatement (UncheckedStatement uncheckedStatement, T data)
		{
			var handler = UncheckedStatementVisited;
			if (handler != null)
				handler (uncheckedStatement);
			return VisitChildren (uncheckedStatement, data);
		}
		
		public event ObserveNodeHandler<UnsafeStatement> UnsafeStatementVisited;

		S IAstVisitor<T, S>.VisitUnsafeStatement (UnsafeStatement unsafeStatement, T data)
		{
			var handler = UnsafeStatementVisited;
			if (handler != null)
				handler (unsafeStatement);
			return VisitChildren (unsafeStatement, data);
		}
		
		public event ObserveNodeHandler<UsingStatement> UsingStatementVisited;

		S IAstVisitor<T, S>.VisitUsingStatement (UsingStatement usingStatement, T data)
		{
			var handler = UsingStatementVisited;
			if (handler != null)
				handler (usingStatement);
			return VisitChildren (usingStatement, data);
		}
		
		public event ObserveNodeHandler<VariableDeclarationStatement> VariableDeclarationStatementVisited;

		S IAstVisitor<T, S>.VisitVariableDeclarationStatement (VariableDeclarationStatement variableDeclarationStatement, T data)
		{
			var handler = VariableDeclarationStatementVisited;
			if (handler != null)
				handler (variableDeclarationStatement);
			return VisitChildren (variableDeclarationStatement, data);
		}
		
		public event ObserveNodeHandler<WhileStatement> WhileStatementVisited;

		S IAstVisitor<T, S>.VisitWhileStatement (WhileStatement whileStatement, T data)
		{
			var handler = WhileStatementVisited;
			if (handler != null)
				handler (whileStatement);
			return VisitChildren (whileStatement, data);
		}
		
		public event ObserveNodeHandler<YieldBreakStatement> YieldBreakStatementVisited;

		S IAstVisitor<T, S>.VisitYieldBreakStatement (YieldBreakStatement yieldBreakStatement, T data)
		{
			var handler = YieldBreakStatementVisited;
			if (handler != null)
				handler (yieldBreakStatement);
			return VisitChildren (yieldBreakStatement, data);
		}
		
		public event ObserveNodeHandler<YieldStatement> YieldStatementVisited;

		S IAstVisitor<T, S>.VisitYieldStatement (YieldStatement yieldStatement, T data)
		{
			var handler = YieldStatementVisited;
			if (handler != null)
				handler (yieldStatement);
			return VisitChildren (yieldStatement, data);
		}
		
		public event ObserveNodeHandler<AnonymousMethodExpression> AnonymousMethodExpressionVisited;

		S IAstVisitor<T, S>.VisitAnonymousMethodExpression (AnonymousMethodExpression anonymousMethodExpression, T data)
		{
			var handler = AnonymousMethodExpressionVisited;
			if (handler != null)
				handler (anonymousMethodExpression);
			return VisitChildren (anonymousMethodExpression, data);
		}
		
		public event ObserveNodeHandler<LambdaExpression> LambdaExpressionVisited;

		S IAstVisitor<T, S>.VisitLambdaExpression (LambdaExpression lambdaExpression, T data)
		{
			var handler = LambdaExpressionVisited;
			if (handler != null)
				handler (lambdaExpression);
			return VisitChildren (lambdaExpression, data);
		}
		
		public event ObserveNodeHandler<AssignmentExpression> AssignmentExpressionVisited;

		S IAstVisitor<T, S>.VisitAssignmentExpression (AssignmentExpression assignmentExpression, T data)
		{
			var handler = AssignmentExpressionVisited;
			if (handler != null)
				handler (assignmentExpression);
			return VisitChildren (assignmentExpression, data);
		}
		
		public event ObserveNodeHandler<BaseReferenceExpression> BaseReferenceExpressionVisited;

		S IAstVisitor<T, S>.VisitBaseReferenceExpression (BaseReferenceExpression baseReferenceExpression, T data)
		{
			var handler = BaseReferenceExpressionVisited;
			if (handler != null)
				handler (baseReferenceExpression);
			return VisitChildren (baseReferenceExpression, data);
		}
		
		public event ObserveNodeHandler<BinaryOperatorExpression> BinaryOperatorExpressionVisited;

		S IAstVisitor<T, S>.VisitBinaryOperatorExpression (BinaryOperatorExpression binaryOperatorExpression, T data)
		{
			var handler = BinaryOperatorExpressionVisited;
			if (handler != null)
				handler (binaryOperatorExpression);
			return VisitChildren (binaryOperatorExpression, data);
		}
		
		public event ObserveNodeHandler<CastExpression> CastExpressionVisited;

		S IAstVisitor<T, S>.VisitCastExpression (CastExpression castExpression, T data)
		{
			var handler = CastExpressionVisited;
			if (handler != null)
				handler (castExpression);
			return VisitChildren (castExpression, data);
		}
		
		public event ObserveNodeHandler<CheckedExpression> CheckedExpressionVisited;

		S IAstVisitor<T, S>.VisitCheckedExpression (CheckedExpression checkedExpression, T data)
		{
			var handler = CheckedExpressionVisited;
			if (handler != null)
				handler (checkedExpression);
			return VisitChildren (checkedExpression, data);
		}
		
		public event ObserveNodeHandler<ConditionalExpression> ConditionalExpressionVisited;

		S IAstVisitor<T, S>.VisitConditionalExpression (ConditionalExpression conditionalExpression, T data)
		{
			var handler = ConditionalExpressionVisited;
			if (handler != null)
				handler (conditionalExpression);
			return VisitChildren (conditionalExpression, data);
		}
		
		public event ObserveNodeHandler<IdentifierExpression> IdentifierExpressionVisited;

		S IAstVisitor<T, S>.VisitIdentifierExpression (IdentifierExpression identifierExpression, T data)
		{
			var handler = IdentifierExpressionVisited;
			if (handler != null)
				handler (identifierExpression);
			return VisitChildren (identifierExpression, data);
		}
		
		public event ObserveNodeHandler<IndexerExpression> IndexerExpressionVisited;

		S IAstVisitor<T, S>.VisitIndexerExpression (IndexerExpression indexerExpression, T data)
		{
			var handler = IndexerExpressionVisited;
			if (handler != null)
				handler (indexerExpression);
			return VisitChildren (indexerExpression, data);
		}
		
		public event ObserveNodeHandler<InvocationExpression> InvocationExpressionVisited;

		S IAstVisitor<T, S>.VisitInvocationExpression (InvocationExpression invocationExpression, T data)
		{
			var handler = InvocationExpressionVisited;
			if (handler != null)
				handler (invocationExpression);
			return VisitChildren (invocationExpression, data);
		}
		
		public event ObserveNodeHandler<DirectionExpression> DirectionExpressionVisited;

		S IAstVisitor<T, S>.VisitDirectionExpression (DirectionExpression directionExpression, T data)
		{
			var handler = DirectionExpressionVisited;
			if (handler != null)
				handler (directionExpression);
			return VisitChildren (directionExpression, data);
		}
		
		public event ObserveNodeHandler<MemberReferenceExpression> MemberReferenceExpressionVisited;

		S IAstVisitor<T, S>.VisitMemberReferenceExpression (MemberReferenceExpression memberReferenceExpression, T data)
		{
			var handler = MemberReferenceExpressionVisited;
			if (handler != null)
				handler (memberReferenceExpression);
			return VisitChildren (memberReferenceExpression, data);
		}
		
		public event ObserveNodeHandler<NullReferenceExpression> NullReferenceExpressionVisited;

		S IAstVisitor<T, S>.VisitNullReferenceExpression (NullReferenceExpression nullReferenceExpression, T data)
		{
			var handler = NullReferenceExpressionVisited;
			if (handler != null)
				handler (nullReferenceExpression);
			return VisitChildren (nullReferenceExpression, data);
		}
		
		public event ObserveNodeHandler<ObjectCreateExpression> ObjectCreateExpressionVisited;

		S IAstVisitor<T, S>.VisitObjectCreateExpression (ObjectCreateExpression objectCreateExpression, T data)
		{
			var handler = ObjectCreateExpressionVisited;
			if (handler != null)
				handler (objectCreateExpression);
			return VisitChildren (objectCreateExpression, data);
		}
		
		public event ObserveNodeHandler<AnonymousTypeCreateExpression> AnonymousTypeCreateExpressionVisited;

		S IAstVisitor<T, S>.VisitAnonymousTypeCreateExpression (AnonymousTypeCreateExpression anonymousTypeCreateExpression, T data)
		{
			var handler = AnonymousTypeCreateExpressionVisited;
			if (handler != null)
				handler (anonymousTypeCreateExpression);
			return VisitChildren (anonymousTypeCreateExpression, data);
		}
		
		public event ObserveNodeHandler<ArrayCreateExpression> ArrayCreateExpressionVisited;

		S IAstVisitor<T, S>.VisitArrayCreateExpression (ArrayCreateExpression arraySCreateExpression, T data)
		{
			var handler = ArrayCreateExpressionVisited;
			if (handler != null)
				handler (arraySCreateExpression);
			return VisitChildren (arraySCreateExpression, data);
		}
		
		public event ObserveNodeHandler<ParenthesizedExpression> ParenthesizedExpressionVisited;

		S IAstVisitor<T, S>.VisitParenthesizedExpression (ParenthesizedExpression parenthesizedExpression, T data)
		{
			var handler = ParenthesizedExpressionVisited;
			if (handler != null)
				handler (parenthesizedExpression);
			return VisitChildren (parenthesizedExpression, data);
		}
		
		public event ObserveNodeHandler<PointerReferenceExpression> PointerReferenceExpressionVisited;

		S IAstVisitor<T, S>.VisitPointerReferenceExpression (PointerReferenceExpression pointerReferenceExpression, T data)
		{
			var handler = PointerReferenceExpressionVisited;
			if (handler != null)
				handler (pointerReferenceExpression);
			return VisitChildren (pointerReferenceExpression, data);
		}
		
		public event ObserveNodeHandler<PrimitiveExpression> PrimitiveExpressionVisited;

		S IAstVisitor<T, S>.VisitPrimitiveExpression (PrimitiveExpression primitiveExpression, T data)
		{
			var handler = PrimitiveExpressionVisited;
			if (handler != null)
				handler (primitiveExpression);
			return VisitChildren (primitiveExpression, data);
		}
		
		public event ObserveNodeHandler<SizeOfExpression> SizeOfExpressionVisited;

		S IAstVisitor<T, S>.VisitSizeOfExpression (SizeOfExpression sizeOfExpression, T data)
		{
			var handler = SizeOfExpressionVisited;
			if (handler != null)
				handler (sizeOfExpression);
			return VisitChildren (sizeOfExpression, data);
		}
		
		public event ObserveNodeHandler<StackAllocExpression> StackAllocExpressionVisited;

		S IAstVisitor<T, S>.VisitStackAllocExpression (StackAllocExpression stackAllocExpression, T data)
		{
			var handler = StackAllocExpressionVisited;
			if (handler != null)
				handler (stackAllocExpression);
			return VisitChildren (stackAllocExpression, data);
		}
		
		public event ObserveNodeHandler<ThisReferenceExpression> ThisReferenceExpressionVisited;

		S IAstVisitor<T, S>.VisitThisReferenceExpression (ThisReferenceExpression thisReferenceExpression, T data)
		{
			var handler = ThisReferenceExpressionVisited;
			if (handler != null)
				handler (thisReferenceExpression);
			return VisitChildren (thisReferenceExpression, data);
		}
		
		public event ObserveNodeHandler<TypeOfExpression> TypeOfExpressionVisited;

		S IAstVisitor<T, S>.VisitTypeOfExpression (TypeOfExpression typeOfExpression, T data)
		{
			var handler = TypeOfExpressionVisited;
			if (handler != null)
				handler (typeOfExpression);
			return VisitChildren (typeOfExpression, data);
		}
		
		public event ObserveNodeHandler<TypeReferenceExpression> TypeReferenceExpressionVisited;

		S IAstVisitor<T, S>.VisitTypeReferenceExpression (TypeReferenceExpression typeReferenceExpression, T data)
		{
			var handler = TypeReferenceExpressionVisited;
			if (handler != null)
				handler (typeReferenceExpression);
			return VisitChildren (typeReferenceExpression, data);
		}
		
		public event ObserveNodeHandler<UnaryOperatorExpression> UnaryOperatorExpressionVisited;

		S IAstVisitor<T, S>.VisitUnaryOperatorExpression (UnaryOperatorExpression unaryOperatorExpression, T data)
		{
			var handler = UnaryOperatorExpressionVisited;
			if (handler != null)
				handler (unaryOperatorExpression);
			return VisitChildren (unaryOperatorExpression, data);
		}
		
		public event ObserveNodeHandler<UncheckedExpression> UncheckedExpressionVisited;

		S IAstVisitor<T, S>.VisitUncheckedExpression (UncheckedExpression uncheckedExpression, T data)
		{
			var handler = UncheckedExpressionVisited;
			if (handler != null)
				handler (uncheckedExpression);
			return VisitChildren (uncheckedExpression, data);
		}
		
		public event ObserveNodeHandler<QueryExpression> QueryExpressionVisited;

		S IAstVisitor<T, S>.VisitQueryExpression (QueryExpression queryExpression, T data)
		{
			var handler = QueryExpressionVisited;
			if (handler != null)
				handler (queryExpression);
			return VisitChildren (queryExpression, data);
		}
		
		public event ObserveNodeHandler<QueryContinuationClause> QueryContinuationClauseVisited;

		S IAstVisitor<T, S>.VisitQueryContinuationClause (QueryContinuationClause queryContinuationClause, T data)
		{
			var handler = QueryContinuationClauseVisited;
			if (handler != null)
				handler (queryContinuationClause);
			return VisitChildren (queryContinuationClause, data);
		}
		
		public event ObserveNodeHandler<QueryFromClause> QueryFromClauseVisited;

		S IAstVisitor<T, S>.VisitQueryFromClause (QueryFromClause queryFromClause, T data)
		{
			var handler = QueryFromClauseVisited;
			if (handler != null)
				handler (queryFromClause);
			return VisitChildren (queryFromClause, data);
		}
		
		public event ObserveNodeHandler<QueryLetClause> QueryLetClauseVisited;

		S IAstVisitor<T, S>.VisitQueryLetClause (QueryLetClause queryLetClause, T data)
		{
			var handler = QueryLetClauseVisited;
			if (handler != null)
				handler (queryLetClause);
			return VisitChildren (queryLetClause, data);
		}
		
		public event ObserveNodeHandler<QueryWhereClause> QueryWhereClauseVisited;

		S IAstVisitor<T, S>.VisitQueryWhereClause (QueryWhereClause queryWhereClause, T data)
		{
			var handler = QueryWhereClauseVisited;
			if (handler != null)
				handler (queryWhereClause);
			return VisitChildren (queryWhereClause, data);
		}
		
		public event ObserveNodeHandler<QueryJoinClause> QueryJoinClauseVisited;

		S IAstVisitor<T, S>.VisitQueryJoinClause (QueryJoinClause queryJoinClause, T data)
		{
			var handler = QueryJoinClauseVisited;
			if (handler != null)
				handler (queryJoinClause);
			return VisitChildren (queryJoinClause, data);
		}
		
		public event ObserveNodeHandler<QueryOrderClause> QueryOrderClauseVisited;

		S IAstVisitor<T, S>.VisitQueryOrderClause (QueryOrderClause queryOrderClause, T data)
		{
			var handler = QueryOrderClauseVisited;
			if (handler != null)
				handler (queryOrderClause);
			return VisitChildren (queryOrderClause, data);
		}
		
		public event ObserveNodeHandler<QueryOrdering> QueryOrderingVisited;

		S IAstVisitor<T, S>.VisitQueryOrdering (QueryOrdering queryOrdering, T data)
		{
			var handler = QueryOrderingVisited;
			if (handler != null)
				handler (queryOrdering);
			return VisitChildren (queryOrdering, data);
		}
		
		public event ObserveNodeHandler<QuerySelectClause> QuerySelectClauseVisited;

		S IAstVisitor<T, S>.VisitQuerySelectClause (QuerySelectClause querySelectClause, T data)
		{
			var handler = QuerySelectClauseVisited;
			if (handler != null)
				handler (querySelectClause);
			return VisitChildren (querySelectClause, data);
		}
		
		public event ObserveNodeHandler<QueryGroupClause> QueryGroupClauseVisited;

		S IAstVisitor<T, S>.VisitQueryGroupClause (QueryGroupClause queryGroupClause, T data)
		{
			var handler = QueryGroupClauseVisited;
			if (handler != null)
				handler (queryGroupClause);
			return VisitChildren (queryGroupClause, data);
		}
		
		public event ObserveNodeHandler<AsExpression> AsExpressionVisited;

		S IAstVisitor<T, S>.VisitAsExpression (AsExpression asExpression, T data)
		{
			var handler = AsExpressionVisited;
			if (handler != null)
				handler (asExpression);
			return VisitChildren (asExpression, data);
		}
		
		public event ObserveNodeHandler<IsExpression> IsExpressionVisited;

		S IAstVisitor<T, S>.VisitIsExpression (IsExpression isExpression, T data)
		{
			var handler = IsExpressionVisited;
			if (handler != null)
				handler (isExpression);
			return VisitChildren (isExpression, data);
		}
		
		public event ObserveNodeHandler<DefaultValueExpression> DefaultValueExpressionVisited;

		S IAstVisitor<T, S>.VisitDefaultValueExpression (DefaultValueExpression defaultValueExpression, T data)
		{
			var handler = DefaultValueExpressionVisited;
			if (handler != null)
				handler (defaultValueExpression);
			return VisitChildren (defaultValueExpression, data);
		}
		
		public event ObserveNodeHandler<UndocumentedExpression> UndocumentedExpressionVisited;

		S IAstVisitor<T, S>.VisitUndocumentedExpression (UndocumentedExpression undocumentedExpression, T data)
		{
			var handler = UndocumentedExpressionVisited;
			if (handler != null)
				handler (undocumentedExpression);
			return VisitChildren (undocumentedExpression, data);
		}
		
		public event ObserveNodeHandler<ArrayInitializerExpression> ArrayInitializerExpressionVisited;

		S IAstVisitor<T, S>.VisitArrayInitializerExpression (ArrayInitializerExpression arrayInitializerExpression, T data)
		{
			var handler = ArrayInitializerExpressionVisited;
			if (handler != null)
				handler (arrayInitializerExpression);
			return VisitChildren (arrayInitializerExpression, data);
		}
		
		public event ObserveNodeHandler<ArraySpecifier> ArraySpecifierVisited;

		S IAstVisitor<T, S>.VisitArraySpecifier (ArraySpecifier arraySpecifier, T data)
		{
			var handler = ArraySpecifierVisited;
			if (handler != null)
				handler (arraySpecifier);
			return VisitChildren (arraySpecifier, data);
		}
		
		public event ObserveNodeHandler<NamedArgumentExpression> NamedArgumentExpressionVisited;

		S IAstVisitor<T, S>.VisitNamedArgumentExpression (NamedArgumentExpression namedArgumentExpression, T data)
		{
			var handler = NamedArgumentExpressionVisited;
			if (handler != null)
				handler (namedArgumentExpression);
			return VisitChildren (namedArgumentExpression, data);
		}
		
		public event ObserveNodeHandler<EmptyExpression> EmptyExpressionVisited;

		S IAstVisitor<T, S>.VisitEmptyExpression (EmptyExpression emptyExpression, T data)
		{
			var handler = EmptyExpressionVisited;
			if (handler != null)
				handler (emptyExpression);
			return VisitChildren (emptyExpression, data);
		}
		
		S IAstVisitor<T, S>.VisitPatternPlaceholder (AstNode placeholder, PatternMatching.Pattern pattern, T data)
		{
			return VisitChildren (placeholder, data);
		}
	}
}


