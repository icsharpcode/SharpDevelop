// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Used to convert VB.NET and C# to Python.
	/// </summary>
	public class NRefactoryToPythonConverter : IAstVisitor
	{
		int indent;
		string indentString = "\t";
		StringBuilder codeBuilder;
		
		// Holds the constructor for the class being converted. This is used to identify class fields.
		PythonConstructorInfo constructorInfo;
		
		// Holds the parameters of the current method. This is used to identify
		// references to fields or parameters.
		List<ParameterDeclarationExpression> methodParameters = new List<ParameterDeclarationExpression>();
		SupportedLanguage language;
		List<MethodDeclaration> entryPointMethods;
		
		public NRefactoryToPythonConverter(SupportedLanguage language)
		{
			this.language = language;
		}
		
		public NRefactoryToPythonConverter()
		{
		}
		
		/// <summary>
		/// Gets or sets the source language that will be converted to python.
		/// </summary>
		public SupportedLanguage SupportedLanguage {
			get { return language; }
		}
		
		/// <summary>
		/// Creates either C# to Python or VB.NET to Python converter based on the filename extension that is to be converted.
		/// </summary>
		/// <returns>Null if the file cannot be converted.</returns>
		public static NRefactoryToPythonConverter Create(string fileName)
		{
			if (CanConvert(fileName)) {
				return new NRefactoryToPythonConverter(GetSupportedLanguage(fileName));
			}
			return null;
		}
		
		/// <summary>
		/// Only C# (.cs) or VB.NET (.vb) files can be converted.
		/// </summary>
		public static bool CanConvert(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (!String.IsNullOrEmpty(extension)) {
				extension = extension.ToLowerInvariant();
				return (extension == ".cs") || (extension == ".vb");
			}
			return false;
		}
				
		/// <summary>
		/// Gets the indentation string to use in the text editor based on the text editor properties.
		/// </summary>
		public static string GetIndentString(ITextEditorProperties textEditorProperties)
		{
			if (textEditorProperties.ConvertTabsToSpaces) {
				return new String(' ', textEditorProperties.IndentationSize);
			}
			return "\t";
		}
		
		/// <summary>
		/// Gets or sets the string that will be used to indent the generated Python code.
		/// </summary>
		public string IndentString {
			get { return indentString; }
			set { indentString = value; }
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
		/// Converts the source code to Python.
		/// </summary>
		public string Convert(string source)
		{
			return Convert(source, language);
		}
		
		/// <summary>
		/// Converts the source code to Python.
		/// </summary>
		public string Convert(string source, SupportedLanguage language)
		{
			// Convert to NRefactory code DOM.
			CompilationUnit unit = GenerateCompilationUnit(source, language);
			
			// Convert to Python code.3
			entryPointMethods = new List<MethodDeclaration>();
			codeBuilder = new StringBuilder();
			unit.AcceptVisitor(this, null);
			
			return codeBuilder.ToString().TrimEnd();
		}
		
		/// <summary>
		/// Gets a list of possible entry point methods found when converting the
		/// python source code.
		/// </summary>
		public ReadOnlyCollection<MethodDeclaration> EntryPointMethods {
			get { return entryPointMethods.AsReadOnly(); }
		}

		/// <summary>
		/// Converts from the NRefactory's binary operator type to a string.
		/// </summary>
		public static string GetBinaryOperator(BinaryOperatorType binaryOperatorType)
		{
			switch (binaryOperatorType) {
				case BinaryOperatorType.Add:
					return "+";
				case BinaryOperatorType.BitwiseAnd:
					return "&";
				case BinaryOperatorType.BitwiseOr:
					return "|";
				case BinaryOperatorType.Divide:
				case BinaryOperatorType.DivideInteger:
					return "/";
				case BinaryOperatorType.ShiftLeft:
					return "<<";
				case BinaryOperatorType.ShiftRight:
					return ">>";
				case BinaryOperatorType.GreaterThan:
					return ">";
				case BinaryOperatorType.GreaterThanOrEqual:
					return ">=";
				case BinaryOperatorType.InEquality:
					return "!=";
				case BinaryOperatorType.LessThan:
					return "<";
				case BinaryOperatorType.LessThanOrEqual:
					return "<=";
				case BinaryOperatorType.LogicalAnd:
					return "and";
				case BinaryOperatorType.LogicalOr:
					return "or";
				case BinaryOperatorType.Modulus:
					return "%";
				case BinaryOperatorType.Multiply:
					return "*";
				case BinaryOperatorType.ReferenceEquality:
					return "is";
				case BinaryOperatorType.Subtract:
					return "-";
				case BinaryOperatorType.Concat:
					return "+";
				default:
					return "==";
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
	
		public object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			string arrayType = arrayCreateExpression.CreateType.Type;
			if (arrayCreateExpression.ArrayInitializer.CreateExpressions.Count == 0) {
				Append("System.Array.CreateInstance(" + arrayType);
				if (arrayCreateExpression.Arguments.Count > 0) {
					foreach (Expression expression in arrayCreateExpression.Arguments) {
						Append(", ");
						expression.AcceptVisitor(this, data);
					}
					Append(")");
				} else {
					Append(", 0)");
				}
			} else {
				Append("System.Array[" + arrayType + "]");
	
				// Add initializers.
				Append("((");
				bool firstItem = true;
				foreach (Expression expression in arrayCreateExpression.ArrayInitializer.CreateExpressions) {
					if (firstItem) {
						firstItem = false;
					} else {
						Append(", ");
					}
					expression.AcceptVisitor(this, data);
				}
				Append("))");
			}
			return null;
		}
				
		public object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			switch (assignmentExpression.Op) {
				case AssignmentOperatorType.Assign:
					return CreateSimpleAssignment(assignmentExpression, "=", data);
				case AssignmentOperatorType.Add:
					if (IsAddEventHandler(assignmentExpression)) {
						return CreateHandlerStatement(assignmentExpression.Left, "+=", assignmentExpression.Right);
					} 
					return CreateSimpleAssignment(assignmentExpression, "+=", data);
				case AssignmentOperatorType.Subtract:
					if (IsRemoveEventHandler(assignmentExpression)) {
						return CreateHandlerStatement(assignmentExpression.Left, "-=", assignmentExpression.Right);
					} 
					return CreateSimpleAssignment(assignmentExpression, "-=", data);
				case AssignmentOperatorType.Modulus:
					return CreateSimpleAssignment(assignmentExpression, "%=", data);
				case AssignmentOperatorType.Multiply:
					return CreateSimpleAssignment(assignmentExpression, "*=", data);
				case AssignmentOperatorType.Divide:
				case AssignmentOperatorType.DivideInteger:
					return CreateSimpleAssignment(assignmentExpression, "/=", data);
				case AssignmentOperatorType.BitwiseAnd:
					return CreateSimpleAssignment(assignmentExpression, "&=", data);
				case AssignmentOperatorType.BitwiseOr:
					return CreateSimpleAssignment(assignmentExpression, "|=", data);
				case AssignmentOperatorType.ExclusiveOr:
					return CreateSimpleAssignment(assignmentExpression, "^=", data);
				case AssignmentOperatorType.ShiftLeft:
					return CreateSimpleAssignment(assignmentExpression, "<<=", data);
				case AssignmentOperatorType.ShiftRight:
					return CreateSimpleAssignment(assignmentExpression, ">>=", data);
				case AssignmentOperatorType.ConcatString:
					return CreateSimpleAssignment(assignmentExpression, "+=", data);
				case AssignmentOperatorType.Power:
					return CreateSimpleAssignment(assignmentExpression, "**=", data);
			}
					
			return null;
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
			Append("self");
			return null;
		}
		
		public object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			binaryOperatorExpression.Left.AcceptVisitor(this, data);
			Append(" ");
			Append(GetBinaryOperator(binaryOperatorExpression.Op));
			Append(" ");
			binaryOperatorExpression.Right.AcceptVisitor(this, data);
			return null;
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
			AppendIndentedLine("break");
			return null;
		}
		
		public object VisitCaseLabel(CaseLabel caseLabel, object data)
		{
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
		
		public object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			// Visit the child items of the compilation unit.
			compilationUnit.AcceptChildren(this, data);
			return null;
		}
		
		/// <summary>
		/// An ternary operator expression:
		/// 
		/// string a = test ? "Ape" : "Monkey";
		/// 
		/// In Python this gets converted to:
		/// 
		/// a = "Ape" if test else "Monkey"
		/// </summary>
		public object VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{
			// Add true part.
			conditionalExpression.TrueExpression.AcceptVisitor(this, data);
			
			// Add condition.
			Append(" if ");
			conditionalExpression.Condition.AcceptVisitor(this, data);
			
			// Add false part.
			Append(" else ");
			conditionalExpression.FalseExpression.AcceptVisitor(this, data);
			return null;
		}
		
		public object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			return null;
		}
		
		public object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
		{
			Console.WriteLine("VisitConstructorInitializer");
			return null;
		}
		
		public object VisitContinueStatement(ContinueStatement continueStatement, object data)
		{
			AppendIndentedLine("continue");
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
			AppendIndentedLine("def __del__(self):");
			IncreaseIndent();
			destructorDeclaration.Body.AcceptVisitor(this, data);
			DecreaseIndent();
			return null;
		}
		
		public object VisitDirectionExpression(DirectionExpression directionExpression, object data)
		{
			Console.WriteLine("VisitDirectionExpression");
			return null;
		}
		
		public object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			AppendIndented("while ");
			doLoopStatement.Condition.AcceptVisitor(this, data);
			Append(":");
			AppendNewLine();
			
			IncreaseIndent();
			doLoopStatement.EmbeddedStatement.AcceptVisitor(this, data);
			DecreaseIndent();
			
			return null;
		}

		public object VisitElseIfSection(ElseIfSection elseIfSection, object data)
		{
			// Convert condition.
			AppendIndented("elif ");
			elseIfSection.Condition.AcceptVisitor(this, data);
			Append(":");
			AppendNewLine();
			
			// Convert else if body statements.
			IncreaseIndent();
			elseIfSection.EmbeddedStatement.AcceptVisitor(this, data);
			DecreaseIndent();
			
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
		
		public object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			// Convert the expression.
			AppendIndented(String.Empty);
			expressionStatement.Expression.AcceptVisitor(this, data);
			AppendNewLine();
			return null;
		}
		
		public object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{					
			return null;
		}	
				
		public object VisitFixedStatement(FixedStatement fixedStatement, object data)
		{
			Console.WriteLine("VisitFixedStatement");
			return null;
		}
		
		public object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			// Convert the for loop's initializers.
			AppendIndented(String.Empty);
			CreateInitStatement(foreachStatement);
			AppendNewLine();

			// Convert the for loop's test expression.
			AppendIndentedLine("while enumerator.MoveNext():");

			// Move the initializer in the foreach loop to the
			// first line of the for loop's body.
			IncreaseIndent();
			AppendIndentedLine(foreachStatement.VariableName + " = enumerator.Current");
			
			// Visit the for loop's body.
			foreachStatement.EmbeddedStatement.AcceptVisitor(this, data);
			DecreaseIndent();

			return null;
		}
		
		public object VisitForNextStatement(ForNextStatement forNextStatement, object data)
		{
			Console.WriteLine("VisitForNextStatement");
			return null;
		}
		
		/// <summary>
		/// Converts from an NRefactory for loop:
		/// 
		/// for (int i = 0; i &lt; 5; i = i + 1)
		/// 
		/// to Python's:
		/// 
		/// i = 0
		/// while i &lt; 5:
		/// </summary>
		public object VisitForStatement(ForStatement forStatement, object data)
		{		
			// Convert the for loop's initializers.
			foreach (Statement statement in forStatement.Initializers) {
				statement.AcceptVisitor(this, data);
			}

			// Convert the for loop's test expression.
			AppendIndented("while ");
			forStatement.Condition.AcceptVisitor(this, data);
			Append(":");
			AppendNewLine();
					
			// Visit the for loop's body.
			IncreaseIndent();
			forStatement.EmbeddedStatement.AcceptVisitor(this, data);
		
			// Convert the for loop's increment statement.
			foreach (Statement statement in forStatement.Iterator) {
				statement.AcceptVisitor(this, data);
			}
			DecreaseIndent();
			
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
		
		public object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			if (IsField(identifierExpression.Identifier)) {	
				Append("self._" + identifierExpression.Identifier);
			} else {
				Append(identifierExpression.Identifier);
			}
			return null;
		}
		
		public object VisitIfElseStatement(IfElseStatement ifElseStatement, object data)
		{
			// Convert condition.
			AppendIndented("if ");
			ifElseStatement.Condition.AcceptVisitor(this, data);
			Append(":");
			AppendNewLine();
			
			// Convert true statements.
			IncreaseIndent();
			foreach (Statement statement in ifElseStatement.TrueStatement) {
				statement.AcceptVisitor(this, data);
			}
			DecreaseIndent();

			// Convert else if sections.
			if (ifElseStatement.HasElseIfSections) {
				foreach (ElseIfSection elseIfSection in ifElseStatement.ElseIfSections) {
					elseIfSection.AcceptVisitor(this, data);
				}
			}
			
			// Convert false statements.
			if (ifElseStatement.HasElseStatements) {
				AppendIndentedLine("else:");
				IncreaseIndent();
				foreach (Statement statement in ifElseStatement.FalseStatement) {
					statement.AcceptVisitor(this, data);
				}
				DecreaseIndent();
			}

			return null;
		}
		
		public object VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data)
		{
			Console.WriteLine("VisitIndexerDeclaration");
			return null;
		}
		
		public object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{				
			indexerExpression.TargetObject.AcceptVisitor(this, data);

			// Add indices.
			foreach (Expression expression in indexerExpression.Indexes) {
				Append("[");
				expression.AcceptVisitor(this, data);
				Append("]");
			}

			return null;
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

		public object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{					
			MemberReferenceExpression memberRefExpression = invocationExpression.TargetObject as MemberReferenceExpression;
			IdentifierExpression identifierExpression = invocationExpression.TargetObject as IdentifierExpression;
			if (memberRefExpression != null) {
				memberRefExpression.TargetObject.AcceptVisitor(this, data);
				Append("." +  memberRefExpression.MemberName);
			} else if (identifierExpression != null) {
				Append("self." + identifierExpression.Identifier);
			}
				
			// Create method parameters
			Append("(");
			bool firstParam = true;
			foreach (Expression param in invocationExpression.Arguments) {
				if (firstParam) {
					firstParam = false;
				} else {
					Append(", ");
				}
				param.AcceptVisitor(this, data);
			}
			Append(")");
			return null;
		}
		
		public object VisitLabelStatement(LabelStatement labelStatement, object data)
		{
			Console.WriteLine("VisitLabelStatement");
			return null;
		}
		
		/// <summary>
		/// The variable declaration is not created if the variable has no initializer.
		/// </summary>
		public object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{			
			VariableDeclaration variableDeclaration = localVariableDeclaration.Variables[0];
			if (!variableDeclaration.Initializer.IsNull) {

				// Create variable declaration.
				AppendIndented(variableDeclaration.Name + " = ");
			
				// Generate the variable initializer.
				variableDeclaration.Initializer.AcceptVisitor(this, data);
				AppendNewLine();
			}
			return null;
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
			// Add method name.
			string methodName = methodDeclaration.Name;
			AppendIndented("def " + methodName);
					
			// Add the parameters.
			AddParameters(methodDeclaration);
			methodParameters = methodDeclaration.Parameters;
			AppendNewLine();
			
			IncreaseIndent();
			if (methodDeclaration.Body.Children.Count > 0) {
				methodDeclaration.Body.AcceptVisitor(this, data);
			} else {
				AppendIndentedPassStatement();
			}
			
			DecreaseIndent();
			AppendNewLine();
			
			if (IsStatic(methodDeclaration)) {
				AppendIndentedLine(methodDeclaration.Name + " = staticmethod(" + methodDeclaration.Name + ")");
				AppendNewLine();
				
				// Save Main entry point method.
				SaveMethodIfMainEntryPoint(methodDeclaration);
			}
			
			return null;
		}
		
		public object VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data)
		{
			Append(namedArgumentExpression.Name);
			Append(" = ");
			namedArgumentExpression.Expression.AcceptVisitor(this, data);
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
			Append(objectCreateExpression.CreateType.Type);
			if (IsGenericType(objectCreateExpression)) {
				AppendGenericTypes(objectCreateExpression);
			}
			Append("(");

			// Add parameters.
			bool firstParameter = true;
			foreach (Expression expression in objectCreateExpression.Parameters) {
				if (!firstParameter) {
					Append(", ");
				}
				expression.AcceptVisitor(this, data);
				firstParameter = false;
			}
			
			// Add object initializers.
			bool firstInitializer = true;
			foreach (Expression expression in objectCreateExpression.ObjectInitializer.CreateExpressions) {
				if (!firstInitializer) {
					Append(", ");
				}
				expression.AcceptVisitor(this, data);
				firstInitializer = false;
			}
			
			Append(")");
			return null;
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
		
		public object VisitExternAliasDirective(ExternAliasDirective externAliasDirective, object data)
		{
			Console.WriteLine("ExternAliasDirective");
			return null;
		}
		
		public object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			Console.WriteLine("VisitParameterDeclarationExpression");
			return null;
		}
		
		public object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			Append("(" );
			parenthesizedExpression.Expression.AcceptVisitor(this, data);
			Append(")");
			return null;
		}
		
		public object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			Console.WriteLine("VisitPointerReferenceExpression");
			return null;
		}
		
		public object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			if (primitiveExpression.Value == null) {
				Append("None");
			} else if (primitiveExpression.Value is Boolean) {
				Append(primitiveExpression.Value.ToString());
			} else {
				Append(primitiveExpression.StringValue);
			}
			return null;
		}
		
		public object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			string propertyName = propertyDeclaration.Name;

			// Add get statements.
			AppendIndentedLine("def get_" + propertyName + "(self):");
			IncreaseIndent();
			propertyDeclaration.GetRegion.Block.AcceptVisitor(this, data);
			DecreaseIndent();
			AppendNewLine();
		
			// Add set statements.
			AppendIndentedLine("def set_" + propertyName + "(self, value):");
			IncreaseIndent();
			propertyDeclaration.SetRegion.Block.AcceptVisitor(this, data);
			DecreaseIndent();
			AppendNewLine();
			
			// Add property definition.
			AppendIndentedLine(String.Concat(propertyName, " = property(fget=get_", propertyName, ", fset=set_", propertyName, ")"));
			AppendNewLine();
			
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
			AppendIndented("return ");
			returnStatement.Expression.AcceptVisitor(this, data);
			AppendNewLine();
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
			bool firstSection = true;
			foreach (SwitchSection section in switchStatement.SwitchSections) {
				// Create if/elif/else condition.
				CreateSwitchCaseCondition(switchStatement.SwitchExpression, section, firstSection);

				// Create if/elif/else body.
				IncreaseIndent();
				section.AcceptChildren(this, data);
				DecreaseIndent();
						
				firstSection = false;
			}
			return null;
		}
		
		public object VisitTemplateDefinition(TemplateDefinition templateDefinition, object data)
		{
			return null;
		}
		
		public object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			Append("self");
			return null;
		}

		/// <summary>
		/// Converts an NRefactory throw statement to a code dom's throw exception statement.
		/// </summary>
		public object VisitThrowStatement(ThrowStatement throwStatement, object data)
		{
			AppendIndented("raise ");
		 	throwStatement.Expression.AcceptVisitor(this, data);
		 	AppendNewLine();
			return null;
		}
		
		/// <summary>
		/// Converts an NRefactory try-catch statement to a code dom
		/// try-catch statement.
		/// </summary>
		public object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{			
			// Convert try-catch body.
			AppendIndentedLine("try:");
			IncreaseIndent();
			tryCatchStatement.StatementBlock.AcceptVisitor(this, data);
			DecreaseIndent();
			
			// Convert catches.
			foreach (CatchClause catchClause in tryCatchStatement.CatchClauses) {
				AppendIndented("except ");
				Append(catchClause.TypeReference.Type);
				Append(", " + catchClause.VariableName + ":");
				AppendNewLine();
			
				// Convert catch child statements.
				IncreaseIndent();
				catchClause.StatementBlock.AcceptVisitor(this, data);
				DecreaseIndent();
			}
			
			// Convert finally block.
			AppendIndentedLine("finally:");
			IncreaseIndent();
			tryCatchStatement.FinallyBlock.AcceptVisitor(this, data);
			DecreaseIndent();
			
			return null;
		}
		
		/// <summary>
		/// Visits a class.
		/// </summary>
		public object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			AppendIndentedLine("class " + typeDeclaration.Name + "(object):");
			IncreaseIndent();
			if (typeDeclaration.Children.Count > 0) {
				// Look for fields or a constructor for the type.
				constructorInfo = PythonConstructorInfo.GetConstructorInfo(typeDeclaration);
				if (constructorInfo != null) {
					CreateConstructor(constructorInfo);
				}
				
				// Visit the rest of the class.
				typeDeclaration.AcceptChildren(this, data);
			} else {
				AppendIndentedPassStatement();
			}
			DecreaseIndent();

			return null;
		}
		
		public object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			Append("clr.GetClrType(");
			Append(typeOfExpression.TypeReference.Type);
			Append(")");
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
					// Change i++ or ++i to i += 1
					return CreateIncrementStatement(unaryOperatorExpression);
				case UnaryOperatorType.Decrement:
				case UnaryOperatorType.PostDecrement:
					// Change --i or i-- to i -= 1.
					return CreateDecrementStatement(unaryOperatorExpression);
				case UnaryOperatorType.Minus:
					return CreateUnaryOperatorStatement(GetBinaryOperator(BinaryOperatorType.Subtract), unaryOperatorExpression.Expression);
				case UnaryOperatorType.Plus:
					return CreateUnaryOperatorStatement(GetBinaryOperator(BinaryOperatorType.Add), unaryOperatorExpression.Expression);
				case UnaryOperatorType.Not:
					return CreateUnaryOperatorStatement("not ", unaryOperatorExpression.Expression);
				case UnaryOperatorType.BitNot:
					return CreateUnaryOperatorStatement("~", unaryOperatorExpression.Expression);
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
			// Add import statements for each using.
			foreach (Using @using in usingDeclaration.Usings) {		
				AppendIndentedLine("from " + @using.Name + " import *");
			}
			return null;
		}
		
		public object VisitUsingStatement(UsingStatement usingStatement, object data)
		{
			return null;
		}
		
		public object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			AppendIndented(variableDeclaration.Name + " = ");
			variableDeclaration.Initializer.AcceptVisitor(this, data);
			AppendNewLine();
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
			memberReferenceExpression.TargetObject.AcceptVisitor(this, data);
			if (memberReferenceExpression.TargetObject is ThisReferenceExpression) {
				Append("._"); 
			} else {
				Append(".");
			}
			Append(memberReferenceExpression.MemberName);
			return null;
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
		/// Converts a post or pre increment expression to an assign statement.
		/// This converts "i++" and "++i" to "i = i + 1" since python
		/// does not support post increment expressions.
		/// </summary>
		object CreateIncrementStatement(UnaryOperatorExpression unaryOperatorExpression)
		{
			return CreateIncrementStatement(unaryOperatorExpression, 1, GetBinaryOperator(BinaryOperatorType.Add));
		}

		/// <summary>
		/// Converts a post or pre decrement expression to an assign statement.
		/// This converts "i--" and "--i" to "i -= 1" since python 
		/// does not support post increment expressions.
		/// </summary>
		object CreateDecrementStatement(UnaryOperatorExpression unaryOperatorExpression)
		{
			return CreateIncrementStatement(unaryOperatorExpression, 1, GetBinaryOperator(BinaryOperatorType.Subtract));
		}
		
		/// <summary>
		/// Converts a post or pre increment expression to an assign statement.
		/// This converts "i++" and "++i" to "i += 1" since python 
		/// does not support post increment expressions.
		/// </summary>
		object CreateIncrementStatement(UnaryOperatorExpression unaryOperatorExpression, int increment, string binaryOperator)
		{
			unaryOperatorExpression.Expression.AcceptVisitor(this, null);
			Append(" " + binaryOperator + "= ");
			Append(increment.ToString());
			
			return null;
		}
		
		/// <summary>
		/// Creates the statement used to initialize the for loop. The
		/// initialize statement will be "enumerator = variableName.GetEnumerator()"
		/// which simulates what happens in a foreach loop.
		/// </summary>
		object CreateInitStatement(ForeachStatement foreachStatement)
		{
			Append("enumerator = ");
			AppendForeachVariableName(foreachStatement);
			Append(".GetEnumerator()");

			return null;
		}
		
		/// <summary>
		/// Gets the name of the variable that is used in the
		/// foreach loop as the item being iterated and appends the code.
		/// </summary>
		void AppendForeachVariableName(ForeachStatement foreachStatement)
		{
			IdentifierExpression identifierExpression = foreachStatement.Expression as IdentifierExpression;
			InvocationExpression invocationExpression = foreachStatement.Expression as InvocationExpression;
			if (identifierExpression != null) {
				Append(identifierExpression.Identifier);
			} else if (invocationExpression != null) {
				invocationExpression.AcceptVisitor(this, null);
			}
		}
				
		/// <summary>
		/// Determines whether the identifier refers to a field in the
		/// current class.
		/// </summary>
		bool IsField(string name)
		{
			// Check the current method's parameters.
			foreach (ParameterDeclarationExpression param in methodParameters) {
				if (param.ParameterName == name) {
					return false;
				}
			}

			// Check the current class's fields.
			if (constructorInfo != null) {
				foreach (FieldDeclaration field in constructorInfo.Fields) {
					if (field.Fields[0].Name == name) {
						return true;
					}
				}
			}
			return false;
		}		
		
		/// <summary>
		/// Creates an attach statement (i.e. button.Click += ButtonClick)
		/// or remove statement (i.e. button.Click -= ButtonClick)
		/// </summary>
		object CreateHandlerStatement(Expression eventExpression, string addRemoveOperator, Expression eventHandlerExpression)
		{
			CreateEventReferenceExpression(eventExpression);
			Append(" " + addRemoveOperator + " ");
			CreateDelegateCreateExpression(eventHandlerExpression);
			return null;
		}
		
		/// <summary>
		/// Converts an expression to a CodeEventReferenceExpression
		/// (i.e. the "button.Click" part of "button.Click += ButtonClick".
		/// </summary>
		object CreateEventReferenceExpression(Expression eventExpression)
		{
			// Create event reference.
			MemberReferenceExpression memberRef = eventExpression as MemberReferenceExpression;
			memberRef.AcceptVisitor(this, null);
			return null;
		}
		
		/// <summary>
		/// Creates an event handler expression
		/// (i.e. the "ButtonClick" part of "button.Click += ButtonClick")
		/// </summary>
		object CreateDelegateCreateExpression(Expression eventHandlerExpression)
		{
			// Create event handler expression.
			IdentifierExpression identifierExpression = eventHandlerExpression as IdentifierExpression;
			ObjectCreateExpression objectCreateExpression = eventHandlerExpression as ObjectCreateExpression;
			if (identifierExpression != null) {
				Append(identifierExpression.Identifier);
			} else if (objectCreateExpression != null) {
				CreateDelegateCreateExpression(objectCreateExpression.Parameters[0]);
			}
			return null;
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
				
		void Append(string code)
		{
			codeBuilder.Append(code);
		}
		
		void AppendIndented(string code)
		{
			for (int i = 0; i < indent; ++i) {
				codeBuilder.Append(indentString);
			}
			codeBuilder.Append(code);
		}
		
		void AppendIndentedPassStatement()
		{
			AppendIndentedLine("pass");
		}
		
		void AppendIndentedLine(string code)
		{
			AppendIndented(code + "\r\n");
		}
		
		void AppendNewLine()
		{
			Append("\r\n");
		}
		
		void IncreaseIndent()
		{
			++indent;
		}
		
		void DecreaseIndent()
		{
			--indent;
		}
		
		void CreateConstructor(PythonConstructorInfo constructorInfo)
		{
			if (constructorInfo.Constructor != null) {
				AppendIndented("def __init__");
				AddParameters(constructorInfo.Constructor);
				methodParameters = constructorInfo.Constructor.Parameters;
			} else {
				AppendIndented("def __init__(self):");
			}
			AppendNewLine();
			
			// Add fields at start of constructor.
			IncreaseIndent();
			if (constructorInfo.Fields.Count > 0) {
				foreach (FieldDeclaration field in constructorInfo.Fields) {
					// Ignore field if it has no initializer.
					if (FieldHasInitialValue(field)) {
						CreateFieldInitialization(field);
					}
				}
			}  
			
			if (!IsEmptyConstructor(constructorInfo.Constructor)) {
				constructorInfo.Constructor.Body.AcceptVisitor(this, null);
				AppendNewLine();
			} else if (constructorInfo.Fields.Count == 0) {
				AppendIndentedPassStatement();
			} else {
				AppendNewLine();
			}
			
			DecreaseIndent();
		}
		
		/// <summary>
		/// Returns true if the constructor has no statements in its body.
		/// </summary>
		static bool IsEmptyConstructor(ConstructorDeclaration constructor)
		{
			if (constructor != null) {
				return constructor.Body.Children.Count == 0;
			}
			return true;
		}
		
		/// <summary>
		/// Creates a field initialization statement.
		/// </summary>
		void CreateFieldInitialization(FieldDeclaration field)
		{
			VariableDeclaration variable = field.Fields[0];
			string oldVariableName = variable.Name;
			variable.Name = "self._" + variable.Name;
			VisitVariableDeclaration(variable, null);
			variable.Name = oldVariableName;
		}
		
		/// <summary>
		/// Adds the method or constructor parameters.
		/// </summary>
		void AddParameters(ParametrizedNode method)
		{			
			Append("(");
			List<ParameterDeclarationExpression> parameters = method.Parameters;
			if (parameters.Count > 0) {
				if (!IsStatic(method)) {
					Append("self, ");
				}
				for (int i = 0; i < parameters.Count; ++i) {
					if (i > 0) {
						Append(", ");
					}
					Append(parameters[i].ParameterName);
				}
			} else {
				if (!IsStatic(method)) {
					Append("self");
				}				
			}
			Append("):");
		}
				
		bool IsStatic(ParametrizedNode method)
		{
			return (method.Modifier & Modifiers.Static) == Modifiers.Static;
		}
			
		/// <summary>
		/// Creates assignments of the form:
		/// i = 1
		/// </summary>
		object CreateSimpleAssignment(AssignmentExpression assignmentExpression, string op, object data)
		{
			assignmentExpression.Left.AcceptVisitor(this, data);
			Append(" " + op + " ");
			assignmentExpression.Right.AcceptVisitor(this, data);
			return null;
		}

		/// <summary>
		/// Creates the rhs of expressions such as:
		/// i = -1
		/// i = +1
		/// </summary>
		object CreateUnaryOperatorStatement(string op, Expression expression)
		{
			Append(op);
			expression.AcceptVisitor(this, null);
			return null;
		}
		
		/// <summary>
		/// Converts a switch case statement to an if/elif/else in Python.
		/// </summary>
		/// <param name="switchExpression">This contains the item being tested in the switch.</param>
		/// <param name="section">This contains the switch section currently being converted.</param>
		/// <param name="firstSection">True if the section is the first in the switch. If true then
		/// an if statement will be generated, otherwise an elif will be generated.</param>
		void CreateSwitchCaseCondition(Expression switchExpression, SwitchSection section, bool firstSection)
		{
			bool firstLabel = true;
			foreach (CaseLabel label in section.SwitchLabels) {
				if (firstLabel) {
					if (label.IsDefault) {
						// Create else condition.
						AppendIndented("else");
					} else if (firstSection) {
						// Create if condition.
						AppendIndented(String.Empty);
						CreateSwitchCaseCondition("if ", switchExpression, label);
					} else {
						// Create elif condition.
						AppendIndented(String.Empty);
						CreateSwitchCaseCondition("elif ", switchExpression, label);
					}
				} else {
					CreateSwitchCaseCondition(" or ", switchExpression, label);
				}	
				firstLabel = false;
			}	
			
			Append(":");
			AppendNewLine();
		}
		
		/// <summary>
		/// Creates the switch test condition
		/// </summary>
		/// <param name="prefix">This is a string which is either "if ", "elif ", "else " or " or ".</param>
		void CreateSwitchCaseCondition(string prefix, Expression switchExpression, CaseLabel label)
		{
			Append(prefix);
			switchExpression.AcceptVisitor(this, null);
			Append(" == ");
			label.Label.AcceptVisitor(this, null);
		}
		
		/// <summary>
		/// Gets the supported language either C# or VB.NET
		/// </summary>
		static SupportedLanguage GetSupportedLanguage(string fileName)
		{
			string extension = Path.GetExtension(fileName.ToLowerInvariant());
			if (extension == ".vb") {
				return SupportedLanguage.VBNet;
			}
			return SupportedLanguage.CSharp;
		}
		
		/// <summary>
		/// Saves the method declaration if it is a main entry point.
		/// </summary>
		void SaveMethodIfMainEntryPoint(MethodDeclaration method)
		{
			if (method.Name == "Main") {
				entryPointMethods.Add(method);
			}
		}
		
		/// <summary>
		/// Returns true if the object being created is a generic.
		/// </summary>
		static bool IsGenericType(ObjectCreateExpression expression)
		{
			return expression.CreateType.GenericTypes.Count > 0;
		}
		
		/// <summary>
		/// Appends the types used when creating a generic surrounded by square brackets.
		/// </summary>
		void AppendGenericTypes(ObjectCreateExpression expression)
		{
			Append("[");
			List<TypeReference> typeRefs = expression.CreateType.GenericTypes;
			for (int i = 0; i < typeRefs.Count; ++i) {
				if (i != 0) {
					Append(", ");
				}
				TypeReference typeRef = typeRefs[i];
				if (typeRef.IsArrayType) {
					Append("System.Array[" + typeRef.Type + "]");
				} else {
					Append(typeRef.Type);
				}
			}
			Append("]");
		}
	}
}
