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
using Dom = ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Used to convert VB.NET and C# to Ruby.
	/// </summary>
	public class NRefactoryToRubyConverter : NodeTrackingAstVisitor, IOutputFormatter
	{
		SupportedLanguage language;
		Dom.ParseInformation parseInfo;
		
		string indentString = "\t";
		RubyCodeBuilder codeBuilder;
		
		NamespaceDeclaration currentNamespaceDeclaration;
		TypeDeclaration currentTypeDeclaration;
		MethodDeclaration currentMethod;
		bool addedMscorlibRequire;
		
		// Holds the constructor for the class being converted. This is used to identify class fields.
		RubyConstructorInfo constructorInfo;
		
		// Holds the parameters of the current method. This is used to identify
		// references to fields or parameters.
		List<ParameterDeclarationExpression> methodParameters = new List<ParameterDeclarationExpression>();
		
		// Holds the names of any parameters defined for this class.
		List<string> propertyNames = new List<string>();

		List<MethodDeclaration> entryPointMethods;
		
		SpecialNodesInserter specialNodesInserter;

		public NRefactoryToRubyConverter(SupportedLanguage language, Dom.ParseInformation parseInfo)
		{
			this.language = language;
			this.parseInfo = parseInfo;
		}
		
		public NRefactoryToRubyConverter(SupportedLanguage language)
			: this(language,
			       new Dom.ParseInformation(new Dom.DefaultCompilationUnit(new Dom.DefaultProjectContent())))
		{
		}
		
		/// <summary>
		/// Gets or sets the source language that will be converted to Ruby.
		/// </summary>
		public SupportedLanguage SupportedLanguage {
			get { return language; }
		}
		
		/// <summary>
		/// Creates either C# to Ruby or VB.NET to Ruby converter based on the filename extension that is to be converted.
		/// </summary>
		/// <returns>Null if the file cannot be converted.</returns>
		public static NRefactoryToRubyConverter Create(string fileName, Dom.ParseInformation parseInfo)
		{
			if (CanConvert(fileName)) {
				if (parseInfo == null) {
					throw new ArgumentNullException("parseInfo");
				}
				return new NRefactoryToRubyConverter(GetSupportedLanguage(fileName), parseInfo);
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
		/// Gets or sets the string that will be used to indent the generated Ruby code.
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
		/// Converts the source code to Ruby.
		/// </summary>
		public string Convert(string source)
		{
			return Convert(source, language);
		}
		
		/// <summary>
		/// Converts the source code to Ruby.
		/// </summary>
		public string Convert(string source, SupportedLanguage language)
		{
			// Convert to NRefactory code DOM.
			CompilationUnit unit = GenerateCompilationUnit(source, language);
			
			SpecialOutputVisitor specialOutputVisitor = new SpecialOutputVisitor(this);
			specialNodesInserter = new SpecialNodesInserter(unit.UserData as List<ISpecial>, specialOutputVisitor);
			
			// Convert to Ruby code.
			entryPointMethods = new List<MethodDeclaration>();
			codeBuilder = new RubyCodeBuilder();
			codeBuilder.IndentString = indentString;
			unit.AcceptVisitor(this, null);
			
			return codeBuilder.ToString().Trim();
		}

		/// <summary>
		/// Gets a list of possible entry point methods found when converting the
		/// ruby source code.
		/// </summary>
		public ReadOnlyCollection<MethodDeclaration> EntryPointMethods {
			get { return entryPointMethods.AsReadOnly(); }
		}

		public string GenerateMainMethodCall(MethodDeclaration methodDeclaration)
		{
			StringBuilder code = new StringBuilder();
			code.Append(GetTypeName(methodDeclaration));
			code.Append('.');
			code.Append(methodDeclaration.Name);
			code.Append('(');
			if (methodDeclaration.Parameters.Count > 0) {
				code.Append("nil");
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
				Append("Array[" + arrayType + "].new");
				
				// Add initializers.
				Append("([");
				bool firstItem = true;
				foreach (Expression expression in arrayCreateExpression.ArrayInitializer.CreateExpressions) {
					if (firstItem) {
						firstItem = false;
					} else {
						Append(", ");
					}
					expression.AcceptVisitor(this, data);
				}
				Append("])");
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
						return CreateAddHandlerStatement(assignmentExpression.Left, assignmentExpression.Right);
					}
					return CreateSimpleAssignment(assignmentExpression, "+=", data);
				case AssignmentOperatorType.Subtract:
					if (IsRemoveEventHandler(assignmentExpression)) {
						return CreateRemoveHandlerStatement(assignmentExpression.Left, assignmentExpression.Right);
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
		
		/// <summary>
		/// Converts a base class reference to a this reference.
		/// Ruby has no concept of a direct base class reference so
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
		
		public override object TrackedVisitBreakStatement(BreakStatement breakStatement, object data)
		{
			AppendIndentedLine("break");
			return null;
		}
		
		/// <summary>
		/// A ternary operator expression. The right hand side of the following:
		/// 
		/// string a = test ? "Ape" : "Monkey";
		/// 
		/// In Ruby this gets converted to:
		/// 
		/// a = test : "Ape" : "Monkey"
		/// </summary>
		public override object TrackedVisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{	
			// Add condition.
			conditionalExpression.Condition.AcceptVisitor(this, data);
			Append(" ? ");
			
			// Add true part.
			conditionalExpression.TrueExpression.AcceptVisitor(this, data);
			
			Append(" : ");

			// Add false part.
			conditionalExpression.FalseExpression.AcceptVisitor(this, data);
			return null;
		}
		
		public override object TrackedVisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			CreateConstructor(constructorInfo);
			return null;
		}
		
		public override object TrackedVisitContinueStatement(ContinueStatement continueStatement, object data)
		{
			AppendIndentedLine("next");
			return null;
		}
		
		public override object TrackedVisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			AppendIndentedLine("def finalize()");
			IncreaseIndent();
			destructorDeclaration.Body.AcceptVisitor(this, data);
			DecreaseIndent();
			AppendIndentedLine("end");
			return null;
		}
		
		public override object TrackedVisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			AppendIndented("while ");
			doLoopStatement.Condition.AcceptVisitor(this, data);
			AppendLine();
			
			IncreaseIndent();
			doLoopStatement.EmbeddedStatement.AcceptVisitor(this, data);
			DecreaseIndent();
			
			AppendIndentedLine("end");
			return null;
		}
		
		public override object TrackedVisitElseIfSection(ElseIfSection elseIfSection, object data)
		{
			AppendIndented("elsif ");
			elseIfSection.Condition.AcceptVisitor(this, data);
			Append(" then");
			AppendLine();
			
			// Convert else if body statements.
			IncreaseIndent();
			elseIfSection.EmbeddedStatement.AcceptVisitor(this, data);
			DecreaseIndent();
			
			return null;
		}
		
		public override object TrackedVisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			AppendIndented(String.Empty);
			expressionStatement.Expression.AcceptVisitor(this, data);
			AppendLine();
			return null;
		}
		
		public override object TrackedVisitExitStatement(ExitStatement exitStatement, object data)
		{
			AppendIndentedLine("break");
			return null;
		}
		
		public override object TrackedVisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			return null;
		}
		
		public override object TrackedVisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			// Convert the for loop's initializers.
			AppendIndented(String.Empty);
			CreateInitStatement(foreachStatement);
			AppendLine();

			// Convert the for loop's test expression.
			AppendIndentedLine("while enumerator.MoveNext()");

			// Move the initializer in the foreach loop to the
			// first line of the for loop's body.
			IncreaseIndent();
			AppendIndentedLine(foreachStatement.VariableName + " = enumerator.Current");
			
			// Visit the for loop's body.
			foreachStatement.EmbeddedStatement.AcceptVisitor(this, data);
			DecreaseIndent();
			
			AppendIndentedLine("end");
			
			return null;
		}
		
		public override object TrackedVisitForStatement(ForStatement forStatement, object data)
		{
			// Convert the for loop's initializers.
			foreach (Statement statement in forStatement.Initializers) {
				statement.AcceptVisitor(this, data);
			}
			
			// Convert the for loop's test expression.
			AppendIndented("while ");
			forStatement.Condition.AcceptVisitor(this, data);
			AppendLine();
			
			// Visit the for loop's body.
			IncreaseIndent();
			forStatement.EmbeddedStatement.AcceptVisitor(this, data);
			
			// Convert the for loop's increment statement.
			foreach (Statement statement in forStatement.Iterator) {
				statement.AcceptVisitor(this, data);
			}
			DecreaseIndent();
			AppendIndentedLine("end");
			
			return null;
		}
		
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
			AppendIndentedLine("end");
			
			return null;
		}
		
		public override object TrackedVisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			string name = identifierExpression.Identifier;
			if (IsField(name)) {
				Append("@" + name);
			} else if (IsProperty(name) && !IsMethodParameter(name)) {
				Append("self." + name);
			} else {
				Append(name);
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
		
		public override object TrackedVisitIfElseStatement(IfElseStatement ifElseStatement, object data)
		{
			// Convert condition.
			AppendIndented("if ");
			ifElseStatement.Condition.AcceptVisitor(this, data);
			Append(" then");
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
				AppendIndentedLine("else");
				IncreaseIndent();
				foreach (Statement statement in ifElseStatement.FalseStatement) {
					statement.AcceptVisitor(this, data);
				}
				DecreaseIndent();
			}
			AppendIndentedLine("end");

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

		/// <summary>
		/// The variable declaration is not created if the variable has no initializer.
		/// </summary>
		public override object TrackedVisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			foreach (VariableDeclaration variableDeclaration in localVariableDeclaration.Variables) {
				if (!variableDeclaration.Initializer.IsNull) {
	
					// Create variable declaration.
					AppendIndented(variableDeclaration.Name + " = ");
					
					// Generate the variable initializer.
					AddTypeToArrayInitializerIfMissing(variableDeclaration);
					variableDeclaration.Initializer.AcceptVisitor(this, data);
					AppendLine();
				}
			}
			return null;
		}
		
		public override object TrackedVisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			memberReferenceExpression.TargetObject.AcceptVisitor(this, data);
			if ((memberReferenceExpression.TargetObject is ThisReferenceExpression) && !IsProperty(memberReferenceExpression.MemberName)) {
				Append(".@");
			} else {
				Append(".");
			}
			Append(memberReferenceExpression.MemberName);
			return null;
		}

		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			codeBuilder.AppendLineIfPreviousLineIsEndStatement();
			
			// Add method name.
			currentMethod = methodDeclaration;
			string methodName = methodDeclaration.Name;
			AppendIndented("def ");
			if (IsStatic(methodDeclaration)) {
				Append(currentTypeDeclaration.Name);
				Append(".");
			}
			Append(methodName);
			
			// Add the parameters.
			AddParameters(methodDeclaration);
			methodParameters = methodDeclaration.Parameters;
			AppendLine();
			
			IncreaseIndent();
			if (methodDeclaration.Body.Children.Count > 0) {
				methodDeclaration.Body.AcceptVisitor(this, data);
			}			
			DecreaseIndent();
			AppendIndentedLine("end");
			AppendLine();
			
			if (IsStatic(methodDeclaration)) {
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
		
		public override object TrackedVisitMemberInitializerExpression(MemberInitializerExpression memberInitializerExpression, object data)
		{
			Append(memberInitializerExpression.Name);
			Append(" = ");
			memberInitializerExpression.Expression.AcceptVisitor(this, data);
			return null;
		}
		
		public override object TrackedVisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			currentNamespaceDeclaration = namespaceDeclaration;
			
			codeBuilder.AppendLineIfPreviousLineIsCode();
			AppendIndentedLine("module " + namespaceDeclaration.Name);
			
			IncreaseIndent();
			namespaceDeclaration.AcceptChildren(this, data);
			DecreaseIndent();
			
			AppendIndentedLine("end");
			
			return null;
		}
		
		public override object TrackedVisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			Append(objectCreateExpression.CreateType.Type);
			if (IsGenericType(objectCreateExpression)) {
				AppendGenericTypes(objectCreateExpression);
			}
			Append(".new(");

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
		
		public override object TrackedVisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			Append("(" );
			parenthesizedExpression.Expression.AcceptVisitor(this, data);
			Append(")");
			return null;
		}
		
		public override object TrackedVisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			if (primitiveExpression.Value == null) {
				Append("nil");
			} else if (primitiveExpression.Value is Boolean) {
				Append(primitiveExpression.Value.ToString().ToLowerInvariant());
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
				AppendIndentedLine("def " + propertyName);
				IncreaseIndent();
				propertyDeclaration.GetRegion.Block.AcceptVisitor(this, data);
				DecreaseIndent();
				AppendIndentedLine("end");
				AppendLine();
			}
			
			// Add set statements.
			if (propertyDeclaration.HasSetRegion) {
				AppendIndentedLine("def " + propertyName + "=(value)");
				IncreaseIndent();
				propertyDeclaration.SetRegion.Block.AcceptVisitor(this, data);
				DecreaseIndent();
				AppendIndentedLine("end");
				AppendLine();
			}
			
			return null;
		}
		
		public override object TrackedVisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data)
		{
			return null;
		}
		
		public override object TrackedVisitPropertySetRegion(PropertySetRegion propertySetRegion, object data)
		{
			return null;
		}
		
		public override object TrackedVisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			AppendIndented("return ");
			returnStatement.Expression.AcceptVisitor(this, data);
			AppendLine();
			return null;
		}
		
		public override object TrackedVisitSwitchStatement(SwitchStatement switchStatement, object data)
		{
			AppendIndented("case ");
			switchStatement.SwitchExpression.AcceptVisitor(this, null);
			AppendLine();
			IncreaseIndent();
			
			foreach (SwitchSection section in switchStatement.SwitchSections) {
				CreateSwitchCondition(section);
				
				IncreaseIndent();
				CreateSwitchConditionBody(section);
				DecreaseIndent();
			}
			DecreaseIndent();
			AppendIndentedLine("end");
			return null;
		}
		
		public override object TrackedVisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			Append("self");
			return null;
		}
		
		public override object TrackedVisitThrowStatement(ThrowStatement throwStatement, object data)
		{
			AppendIndented("raise ");
			throwStatement.Expression.AcceptVisitor(this, data);
			AppendLine();
			return null;
		}
		
		public override object TrackedVisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{
			// Convert try-catch body.
			AppendIndentedLine("begin");
			IncreaseIndent();
			tryCatchStatement.StatementBlock.AcceptVisitor(this, data);
			DecreaseIndent();
			
			// Convert catches.
			foreach (CatchClause catchClause in tryCatchStatement.CatchClauses) {
				AppendIndented("rescue ");
				Append(catchClause.TypeReference.Type);
				Append(" => " + catchClause.VariableName);
				AppendLine();
				
				// Convert catch child statements.
				IncreaseIndent();
				catchClause.StatementBlock.AcceptVisitor(this, data);
				DecreaseIndent();
			}
			
			// Convert finally block.
			AppendIndentedLine("ensure");
			IncreaseIndent();
			tryCatchStatement.FinallyBlock.AcceptVisitor(this, data);
			DecreaseIndent();
			AppendIndentedLine("end");
			
			return null;
		}
		
		/// <summary>
		/// Visits a class.
		/// </summary>
		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			currentTypeDeclaration = typeDeclaration;
			
			if (currentNamespaceDeclaration == null) {
				codeBuilder.AppendLineIfPreviousLineIsCode();
			}
			
			AppendIndented("class " + typeDeclaration.Name);
			AppendBaseTypes(typeDeclaration.BaseTypes);
			AppendLine();
			IncreaseIndent();
			if (typeDeclaration.Children.Count > 0) {
				// Look for fields or a constructor for the type.
				constructorInfo = RubyConstructorInfo.GetConstructorInfo(typeDeclaration);
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
			} 
			DecreaseIndent();
			codeBuilder.TrimEnd();
			AppendLine();
			AppendIndentedLine("end");
			return null;
		}
		
		public override object TrackedVisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			codeBuilder.InsertIndentedLine("require \"mscorlib\"\r\n");
			Append(GetTypeName(typeOfExpression.TypeReference));
			Append(".to_clr_type");
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
		
		/// <summary>
		/// Converts using declarations into Ruby require statements.
		/// </summary>
		public override object TrackedVisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			if (!addedMscorlibRequire) {
				AppendIndentedLine("require \"mscorlib\"");
				addedMscorlibRequire = true;
			}
			
			// Add import statements for each using.
			foreach (Using @using in usingDeclaration.Usings) {
				AppendIndentedLine("require \"" + @using.Name + ", Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\"");
			}
			return null;
		}
				
		public override object TrackedVisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			AppendIndented(variableDeclaration.Name + " = ");
			variableDeclaration.Initializer.AcceptVisitor(this, data);
			AppendLine();
			return null;
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
		
		void IncreaseIndent()
		{
			codeBuilder.IncreaseIndent();
		}
		
		void DecreaseIndent()
		{
			codeBuilder.DecreaseIndent();
		}
		
		void Append(string code)
		{
			codeBuilder.Append(code);
		}
		
		void AppendIndented(string code)
		{
			codeBuilder.AppendIndented(code);
		}
		
		void AppendIndentedLine(string code)
		{
			codeBuilder.AppendIndentedLine(code);
		}
		
		void AppendLine()
		{
			codeBuilder.AppendLine();
		}
		
		/// <summary>
		/// Adds the method or constructor parameters.
		/// </summary>
		void AddParameters(ParametrizedNode method)
		{
			Append("(");
			List<ParameterDeclarationExpression> parameters = method.Parameters;
			if (parameters.Count > 0) {
				for (int i = 0; i < parameters.Count; ++i) {
					if (i > 0) {
						Append(", ");
					}
					Append(parameters[i].ParameterName);
				}
			}
			Append(")");
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
		/// Creates an attach statement (i.e. button.Click += ButtonClick)
		/// </summary>
		object CreateAddHandlerStatement(Expression eventExpression, Expression eventHandlerExpression)
		{
			CreateEventReferenceExpression(eventExpression);
			CreateDelegateCreateExpression(eventHandlerExpression);
			return null;
		}
		
		/// <summary>
		/// Creates a remove statement (i.e. button.Click -= ButtonClick)
		/// </summary>
		object CreateRemoveHandlerStatement(Expression eventExpression, Expression eventHandlerExpression)
		{
			CreateEventReferenceExpression(eventExpression);
			Append(".remove(On");
			eventHandlerExpression.AcceptVisitor(this, null);
			Append(")");
			return null;
		}
		
		/// <summary>
		/// Converts an expression to an event reference in Ruby.
		/// (i.e. the "button.Click" part of "button.Click += ButtonClick".
		/// </summary>
		object CreateEventReferenceExpression(Expression eventExpression)
		{
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
				CreateDelegateCreateExpression(identifierExpression);
			} else if (memberRefExpression != null) {
				CreateDelegateCreateExpression(memberRefExpression);
			} else if (objectCreateExpression != null) {
				CreateDelegateCreateExpression(objectCreateExpression.Parameters[0]);
			}
			return null;
		}
		
		void CreateDelegateCreateExpression(IdentifierExpression identifierExpression)
		{
			string parameters = GetParameterNameStringForMethod(identifierExpression.Identifier);
			AppendDelegateCreateExpressionStart(parameters);			
			Append("self." + identifierExpression.Identifier);
			AppendDelegateCreateExpressionEnd(parameters);
		}
		
		void AppendDelegateCreateExpressionStart(string parameters)
		{
			Append(" { ");
			if (parameters.Length > 0) {
				Append(String.Format("|{0}|", parameters));
				Append(" ");
			}
		}
		
		void AppendDelegateCreateExpressionEnd(string parameters)
		{
			Append(String.Format("({0})", parameters));
			Append(" }");
		}
	
		void CreateDelegateCreateExpression(MemberReferenceExpression memberRefExpression)
		{
			string parameters = GetParameterNameStringForMethod(memberRefExpression.MemberName);
			AppendDelegateCreateExpressionStart(parameters);
			memberRefExpression.AcceptVisitor(this, null);
			AppendDelegateCreateExpressionEnd(parameters);
		}
		
		string GetParameterNameStringForMethod(string methodName)
		{
			if (parseInfo.CompilationUnit != null) {
				string className = GetFullyQualifiedCurrentClassName();
				Dom.IClass c = parseInfo.CompilationUnit.ProjectContent.GetClass(className, 0);
				return GetParameterNameStringForMethod(c, methodName);
			}
			return String.Empty;
		}
		
		string GetFullyQualifiedCurrentClassName()
		{
			string namespacePrefix = String.Empty;
			if (currentNamespaceDeclaration != null) {
				namespacePrefix = currentNamespaceDeclaration.Name + ".";
			}
			return namespacePrefix + currentTypeDeclaration.Name;
		}
		
		string GetParameterNameStringForMethod(Dom.IClass c, string methodName)
		{
			if (c != null) {
				foreach (Dom.IMethod method in c.Methods) {
					if (methodName == method.Name) {
						return GetParameterNameStringForMethod(method);
					}
				}
			}
			return String.Empty;
		}
		
		string GetParameterNameStringForMethod(Dom.IMethod method)
		{
			StringBuilder parameters = new StringBuilder();
			for (int i = 0; i < method.Parameters.Count; ++i) {
				Dom.IParameter parameter = method.Parameters[i];
				if (i > 0) {
					parameters.Append(", ");
				}
				parameters.Append(parameter.Name);
			}
			return parameters.ToString();
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
		
		void CreateConstructor(RubyConstructorInfo constructorInfo)
		{
			if (constructorInfo.Constructor != null) {
				AppendIndented("def initialize");
				AddParameters(constructorInfo.Constructor);
				methodParameters = constructorInfo.Constructor.Parameters;
			} else {
				AppendIndented("def initialize()");
			}
			AppendLine();
			
			// Add fields at start of constructor.
			IncreaseIndent();
			if (constructorInfo.Fields.Count > 0) {
				foreach (FieldDeclaration field in constructorInfo.Fields) {
					CreateFieldInitialization(field);
				}
			}
			
			if (!IsEmptyConstructor(constructorInfo.Constructor)) {
				constructorInfo.Constructor.Body.AcceptVisitor(this, null);
			}
			
			DecreaseIndent();
			AppendIndentedLine("end");
			AppendLine();
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
		/// Checks that the field declaration has an initializer that
		/// sets an initial value.
		/// </summary>
		static bool FieldHasInitialValue(VariableDeclaration variableDeclaration)
		{
			Expression initializer = variableDeclaration.Initializer;
			return !initializer.IsNull;
		}
		
		void CreateFieldInitialization(FieldDeclaration field)
		{
			foreach (VariableDeclaration variable in field.Fields) {
				// Ignore field if it has no initializer.
				if (FieldHasInitialValue(variable)) {
					AddTypeToArrayInitializerIfMissing(variable);
					
					string oldVariableName = variable.Name;
					variable.Name = "@" + variable.Name;
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
			return name.Replace(".", "::");
		}
		
		/// <summary>
		/// Gets the type name that defines the method.
		/// </summary>
		static string GetTypeName(MethodDeclaration methodDeclaration)
		{
			TypeDeclaration type = methodDeclaration.Parent as TypeDeclaration;
			return type.Name;
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
		
		bool IsStatic(ParametrizedNode method)
		{
			return (method.Modifier & Modifiers.Static) == Modifiers.Static;
		}
		
		void CreateSwitchCondition(SwitchSection section)
		{
			bool firstLabel = true;
			foreach (CaseLabel label in section.SwitchLabels) {
				if (firstLabel) {
					firstLabel = false;
					if (label.IsDefault) {
						AppendIndented("else");
					} else {
						// Create if condition.
						AppendIndented("when ");
					}
				} else {
					Append(", ");
				}
				label.Label.AcceptVisitor(this, null);
			}
			AppendLine();
		}
				
		/// <summary>
		/// Creates the statements inside a switch case statement.
		/// </summary>
		void CreateSwitchConditionBody(SwitchSection section)
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
		}
		
		/// <summary>
		/// Appends any comments that appear before this node.
		/// </summary>
		protected override void BeginVisit(INode node)
		{
		//	xmlDocComments.Clear();
		//	currentNode = node;
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
			} else if ((comment.CommentType == CommentType.Block) || (comment.CommentType == CommentType.Documentation)) {
				AppendMultilineComment(comment);
			}
		}
				
		void IOutputFormatter.PrintPreprocessingDirective(PreprocessingDirective directive, bool forceWriteInPreviousBlock)
		{
		}
		
		void IOutputFormatter.PrintBlankLine(bool forceWriteInPreviousBlock)
		{
		}
		
		#endregion
		
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
		
		void AppendBaseTypes(List<TypeReference> baseTypes)
		{
			if (baseTypes.Count > 0) {
				Append(" < ");
				for (int i = 0; i < baseTypes.Count; ++i) {
					TypeReference typeRef = baseTypes[i];
					if (i > 0) {
						Append(", ");
					}
					Append(GetTypeName(typeRef));
				}
			}
		}
	}
}
