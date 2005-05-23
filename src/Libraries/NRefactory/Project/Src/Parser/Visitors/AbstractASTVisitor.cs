using System;
using System.Diagnostics;
using System.Collections;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Parser
{
	public abstract class AbstractASTVisitor : IASTVisitor
	{
		protected Stack blockStack = new Stack();
		
		public BlockStatement CurrentBlock {
			get {
				if (blockStack.Count == 0) {
					return null;
				}
				return (BlockStatement)blockStack.Peek();
			}
		}
		
		public virtual object Visit(INode node, object data)
		{
			Console.WriteLine("Warning, INode visited!");
			Console.WriteLine("Type is " + node.GetType());
			Console.WriteLine("Visitor is " + this.GetType());
			return node.AcceptChildren(this, data);
		}
		
		public virtual object Visit(CompilationUnit compilationUnit, object data)
		{
			if (compilationUnit == null) {
				return data;
			}
			return compilationUnit.AcceptChildren(this, data);
		}
		
		public virtual object Visit(TypeReference typeReference, object data)
		{
			Debug.Assert(typeReference != null);
			return data;
		}
		
		public virtual object Visit(AttributeSection attributeSection, object data)
		{
			Debug.Assert(attributeSection != null);
			Debug.Assert(attributeSection.Attributes != null);
			foreach (ICSharpCode.NRefactory.Parser.AST.Attribute attribute in attributeSection.Attributes) {
				Debug.Assert(attribute != null);
				attribute.AcceptVisitor(this, data);
			}
			return data;
		}
		
		public virtual object Visit(ICSharpCode.NRefactory.Parser.AST.Attribute attribute, object data)
		{
			Debug.Assert(attribute != null);
			Debug.Assert(attribute.PositionalArguments != null);
			Debug.Assert(attribute.NamedArguments != null);
			foreach (Expression e in attribute.PositionalArguments) {
				Debug.Assert(e != null);
				e.AcceptVisitor(this, data);
			}
			foreach (NamedArgumentExpression n in attribute.NamedArguments) {
				Debug.Assert(n != null);
				n.AcceptVisitor(this, data);
			}
			return data;
		}
		
		public virtual object Visit(NamedArgumentExpression namedArgumentExpression, object data)
		{
			Debug.Assert(namedArgumentExpression != null);
			Debug.Assert(namedArgumentExpression.Expression != null);
			return namedArgumentExpression.Expression.AcceptVisitor(this, data);
		}
		
		#region global scope
		public virtual object Visit(Using @using, object data)
		{
			Debug.Assert(@using != null);
			return data;
		}
		
		public virtual object Visit(UsingDeclaration usingDeclaration, object data)
		{
			Debug.Assert(usingDeclaration != null);
			foreach (Using ud in usingDeclaration.Usings) {
				ud.AcceptVisitor(this, data);
			}
			return data;
		}
		
		public virtual object Visit(NamespaceDeclaration namespaceDeclaration, object data)
		{
			Debug.Assert(namespaceDeclaration != null);
			return namespaceDeclaration.AcceptChildren(this, data);
		}
		
		public virtual object Visit(TypeDeclaration typeDeclaration, object data)
		{
			Debug.Assert(typeDeclaration != null);
			Debug.Assert(typeDeclaration.Attributes != null);
			foreach (AttributeSection section in typeDeclaration.Attributes) {
				section.AcceptVisitor(this, data);
			}
			foreach (TemplateDefinition templateDefinition in typeDeclaration.Templates) {
				templateDefinition.AcceptVisitor(this, data);
			}
			return typeDeclaration.AcceptChildren(this, data);
		}
		
		public virtual object Visit(TemplateDefinition templateDefinition, object data)
		{
			object o = null;
			foreach (TypeReference typeReference in templateDefinition.Bases) {
				o = typeReference.AcceptVisitor(this, data);
			}
			return o;
		}
		
		public virtual object Visit(DelegateDeclaration delegateDeclaration, object data)
		{
			Debug.Assert(delegateDeclaration != null);
			Debug.Assert(delegateDeclaration.Attributes != null);
			Debug.Assert(delegateDeclaration.Parameters != null);
			Debug.Assert(delegateDeclaration.ReturnType != null);
			
			foreach (AttributeSection section in delegateDeclaration.Attributes) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			
			foreach (ParameterDeclarationExpression p in delegateDeclaration.Parameters) {
				Debug.Assert(p != null);
				p.AcceptVisitor(this, data);
			}
			return delegateDeclaration.ReturnType.AcceptVisitor(this, data);
		}
		// VB only:
		public virtual object Visit(OptionDeclaration optionDeclaration, object data)
		{
			Debug.Assert(optionDeclaration != null);
			return data;
		}
		#endregion
		
		#region type level
		public virtual object Visit(FieldDeclaration fieldDeclaration, object data)
		{
			Debug.Assert(fieldDeclaration != null);
			Debug.Assert(fieldDeclaration.Attributes != null);
			Debug.Assert(fieldDeclaration.TypeReference != null);
			Debug.Assert(fieldDeclaration.Fields != null);
			
			foreach (AttributeSection section in fieldDeclaration.Attributes) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			fieldDeclaration.TypeReference.AcceptVisitor(this, data);
			foreach (VariableDeclaration var in fieldDeclaration.Fields) {
				Debug.Assert(var != null);
				var.AcceptVisitor(this, fieldDeclaration);
			}
			return data;
		}
		
		public virtual object Visit(VariableDeclaration variableDeclaration, object data)
		{
			Debug.Assert(variableDeclaration != null);
			Debug.Assert(variableDeclaration.Initializer != null);
			return variableDeclaration.Initializer.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(PropertyDeclaration propertyDeclaration, object data)
		{
			Debug.Assert(propertyDeclaration != null);
			Debug.Assert(propertyDeclaration.Attributes != null);
			Debug.Assert(propertyDeclaration.TypeReference != null);
			Debug.Assert(propertyDeclaration.GetRegion != null);
			Debug.Assert(propertyDeclaration.SetRegion != null);
			
			foreach (AttributeSection section in propertyDeclaration.Attributes) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			propertyDeclaration.TypeReference.AcceptVisitor(this, data);
			propertyDeclaration.GetRegion.AcceptVisitor(this, data);
			propertyDeclaration.SetRegion.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(PropertyGetRegion propertyGetRegion, object data)
		{
			Debug.Assert(propertyGetRegion != null);
			Debug.Assert(propertyGetRegion.Attributes != null);
			Debug.Assert(propertyGetRegion.Block != null);
			
			foreach (AttributeSection section in propertyGetRegion.Attributes) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			blockStack.Push(propertyGetRegion.Block);
			propertyGetRegion.Block.AcceptChildren(this, data);
			blockStack.Pop();
			return data;
		}
		
		public virtual object Visit(PropertySetRegion propertySetRegion, object data)
		{
			Debug.Assert(propertySetRegion != null);
			Debug.Assert(propertySetRegion.Attributes != null);
			Debug.Assert(propertySetRegion.Block != null);
			
			foreach (AttributeSection section in propertySetRegion.Attributes) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			blockStack.Push(propertySetRegion.Block);
			propertySetRegion.Block.AcceptChildren(this, data);
			blockStack.Pop();
			return data;
		}
		
		public virtual object Visit(EventDeclaration eventDeclaration, object data)
		{
			Debug.Assert(eventDeclaration != null);
			Debug.Assert(eventDeclaration.Attributes != null);
			Debug.Assert(eventDeclaration.TypeReference != null);
			Debug.Assert(eventDeclaration.Parameters != null);
			Debug.Assert(eventDeclaration.VariableDeclarators != null);
			Debug.Assert(eventDeclaration.AddRegion != null);
			Debug.Assert(eventDeclaration.RemoveRegion != null);
			
			foreach (AttributeSection section in eventDeclaration.Attributes) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			eventDeclaration.TypeReference.AcceptVisitor(this, data);
			foreach (ParameterDeclarationExpression p in eventDeclaration.Parameters) {
				Debug.Assert(p != null);
				p.AcceptVisitor(this, data);
			}
			foreach (VariableDeclaration v in eventDeclaration.VariableDeclarators) {
				Debug.Assert(v != null);
				v.AcceptVisitor(this, data);
			}
			eventDeclaration.AddRegion.AcceptVisitor(this, data);
			eventDeclaration.RemoveRegion.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(EventAddRegion eventAddRegion, object data)
		{
			Debug.Assert(eventAddRegion != null);
			Debug.Assert(eventAddRegion.Attributes != null);
			Debug.Assert(eventAddRegion.Block != null);
			foreach (AttributeSection section in eventAddRegion.Attributes) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			blockStack.Push(eventAddRegion.Block);
			eventAddRegion.Block.AcceptChildren(this, data);
			blockStack.Pop();
			return data;
		}
		
		public virtual object Visit(EventRemoveRegion eventRemoveRegion, object data)
		{
			Debug.Assert(eventRemoveRegion != null);
			Debug.Assert(eventRemoveRegion.Attributes != null);
			Debug.Assert(eventRemoveRegion.Block != null);
			
			foreach (AttributeSection section in eventRemoveRegion.Attributes) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			blockStack.Push(eventRemoveRegion.Block);
			eventRemoveRegion.Block.AcceptChildren(this, data);
			blockStack.Pop();
			return data;
		}
		
		// general:
		public virtual object Visit(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			Debug.Assert(parameterDeclarationExpression != null);
			Debug.Assert(parameterDeclarationExpression.Attributes != null);
			Debug.Assert(parameterDeclarationExpression.TypeReference != null);
			foreach (AttributeSection section in parameterDeclarationExpression.Attributes) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			return parameterDeclarationExpression.TypeReference.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(MethodDeclaration methodDeclaration, object data)
		{
			Debug.Assert(methodDeclaration != null);
			Debug.Assert(methodDeclaration.Attributes != null);
			Debug.Assert(methodDeclaration.TypeReference != null);
			Debug.Assert(methodDeclaration.Body != null);
			
			foreach (AttributeSection section in methodDeclaration.Attributes) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			methodDeclaration.TypeReference.AcceptVisitor(this, data);
			foreach (ParameterDeclarationExpression p in methodDeclaration.Parameters) {
				p.AcceptVisitor(this, data);
			}
			blockStack.Push(methodDeclaration.Body);
			methodDeclaration.Body.AcceptChildren(this, data);
			blockStack.Pop();
			return data;
		}
		
		public virtual object Visit(ConstructorDeclaration constructorDeclaration, object data)
		{
			Debug.Assert(constructorDeclaration != null);
			Debug.Assert(constructorDeclaration.Attributes != null);
			Debug.Assert(constructorDeclaration.Parameters != null);
			Debug.Assert(constructorDeclaration.ConstructorInitializer != null);
			Debug.Assert(constructorDeclaration.Body != null);
			
			foreach (AttributeSection section in constructorDeclaration.Attributes) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			foreach (ParameterDeclarationExpression p in constructorDeclaration.Parameters) {
				Debug.Assert(p != null);
				p.AcceptVisitor(this, data);
			}
			constructorDeclaration.ConstructorInitializer.AcceptVisitor(this, data);
			blockStack.Push(constructorDeclaration.Body);
			constructorDeclaration.Body.AcceptChildren(this, data);
			blockStack.Pop();
			return data;
		}
		
		public virtual object Visit(IndexerDeclaration indexerDeclaration, object data)
		{
			Debug.Assert(indexerDeclaration != null);
			Debug.Assert(indexerDeclaration.Attributes != null);
			Debug.Assert(indexerDeclaration.Parameters != null);
			Debug.Assert(indexerDeclaration.GetRegion != null);
			Debug.Assert(indexerDeclaration.SetRegion != null);
			
			foreach (AttributeSection section in indexerDeclaration.Attributes) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			foreach (ParameterDeclarationExpression p in indexerDeclaration.Parameters) {
				p.AcceptVisitor(this, data);
			}
			indexerDeclaration.GetRegion.AcceptVisitor(this, data);
			indexerDeclaration.SetRegion.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(ConstructorInitializer constructorInitializer, object data)
		{
			Debug.Assert(constructorInitializer != null);
			Debug.Assert(constructorInitializer.Arguments != null);
			
			foreach (Expression expr in constructorInitializer.Arguments) {
				Debug.Assert(expr != null);
				expr.AcceptVisitor(this, data);
			}
			return data;
		}
		
		public virtual object Visit(DestructorDeclaration destructorDeclaration, object data)
		{
			Debug.Assert(destructorDeclaration != null);
			Debug.Assert(destructorDeclaration.Attributes != null);
			Debug.Assert(destructorDeclaration.Body != null);
			
			foreach (AttributeSection section in destructorDeclaration.Attributes) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			blockStack.Push(destructorDeclaration.Body);
			destructorDeclaration.Body.AcceptChildren(this, data);
			blockStack.Pop();
			return data;
		}
		
		public virtual object Visit(OperatorDeclaration operatorDeclaration, object data)
		{
			Debug.Assert(operatorDeclaration != null);
			Debug.Assert(operatorDeclaration.Attributes != null);
			Debug.Assert(operatorDeclaration.Body != null);
			foreach (AttributeSection section in operatorDeclaration.Attributes) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			blockStack.Push(operatorDeclaration.Body);
			operatorDeclaration.Body.AcceptChildren(this, data);
			blockStack.Pop();
			return data;
		}
		
		// VB only:
		public virtual object Visit(DeclareDeclaration declareDeclaration, object data)
		{
			Debug.Assert(declareDeclaration != null);
			Debug.Assert(declareDeclaration.Attributes != null);
			Debug.Assert(declareDeclaration.TypeReference != null);
			Debug.Assert(declareDeclaration.Parameters != null);
			
			foreach (AttributeSection section in declareDeclaration.Attributes) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			declareDeclaration.TypeReference.AcceptVisitor(this, data);
			foreach (ParameterDeclarationExpression p in declareDeclaration.Parameters) {
				Debug.Assert(p != null);
				p.AcceptVisitor(this, data);
			}
			return data;
		}
		#endregion
		
		#region statements
		public virtual object Visit(BlockStatement blockStatement, object data)
		{
			Debug.Assert(blockStatement != null);
			blockStack.Push(blockStatement);
			blockStatement.AcceptChildren(this, data);
			blockStack.Pop();
			return data;
		}
		
		public virtual object Visit(AddHandlerStatement addHandlerStatement, object data)
		{
			Debug.Assert(addHandlerStatement != null);
			Debug.Assert(addHandlerStatement.EventExpression != null);
			Debug.Assert(addHandlerStatement.HandlerExpression != null);
			
			addHandlerStatement.EventExpression.AcceptVisitor(this, data);
			addHandlerStatement.HandlerExpression.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(RemoveHandlerStatement removeHandlerStatement, object data)
		{
			Debug.Assert(removeHandlerStatement != null);
			Debug.Assert(removeHandlerStatement.EventExpression != null);
			Debug.Assert(removeHandlerStatement.HandlerExpression != null);
			
			removeHandlerStatement.EventExpression.AcceptVisitor(this, data);
			removeHandlerStatement.HandlerExpression.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(RaiseEventStatement raiseEventStatement, object data)
		{
			Debug.Assert(raiseEventStatement != null);
			Debug.Assert(raiseEventStatement.Parameters != null);
			foreach (ParameterDeclarationExpression p in raiseEventStatement.Parameters) {
				Debug.Assert(p != null);
				p.AcceptVisitor(this, data);
			}
			return data;
		}
		
		public virtual object Visit(EraseStatement eraseStatement, object data)
		{
			Debug.Assert(eraseStatement != null);
			Debug.Assert(eraseStatement.Expressions != null);
			foreach (Expression e in eraseStatement.Expressions) {
				Debug.Assert(e != null);
				e.AcceptVisitor(this, data);
			}
			return data;
		}
		
		public virtual object Visit(ErrorStatement errorStatement, object data)
		{
			Debug.Assert(errorStatement != null);
			Debug.Assert(errorStatement.Expression != null);
			errorStatement.Expression.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(OnErrorStatement onErrorStatement, object data)
		{
			Debug.Assert(onErrorStatement != null);
			Debug.Assert(onErrorStatement.EmbeddedStatement != null);
			onErrorStatement.EmbeddedStatement.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(ReDimStatement reDimStatement, object data)
		{
			Debug.Assert(reDimStatement != null);
			Debug.Assert(reDimStatement.ReDimClauses != null);
			foreach (Expression clause in reDimStatement.ReDimClauses) {
				Debug.Assert(clause != null);
				clause.AcceptVisitor(this, data);
			}
			return data;
		}
		
//		public virtual object Visit(ReDimClause reDimClause, object data)
//		{
//			Debug.Assert(reDimClause != null);
//			Debug.Assert(reDimClause.Initializers != null);
//			foreach (Expression e in reDimClause.Initializers) {
//				Debug.Assert(e != null);
//				e.AcceptVisitor(this, data);
//			}
//			return data;
//		}
		
		public virtual object Visit(UsingStatement usingStatement, object data)
		{
			Debug.Assert(usingStatement != null);
			Debug.Assert(usingStatement.ResourceAcquisition != null);
			Debug.Assert(usingStatement.EmbeddedStatement != null);
			usingStatement.ResourceAcquisition.AcceptVisitor(this, data);
			usingStatement.EmbeddedStatement.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(WithStatement withStatement, object data)
		{
			Debug.Assert(withStatement != null);
			Debug.Assert(withStatement.Expression  != null);
			Debug.Assert(withStatement.Body != null);
			withStatement.Expression.AcceptVisitor(this, data);
			withStatement.Body.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(EmptyStatement emptyStatement, object data)
		{
			Debug.Assert(emptyStatement != null);
			return data;
		}
		
		public virtual object Visit(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			Debug.Assert(localVariableDeclaration != null);
			Debug.Assert(localVariableDeclaration.TypeReference != null);
			Debug.Assert(localVariableDeclaration.Variables != null);
			localVariableDeclaration.TypeReference.AcceptVisitor(this, data);
			foreach (VariableDeclaration decl in localVariableDeclaration.Variables) {
				Debug.Assert(decl != null);
				decl.AcceptVisitor(this, data);
			}
			return data;
		}
		
		public virtual object Visit(IfElseStatement ifElseStatement, object data)
		{
			Debug.Assert(ifElseStatement != null);
			Debug.Assert(ifElseStatement.Condition != null);
			Debug.Assert(ifElseStatement.TrueStatement != null);
			Debug.Assert(ifElseStatement.ElseIfSections != null);
			Debug.Assert(ifElseStatement.FalseStatement != null);
			
			ifElseStatement.Condition.AcceptVisitor(this, data);
			foreach (Statement stmt in ifElseStatement.TrueStatement) {
				Debug.Assert(stmt != null);
				stmt.AcceptVisitor(this, data);
			}
			
			foreach (ElseIfSection elseIfSection in ifElseStatement.ElseIfSections) {
				Debug.Assert(elseIfSection != null);
				elseIfSection.AcceptVisitor(this, data);
			}
			
			foreach (Statement stmt in ifElseStatement.FalseStatement) {
				Debug.Assert(stmt != null);
				stmt.AcceptVisitor(this, data);
			}
			return data;
		}
		
		public virtual object Visit(ElseIfSection elseIfSection, object data)
		{
			Debug.Assert(elseIfSection != null);
			Debug.Assert(elseIfSection.Condition != null);
			Debug.Assert(elseIfSection.EmbeddedStatement != null);
			
			elseIfSection.Condition.AcceptVisitor(this,data);
			elseIfSection.EmbeddedStatement.AcceptVisitor(this,data);
			return data;
		}
		
		public virtual object Visit(DoLoopStatement doLoopStatement, object data)
		{
			Debug.Assert(doLoopStatement != null);
			Debug.Assert(doLoopStatement.Condition != null);
			Debug.Assert(doLoopStatement.EmbeddedStatement != null);
			
			doLoopStatement.Condition.AcceptVisitor(this, data);
			doLoopStatement.EmbeddedStatement.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(ForStatement forStatement, object data)
		{
			Debug.Assert(forStatement != null);
			Debug.Assert(forStatement.Initializers != null);
			Debug.Assert(forStatement.Condition != null);
			Debug.Assert(forStatement.Iterator != null);
			Debug.Assert(forStatement.EmbeddedStatement != null);
			
			foreach(INode n in forStatement.Initializers) {
				Debug.Assert(n != null);
				n.AcceptVisitor(this, data);
			}
			forStatement.Condition.AcceptVisitor(this, data);
			foreach(INode n in forStatement.Iterator) {
				Debug.Assert(n != null);
				n.AcceptVisitor(this, data);
			}
			return forStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(ForeachStatement foreachStatement, object data)
		{
			Debug.Assert(foreachStatement != null);
			Debug.Assert(foreachStatement.TypeReference != null);
			Debug.Assert(foreachStatement.Expression != null);
			Debug.Assert(foreachStatement.EmbeddedStatement != null);
			
			foreachStatement.TypeReference.AcceptVisitor(this, data);
			foreachStatement.Expression.AcceptVisitor(this, data);
			return foreachStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(LabelStatement labelStatement, object data)
		{
			Debug.Assert(labelStatement != null);
			return data;
		}
		
		public virtual object Visit(GotoStatement gotoStatement, object data)
		{
			Debug.Assert(gotoStatement != null);
			return data;
		}
		
		public virtual object Visit(GotoCaseStatement gotoCaseStatement, object data)
		{
			Debug.Assert(gotoCaseStatement != null);
			Debug.Assert(gotoCaseStatement.Expression != null);
			gotoCaseStatement.Expression.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(BreakStatement breakStatement, object data)
		{
			Debug.Assert(breakStatement != null);
			return data;
		}
		
		public virtual object Visit(StopStatement stopStatement, object data)
		{
			Debug.Assert(stopStatement != null);
			return data;
		}
		
		public virtual object Visit(ResumeStatement resumeStatement, object data)
		{
			Debug.Assert(resumeStatement != null);
			return data;
		}
		
		public virtual object Visit(EndStatement endStatement, object data)
		{
			Debug.Assert(endStatement != null);
			return data;
		}
		
		public virtual object Visit(ContinueStatement continueStatement, object data)
		{
			Debug.Assert(continueStatement != null);
			return data;
		}
		
		public virtual object Visit(YieldStatement yieldStatement, object data)
		{
			Debug.Assert(yieldStatement != null);
			Debug.Assert(yieldStatement.Statement != null);
			return yieldStatement.Statement.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(ReturnStatement returnStatement, object data)
		{
			Debug.Assert(returnStatement != null);
			Debug.Assert(returnStatement.Expression != null);
			return returnStatement.Expression.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(SwitchStatement switchStatement, object data)
		{
			Debug.Assert(switchStatement != null);
			Debug.Assert(switchStatement.SwitchExpression != null);
			Debug.Assert(switchStatement.SwitchSections != null);
			
			switchStatement.SwitchExpression.AcceptVisitor(this, data);
			foreach (SwitchSection section in switchStatement.SwitchSections) {
				Debug.Assert(section != null);
				section.AcceptVisitor(this, data);
			}
			return data;
		}
		
		public virtual object Visit(SwitchSection switchSection, object data)
		{
			Debug.Assert(switchSection != null);
			Debug.Assert(switchSection.SwitchLabels != null);
			
			foreach (CaseLabel label in switchSection.SwitchLabels) {
				Debug.Assert(label != null);
				label.AcceptVisitor(this, data);
			}
			return switchSection.AcceptChildren(this, data);
		}
		
		public virtual object Visit(CaseLabel caseLabel, object data)
		{
			Debug.Assert(caseLabel != null);
			Debug.Assert(caseLabel.Label != null);
			return caseLabel.Label.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(LockStatement lockStatement, object data)
		{
			Debug.Assert(lockStatement != null);
			Debug.Assert(lockStatement.EmbeddedStatement != null);
			return lockStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(FixedStatement fixedStatement, object data)
		{
			Debug.Assert(fixedStatement != null);
			Debug.Assert(fixedStatement.TypeReference != null);
			Debug.Assert(fixedStatement.PointerDeclarators != null);
			Debug.Assert(fixedStatement.EmbeddedStatement != null);
			
			fixedStatement.TypeReference.AcceptVisitor(this, data);
			foreach (VariableDeclaration var in fixedStatement.PointerDeclarators) {
				Debug.Assert(var != null);
				var.AcceptVisitor(this, data);
			}
			fixedStatement.EmbeddedStatement.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(UnsafeStatement unsafeStatement, object data)
		{
			Debug.Assert(unsafeStatement != null);
			Debug.Assert(unsafeStatement.Block != null);
			unsafeStatement.Block.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(CheckedStatement checkedStatement, object data)
		{
			Debug.Assert(checkedStatement != null);
			Debug.Assert(checkedStatement.Block != null);
			checkedStatement.Block.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(UncheckedStatement uncheckedStatement, object data)
		{
			Debug.Assert(uncheckedStatement != null);
			Debug.Assert(uncheckedStatement.Block != null);
			uncheckedStatement.Block.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(TryCatchStatement tryCatchStatement, object data)
		{
			Debug.Assert(tryCatchStatement != null);
			Debug.Assert(tryCatchStatement.StatementBlock != null);
			Debug.Assert(tryCatchStatement.CatchClauses != null);
			Debug.Assert(tryCatchStatement.FinallyBlock != null);
			
			tryCatchStatement.StatementBlock.AcceptVisitor(this, data);
			foreach (CatchClause catchClause in tryCatchStatement.CatchClauses) {
				Debug.Assert(catchClause != null);
				catchClause.AcceptVisitor(this, data);
			}
			return tryCatchStatement.FinallyBlock.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(CatchClause catchClause, object data)
		{
			Debug.Assert(catchClause != null);
			Debug.Assert(catchClause.StatementBlock != null);
			return catchClause.StatementBlock.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(ThrowStatement throwStatement, object data)
		{
			Debug.Assert(throwStatement != null);
			Debug.Assert(throwStatement.Expression != null);
			return throwStatement.Expression.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(StatementExpression statementExpression, object data)
		{
			Debug.Assert(statementExpression != null);
			Debug.Assert(statementExpression.Expression != null);
			return statementExpression.Expression.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(ExitStatement exitStatement, object data)
		{
			Debug.Assert(exitStatement != null);
			return data;
		}
		
		public virtual object Visit(ForNextStatement forNextStatement, object data)
		{
			Debug.Assert(forNextStatement != null);
			Debug.Assert(forNextStatement.Start != null);
			Debug.Assert(forNextStatement.End != null);
			Debug.Assert(forNextStatement.Step != null);
			Debug.Assert(forNextStatement.EmbeddedStatement != null);
			Debug.Assert(forNextStatement.TypeReference != null);
			
			forNextStatement.Start.AcceptVisitor(this, data);
			forNextStatement.End.AcceptVisitor(this, data);
			forNextStatement.Step.AcceptVisitor(this, data);
			forNextStatement.EmbeddedStatement.AcceptVisitor(this, data);
			forNextStatement.TypeReference.AcceptVisitor(this, data);
			return data;
		}
		
		#endregion
		
		#region expressions
		public virtual object Visit(PrimitiveExpression primitiveExpression, object data)
		{
			Debug.Assert(primitiveExpression != null);
			return data;
		}
		
		public virtual object Visit(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			Debug.Assert(unaryOperatorExpression != null);
			Debug.Assert(unaryOperatorExpression.Expression != null);
			return unaryOperatorExpression.Expression.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			Debug.Assert(binaryOperatorExpression != null);
			Debug.Assert(binaryOperatorExpression.Left != null);
			Debug.Assert(binaryOperatorExpression.Right != null);
			
			binaryOperatorExpression.Left.AcceptVisitor(this, data);
			return binaryOperatorExpression.Right.AcceptVisitor(this, data);;
		}
		
		public virtual object Visit(ConditionalExpression conditionalExpression, object data)
		{
			Debug.Assert(conditionalExpression != null);
			Debug.Assert(conditionalExpression.Condition != null);
			Debug.Assert(conditionalExpression.TrueExpression != null);
			Debug.Assert(conditionalExpression.FalseExpression != null);
			
			conditionalExpression.Condition.AcceptVisitor(this, data);
			conditionalExpression.TrueExpression.AcceptVisitor(this, data);
			conditionalExpression.FalseExpression.AcceptVisitor(this, data);
			return data;
		}
		public virtual object Visit(ParenthesizedExpression parenthesizedExpression, object data)
		{
			Debug.Assert(parenthesizedExpression != null);
			Debug.Assert(parenthesizedExpression.Expression != null);
			return parenthesizedExpression.Expression.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(ClassReferenceExpression classReferenceExpression, object data)
		{
			Debug.Assert(classReferenceExpression != null);
			
			return data;
		}
		
		public virtual object Visit(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			Debug.Assert(fieldReferenceExpression != null);
			Debug.Assert(fieldReferenceExpression.TargetObject != null);
			return fieldReferenceExpression.TargetObject.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(IndexerExpression indexerExpression, object data)
		{
			Debug.Assert(indexerExpression != null);
			Debug.Assert(indexerExpression.TargetObject != null);
			Debug.Assert(indexerExpression.Indices != null);
			
			indexerExpression.TargetObject.AcceptVisitor(this, data);
			foreach (Expression e in indexerExpression.Indices) {
				Debug.Assert(e != null);
				e.AcceptVisitor(this, data);
			}
			return data;
		}
		
		public virtual object Visit(InvocationExpression invocationExpression, object data)
		{
			Debug.Assert(invocationExpression != null);
			Debug.Assert(invocationExpression.TargetObject != null);
			Debug.Assert(invocationExpression.Parameters != null);
			
			invocationExpression.TargetObject.AcceptVisitor(this, data);
			foreach (Expression e in invocationExpression.Parameters) {
				Debug.Assert(e != null);
				e.AcceptVisitor(this, data);
			}
			return data;
		}
		
		public virtual object Visit(IdentifierExpression identifierExpression, object data)
		{
			Debug.Assert(identifierExpression != null);
			return data;
		}
		
		public virtual object Visit(TypeReferenceExpression typeReferenceExpression, object data)
		{
			Debug.Assert(typeReferenceExpression != null);
			Debug.Assert(typeReferenceExpression.TypeReference != null);
			typeReferenceExpression.TypeReference.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(AssignmentExpression assignmentExpression, object data)
		{
			Debug.Assert(assignmentExpression != null);
			Debug.Assert(assignmentExpression.Left != null);
			Debug.Assert(assignmentExpression.Right != null);
			assignmentExpression.Left.AcceptVisitor(this, data);
			return assignmentExpression.Right.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(CastExpression castExpression, object data)
		{
			Debug.Assert(castExpression != null);
			Debug.Assert(castExpression.CastTo != null);
			Debug.Assert(castExpression.Expression != null);
			castExpression.CastTo.AcceptVisitor(this, data);
			return castExpression.Expression.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(ThisReferenceExpression thisReferenceExpression, object data)
		{
			Debug.Assert(thisReferenceExpression != null);
			return data;
		}
		
		public virtual object Visit(BaseReferenceExpression baseReferenceExpression, object data)
		{
			Debug.Assert(baseReferenceExpression != null);
			return data;
		}
		
		public virtual object Visit(ObjectCreateExpression objectCreateExpression, object data)
		{
			Debug.Assert(objectCreateExpression != null);
			Debug.Assert(objectCreateExpression.CreateType != null);
			Debug.Assert(objectCreateExpression.Parameters != null);
			
			objectCreateExpression.CreateType.AcceptVisitor(this, data);
			foreach (Expression e in objectCreateExpression.Parameters) {
				Debug.Assert(e != null);
				e.AcceptVisitor(this, data);
			}
			return data;
		}
		
		public virtual object Visit(ArrayCreateExpression arrayCreateExpression, object data)
		{
			Debug.Assert(arrayCreateExpression != null);
			Debug.Assert(arrayCreateExpression.CreateType != null);
			Debug.Assert(arrayCreateExpression.Parameters != null);
			Debug.Assert(arrayCreateExpression.ArrayInitializer != null);
			
			arrayCreateExpression.CreateType.AcceptVisitor(this, data);
			foreach (Expression p in arrayCreateExpression.Parameters) {
				Debug.Assert(p != null);
				p.AcceptVisitor(this, data);
			}
			return arrayCreateExpression.ArrayInitializer.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(ArrayInitializerExpression arrayInitializerExpression, object data)
		{
			Debug.Assert(arrayInitializerExpression != null);
			Debug.Assert(arrayInitializerExpression.CreateExpressions != null);
			
			foreach (Expression e in arrayInitializerExpression.CreateExpressions) {
				Debug.Assert(e != null);
				e.AcceptVisitor(this, data);
			}
			return data;
		}
		
		public virtual object Visit(CheckedExpression checkedExpression, object data)
		{
			Debug.Assert(checkedExpression != null);
			Debug.Assert(checkedExpression.Expression != null);
			checkedExpression.Expression.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(UncheckedExpression uncheckedExpression, object data)
		{
			Debug.Assert(uncheckedExpression != null);
			Debug.Assert(uncheckedExpression.Expression != null);
			uncheckedExpression.Expression.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			Debug.Assert(anonymousMethodExpression != null);
			Debug.Assert(anonymousMethodExpression.Parameters != null);
			Debug.Assert(anonymousMethodExpression.Body != null);
			
			foreach (ParameterDeclarationExpression p in anonymousMethodExpression.Parameters) {
				p.AcceptVisitor(this, data);
			}
			blockStack.Push(anonymousMethodExpression.Body);
			anonymousMethodExpression.Body.AcceptChildren(this, data);
			blockStack.Pop();
			
			return data;
		}
		
		public virtual object Visit(TypeOfIsExpression typeOfIsExpression, object data)
		{
			Debug.Assert(typeOfIsExpression != null);
			Debug.Assert(typeOfIsExpression.Expression != null);
			Debug.Assert(typeOfIsExpression.TypeReference != null);
			
			typeOfIsExpression.Expression.AcceptVisitor(this, data);
			typeOfIsExpression.TypeReference.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(TypeOfExpression typeOfExpression, object data)
		{
			Debug.Assert(typeOfExpression != null);
			Debug.Assert(typeOfExpression.TypeReference != null);
			
			typeOfExpression.TypeReference.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(AddressOfExpression addressOfExpression, object data)
		{
			Debug.Assert(addressOfExpression != null);
			Debug.Assert(addressOfExpression.Expression != null);
			
			addressOfExpression.Expression.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(SizeOfExpression sizeOfExpression, object data)
		{
			Debug.Assert(sizeOfExpression != null);
			Debug.Assert(sizeOfExpression.TypeReference != null);
			
			sizeOfExpression.TypeReference.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			Debug.Assert(pointerReferenceExpression != null);
			Debug.Assert(pointerReferenceExpression.TargetObject != null);
			
			pointerReferenceExpression.TargetObject.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(DirectionExpression directionExpression, object data)
		{
			Debug.Assert(directionExpression != null);
			Debug.Assert(directionExpression.Expression != null);
			
			directionExpression.Expression.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(StackAllocExpression stackAllocExpression, object data)
		{
			Debug.Assert(stackAllocExpression != null);
			Debug.Assert(stackAllocExpression.TypeReference != null);
			Debug.Assert(stackAllocExpression.Expression != null);
			stackAllocExpression.TypeReference.AcceptVisitor(this, data);
			stackAllocExpression.Expression.AcceptVisitor(this, data);
			return data;
		}
		
		public virtual object Visit(ArrayCreationParameter arrayCreationParameter, object data)
		{
			Debug.Assert(arrayCreationParameter != null);
			Debug.Assert(arrayCreationParameter.Expressions != null);
			foreach (Expression expr in arrayCreationParameter.Expressions) {
				Debug.Assert(expr != null);
				expr.AcceptVisitor(this, data);
			}
			return data;
		}
		#endregion
	}
}
