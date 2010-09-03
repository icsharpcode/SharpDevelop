// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.NRefactory.Ast;
using B = Boo.Lang.Compiler.Ast;

namespace NRefactoryToBooConverter
{
	partial class ConvertVisitor
	{
		void ConvertStatements(IEnumerable statements, B.Block b)
		{
			foreach (Statement n in statements) {
				AddToBlock(n, b);
			}
		}
		
		Statement currentStatement;
		
		object ConvertStatementInternal(Statement stmt)
		{
			Statement oldStatement = currentStatement;
			currentStatement = stmt;
			try {
				return stmt.AcceptVisitor(this, null);
			} finally {
				currentStatement = oldStatement;
			}
		}
		
		void AddToBlock(Statement n, B.Block b)
		{
			object result = ConvertStatementInternal(n);
			if (result is ArrayList) {
				foreach (B.Statement stmt in (ArrayList)result) {
					b.Add(stmt);
				}
			} else {
				B.Statement stmt = (B.Statement)result;
				if (stmt != null) {
					b.Add(stmt);
				}
			}
		}
		
		ArrayList ConvertStatements(IEnumerable statements)
		{
			ArrayList r = new ArrayList();
			foreach (Statement n in statements) {
				object result = ConvertStatementInternal(n);
				if (result is ArrayList) {
					r.AddRange((ArrayList)result);
				} else if (result != null) {
					r.Add(result);
				}
			}
			return r;
		}
		
		B.Block ConvertBlock(Statement statement)
		{
			if (statement == null || statement.IsNull)
				return null;
			List<Statement> statements = new List<Statement>(1);
			statements.Add(statement);
			return ConvertBlock(statements);
		}
		
		B.Block ConvertBlock(BlockStatement block)
		{
			B.Block b = new B.Block(GetLexicalInfo(block));
			b.EndSourceLocation = GetLocation(block.EndLocation);
			ConvertStatements(block.Children, b);
			return b;
		}
		
		B.Block ConvertBlock(List<Statement> statements)
		{
			if (statements.Count == 1) {
				if (statements[0] is BlockStatement)
					return ConvertBlock(statements[0] as BlockStatement);
			}
			B.Block b = new B.Block();
			ConvertStatements(statements, b);
			return b;
		}
		
		public object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			return ConvertBlock(blockStatement);
		}
		
		B.MacroStatement CreateMacro(INode node, string name, Statement embedded, params Expression[] arguments)
		{
			B.MacroStatement macro = new B.MacroStatement(GetLexicalInfo(node));
			macro.Name = name;
			ConvertExpressions(arguments, macro.Arguments);
			if (embedded is BlockStatement) {
				macro.Body = ConvertBlock((BlockStatement)embedded);
			} else {
				macro.Body = new B.Block();
				macro.Body.Add((B.Statement)embedded.AcceptVisitor(this, null));
			}
			return macro;
		}
		
		public object VisitFixedStatement(FixedStatement fixedStatement, object data)
		{
			AddError(fixedStatement, "FixedStatement is not supported.");
			return null;
		}
		
		public object VisitUnsafeStatement(UnsafeStatement unsafeStatement, object data)
		{
			AddError(unsafeStatement, "UnsafeStatement is not supported.");
			return null;
		}
		
		public object VisitCheckedStatement(CheckedStatement checkedStatement, object data)
		{
			return CreateMacro(checkedStatement, "checked", checkedStatement.Block);
		}
		
		public object VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
		{
			return CreateMacro(uncheckedStatement, "unchecked", uncheckedStatement.Block);
		}
		
		public object VisitExitStatement(ExitStatement exitStatement, object data)
		{
			if (exitStatement.ExitType == ExitType.Function || exitStatement.ExitType == ExitType.Sub || exitStatement.ExitType == ExitType.Property) {
				AddWarning(exitStatement, "ExitStatement is converted to 'return'");
				return new B.ReturnStatement(GetLexicalInfo(exitStatement));
			} else {
				AddWarning(exitStatement, "ExitStatement is converted to 'break'");
				return new B.BreakStatement(GetLexicalInfo(exitStatement));
			}
		}
		
		/// <summary>
		/// Make a loop:
		/// $initializers
		/// goto converterGeneratedName#
		/// while true:
		///     $iterators
		///     :converterGeneratedName#
		///     break $conditionType $condition
		/// 	$body
		/// </summary>
		ArrayList MakeManualLoop(INode node, List<Statement> initializers, B.StatementModifierType conditionType, Expression condition, List<Statement> iterators, Statement body)
		{
			// we use this "while true" form because "continue" must not skip the iterator.
			
			ArrayList list = ConvertStatements(initializers);
			B.LabelStatement labelStatement = MakeLabel(GenerateName());
			B.GotoStatement gotoStatement = new B.GotoStatement();
			gotoStatement.Label = new B.ReferenceExpression(labelStatement.Name);
			list.Add(gotoStatement);
			
			B.WhileStatement w = new B.WhileStatement(GetLexicalInfo(node));
			w.Condition = new B.BoolLiteralExpression(true);
			list.Add(w);
			w.Block = ConvertBlock(iterators);
			B.BreakStatement breakStatement = new B.BreakStatement();
			breakStatement.Modifier = new B.StatementModifier(conditionType, ConvertExpression(condition));
			w.Block.Add(labelStatement);
			w.Block.Add(breakStatement);
			foreach (B.Statement st in ConvertBlock(body).Statements) {
				w.Block.Add(st);
			}
			return list;
		}
		
		ArrayList MakeManualLoop(ForNextStatement forNextStatement)
		{
			Expression var = new IdentifierExpression(forNextStatement.VariableName);
			List<Statement> initializers = new List<Statement>(1);
			initializers.Add(new ExpressionStatement(new AssignmentExpression(var, AssignmentOperatorType.Assign, forNextStatement.Start)));
			List<Statement> iterators = new List<Statement>(1);
			Expression step = forNextStatement.Step;
			if (step == null || step.IsNull)
				step = new PrimitiveExpression(1, "1");
			iterators.Add(new ExpressionStatement(new AssignmentExpression(var, AssignmentOperatorType.Add, step)));
			PrimitiveExpression stepPE = step as PrimitiveExpression;
			if (stepPE == null || !(stepPE.Value is int)) {
				AddError(forNextStatement, "Step must be an integer literal");
				return null;
			}
			BinaryOperatorType conditionOperator;
			if ((int)stepPE.Value < 0) {
				conditionOperator = BinaryOperatorType.GreaterThanOrEqual;// counting down
			} else {
				conditionOperator = BinaryOperatorType.LessThanOrEqual;// counting up
			}
			Expression condition = new BinaryOperatorExpression(var, conditionOperator, forNextStatement.End);
			return MakeManualLoop(forNextStatement, initializers, B.StatementModifierType.Unless, condition, iterators, forNextStatement.EmbeddedStatement);
		}
		
		public object VisitForNextStatement(ForNextStatement forNextStatement, object data)
		{
			if (forNextStatement.TypeReference.IsNull)
				return MakeManualLoop(forNextStatement);
			B.ForStatement fs = new B.ForStatement(GetLexicalInfo(forNextStatement));
			fs.Block = ConvertBlock(forNextStatement.EmbeddedStatement);
			fs.Declarations.Add(new B.Declaration(forNextStatement.VariableName, null));
			B.Expression start = ConvertExpression(forNextStatement.Start);
			Expression end = forNextStatement.End;
			if (forNextStatement.Step == null || forNextStatement.Step.IsNull) {
				// range only goes to end - 1, so increment end
				end = Expression.AddInteger(end, 1);
				fs.Iterator = MakeMethodCall("range", start, ConvertExpression(end));
			} else {
				PrimitiveExpression stepPE = forNextStatement.Step as PrimitiveExpression;
				if (stepPE == null || !(stepPE.Value is int)) {
					AddError(forNextStatement, "Step must be an integer literal");
				} else {
					if ((int)stepPE.Value < 0)
						end = Expression.AddInteger(end, -1);
					else
						end = Expression.AddInteger(end, 1);
				}
				fs.Iterator = MakeMethodCall("range", start, ConvertExpression(end), ConvertExpression(forNextStatement.Step));
			}
			return fs;
		}
		
		public object VisitForStatement(ForStatement forStatement, object data)
		{
			return MakeManualLoop(forStatement, forStatement.Initializers, B.StatementModifierType.Unless,
			                      forStatement.Condition, forStatement.Iterator, forStatement.EmbeddedStatement);
		}
		
		public object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			bool frontCondition = doLoopStatement.ConditionPosition != ConditionPosition.End;
			bool negateCondition = doLoopStatement.ConditionType == ConditionType.Until;
			if (frontCondition && negateCondition) {
				// VB: Do Unless * : ** : Loop
				B.UnlessStatement u = new B.UnlessStatement(GetLexicalInfo(doLoopStatement));
				u.Condition = ConvertExpression(doLoopStatement.Condition);
				u.Block = ConvertBlock(doLoopStatement.EmbeddedStatement);
				return u;
			}
			// While and Do loop
			B.WhileStatement w = new B.WhileStatement(GetLexicalInfo(doLoopStatement));
			if (frontCondition)
				w.Condition = ConvertExpression(doLoopStatement.Condition);
			else
				w.Condition = new B.BoolLiteralExpression(true);
			w.Block = ConvertBlock(doLoopStatement.EmbeddedStatement);
			if (!frontCondition) {
				B.BreakStatement breakStatement = new B.BreakStatement();
				breakStatement.Modifier = new B.StatementModifier(negateCondition ? B.StatementModifierType.If : B.StatementModifierType.Unless,
				                                                  ConvertExpression(doLoopStatement.Condition));
				w.Block.Add(breakStatement);
			}
			return w;
		}
		
		public object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			B.ForStatement fs = new B.ForStatement(GetLexicalInfo(foreachStatement));
			fs.EndSourceLocation = GetLocation(foreachStatement.EndLocation);
			fs.Iterator = ConvertExpression(foreachStatement.Expression);
			fs.Declarations.Add(new B.Declaration(foreachStatement.VariableName, ConvertTypeReference(foreachStatement.TypeReference)));
			fs.Block = ConvertBlock(foreachStatement.EmbeddedStatement);
			return fs;
		}
		
		public object VisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data)
		{
			B.Expression expr = new B.BinaryExpression(GetLexicalInfo(addHandlerStatement),
			                                           B.BinaryOperatorType.InPlaceAddition,
			                                           ConvertExpression(addHandlerStatement.EventExpression),
			                                           ConvertExpression(addHandlerStatement.HandlerExpression));
			return new B.ExpressionStatement(expr);
		}
		
		public object VisitRemoveHandlerStatement(RemoveHandlerStatement removeHandlerStatement, object data)
		{
			B.Expression expr = new B.BinaryExpression(GetLexicalInfo(removeHandlerStatement),
			                                           B.BinaryOperatorType.InPlaceSubtraction,
			                                           ConvertExpression(removeHandlerStatement.EventExpression),
			                                           ConvertExpression(removeHandlerStatement.HandlerExpression));
			return new B.ExpressionStatement(expr);
		}
		
		public object VisitRaiseEventStatement(RaiseEventStatement raiseEventStatement, object data)
		{
			B.MethodInvocationExpression mie = new B.MethodInvocationExpression(GetLexicalInfo(raiseEventStatement));
			mie.Target = new B.ReferenceExpression(raiseEventStatement.EventName);
			ConvertExpressions(raiseEventStatement.Arguments, mie.Arguments);
			return new B.ExpressionStatement(mie);
		}
		
		public object VisitEraseStatement(EraseStatement eraseStatement, object data)
		{
			ArrayList statements = new ArrayList();
			foreach (Expression expr in eraseStatement.Expressions) {
				B.Expression e = ConvertExpression(expr);
				e = new B.BinaryExpression(B.BinaryOperatorType.Assign, e, new B.NullLiteralExpression());
				statements.Add(new B.ExpressionStatement(e));
			}
			return statements;
		}
		
		public object VisitReDimStatement(ReDimStatement reDimStatement, object data)
		{
			// Redim [Preserve] a(newBounds)
			// without preserve:
			//    a = array(Type, newBounds)
			// with preserve:
			//    ??1 = array(Type, newBounds)
			//    Array.Copy(a, ??1, System.Math.Min(a.Length, ??1.Length))
			//    a = ??1
			if (reDimStatement.IsPreserve)
				AddError(reDimStatement, "Redim Preserve is not supported.");
			ArrayList list = new ArrayList();
			foreach (InvocationExpression o in reDimStatement.ReDimClauses) {
				IdentifierExpression identifier = o.TargetObject as IdentifierExpression;
				if (identifier == null) {
					AddError(o, "Sorry, that expression is too complex to be resolved by the converter.");
				} else {
					if (identifier.TypeArguments != null && identifier.TypeArguments.Count > 0) {
						AddError(o, "Type parameters are not allowed here.");
					}
					
					// first we need to find out the array type
					VariableResolver resolver = new VariableResolver(nameComparer);
					TypeReference r = resolver.FindType(identifier.Identifier, reDimStatement);
					if (r == null) {
						AddError(o, "The name '" + identifier.Identifier + "' could not be resolved by the converter.");
					} else if (!r.IsArrayType) {
						AddError(o, identifier.Identifier + " is not an array.");
					} else {
						ArrayCreateExpression ace = new ArrayCreateExpression(r);
						foreach (Expression boundExpr in o.Arguments) {
							ace.Arguments.Add(Expression.AddInteger((Expression)boundExpr, 1));
						}
						ace.StartLocation = o.StartLocation;
						B.Expression expr = new B.ReferenceExpression(GetLexicalInfo(identifier), identifier.Identifier);
						expr = new B.BinaryExpression(GetLexicalInfo(reDimStatement), B.BinaryOperatorType.Assign, expr, ConvertExpression(ace));
						list.Add(new B.ExpressionStatement(expr));
					}
				}
			}
			return list;
		}
		
		public object VisitExpressionStatement(ExpressionStatement statementExpression, object data)
		{
			B.ExpressionStatement st = new B.ExpressionStatement(GetLexicalInfo(statementExpression));
			st.Expression = ConvertExpression(statementExpression.Expression);
			return st;
		}
		
		public object VisitLocalVariableDeclaration(LocalVariableDeclaration lvd, object data)
		{
			ArrayList list = new ArrayList();
			for (int i = 0; i < lvd.Variables.Count; i++) {
				B.DeclarationStatement varDecl = new B.DeclarationStatement(GetLexicalInfo(lvd));
				varDecl.Declaration = new B.Declaration(GetLexicalInfo(lvd.Variables[i]), lvd.Variables[i].Name, ConvertTypeReference(lvd.GetTypeForVariable(i)));
				varDecl.Initializer = ConvertExpression(lvd.Variables[i].Initializer);
				list.Add(varDecl);
			}
			return list;
		}
		
		public object VisitEmptyStatement(EmptyStatement emptyStatement, object data)
		{
			return null;
		}
		
		public object VisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			return new B.ReturnStatement(GetLexicalInfo(returnStatement), ConvertExpression(returnStatement.Expression), null);
		}
		
		public object VisitYieldStatement(YieldStatement yieldStatement, object data)
		{
			ReturnStatement rs = yieldStatement.Statement as ReturnStatement;
			if (rs == null)
				return new B.ReturnStatement(GetLexicalInfo(yieldStatement));
			return new B.YieldStatement(GetLexicalInfo(yieldStatement), ConvertExpression(rs.Expression));
		}
		
		public object VisitThrowStatement(ThrowStatement throwStatement, object data)
		{
			return new B.RaiseStatement(GetLexicalInfo(throwStatement), ConvertExpression(throwStatement.Expression), null);
		}
		
		public object VisitIfElseStatement(IfElseStatement ifElseStatement, object data)
		{
			B.IfStatement ifs = new B.IfStatement(GetLexicalInfo(ifElseStatement));
			B.IfStatement outerIf = ifs;
			ifs.EndSourceLocation = GetLocation(ifElseStatement.EndLocation);
			ifs.Condition = ConvertExpression(ifElseStatement.Condition);
			ifs.TrueBlock = ConvertBlock(ifElseStatement.TrueStatement);
			if (ifElseStatement.HasElseIfSections) {
				foreach (ElseIfSection sec in ifElseStatement.ElseIfSections) {
					B.IfStatement elif = new B.IfStatement(GetLexicalInfo(sec));
					elif.EndSourceLocation = GetLocation(sec.EndLocation);
					elif.Condition = ConvertExpression(sec.Condition);
					elif.TrueBlock = ConvertBlock(sec.EmbeddedStatement);
					ifs.FalseBlock = new B.Block();
					ifs.FalseBlock.Add(elif);
					ifs = elif;
				}
			}
			if (ifElseStatement.HasElseStatements) {
				ifs.FalseBlock = ConvertBlock(ifElseStatement.FalseStatement);
			}
			return outerIf;
		}
		
		public object VisitElseIfSection(ElseIfSection elseIfSection, object data)
		{
			throw new ApplicationException("ElseIfSection visited");
		}
		
		B.LabelStatement MakeLabel(string name)
		{
			return new B.LabelStatement(lastLexicalInfo, name);
		}
		
		public object VisitLabelStatement(LabelStatement labelStatement, object data)
		{
			return new B.LabelStatement(GetLexicalInfo(labelStatement), labelStatement.Label);
		}
		
		public object VisitGotoStatement(GotoStatement gotoStatement, object data)
		{
			return new B.GotoStatement(GetLexicalInfo(gotoStatement), new B.ReferenceExpression(gotoStatement.Label));
		}
		
		public object VisitStopStatement(StopStatement stopStatement, object data)
		{
			return new B.ExpressionStatement(MakeMethodCall("System.Diagnostics.Debugger.Break"));
		}
		
		public object VisitEndStatement(EndStatement endStatement, object data)
		{
			return new B.ExpressionStatement(MakeMethodCall("System.Environment.Exit", new B.IntegerLiteralExpression(0)));
		}
		
		public object VisitBreakStatement(BreakStatement breakStatement, object data)
		{
			return new B.BreakStatement(GetLexicalInfo(breakStatement));
		}
		
		public object VisitContinueStatement(ContinueStatement continueStatement, object data)
		{
			return new B.ContinueStatement(GetLexicalInfo(continueStatement));
		}
		
		public object VisitLockStatement(LockStatement lockStatement, object data)
		{
			return CreateMacro(lockStatement, "lock", lockStatement.EmbeddedStatement, lockStatement.LockExpression);
		}
		
		public object VisitUsingStatement(UsingStatement usingStatement, object data)
		{
			LocalVariableDeclaration varDecl = usingStatement.ResourceAcquisition as LocalVariableDeclaration;
			Expression expr;
			if (varDecl != null) {
				if (varDecl.Variables.Count != 1) {
					AddError(usingStatement, "Only one variable can be used with the using statement.");
					return null;
				}
				expr = new AssignmentExpression(new IdentifierExpression(varDecl.Variables[0].Name),
				                                AssignmentOperatorType.Assign,
				                                varDecl.Variables[0].Initializer);
			} else {
				ExpressionStatement se = usingStatement.ResourceAcquisition as ExpressionStatement;
				if (se == null) {
					AddError(usingStatement, "Expected: VariableDeclaration or StatementExpression");
					return null;
				}
				expr = se.Expression;
			}
			return CreateMacro(usingStatement, "using", usingStatement.EmbeddedStatement, expr);
		}
		
		public object VisitWithStatement(WithStatement withStatement, object data)
		{
			return CreateMacro(withStatement, "with", withStatement.Body, withStatement.Expression);
		}
		
		public object VisitOnErrorStatement(OnErrorStatement onErrorStatement, object data)
		{
			AddError(onErrorStatement, "old VB-style exception handling is not supported.");
			return null;
		}
		
		public object VisitErrorStatement(ErrorStatement errorStatement, object data)
		{
			AddError(errorStatement, "old VB-style exception handling is not supported.");
			return null;
		}
		
		public object VisitResumeStatement(ResumeStatement resumeStatement, object data)
		{
			AddError(resumeStatement, "old VB-style exception handling is not supported.");
			return null;
		}
		
		public object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{
			B.TryStatement t = new B.TryStatement(GetLexicalInfo(tryCatchStatement));
			t.EndSourceLocation = GetLocation(tryCatchStatement.EndLocation);
			t.ProtectedBlock = ConvertBlock(tryCatchStatement.StatementBlock);
			t.EnsureBlock = ConvertBlock(tryCatchStatement.FinallyBlock);
			foreach (CatchClause clause in tryCatchStatement.CatchClauses) {
				B.ExceptionHandler handler = new B.ExceptionHandler(GetLexicalInfo(clause));
				handler.Block = ConvertBlock(clause.StatementBlock);
				B.TypeReference typeRef = ConvertTypeReference(clause.TypeReference);
				string name = clause.VariableName;
				if (typeRef != null) {
					if (name == null || name.Length == 0)
						name = GenerateName();
					handler.Declaration = new B.Declaration(name, typeRef);
				} else {
					if (name != null && name.Length > 0)
						handler.Declaration = new B.Declaration(name, null);
				}
				t.ExceptionHandlers.Add(handler);
			}
			return t;
		}
		
		public object VisitCatchClause(CatchClause catchClause, object data)
		{
			throw new ApplicationException("CatchClause visited.");
		}
		
		B.Statement GetLastStatement(B.Block block)
		{
			if (block == null || block.Statements.Count == 0)
				return null;
			return block.Statements[block.Statements.Count - 1];
		}
		
		string currentSwitchTempName;
		
		public object VisitSwitchStatement(SwitchStatement switchStatement, object data)
		{
			// We have a problem: given is still not implemented in boo.
			// So here's the if / else workaround:
			string oldSwitchTempName = currentSwitchTempName;
			currentSwitchTempName = GenerateName();
			ArrayList l = new ArrayList(3);
			B.BinaryExpression init = new B.BinaryExpression(B.BinaryOperatorType.Assign,
			                                                 new B.ReferenceExpression(currentSwitchTempName),
			                                                 ConvertExpression(switchStatement.SwitchExpression));
			l.Add(new B.ExpressionStatement(init));
			B.IfStatement dummyStatement = new B.IfStatement(GetLexicalInfo(switchStatement));
			B.IfStatement first = null;
			B.IfStatement current = dummyStatement;
			B.BreakStatement bs;
			for (int i = 0; i < switchStatement.SwitchSections.Count; i++) {
				current = (B.IfStatement)((INode)switchStatement.SwitchSections[i]).AcceptVisitor(this, current);
				if (i == 0) {
					first = current;
				}
				bs = GetLastStatement(current.TrueBlock) as B.BreakStatement;
				if (bs != null)
					bs.ReplaceBy(null);
			}
			bs = GetLastStatement(current.FalseBlock) as B.BreakStatement;
			if (bs != null)
				bs.ReplaceBy(null);
			
			string endSwitchName = currentSwitchTempName + "_end";
			first.Accept(new ReplaceBreakStatementsVisitor(endSwitchName));
			
			FindUnneededLabelsVisitor fulv = new FindUnneededLabelsVisitor(currentSwitchTempName + "_", nameComparer);
			first.Accept(fulv);
			
			bool needsEndLabel = fulv.NeededLabels.Contains(endSwitchName);
			
			fulv.RemoveLabels(); // remove "goto case" labels that aren't needed
			currentSwitchTempName = oldSwitchTempName;
			
			if (first == dummyStatement) {
				l.AddRange(dummyStatement.FalseBlock.Statements);
			} else {
				l.Add(first);
			}
			if (needsEndLabel)
				l.Add(MakeLabel(endSwitchName));
			
			return l;
		}
		
		public object VisitSwitchSection(SwitchSection switchSection, object data)
		{
			B.IfStatement surroundingIf = (B.IfStatement)data;
			bool isDefault = false;
			ArrayList conditions = new ArrayList();
			ArrayList labels = new ArrayList();
			foreach (CaseLabel caseLabel in switchSection.SwitchLabels) {
				if (caseLabel.IsDefault) {
					isDefault = true;
				} else {
					if (caseLabel.BinaryOperatorType != BinaryOperatorType.None) {
						AddError(caseLabel, "VB's Case Is currently not supported (Daniel's too lazy for VB stuff)");
					} else {
						B.Expression expr = ConvertExpression(caseLabel.Label);
						if (expr != null) {
							conditions.Add(new B.BinaryExpression(B.BinaryOperatorType.Equality,
							                                      new B.ReferenceExpression(currentSwitchTempName),
							                                      expr));
							string labelName = expr.ToCodeString().GetHashCode().ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
							labels.Add(MakeLabel(currentSwitchTempName + "_" + labelName));
						}
					}
				}
			}
			B.IfStatement s = null;
			if (conditions.Count > 0) {
				s = new B.IfStatement(GetLexicalInfo(switchSection));
				if (surroundingIf != null) {
					s.FalseBlock = surroundingIf.FalseBlock;
					surroundingIf.FalseBlock = new B.Block();
					surroundingIf.FalseBlock.Add(s);
				}
				s.TrueBlock = new B.Block();
				foreach (B.Statement stmt in labels) {
					s.TrueBlock.Add(stmt);
				}
				B.Expression combined = (B.Expression)conditions[0];
				for (int i = 1; i < conditions.Count; i++) {
					combined = new B.BinaryExpression(B.BinaryOperatorType.Or, combined, (B.Expression)conditions[i]);
				}
				s.Condition = combined;
				foreach (Statement node in switchSection.Children) {
					AddToBlock(node, s.TrueBlock);
				}
			}
			if (s == null)
				s = surroundingIf;
			if (isDefault) {
				if (s.FalseBlock == null)
					s.FalseBlock = new B.Block();
				s.FalseBlock.Add(MakeLabel(currentSwitchTempName + "_default"));
				foreach (Statement node in switchSection.Children) {
					AddToBlock(node, s.FalseBlock);
				}
			}
			return s;
		}
		
		public object VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data)
		{
			if (currentSwitchTempName == null) {
				AddError(gotoCaseStatement, "goto case cannot be used outside switch");
				return null;
			}
			string labelName;
			if (gotoCaseStatement.IsDefaultCase) {
				labelName = "default";
			} else {
				B.Expression expr = ConvertExpression(gotoCaseStatement.Expression);
				if (expr == null) return null;
				labelName = expr.ToCodeString().GetHashCode().ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
			}
			return new B.GotoStatement(GetLexicalInfo(gotoCaseStatement),
			                           new B.ReferenceExpression(currentSwitchTempName + "_" + labelName));
		}
		
		public object VisitCaseLabel(CaseLabel caseLabel, object data)
		{
			throw new ApplicationException("CaseLabel was visited.");
		}
	}
}
