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
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Visitors;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Used to convert VB.NET and C# to Python.
	/// </summary>
	public class NRefactoryToPythonConverter : NodeTrackingAstVisitor, IOutputFormatter
	{
		string indentString = "\t";
		PythonCodeBuilder codeBuilder;
		
		// Holds the constructor for the class being converted. This is used to identify class fields.
		PythonConstructorInfo constructorInfo;
		
		// Holds the parameters of the current method. This is used to identify
		// references to fields or parameters.
		List<ParameterDeclarationExpression> methodParameters = new List<ParameterDeclarationExpression>();
		MethodDeclaration currentMethod;

		// Holds the names of any parameters defined for this class.
		List<string> propertyNames = new List<string>();
		
		SupportedLanguage language;
		List<MethodDeclaration> entryPointMethods;

		SpecialNodesInserter specialNodesInserter;
		INode currentNode;
		List<Comment> xmlDocComments = new List<Comment>();
		
		static readonly string Docstring = "\"\"\"";

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
		/// Gets or sets the string that will be used to indent the generated Python code.
		/// </summary>
		public string IndentString {
			get { return indentString; }
			set { indentString = value; }
		}

		/// <summary>
		/// Generates compilation unit from the code.
		/// </summary>
		/// <remarks>
		/// Uses ISpecials so comments can be converted.
		/// </remarks>
		/// <param name="source">
		/// The code to convert to a compilation unit.
		/// </param>
		public CompilationUnit GenerateCompilationUnit(string source, SupportedLanguage language)
		{
			using (IParser parser = ParserFactory.CreateParser(language, new StringReader(source))) {
				parser.Parse();
				parser.CompilationUnit.UserData = parser.Lexer.SpecialTracker.RetrieveSpecials();
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
			
			SpecialOutputVisitor specialOutputVisitor = new SpecialOutputVisitor(this);
			specialNodesInserter = new SpecialNodesInserter(unit.UserData as List<ISpecial>, specialOutputVisitor);
			
			// Convert to Python code.
			entryPointMethods = new List<MethodDeclaration>();
			codeBuilder = new PythonCodeBuilder();
			codeBuilder.IndentString = indentString;
			unit.AcceptVisitor(this, null);
			
			return codeBuilder.ToString().Trim();
		}
		
		/// <summary>
		/// Gets a list of possible entry point methods found when converting the
		/// python source code.
		/// </summary>
		public ReadOnlyCollection<MethodDeclaration> EntryPointMethods {
			get { return entryPointMethods.AsReadOnly(); }
		}
		
		/// <summary>
		/// Generates code to call the main entry point.
		/// </summary>
		public string GenerateMainMethodCall(MethodDeclaration methodDeclaration)
		{
			StringBuilder code = new StringBuilder();
			code.Append(GetTypeName(methodDeclaration));
			code.Append('.');
			code.Append(methodDeclaration.Name);
			code.Append('(');
			if (methodDeclaration.Parameters.Count > 0) {
				code.Append("None");
			}
			code.Append(')');
			
			return code.ToString();
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
				case BinaryOperatorType.ExclusiveOr:
					return "^";
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
		
		public override object TrackedVisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data)
		{
			Console.WriteLine("VisitAddHandlerStatement");
			return null;
		}
		
		public override object TrackedVisitAddressOfExpression(AddressOfExpression addressOfExpression, object data)
		{
			Console.WriteLine("VisitAddressOfExpression");
			return null;
		}
		
		public override object TrackedVisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			Console.WriteLine("VisitAnonymousMethodExpression");
			return null;
		}
		
		public override object TrackedVisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			string arrayType = GetTypeName(arrayCreateExpression.CreateType);
			if (arrayCreateExpression.ArrayInitializer.CreateExpressions.Count == 0) {
				Append("Array.CreateInstance(" + arrayType);
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
				Append("Array[" + arrayType + "]");
				
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
		
		public override object TrackedVisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
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
		
		public override object TrackedVisitAttribute(ICSharpCode.NRefactory.Ast.Attribute attribute, object data)
		{
			return null;
		}
		
		public override object TrackedVisitAttributeSection(AttributeSection attributeSection, object data)
		{
			return null;
		}

		/// <summary>
		/// Converts a base class reference to a this reference.
		/// Python has no concept of a direct base class reference so
		/// "base" is converted to "self".
		/// </summary>
		public override object TrackedVisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			Append("self");
			return null;
		}
		
		public override object TrackedVisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
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
		public override object TrackedVisitBlockStatement(BlockStatement blockStatement, object data)
		{
			return blockStatement.AcceptChildren(this, data);
		}
		
		public override object TrackedVisitBreakStatement(BreakStatement breakStatement, object data)
		{
			AppendIndentedLine("break");
			return null;
		}
		
		public override object TrackedVisitCaseLabel(CaseLabel caseLabel, object data)
		{
			return null;
		}

		/// <summary>
		/// Ignore the cast and just visit the expression inside the cast.
		/// </summary>
		public override object TrackedVisitCastExpression(CastExpression castExpression, object data)
		{
			return castExpression.Expression.AcceptVisitor(this, data);
		}
		
		public override object TrackedVisitCatchClause(CatchClause catchClause, object data)
		{
			Console.WriteLine("VisitCatchClause");
			return null;
		}
		
		public override object TrackedVisitCheckedExpression(CheckedExpression checkedExpression, object data)
		{
			Console.WriteLine("VisitCheckedExpression");
			return null;
		}
		
		public override object TrackedVisitCheckedStatement(CheckedStatement checkedStatement, object data)
		{
			Console.WriteLine("VisitCheckedStatement");
			return null;
		}
		
		public override object TrackedVisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data)
		{
			Console.WriteLine("VisitClassReferenceExpression");
			return null;
		}
		
		public override object TrackedVisitCompilationUnit(CompilationUnit compilationUnit, object data)
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
		public override object TrackedVisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
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
		
		public override object TrackedVisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			CreateConstructor(constructorInfo);
			return null;
		}
		
		public override object TrackedVisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
		{
			Console.WriteLine("VisitConstructorInitializer");
			return null;
		}
		
		public override object TrackedVisitContinueStatement(ContinueStatement continueStatement, object data)
		{
			AppendIndentedLine("continue");
			return null;
		}
		
		public override object TrackedVisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data)
		{
			Console.WriteLine("VisitDeclareDeclaration");
			return null;
		}
		
		public override object TrackedVisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			Console.WriteLine("VisitDefaultValueExpression");
			return null;
		}
		
		public override object TrackedVisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			Console.WriteLine("VisitDelegateDeclaration");
			return null;
		}
		
		public override object TrackedVisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			AppendIndentedLine("def __del__(self):");
			IncreaseIndent();
			destructorDeclaration.Body.AcceptVisitor(this, data);
			DecreaseIndent();
			return null;
		}
		
		public override object TrackedVisitDirectionExpression(DirectionExpression directionExpression, object data)
		{
			Console.WriteLine("VisitDirectionExpression");
			return null;
		}
		
		public override object TrackedVisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			AppendIndented("while ");
			doLoopStatement.Condition.AcceptVisitor(this, data);
			Append(":");
			AppendLine();
			
			IncreaseIndent();
			doLoopStatement.EmbeddedStatement.AcceptVisitor(this, data);
			DecreaseIndent();
			
			return null;
		}

		public override object TrackedVisitElseIfSection(ElseIfSection elseIfSection, object data)
		{
			// Convert condition.
			AppendIndented("elif ");
			elseIfSection.Condition.AcceptVisitor(this, data);
			Append(":");
			AppendLine();
			
			// Convert else if body statements.
			IncreaseIndent();
			elseIfSection.EmbeddedStatement.AcceptVisitor(this, data);
			DecreaseIndent();
			
			return null;
		}
		
		public override object TrackedVisitEmptyStatement(EmptyStatement emptyStatement, object data)
		{
			Console.WriteLine("VisitEmptyStatement");
			return null;
		}
		
		public override object TrackedVisitEndStatement(EndStatement endStatement, object data)
		{
			Console.WriteLine("VistEndStatement");
			return null;
		}
		
		public override object TrackedVisitEraseStatement(EraseStatement eraseStatement, object data)
		{
			Console.WriteLine("VisitEraseStatement");
			return null;
		}
		
		public override object TrackedVisitErrorStatement(ErrorStatement errorStatement, object data)
		{
			Console.WriteLine("VisitErrorStatement");
			return null;
		}
		
		public override object TrackedVisitEventAddRegion(EventAddRegion eventAddRegion, object data)
		{
			Console.WriteLine("VisitEventAddRegion");
			return null;
		}
		
		public override object TrackedVisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			Console.WriteLine("VisitEventDeclaration");
			return null;
		}
		
		public override object TrackedVisitEventRaiseRegion(EventRaiseRegion eventRaiseRegion, object data)
		{
			Console.WriteLine("VisitEventRaiseRegion");
			return null;
		}
		
		public override object TrackedVisitEventRemoveRegion(EventRemoveRegion eventRemoveRegion, object data)
		{
			Console.WriteLine("VisitEventRemoveRegion");
			return null;
		}
		
		public override object TrackedVisitExitStatement(ExitStatement exitStatement, object data)
		{
			AppendIndentedLine("break");
			return null;
		}
		
		public override object TrackedVisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			// Convert the expression.
			AppendIndented(String.Empty);
			expressionStatement.Expression.AcceptVisitor(this, data);
			AppendLine();
			return null;
		}
		
		public override object TrackedVisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			return null;
		}
		
		public override object TrackedVisitFixedStatement(FixedStatement fixedStatement, object data)
		{
			Console.WriteLine("VisitFixedStatement");
			return null;
		}
		
		public override object TrackedVisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			// Convert the for loop's initializers.
			AppendIndented(String.Empty);
			CreateInitStatement(foreachStatement);
			AppendLine();

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
		
		/// <summary>
		/// Converts from an NRefactory VB.NET for next loop:
		/// 
		/// for i As Integer = 0 To 4
		/// Next
		/// 
		/// to Python's:
		/// 
		/// i = 0
		/// while i &lt; 5:
		/// </summary>
		public override object TrackedVisitForNextStatement(ForNextStatement forNextStatement, object data)
		{
			// Convert the for loop's initializers.
			string variableName = forNextStatement.VariableName;
			AppendIndented(variableName);
			Append(" = ");
			forNextStatement.Start.AcceptVisitor(this, data);
			AppendLine();
			
			// Convert the for loop's test expression.
			AppendIndented("while ");
			Append(variableName);
			Append(" <= ");
			forNextStatement.End.AcceptVisitor(this, data);
			Append(":");
			AppendLine();
			
			// Visit the for loop's body.
			IncreaseIndent();
			forNextStatement.EmbeddedStatement.AcceptVisitor(this, data);
			
			// Convert the for loop's increment statement.
			AppendIndented(variableName);
			Append(" = ");
			Append(variableName);
			Append(" + ");
			if (forNextStatement.Step.IsNull) {
				Append("1");
			} else {
				forNextStatement.Step.AcceptVisitor(this, data);
			}
			AppendLine();
			DecreaseIndent();
			
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
		public override object TrackedVisitForStatement(ForStatement forStatement, object data)
		{
			// Convert the for loop's initializers.
			foreach (Statement statement in forStatement.Initializers) {
				statement.AcceptVisitor(this, data);
			}

			// Convert the for loop's test expression.
			AppendIndented("while ");
			forStatement.Condition.AcceptVisitor(this, data);
			Append(":");
			AppendLine();
			
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
		
		public override object TrackedVisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data)
		{
			Console.WriteLine("VisitGotoCaseStatement");
			return null;
		}
		
		public override object TrackedVisitGotoStatement(GotoStatement gotoStatement, object data)
		{
			Console.WriteLine("VisitGotoStatement");
			return null;
		}
		
		public override object TrackedVisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			string name = identifierExpression.Identifier;
			if (IsField(name)) {
				Append("self._" + name);
			} else if (IsProperty(name) && !IsMethodParameter(name)) {
				Append("self." + name);
			} else {
				Append(name);
			}
			return null;
		}
		
		public override object TrackedVisitIfElseStatement(IfElseStatement ifElseStatement, object data)
		{
			// Convert condition.
			AppendIndented("if ");
			ifElseStatement.Condition.AcceptVisitor(this, data);
			Append(":");
			AppendLine();
			
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
		
		public override object TrackedVisitIndexerExpression(IndexerExpression indexerExpression, object data)
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
		
		public override object TrackedVisitInnerClassTypeReference(InnerClassTypeReference innerClassTypeReference, object data)
		{
			Console.WriteLine("VisitInnerClassTypeReference");
			return null;
		}
		
		public override object TrackedVisitInterfaceImplementation(InterfaceImplementation interfaceImplementation, object data)
		{
			Console.WriteLine("VisitInterfaceImplementation");
			return null;
		}

		public override object TrackedVisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			MemberReferenceExpression memberRefExpression = invocationExpression.TargetObject as MemberReferenceExpression;
			IdentifierExpression identifierExpression = invocationExpression.TargetObject as IdentifierExpression;
			if (memberRefExpression != null) {
				memberRefExpression.TargetObject.AcceptVisitor(this, data);
				Append("." +  memberRefExpression.MemberName);
			} else if (identifierExpression != null) {
				if ((currentMethod != null) && IsStatic(currentMethod)) {
					Append(GetTypeName(currentMethod) + ".");
				} else {
					Append("self.");
				}
				Append(identifierExpression.Identifier);
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
		
		public override object TrackedVisitLabelStatement(LabelStatement labelStatement, object data)
		{
			Console.WriteLine("VisitLabelStatement");
			return null;
		}
		
		/// <summary>
		/// The variable declaration is not created if the variable has no initializer.
		/// </summary>
		public override object TrackedVisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			foreach (VariableDeclaration variableDeclaration in localVariableDeclaration.Variables) {
				if (!variableDeclaration.Initializer.IsNull) {
					
					AddTypeToArrayInitializerIfMissing(variableDeclaration);
	
					// Create variable declaration.
					AppendIndented(variableDeclaration.Name + " = ");
					
					// Generate the variable initializer.
					variableDeclaration.Initializer.AcceptVisitor(this, data);
					AppendLine();
				}
			}
			return null;
		}
		
		public override object TrackedVisitLockStatement(LockStatement lockStatement, object data)
		{
			Console.WriteLine("VisitLockStatement");
			return null;
		}
		
		public override object TrackedVisitMemberInitializerExpression(MemberInitializerExpression memberInitializerExpression, object data)
		{
			Append(memberInitializerExpression.Name);
			Append(" = ");
			memberInitializerExpression.Expression.AcceptVisitor(this, data);
			return null;
		}
		
		/// <summary>
		/// Adds a CodeMemberMethod to the current class being visited.
		/// </summary>
		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			// Add method name.
			currentMethod = methodDeclaration;
			string methodName = methodDeclaration.Name;
			AppendIndented("def " + methodName);
			
			// Add the parameters.
			AddParameters(methodDeclaration);
			methodParameters = methodDeclaration.Parameters;
			AppendLine();
			
			IncreaseIndent();
			AppendDocstring(xmlDocComments);
			if (methodDeclaration.Body.Children.Count > 0) {
				methodDeclaration.Body.AcceptVisitor(this, data);
			} else {
				AppendIndentedPassStatement();
			}
			
			DecreaseIndent();
			AppendLine();
			
			if (IsStatic(methodDeclaration)) {
				AppendIndentedLine(methodDeclaration.Name + " = staticmethod(" + methodDeclaration.Name + ")");
				AppendLine();
				
				// Save Main entry point method.
				SaveMethodIfMainEntryPoint(methodDeclaration);
			}
			
			currentMethod = null;
			
			return null;
		}
		
		public override object TrackedVisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data)
		{
			Append(namedArgumentExpression.Name);
			Append(" = ");
			namedArgumentExpression.Expression.AcceptVisitor(this, data);
			return null;
		}
		
		/// <summary>
		/// Visits the namespace declaration and all child nodes.
		/// </summary>
		public override object TrackedVisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			return namespaceDeclaration.AcceptChildren(this, data);
		}
		
		/// <summary>
		/// Converts an NRefactory's ObjectCreateExpression to a code dom's
		/// CodeObjectCreateExpression.
		/// </summary>
		public override object TrackedVisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
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
		
		public override object TrackedVisitOnErrorStatement(OnErrorStatement onErrorStatement, object data)
		{
			return null;
		}
		
		public override object TrackedVisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			Console.WriteLine("VisitOperatorDeclaration");
			return null;
		}
		
		public override object TrackedVisitOptionDeclaration(OptionDeclaration optionDeclaration, object data)
		{
			Console.WriteLine("VisitOptionDeclaration");
			return null;
		}
		
		public override object TrackedVisitExternAliasDirective(ExternAliasDirective externAliasDirective, object data)
		{
			Console.WriteLine("ExternAliasDirective");
			return null;
		}
		
		public override object TrackedVisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			Console.WriteLine("VisitParameterDeclarationExpression");
			return null;
		}
		
		public override object TrackedVisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			Append("(" );
			parenthesizedExpression.Expression.AcceptVisitor(this, data);
			Append(")");
			return null;
		}
		
		public override object TrackedVisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			Console.WriteLine("VisitPointerReferenceExpression");
			return null;
		}
		
		public override object TrackedVisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
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
		
		public override object TrackedVisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			string propertyName = propertyDeclaration.Name;
			propertyNames.Add(propertyName);

			// Add get statements.
			if (propertyDeclaration.HasGetRegion) {
				AppendIndentedLine("def get_" + propertyName + "(self):");
				IncreaseIndent();
				propertyDeclaration.GetRegion.Block.AcceptVisitor(this, data);
				DecreaseIndent();
				AppendLine();
			}
			
			// Add set statements.
			if (propertyDeclaration.HasSetRegion) {
				AppendIndentedLine("def set_" + propertyName + "(self, value):");
				IncreaseIndent();
				propertyDeclaration.SetRegion.Block.AcceptVisitor(this, data);
				DecreaseIndent();
				AppendLine();
			}
			
			AppendPropertyDecorator(propertyDeclaration);
			AppendLine();
			
			return null;
		}
		
		public override object TrackedVisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data)
		{
			Console.WriteLine("VisitPropertyGetRegion");
			return null;
		}
		
		public override object TrackedVisitPropertySetRegion(PropertySetRegion propertySetRegion, object data)
		{
			Console.WriteLine("VisitPropertySetRegion");
			return null;
		}
		
		public override object TrackedVisitRaiseEventStatement(RaiseEventStatement raiseEventStatement, object data)
		{
			Console.WriteLine("VisitRaiseEventStatement");
			return null;
		}
		
		public override object TrackedVisitReDimStatement(ReDimStatement reDimStatement, object data)
		{
			Console.WriteLine("VisitReDimStatement");
			return null;
		}
		
		public override object TrackedVisitRemoveHandlerStatement(RemoveHandlerStatement removeHandlerStatement, object data)
		{
			Console.WriteLine("VisitRemoveHandlerStatement");
			return null;
		}
		
		public override object TrackedVisitResumeStatement(ResumeStatement resumeStatement, object data)
		{
			Console.WriteLine("VisitResumeStatement");
			return null;
		}
		
		/// <summary>
		/// Converts a NRefactory ReturnStatement to a code dom's
		/// CodeMethodReturnStatement.
		/// </summary>
		public override object TrackedVisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			AppendIndented("return ");
			returnStatement.Expression.AcceptVisitor(this, data);
			AppendLine();
			return null;
		}
		
		public override object TrackedVisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
		{
			Console.WriteLine("VisitSizeOfExpression");
			return null;
		}
		
		public override object TrackedVisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
		{
			return null;
		}
		
		public override object TrackedVisitStopStatement(StopStatement stopStatement, object data)
		{
			return null;
		}
		
		public override object TrackedVisitSwitchSection(SwitchSection switchSection, object data)
		{
			return null;
		}
		
		public override object TrackedVisitSwitchStatement(SwitchStatement switchStatement, object data)
		{
			bool firstSection = true;
			foreach (SwitchSection section in switchStatement.SwitchSections) {
				// Create if/elif/else condition.
				CreateSwitchCaseCondition(switchStatement.SwitchExpression, section, firstSection);

				// Create if/elif/else body.
				IncreaseIndent();
				CreateSwitchCaseBody(section);
				DecreaseIndent();
				
				firstSection = false;
			}
			return null;
		}
		
		public override object TrackedVisitTemplateDefinition(TemplateDefinition templateDefinition, object data)
		{
			return null;
		}
		
		public override object TrackedVisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			Append("self");
			return null;
		}

		/// <summary>
		/// Converts an NRefactory throw statement to a code dom's throw exception statement.
		/// </summary>
		public override object TrackedVisitThrowStatement(ThrowStatement throwStatement, object data)
		{
			AppendIndented("raise ");
			throwStatement.Expression.AcceptVisitor(this, data);
			AppendLine();
			return null;
		}
		
		/// <summary>
		/// Converts an NRefactory try-catch statement to a code dom
		/// try-catch statement.
		/// </summary>
		public override object TrackedVisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
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
				AppendLine();
				
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
		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			codeBuilder.AppendLineIfPreviousLineIsCode();
			AppendIndented("class " + typeDeclaration.Name);
			AppendBaseTypes(typeDeclaration.BaseTypes);
			AppendLine();
			IncreaseIndent();
			AppendDocstring(xmlDocComments);
			if (typeDeclaration.Children.Count > 0) {
				// Look for fields or a constructor for the type.
				constructorInfo = PythonConstructorInfo.GetConstructorInfo(typeDeclaration);
				if (constructorInfo != null) {
					if (constructorInfo.Constructor != null) {
						// Generate constructor later when VisitConstructorDeclaration method is called.
						// This allows the constructor comments to be converted in the right place.
					} else {
						CreateConstructor(constructorInfo);
					}
				}
				
				// Visit the rest of the class.
				typeDeclaration.AcceptChildren(this, data);
			} else {
				AppendIndentedPassStatement();
			}
			DecreaseIndent();

			return null;
		}
		
		public override object TrackedVisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			codeBuilder.InsertIndentedLine("import clr\r\n");
			Append("clr.GetClrType(");
			Append(GetTypeName(typeOfExpression.TypeReference));
			Append(")");
			return null;
		}
		
		public override object TrackedVisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
		{
			Console.WriteLine("VisitTypeOfIsExpression");
			return null;
		}
		
		public override object TrackedVisitTypeReference(TypeReference typeReference, object data)
		{
			Console.WriteLine("VisitTypeReference");
			return null;
		}
		
		public override object TrackedVisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data)
		{
			Append(GetTypeName(typeReferenceExpression.TypeReference));
			return null;
		}
		
		public override object TrackedVisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
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
		
		public override object TrackedVisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			return null;
		}
		
		public override object TrackedVisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
		{
			return null;
		}
		
		public override object TrackedVisitUnsafeStatement(UnsafeStatement unsafeStatement, object data)
		{
			return null;
		}
		
		public override object TrackedVisitUsing(Using @using, object data)
		{
			return null;
		}
		
		/// <summary>
		/// Converts using declarations into Python import statements.
		/// </summary>
		public override object TrackedVisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			// Add import statements for each using.
			foreach (Using @using in usingDeclaration.Usings) {
				AppendIndentedLine("from " + @using.Name + " import *");
			}
			return null;
		}
		
		public override object TrackedVisitUsingStatement(UsingStatement usingStatement, object data)
		{
			return null;
		}
		
		public override object TrackedVisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			AppendIndented(variableDeclaration.Name + " = ");
			variableDeclaration.Initializer.AcceptVisitor(this, data);
			AppendLine();
			return null;
		}
		
		public override object TrackedVisitWithStatement(WithStatement withStatement, object data)
		{
			return null;
		}
		
		public override object TrackedVisitYieldStatement(YieldStatement yieldStatement, object data)
		{
			return null;
		}
		
		public override object TrackedVisitCollectionInitializerExpression(CollectionInitializerExpression collectionInitializerExpression, object data)
		{
			return null;
		}
		
		public override object TrackedVisitLambdaExpression(LambdaExpression lambdaExpression, object data)
		{
			return null;
		}

		public override object TrackedVisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			memberReferenceExpression.TargetObject.AcceptVisitor(this, data);
			if ((memberReferenceExpression.TargetObject is ThisReferenceExpression) && !IsProperty(memberReferenceExpression.MemberName)) {
				Append("._");
			} else {
				Append(".");
			}
			Append(memberReferenceExpression.MemberName);
			return null;
		}
		
		public override object TrackedVisitQueryExpression(QueryExpression queryExpression, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionFromClause(QueryExpressionFromClause queryExpressionFromClause, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionGroupClause(QueryExpressionGroupClause queryExpressionGroupClause, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionJoinClause(QueryExpressionJoinClause queryExpressionJoinClause, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionLetClause(QueryExpressionLetClause queryExpressionLetClause, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionOrderClause(QueryExpressionOrderClause queryExpressionOrderClause, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionOrdering(QueryExpressionOrdering queryExpressionOrdering, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionSelectClause(QueryExpressionSelectClause queryExpressionSelectClause, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionWhereClause(QueryExpressionWhereClause queryExpressionWhereClause, object data)
		{
			return null;
		}
		
		public override object TrackedVisitExpressionRangeVariable(ExpressionRangeVariable expressionRangeVariable, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionAggregateClause(QueryExpressionAggregateClause queryExpressionAggregateClause, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionDistinctClause(QueryExpressionDistinctClause queryExpressionDistinctClause, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionGroupJoinVBClause(QueryExpressionGroupJoinVBClause queryExpressionGroupJoinVBClause, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionGroupVBClause(QueryExpressionGroupVBClause queryExpressionGroupVBClause, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionJoinConditionVB(QueryExpressionJoinConditionVB queryExpressionJoinConditionVB, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionJoinVBClause(QueryExpressionJoinVBClause queryExpressionJoinVBClause, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionLetVBClause(QueryExpressionLetVBClause queryExpressionLetVBClause, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionPartitionVBClause(QueryExpressionPartitionVBClause queryExpressionPartitionVBClause, object data)
		{
			return null;
		}
		
		public override object TrackedVisitQueryExpressionSelectVBClause(QueryExpressionSelectVBClause queryExpressionSelectVBClause, object data)
		{
			return null;
		}
		
		/// <summary>
		/// Appends any comments that appear before this node.
		/// </summary>
		protected override void BeginVisit(INode node)
		{
			xmlDocComments.Clear();
			currentNode = node;
			specialNodesInserter.AcceptNodeStart(node);
		}
		
		#region IOutputFormatter
		
		int IOutputFormatter.IndentationLevel {
			get { return codeBuilder.Indent; }
			set { ; }
		}
		
		string IOutputFormatter.Text {
			get { return String.Empty; }
		}
		
		bool IOutputFormatter.IsInMemberBody {
			get { return false; }
			set { ; }
		}
		
		void IOutputFormatter.NewLine()
		{
		}
		
		void IOutputFormatter.Indent()
		{
			
		}
		
		void IOutputFormatter.PrintComment(Comment comment, bool forceWriteInPreviousBlock)
		{
			if (comment.CommentType == CommentType.SingleLine) {
				AppendSingleLineComment(comment);
			} else if (comment.CommentType == CommentType.Block) {
				AppendMultilineComment(comment);
			} else if (comment.CommentType == CommentType.Documentation) {
				if (SupportsDocstring(currentNode)) {
					xmlDocComments.Add(comment);
				} else {
					AppendSingleLineComment(comment);
				}
			}
		}
		
		void IOutputFormatter.PrintPreprocessingDirective(PreprocessingDirective directive, bool forceWriteInPreviousBlock)
		{
		}
		
		void IOutputFormatter.PrintBlankLine(bool forceWriteInPreviousBlock)
		{
		}
		
		#endregion
		
		/// <summary>
		/// Checks that the field declaration has an initializer that
		/// sets an initial value.
		/// </summary>
		static bool FieldHasInitialValue(VariableDeclaration variableDeclaration)
		{
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
			MemberReferenceExpression memberRefExpression = foreachStatement.Expression as MemberReferenceExpression;
			if (identifierExpression != null) {
				Append(identifierExpression.Identifier);
			} else if (invocationExpression != null) {
				invocationExpression.AcceptVisitor(this, null);
			} else if (memberRefExpression != null) {
				memberRefExpression.AcceptVisitor(this, null);
			}
		}
		
		/// <summary>
		/// Determines whether the identifier refers to a field in the
		/// current class.
		/// </summary>
		bool IsField(string name)
		{
			// Check the current method's parameters.
			if (IsMethodParameter(name)) {
				return false;
			}

			// Check the current class's fields.
			if (constructorInfo != null) {
				foreach (FieldDeclaration field in constructorInfo.Fields) {
					foreach (VariableDeclaration variable in field.Fields) {
						if (variable.Name == name) {
							return true;
						}
					}
				}
			}
			return false;
		}
		
		bool IsMethodParameter(string name)
		{
			foreach (ParameterDeclarationExpression param in methodParameters) {
				if (param.ParameterName == name) {
					return true;
				}
			}
			return false;
		}
		
		bool IsProperty(string name)
		{
			return propertyNames.Contains(name);
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
			MemberReferenceExpression memberRefExpression = eventHandlerExpression as MemberReferenceExpression;
			if (identifierExpression != null) {
				Append("self." + identifierExpression.Identifier);
			} else if (memberRefExpression != null) {
				memberRefExpression.AcceptVisitor(this, null);
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
			codeBuilder.AppendIndented(code);
		}
		
		void AppendIndentedPassStatement()
		{
			AppendIndentedLine("pass");
		}
		
		void AppendIndentedLine(string code)
		{
			codeBuilder.AppendIndentedLine(code);
		}
		
		void AppendLine()
		{
			codeBuilder.AppendLine();
		}
		
		void IncreaseIndent()
		{
			codeBuilder.IncreaseIndent();
		}
		
		void DecreaseIndent()
		{
			codeBuilder.DecreaseIndent();
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
			AppendLine();
			
			// Add fields at start of constructor.
			IncreaseIndent();
			AppendDocstring(xmlDocComments);
			if (constructorInfo.Fields.Count > 0) {
				foreach (FieldDeclaration field in constructorInfo.Fields) {
					CreateFieldInitialization(field);
				}
			}
			
			if (!IsEmptyConstructor(constructorInfo.Constructor)) {
				constructorInfo.Constructor.Body.AcceptVisitor(this, null);
				AppendLine();
			} else if (constructorInfo.Fields.Count == 0) {
				AppendIndentedPassStatement();
			} else {
				AppendLine();
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
			foreach (VariableDeclaration variable in field.Fields) {				
				// Ignore field if it has no initializer.
				if (FieldHasInitialValue(variable)) {
					AddTypeToArrayInitializerIfMissing(variable);
					
					string oldVariableName = variable.Name;
					variable.Name = "self._" + variable.Name;
					VisitVariableDeclaration(variable, null);
					variable.Name = oldVariableName;
				}
			}
		}
		
		void AddTypeToArrayInitializerIfMissing(VariableDeclaration variable)
		{
			ArrayCreateExpression arrayCreate = variable.Initializer as ArrayCreateExpression;
			if (IsArrayMissingTypeToCreate(arrayCreate)) {
				arrayCreate.CreateType = variable.TypeReference;
			}
		}
		
		bool IsArrayMissingTypeToCreate(ArrayCreateExpression arrayCreate)
		{
			if (arrayCreate != null) {
				return String.IsNullOrEmpty(arrayCreate.CreateType.Type);
			}
			return false;
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
			AppendLine();
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
		/// Creates the statements inside a switch case statement.
		/// </summary>
		void CreateSwitchCaseBody(SwitchSection section)
		{
			int statementsAdded = 0;
			foreach (INode node in section.Children) {
				if (node is BreakStatement) {
					// ignore.
				} else {
					statementsAdded++;
					node.AcceptVisitor(this, null);
				}
			}

			// Check for empty body.
			if (statementsAdded == 0) {
				AppendIndentedLine("pass");
			}
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
					Append("Array[" + GetTypeName(typeRef) + "]");
				} else {
					Append(GetTypeName(typeRef));
				}
			}
			Append("]");
		}
		
		/// <summary>
		/// If the type is String or Int32 then it returns "str" and "int".
		/// </summary>
		/// <remarks>If the type is a keyword (e.g. uint) then the TypeRef.Type returns
		/// the full type name. It returns the short type name if the type is not a keyword. So
		/// this method will strip the namespace from the name.
		/// </remarks>
		string GetTypeName(TypeReference typeRef)
		{
			string name = typeRef.Type;
			if (name == typeof(String).FullName) {
				return "str";
			} else if ((name == typeof(int).FullName) || ((name == typeof(int).Name))) {
				return "int";
			} else if (typeRef.IsKeyword) {
				// Remove namespace from type name.
				int index = name.LastIndexOf('.');
				if (index > 0) {
					return name.Substring(index + 1);
				}
			}
			return name;
		}
		
		/// <summary>
		/// Gets the type name that defines the method.
		/// </summary>
		static string GetTypeName(MethodDeclaration methodDeclaration)
		{
			TypeDeclaration type = methodDeclaration.Parent as TypeDeclaration;
			return type.Name;
		}
		
		void AppendMultilineComment(Comment comment)
		{
			string[] lines = comment.CommentText.Split(new char[] {'\n'});
			for (int i = 0; i < lines.Length; ++i) {
				string line = "# " + lines[i].Trim();
				if ((i == 0) && !comment.CommentStartsLine) {
					codeBuilder.AppendToPreviousLine(" " + line);
				} else {
					AppendIndentedLine(line);
				}
			}
		}

		void AppendSingleLineComment(Comment comment)
		{
			if (comment.CommentStartsLine) {
				codeBuilder.AppendIndentedLine("#" + comment.CommentText);
			} else {
				codeBuilder.AppendToPreviousLine(" #" + comment.CommentText);
			}
		}
		
		void AppendDocstring(List<Comment> xmlDocComments)
		{
			if (xmlDocComments.Count > 1) {
				// Multiline docstring.
				for (int i = 0; i < xmlDocComments.Count; ++i) {
					string line = xmlDocComments[i].CommentText;
					if (i == 0) {
						AppendIndented(Docstring);
					} else {
						AppendIndented(String.Empty);
					}
					Append(line);
					AppendLine();
				}
				AppendIndentedLine(Docstring);
			} else if (xmlDocComments.Count == 1) {
				// Single line docstring.
				AppendIndentedLine(Docstring + xmlDocComments[0].CommentText + Docstring);
			}
		}
		
		/// <summary>
		/// Returns true if the node is a type declaration or a method since these can have
		/// python docstrings.
		/// </summary>
		bool SupportsDocstring(INode node)
		{
			return (node is TypeDeclaration) || (node is MethodDeclaration) || (node is ConstructorDeclaration);
		}
		
		void AppendPropertyDecorator(PropertyDeclaration propertyDeclaration)
		{
			string propertyName = propertyDeclaration.Name;
			AppendIndented(propertyName);
			Append(" = property(");
			
			bool addedParameter = false;
			if (propertyDeclaration.HasGetRegion) {
				Append("fget=get_" + propertyName);
				addedParameter = true;
			}
			
			if (propertyDeclaration.HasSetRegion) {
				if (addedParameter) {
					Append(", ");
				}
				Append("fset=set_" + propertyName);
			}
			Append(")");
			AppendLine();
		}
		
		void AppendBaseTypes(List<TypeReference> baseTypes)
		{
			Append("(");
			if (baseTypes.Count == 0) {
				Append("object");
			} else {
				for (int i = 0; i < baseTypes.Count; ++i) {
					TypeReference typeRef = baseTypes[i];
					if (i > 0) {
						Append(", ");
					}
					Append(GetTypeName(typeRef));
				}
			}
			Append("):");
		}
	}
}
