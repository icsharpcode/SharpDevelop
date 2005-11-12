// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST 
{
	public class IfElseStatement : Statement
	{
		Expression condition;
		ArrayList trueStatement; // List for stmt : stmt : stmt ... in VB.NET
		ArrayList falseStatement; // [Statement]
		
		ArrayList elseIfSections = new ArrayList(1); // VB.NET only, [ElseIfSection]
		
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
		
		public ArrayList ElseIfSections {
			get {
				return elseIfSections;
			}
			set {
				elseIfSections = value == null ? new ArrayList(1) : value;
			}
		}
		
		public ArrayList TrueStatement {
			get {
				return trueStatement;
			}
			set {
				trueStatement = value == null ? new ArrayList(1) : value;
			}
		}
		
		public ArrayList FalseStatement {
			get {
				return falseStatement;
			}
			set {
				falseStatement = value == null ? new ArrayList(1) : value;
			}
		}
		
		public IfElseStatement(Expression condition, Statement trueStatement)
		{
			this.Condition      = condition;
			this.trueStatement  = new ArrayList(1);
			this.falseStatement = new ArrayList(1);
			this.trueStatement.Add(Statement.CheckNull(trueStatement));
		}
		
		public IfElseStatement(Expression condition, Statement trueStatement, Statement falseStatement)
		{
			this.Condition      = condition;
			this.trueStatement  = new ArrayList(1);
			this.falseStatement = new ArrayList(1);
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
