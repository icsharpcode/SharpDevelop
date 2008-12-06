// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using Attribute=ICSharpCode.NRefactory.Ast.Attribute;

namespace Debugger.AddIn
{
	public class NotImplementedAstVisitor: IAstVisitor 
	{
		public virtual object VisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data)
		{
			throw new NotImplementedException("AddHandlerStatement");
		}
		
		public virtual object VisitAddressOfExpression(AddressOfExpression addressOfExpression, object data)
		{
			throw new NotImplementedException("AddressOfExpression");
		}
		
		public virtual object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			throw new NotImplementedException("AnonymousMethodExpression");
		}
		
		public virtual object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			throw new NotImplementedException("ArrayCreateExpression");
		}
		
		public virtual object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			throw new NotImplementedException("AssignmentExpression");
		}
		
		public virtual object VisitAttribute(Attribute attribute, object data)
		{
			throw new NotImplementedException("Attribute");
		}
		
		public virtual object VisitAttributeSection(AttributeSection attributeSection, object data)
		{
			throw new NotImplementedException("AttributeSection");
		}
		
		public virtual object VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			throw new NotImplementedException("BaseReferenceExpression");
		}
		
		public virtual object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			throw new NotImplementedException("BinaryOperatorExpression");
		}
		
		public virtual object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			throw new NotImplementedException("BlockStatement");
		}
		
		public virtual object VisitBreakStatement(BreakStatement breakStatement, object data)
		{
			throw new NotImplementedException("BreakStatement");
		}
		
		public virtual object VisitCaseLabel(CaseLabel caseLabel, object data)
		{
			throw new NotImplementedException("CaseLabel");
		}
		
		public virtual object VisitCastExpression(CastExpression castExpression, object data)
		{
			throw new NotImplementedException("CastExpression");
		}
		
		public virtual object VisitCatchClause(CatchClause catchClause, object data)
		{
			throw new NotImplementedException("CatchClause");
		}
		
		public virtual object VisitCheckedExpression(CheckedExpression checkedExpression, object data)
		{
			throw new NotImplementedException("CheckedExpression");
		}
		
		public virtual object VisitCheckedStatement(CheckedStatement checkedStatement, object data)
		{
			throw new NotImplementedException("CheckedStatement");
		}
		
		public virtual object VisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data)
		{
			throw new NotImplementedException("ClassReferenceExpression");
		}
		
		public virtual object VisitCollectionInitializerExpression(CollectionInitializerExpression collectionInitializerExpression, object data)
		{
			throw new NotImplementedException("CollectionInitializerExpression");
		}
		
		public virtual object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			throw new NotImplementedException("CompilationUnit");
		}
		
		public virtual object VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{
			throw new NotImplementedException("ConditionalExpression");
		}
		
		public virtual object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			throw new NotImplementedException("ConstructorDeclaration");
		}
		
		public virtual object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
		{
			throw new NotImplementedException("ConstructorInitializer");
		}
		
		public virtual object VisitContinueStatement(ContinueStatement continueStatement, object data)
		{
			throw new NotImplementedException("ContinueStatement");
		}
		
		public virtual object VisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data)
		{
			throw new NotImplementedException("DeclareDeclaration");
		}
		
		public virtual object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			throw new NotImplementedException("DefaultValueExpression");
		}
		
		public virtual object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			throw new NotImplementedException("DelegateDeclaration");
		}
		
		public virtual object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			throw new NotImplementedException("DestructorDeclaration");
		}
		
		public virtual object VisitDirectionExpression(DirectionExpression directionExpression, object data)
		{
			throw new NotImplementedException("DirectionExpression");
		}
		
		public virtual object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			throw new NotImplementedException("DoLoopStatement");
		}
		
		public virtual object VisitElseIfSection(ElseIfSection elseIfSection, object data)
		{
			throw new NotImplementedException("ElseIfSection");
		}
		
		public virtual object VisitEmptyStatement(EmptyStatement emptyStatement, object data)
		{
			throw new NotImplementedException("EmptyStatement");
		}
		
		public virtual object VisitEndStatement(EndStatement endStatement, object data)
		{
			throw new NotImplementedException("EndStatement");
		}
		
		public virtual object VisitEraseStatement(EraseStatement eraseStatement, object data)
		{
			throw new NotImplementedException("EraseStatement");
		}
		
		public virtual object VisitErrorStatement(ErrorStatement errorStatement, object data)
		{
			throw new NotImplementedException("ErrorStatement");
		}
		
		public virtual object VisitEventAddRegion(EventAddRegion eventAddRegion, object data)
		{
			throw new NotImplementedException("EventAddRegion");
		}
		
		public virtual object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			throw new NotImplementedException("EventDeclaration");
		}
		
		public virtual object VisitEventRaiseRegion(EventRaiseRegion eventRaiseRegion, object data)
		{
			throw new NotImplementedException("EventRaiseRegion");
		}
		
		public virtual object VisitEventRemoveRegion(EventRemoveRegion eventRemoveRegion, object data)
		{
			throw new NotImplementedException("EventRemoveRegion");
		}
		
		public virtual object VisitExitStatement(ExitStatement exitStatement, object data)
		{
			throw new NotImplementedException("ExitStatement");
		}
		
		public virtual object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			throw new NotImplementedException("ExpressionStatement");
		}
		
		public virtual object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			throw new NotImplementedException("FieldDeclaration");
		}
		
		public virtual object VisitFixedStatement(FixedStatement fixedStatement, object data)
		{
			throw new NotImplementedException("FixedStatement");
		}
		
		public virtual object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			throw new NotImplementedException("ForeachStatement");
		}
		
		public virtual object VisitForNextStatement(ForNextStatement forNextStatement, object data)
		{
			throw new NotImplementedException("ForNextStatement");
		}
		
		public virtual object VisitForStatement(ForStatement forStatement, object data)
		{
			throw new NotImplementedException("ForStatement");
		}
		
		public virtual object VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data)
		{
			throw new NotImplementedException("GotoCaseStatement");
		}
		
		public virtual object VisitGotoStatement(GotoStatement gotoStatement, object data)
		{
			throw new NotImplementedException("GotoStatement");
		}
		
		public virtual object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			throw new NotImplementedException("IdentifierExpression");
		}
		
		public virtual object VisitIfElseStatement(IfElseStatement ifElseStatement, object data)
		{
			throw new NotImplementedException("IfElseStatement");
		}
		
		public virtual object VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data)
		{
			throw new NotImplementedException("IndexerDeclaration");
		}
		
		public virtual object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			throw new NotImplementedException("IndexerExpression");
		}
		
		public virtual object VisitInnerClassTypeReference(InnerClassTypeReference innerClassTypeReference, object data)
		{
			throw new NotImplementedException("InnerClassTypeReference");
		}
		
		public virtual object VisitInterfaceImplementation(InterfaceImplementation interfaceImplementation, object data)
		{
			throw new NotImplementedException("InterfaceImplementation");
		}
		
		public virtual object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			throw new NotImplementedException("InvocationExpression");
		}
		
		public virtual object VisitLabelStatement(LabelStatement labelStatement, object data)
		{
			throw new NotImplementedException("LabelStatement");
		}
		
		public virtual object VisitLambdaExpression(LambdaExpression lambdaExpression, object data)
		{
			throw new NotImplementedException("LambdaExpression");
		}
		
		public virtual object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			throw new NotImplementedException("LocalVariableDeclaration");
		}
		
		public virtual object VisitLockStatement(LockStatement lockStatement, object data)
		{
			throw new NotImplementedException("LockStatement");
		}
		
		public virtual object VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			throw new NotImplementedException("MemberReferenceExpression");
		}
		
		public virtual object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			throw new NotImplementedException("MethodDeclaration");
		}
		
		public virtual object VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data)
		{
			throw new NotImplementedException("NamedArgumentExpression");
		}
		
		public virtual object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			throw new NotImplementedException("NamespaceDeclaration");
		}
		
		public virtual object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			throw new NotImplementedException("ObjectCreateExpression");
		}
		
		public virtual object VisitOnErrorStatement(OnErrorStatement onErrorStatement, object data)
		{
			throw new NotImplementedException("OnErrorStatement");
		}
		
		public virtual object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			throw new NotImplementedException("OperatorDeclaration");
		}
		
		public virtual object VisitOptionDeclaration(OptionDeclaration optionDeclaration, object data)
		{
			throw new NotImplementedException("OptionDeclaration");
		}
		
		public virtual object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			throw new NotImplementedException("ParameterDeclarationExpression");
		}
		
		public virtual object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			throw new NotImplementedException("ParenthesizedExpression");
		}
		
		public virtual object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			throw new NotImplementedException("PointerReferenceExpression");
		}
		
		public virtual object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			throw new NotImplementedException("PrimitiveExpression");
		}
		
		public virtual object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			throw new NotImplementedException("PropertyDeclaration");
		}
		
		public virtual object VisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data)
		{
			throw new NotImplementedException("PropertyGetRegion");
		}
		
		public virtual object VisitPropertySetRegion(PropertySetRegion propertySetRegion, object data)
		{
			throw new NotImplementedException("PropertySetRegion");
		}
		
		public virtual object VisitQueryExpression(QueryExpression queryExpression, object data)
		{
			throw new NotImplementedException("QueryExpression");
		}
		
		public virtual object VisitQueryExpressionFromClause(QueryExpressionFromClause queryExpressionFromClause, object data)
		{
			throw new NotImplementedException("QueryExpressionFromClause");
		}
		
		public virtual object VisitQueryExpressionGroupClause(QueryExpressionGroupClause queryExpressionGroupClause, object data)
		{
			throw new NotImplementedException("QueryExpressionGroupClause");
		}
		
		public virtual object VisitQueryExpressionJoinClause(QueryExpressionJoinClause queryExpressionJoinClause, object data)
		{
			throw new NotImplementedException("QueryExpressionJoinClause");
		}
		
		public virtual object VisitQueryExpressionLetClause(QueryExpressionLetClause queryExpressionLetClause, object data)
		{
			throw new NotImplementedException("QueryExpressionLetClause");
		}
		
		public virtual object VisitQueryExpressionOrderClause(QueryExpressionOrderClause queryExpressionOrderClause, object data)
		{
			throw new NotImplementedException("QueryExpressionOrderClause");
		}
		
		public virtual object VisitQueryExpressionOrdering(QueryExpressionOrdering queryExpressionOrdering, object data)
		{
			throw new NotImplementedException("QueryExpressionOrdering");
		}
		
		public virtual object VisitQueryExpressionSelectClause(QueryExpressionSelectClause queryExpressionSelectClause, object data)
		{
			throw new NotImplementedException("QueryExpressionSelectClause");
		}
		
		public virtual object VisitQueryExpressionWhereClause(QueryExpressionWhereClause queryExpressionWhereClause, object data)
		{
			throw new NotImplementedException("QueryExpressionWhereClause");
		}
		
		public virtual object VisitRaiseEventStatement(RaiseEventStatement raiseEventStatement, object data)
		{
			throw new NotImplementedException("RaiseEventStatement");
		}
		
		public virtual object VisitReDimStatement(ReDimStatement reDimStatement, object data)
		{
			throw new NotImplementedException("ReDimStatement");
		}
		
		public virtual object VisitRemoveHandlerStatement(RemoveHandlerStatement removeHandlerStatement, object data)
		{
			throw new NotImplementedException("RemoveHandlerStatement");
		}
		
		public virtual object VisitResumeStatement(ResumeStatement resumeStatement, object data)
		{
			throw new NotImplementedException("ResumeStatement");
		}
		
		public virtual object VisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			throw new NotImplementedException("ReturnStatement");
		}
		
		public virtual object VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
		{
			throw new NotImplementedException("SizeOfExpression");
		}
		
		public virtual object VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
		{
			throw new NotImplementedException("StackAllocExpression");
		}
		
		public virtual object VisitStopStatement(StopStatement stopStatement, object data)
		{
			throw new NotImplementedException("StopStatement");
		}
		
		public virtual object VisitSwitchSection(SwitchSection switchSection, object data)
		{
			throw new NotImplementedException("SwitchSection");
		}
		
		public virtual object VisitSwitchStatement(SwitchStatement switchStatement, object data)
		{
			throw new NotImplementedException("SwitchStatement");
		}
		
		public virtual object VisitTemplateDefinition(TemplateDefinition templateDefinition, object data)
		{
			throw new NotImplementedException("TemplateDefinition");
		}
		
		public virtual object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			throw new NotImplementedException("ThisReferenceExpression");
		}
		
		public virtual object VisitThrowStatement(ThrowStatement throwStatement, object data)
		{
			throw new NotImplementedException("ThrowStatement");
		}
		
		public virtual object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{
			throw new NotImplementedException("TryCatchStatement");
		}
		
		public virtual object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			throw new NotImplementedException("TypeDeclaration");
		}
		
		public virtual object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			throw new NotImplementedException("TypeOfExpression");
		}
		
		public virtual object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
		{
			throw new NotImplementedException("TypeOfIsExpression");
		}
		
		public virtual object VisitTypeReference(TypeReference typeReference, object data)
		{
			throw new NotImplementedException("TypeReference");
		}
		
		public virtual object VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data)
		{
			throw new NotImplementedException("TypeReferenceExpression");
		}
		
		public virtual object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			throw new NotImplementedException("UnaryOperatorExpression");
		}
		
		public virtual object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			throw new NotImplementedException("UncheckedExpression");
		}
		
		public virtual object VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
		{
			throw new NotImplementedException("UncheckedStatement");
		}
		
		public virtual object VisitUnsafeStatement(UnsafeStatement unsafeStatement, object data)
		{
			throw new NotImplementedException("UnsafeStatement");
		}
		
		public virtual object VisitUsing(Using @using, object data)
		{
			throw new NotImplementedException("Using");
		}
		
		public virtual object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			throw new NotImplementedException("UsingDeclaration");
		}
		
		public virtual object VisitUsingStatement(UsingStatement usingStatement, object data)
		{
			throw new NotImplementedException("UsingStatement");
		}
		
		public virtual object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			throw new NotImplementedException("VariableDeclaration");
		}
		
		public virtual object VisitWithStatement(WithStatement withStatement, object data)
		{
			throw new NotImplementedException("WithStatement");
		}
		
		public virtual object VisitYieldStatement(YieldStatement yieldStatement, object data)
		{
			throw new NotImplementedException("YieldStatement");
		}
		
		public virtual object VisitExpressionRangeVariable(ExpressionRangeVariable expressionRangeVariable, object data)
		{
			throw new NotImplementedException("ExpressionRangeVariable");
		}
		
		public virtual object VisitQueryExpressionAggregateClause(QueryExpressionAggregateClause queryExpressionAggregateClause, object data)
		{
			throw new NotImplementedException("QueryExpressionAggregateClause");
		}
		
		public virtual object VisitQueryExpressionDistinctClause(QueryExpressionDistinctClause queryExpressionDistinctClause, object data)
		{
			throw new NotImplementedException("QueryExpressionDistinctClause");
		}
		
		public virtual object VisitQueryExpressionGroupJoinVBClause(QueryExpressionGroupJoinVBClause queryExpressionGroupJoinVBClause, object data)
		{
			throw new NotImplementedException("QueryExpressionGroupJoinVBClause");
		}
		
		public virtual object VisitQueryExpressionGroupVBClause(QueryExpressionGroupVBClause queryExpressionGroupVBClause, object data)
		{
			throw new NotImplementedException("QueryExpressionGroupVBClause");
		}
		
		public virtual object VisitQueryExpressionJoinConditionVB(QueryExpressionJoinConditionVB queryExpressionJoinConditionVB, object data)
		{
			throw new NotImplementedException("QueryExpressionJoinConditionVB");
		}
		
		public virtual object VisitQueryExpressionJoinVBClause(QueryExpressionJoinVBClause queryExpressionJoinVBClause, object data)
		{
			throw new NotImplementedException("QueryExpressionJoinVBClause");
		}
		
		public virtual object VisitQueryExpressionLetVBClause(QueryExpressionLetVBClause queryExpressionLetVBClause, object data)
		{
			throw new NotImplementedException("QueryExpressionLetVBClause");
		}
		
		public virtual object VisitQueryExpressionPartitionVBClause(QueryExpressionPartitionVBClause queryExpressionPartitionVBClause, object data)
		{
			throw new NotImplementedException("QueryExpressionPartitionVBClause");
		}
		
		public virtual object VisitQueryExpressionSelectVBClause(QueryExpressionSelectVBClause queryExpressionSelectVBClause, object data)
		{
			throw new NotImplementedException("QueryExpressionSelectVBClause");
		}
	}
}
