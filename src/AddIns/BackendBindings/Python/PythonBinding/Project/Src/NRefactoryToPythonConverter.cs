// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Base class for the CSharpToPythonConverter and
	/// VBNetToPythonConverter. Used to convert VB.NET and C# to 
	/// Python.
	/// </summary>
	public class NRefactoryToPythonConverter : IAstVisitor
	{
		CodeCompileUnit pythonCodeCompileUnit;
		CodeNamespace currentNamespace;
		CodeTypeDeclaration currentClass;
		List<FieldDeclaration> currentClassFields = new List<FieldDeclaration>();
		List<FieldDeclaration> allCurrentClassFields = new List<FieldDeclaration>();
		Stack<CodeStatementCollection> statementsStack = new Stack<CodeStatementCollection>();
		CodeStatementCollection nullStatementCollection = new CodeStatementCollection();

		public NRefactoryToPythonConverter()
		{
		}

		/// <summary>
		/// Generates compilation unit from the code.
		/// </summary>
		/// <param name="source">
		/// The code to convert to a compilation unit.
		/// </param>
		public CompilationUnit GenerateCompilationUnit(string source, SupportedLanguage language)
		{
			using (IParser parser = ParserFactory.CreateParser(language, new StringReader(source))) {
				parser.Parse();
				return parser.CompilationUnit;
			}
		}

		/// <summary>
		/// Converts a code compile unit to Python source code.
		/// </summary>
		public static string ConvertCodeCompileUnitToPython(CodeCompileUnit codeCompileUnit)
		{
			//PythonProvider pythonProvider = new PythonProvider();
			StringWriter writer = new StringWriter();
			CodeGeneratorOptions options = new CodeGeneratorOptions();
			options.BlankLinesBetweenMembers = false;
			options.IndentString = "\t";
			//pythonProvider.GenerateCodeFromCompileUnit(codeCompileUnit, writer, options);
			
			Console.WriteLine("CompileUnit: " + writer.ToString().Trim());
			
			return writer.ToString().Trim();
		}
		
		/// <summary>
		/// Converts the source code to Python.
		/// </summary>
		public string Convert(string source, SupportedLanguage language)
		{
			CodeCompileUnit pythonCodeCompileUnit = ConvertToCodeCompileUnit(source, language);
			
			// Convert to Python source code.
			return ConvertCodeCompileUnitToPython(pythonCodeCompileUnit);
		}
		
		/// <summary>
		/// Converts the C# or VB.NET source code to a code DOM that 
		/// the PythonProvider can use to generate Python. Using the
		/// NRefactory code DOM (CodeDomVisitor class) does not
		/// create a code DOM that is easy to convert to Python. 
		/// </summary>		
		public CodeCompileUnit ConvertToCodeCompileUnit(string source, SupportedLanguage language)
		{
			// Convert to code DOM.
			CompilationUnit unit = GenerateCompilationUnit(source, language);
			
			// Convert to Python code DOM.
			return (CodeCompileUnit)unit.AcceptVisitor(this, null);			
		}

		/// <summary>
		/// Converts from the NRefactory's binary operator type to the
		/// CodeDom's binary operator type.
		/// </summary>
		public static CodeBinaryOperatorType ConvertBinaryOperatorType(BinaryOperatorType binaryOperatorType)
		{
			switch (binaryOperatorType) {
				case BinaryOperatorType.Add:
					return CodeBinaryOperatorType.Add;
				case BinaryOperatorType.BitwiseAnd:
					return CodeBinaryOperatorType.BitwiseAnd;
				case BinaryOperatorType.BitwiseOr:
					return CodeBinaryOperatorType.BitwiseOr;
				case BinaryOperatorType.Divide:
				case BinaryOperatorType.DivideInteger:
					return CodeBinaryOperatorType.Divide;
				case BinaryOperatorType.GreaterThan:
					return CodeBinaryOperatorType.GreaterThan;
				case BinaryOperatorType.GreaterThanOrEqual:
					return CodeBinaryOperatorType.GreaterThanOrEqual;
				case BinaryOperatorType.InEquality:
					return CodeBinaryOperatorType.IdentityInequality;
				case BinaryOperatorType.LessThan:
					return CodeBinaryOperatorType.LessThan;
				case BinaryOperatorType.LessThanOrEqual:
					return CodeBinaryOperatorType.LessThanOrEqual;
				case BinaryOperatorType.LogicalAnd:
					return CodeBinaryOperatorType.BooleanAnd;
				case BinaryOperatorType.LogicalOr:
					return CodeBinaryOperatorType.BooleanOr;
				case BinaryOperatorType.Modulus:
					return CodeBinaryOperatorType.Modulus;
				case BinaryOperatorType.Multiply:
					return CodeBinaryOperatorType.Multiply;
				case BinaryOperatorType.ReferenceEquality:
					return CodeBinaryOperatorType.IdentityEquality;
				case BinaryOperatorType.Subtract:
					return CodeBinaryOperatorType.Subtract;
				default:
					return CodeBinaryOperatorType.ValueEquality;
			}
		}
		
		public object VisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data)
		{
			Console.WriteLine("VisitAddHandlerStatement");			
			return null;
		}
		
		public object VisitAddressOfExpression(AddressOfExpression addressOfExpression, object data)
		{
			Console.WriteLine("VisitAddressOfExpression");			
			return null;
		}
		
		public object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			Console.WriteLine("VisitAnonymousMethodExpression");			
			return null;
		}
		
		/// <summary>
		/// Converts an NRefactory array creation expression to 
		/// code dom array creation expression.
		/// </summary>
		public object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			CodeArrayCreateExpression codeArrayCreateExpression = new CodeArrayCreateExpression();
			codeArrayCreateExpression.CreateType.BaseType = arrayCreateExpression.CreateType.Type;

			// Add initializers.
			foreach (CodeExpression expression in ConvertExpressions(arrayCreateExpression.ArrayInitializer.CreateExpressions)) {
				codeArrayCreateExpression.Initializers.Add(expression);
			}
			return codeArrayCreateExpression;
		}
				
		/// <summary>
		/// Converts an NRefactory's assignment expression to a
		/// CodeDom's CodeAssignmentStatement.
		/// </summary>
		public object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			if (IsAddEventHandler(assignmentExpression)) {
				return CreateAttachEventStatement(assignmentExpression.Left, assignmentExpression.Right);
			} else if (IsRemoveEventHandler(assignmentExpression)) {
				return CreateRemoveEventStatement(assignmentExpression.Left, assignmentExpression.Right);
			} else if (assignmentExpression.Op == AssignmentOperatorType.Add) {
				return CreateAddAssignmentStatement(assignmentExpression);
			} else if (assignmentExpression.Op == AssignmentOperatorType.Subtract) {
				return CreateSubtractAssignmentStatement(assignmentExpression);
			}
			
			// Create a simple assignment
			CodeAssignStatement codeAssignStatement = new CodeAssignStatement();
			codeAssignStatement.Left = (CodeExpression)assignmentExpression.Left.AcceptVisitor(this, data);
			codeAssignStatement.Right = (CodeExpression)assignmentExpression.Right.AcceptVisitor(this, data);
			return codeAssignStatement;
		}
		
		public object VisitAttribute(ICSharpCode.NRefactory.Ast.Attribute attribute, object data)
		{
			return null;
		}
		
		public object VisitAttributeSection(AttributeSection attributeSection, object data)
		{
			return null;
		}

		/// <summary>
		/// Converts a base class reference to a this reference.
		/// Python has no concept of a direct base class reference so
		/// "base" is converted to "self".
		/// </summary>
		public object VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			return new CodeThisReferenceExpression();
		}
		
		/// <summary>
		/// Converts from the NRefactory's binary operator expression to
		/// a CodeDom's CodeBinaryOperatorExpression.
		/// </summary>
		public object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			CodeBinaryOperatorExpression codeBinaryOperatorExpression = new CodeBinaryOperatorExpression();
			codeBinaryOperatorExpression.Operator = ConvertBinaryOperatorType(binaryOperatorExpression.Op);
			codeBinaryOperatorExpression.Left = (CodeExpression)binaryOperatorExpression.Left.AcceptVisitor(this, data);
			codeBinaryOperatorExpression.Right = (CodeExpression)binaryOperatorExpression.Right.AcceptVisitor(this, data);
			return codeBinaryOperatorExpression;
		}
		
		/// <summary>
		/// Visits the statement's children.
		/// </summary>
		public object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			return blockStatement.AcceptChildren(this, data);
		}
		
		public object VisitBreakStatement(BreakStatement breakStatement, object data)
		{
			Console.WriteLine("VisitBreakStatement");
			return null;
		}
		
		public object VisitCaseLabel(CaseLabel caseLabel, object data)
		{
			Console.WriteLine("VisitCaseLabel");
			return null;
		}

		/// <summary>
		/// Ignore the cast and just visit the expression inside the cast.
		/// </summary>
		public object VisitCastExpression(CastExpression castExpression, object data)
		{
			return castExpression.Expression.AcceptVisitor(this, data);
		}
		
		public object VisitCatchClause(CatchClause catchClause, object data)
		{
			Console.WriteLine("VisitCatchClause");
			return null;
		}
		
		public object VisitCheckedExpression(CheckedExpression checkedExpression, object data)
		{
			Console.WriteLine("VisitCheckedExpression");
			return null;
		}
		
		public object VisitCheckedStatement(CheckedStatement checkedStatement, object data)
		{
			Console.WriteLine("VisitCheckedStatement");
			return null;
		}
		
		public object VisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data)
		{
			Console.WriteLine("VisitClassReferenceExpression");
			return null;
		}
		
		/// <summary>
		/// Converts from an NRefactory's compilation unit to a code dom's
		/// compilation unit.
		/// </summary>
		public object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			// Create  code compile unit with one root namespace
			// that has no name.	
			pythonCodeCompileUnit = new CodeCompileUnit();
			currentNamespace = new CodeNamespace();
			pythonCodeCompileUnit.Namespaces.Add(currentNamespace);
			
			// Visit the child items of the compilation unit.
			compilationUnit.AcceptChildren(this, data);
		
			return pythonCodeCompileUnit;
		}
		
		public object VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{
			Console.WriteLine("VisitConditionalExpression");
			return null;
		}
		
		/// <summary>
		/// Adds a CodeMemberMethod to the current class being visited.
		/// </summary>
		public object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			CodeConstructor codeConstructor = new CodeConstructor();
			codeConstructor.Name = constructorDeclaration.Name;
			currentClass.Members.Add(codeConstructor);
			
			// Add the constructor body.
			Console.WriteLine(constructorDeclaration.Body.ToString());
			statementsStack.Push(codeConstructor.Statements);
			constructorDeclaration.Body.AcceptVisitor(this, data);
			statementsStack.Pop();
			
			return null;
		}
		
		public object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
		{
			Console.WriteLine("VisitConstructorInitializer");
			return null;
		}
		
		public object VisitContinueStatement(ContinueStatement continueStatement, object data)
		{
			Console.WriteLine("VisitContinueStatement");
			return null;
		}
		
		public object VisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data)
		{
			Console.WriteLine("VisitDeclareDeclaration");
			return null;
		}
		
		public object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			Console.WriteLine("VisitDefaultValueExpression");
			return null;
		}
		
		public object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			Console.WriteLine("VisitDelegateDeclaration");			
			return null;
		}
		
		public object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			Console.WriteLine("VisitDestructorDeclaration");
			return null;
		}
		
		public object VisitDirectionExpression(DirectionExpression directionExpression, object data)
		{
			Console.WriteLine("VisitDirectionExpression");
			return null;
		}
		
		public object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			Console.WriteLine("VisitDoLoopStatement");
			return null;
		}

		/// <summary>
		/// Converts the else if statement to a code dom statement. The 
		/// code dom does not support else if directly so we need to
		/// add the condition to the false statements of the original if.
		/// 
		/// if test1
		/// else if test2
		/// end if
		/// 
		/// converts to:
		/// 
		/// if test1
		/// else
		/// 	if test 2
		/// 	end if
		/// end if
		/// </summary>
		public object VisitElseIfSection(ElseIfSection elseIfSection, object data)
		{
			// Convert condition.
			CodeConditionStatement conditionStatement = new CodeConditionStatement();
			conditionStatement.Condition = (CodeExpression)elseIfSection.Condition.AcceptVisitor(this, data);
			
			// Convert else if body statements.
			statementsStack.Push(conditionStatement.TrueStatements);
			elseIfSection.EmbeddedStatement.AcceptVisitor(this, data);
			statementsStack.Pop();
			
			AddStatement(conditionStatement);
			return null;
		}
		
		public object VisitEmptyStatement(EmptyStatement emptyStatement, object data)
		{
			Console.WriteLine("VisitEmptyStatement");
			return null;
		}
		
		public object VisitEndStatement(EndStatement endStatement, object data)
		{
			Console.WriteLine("VistEndStatement");
			return null;
		}
		
		public object VisitEraseStatement(EraseStatement eraseStatement, object data)
		{
			Console.WriteLine("VisitEraseStatement");
			return null;
		}
		
		public object VisitErrorStatement(ErrorStatement errorStatement, object data)
		{
			Console.WriteLine("VisitErrorStatement");
			return null;
		}
		
		public object VisitEventAddRegion(EventAddRegion eventAddRegion, object data)
		{
			Console.WriteLine("VisitEventAddRegion");
			return null;
		}
		
		public object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			Console.WriteLine("VisitEventDeclaration");
			return null;
		}
		
		public object VisitEventRaiseRegion(EventRaiseRegion eventRaiseRegion, object data)
		{
			Console.WriteLine("VisitEventRaiseRegion");
			return null;
		}
		
		public object VisitEventRemoveRegion(EventRemoveRegion eventRemoveRegion, object data)
		{
			Console.WriteLine("VisitEventRemoveRegion");
			return null;
		}
		
		public object VisitExitStatement(ExitStatement exitStatement, object data)
		{
			Console.WriteLine("VisitExitStatement");
			return null;
		}
		
		/// <summary>
		/// Visits the expression statement.
		/// </summary>
		/// <remarks>
		/// When converting the ExpressionStatement's expression we
		/// allow it to be either an Expression or a Statement.
		/// One example is NRefactory's AssignExpression which 
		/// is converted to a code dom's CodeAssignStatement. If
		/// the expression is really an expression then we wrap
		/// the expression inside a CodeExpressionStatement.
		/// </remarks>
		public object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			Console.WriteLine("VisitExpressionStatement: " + expressionStatement.ToString());
			
			// Convert the expression.
			// Sometimes an expression is actually a statement
			object convertedExpression = expressionStatement.Expression.AcceptVisitor(this, data);
			if (convertedExpression is CodeStatement) {
				CodeStatement convertedStatement = (CodeStatement)convertedExpression;
				AddStatement(convertedStatement);
				return convertedStatement;
			} 
			
			// Create a code expression statement to contain the expression.
			CodeExpressionStatement createdStatement = new CodeExpressionStatement();
			createdStatement.Expression = (CodeExpression)convertedExpression;
			AddStatement(createdStatement);
			
			return createdStatement;
		}
		
		/// <summary>
		/// Saves the field information so it can be added to the
		/// class constructor. This is done since python requires you
		/// to initialize any class instance variables in the
		/// __init__ method. For example, consider the equivalent 
		/// C# and Python code:
		/// 
		/// class Foo
		/// {
		/// 	private int i = 0;
		/// }
		/// 
		/// class Foo:
		/// 	def __init__(self):
		/// 		i = 0
		/// 
		/// The only difference is that the initialization is moved to the
		/// class constructor.
		/// </summary>
		public object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			// Save the field until the class has been processed.
			currentClassFields.Add(fieldDeclaration);						
			return null;
		}
				
		public object VisitFixedStatement(FixedStatement fixedStatement, object data)
		{
			Console.WriteLine("VisitFixedStatement");
			return null;
		}
		
		/// <summary>
		/// Converts a NRefactory's foreach statement to a CodeDom
		/// CodeIterationStatement.
		/// </summary>
		public object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			// Convert the for loop's initializers.
			CodeIterationStatement codeIterationStatement = new CodeIterationStatement();				
			statementsStack.Push(nullStatementCollection);
			codeIterationStatement.InitStatement = CreateInitStatement(foreachStatement);
			statementsStack.Pop();

			// Convert the for loop's test expression.
			codeIterationStatement.TestExpression = CreateTestExpression(foreachStatement);
			
			// Move the initializer in the foreach loop to the
			// first line of the for loop's body since the
			// code dom does not support multiple InitStatements.
			codeIterationStatement.Statements.Add(CreateIteratorVariableDeclaration(foreachStatement));
			
			// Visit the for loop's body.
			statementsStack.Push(codeIterationStatement.Statements);
			foreachStatement.EmbeddedStatement.AcceptVisitor(this, data);
			statementsStack.Pop();
		
			AddStatement(codeIterationStatement);
			return null;
		}
		
		public object VisitForNextStatement(ForNextStatement forNextStatement, object data)
		{
			Console.WriteLine("VisitForNextStatement");
			return null;
		}
		
		/// <summary>
		/// Converts from an NRefactory for loop to a CodeDOM iteration
		/// statement.
		/// </summary>
		public object VisitForStatement(ForStatement forStatement, object data)
		{
			CodeIterationStatement codeIterationStatement = new CodeIterationStatement();
			
			// Convert the for loop's initializers.
			statementsStack.Push(nullStatementCollection);
			foreach (Statement statement in forStatement.Initializers) {
				codeIterationStatement.InitStatement = (CodeStatement)statement.AcceptVisitor(this, data);
			}
			statementsStack.Pop();

			// Convert the for loop's test expression.
			codeIterationStatement.TestExpression = (CodeExpression)forStatement.Condition.AcceptVisitor(this, data);
			
			// Convert the for loop's increment statement.
			statementsStack.Push(nullStatementCollection);
			foreach (Statement statement in forStatement.Iterator) {
				codeIterationStatement.IncrementStatement = (CodeStatement)statement.AcceptVisitor(this, data);
			}
			statementsStack.Pop();
			
			// Visit the for loop's body.
			statementsStack.Push(codeIterationStatement.Statements);
			forStatement.EmbeddedStatement.AcceptVisitor(this, data);
			statementsStack.Pop();
			
			AddStatement(codeIterationStatement);
			return null;
		}
		
		public object VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data)
		{
			Console.WriteLine("VisitGotoCaseStatement");
			return null;
		}
		
		public object VisitGotoStatement(GotoStatement gotoStatement, object data)
		{
			Console.WriteLine("VisitGotoStatement");
			return null;
		}
		
		/// <summary>
		/// Converts from an NRefactory IdentifierExpression to a 
		/// CodeFieldReferenceExpression if the identifier refers to
		/// a field. If it refers to a local variable it returns a
		/// CodeVariableReferenceExpression.
		/// </summary>
		public object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			if (IsField(identifierExpression.Identifier)) {	
				CodeFieldReferenceExpression fieldRef = new CodeFieldReferenceExpression();
				fieldRef.FieldName = "_" + identifierExpression.Identifier;
				fieldRef.TargetObject = new CodeThisReferenceExpression();
				return fieldRef;
			} else {
				CodeVariableReferenceExpression variableRef = new CodeVariableReferenceExpression();
				variableRef.VariableName = identifierExpression.Identifier;
				return variableRef;
			}
		}
		
		/// <summary>
		/// Converts an NRefactory's if-else statement to a 
		/// CodeDOM's CodeConditionStatement.
		/// </summary>
		public object VisitIfElseStatement(IfElseStatement ifElseStatement, object data)
		{
			// Convert condition.
			CodeConditionStatement conditionStatement = new CodeConditionStatement();
			conditionStatement.Condition = (CodeExpression)ifElseStatement.Condition.AcceptVisitor(this, data);
			
			// Convert true statements.
			statementsStack.Push(conditionStatement.TrueStatements);
			foreach (Statement statement in ifElseStatement.TrueStatement) {
				statement.AcceptVisitor(this, data);
			}
			statementsStack.Pop();
			
			// Convert false statements.
			statementsStack.Push(conditionStatement.FalseStatements);
			foreach (Statement statement in ifElseStatement.FalseStatement) {
				statement.AcceptVisitor(this, data);
			}
			statementsStack.Pop();

			// Convert else if sections.
			statementsStack.Push(conditionStatement.FalseStatements);
			foreach (ElseIfSection elseIfSection in ifElseStatement.ElseIfSections) {
				elseIfSection.AcceptVisitor(this, data);
			}
			statementsStack.Pop();

			// Add condition statement to current method.
			AddStatement(conditionStatement);
			return null;
		}
		
		public object VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data)
		{
			Console.WriteLine("VisitIndexerDeclaration");
			return null;
		}
		
		/// <summary>
		/// Converts from an NRefactory array indexer expression to a
		/// code dom CodeArrayIndexerExpression.
		/// </summary>
		public object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{				
			CodeArrayIndexerExpression codeArrayIndexerExpression = new CodeArrayIndexerExpression();
			codeArrayIndexerExpression.TargetObject = (CodeExpression)indexerExpression.TargetObject.AcceptVisitor(this, data);

			// Add indices.
			foreach (CodeExpression expression in ConvertExpressions(indexerExpression.Indexes)) {
				codeArrayIndexerExpression.Indices.Add(expression);
			}

			return codeArrayIndexerExpression;
		}
		
		public object VisitInnerClassTypeReference(InnerClassTypeReference innerClassTypeReference, object data)
		{
			Console.WriteLine("VisitInnerClassTypeReference");
			return null;
		}
		
		public object VisitInterfaceImplementation(InterfaceImplementation interfaceImplementation, object data)
		{
			Console.WriteLine("VisitInterfaceImplementation");
			return null;
		}

		/// <summary>
		/// Converts an NRefactory Invocation Expression to a code dom's
		/// CodeMethodInvokeStatement.
		/// </summary>
		public object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{					
			CodeMethodReferenceExpression methodRef = new CodeMethodReferenceExpression();

			MemberReferenceExpression memberRefExpression = invocationExpression.TargetObject as MemberReferenceExpression;
			IdentifierExpression identifierExpression = invocationExpression.TargetObject as IdentifierExpression;
			if (memberRefExpression != null) {
				methodRef.MethodName = memberRefExpression.MemberName;
				methodRef.TargetObject = (CodeExpression)memberRefExpression.TargetObject.AcceptVisitor(this, data);
				
			} else if (identifierExpression != null) {
				methodRef.MethodName = identifierExpression.Identifier;
				methodRef.TargetObject = new CodeThisReferenceExpression();
			}
				
			// Create method invoke.
			CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression();
			methodInvoke.Parameters.AddRange(ConvertExpressions(invocationExpression.Arguments));
			methodInvoke.Method = methodRef;
			return methodInvoke;
		}
		
		public object VisitLabelStatement(LabelStatement labelStatement, object data)
		{
			Console.WriteLine("VisitLabelStatement");
			return null;
		}
		
		/// <summary>
		/// Adds the local variable declaration to the current method if
		/// this one exists.
		/// </summary>
		public object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			// Create variable declaration.
			VariableDeclaration variableDeclaration = localVariableDeclaration.Variables[0];
			string variableType = localVariableDeclaration.TypeReference.SystemType;	
			string variableName = variableDeclaration.Name;
			CodeVariableDeclarationStatement codeVariableDeclarationStatement = new CodeVariableDeclarationStatement(variableType, variableName);
			
			// Convert the variable initializer from an NRefactory AST 
			// expression to a code dom expression.
			codeVariableDeclarationStatement.InitExpression = (CodeExpression)variableDeclaration.Initializer.AcceptVisitor(this, data);
			
			// Add variable to current block.
			AddStatement(codeVariableDeclarationStatement);
			return codeVariableDeclarationStatement;
		}
		
		public object VisitLockStatement(LockStatement lockStatement, object data)
		{
			Console.WriteLine("VisitLockStatement");
			return null;
		}
		
		/// <summary>
		/// Adds a CodeMemberMethod to the current class being visited.
		/// </summary>
		public object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Name = methodDeclaration.Name;
			codeMemberMethod.Attributes = ConvertMethodModifier(methodDeclaration.Modifier);
			currentClass.Members.Add(codeMemberMethod);
			
			// Set user data so accepts and returns statements
			// are not added before method.
			codeMemberMethod.UserData["HasAccepts"] = false;
			codeMemberMethod.UserData["HasReturns"] = false;
			
			// Add the parameters.
			foreach (ParameterDeclarationExpression parameter in methodDeclaration.Parameters) {
				codeMemberMethod.Parameters.Add(ConvertParameter(parameter));
			}
			
			// Add the method body.
			statementsStack.Push(codeMemberMethod.Statements);
			methodDeclaration.Body.AcceptVisitor(this, data);
			statementsStack.Pop();
				
			return null;
		}
		
		public object VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data)
		{
			Console.WriteLine("VisitNamedArgumentExpression");
			return null;
		}
		
		/// <summary>
		/// Visits the namespace declaration and all child nodes.
		/// </summary>
		public object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			return namespaceDeclaration.AcceptChildren(this, data);
		}
		
		/// <summary>
		/// Converts an NRefactory's ObjectCreateExpression to a code dom's
		/// CodeObjectCreateExpression.
		/// </summary>
		public object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			CodeObjectCreateExpression codeObjectCreateExpression = new CodeObjectCreateExpression();
			codeObjectCreateExpression.CreateType.BaseType = objectCreateExpression.CreateType.Type;
			return codeObjectCreateExpression;
		}
		
		public object VisitOnErrorStatement(OnErrorStatement onErrorStatement, object data)
		{
			return null;
		}
		
		public object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			Console.WriteLine("VisitOperatorDeclaration");
			return null;
		}
		
		public object VisitOptionDeclaration(OptionDeclaration optionDeclaration, object data)
		{
			Console.WriteLine("VisitOptionDeclaration");
			return null;
		}
		
		public object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			Console.WriteLine("VisitParameterDeclarationExpression");
			return null;
		}
		
		public object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			Console.WriteLine("VisitParenthesizedExpression");
			return null;
		}
		
		public object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			Console.WriteLine("VisitPointerReferenceExpression");
			return null;
		}
		
		/// <summary>
		/// Converts a NRefactory primitive expression to a CodeDom's
		/// CodePrimitiveExpression.
		/// </summary>
		public object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			return new CodePrimitiveExpression(primitiveExpression.Value);
		}
		
		/// <summary>
		/// Converts an NRefactory property declaration to a code dom
		/// property.
		/// </summary>
		public object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			CodeMemberProperty property = new CodeMemberProperty();
			property.Name = propertyDeclaration.Name;
			property.Attributes = MemberAttributes.Public;
			
			// Set user data so accepts and returns statements
			// are not added before get and set methods.
			property.UserData["HasAccepts"] = false;
			property.UserData["HasReturns"] = false;
		
			// Add get statements.
			statementsStack.Push(property.GetStatements);
			propertyDeclaration.GetRegion.Block.AcceptVisitor(this, data);
			statementsStack.Pop();
			
			// Add set statements.
			statementsStack.Push(property.SetStatements);
			propertyDeclaration.SetRegion.Block.AcceptVisitor(this, data);
			statementsStack.Pop();
			
			// Add the property to the current class.
			currentClass.Members.Add(property);
			return null;
		}
		
		public object VisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data)
		{
			Console.WriteLine("VisitPropertyGetRegion");
			return null;
		}
		
		public object VisitPropertySetRegion(PropertySetRegion propertySetRegion, object data)
		{
			Console.WriteLine("VisitPropertySetRegion");
			return null;
		}
		
		public object VisitRaiseEventStatement(RaiseEventStatement raiseEventStatement, object data)
		{
			Console.WriteLine("VisitRaiseEventStatement");
			return null;
		}
		
		public object VisitReDimStatement(ReDimStatement reDimStatement, object data)
		{
			Console.WriteLine("VisitReDimStatement");
			return null;
		}
		
		/// <summary>
		/// Converts a NRefactory remove handler statement 
		/// (e.g. button.Click -= ButtonClick) to a
		/// code dom statement.
		/// </summary>
		public object VisitRemoveHandlerStatement(RemoveHandlerStatement removeHandlerStatement, object data)
		{
			Console.WriteLine("VisitRemoveHandlerStatement");
			return null;
		}
		
		public object VisitResumeStatement(ResumeStatement resumeStatement, object data)
		{
			Console.WriteLine("VisitResumeStatement");
			return null;
		}
		
		/// <summary>
		/// Converts a NRefactory ReturnStatement to a code dom's
		/// CodeMethodReturnStatement.
		/// </summary>
		public object VisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			CodeMethodReturnStatement codeMethodReturnStatement = new CodeMethodReturnStatement();
			codeMethodReturnStatement.Expression = (CodeExpression)returnStatement.Expression.AcceptVisitor(this, data);
			AddStatement(codeMethodReturnStatement);
			return null;
		}
		
		public object VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
		{
			Console.WriteLine("VisitSizeOfExpression");
			return null;
		}
		
		public object VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
		{
			return null;
		}
		
		public object VisitStopStatement(StopStatement stopStatement, object data)
		{
			return null;
		}
		
		public object VisitSwitchSection(SwitchSection switchSection, object data)
		{
			return null;
		}
		
		public object VisitSwitchStatement(SwitchStatement switchStatement, object data)
		{
			return null;
		}
		
		public object VisitTemplateDefinition(TemplateDefinition templateDefinition, object data)
		{
			return null;
		}
		
		public object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			Console.WriteLine("VisitThisReferenceExpression");
			return null;
		}

		/// <summary>
		/// Converts an NRefactory throw statement to a code dom's throw exception statement.
		/// </summary>
		public object VisitThrowStatement(ThrowStatement throwStatement, object data)
		{
			CodeThrowExceptionStatement codeThrowExceptionStatement = new CodeThrowExceptionStatement();
			codeThrowExceptionStatement.ToThrow = (CodeExpression)throwStatement.Expression.AcceptVisitor(this, data);
			AddStatement(codeThrowExceptionStatement);
			return null;
		}
		
		/// <summary>
		/// Converts an NRefactory try-catch statement to a code dom
		/// try-catch statement.
		/// </summary>
		public object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{			
			// Convert try-catch body.
			CodeTryCatchFinallyStatement codeTryCatchStatement = new CodeTryCatchFinallyStatement();
			statementsStack.Push(codeTryCatchStatement.TryStatements);
			tryCatchStatement.StatementBlock.AcceptVisitor(this, data);
			statementsStack.Pop();
			
			// Convert catches.
			foreach (CatchClause catchClause in tryCatchStatement.CatchClauses) {
				CodeCatchClause codeCatchClause = new CodeCatchClause();
				codeCatchClause.CatchExceptionType.BaseType = catchClause.TypeReference.Type;
				codeCatchClause.LocalName = catchClause.VariableName;
				
				// Convert catch child statements.
				statementsStack.Push(codeCatchClause.Statements);
				catchClause.StatementBlock.AcceptVisitor(this, data);
				statementsStack.Pop();
				
				codeTryCatchStatement.CatchClauses.Add(codeCatchClause);
			}
			
			// Convert finally block.
			statementsStack.Push(codeTryCatchStatement.FinallyStatements);
			tryCatchStatement.FinallyBlock.AcceptVisitor(this, data);
			statementsStack.Pop();
			
			AddStatement(codeTryCatchStatement);
			return null;
		}
		
		/// <summary>
		/// Visits a class.
		/// </summary>
		public object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			// Create a new type.
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(typeDeclaration.Name);
			currentClass = codeTypeDeclaration;

			// Ensure __slots__ is not added to generated code.
			codeTypeDeclaration.UserData["HasSlots"] = false;

			// Add the type to the current namespace.
			currentNamespace.Types.Add(codeTypeDeclaration);

			// Visit the rest of the class.
			object o = typeDeclaration.AcceptChildren(this, data);
			
			// Add any class fields that were found to the constructor.
			if (currentClassFields.Count > 0) {
				AddFieldsToConstructor();
				currentClassFields.Clear();
			}
			
			return o;
		}
		
		public object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			return null;
		}
		
		public object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
		{
			Console.WriteLine("VisitTypeOfIsExpression");
			return null;
		}
		
		public object VisitTypeReference(TypeReference typeReference, object data)
		{
			Console.WriteLine("VisitTypeReference");
			return null;
		}
		
		public object VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data)
		{
			Console.WriteLine("VisitTypeReferenceExpression");
			return null;
		}
		
		/// <summary>
		/// Converts a unary operator expression to a code dom expression.
		/// </summary>
		public object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			switch (unaryOperatorExpression.Op) {
				case UnaryOperatorType.PostIncrement:
				case UnaryOperatorType.Increment:
					// Change i++ or ++i to i = i + 1
					return CreateIncrementStatement(unaryOperatorExpression);
				case UnaryOperatorType.Decrement:
				case UnaryOperatorType.PostDecrement:
					// Change --i or i-- to i = i - 1.
					return CreateDecrementStatement(unaryOperatorExpression);
			}
			return null;
		}
		
		public object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			return null;
		}
		
		public object VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
		{
			return null;
		}
		
		public object VisitUnsafeStatement(UnsafeStatement unsafeStatement, object data)
		{
			return null;
		}
		
		public object VisitUsing(Using @using, object data)
		{
			return null;
		}
		
		/// <summary>
		/// Converts using declarations into Python import statements.
		/// </summary>
		public object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			foreach (Using @using in usingDeclaration.Usings) {		
				// Add import statements for each using.
				CodeNamespaceImport import = new CodeNamespaceImport(@using.Name);
				currentNamespace.Imports.Add(import);				
			}
			
			return usingDeclaration.AcceptChildren(this, data);
		}
		
		public object VisitUsingStatement(UsingStatement usingStatement, object data)
		{
			return null;
		}
		
		public object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			Console.WriteLine("VisitVariableDeclaration");
			return null;
		}
		
		public object VisitWithStatement(WithStatement withStatement, object data)
		{
			return null;
		}
		
		public object VisitYieldStatement(YieldStatement yieldStatement, object data)
		{
			return null;
		}
		
		public object VisitCollectionInitializerExpression(CollectionInitializerExpression collectionInitializerExpression, object data)
		{
			return null;
		}
		
		public object VisitLambdaExpression(LambdaExpression lambdaExpression, object data)
		{
			return null;
		}

		/// <summary>
		/// Converts an NRefactory MemberReferenceExpression to a
		/// code dom's CodeFieldReferenceExpression.
		/// </summary>
		public object VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			CodeFieldReferenceExpression codeFieldReferenceExpression = new CodeFieldReferenceExpression();
			codeFieldReferenceExpression.FieldName = memberReferenceExpression.MemberName;
			codeFieldReferenceExpression.TargetObject = (CodeExpression)memberReferenceExpression.TargetObject.AcceptVisitor(this, data);
			return codeFieldReferenceExpression;
		}
		
		public object VisitQueryExpression(QueryExpression queryExpression, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionFromClause(QueryExpressionFromClause queryExpressionFromClause, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionGroupClause(QueryExpressionGroupClause queryExpressionGroupClause, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionIntoClause(QueryExpressionIntoClause queryExpressionIntoClause, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionJoinClause(QueryExpressionJoinClause queryExpressionJoinClause, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionLetClause(QueryExpressionLetClause queryExpressionLetClause, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionOrderClause(QueryExpressionOrderClause queryExpressionOrderClause, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionOrdering(QueryExpressionOrdering queryExpressionOrdering, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionSelectClause(QueryExpressionSelectClause queryExpressionSelectClause, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionWhereClause(QueryExpressionWhereClause queryExpressionWhereClause, object data)
		{
			return null;
		}
		
		public object VisitExpressionRangeVariable(ExpressionRangeVariable expressionRangeVariable, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionAggregateClause(QueryExpressionAggregateClause queryExpressionAggregateClause, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionDistinctClause(QueryExpressionDistinctClause queryExpressionDistinctClause, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionGroupJoinVBClause(QueryExpressionGroupJoinVBClause queryExpressionGroupJoinVBClause, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionGroupVBClause(QueryExpressionGroupVBClause queryExpressionGroupVBClause, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionJoinConditionVB(QueryExpressionJoinConditionVB queryExpressionJoinConditionVB, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionJoinVBClause(QueryExpressionJoinVBClause queryExpressionJoinVBClause, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionLetVBClause(QueryExpressionLetVBClause queryExpressionLetVBClause, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionPartitionVBClause(QueryExpressionPartitionVBClause queryExpressionPartitionVBClause, object data)
		{
			return null;
		}
		
		public object VisitQueryExpressionSelectVBClause(QueryExpressionSelectVBClause queryExpressionSelectVBClause, object data)
		{
			return null;
		}
		
		/// <summary>
		/// Converts from the NRefactory method modifier to code DOM 
		/// member attributes.
		/// </summary>
		MemberAttributes ConvertMethodModifier(Modifiers modifiers)
		{
			MemberAttributes attributes = MemberAttributes.Public;
			
//			if (modifiers & Modifiers.Public == Modifiers.Public) {
//				atributes
//			}
			
			return attributes;
		}
		
		/// <summary>
		/// Adds fields to the constructor.
		/// </summary>
		/// <remarks>
		/// The fields should be inserted in the same order as they are
		/// in the currentClassFields collection and they should be
		/// inserted before any other code in the constructor.
		/// </remarks>
		void AddFieldsToConstructor()
		{
			CodeConstructor constructor = GetOrCreateConstructor(currentClass);
			
			// Add fields to the constructor only if they have
			// initializers.
			for (int i = 0; i < currentClassFields.Count; ++i) {
				FieldDeclaration fieldDeclaration = currentClassFields[i];
				if (FieldHasInitialValue(fieldDeclaration)) {
					CodeAssignStatement assignStatement = CreateAssignStatement(fieldDeclaration);
					constructor.Statements.Insert(i, assignStatement);
				}
			}
		}
		
		/// <summary>
		/// Checks that the field declaration has an initializer that
		/// sets an initial value.
		/// </summary>
		static bool FieldHasInitialValue(FieldDeclaration fieldDeclaration)
		{
			VariableDeclaration variableDeclaration = fieldDeclaration.Fields[0];
			Expression initializer = variableDeclaration.Initializer;
			return !initializer.IsNull;
		}
		
		/// <summary>
		/// Converts a field declaration into an assign statement. This
		/// assign statement should then be added to the class's constructor.
		/// This method is used when adding field initializer statements
		/// to the constructor.
		/// </summary>
		CodeAssignStatement CreateAssignStatement(FieldDeclaration fieldDeclaration)
		{					
			// Create the lhs of the assign statement.
			CodeAssignStatement assignStatement = new CodeAssignStatement();
			CodeThisReferenceExpression thisReferenceExpression = new CodeThisReferenceExpression();
			VariableDeclaration variableDeclaration = fieldDeclaration.Fields[0];			
			string fieldName = variableDeclaration.Name;
			CodeFieldReferenceExpression fieldReferenceExpression = new CodeFieldReferenceExpression(thisReferenceExpression, "_" + fieldName);
			assignStatement.Left = fieldReferenceExpression;
			
			// Create the rhs of the assign statement.
			assignStatement.Right = (CodeExpression)variableDeclaration.Initializer.AcceptVisitor(this, null);
			
			return assignStatement;
		}
		
		/// <summary>
		/// Gets the constructor for the class or creates a new one if
		/// it does not exist.
		/// </summary>
		static CodeConstructor GetOrCreateConstructor(CodeTypeDeclaration typeDeclaration)
		{
			// Find an existing constructor.
			CodeConstructor constructor = GetConstructor(typeDeclaration);
			if (constructor == null) {
				// Create a new constructor.
				constructor = new CodeConstructor();
				typeDeclaration.Members.Add(constructor);
			}
			return constructor;
		}

		/// <summary>
		/// Gets the constructor for the class. Returns null if none is
		/// found.
		/// </summary>
		static CodeConstructor GetConstructor(CodeTypeDeclaration typeDeclaration)
		{
			foreach (CodeTypeMember member in typeDeclaration.Members) {
				CodeConstructor constructor = member as CodeConstructor;
				if (constructor != null) {
					return constructor;
				}
			}
			return null;
		}
				
		/// <summary>
		/// Adds the statement to the currently active statement collection.
		/// </summary>
		void AddStatement(CodeStatement statement)
		{
			CodeStatementCollection currentStatementCollection = statementsStack.Peek();
			currentStatementCollection.Add(statement);
		}
		
		/// <summary>
		/// Converts an NRefactory parameter to a CodeDom parameter declaration.
		/// </summary>
		CodeParameterDeclarationExpression ConvertParameter(ParameterDeclarationExpression expression)
		{
			CodeParameterDeclarationExpression parameter = new CodeParameterDeclarationExpression();
			parameter.Name = expression.ParameterName;
			return parameter;
		}

		/// <summary>
		/// Converts a post or pre increment expression to an assign statement.
		/// This converts "i++" and "++i" to "i = i + 1" since the code dom 
		/// does not support post increment expressions.
		/// </summary>
		CodeAssignStatement CreateIncrementStatement(UnaryOperatorExpression unaryOperatorExpression)
		{
			return CreateIncrementStatement(unaryOperatorExpression, 1, CodeBinaryOperatorType.Add);
		}

		/// <summary>
		/// Converts a post or pre decrement expression to an assign statement.
		/// This converts "i--" and "--i" to "i = i - 1" since the code dom 
		/// does not support post increment expressions.
		/// </summary>
		CodeAssignStatement CreateDecrementStatement(UnaryOperatorExpression unaryOperatorExpression)
		{
			return CreateIncrementStatement(unaryOperatorExpression, 1, CodeBinaryOperatorType.Subtract);
		}
		
		/// <summary>
		/// Converts a post or pre increment expression to an assign statement.
		/// This converts "i++" and "++i" to "i = i + 1" since the code dom 
		/// does not support post increment expressions.
		/// </summary>
		CodeAssignStatement CreateIncrementStatement(UnaryOperatorExpression unaryOperatorExpression, int increment, CodeBinaryOperatorType binaryOperatorType)
		{
			// Lhs: i
			CodeExpression expression = (CodeExpression)unaryOperatorExpression.Expression.AcceptVisitor(this, null);
			CodeAssignStatement assignStatement = new CodeAssignStatement();
			assignStatement.Left = expression;

			// Rhs: i + increment
			CodeBinaryOperatorExpression binaryExpression = new CodeBinaryOperatorExpression();
			binaryExpression.Operator = binaryOperatorType;
			binaryExpression.Left = expression;
			binaryExpression.Right = new CodePrimitiveExpression(increment);
			assignStatement.Right = binaryExpression;

			return assignStatement;
		}
				
		/// <summary>
		/// Creates the code dom's expression for test expression for the
		/// NRefactory foreach statement. The test expression will be
		/// "enumerator.MoveNext()" which simulates what happens the test
		/// condition in a foreach loop.
		/// </summary>
		CodeExpression CreateTestExpression(ForeachStatement foreachStatement)
		{
			CodeVariableReferenceExpression variableRef = new CodeVariableReferenceExpression();
			variableRef.VariableName = "enumerator";
			
			CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression();
			methodInvoke.Method.MethodName = "MoveNext";
			methodInvoke.Method.TargetObject = variableRef;
			return methodInvoke;
		}
		
		/// <summary>
		/// Creates the statement used to initialize the for loop. The
		/// initialize statement will be "enumerator = variableName.GetEnumerator()"
		/// which simulates what happens in a foreach loop.
		/// </summary>
		CodeStatement CreateInitStatement(ForeachStatement foreachStatement)
		{
			CodeVariableReferenceExpression variableRef = new CodeVariableReferenceExpression();
			variableRef.VariableName = GetForeachVariableName(foreachStatement);
			
			CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression();
			methodInvoke.Method.MethodName = "GetEnumerator";
			methodInvoke.Method.TargetObject = variableRef;

			CodeVariableDeclarationStatement variableDeclarationStatement = new CodeVariableDeclarationStatement();	
			variableDeclarationStatement.InitExpression = methodInvoke;
			variableDeclarationStatement.Name = "enumerator";
			
			return variableDeclarationStatement;
		}
		
		/// <summary>
		/// Gets the name of the variable that is used in the
		/// foreach loop as the item being iterated.
		/// </summary>
		string GetForeachVariableName(ForeachStatement foreachStatement)
		{
			IdentifierExpression identifierExpression = foreachStatement.Expression as IdentifierExpression;
			return identifierExpression.Identifier;
		}
	
		/// <summary>
		/// Creates a code variable declaration statement for the iterator
		/// used in the foreach loop. This statement should be like:
		/// 
		/// "i = enumerator.Current"
		/// </summary>
		CodeVariableDeclarationStatement CreateIteratorVariableDeclaration(ForeachStatement foreachStatement)
		{
			CodePropertyReferenceExpression propertyRef = new CodePropertyReferenceExpression();
			propertyRef.PropertyName = "Current";
			propertyRef.TargetObject = new CodeVariableReferenceExpression("enumerator");
			
			CodeVariableDeclarationStatement variableDeclaration = new CodeVariableDeclarationStatement();
			variableDeclaration.Name = foreachStatement.VariableName;
			variableDeclaration.InitExpression = propertyRef;
			
			return variableDeclaration;
		}
				
		/// <summary>
		/// Determines whether the identifier refers to a field in the
		/// current class.
		/// </summary>
		bool IsField(string name)
		{
			foreach (FieldDeclaration field in currentClassFields) {
				if (field.Fields[0].Name == name) {
					return true;
				}
			}
			return false;
		}		
		
		/// <summary>
		/// Converts a collection of NRefactory expressions to code dom expressions.
		/// </summary>
		CodeExpression[] ConvertExpressions(List<Expression> expressions)
		{
			List<CodeExpression> codeExpressions = new List<CodeExpression>();
			foreach (Expression expression in expressions) {
				CodeExpression convertedCodeExpression = (CodeExpression)expression.AcceptVisitor(this, null);
				if (convertedCodeExpression != null) {
					codeExpressions.Add(convertedCodeExpression);
				}
			}
			return codeExpressions.ToArray();
		}
		
		/// <summary>
		/// Creates a CodeAttachEventStatement 
		/// (i.e. button.Click += ButtonClick)
		/// </summary>
		CodeAttachEventStatement CreateAttachEventStatement(Expression eventExpression, Expression eventHandlerExpression)
		{			
			// Create statement.
			CodeAttachEventStatement attachEventStatement = new CodeAttachEventStatement();
			attachEventStatement.Event = CreateEventReferenceExpression(eventExpression);
			attachEventStatement.Listener = CreateDelegateCreateExpression(eventHandlerExpression);
			
			return attachEventStatement;
		}
		
		/// <summary>
		/// Creates a CodeRemoveEventStatement 
		/// (i.e. button.Click -= ButtonClick)
		/// </summary>
		CodeRemoveEventStatement CreateRemoveEventStatement(Expression eventExpression, Expression eventHandlerExpression)
		{					
			// Create statement.
			CodeRemoveEventStatement removeEventStatement = new CodeRemoveEventStatement();
			removeEventStatement.Event = CreateEventReferenceExpression(eventExpression);
			removeEventStatement.Listener = CreateDelegateCreateExpression(eventHandlerExpression);
			return removeEventStatement;
		}
		
		/// <summary>
		/// Converts an expression to a CodeEventReferenceExpression
		/// (i.e. the "button.Click" part of "button.Click += ButtonClick".
		/// </summary>
		CodeEventReferenceExpression CreateEventReferenceExpression(Expression eventExpression)
		{
			// Create event reference.
			MemberReferenceExpression memberRef = eventExpression as MemberReferenceExpression;
			CodeEventReferenceExpression eventRef = new CodeEventReferenceExpression();
			eventRef.TargetObject = (CodeExpression)memberRef.AcceptVisitor(this, null);
			return eventRef;
		}
		
		/// <summary>
		/// Converts an event handler expression to a CodeDelegateCreateExpression.
		/// (i.e. the "ButtonClick" part of "button.Click += ButtonClick")
		/// </summary>
		CodeDelegateCreateExpression CreateDelegateCreateExpression(Expression eventHandlerExpression)
		{
			// Create event handler expression.
			IdentifierExpression identifierExpression = eventHandlerExpression as IdentifierExpression;
			CodeDelegateCreateExpression listenerExpression = new CodeDelegateCreateExpression();
			listenerExpression.MethodName = identifierExpression.Identifier;			
			return listenerExpression;
		}
		
		/// <summary>
		/// Determines whether the assignment expression is actually an
		/// event handler attach statement.
		/// </summary>
		static bool IsAddEventHandler(AssignmentExpression assignmentExpression)
		{
			return (assignmentExpression.Op == AssignmentOperatorType.Add) &&
				(assignmentExpression.Left is MemberReferenceExpression);
		}

		/// <summary>
		/// Determines whether the assignment expression is actually an
		/// event handler remove statement.
		/// </summary>
		static bool IsRemoveEventHandler(AssignmentExpression assignmentExpression)
		{
			return (assignmentExpression.Op == AssignmentOperatorType.Subtract) &&
				(assignmentExpression.Left is MemberReferenceExpression);
		}
		
		/// <summary>
		/// Creates an assignment statement of the form:
		/// i = i + 10.
		/// </summary>
		CodeAssignStatement CreateAddAssignmentStatement(AssignmentExpression assignmentExpression)
		{
			return CreateAssignmentStatementWithBinaryOperatorExpression(assignmentExpression, CodeBinaryOperatorType.Add);
		}
	
		/// <summary>
		/// Creates an assign statement with the right hand side of the 
		/// assignment using a binary operator.
		/// </summary>
		CodeAssignStatement CreateAssignmentStatementWithBinaryOperatorExpression(AssignmentExpression assignmentExpression, CodeBinaryOperatorType binaryOperatorType)
		{
			// Create the left hand side of the assignment.
			CodeAssignStatement codeAssignStatement = new CodeAssignStatement();
			codeAssignStatement.Left = (CodeExpression)assignmentExpression.Left.AcceptVisitor(this, null);

			// Create the right hand side of the assignment.
			CodeBinaryOperatorExpression binaryExpression = new CodeBinaryOperatorExpression();
			binaryExpression.Operator = binaryOperatorType;
			binaryExpression.Left = codeAssignStatement.Left;
			binaryExpression.Right = (CodeExpression)assignmentExpression.Right.AcceptVisitor(this, null);
			codeAssignStatement.Right = binaryExpression;

			return codeAssignStatement;
		}
		
		/// <summary>
		/// Creates an assignment statement of the form:
		/// i = i - 10.
		/// </summary>
		CodeAssignStatement CreateSubtractAssignmentStatement(AssignmentExpression assignmentExpression)
		{
			return CreateAssignmentStatementWithBinaryOperatorExpression(assignmentExpression, CodeBinaryOperatorType.Subtract);
		}		
	}
}
