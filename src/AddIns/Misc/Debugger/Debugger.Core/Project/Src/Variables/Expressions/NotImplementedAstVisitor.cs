// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Ast;
using Debugger.Wrappers.CorSym;

namespace Debugger
{
	abstract class NotImplementedAstVisitor: IAstVisitor
	{
		public virtual object VisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitAddressOfExpression(AddressOfExpression addressOfExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitAttribute(ICSharpCode.NRefactory.Ast.Attribute attribute, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitAttributeSection(AttributeSection attributeSection, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitBreakStatement(BreakStatement breakStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitCaseLabel(CaseLabel caseLabel, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitCastExpression(CastExpression castExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitCatchClause(CatchClause catchClause, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitCheckedExpression(CheckedExpression checkedExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitCheckedStatement(CheckedStatement checkedStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitCollectionInitializerExpression(CollectionInitializerExpression collectionInitializerExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitContinueStatement(ContinueStatement continueStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitDirectionExpression(DirectionExpression directionExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitElseIfSection(ElseIfSection elseIfSection, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitEmptyStatement(EmptyStatement emptyStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitEndStatement(EndStatement endStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitEraseStatement(EraseStatement eraseStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitErrorStatement(ErrorStatement errorStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitEventAddRegion(EventAddRegion eventAddRegion, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitEventRaiseRegion(EventRaiseRegion eventRaiseRegion, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitEventRemoveRegion(EventRemoveRegion eventRemoveRegion, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitExitStatement(ExitStatement exitStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitFixedStatement(FixedStatement fixedStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitForNextStatement(ForNextStatement forNextStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitForStatement(ForStatement forStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitGotoStatement(GotoStatement gotoStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitIfElseStatement(IfElseStatement ifElseStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitInnerClassTypeReference(InnerClassTypeReference innerClassTypeReference, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitInterfaceImplementation(InterfaceImplementation interfaceImplementation, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitLabelStatement(LabelStatement labelStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitLambdaExpression(LambdaExpression lambdaExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitLockStatement(LockStatement lockStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitOnErrorStatement(OnErrorStatement onErrorStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitOptionDeclaration(OptionDeclaration optionDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitPropertySetRegion(PropertySetRegion propertySetRegion, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitQueryExpression(QueryExpression queryExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitQueryExpressionFromClause(QueryExpressionFromClause queryExpressionFromClause, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitQueryExpressionGroupClause(QueryExpressionGroupClause queryExpressionGroupClause, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitQueryExpressionIntoClause(QueryExpressionIntoClause queryExpressionIntoClause, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitQueryExpressionJoinClause(QueryExpressionJoinClause queryExpressionJoinClause, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitQueryExpressionLetClause(QueryExpressionLetClause queryExpressionLetClause, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitQueryExpressionOrdering(QueryExpressionOrdering queryExpressionOrdering, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitQueryExpressionSelectClause(QueryExpressionSelectClause queryExpressionSelectClause, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitQueryExpressionWhereClause(QueryExpressionWhereClause queryExpressionWhereClause, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitRaiseEventStatement(RaiseEventStatement raiseEventStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitReDimStatement(ReDimStatement reDimStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitRemoveHandlerStatement(RemoveHandlerStatement removeHandlerStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitResumeStatement(ResumeStatement resumeStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitStopStatement(StopStatement stopStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitSwitchSection(SwitchSection switchSection, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitSwitchStatement(SwitchStatement switchStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitTemplateDefinition(TemplateDefinition templateDefinition, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitThrowStatement(ThrowStatement throwStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitTypeReference(TypeReference typeReference, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitUnsafeStatement(UnsafeStatement unsafeStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitUsing(Using @using, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitUsingStatement(UsingStatement usingStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitWithStatement(WithStatement withStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public virtual object VisitYieldStatement(YieldStatement yieldStatement, object data)
		{
			throw new NotImplementedException();
		}
	}
}
