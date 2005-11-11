// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST 
{
	public class TryCatchStatement : Statement
	{
		Statement         statementBlock;
		List<CatchClause> catchClauses;
		Statement         finallyBlock;
		
		public Statement StatementBlock {
			get {
				return statementBlock;
			}
			set {
				statementBlock = Statement.CheckNull(value);
			}
		}
		
		public List<CatchClause> CatchClauses {
			get {
				return catchClauses;
			}
			set {
				catchClauses = value ?? new List<CatchClause>(1);
			}
		}
		
		public Statement FinallyBlock {
			get {
				return finallyBlock;
			}
			set {
				finallyBlock = Statement.CheckNull(value);
			}
		}
		
		public TryCatchStatement(Statement statementBlock, List<CatchClause> catchClauses, Statement finallyBlock)
		{
			this.StatementBlock = statementBlock;
			this.CatchClauses = catchClauses;
			this.FinallyBlock = finallyBlock;
		}
		
		public TryCatchStatement(Statement statementBlock, List<CatchClause> catchClauses) : this(statementBlock, catchClauses, null)
		{
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[TryCatchStatement: StatementBlock={0}, CatchClauses={1}, FinallyBlock={2}]",
			                     statementBlock,
			                     GetCollectionString(catchClauses),
			                     finallyBlock);
		}
	}
	
	public class CatchClause : AbstractNode
	{
		TypeReference typeReference = TypeReference.Null;
		string     variableName = "";
		Statement  statementBlock = Statement.Null;
		Expression condition = Expression.Null;
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				typeReference = TypeReference.CheckNull(value);
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
		
		public string VariableName {
			get {
				return variableName;
			}
			set {
				variableName = value == null ? String.Empty : value;
			}
		}
		
		public Statement StatementBlock {
			get {
				return statementBlock;
			}
			set {
				statementBlock = Statement.CheckNull(value);
			}
		}
		
		public CatchClause(TypeReference typeReference, string variableName, Statement statementBlock)
		{
			this.TypeReference  = typeReference;
			this.VariableName   = variableName;
			this.StatementBlock = statementBlock;
		}
		
		public CatchClause(TypeReference typeReference, string variableName, Statement statementBlock, Expression condition)
		{
			this.TypeReference  = typeReference;
			this.VariableName   = variableName;
			this.StatementBlock = statementBlock;
			this.Condition      = condition;
		}
		
		public CatchClause(Statement statementBlock)
		{
			this.StatementBlock = statementBlock;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[CatchClause: TypeReference={0}, VariableName={1}, StatementBlock={2}]", 
			                     TypeReference,
			                     variableName,
			                     statementBlock);
		}
	}
}
