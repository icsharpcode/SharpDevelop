// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
		bool methodHasOutputParameters = false;

		// Holds the names of any parameters defined for this class.
		List<string> propertyNames = new List<string>();

		SupportedLanguage language;
		List<MethodDeclaration> entryPointMethods;

		SpecialNodesInserter specialNodesInserter;
		INode currentNode;
		List<Comment> xmlDocComments = new List<Comment>();

		// helpers for managing current types
		bool clrImported = false;
		TypeDeclaration currentType;
		List<string> importLibraries = new List<string>();
		Dictionary<string, string> typeNameMap = new Dictionary<string, string>();

		// flag for accumulating statements which should generated before and 
		// after the current statement
		bool accumulatePostfixStatements = false;
		List<Expression> postfixStatements = new List<Expression>();
		PrefixExpressionExtractor prefixExpressionExtractor = new PrefixExpressionExtractor();

		// flag during yield statement to suppress child return statements
		private bool suppressReturn = false;

		// flag during case statement to suppress child break statements
		private bool suppressBreak = false;

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

			clrImported = false;
			importLibraries = new List<string>();
			typeNameMap = new Dictionary<string, string>();

			// Convert to Python code.
			entryPointMethods = new List<MethodDeclaration>();
			codeBuilder = new PythonCodeBuilder();
			codeBuilder.IndentString = indentString;
			InsertStandardImports();
			unit.AcceptVisitor(this, null);

			return codeBuilder.ToString().Trim();
		}

		/// <summary>
		/// Standard imports to maximize code compatibility
		/// </summary>
		private void InsertStandardImports()
		{
			codeBuilder.AppendIndentedLine("import System");

			typeNameMap.Add("System.Int32", "int");
			typeNameMap.Add("System.String", "str");
			typeNameMap.Add("System.Object", "object");
			typeNameMap.Add("Int32", "int");
			typeNameMap.Add("String", "str");
			typeNameMap.Add("Object", "object");
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
			bool isReference = accumulatePostfixStatements && !(anonymousMethodExpression.Parent is Statement);
			if (!isReference)
				Append("def ");

			// handle known special cases.  Namely variable initialize
			if (anonymousMethodExpression.Parent is ObjectCreateExpression) {
				// Use the CreateType type to generate the anonymous method name
				var objectCreationExpr = anonymousMethodExpression.Parent as ObjectCreateExpression;
				var match = Regex.Match(GetTypeName(objectCreationExpr.CreateType), @"\.?(\w+)$", RegexOptions.Singleline);
				string methodName = (match.Success) ? match.Groups[0].ToString()
					: Regex.Replace(GetTypeName(objectCreationExpr.CreateType), @"\W", "");
				if (methodName.StartsWith("System")) methodName = methodName.Substring(6);
				methodName += "_" + anonymousMethodExpression.StartLocation.Line.ToString()
									+ "_" + anonymousMethodExpression.StartLocation.Column.ToString();
				Append(methodName);
			} else if (anonymousMethodExpression.Parent is VariableDeclaration) {
				// this will still generate a "func = func" statement after definition but is harmless in python
				var varDecl = anonymousMethodExpression.Parent as VariableDeclaration;
				Append(varDecl.Name);
				//varDecl.Name.AcceptVisitor(this, data);
			} else {
				string methodName = "anonMethod_" + anonymousMethodExpression.StartLocation.Line.ToString()
									+ "_" + anonymousMethodExpression.StartLocation.Column.ToString();
				Append(methodName);
			}

			// method name reference written during normal passes
			//   definition written in prefix pass
			if (isReference)
				return null;

			// Add the parameters.
			Append("(");
			for (int i = 0; i < anonymousMethodExpression.Parameters.Count; ++i) {
				if (i > 0) {
					Append(", ");
				}
				Append(anonymousMethodExpression.Parameters[i].ParameterName);
			}
			Append("):");

			// merge parameters with parent method with anonymous due to closures
			//   push old parameters on stack while we write out the inline method
			var oldMethodParameters = methodParameters;
			methodParameters = new List<ParameterDeclarationExpression>(anonymousMethodExpression.Parameters);
			methodParameters.AddRange(methodParameters);
			AppendLine();

			IncreaseIndent();
			AppendDocstring(xmlDocComments);
			if (anonymousMethodExpression.Body.Children.Count > 0) {
				anonymousMethodExpression.Body.AcceptVisitor(this, data);
			} else {
				AppendIndentedPassStatement();
			}
			DecreaseIndent();
			methodParameters = oldMethodParameters;
			return null;
		}

		public override object TrackedVisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			string arrayType = GetTypeName(arrayCreateExpression.CreateType);
			if (arrayCreateExpression.ArrayInitializer.CreateExpressions.Count == 0) {
				Append(GetTypeName("System.Array") + ".CreateInstance(" + arrayType);
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
				Append(GetTypeName("System.Array") + "[" + arrayType + "]");

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
					if (assignmentExpression.Right is InvocationExpression) {
						return CreateInvocationAssignment(assignmentExpression, data);
					}
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
			if (suppressBreak)
				AppendIndentedLine("pass #break"); // use pass as noop
			else
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
			directionExpression.Expression.AcceptVisitor(this, data);
			return null;
		}

		public override object TrackedVisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			AppendIndented("while ");
			if (doLoopStatement.Condition.IsNull)
				Append("True");
			else
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

		/// <summary>
		/// Convert `foreach (var x in items)` to `for x in items:` 
		/// </summary>
		/// <param name="foreachStatement"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public override object TrackedVisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			//  while statement does not work with lambda expressions 
			//  and is more pythonic
			AppendIndented(String.Empty);
			Append("for ");
			Append(foreachStatement.VariableName);
			Append(" in ");
			AppendForeachVariableName(foreachStatement);
			Append(":");
			AppendLine();
			IncreaseIndent();

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
		///     for i in xrange(0,5,1):
		/// or
		///     i = 0
		///     while i &lt; 5:
		/// </summary>
		public override object TrackedVisitForStatement(ForStatement forStatement, object data)
		{
			// Check if forStatement can be represented as simple range
			if (!TryWriteSimpleForStatement(forStatement, data)) {
				// Convert the for loop's initializers.
				foreach (Statement statement in forStatement.Initializers) {
					statement.AcceptVisitor(this, data);
				}

				// Convert the for loop's test expression.
				AppendIndented("while ");
				if (forStatement.Condition.IsNull)
					Append("True");
				else
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
			}
			return null;
		}

		/// <summary>
		/// Handle the fairly common integer increment loop
		/// </summary>
		/// <remarks>The complexity of this method is to guarantee that 
		/// it is only used on integer based variables and that we know that 
		/// its one of the common loop conditions</remarks>
		/// <param name="forStatement">Statement to handle</param>
		/// <param name="data">Additional value for visitor</param>
		/// <returns>Returns true if the simple for statement was written</returns>
		private bool TryWriteSimpleForStatement(ForStatement forStatement, object data)
		{
			if (forStatement.Initializers.Count != 1
				|| forStatement.Iterator.Count != 1
				|| forStatement.Condition.IsNull
				) {
				return false;
			}
			string variableName = null;
			Expression variableExpression = null;
			Expression startExpression = null;
			Expression endExpression = null;
			object stepExpression = null;
			bool negateStepExpression = false;

			// Locate the initializer variable and determine if xrange applies
			var localVariableDecl = forStatement.Initializers[0] as LocalVariableDeclaration;
			var initializerStatement = forStatement.Initializers[0] as ExpressionStatement;
			if (localVariableDecl != null && localVariableDecl.Variables.Count == 1) {
				// validate the data type is an integer
				var variableDecl = localVariableDecl.Variables[0] as VariableDeclaration;
				if (!IsIntegerValue(localVariableDecl.TypeReference))
					return false;
				variableName = variableDecl.Name;
				startExpression = variableDecl.Initializer;
			} else if (initializerStatement != null) {
				var initializerExpression = initializerStatement.Expression as AssignmentExpression;
				if (initializerExpression == null || initializerExpression.Op != AssignmentOperatorType.Assign)
					return false;
				var initializerVariable = initializerExpression.Left as IdentifierExpression;
				if (initializerVariable == null)
					return false;
				variableName = initializerVariable.Identifier;
				variableExpression = initializerVariable;
				if (initializerExpression.Right is PrimitiveExpression) {
					var primativeExpr = initializerExpression.Right as PrimitiveExpression;
					if (!IsIntegerValue(primativeExpr.Value.GetType())) return false;
				}
				startExpression = initializerExpression.Right;
			} else {
				return false;
			}
			// get the step size from the iterator statement and determine increment direction
			var iteratorStatement = forStatement.Iterator[0] as ExpressionStatement;
			if (iteratorStatement == null)
				return false;
			var iteratorExpression = iteratorStatement.Expression as UnaryOperatorExpression;
			if (iteratorExpression != null) {
				switch (iteratorExpression.Op) {
					case UnaryOperatorType.Increment:
					case UnaryOperatorType.PostIncrement:
						stepExpression = "1";
						break;
					case UnaryOperatorType.Decrement:
					case UnaryOperatorType.PostDecrement:
						stepExpression = "1";
						negateStepExpression = true;
						break;
					default:
						return false;
				}
				var iteratorVariable = iteratorExpression.Expression as IdentifierExpression;
				if (iteratorVariable == null || iteratorVariable.Identifier != variableName)
					return false;
			} else {
				var stepAssignmentExpression = iteratorStatement.Expression as AssignmentExpression;
				if (stepAssignmentExpression == null ||
					(stepAssignmentExpression.Op != AssignmentOperatorType.Add &&
					 stepAssignmentExpression.Op != AssignmentOperatorType.Subtract &&
					 stepAssignmentExpression.Op != AssignmentOperatorType.Assign))
					return false;
				negateStepExpression = stepAssignmentExpression.Op == AssignmentOperatorType.Subtract;

				var iteratorVariable = stepAssignmentExpression.Left as IdentifierExpression;
				if (iteratorVariable == null || iteratorVariable.Identifier != variableName)
					return false;
				if (stepAssignmentExpression.Right is PrimitiveExpression
					|| stepAssignmentExpression.Right is IdentifierExpression
					|| stepAssignmentExpression.Right is MemberReferenceExpression
					) {
					// handles += and -=
					stepExpression = stepAssignmentExpression.Right;
				} else if (stepAssignmentExpression.Op == AssignmentOperatorType.Assign
					  && stepAssignmentExpression.Right is BinaryOperatorExpression
					  ) {
					// Handle i = i +/- step
					var stepRightSubCondition = stepAssignmentExpression.Right as BinaryOperatorExpression;
					if ((stepRightSubCondition.Op != BinaryOperatorType.Add &&
						 stepRightSubCondition.Op != BinaryOperatorType.Subtract)
						) {
						return false;
					}
					var stepLastVariable = stepRightSubCondition.Left as IdentifierExpression;
					if (stepLastVariable == null || stepLastVariable.Identifier != variableName) {
						return false;
					}
					negateStepExpression = (stepRightSubCondition.Op == BinaryOperatorType.Subtract);
					if (!(stepRightSubCondition.Right is PrimitiveExpression
						|| stepRightSubCondition.Right is IdentifierExpression
						|| stepRightSubCondition.Right is MemberReferenceExpression
						)) {
						return false;
					}
					stepExpression = stepRightSubCondition.Right;

				} else {
					return false;
				}
			}

			// determine the max value from the condition test
			var condition = forStatement.Condition as BinaryOperatorExpression;
			if (condition == null)
				return false;

			// verify simple inequality and step direct is correct
			if (!((condition.Op == BinaryOperatorType.GreaterThan && negateStepExpression)
				  || (condition.Op == BinaryOperatorType.LessThan && !negateStepExpression)))
				return false;

			var compareVariable = condition.Left as IdentifierExpression;
			if (compareVariable == null || compareVariable.Identifier != variableName)
				return false;
			endExpression = condition.Right;

			// Write out the for loop
			AppendIndented("for ");
			if (variableExpression != null)
				variableExpression.AcceptVisitor(this, null);
			else
				Append(variableName);
			Append(" in xrange(");
			startExpression.AcceptVisitor(this, null);
			Append(",");
			endExpression.AcceptVisitor(this, null);
			Append(",");
			if (negateStepExpression) Append("-");
			if (stepExpression is string) {
				Append(stepExpression.ToString());
			} else {
				((Expression)stepExpression).AcceptVisitor(this, null);
			}
			Append("):");
			AppendLine();

			// Visit the for loop's body.
			IncreaseIndent();
			forStatement.EmbeddedStatement.AcceptVisitor(this, data);
			DecreaseIndent();
			return true;
		}

		/// <summary>
		/// Test if the TypeReference is an integer type
		/// </summary>
		/// <param name="typeReference">TypeReference to test</param>
		/// <returns>True if the type is an integer type</returns>
		private static bool IsIntegerValue(TypeReference typeReference)
		{
			if (typeReference.IsArrayType)
				return false;
			if (!typeReference.Type.StartsWith("System."))
				return false;
			return IsIntegerValue(Type.GetType(typeReference.Type, false, true));
		}

		/// <summary>
		/// Test if the Type is an integer type
		/// </summary>
		/// <param name="type">Type to test</param>
		/// <returns>True if the type is an integer type</returns>
		private static bool IsIntegerValue(Type type)
		{
			if (type == null || !type.IsValueType || !type.IsPrimitive || type.IsArray || type.IsGenericType)
				return false;
			TypeCode code = Type.GetTypeCode(type);
			switch (code) {
				case TypeCode.Byte:
				case TypeCode.Char:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					break;
				default:
					return false;
			}
			return true;
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
				AppendFieldReferenceName(name);
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

		private object CreateInvocationAssignment(AssignmentExpression assignmentExpression, object data)
		{
			assignmentExpression.Left.AcceptVisitor(this, data);
			var invocationExpression = assignmentExpression.Right as InvocationExpression;
			if (invocationExpression == null) {
				Append(" = ");
				assignmentExpression.Right.AcceptVisitor(this, data);
				return null;
			}

			CreateInvocationAssignmentParameters(invocationExpression, data, false);
			invocationExpression.AcceptVisitor(this, data);
			return null;
		}

		private void CreateInvocationAssignmentParameters(InvocationExpression invocationExpression, object data, bool firstArg)
		{
			// out and ref values are passed via return tuple
			//   should verify that the objects are not clr.Reference
			foreach (var param in invocationExpression.Arguments) {
				var directionExpression = param as DirectionExpression;
				if (directionExpression == null) continue;
				if (directionExpression.FieldDirection == FieldDirection.Ref ||
					directionExpression.FieldDirection == FieldDirection.Out) {
					if (!firstArg) Append(", ");
					directionExpression.AcceptVisitor(this, data);
					firstArg = false;
				}
			}
			Append(" = ");
		}

		private bool HasReferences(IEnumerable<Expression> parameters)
		{
			foreach (var param in parameters) {
				var expression = param as DirectionExpression;
				if (expression == null) continue;
				if (expression.FieldDirection == FieldDirection.Out || expression.FieldDirection == FieldDirection.Ref)
					return true;
			}
			return false;
		}

		public override object TrackedVisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			// Special case for searching if single line statement with 
			if (HasReferences(invocationExpression.Arguments)) {
				var expression = invocationExpression.Parent as ExpressionStatement;
				if (expression != null && expression.Parent is BlockStatement) {
					CreateInvocationAssignmentParameters(invocationExpression, data, true);
				}
			}

			MemberReferenceExpression memberRefExpression = invocationExpression.TargetObject as MemberReferenceExpression;
			IdentifierExpression identifierExpression = invocationExpression.TargetObject as IdentifierExpression;
			if (memberRefExpression != null) {
				memberRefExpression.TargetObject.AcceptVisitor(this, data);
				Append("." + memberRefExpression.MemberName);
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
				if (param is DirectionExpression) {
					if (((DirectionExpression)param).FieldDirection == FieldDirection.Out)
						continue;
				}
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
			AppendIndented(GetTypeName("System.Threading.Monitor"));
			Append(".Enter(");
			lockStatement.LockExpression.AcceptVisitor(this, data);
			Append(")");
			AppendLine();

			AppendIndentedLine("try:");
			IncreaseIndent();
			lockStatement.EmbeddedStatement.AcceptVisitor(this, data);
			DecreaseIndent();

			// Convert finally block.
			AppendIndentedLine("finally:");
			IncreaseIndent();
			AppendIndented(GetTypeName("System.Threading.Monitor"));
			Append(".Exit(");
			lockStatement.LockExpression.AcceptVisitor(this, data);
			Append(")");
			AppendLine();
			DecreaseIndent();
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

			var oldMethodHasOutputParameters = methodHasOutputParameters;
			methodHasOutputParameters = false;

			string methodName = methodDeclaration.Name;
			AppendIndented("def " + methodName);

			// Add the parameters.
			AddParameters(methodDeclaration);
			var oldMethodParameters = methodParameters;
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

			methodParameters = oldMethodParameters;
			methodHasOutputParameters = oldMethodHasOutputParameters;
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
			Append(GetTypeName(objectCreateExpression.CreateType));
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
			Append("(");
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
			} else if (primitiveExpression.Value is Single
				  || primitiveExpression.Value is UInt16
				  || primitiveExpression.Value is UInt32
				  || primitiveExpression.Value is Byte
				  ) // special handling for primatives that do not directly map to Python
			{
				var t = primitiveExpression.Value.GetType();
				Append(string.Format("{0}({1})", t.Name, primitiveExpression.Value));
			} else {
				Append(primitiveExpression.StringValue);
			}
			return null;
		}

		public override object TrackedVisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			string propertyName = propertyDeclaration.Name;
			propertyNames.Add(propertyName);

			bool isAnonymous = (propertyDeclaration.HasGetRegion && propertyDeclaration.GetRegion.Block.IsNull
								&& propertyDeclaration.HasSetRegion && propertyDeclaration.SetRegion.Block.IsNull);

			// Add get statements.
			if (propertyDeclaration.HasGetRegion) {
				AppendIndentedLine("def get_" + propertyName + "(self):");
				IncreaseIndent();
				if (isAnonymous) {
					AppendIndentedLine("return self._" + propertyDeclaration.Name.ToLower()); // maybe do some type casting?
				} else if (propertyDeclaration.GetRegion.Block.IsNull) {
					AppendIndentedLine("pass");
				} else {
					propertyDeclaration.GetRegion.Block.AcceptVisitor(this, data);
				}
				DecreaseIndent();
				AppendLine();
			}

			// Add set statements.
			if (propertyDeclaration.HasSetRegion) {
				AppendIndentedLine("def set_" + propertyName + "(self, value):");
				IncreaseIndent();
				if (isAnonymous) {
					AppendIndentedLine("self._" + propertyDeclaration.Name.ToLower() + " = value"); // maybe do some type casting?
				} else if (propertyDeclaration.GetRegion.Block.IsNull) {
					AppendIndentedLine("pass");
				} else {
					propertyDeclaration.SetRegion.Block.AcceptVisitor(this, data);
				}
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
			// yield statements will generate the correct line start and end
			if (suppressReturn) {
				returnStatement.Expression.AcceptVisitor(this, data);
			} else {
				AppendIndented("return ");

				// python returns out and ref generally as a return tuple
				if (methodHasOutputParameters)
					Append("(");

				returnStatement.Expression.AcceptVisitor(this, data);

				if (methodHasOutputParameters) {
					if (currentMethod != null) {
						foreach (var param in currentMethod.Parameters) {
							if (param.ParamModifier == ParameterModifiers.Out
								|| param.ParamModifier == ParameterModifiers.Ref) {
								Append(", ");
								Append(param.ParameterName);
							}
						}
					}
					Append(")");
				}
				AppendLine();
			}
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
			var oldSuppressBreak = suppressBreak;
			this.suppressBreak = true;
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

			suppressBreak = oldSuppressBreak;
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
				Append(GetTypeName(catchClause.TypeReference));
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

			this.currentType = typeDeclaration;
			if (typeDeclaration.Type == ClassType.Enum) {
				CreateEnumeration(typeDeclaration);
			} else {
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
			}
			this.currentType = null;
			DecreaseIndent();

			return null;
		}

		public override object TrackedVisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			// clr was added to list of standard imports
			if (!clrImported) {
				codeBuilder.InsertIndentedLine("import clr");
				clrImported = true;
			}
			Append("clr.GetClrType(");
			Append(GetTypeName(typeOfExpression.TypeReference));
			Append(")");
			return null;
		}

		public override object TrackedVisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
		{
			Append("isinstance(");
			typeOfIsExpression.Expression.AcceptVisitor(this, data);
			Append(",");
			typeOfIsExpression.TypeReference.AcceptVisitor(this, data);
			Append(")");
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
				// Change i++ or ++i to i += 1
				case UnaryOperatorType.PostIncrement:
					return CreateIncrementStatement(unaryOperatorExpression);
				case UnaryOperatorType.Increment:
					return CreateIncrementStatement(unaryOperatorExpression, prefix: true);
				case UnaryOperatorType.Decrement:
					return CreateDecrementStatement(unaryOperatorExpression, prefix: true);
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
				if (@using.IsAlias) {
					typeNameMap[@using.Alias.Type] = @using.Name;
					var match = Regex.Match(@using.Alias.Type, @"^(?<lib>.*)\.(?<name>\w+)$", RegexOptions.Singleline);
					if (match.Success)
						AppendIndentedLine("from " + match.Groups["lib"] + " import "
							+ match.Groups["name"] + " as " + @using.Name);
					else
						AppendIndentedLine(@using.Name + " = " + @using.Alias.Type);
				} else {
					AppendIndentedLine("from " + @using.Name + " import *");
					importLibraries.Insert(0, @using.Name + "."); // insert in reverse order
				}

			}
			return null;
		}

		public override object TrackedVisitUsingStatement(UsingStatement usingStatement, object data)
		{
			// first check if using is either LocalVariable or Variable assignment and try to get the variable name
			string variableName = null;
			bool skipInitializer = false;
			var localVariableDecl = usingStatement.ResourceAcquisition as LocalVariableDeclaration;
			var initializerStatement = usingStatement.ResourceAcquisition as ExpressionStatement;
			if (localVariableDecl != null && localVariableDecl.Variables.Count == 1) {
				// validate the data type is an integer
				var variableDecl = localVariableDecl.Variables[0];
				variableName = variableDecl.Name;
			} else if (initializerStatement != null) {
				var initializerExpression = initializerStatement.Expression as AssignmentExpression;
				if (initializerExpression != null && initializerExpression.Op == AssignmentOperatorType.Assign) {
					var initializerVariable = initializerExpression.Left as IdentifierExpression;
					if (initializerVariable != null)
						variableName = initializerVariable.Identifier;
				} else if (initializerStatement.Expression is ObjectCreateExpression) {
					var createObjectStatement = initializerStatement.Expression as ObjectCreateExpression;
					// create an anonymous variable placeholder
					variableName = "anon" + createObjectStatement.StartLocation.Line.ToString();
					AppendIndented(variableName);
					Append(" = ");
					createObjectStatement.AcceptVisitor(this, data);
					AppendLine();
					skipInitializer = true;
				}
			}

			if (!skipInitializer)
				usingStatement.ResourceAcquisition.AcceptVisitor(this, data);

			AppendIndentedLine("try:");
			IncreaseIndent();
			if (IsEmptyStatement(usingStatement.EmbeddedStatement))
				AppendIndentedLine("pass");
			else
				usingStatement.EmbeddedStatement.AcceptVisitor(this, data);
			DecreaseIndent();

			// Convert finally block.
			AppendIndentedLine("finally:");
			IncreaseIndent();
			if (!string.IsNullOrEmpty(variableName)) {
				AppendIndented(variableName);
				Append(".Dispose()");
				AppendLine();
			} else {
				AppendIndentedLine("pass");
			}
			DecreaseIndent();
			return null;
		}

		private static bool IsEmptyStatement(Statement statement)
		{
			return statement.IsNull
				   || (statement is BlockStatement && statement.Children.Count == 0);
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
			if (yieldStatement.IsYieldBreak)
				AppendIndentedLine("raise StopIteration");
			else if (yieldStatement.IsYieldReturn) {
				// suppress the return in child statement
				var oldSuppressReturn = suppressReturn;
				suppressReturn = true;
				AppendIndented("yield ");
				yieldStatement.Statement.AcceptVisitor(this, data);
				suppressReturn = oldSuppressReturn;
				AppendLine();
			}
			return null;
		}

		public override object TrackedVisitCollectionInitializerExpression(CollectionInitializerExpression collectionInitializerExpression, object data)
		{
			return null;
		}

		public override object TrackedVisitLambdaExpression(LambdaExpression lambdaExpression, object data)
		{
			if (!lambdaExpression.ExpressionBody.IsNull) // simple lambda  otherwise anonymous method
            {
				Append("lambda ");
				for (int i = 0; i < lambdaExpression.Parameters.Count; ++i) {
					if (i > 0) {
						Append(", ");
					}
					Append(lambdaExpression.Parameters[i].ParameterName);
				}
				Append(": ");

				// merge parameters with parent method with lambda due to closures
				var oldMethodParameters = methodParameters;
				methodParameters = new List<ParameterDeclarationExpression>(lambdaExpression.Parameters);
				methodParameters.AddRange(methodParameters);

				lambdaExpression.ExpressionBody.AcceptVisitor(this, null);

				methodParameters = oldMethodParameters;
			} else if (!lambdaExpression.StatementBody.IsNull) {
				// handle known special cases.  Namely variable initialize
				string methodName = "lambdaMethod_" + lambdaExpression.StartLocation.Line.ToString()
									+ "_" + lambdaExpression.StartLocation.Column.ToString();

				// method name reference written during normal passes
				//   definition written in prefix pass
				if (accumulatePostfixStatements && !(lambdaExpression.Parent is Statement)) {
					Append(methodName);
				} else {
					// merge parameters with parent method with lambda due to closures
					var oldMethodParameters = methodParameters;
					methodParameters = new List<ParameterDeclarationExpression>(lambdaExpression.Parameters);
					methodParameters.AddRange(methodParameters);

					Append("def ");
					Append(methodName);
					Append("(");
					// Add the parameters.
					for (int i = 0; i < lambdaExpression.Parameters.Count; ++i) {
						if (i > 0) {
							Append(", ");
						}
						Append(lambdaExpression.Parameters[i].ParameterName);
					}
					Append("):");
					AppendLine();

					IncreaseIndent();
					AppendDocstring(xmlDocComments);
					lambdaExpression.StatementBody.AcceptVisitor(this, data);
					DecreaseIndent();

					methodParameters = oldMethodParameters;
				}
			}

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

			// Search the Method Statements for prefix/postfix/lambda expressions
			//  and move the prefix/lambda to line before current statement
			//  move the postfix statements to line after current statement
			if (node is MethodDeclaration) {
				accumulatePostfixStatements = true;
			}
			if (accumulatePostfixStatements && (node is Statement) && !(node is BlockStatement)) {
				bool oldAccumulatePostfixStatements = accumulatePostfixStatements;
				accumulatePostfixStatements = false;
				node.AcceptVisitor(prefixExpressionExtractor, null);
				if (prefixExpressionExtractor.Statements.Count > 0) {
					CreatePostfixStatements(prefixExpressionExtractor.Statements);
					prefixExpressionExtractor.Statements.Clear();
				}
				accumulatePostfixStatements = oldAccumulatePostfixStatements;
			}
		}

		protected override void EndVisit(INode node)
		{
			if (node is MethodDeclaration) {
				accumulatePostfixStatements = false;
				if (this.postfixStatements.Count > 0)
					this.postfixStatements.Clear();
			} else if (node is Statement) {
				if (this.postfixStatements.Count > 0) {
					CreatePostfixStatements(this.postfixStatements);
					this.postfixStatements.Clear();
				}
			}
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
		object CreateIncrementStatement(UnaryOperatorExpression unaryOperatorExpression, bool prefix = false)
		{
			return CreateIncrementStatement(unaryOperatorExpression, 1, GetBinaryOperator(BinaryOperatorType.Add), prefix);
		}

		/// <summary>
		/// Converts a post or pre decrement expression to an assign statement.
		/// This converts "i--" and "--i" to "i -= 1" since python
		/// does not support post increment expressions.
		/// </summary>
		object CreateDecrementStatement(UnaryOperatorExpression unaryOperatorExpression, bool prefix = false)
		{
			return CreateIncrementStatement(unaryOperatorExpression, 1, GetBinaryOperator(BinaryOperatorType.Subtract), prefix);
		}

		/// <summary>
		/// Converts a post or pre increment expression to an assign statement.
		/// This converts "i++" and "++i" to "i += 1" since python
		/// does not support post increment expressions.
		/// </summary>
		object CreateIncrementStatement(UnaryOperatorExpression unaryOperatorExpression, int increment, string binaryOperator, bool prefix = false)
		{
			if (accumulatePostfixStatements && !(unaryOperatorExpression.Parent is Statement)) {
				unaryOperatorExpression.Expression.AcceptVisitor(this, null);

				// prefix handled by the PrefixExpressionExtractor
				if (!prefix) postfixStatements.Add(unaryOperatorExpression);
			} else {
				unaryOperatorExpression.Expression.AcceptVisitor(this, null);
				Append(" " + binaryOperator + "= ");
				Append(increment.ToString());
			}
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

		void AppendFieldReferenceName(string name)
		{
			// Check the current class's fields.
			bool nameWritten = false;
			if (constructorInfo != null) {
				foreach (FieldDeclaration field in constructorInfo.Fields) {
					foreach (VariableDeclaration variable in field.Fields) {
						if (variable.Name == name) {
							var fieldDecl = field.TypeReference.Parent as FieldDeclaration;
							if (fieldDecl == null && ((field.Modifier & Modifiers.Const) != 0)) {
								variable.Initializer.AcceptVisitor(this, null);
							} else {
								var typeDecl = fieldDecl.Parent as TypeDeclaration;
								if (typeDecl == null) typeDecl = currentType; // use current type
								if (typeDecl != null) {
									if (field.TypeReference.IsNull || IsStatic(field))
										Append(typeDecl.Name + "." + name);
									else if (IsPrivate(field))
										Append("self._" + name);
									else
										Append("self." + name);  // no underscore for public methods
								}
							}
							nameWritten = true;
						}
					}
				}
			}
			if (!nameWritten) {
				Append("self._" + name);
			}
		}

		bool IsStaticField(string name)
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
							if (field.TypeReference.IsNull || (field.Modifier & (Modifiers.Const | Modifiers.Static)) != 0)
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
			if ((assignmentExpression.Op == AssignmentOperatorType.Add)
				&& (assignmentExpression.Left is MemberReferenceExpression)
				&& ((assignmentExpression.Right is IdentifierExpression)
				  || (assignmentExpression.Right is ObjectCreateExpression)
				  || (assignmentExpression.Right is MemberReferenceExpression)
				  )
				) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// Determines whether the assignment expression is actually an
		/// event handler remove statement.
		/// </summary>
		static bool IsRemoveEventHandler(AssignmentExpression assignmentExpression)
		{
			if ((assignmentExpression.Op == AssignmentOperatorType.Subtract)
				&& (assignmentExpression.Left is MemberReferenceExpression)
				&& ((assignmentExpression.Right is IdentifierExpression)
				  || (assignmentExpression.Right is ObjectCreateExpression)
				  || (assignmentExpression.Right is MemberReferenceExpression)
				  )
				) {
				return true;
			}
			return false;
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
			// handle static fields
			foreach (var field in constructorInfo.Fields) {
				CreateStaticFieldInitialization(field, null);
			}

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
			foreach (var field in constructorInfo.Fields) {
				CreateFieldInitialization(field);
			}
			foreach (var property in constructorInfo.Properties) {
				CreatePropertyInitialization(property);
			}

			if (!IsEmptyConstructor(constructorInfo.Constructor)) {
				constructorInfo.Constructor.Body.AcceptVisitor(this, null);
				AppendLine();
			} else {
				AppendIndentedPassStatement();
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

		void CreateEnumeration(TypeDeclaration typeDeclaration)
		{
			object current = 0;
			foreach (INode node in typeDeclaration.Children) {
				var field = node as FieldDeclaration;
				if (field != null) {
					object last = CreateStaticFieldInitialization(field, current);
					if (last != null) current = last;
				}
			}
			if (current.Equals(0)) {
				AppendIndentedLine("pass");
			}
		}

		/// <summary>
		/// Creates a field initialization statement.
		/// </summary>
		void CreateFieldInitialization(FieldDeclaration field)
		{
			if (field.TypeReference.IsNull || (field.Modifier & (Modifiers.Const | Modifiers.Static)) != 0)
				return;
			foreach (VariableDeclaration variable in field.Fields) {
				// Ignore field if it has no initializer.
				if (FieldHasInitialValue(variable)) {
					AddTypeToArrayInitializerIfMissing(variable);

					string oldVariableName = variable.Name;
					if (IsPrivate(field))
						variable.Name = "self._" + variable.Name;
					else
						variable.Name = "self." + variable.Name;
					VisitVariableDeclaration(variable, null);
					variable.Name = oldVariableName;
				}
			}
		}
		/// <summary>
		/// Creates a field initialization statement.
		/// </summary>
		object CreateStaticFieldInitialization(FieldDeclaration field, object value)
		{
			if (!field.TypeReference.IsNull && (field.Modifier & (Modifiers.Const | Modifiers.Static)) == 0)
				return value;

			foreach (VariableDeclaration variable in field.Fields) {
				// Ignore field if it has no initializer.
				if (FieldHasInitialValue(variable)) {
					AddTypeToArrayInitializerIfMissing(variable);
					VisitVariableDeclaration(variable, null);
					value = variable;
				} else {
					var line = variable.Name + " = ";
					if (variable.TypeReference.IsNull && value != null) {
						var declaration = value as VariableDeclaration;
						if (declaration != null) {
							line += declaration.Name;
							line += " + 1";
						} else {
							line += value.ToString();
						}
						value = variable;
					} else if (variable.TypeReference.IsArrayType || !variable.TypeReference.Type.StartsWith("System.")) {
						line += "None";
					} else {
						Type type = Type.GetType(variable.TypeReference.Type, false, true);
						if (type != null && type.IsValueType) {
							//object default_value = Activator.CreateInstance(type);
							line += GetTypeName(variable.TypeReference) + "()";
							value = variable;
						} else {
							line += "None";
						}
					}
					AppendIndentedLine(line);
				}
			}
			return value;
		}

		bool IsValueType(TypeReference typeRef)
		{
			if (typeRef.IsArrayType)
				return true;
			if (!typeRef.Type.StartsWith("System."))
				return false;
			Type type = Type.GetType(typeRef.Type, false, true);
			return (type != null && type.IsValueType);
		}

		/// <summary>
		/// Creates a property initialization statement.
		/// </summary>
		void CreatePropertyInitialization(PropertyDeclaration property)
		{

			if (property.Initializer == null || property.Initializer.IsNull) {
				AppendIndented("self._" + property.Name.ToLower() + " = ");
				if (property.TypeReference.IsArrayType || !property.TypeReference.Type.StartsWith("System.")) {
					Append("None");
				} else {
					Type type = Type.GetType(property.TypeReference.Type, false, true);
					if (type != null && type.IsValueType) {
						//object default_value = Activator.CreateInstance(type);
						Append(GetTypeName(property.TypeReference));
						Append("()");
					} else {
						Append("None");
					}
				}
				AppendLine();
			} else {
				AppendIndented("self._" + property.Name.ToLower() + " = ");
				property.Initializer.AcceptVisitor(this, null);
				AppendLine();
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
					var param = parameters[i];
					if (param.ParamModifier == ParameterModifiers.Out
						|| param.ParamModifier == ParameterModifiers.Ref) {
						// track both ref and output for return statement
						methodHasOutputParameters = true;
						// drop output parameters from declarations
						if (param.ParamModifier == ParameterModifiers.Out)
							continue;
					}
					if (i > 0) {
						Append(", ");
					}
					Append(param.ParameterName);
				}
			} else {
				if (!IsStatic(method)) {
					Append("self");
				}
			}
			Append("):");
		}

		bool IsStatic(AttributedNode method)
		{
			return method == null || (method.Modifier & (Modifiers.Const | Modifiers.Static)) != 0;
		}

		bool IsPrivate(AttributedNode node)
		{
			return node != null && (((node.Modifier & (Modifiers.Private)) == Modifiers.Private)
				|| ((node.Modifier & (Modifiers.Public | Modifiers.Internal)) == 0));
		}

		/// <summary>
		/// Creates assignments of the form:
		/// i = 1
		/// </summary>
		object CreateSimpleAssignment(AssignmentExpression assignmentExpression, string op, object data)
		{
			if (accumulatePostfixStatements && !(assignmentExpression.Parent is Statement)) {
				assignmentExpression.Left.AcceptVisitor(this, null);
				postfixStatements.Add(assignmentExpression);
			} else {
				assignmentExpression.Left.AcceptVisitor(this, data);
				Append(" " + op + " ");
				assignmentExpression.Right.AcceptVisitor(this, data);
			}
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
					Append(GetTypeName("System.Array") + "[" + GetTypeName(typeRef) + "]");
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

			if (typeNameMap.ContainsKey(name)) {
				return typeNameMap[name];
			}
			foreach (string library in importLibraries) {
				if (name.StartsWith(library))
					return name.Substring(library.Length);
			}
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
		/// Converts name to appropriately scoped version based on imports
		/// </summary>
		/// <remarks>If the type is a keyword (e.g. uint) then the TypeRef.Type returns
		/// the full type name. It returns the short type name if the type is not a keyword. So
		/// this method will strip the namespace from the name.
		/// </remarks>
		private string GetTypeName(string name)
		{
			if (typeNameMap.ContainsKey(name)) {
				return typeNameMap[name];
			}
			foreach (string library in importLibraries) {
				if (name.StartsWith(library))
					return name.Substring(library.Length);
			}
			return name;
		}

		/// <summary>
		/// Gets the type name that defines the method.
		/// </summary>
		string GetTypeName(MethodDeclaration methodDeclaration)
		{
			TypeDeclaration type = methodDeclaration.Parent as TypeDeclaration;
			string name = type.Name;
			if (typeNameMap.ContainsKey(name)) {
				return typeNameMap[name];
			}
			foreach (string library in importLibraries) {
				if (name.StartsWith(library))
					return name.Substring(library.Length);
			}
			return name;
		}

		void AppendMultilineComment(Comment comment)
		{
			string[] lines = comment.CommentText.Split(new char[] { '\n' });
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
					if (IsValueType(typeRef))  // fix for enumerations
						continue;
					if (i > 0) {
						Append(", ");
					}
					Append(GetTypeName(typeRef));
				}
			}
			Append("):");
		}

		/// <summary>
		/// Generate the PostFix increment statements
		/// </summary>
		/// <param name="list">list of statements to write</param>
		private void CreatePostfixStatements(IEnumerable<Expression> list)
		{
			bool oldAccumulatePostfixStatements = accumulatePostfixStatements;
			accumulatePostfixStatements = false;
			foreach (var expression in list) {
				AppendIndented(String.Empty);
				expression.AcceptVisitor(this, null);
				AppendLine();
			}
			accumulatePostfixStatements = oldAccumulatePostfixStatements;
		}

	}
}
