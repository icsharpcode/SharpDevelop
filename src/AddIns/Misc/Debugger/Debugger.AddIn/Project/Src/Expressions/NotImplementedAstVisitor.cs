// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.NRefactory;
using System;

namespace Debugger.AddIn
{
	public class NotImplementedAstVisitor: IAstVisitor 
	{
		public virtual object VisitAddHandlerStatement(ICSharpCode.NRefactory.Ast.AddHandlerStatement addHandlerStatement, object data)
		{
			throw new NotImplementedException("AddHandlerStatement");
		}
		
		public virtual object VisitAddressOfExpression(ICSharpCode.NRefactory.Ast.AddressOfExpression addressOfExpression, object data)
		{
			throw new NotImplementedException("AddressOfExpression");
		}
		
		public virtual object VisitAnonymousMethodExpression(ICSharpCode.NRefactory.Ast.AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			throw new NotImplementedException("AnonymousMethodExpression");
		}
		
		public virtual object VisitArrayCreateExpression(ICSharpCode.NRefactory.Ast.ArrayCreateExpression arrayCreateExpression, object data)
		{
			throw new NotImplementedException("ArrayCreateExpression");
		}
		
		public virtual object VisitAssignmentExpression(ICSharpCode.NRefactory.Ast.AssignmentExpression assignmentExpression, object data)
		{
			throw new NotImplementedException("AssignmentExpression");
		}
		
		public virtual object VisitAttribute(ICSharpCode.NRefactory.Ast.Attribute attribute, object data)
		{
			throw new NotImplementedException("Attribute");
		}
		
		public virtual object VisitAttributeSection(ICSharpCode.NRefactory.Ast.AttributeSection attributeSection, object data)
		{
			throw new NotImplementedException("AttributeSection");
		}
		
		public virtual object VisitBaseReferenceExpression(ICSharpCode.NRefactory.Ast.BaseReferenceExpression baseReferenceExpression, object data)
		{
			throw new NotImplementedException("BaseReferenceExpression");
		}
		
		public virtual object VisitBinaryOperatorExpression(ICSharpCode.NRefactory.Ast.BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			throw new NotImplementedException("BinaryOperatorExpression");
		}
		
		public virtual object VisitBlockStatement(ICSharpCode.NRefactory.Ast.BlockStatement blockStatement, object data)
		{
			throw new NotImplementedException("BlockStatement");
		}
		
		public virtual object VisitBreakStatement(ICSharpCode.NRefactory.Ast.BreakStatement breakStatement, object data)
		{
			throw new NotImplementedException("BreakStatement");
		}
		
		public virtual object VisitCaseLabel(ICSharpCode.NRefactory.Ast.CaseLabel caseLabel, object data)
		{
			throw new NotImplementedException("CaseLabel");
		}
		
		public virtual object VisitCastExpression(ICSharpCode.NRefactory.Ast.CastExpression castExpression, object data)
		{
			throw new NotImplementedException("CastExpression");
		}
		
		public virtual object VisitCatchClause(ICSharpCode.NRefactory.Ast.CatchClause catchClause, object data)
		{
			throw new NotImplementedException("CatchClause");
		}
		
		public virtual object VisitCheckedExpression(ICSharpCode.NRefactory.Ast.CheckedExpression checkedExpression, object data)
		{
			throw new NotImplementedException("CheckedExpression");
		}
		
		public virtual object VisitCheckedStatement(ICSharpCode.NRefactory.Ast.CheckedStatement checkedStatement, object data)
		{
			throw new NotImplementedException("CheckedStatement");
		}
		
		public virtual object VisitClassReferenceExpression(ICSharpCode.NRefactory.Ast.ClassReferenceExpression classReferenceExpression, object data)
		{
			throw new NotImplementedException("ClassReferenceExpression");
		}
		
		public virtual object VisitCollectionInitializerExpression(ICSharpCode.NRefactory.Ast.CollectionInitializerExpression collectionInitializerExpression, object data)
		{
			throw new NotImplementedException("CollectionInitializerExpression");
		}
		
		public virtual object VisitCompilationUnit(ICSharpCode.NRefactory.Ast.CompilationUnit compilationUnit, object data)
		{
			throw new NotImplementedException("CompilationUnit");
		}
		
		public virtual object VisitConditionalExpression(ICSharpCode.NRefactory.Ast.ConditionalExpression conditionalExpression, object data)
		{
			throw new NotImplementedException("ConditionalExpression");
		}
		
		public virtual object VisitConstructorDeclaration(ICSharpCode.NRefactory.Ast.ConstructorDeclaration constructorDeclaration, object data)
		{
			throw new NotImplementedException("ConstructorDeclaration");
		}
		
		public virtual object VisitConstructorInitializer(ICSharpCode.NRefactory.Ast.ConstructorInitializer constructorInitializer, object data)
		{
			throw new NotImplementedException("ConstructorInitializer");
		}
		
		public virtual object VisitContinueStatement(ICSharpCode.NRefactory.Ast.ContinueStatement continueStatement, object data)
		{
			throw new NotImplementedException("ContinueStatement");
		}
		
		public virtual object VisitDeclareDeclaration(ICSharpCode.NRefactory.Ast.DeclareDeclaration declareDeclaration, object data)
		{
			throw new NotImplementedException("DeclareDeclaration");
		}
		
		public virtual object VisitDefaultValueExpression(ICSharpCode.NRefactory.Ast.DefaultValueExpression defaultValueExpression, object data)
		{
			throw new NotImplementedException("DefaultValueExpression");
		}
		
		public virtual object VisitDelegateDeclaration(ICSharpCode.NRefactory.Ast.DelegateDeclaration delegateDeclaration, object data)
		{
			throw new NotImplementedException("DelegateDeclaration");
		}
		
		public virtual object VisitDestructorDeclaration(ICSharpCode.NRefactory.Ast.DestructorDeclaration destructorDeclaration, object data)
		{
			throw new NotImplementedException("DestructorDeclaration");
		}
		
		public virtual object VisitDirectionExpression(ICSharpCode.NRefactory.Ast.DirectionExpression directionExpression, object data)
		{
			throw new NotImplementedException("DirectionExpression");
		}
		
		public virtual object VisitDoLoopStatement(ICSharpCode.NRefactory.Ast.DoLoopStatement doLoopStatement, object data)
		{
			throw new NotImplementedException("DoLoopStatement");
		}
		
		public virtual object VisitElseIfSection(ICSharpCode.NRefactory.Ast.ElseIfSection elseIfSection, object data)
		{
			throw new NotImplementedException("ElseIfSection");
		}
		
		public virtual object VisitEmptyStatement(ICSharpCode.NRefactory.Ast.EmptyStatement emptyStatement, object data)
		{
			throw new NotImplementedException("EmptyStatement");
		}
		
		public virtual object VisitEndStatement(ICSharpCode.NRefactory.Ast.EndStatement endStatement, object data)
		{
			throw new NotImplementedException("EndStatement");
		}
		
		public virtual object VisitEraseStatement(ICSharpCode.NRefactory.Ast.EraseStatement eraseStatement, object data)
		{
			throw new NotImplementedException("EraseStatement");
		}
		
		public virtual object VisitErrorStatement(ICSharpCode.NRefactory.Ast.ErrorStatement errorStatement, object data)
		{
			throw new NotImplementedException("ErrorStatement");
		}
		
		public virtual object VisitEventAddRegion(ICSharpCode.NRefactory.Ast.EventAddRegion eventAddRegion, object data)
		{
			throw new NotImplementedException("EventAddRegion");
		}
		
		public virtual object VisitEventDeclaration(ICSharpCode.NRefactory.Ast.EventDeclaration eventDeclaration, object data)
		{
			throw new NotImplementedException("EventDeclaration");
		}
		
		public virtual object VisitEventRaiseRegion(ICSharpCode.NRefactory.Ast.EventRaiseRegion eventRaiseRegion, object data)
		{
			throw new NotImplementedException("EventRaiseRegion");
		}
		
		public virtual object VisitEventRemoveRegion(ICSharpCode.NRefactory.Ast.EventRemoveRegion eventRemoveRegion, object data)
		{
			throw new NotImplementedException("EventRemoveRegion");
		}
		
		public virtual object VisitExitStatement(ICSharpCode.NRefactory.Ast.ExitStatement exitStatement, object data)
		{
			throw new NotImplementedException("ExitStatement");
		}
		
		public virtual object VisitExpressionStatement(ICSharpCode.NRefactory.Ast.ExpressionStatement expressionStatement, object data)
		{
			throw new NotImplementedException("ExpressionStatement");
		}
		
		public virtual object VisitFieldDeclaration(ICSharpCode.NRefactory.Ast.FieldDeclaration fieldDeclaration, object data)
		{
			throw new NotImplementedException("FieldDeclaration");
		}
		
		public virtual object VisitFixedStatement(ICSharpCode.NRefactory.Ast.FixedStatement fixedStatement, object data)
		{
			throw new NotImplementedException("FixedStatement");
		}
		
		public virtual object VisitForeachStatement(ICSharpCode.NRefactory.Ast.ForeachStatement foreachStatement, object data)
		{
			throw new NotImplementedException("ForeachStatement");
		}
		
		public virtual object VisitForNextStatement(ICSharpCode.NRefactory.Ast.ForNextStatement forNextStatement, object data)
		{
			throw new NotImplementedException("ForNextStatement");
		}
		
		public virtual object VisitForStatement(ICSharpCode.NRefactory.Ast.ForStatement forStatement, object data)
		{
			throw new NotImplementedException("ForStatement");
		}
		
		public virtual object VisitGotoCaseStatement(ICSharpCode.NRefactory.Ast.GotoCaseStatement gotoCaseStatement, object data)
		{
			throw new NotImplementedException("GotoCaseStatement");
		}
		
		public virtual object VisitGotoStatement(ICSharpCode.NRefactory.Ast.GotoStatement gotoStatement, object data)
		{
			throw new NotImplementedException("GotoStatement");
		}
		
		public virtual object VisitIdentifierExpression(ICSharpCode.NRefactory.Ast.IdentifierExpression identifierExpression, object data)
		{
			throw new NotImplementedException("IdentifierExpression");
		}
		
		public virtual object VisitIfElseStatement(ICSharpCode.NRefactory.Ast.IfElseStatement ifElseStatement, object data)
		{
			throw new NotImplementedException("IfElseStatement");
		}
		
		public virtual object VisitIndexerDeclaration(ICSharpCode.NRefactory.Ast.IndexerDeclaration indexerDeclaration, object data)
		{
			throw new NotImplementedException("IndexerDeclaration");
		}
		
		public virtual object VisitIndexerExpression(ICSharpCode.NRefactory.Ast.IndexerExpression indexerExpression, object data)
		{
			throw new NotImplementedException("IndexerExpression");
		}
		
		public virtual object VisitInnerClassTypeReference(ICSharpCode.NRefactory.Ast.InnerClassTypeReference innerClassTypeReference, object data)
		{
			throw new NotImplementedException("InnerClassTypeReference");
		}
		
		public virtual object VisitInterfaceImplementation(ICSharpCode.NRefactory.Ast.InterfaceImplementation interfaceImplementation, object data)
		{
			throw new NotImplementedException("InterfaceImplementation");
		}
		
		public virtual object VisitInvocationExpression(ICSharpCode.NRefactory.Ast.InvocationExpression invocationExpression, object data)
		{
			throw new NotImplementedException("InvocationExpression");
		}
		
		public virtual object VisitLabelStatement(ICSharpCode.NRefactory.Ast.LabelStatement labelStatement, object data)
		{
			throw new NotImplementedException("LabelStatement");
		}
		
		public virtual object VisitLambdaExpression(ICSharpCode.NRefactory.Ast.LambdaExpression lambdaExpression, object data)
		{
			throw new NotImplementedException("LambdaExpression");
		}
		
		public virtual object VisitLocalVariableDeclaration(ICSharpCode.NRefactory.Ast.LocalVariableDeclaration localVariableDeclaration, object data)
		{
			throw new NotImplementedException("LocalVariableDeclaration");
		}
		
		public virtual object VisitLockStatement(ICSharpCode.NRefactory.Ast.LockStatement lockStatement, object data)
		{
			throw new NotImplementedException("LockStatement");
		}
		
		public virtual object VisitMemberReferenceExpression(ICSharpCode.NRefactory.Ast.MemberReferenceExpression memberReferenceExpression, object data)
		{
			throw new NotImplementedException("MemberReferenceExpression");
		}
		
		public virtual object VisitMethodDeclaration(ICSharpCode.NRefactory.Ast.MethodDeclaration methodDeclaration, object data)
		{
			throw new NotImplementedException("MethodDeclaration");
		}
		
		public virtual object VisitNamedArgumentExpression(ICSharpCode.NRefactory.Ast.NamedArgumentExpression namedArgumentExpression, object data)
		{
			throw new NotImplementedException("NamedArgumentExpression");
		}
		
		public virtual object VisitNamespaceDeclaration(ICSharpCode.NRefactory.Ast.NamespaceDeclaration namespaceDeclaration, object data)
		{
			throw new NotImplementedException("NamespaceDeclaration");
		}
		
		public virtual object VisitObjectCreateExpression(ICSharpCode.NRefactory.Ast.ObjectCreateExpression objectCreateExpression, object data)
		{
			throw new NotImplementedException("ObjectCreateExpression");
		}
		
		public virtual object VisitOnErrorStatement(ICSharpCode.NRefactory.Ast.OnErrorStatement onErrorStatement, object data)
		{
			throw new NotImplementedException("OnErrorStatement");
		}
		
		public virtual object VisitOperatorDeclaration(ICSharpCode.NRefactory.Ast.OperatorDeclaration operatorDeclaration, object data)
		{
			throw new NotImplementedException("OperatorDeclaration");
		}
		
		public virtual object VisitOptionDeclaration(ICSharpCode.NRefactory.Ast.OptionDeclaration optionDeclaration, object data)
		{
			throw new NotImplementedException("OptionDeclaration");
		}
		
		public virtual object VisitParameterDeclarationExpression(ICSharpCode.NRefactory.Ast.ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			throw new NotImplementedException("ParameterDeclarationExpression");
		}
		
		public virtual object VisitParenthesizedExpression(ICSharpCode.NRefactory.Ast.ParenthesizedExpression parenthesizedExpression, object data)
		{
			throw new NotImplementedException("ParenthesizedExpression");
		}
		
		public virtual object VisitPointerReferenceExpression(ICSharpCode.NRefactory.Ast.PointerReferenceExpression pointerReferenceExpression, object data)
		{
			throw new NotImplementedException("PointerReferenceExpression");
		}
		
		public virtual object VisitPrimitiveExpression(ICSharpCode.NRefactory.Ast.PrimitiveExpression primitiveExpression, object data)
		{
			throw new NotImplementedException("PrimitiveExpression");
		}
		
		public virtual object VisitPropertyDeclaration(ICSharpCode.NRefactory.Ast.PropertyDeclaration propertyDeclaration, object data)
		{
			throw new NotImplementedException("PropertyDeclaration");
		}
		
		public virtual object VisitPropertyGetRegion(ICSharpCode.NRefactory.Ast.PropertyGetRegion propertyGetRegion, object data)
		{
			throw new NotImplementedException("PropertyGetRegion");
		}
		
		public virtual object VisitPropertySetRegion(ICSharpCode.NRefactory.Ast.PropertySetRegion propertySetRegion, object data)
		{
			throw new NotImplementedException("PropertySetRegion");
		}
		
		public virtual object VisitQueryExpression(ICSharpCode.NRefactory.Ast.QueryExpression queryExpression, object data)
		{
			throw new NotImplementedException("QueryExpression");
		}
		
		public virtual object VisitQueryExpressionFromClause(ICSharpCode.NRefactory.Ast.QueryExpressionFromClause queryExpressionFromClause, object data)
		{
			throw new NotImplementedException("QueryExpressionFromClause");
		}
		
		public virtual object VisitQueryExpressionGroupClause(ICSharpCode.NRefactory.Ast.QueryExpressionGroupClause queryExpressionGroupClause, object data)
		{
			throw new NotImplementedException("QueryExpressionGroupClause");
		}
		
		public virtual object VisitQueryExpressionIntoClause(ICSharpCode.NRefactory.Ast.QueryExpressionIntoClause queryExpressionIntoClause, object data)
		{
			throw new NotImplementedException("QueryExpressionIntoClause");
		}
		
		public virtual object VisitQueryExpressionJoinClause(ICSharpCode.NRefactory.Ast.QueryExpressionJoinClause queryExpressionJoinClause, object data)
		{
			throw new NotImplementedException("QueryExpressionJoinClause");
		}
		
		public virtual object VisitQueryExpressionLetClause(ICSharpCode.NRefactory.Ast.QueryExpressionLetClause queryExpressionLetClause, object data)
		{
			throw new NotImplementedException("QueryExpressionLetClause");
		}
		
		public virtual object VisitQueryExpressionOrderClause(ICSharpCode.NRefactory.Ast.QueryExpressionOrderClause queryExpressionOrderClause, object data)
		{
			throw new NotImplementedException("QueryExpressionOrderClause");
		}
		
		public virtual object VisitQueryExpressionOrdering(ICSharpCode.NRefactory.Ast.QueryExpressionOrdering queryExpressionOrdering, object data)
		{
			throw new NotImplementedException("QueryExpressionOrdering");
		}
		
		public virtual object VisitQueryExpressionSelectClause(ICSharpCode.NRefactory.Ast.QueryExpressionSelectClause queryExpressionSelectClause, object data)
		{
			throw new NotImplementedException("QueryExpressionSelectClause");
		}
		
		public virtual object VisitQueryExpressionWhereClause(ICSharpCode.NRefactory.Ast.QueryExpressionWhereClause queryExpressionWhereClause, object data)
		{
			throw new NotImplementedException("QueryExpressionWhereClause");
		}
		
		public virtual object VisitRaiseEventStatement(ICSharpCode.NRefactory.Ast.RaiseEventStatement raiseEventStatement, object data)
		{
			throw new NotImplementedException("RaiseEventStatement");
		}
		
		public virtual object VisitReDimStatement(ICSharpCode.NRefactory.Ast.ReDimStatement reDimStatement, object data)
		{
			throw new NotImplementedException("ReDimStatement");
		}
		
		public virtual object VisitRemoveHandlerStatement(ICSharpCode.NRefactory.Ast.RemoveHandlerStatement removeHandlerStatement, object data)
		{
			throw new NotImplementedException("RemoveHandlerStatement");
		}
		
		public virtual object VisitResumeStatement(ICSharpCode.NRefactory.Ast.ResumeStatement resumeStatement, object data)
		{
			throw new NotImplementedException("ResumeStatement");
		}
		
		public virtual object VisitReturnStatement(ICSharpCode.NRefactory.Ast.ReturnStatement returnStatement, object data)
		{
			throw new NotImplementedException("ReturnStatement");
		}
		
		public virtual object VisitSizeOfExpression(ICSharpCode.NRefactory.Ast.SizeOfExpression sizeOfExpression, object data)
		{
			throw new NotImplementedException("SizeOfExpression");
		}
		
		public virtual object VisitStackAllocExpression(ICSharpCode.NRefactory.Ast.StackAllocExpression stackAllocExpression, object data)
		{
			throw new NotImplementedException("StackAllocExpression");
		}
		
		public virtual object VisitStopStatement(ICSharpCode.NRefactory.Ast.StopStatement stopStatement, object data)
		{
			throw new NotImplementedException("StopStatement");
		}
		
		public virtual object VisitSwitchSection(ICSharpCode.NRefactory.Ast.SwitchSection switchSection, object data)
		{
			throw new NotImplementedException("SwitchSection");
		}
		
		public virtual object VisitSwitchStatement(ICSharpCode.NRefactory.Ast.SwitchStatement switchStatement, object data)
		{
			throw new NotImplementedException("SwitchStatement");
		}
		
		public virtual object VisitTemplateDefinition(ICSharpCode.NRefactory.Ast.TemplateDefinition templateDefinition, object data)
		{
			throw new NotImplementedException("TemplateDefinition");
		}
		
		public virtual object VisitThisReferenceExpression(ICSharpCode.NRefactory.Ast.ThisReferenceExpression thisReferenceExpression, object data)
		{
			throw new NotImplementedException("ThisReferenceExpression");
		}
		
		public virtual object VisitThrowStatement(ICSharpCode.NRefactory.Ast.ThrowStatement throwStatement, object data)
		{
			throw new NotImplementedException("ThrowStatement");
		}
		
		public virtual object VisitTryCatchStatement(ICSharpCode.NRefactory.Ast.TryCatchStatement tryCatchStatement, object data)
		{
			throw new NotImplementedException("TryCatchStatement");
		}
		
		public virtual object VisitTypeDeclaration(ICSharpCode.NRefactory.Ast.TypeDeclaration typeDeclaration, object data)
		{
			throw new NotImplementedException("TypeDeclaration");
		}
		
		public virtual object VisitTypeOfExpression(ICSharpCode.NRefactory.Ast.TypeOfExpression typeOfExpression, object data)
		{
			throw new NotImplementedException("TypeOfExpression");
		}
		
		public virtual object VisitTypeOfIsExpression(ICSharpCode.NRefactory.Ast.TypeOfIsExpression typeOfIsExpression, object data)
		{
			throw new NotImplementedException("TypeOfIsExpression");
		}
		
		public virtual object VisitTypeReference(ICSharpCode.NRefactory.Ast.TypeReference typeReference, object data)
		{
			throw new NotImplementedException("TypeReference");
		}
		
		public virtual object VisitTypeReferenceExpression(ICSharpCode.NRefactory.Ast.TypeReferenceExpression typeReferenceExpression, object data)
		{
			throw new NotImplementedException("TypeReferenceExpression");
		}
		
		public virtual object VisitUnaryOperatorExpression(ICSharpCode.NRefactory.Ast.UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			throw new NotImplementedException("UnaryOperatorExpression");
		}
		
		public virtual object VisitUncheckedExpression(ICSharpCode.NRefactory.Ast.UncheckedExpression uncheckedExpression, object data)
		{
			throw new NotImplementedException("UncheckedExpression");
		}
		
		public virtual object VisitUncheckedStatement(ICSharpCode.NRefactory.Ast.UncheckedStatement uncheckedStatement, object data)
		{
			throw new NotImplementedException("UncheckedStatement");
		}
		
		public virtual object VisitUnsafeStatement(ICSharpCode.NRefactory.Ast.UnsafeStatement unsafeStatement, object data)
		{
			throw new NotImplementedException("UnsafeStatement");
		}
		
		public virtual object VisitUsing(ICSharpCode.NRefactory.Ast.Using @using, object data)
		{
			throw new NotImplementedException("Using");
		}
		
		public virtual object VisitUsingDeclaration(ICSharpCode.NRefactory.Ast.UsingDeclaration usingDeclaration, object data)
		{
			throw new NotImplementedException("UsingDeclaration");
		}
		
		public virtual object VisitUsingStatement(ICSharpCode.NRefactory.Ast.UsingStatement usingStatement, object data)
		{
			throw new NotImplementedException("UsingStatement");
		}
		
		public virtual object VisitVariableDeclaration(ICSharpCode.NRefactory.Ast.VariableDeclaration variableDeclaration, object data)
		{
			throw new NotImplementedException("VariableDeclaration");
		}
		
		public virtual object VisitWithStatement(ICSharpCode.NRefactory.Ast.WithStatement withStatement, object data)
		{
			throw new NotImplementedException("WithStatement");
		}
		
		public virtual object VisitYieldStatement(ICSharpCode.NRefactory.Ast.YieldStatement yieldStatement, object data)
		{
			throw new NotImplementedException("YieldStatement");
		}
		
		public virtual object VisitExpressionRangeVariable(ICSharpCode.NRefactory.Ast.ExpressionRangeVariable expressionRangeVariable, object data)
		{
			throw new NotImplementedException("ExpressionRangeVariable");
		}
		
		public virtual object VisitQueryExpressionAggregateClause(ICSharpCode.NRefactory.Ast.QueryExpressionAggregateClause queryExpressionAggregateClause, object data)
		{
			throw new NotImplementedException("QueryExpressionAggregateClause");
		}
		
		public virtual object VisitQueryExpressionDistinctClause(ICSharpCode.NRefactory.Ast.QueryExpressionDistinctClause queryExpressionDistinctClause, object data)
		{
			throw new NotImplementedException("QueryExpressionDistinctClause");
		}
		
		public virtual object VisitQueryExpressionGroupJoinVBClause(ICSharpCode.NRefactory.Ast.QueryExpressionGroupJoinVBClause queryExpressionGroupJoinVBClause, object data)
		{
			throw new NotImplementedException("QueryExpressionGroupJoinVBClause");
		}
		
		public virtual object VisitQueryExpressionGroupVBClause(ICSharpCode.NRefactory.Ast.QueryExpressionGroupVBClause queryExpressionGroupVBClause, object data)
		{
			throw new NotImplementedException("QueryExpressionGroupVBClause");
		}
		
		public virtual object VisitQueryExpressionJoinConditionVB(ICSharpCode.NRefactory.Ast.QueryExpressionJoinConditionVB queryExpressionJoinConditionVB, object data)
		{
			throw new NotImplementedException("QueryExpressionJoinConditionVB");
		}
		
		public virtual object VisitQueryExpressionJoinVBClause(ICSharpCode.NRefactory.Ast.QueryExpressionJoinVBClause queryExpressionJoinVBClause, object data)
		{
			throw new NotImplementedException("QueryExpressionJoinVBClause");
		}
		
		public virtual object VisitQueryExpressionLetVBClause(ICSharpCode.NRefactory.Ast.QueryExpressionLetVBClause queryExpressionLetVBClause, object data)
		{
			throw new NotImplementedException("QueryExpressionLetVBClause");
		}
		
		public virtual object VisitQueryExpressionPartitionVBClause(ICSharpCode.NRefactory.Ast.QueryExpressionPartitionVBClause queryExpressionPartitionVBClause, object data)
		{
			throw new NotImplementedException("QueryExpressionPartitionVBClause");
		}
		
		public virtual object VisitQueryExpressionSelectVBClause(ICSharpCode.NRefactory.Ast.QueryExpressionSelectVBClause queryExpressionSelectVBClause, object data)
		{
			throw new NotImplementedException("QueryExpressionSelectVBClause");
		}
	}
}
