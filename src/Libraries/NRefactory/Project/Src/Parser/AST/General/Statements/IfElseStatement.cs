// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST 
{
	public class IfElseStatement : Statement
	{
		Expression condition;
		List<Statement> trueStatement; // List for stmt : stmt : stmt ... in VB.NET
		List<Statement> falseStatement; // [Statement]
		
		List<Statement> elseIfSections = new List<Statement>(1); // VB.NET only, [ElseIfSection]
		
		public bool HasElseStatements {
			get {
				return falseStatement.Count > 0;
			}
		}
		
		public bool HasElseIfSections {
			get {
				return elseIfSections.Count > 0;
			}
		}
		
		public Expression Condition {
			get {
				return condition;
			}
			set {
				condition = Expression.CheckNull(value);
			}
		}
		
		public List<Statement> ElseIfSections {
			get {
				return elseIfSections;
			}
			set {
				elseIfSections = value ?? new List<Statement>(1);
			}
		}
		
		public List<Statement> TrueStatement {
			get {
				return trueStatement;
			}
			set {
				trueStatement = value ?? new List<Statement>(1);
			}
		}
		
		public List<Statement> FalseStatement {
			get {
				return falseStatement;
			}
			set {
				falseStatement = value ?? new List<Statement>(1);
			}
		}
		
		public IfElseStatement(Expression condition, Statement trueStatement)
		{
			this.Condition      = condition;
			this.trueStatement  = new List<Statement>(1);
			this.falseStatement = new List<Statement>(1);
			this.trueStatement.Add(Statement.CheckNull(trueStatement));
		}
		
		public IfElseStatement(Expression condition, Statement trueStatement, Statement falseStatement)
		{
			this.Condition      = condition;
			this.trueStatement  = new List<Statement>(1);
			this.falseStatement = new List<Statement>(1);
			this.trueStatement.Add(Statement.CheckNull(trueStatement));
			this.falseStatement.Add(Statement.CheckNull(falseStatement));
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[IfElseStatement: Condition={0}, TrueStatement={1}, FalseStatement={2}]",
			                     condition,
			                     trueStatement,
			                     falseStatement
			                     );
		}
	}
	
	public class ElseIfSection : StatementWithEmbeddedStatement
	{
		Expression condition;
		
		public Expression Condition {
			get {
				return condition;
			}
			set {
				condition = Expression.CheckNull(value);
			}
		}
		
		public ElseIfSection(Expression condition, Statement embeddedStatement)
		{
			this.Condition         = condition;
			this.EmbeddedStatement = embeddedStatement;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[ElseIfStatement: Condition={0}, EmbeddedStatement={1}]",
			                     condition,
			                     EmbeddedStatement
			                     );
		}
	}
	
}
