// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.CodeDom;
using System.Text;
using System.Collections;

using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Parser
{
	public class CodeDOMVisitor : AbstractASTVisitor
	{
		Stack namespaceDeclarations = new Stack();
		Stack typeDeclarations      = new Stack();
		Stack codeStack             = new Stack();
		
		TypeDeclaration currentTypeDeclaration = null;

		// dummy collection used to swallow statements
		System.CodeDom.CodeStatementCollection NullStmtCollection = new CodeStatementCollection();
		
		public CodeCompileUnit codeCompileUnit   = new CodeCompileUnit();
		public ArrayList       namespaces        = new ArrayList();
		public ArrayList       withEventsFields  = new ArrayList();
		
		static string[,] typeConversionList = new string[,] {
			{"System.Void",    "void"},
			{"System.Object",  "object"},
			{"System.Boolean", "bool"},
			{"System.Byte",    "byte"},
			{"System.SByte",   "sbyte"},
			{"System.Char",    "char"},
			{"System.Enum",    "enum"},
			{"System.Int16",   "short"},
			{"System.Int32",   "int"},
			{"System.Int64",   "long"},
			{"System.UInt16",  "ushort"},
			{"System.UInt32",  "uint"},
			{"System.UInt64",  "ulong"},
			{"System.Single",  "float"},
			{"System.Double",  "double"},
			{"System.Decimal", "decimal"},
			{"System.String",  "string"}
		};
		
		static string[,] typeConversionListVB = new string[,] {
			{"System.Object",	"OBJECT"},
			{"System.Boolean",	"BOOLEAN"},
			{"System.Byte",		"BYTE"},
			{"System.Char",		"CHAR"},
			{"System.Int16",	"SHORT"},
			{"System.Int32",	"INTEGER"},
			{"System.Int64",	"LONG"},
			{"System.Single",	"SINGLE"},
			{"System.Double",	"DOUBLE"},
			{"System.Decimal",	"DECIMAL"},
			{"System.String",	"STRING"},
			{"System.DateTime",	"DATE"}
		};
		
		static Hashtable typeConversionTable   = new Hashtable();
		static Hashtable typeConversionTableVB = new Hashtable();
		
		static CodeDOMVisitor()
		{
			for (int i = 0; i < typeConversionList.GetLength(0); ++i) {
				typeConversionTable[typeConversionList[i, 1]] = typeConversionList[i, 0];
			}
			
			for (int i = 0; i < typeConversionListVB.GetLength(0); ++i) {
				typeConversionTableVB[typeConversionListVB[i, 1]] = typeConversionListVB[i, 0];
			}
		}
		
		string ConvType(string type)
		{
			if (type == null) {
				return null;
			}
			
			if (typeConversionTable[type] != null) {
				return typeConversionTable[type].ToString();
			}
			if (typeConversionTableVB[type.ToUpper()] != null) {
				return typeConversionTableVB[type.ToUpper()].ToString();
			}
			return type;
		}

		void AddStmt(System.CodeDom.CodeStatement stmt)
		{
			System.CodeDom.CodeStatementCollection stmtCollection = codeStack.Peek() as System.CodeDom.CodeStatementCollection;
			if (stmtCollection != null) {
				stmtCollection.Add(stmt);
			}
		}

		void AddStmt(System.CodeDom.CodeExpression expr)
		{
			System.CodeDom.CodeStatementCollection stmtCollection = codeStack.Peek() as System.CodeDom.CodeStatementCollection;
			if (stmtCollection != null) {
				stmtCollection.Add(expr);
			}
		}

		// FIXME: map all modifiers correctly
		MemberAttributes ConvMemberAttributes(Modifier modifier)
		{
			MemberAttributes attr = (MemberAttributes)0;

			if ((modifier & Modifier.Abstract) != 0)
				attr |=  MemberAttributes.Abstract;
//			if ((modifier & Modifier.None) != 0)
//				attr |=  MemberAttributes.AccessMask;
			if ((modifier & Modifier.Internal) != 0)
				attr |=  MemberAttributes.Assembly;
			if ((modifier & Modifier.Const) != 0)
				attr |=  MemberAttributes.Const;
			if ((modifier & Modifier.Protected) != 0)
				attr |=  MemberAttributes.Family;
			if ((modifier & Modifier.Protected) != 0 && (modifier & Modifier.Internal) != 0)
				attr |=  MemberAttributes.FamilyAndAssembly;
//			if ((modifier & Modifier.None) != 0)
//				attr |=  MemberAttributes.FamilyOrAssembly;
			if ((modifier & Modifier.Sealed) != 0)
				attr |=  MemberAttributes.Final;
			if ((modifier & Modifier.New) != 0)
				attr |=  MemberAttributes.New;
			if ((modifier & Modifier.Virtual) != 0)
				attr |=  MemberAttributes.Overloaded;
			if ((modifier & Modifier.Override) != 0)
				attr |=  MemberAttributes.Override;
			if ((modifier & Modifier.Private) != 0)
				attr |=  MemberAttributes.Private;
			if ((modifier & Modifier.Public) != 0)
				attr |=  MemberAttributes.Public;
//			if ((modifier & Modifier.None) != 0)
//				attr |=  MemberAttributes.ScopeMask;
			if ((modifier & Modifier.Static) != 0)
				attr |=  MemberAttributes.Static;
//			if ((modifier & Modifier.None) != 0)
//				attr |=  MemberAttributes.VTableMask;

			return attr;
		}

		#region ICSharpCode.SharpRefactory.Parser.IASTVisitor interface implementation
		public override object Visit(CompilationUnit compilationUnit, object data)
		{
			if (compilationUnit == null) {
				throw new ArgumentNullException("compilationUnit");
			}
			CodeNamespace globalNamespace = new CodeNamespace("Global");
			namespaces.Add(globalNamespace);
			namespaceDeclarations.Push(globalNamespace);
			compilationUnit.AcceptChildren(this, data);
			codeCompileUnit.Namespaces.Add(globalNamespace);
			return globalNamespace;
		}
		
		public override object Visit(NamespaceDeclaration namespaceDeclaration, object data)
		{
			CodeNamespace currentNamespace = new CodeNamespace(namespaceDeclaration.Name);
			namespaces.Add(currentNamespace);
			// add imports from mother namespace
			foreach (CodeNamespaceImport import in ((CodeNamespace)namespaceDeclarations.Peek()).Imports) {
				currentNamespace.Imports.Add(import);
			}
			namespaceDeclarations.Push(currentNamespace);
			namespaceDeclaration.AcceptChildren(this, data);
			namespaceDeclarations.Pop();
			codeCompileUnit.Namespaces.Add(currentNamespace);
			
			// TODO : Nested namespaces allowed in CodeDOM ? Doesn't seem so :(
			return null;
		}
		
		public override object Visit(UsingDeclaration usingDeclaration, object data)
		{
			foreach (Using u in usingDeclaration.Usings) {
				((CodeNamespace)namespaceDeclarations.Peek()).Imports.Add(new CodeNamespaceImport(u.Name));
			}
			return null;
		}
		
		
		public override object Visit(AttributeSection attributeSection, object data)
		{
			return null;
		}
		
		public override object Visit(TypeDeclaration typeDeclaration, object data)
		{
			this.currentTypeDeclaration = typeDeclaration;
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(typeDeclaration.Name);
			codeTypeDeclaration.IsClass     = typeDeclaration.Type == ClassType.Class;
			codeTypeDeclaration.IsEnum      = typeDeclaration.Type == ClassType.Enum;
			codeTypeDeclaration.IsInterface = typeDeclaration.Type == ClassType.Interface;
			codeTypeDeclaration.IsStruct    = typeDeclaration.Type == ClassType.Struct;
			
			if (typeDeclaration.BaseTypes != null) {
				foreach (TypeReference typeRef in typeDeclaration.BaseTypes) {
					codeTypeDeclaration.BaseTypes.Add(new CodeTypeReference(typeRef.Type));
				}
			}
			
			typeDeclarations.Push(codeTypeDeclaration);
			typeDeclaration.AcceptChildren(this,data);
//			((INode)typeDeclaration.Children[0]).(this, data);
			
			typeDeclarations.Pop();
			
			((CodeNamespace)namespaceDeclarations.Peek()).Types.Add(codeTypeDeclaration);
			
			return null;
		}
		
		public override object Visit(DelegateDeclaration delegateDeclaration, object data)
		{
//			CodeTypeDelegate codeTypeDelegate = new CodeTypeDelegate(delegateDeclaration.Name);
//			codeTypeDelegate.Parameters
//
//			((CodeNamespace)namespaceDeclarations.Peek()).Types.Add(codeTypeDelegate);
			return null;
		}
		
		public override object Visit(VariableDeclaration variableDeclaration, object data)
		{
			return null;
		}
		
		public override object Visit(FieldDeclaration fieldDeclaration, object data)
		{
			for (int i = 0; i < fieldDeclaration.Fields.Count; ++i) {
				VariableDeclaration field = (VariableDeclaration)fieldDeclaration.Fields[i];
				
				if ((fieldDeclaration.Modifier & Modifier.WithEvents) != 0) {
					this.withEventsFields.Add(field);
				}
				string typeString = fieldDeclaration.GetTypeForField(i).Type;
				
				CodeMemberField memberField = new CodeMemberField(new CodeTypeReference(ConvType(typeString)), field.Name);
				memberField.Attributes = ConvMemberAttributes(fieldDeclaration.Modifier);
				if (!field.Initializer.IsNull) {
					memberField.InitExpression = (CodeExpression)field.Initializer.AcceptVisitor(this, data);
				}
				
				((CodeTypeDeclaration)typeDeclarations.Peek()).Members.Add(memberField);
			}
			
			return null;
		}
		
		public override object Visit(MethodDeclaration methodDeclaration, object data)
		{
			CodeMemberMethod memberMethod = new CodeMemberMethod();
			memberMethod.Name = methodDeclaration.Name;
			memberMethod.Attributes = ConvMemberAttributes(methodDeclaration.Modifier);
			
			codeStack.Push(memberMethod.Statements);

			((CodeTypeDeclaration)typeDeclarations.Peek()).Members.Add(memberMethod);

			// Add Method Parameters
			foreach (ParameterDeclarationExpression parameter in methodDeclaration.Parameters)
			{
				memberMethod.Parameters.Add((CodeParameterDeclarationExpression)Visit(parameter, data));
			}

			methodDeclaration.Body.AcceptChildren(this, data);

			codeStack.Pop();

			return null;
		}
		
		public override object Visit(ConstructorDeclaration constructorDeclaration, object data)
		{
			CodeMemberMethod memberMethod = new CodeConstructor();

			codeStack.Push(memberMethod.Statements);
			((CodeTypeDeclaration)typeDeclarations.Peek()).Members.Add(memberMethod);
			constructorDeclaration.Body.AcceptChildren(this, data);
			codeStack.Pop();

			return null;
		}
		
		public override object Visit(BlockStatement blockStatement, object data)
		{
			blockStatement.AcceptChildren(this, data);
			return null;
		}
		
		public override object Visit(StatementExpression statementExpression, object data)
		{
			object exp = statementExpression.Expression.AcceptVisitor(this, data);
			if (exp is CodeExpression) {
				AddStmt(new CodeExpressionStatement((CodeExpression)exp));
			}
			return exp;
		}
		
		public string Convert(TypeReference typeRef)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(ConvType(typeRef.Type));
			
			for (int i = 0; i < typeRef.PointerNestingLevel; ++i) {
				builder.Append('*');
			}
			
			if (typeRef.RankSpecifier != null) {
				for (int i = 0; i < typeRef.RankSpecifier.Length; ++i) {
					builder.Append('[');
					for (int j = 1; j < typeRef.RankSpecifier[i]; ++j) {
						builder.Append(',');
					}
					builder.Append(']');
				}
			}
			
			return builder.ToString();
		}
		
		public override object Visit(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			CodeVariableDeclarationStatement declStmt = null;

			for (int i = 0; i < localVariableDeclaration.Variables.Count; ++i) {
				CodeTypeReference type = new CodeTypeReference(Convert(localVariableDeclaration.GetTypeForVariable(i)));
				VariableDeclaration var = (VariableDeclaration)localVariableDeclaration.Variables[i];
				if (!var.Initializer.IsNull) {
					declStmt = new CodeVariableDeclarationStatement(type,
					                                                var.Name,
					                                                (CodeExpression)((INode)var.Initializer).AcceptVisitor(this, data));
				} else {
					declStmt = new CodeVariableDeclarationStatement(type,
					                                                var.Name);
				}
			}
			
			AddStmt(declStmt);

			return declStmt;
		}
		
		public override object Visit(EmptyStatement emptyStatement, object data)
		{
			CodeSnippetStatement emptyStmt = new CodeSnippetStatement();

			AddStmt(emptyStmt);

			return emptyStmt;
		}
		
		public override object Visit(ReturnStatement returnStatement, object data)
		{
			CodeMethodReturnStatement returnStmt;
			if (returnStatement.Expression.IsNull)
				returnStmt = new CodeMethodReturnStatement();
			else
				returnStmt = new CodeMethodReturnStatement((CodeExpression)returnStatement.Expression.AcceptVisitor(this,data));

			AddStmt(returnStmt);

			return returnStmt;
		}
		
		public override object Visit(IfElseStatement ifElseStatement, object data)
		{
			CodeConditionStatement ifStmt = new CodeConditionStatement();

			ifStmt.Condition = (CodeExpression)ifElseStatement.Condition.AcceptVisitor(this, data);

			codeStack.Push(ifStmt.TrueStatements);
			foreach (Statement stmt in ifElseStatement.TrueStatement) {
				if (stmt is BlockStatement) {
					stmt.AcceptChildren(this, data);
				} else {
					stmt.AcceptVisitor(this, data);
				}
			}
			codeStack.Pop();

			codeStack.Push(ifStmt.FalseStatements);
			foreach (Statement stmt in ifElseStatement.FalseStatement) {
				if (stmt is BlockStatement) {
					stmt.AcceptChildren(this, data);
				} else {
					stmt.AcceptVisitor(this, data);
				}
			}
			codeStack.Pop();

			AddStmt(ifStmt);

			return ifStmt;
		}
		
		public override object Visit(ForStatement forStatement, object data)
		{
			CodeIterationStatement forLoop = new CodeIterationStatement();
			
			if (forStatement.Initializers.Count > 0)  {
				if (forStatement.Initializers.Count > 1) {
					throw new NotSupportedException("CodeDom does not support Multiple For-Loop Initializer Statements");
				}

				foreach (object o in forStatement.Initializers) {
					if (o is Expression) {
						forLoop.InitStatement = new CodeExpressionStatement((CodeExpression)((Expression)o).AcceptVisitor(this,data));
					}
					if (o is Statement) {
						codeStack.Push(NullStmtCollection);
						forLoop.InitStatement = (CodeStatement)((Statement)o).AcceptVisitor(this, data);
						codeStack.Pop();
					}
				}
			}
			
			if (forStatement.Condition == null) {
				forLoop.TestExpression = new CodePrimitiveExpression(true);
			} else {
				forLoop.TestExpression = (CodeExpression)forStatement.Condition.AcceptVisitor(this, data);
			}

			codeStack.Push(forLoop.Statements);
			forStatement.EmbeddedStatement.AcceptVisitor(this, data);
			codeStack.Pop();

			if (forStatement.Iterator.Count > 0) {
				if (forStatement.Initializers.Count > 1) {
					throw new NotSupportedException("CodeDom does not support Multiple For-Loop Iterator Statements");
				}

				foreach (Statement stmt in forStatement.Iterator) {
					forLoop.IncrementStatement = (CodeStatement)stmt.AcceptVisitor(this, data);
				}
			}

			AddStmt(forLoop);

			return forLoop;
		}
		
		public override object Visit(LabelStatement labelStatement, object data)
		{
			System.CodeDom.CodeLabeledStatement labelStmt = new CodeLabeledStatement(labelStatement.Label,(CodeStatement)labelStatement.AcceptVisitor(this, data));

			// Add Statement to Current Statement Collection
			AddStmt(labelStmt);

			return labelStmt;
		}
		
		public override object Visit(GotoStatement gotoStatement, object data)
		{
			System.CodeDom.CodeGotoStatement gotoStmt = new CodeGotoStatement(gotoStatement.Label);

			// Add Statement to Current Statement Collection
			AddStmt(gotoStmt);

			return gotoStmt;
		}
		
		public override object Visit(SwitchStatement switchStatement, object data)
		{
			throw new NotSupportedException("CodeDom does not support Switch Statement");
		}
		
		public override object Visit(TryCatchStatement tryCatchStatement, object data)
		{
			// add a try-catch-finally
			CodeTryCatchFinallyStatement tryStmt = new CodeTryCatchFinallyStatement();

			codeStack.Push(tryStmt.TryStatements);
			
			tryCatchStatement.StatementBlock.AcceptChildren(this, data);
			codeStack.Pop();

			if (!tryCatchStatement.FinallyBlock.IsNull) {
				codeStack.Push(tryStmt.FinallyStatements);
				
				tryCatchStatement.FinallyBlock.AcceptChildren(this,data);
				codeStack.Pop();
			}

			foreach (CatchClause clause in tryCatchStatement.CatchClauses)
			{
				CodeCatchClause catchClause = new CodeCatchClause(clause.VariableName);
				catchClause.CatchExceptionType = new CodeTypeReference(clause.TypeReference.Type);
				tryStmt.CatchClauses.Add(catchClause);

				codeStack.Push(catchClause.Statements);
				
				clause.StatementBlock.AcceptChildren(this, data);
				codeStack.Pop();
			}

			// Add Statement to Current Statement Collection
			AddStmt(tryStmt);

			return tryStmt;
		}
		
		public override object Visit(ThrowStatement throwStatement, object data)
		{
			CodeThrowExceptionStatement throwStmt = new CodeThrowExceptionStatement((CodeExpression)throwStatement.Expression.AcceptVisitor(this, data));

			// Add Statement to Current Statement Collection
			AddStmt(throwStmt);

			return throwStmt;
		}
		
		public override object Visit(FixedStatement fixedStatement, object data)
		{
			throw new NotSupportedException("CodeDom does not support Fixed Statement");
		}
		
		#region Expressions
		public override object Visit(PrimitiveExpression expression, object data)
		{
			return new CodePrimitiveExpression(expression.Value);
		}
		
		public override object Visit(BinaryOperatorExpression expression, object data)
		{
			CodeBinaryOperatorType op = CodeBinaryOperatorType.Add;
			switch (expression.Op) {
				case BinaryOperatorType.Add:
					op = CodeBinaryOperatorType.Add;
					break;
				case BinaryOperatorType.BitwiseAnd:
					op = CodeBinaryOperatorType.BitwiseAnd;
					break;
				case BinaryOperatorType.BitwiseOr:
					op = CodeBinaryOperatorType.BitwiseOr;
					break;
				case BinaryOperatorType.LogicalAnd:
					op = CodeBinaryOperatorType.BooleanAnd;
					break;
				case BinaryOperatorType.LogicalOr:
					op = CodeBinaryOperatorType.BooleanOr;
					break;
				case BinaryOperatorType.Divide:
				case BinaryOperatorType.DivideInteger:
					op = CodeBinaryOperatorType.Divide;
					break;
				case BinaryOperatorType.GreaterThan:
					op = CodeBinaryOperatorType.GreaterThan;
					break;
				case BinaryOperatorType.GreaterThanOrEqual:
					op = CodeBinaryOperatorType.GreaterThanOrEqual;
					break;
				case BinaryOperatorType.Equality:
					op = CodeBinaryOperatorType.IdentityEquality;
					break;
				case BinaryOperatorType.InEquality:
					op = CodeBinaryOperatorType.IdentityInequality;
					break;
				case BinaryOperatorType.LessThan:
					op = CodeBinaryOperatorType.LessThan;
					break;
				case BinaryOperatorType.LessThanOrEqual:
					op = CodeBinaryOperatorType.LessThanOrEqual;
					break;
				case BinaryOperatorType.Modulus:
					op = CodeBinaryOperatorType.Modulus;
					break;
				case BinaryOperatorType.Multiply:
					op = CodeBinaryOperatorType.Multiply;
					break;
				case BinaryOperatorType.Subtract:
					op = CodeBinaryOperatorType.Subtract;
					break;
					//case BinaryOperatorType.ValueEquality:
					//	op = CodeBinaryOperatorType.ValueEquality;
					//	break;
				case BinaryOperatorType.ShiftLeft:
				case BinaryOperatorType.ShiftRight:
					// CodeDOM suxx
					op = CodeBinaryOperatorType.Multiply;
					break;
				case BinaryOperatorType.ReferenceEquality:
					op = CodeBinaryOperatorType.IdentityEquality;
					break;
				case BinaryOperatorType.ReferenceInequality:
					op = CodeBinaryOperatorType.IdentityInequality;
					break;
				case BinaryOperatorType.AsCast:
				case BinaryOperatorType.TypeCheck:
					// CodeDOM suxx
					op = CodeBinaryOperatorType.Add;
					break;
					
				case BinaryOperatorType.ExclusiveOr:
					// TODO ExclusiveOr
					op = CodeBinaryOperatorType.BitwiseAnd;
					break;
			}
			return new CodeBinaryOperatorExpression((CodeExpression)expression.Left.AcceptVisitor(this, data),
			                                        op,
			                                        (CodeExpression)expression.Right.AcceptVisitor(this, data));
		}
		
		public override object Visit(ParenthesizedExpression expression, object data)
		{
			return expression.Expression.AcceptVisitor(this, data);
		}
		
		public override object Visit(InvocationExpression invocationExpression, object data)
		{
			Expression     target     = invocationExpression.TargetObject;
			CodeExpression targetExpr;
			string         methodName = null;
			if (target == null) {
				targetExpr = new CodeThisReferenceExpression();
			} else if (target is FieldReferenceExpression) {
				FieldReferenceExpression fRef = (FieldReferenceExpression)target;
				targetExpr = (CodeExpression)fRef.TargetObject.AcceptVisitor(this, data);
//	 Commented out because of SD2-428: Form designer does not load child controls in panels
//
//				if (fRef.TargetObject is FieldReferenceExpression) {
//					FieldReferenceExpression fRef2 = (FieldReferenceExpression)fRef.TargetObject;
//					if (fRef2.FieldName != null && Char.IsUpper(fRef2.FieldName[0])) {
//						// an exception is thrown if it doesn't end in an indentifier exception
//						// for example for : this.MyObject.MyMethod() leads to an exception, which
//						// is correct in this case ... I know this is really HACKY :)
//						try {
//							CodeExpression tExpr = ConvertToIdentifier(fRef2);
//							if (tExpr != null) {
//								targetExpr = tExpr;
//							}
//						} catch (Exception) {}
//					}
//				}
				methodName = fRef.FieldName;
				// HACK for : Microsoft.VisualBasic.ChrW(NUMBER)
				if (methodName == "ChrW") {
					return new CodeCastExpression("System.Char", GetExpressionList(invocationExpression.Arguments)[0]);
				}
			} else if (target is IdentifierExpression) {
				targetExpr = new CodeThisReferenceExpression();
				methodName = ((IdentifierExpression)target).Identifier;
			} else {
				targetExpr = (CodeExpression)target.AcceptVisitor(this, data);
			}
			return new CodeMethodInvokeExpression(targetExpr, methodName, GetExpressionList(invocationExpression.Arguments));
		}
		
		public override object Visit(IdentifierExpression expression, object data)
		{
			if (IsField(expression.Identifier)) {
				return new CodeVariableReferenceExpression(expression.Identifier);
			}
			return new CodeVariableReferenceExpression(expression.Identifier);
		}
		
		public override object Visit(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			CodeExpression var;

			switch (unaryOperatorExpression.Op) {
				case UnaryOperatorType.Minus:
					if (unaryOperatorExpression.Expression is PrimitiveExpression) {
						PrimitiveExpression expression = (PrimitiveExpression)unaryOperatorExpression.Expression;
						if (expression.Value is int) {
							return new CodePrimitiveExpression(- (int)expression.Value);
						}
						if (expression.Value is System.UInt32 || expression.Value is System.UInt16) {
							return new CodePrimitiveExpression(Int32.Parse("-" + expression.StringValue));
						}
						
						if (expression.Value is long) {
							return new CodePrimitiveExpression(- (long)expression.Value);
						}
						if (expression.Value is double) {
							return new CodePrimitiveExpression(- (double)expression.Value);
						}
						if (expression.Value is float) {
							return new CodePrimitiveExpression(- (float)expression.Value);
						}
						
					}
					return  new CodeBinaryOperatorExpression(new CodePrimitiveExpression(0),
					                                         CodeBinaryOperatorType.Subtract,
					                                         (CodeExpression)unaryOperatorExpression.Expression.AcceptVisitor(this, data));
				case UnaryOperatorType.Plus:
					return unaryOperatorExpression.Expression.AcceptVisitor(this, data);

				case UnaryOperatorType.PostIncrement:
					// emulate i++, with i = i + 1
					var = (CodeExpression)unaryOperatorExpression.Expression.AcceptVisitor(this, data);

					return new CodeAssignStatement(var,
					                               new CodeBinaryOperatorExpression(var,
					                                                                CodeBinaryOperatorType.Add,
					                                                                new CodePrimitiveExpression(1)));

				case UnaryOperatorType.PostDecrement:
					// emulate i--, with i = i - 1
					var = (CodeExpression)unaryOperatorExpression.Expression.AcceptVisitor(this, data);

					return new CodeAssignStatement(var,
					                               new CodeBinaryOperatorExpression(var,
					                                                                CodeBinaryOperatorType.Subtract,
					                                                                new CodePrimitiveExpression(1)));

				case UnaryOperatorType.Decrement:
					// emulate --i, with i = i - 1
					var = (CodeExpression)unaryOperatorExpression.Expression.AcceptVisitor(this, data);

					return new CodeAssignStatement(var,
					                               new CodeBinaryOperatorExpression(var,
					                                                                CodeBinaryOperatorType.Subtract,
					                                                                new CodePrimitiveExpression(1)));

				case UnaryOperatorType.Increment:
					// emulate ++i, with i = i + 1
					var = (CodeExpression)unaryOperatorExpression.Expression.AcceptVisitor(this, data);

					return new CodeAssignStatement(var,
					                               new CodeBinaryOperatorExpression(var,
					                                                                CodeBinaryOperatorType.Add,
					                                                                new CodePrimitiveExpression(1)));

			}
			return null;
		}
		bool methodReference = false;
		
		void AddEventHandler(Expression eventExpr, Expression handler, object data)
		{
			methodReference = true;
			CodeExpression methodInvoker = (CodeExpression)handler.AcceptVisitor(this, data);
			methodReference = false;
			if (!(methodInvoker is CodeObjectCreateExpression)) {
				// we need to create an event handler here
				methodInvoker = new CodeObjectCreateExpression(new CodeTypeReference("System.EventHandler"), methodInvoker);
			}
			
			if (eventExpr is IdentifierExpression) {
				AddStmt(new CodeAttachEventStatement(new CodeEventReferenceExpression(new CodeThisReferenceExpression(), ((IdentifierExpression)eventExpr).Identifier),
				                                     methodInvoker));
			} else {
				FieldReferenceExpression fr = (FieldReferenceExpression)eventExpr;
				AddStmt(new CodeAttachEventStatement(new CodeEventReferenceExpression((CodeExpression)fr.TargetObject.AcceptVisitor(this, data), fr.FieldName),
				                                     methodInvoker));
			}
		}
		
		public override object Visit(AssignmentExpression assignmentExpression, object data)
		{
			if (assignmentExpression.Op == AssignmentOperatorType.Add) {
				AddEventHandler(assignmentExpression.Left, assignmentExpression.Right, data);
			} else {
				if (assignmentExpression.Left is IdentifierExpression) {
					AddStmt(new CodeAssignStatement((CodeExpression)assignmentExpression.Left.AcceptVisitor(this, null), (CodeExpression)assignmentExpression.Right.AcceptVisitor(this, null)));
				} else {
					AddStmt(new CodeAssignStatement((CodeExpression)assignmentExpression.Left.AcceptVisitor(this, null), (CodeExpression)assignmentExpression.Right.AcceptVisitor(this, null)));
				}
			}
			return null;
		}
		
		public override object Visit(AddHandlerStatement addHandlerStatement, object data)
		{
			AddEventHandler(addHandlerStatement.EventExpression, addHandlerStatement.HandlerExpression, data);
			return null;
		}
		
		public override object Visit(AddressOfExpression addressOfExpression, object data)
		{
			return addressOfExpression.Expression.AcceptVisitor(this, data);
		}
		
		public override object Visit(TypeOfExpression typeOfExpression, object data)
		{
			return new CodeTypeOfExpression(ConvType(typeOfExpression.TypeReference.Type));
		}
		
		public override object Visit(CastExpression castExpression, object data)
		{
			string typeRef = ConvType(castExpression.CastTo.Type);
			return new CodeCastExpression(typeRef, (CodeExpression)castExpression.Expression.AcceptVisitor(this, data));
		}
		
		public override object Visit(IndexerExpression indexerExpression, object data)
		{
			return new CodeIndexerExpression((CodeExpression)indexerExpression.TargetObject.AcceptVisitor(this, data), GetExpressionList(indexerExpression.Indices));
		}
		
		public override object Visit(ThisReferenceExpression thisReferenceExpression, object data)
		{
			return new CodeThisReferenceExpression();
		}
		
		public override object Visit(BaseReferenceExpression baseReferenceExpression, object data)
		{
			return new CodeBaseReferenceExpression();
		}
		
		public override object Visit(ArrayCreateExpression arrayCreateExpression, object data)
		{
			if (arrayCreateExpression.ArrayInitializer == null) {
				if (arrayCreateExpression.Rank != null && arrayCreateExpression.Rank.Length > 0) {
					return new CodeArrayCreateExpression(ConvType(arrayCreateExpression.CreateType.Type),
					                                     arrayCreateExpression.Rank[0]);
				}
				return new CodeArrayCreateExpression(ConvType(arrayCreateExpression.CreateType.Type),
				                                     0);
			}
			return new CodeArrayCreateExpression(ConvType(arrayCreateExpression.CreateType.Type),
			                                     GetExpressionList(arrayCreateExpression.ArrayInitializer.CreateExpressions));
		}
		
		public override object Visit(ObjectCreateExpression objectCreateExpression, object data)
		{
			return new CodeObjectCreateExpression(ConvType(objectCreateExpression.CreateType.Type),
			                                      objectCreateExpression.Parameters == null ? null : GetExpressionList(objectCreateExpression.Parameters));
		}
		
		public override object Visit(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			return new CodeParameterDeclarationExpression(new CodeTypeReference(ConvType(parameterDeclarationExpression.TypeReference.Type)), parameterDeclarationExpression.ParameterName);
		}
		
		bool IsField(string type, string fieldName)
		{
			Type t       = null;
			
			t = this.GetType(type); // search in all currently loaded assemblies
			
			bool isField = t != null && (t.IsEnum || t.GetField(fieldName) != null);
			if (!isField) {
				int idx = type.LastIndexOf('.');
				if (idx >= 0) {
					type = type.Substring(0, idx) + "+" + type.Substring(idx + 1);
					isField = IsField(type, fieldName);
				}
			}
			
			return isField;
		}
		
		bool IsFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression)
		{
			if (fieldReferenceExpression.TargetObject is ThisReferenceExpression) {
				foreach (object o in this.currentTypeDeclaration.Children) {
					if (o is FieldDeclaration) {
						FieldDeclaration fd = (FieldDeclaration)o;
						foreach (VariableDeclaration field in fd.Fields) {
							if (fieldReferenceExpression.FieldName == field.Name) {
								return true;
							}
						}
					}
				}
			}
			return false; //Char.IsLower(fieldReferenceExpression.FieldName[0]);
		}
		
		public override object Visit(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			if (methodReference) {
				methodReference = false;
				return new CodeMethodReferenceExpression((CodeExpression)fieldReferenceExpression.TargetObject.AcceptVisitor(this, data), fieldReferenceExpression.FieldName);
			}
			if (IsFieldReferenceExpression(fieldReferenceExpression)) {
				return new CodeFieldReferenceExpression((CodeExpression)fieldReferenceExpression.TargetObject.AcceptVisitor(this, data),
				                                        fieldReferenceExpression.FieldName);
			} else {
				if (fieldReferenceExpression.TargetObject is FieldReferenceExpression) {
					if (IsQualIdent((FieldReferenceExpression)fieldReferenceExpression.TargetObject)) {
						CodeTypeReferenceExpression typeRef = ConvertToIdentifier((FieldReferenceExpression)fieldReferenceExpression.TargetObject);
						if (IsField(typeRef.Type.BaseType, fieldReferenceExpression.FieldName)) {
							return new CodeFieldReferenceExpression(typeRef,
							                                        fieldReferenceExpression.FieldName);
						} else {
							return new CodePropertyReferenceExpression(typeRef,
							                                           fieldReferenceExpression.FieldName);
						}
					}
				}
				
				CodeExpression codeExpression = (CodeExpression)fieldReferenceExpression.TargetObject.AcceptVisitor(this, data);
				return new CodePropertyReferenceExpression(codeExpression,
				                                           fieldReferenceExpression.FieldName);
			}
		}
		#endregion
		
		#endregion
		bool IsQualIdent(FieldReferenceExpression fieldReferenceExpression)
		{
			while (fieldReferenceExpression.TargetObject is FieldReferenceExpression) {
				fieldReferenceExpression = (FieldReferenceExpression)fieldReferenceExpression.TargetObject;
			}
			return fieldReferenceExpression.TargetObject is IdentifierExpression;
		}
		
		bool IsField(string identifier)
		{
			foreach (INode node in currentTypeDeclaration.Children) {
				if (node is FieldDeclaration) {
					FieldDeclaration fd = (FieldDeclaration)node;
					if (fd.GetVariableDeclaration(identifier) != null) {
						return true;
					}
				}
			}
			return false;
		}
		
		CodeTypeReferenceExpression ConvertToIdentifier(FieldReferenceExpression fieldReferenceExpression)
		{
			StringBuilder type = new StringBuilder("");
			
			while (fieldReferenceExpression.TargetObject is FieldReferenceExpression) {
				type.Insert(0,'.');
				type.Insert(1,fieldReferenceExpression.FieldName.ToCharArray());
				fieldReferenceExpression = (FieldReferenceExpression)fieldReferenceExpression.TargetObject;
			}
			
			type.Insert(0,'.');
			type.Insert(1,fieldReferenceExpression.FieldName.ToCharArray());
			
			if (fieldReferenceExpression.TargetObject is IdentifierExpression) {
				type.Insert(0, ((IdentifierExpression)fieldReferenceExpression.TargetObject).Identifier.ToCharArray());
				string oldType = type.ToString();
				int idx = oldType.LastIndexOf('.');
				while (idx > 0) {
					if (Type.GetType(type.ToString()) != null) {
						break;
					}
					string stype = type.ToString().Substring(idx + 1);
					type = new StringBuilder(type.ToString().Substring(0, idx));
					type.Append("+");
					type.Append(stype);
					idx = type.ToString().LastIndexOf('.');
				}
				if (Type.GetType(type.ToString()) == null) {
					type = new StringBuilder(oldType);
				}
				return new CodeTypeReferenceExpression(type.ToString());
			} else {
				return null;
			}
		}
		
		CodeExpression[] GetExpressionList(ArrayList expressionList)
		{
			if (expressionList == null) {
				return new CodeExpression[0];
			}
			CodeExpression[] list = new CodeExpression[expressionList.Count];
			for (int i = 0; i < expressionList.Count; ++i) {
				list[i] = (CodeExpression)((Expression)expressionList[i]).AcceptVisitor(this, null);
				if (list[i] == null) {
					list[i] = new CodePrimitiveExpression(0);
				}
			}
			return list;
		}
		
		Type GetType(string typeName)
		{
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type type = asm.GetType(typeName);
				if (type != null)
				{
					return type;
				}
			}
			return Type.GetType(typeName);
		}
	}
}
