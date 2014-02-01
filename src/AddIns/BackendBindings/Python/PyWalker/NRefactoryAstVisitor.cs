// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace PyWalker
{
	public class NRefactoryAstVisitor : AbstractAstVisitor
	{
		IOutputWriter writer;
			
		public NRefactoryAstVisitor(IOutputWriter writer)
		{
			this.writer = writer;
		}
		
		public override object VisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data)
		{
			return base.VisitAddHandlerStatement(addHandlerStatement, data);
		}
		
		public override object VisitAddressOfExpression(AddressOfExpression addressOfExpression, object data)
		{
			return base.VisitAddressOfExpression(addressOfExpression, data);
		}
		
		public override object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			return base.VisitAnonymousMethodExpression(anonymousMethodExpression, data);
		}
		
		public override object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			return base.VisitArrayCreateExpression(arrayCreateExpression, data);
		}
		
		public override object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			WriteLine("VisitAssignmentExpression");
			return base.VisitAssignmentExpression(assignmentExpression, data);
		}
		
		public override object VisitAttribute(ICSharpCode.NRefactory.Ast.Attribute attribute, object data)
		{
			return base.VisitAttribute(attribute, data);
		}
		
		public override object VisitAttributeSection(AttributeSection attributeSection, object data)
		{
			return base.VisitAttributeSection(attributeSection, data);
		}
		
		public override object VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			return base.VisitBaseReferenceExpression(baseReferenceExpression, data);
		}
		
		public override object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			return base.VisitBinaryOperatorExpression(binaryOperatorExpression, data);
		}
		
		public override object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			return base.VisitBlockStatement(blockStatement, data);
		}
		
		public override object VisitBreakStatement(BreakStatement breakStatement, object data)
		{
			return base.VisitBreakStatement(breakStatement, data);
		}
		
		public override object VisitCaseLabel(CaseLabel caseLabel, object data)
		{
			return base.VisitCaseLabel(caseLabel, data);
		}
		
		public override object VisitCastExpression(CastExpression castExpression, object data)
		{
			return base.VisitCastExpression(castExpression, data);
		}
		
		public override object VisitCatchClause(CatchClause catchClause, object data)
		{
			return base.VisitCatchClause(catchClause, data);
		}
		
		public override object VisitCheckedExpression(CheckedExpression checkedExpression, object data)
		{
			return base.VisitCheckedExpression(checkedExpression, data);
		}
		
		public override object VisitCheckedStatement(CheckedStatement checkedStatement, object data)
		{
			return base.VisitCheckedStatement(checkedStatement, data);
		}
		
		public override object VisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data)
		{
			return base.VisitClassReferenceExpression(classReferenceExpression, data);
		}
		
		public override object VisitCollectionInitializerExpression(CollectionInitializerExpression collectionInitializerExpression, object data)
		{
			WriteLine("VisitCollectionInitializerExpression");
			return base.VisitCollectionInitializerExpression(collectionInitializerExpression, data);
		}
		
		public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			WriteLine("VisitCodeCompileUnit");
			return base.VisitCompilationUnit(compilationUnit, data);
		}
		
		public override object VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{
			return base.VisitConditionalExpression(conditionalExpression, data);
		}
		
		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			WriteLine("VisitConstructorDeclaration");
			return base.VisitConstructorDeclaration(constructorDeclaration, data);
		}
		
		public override object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
		{
			WriteLine("VisitConstructorInitializer");
			return base.VisitConstructorInitializer(constructorInitializer, data);
		}
		
		public override object VisitContinueStatement(ContinueStatement continueStatement, object data)
		{
			return base.VisitContinueStatement(continueStatement, data);
		}
		
		public override object VisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data)
		{
			return base.VisitDeclareDeclaration(declareDeclaration, data);
		}
		
		public override object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			return base.VisitDefaultValueExpression(defaultValueExpression, data);
		}
		
		public override object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			return base.VisitDelegateDeclaration(delegateDeclaration, data);
		}
		
		public override object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			return base.VisitDestructorDeclaration(destructorDeclaration, data);
		}
		
		public override object VisitDirectionExpression(DirectionExpression directionExpression, object data)
		{
			return base.VisitDirectionExpression(directionExpression, data);
		}
		
		public override object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			return base.VisitDoLoopStatement(doLoopStatement, data);
		}
		
		public override object VisitElseIfSection(ElseIfSection elseIfSection, object data)
		{
			return base.VisitElseIfSection(elseIfSection, data);
		}
		
		public override object VisitEmptyStatement(EmptyStatement emptyStatement, object data)
		{
			WriteLine("VisitEmptyStatement");
			return base.VisitEmptyStatement(emptyStatement, data);
		}
		
		public override object VisitEndStatement(EndStatement endStatement, object data)
		{
			return base.VisitEndStatement(endStatement, data);
		}
		
		public override object VisitEraseStatement(EraseStatement eraseStatement, object data)
		{
			return base.VisitEraseStatement(eraseStatement, data);
		}
		
		public override object VisitErrorStatement(ErrorStatement errorStatement, object data)
		{
			return base.VisitErrorStatement(errorStatement, data);
		}
		
		public override object VisitEventAddRegion(EventAddRegion eventAddRegion, object data)
		{
			return base.VisitEventAddRegion(eventAddRegion, data);
		}
		
		public override object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			return base.VisitEventDeclaration(eventDeclaration, data);
		}
		
		public override object VisitEventRaiseRegion(EventRaiseRegion eventRaiseRegion, object data)
		{
			return base.VisitEventRaiseRegion(eventRaiseRegion, data);
		}
		
		public override object VisitEventRemoveRegion(EventRemoveRegion eventRemoveRegion, object data)
		{
			return base.VisitEventRemoveRegion(eventRemoveRegion, data);
		}
		
		public override object VisitExitStatement(ExitStatement exitStatement, object data)
		{
			return base.VisitExitStatement(exitStatement, data);
		}
		
		public override object VisitExpressionRangeVariable(ExpressionRangeVariable expressionRangeVariable, object data)
		{
			return base.VisitExpressionRangeVariable(expressionRangeVariable, data);
		}
		
		public override object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			WriteLine("VisitExpressionStatement");
			return base.VisitExpressionStatement(expressionStatement, data);
		}
		
		public override object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			WriteLine("VisitFieldDeclaration: " + fieldDeclaration.Fields[0].Name);
			return base.VisitFieldDeclaration(fieldDeclaration, data);
		}
		
		public override object VisitFixedStatement(FixedStatement fixedStatement, object data)
		{
			return base.VisitFixedStatement(fixedStatement, data);
		}
		
		public override object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			return base.VisitForeachStatement(foreachStatement, data);
		}
		
		public override object VisitForNextStatement(ForNextStatement forNextStatement, object data)
		{
			return base.VisitForNextStatement(forNextStatement, data);
		}
		
		public override object VisitForStatement(ForStatement forStatement, object data)
		{
			return base.VisitForStatement(forStatement, data);
		}
		
		public override object VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data)
		{
			return base.VisitGotoCaseStatement(gotoCaseStatement, data);
		}
		
		public override object VisitGotoStatement(GotoStatement gotoStatement, object data)
		{
			return base.VisitGotoStatement(gotoStatement, data);
		}
		
		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			WriteLine("VisitIdentifierExpression");
			return base.VisitIdentifierExpression(identifierExpression, data);
		}
		
		public override object VisitIfElseStatement(IfElseStatement ifElseStatement, object data)
		{
			return base.VisitIfElseStatement(ifElseStatement, data);
		}
		
		public override object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			return base.VisitIndexerExpression(indexerExpression, data);
		}
		
		public override object VisitInnerClassTypeReference(InnerClassTypeReference innerClassTypeReference, object data)
		{
			return base.VisitInnerClassTypeReference(innerClassTypeReference, data);
		}
		
		public override object VisitInterfaceImplementation(InterfaceImplementation interfaceImplementation, object data)
		{
			return base.VisitInterfaceImplementation(interfaceImplementation, data);
		}
		
		public override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			return base.VisitInvocationExpression(invocationExpression, data);
		}
		
		public override object VisitLabelStatement(LabelStatement labelStatement, object data)
		{
			return base.VisitLabelStatement(labelStatement, data);
		}
		
		public override object VisitLambdaExpression(LambdaExpression lambdaExpression, object data)
		{
			return base.VisitLambdaExpression(lambdaExpression, data);
		}
		
		public override object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			WriteLine("VisitLocalVariableDeclaration");
			return base.VisitLocalVariableDeclaration(localVariableDeclaration, data);
		}
		
		public override object VisitLockStatement(LockStatement lockStatement, object data)
		{
			return base.VisitLockStatement(lockStatement, data);
		}
		
		public override object VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			WriteLine("VisitMemberReferenceExpression");
			return base.VisitMemberReferenceExpression(memberReferenceExpression, data);
		}
		
		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			WriteLine("VisitMethodDeclaration");
			using (IDisposable indentLevel = Indentation.IncrementLevel()) {
				return base.VisitMethodDeclaration(methodDeclaration, data);
			}
		}
		
		public override object VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data)
		{
			return base.VisitNamedArgumentExpression(namedArgumentExpression, data);
		}
		
		public override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			WriteLine("VisitNamespaceDeclaration");
			return base.VisitNamespaceDeclaration(namespaceDeclaration, data);
		}
		
		public override object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			WriteLine("VisitObjectCreateExpression");
			return base.VisitObjectCreateExpression(objectCreateExpression, data);
		}
		
		public override object VisitOnErrorStatement(OnErrorStatement onErrorStatement, object data)
		{
			return base.VisitOnErrorStatement(onErrorStatement, data);
		}
		
		public override object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			return base.VisitOperatorDeclaration(operatorDeclaration, data);
		}
		
		public override object VisitOptionDeclaration(OptionDeclaration optionDeclaration, object data)
		{
			return base.VisitOptionDeclaration(optionDeclaration, data);
		}
		
		public override object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			return base.VisitParameterDeclarationExpression(parameterDeclarationExpression, data);
		}
		
		public override object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return base.VisitParenthesizedExpression(parenthesizedExpression, data);
		}
		
		public override object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			return base.VisitPointerReferenceExpression(pointerReferenceExpression, data);
		}
		
		public override object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			return base.VisitPrimitiveExpression(primitiveExpression, data);
		}
		
		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			return base.VisitPropertyDeclaration(propertyDeclaration, data);
		}
		
		public override object VisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data)
		{
			return base.VisitPropertyGetRegion(propertyGetRegion, data);
		}
		
		public override object VisitPropertySetRegion(PropertySetRegion propertySetRegion, object data)
		{
			return base.VisitPropertySetRegion(propertySetRegion, data);
		}
		
		public override object VisitQueryExpression(QueryExpression queryExpression, object data)
		{
			return base.VisitQueryExpression(queryExpression, data);
		}
		
		public override object VisitQueryExpressionAggregateClause(QueryExpressionAggregateClause queryExpressionAggregateClause, object data)
		{
			return base.VisitQueryExpressionAggregateClause(queryExpressionAggregateClause, data);
		}
		
		public override object VisitQueryExpressionDistinctClause(QueryExpressionDistinctClause queryExpressionDistinctClause, object data)
		{
			return base.VisitQueryExpressionDistinctClause(queryExpressionDistinctClause, data);
		}
		
		public override object VisitQueryExpressionFromClause(QueryExpressionFromClause queryExpressionFromClause, object data)
		{
			return base.VisitQueryExpressionFromClause(queryExpressionFromClause, data);
		}
		
		public override object VisitQueryExpressionGroupClause(QueryExpressionGroupClause queryExpressionGroupClause, object data)
		{
			return base.VisitQueryExpressionGroupClause(queryExpressionGroupClause, data);
		}
		
		public override object VisitQueryExpressionGroupJoinVBClause(QueryExpressionGroupJoinVBClause queryExpressionGroupJoinVBClause, object data)
		{
			return base.VisitQueryExpressionGroupJoinVBClause(queryExpressionGroupJoinVBClause, data);
		}
		
		public override object VisitQueryExpressionGroupVBClause(QueryExpressionGroupVBClause queryExpressionGroupVBClause, object data)
		{
			return base.VisitQueryExpressionGroupVBClause(queryExpressionGroupVBClause, data);
		}
		
		public override object VisitQueryExpressionJoinClause(QueryExpressionJoinClause queryExpressionJoinClause, object data)
		{
			return base.VisitQueryExpressionJoinClause(queryExpressionJoinClause, data);
		}
		
		public override object VisitQueryExpressionJoinConditionVB(QueryExpressionJoinConditionVB queryExpressionJoinConditionVB, object data)
		{
			return base.VisitQueryExpressionJoinConditionVB(queryExpressionJoinConditionVB, data);
		}
		
		public override object VisitQueryExpressionJoinVBClause(QueryExpressionJoinVBClause queryExpressionJoinVBClause, object data)
		{
			return base.VisitQueryExpressionJoinVBClause(queryExpressionJoinVBClause, data);
		}
		
		public override object VisitQueryExpressionLetClause(QueryExpressionLetClause queryExpressionLetClause, object data)
		{
			return base.VisitQueryExpressionLetClause(queryExpressionLetClause, data);
		}
		
		public override object VisitQueryExpressionLetVBClause(QueryExpressionLetVBClause queryExpressionLetVBClause, object data)
		{
			return base.VisitQueryExpressionLetVBClause(queryExpressionLetVBClause, data);
		}
		
		public override object VisitQueryExpressionOrderClause(QueryExpressionOrderClause queryExpressionOrderClause, object data)
		{
			return base.VisitQueryExpressionOrderClause(queryExpressionOrderClause, data);
		}
		
		public override object VisitQueryExpressionOrdering(QueryExpressionOrdering queryExpressionOrdering, object data)
		{
			return base.VisitQueryExpressionOrdering(queryExpressionOrdering, data);
		}
		
		public override object VisitQueryExpressionPartitionVBClause(QueryExpressionPartitionVBClause queryExpressionPartitionVBClause, object data)
		{
			return base.VisitQueryExpressionPartitionVBClause(queryExpressionPartitionVBClause, data);
		}
		
		public override object VisitQueryExpressionSelectClause(QueryExpressionSelectClause queryExpressionSelectClause, object data)
		{
			return base.VisitQueryExpressionSelectClause(queryExpressionSelectClause, data);
		}
		
		public override object VisitQueryExpressionSelectVBClause(QueryExpressionSelectVBClause queryExpressionSelectVBClause, object data)
		{
			return base.VisitQueryExpressionSelectVBClause(queryExpressionSelectVBClause, data);
		}
		
		public override object VisitQueryExpressionWhereClause(QueryExpressionWhereClause queryExpressionWhereClause, object data)
		{
			return base.VisitQueryExpressionWhereClause(queryExpressionWhereClause, data);
		}
		
		public override object VisitRaiseEventStatement(RaiseEventStatement raiseEventStatement, object data)
		{
			return base.VisitRaiseEventStatement(raiseEventStatement, data);
		}
		
		public override object VisitReDimStatement(ReDimStatement reDimStatement, object data)
		{
			return base.VisitReDimStatement(reDimStatement, data);
		}
		
		public override object VisitRemoveHandlerStatement(RemoveHandlerStatement removeHandlerStatement, object data)
		{
			return base.VisitRemoveHandlerStatement(removeHandlerStatement, data);
		}
		
		public override object VisitResumeStatement(ResumeStatement resumeStatement, object data)
		{
			return base.VisitResumeStatement(resumeStatement, data);
		}
		
		public override object VisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			return base.VisitReturnStatement(returnStatement, data);
		}
		
		public override object VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
		{
			return base.VisitSizeOfExpression(sizeOfExpression, data);
		}
		
		public override object VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
		{
			return base.VisitStackAllocExpression(stackAllocExpression, data);
		}
		
		public override object VisitStopStatement(StopStatement stopStatement, object data)
		{
			return base.VisitStopStatement(stopStatement, data);
		}
		
		public override object VisitSwitchSection(SwitchSection switchSection, object data)
		{
			return base.VisitSwitchSection(switchSection, data);
		}
		
		public override object VisitSwitchStatement(SwitchStatement switchStatement, object data)
		{
			return base.VisitSwitchStatement(switchStatement, data);
		}
		
		public override object VisitTemplateDefinition(TemplateDefinition templateDefinition, object data)
		{
			return base.VisitTemplateDefinition(templateDefinition, data);
		}
		
		public override object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			WriteLine("VisitThisReferenceExpression");
			return base.VisitThisReferenceExpression(thisReferenceExpression, data);
		}
		
		public override object VisitThrowStatement(ThrowStatement throwStatement, object data)
		{
			return base.VisitThrowStatement(throwStatement, data);
		}
		
		public override object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{
			return base.VisitTryCatchStatement(tryCatchStatement, data);
		}
		
		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			WriteLine("VisitTypeDeclaration");
			using (IDisposable indentLevel = Indentation.IncrementLevel()) {
				return base.VisitTypeDeclaration(typeDeclaration, data);
			}
		}
		
		public override object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			return base.VisitTypeOfExpression(typeOfExpression, data);
		}
		
		public override object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
		{
			return base.VisitTypeOfIsExpression(typeOfIsExpression, data);
		}
		
		public override object VisitTypeReference(TypeReference typeReference, object data)
		{
			return base.VisitTypeReference(typeReference, data);
		}
		
		public override object VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data)
		{
			return base.VisitTypeReferenceExpression(typeReferenceExpression, data);
		}
		
		public override object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			return base.VisitUnaryOperatorExpression(unaryOperatorExpression, data);
		}
		
		public override object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			return base.VisitUncheckedExpression(uncheckedExpression, data);
		}
		
		public override object VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
		{
			return base.VisitUncheckedStatement(uncheckedStatement, data);
		}
		
		public override object VisitUnsafeStatement(UnsafeStatement unsafeStatement, object data)
		{
			return base.VisitUnsafeStatement(unsafeStatement, data);
		}
		
		public override object VisitUsing(Using @using, object data)
		{
			WriteLine("VisitUsing");
			return base.VisitUsing(@using, data);
		}
		
		public override object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			WriteLine("VisitUsingDeclaration");
			return base.VisitUsingDeclaration(usingDeclaration, data);
		}
		
		public override object VisitUsingStatement(UsingStatement usingStatement, object data)
		{
			WriteLine("VisitUsingStatement");
			return base.VisitUsingStatement(usingStatement, data);
		}
		
		public override object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			WriteLine("VisitVariableDeclaration");
			return base.VisitVariableDeclaration(variableDeclaration, data);
		}
		
		public override object VisitWithStatement(WithStatement withStatement, object data)
		{
			return base.VisitWithStatement(withStatement, data);
		}
		
		public override object VisitYieldStatement(YieldStatement yieldStatement, object data)
		{
			return base.VisitYieldStatement(yieldStatement, data);
		}
		
		/// <summary>
		/// Writes a line and indents it to the current level.
		/// </summary>
		void WriteLine(string s)
		{
			writer.WriteLine(GetIndent() + s);
		}
		
		string GetIndent()
		{
			StringBuilder indent = new StringBuilder();
			for (int i = 0; i < Indentation.CurrentLevel; ++i) {
				indent.Append('\t');
			}
			return indent.ToString();
		}		
	}
}
