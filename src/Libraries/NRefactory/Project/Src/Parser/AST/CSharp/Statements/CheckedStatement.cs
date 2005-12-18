// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class CheckedStatement : Statement
	{
		Statement block;
		
		public Statement Block {
			get {
				return block;
			}
			set {
				block = Statement.CheckNull(value);
			}
		}
		
		public CheckedStatement(Statement block)
		{
			this.Block = block;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[CheckedStatement: Block={0}]", 
			                     block);
		}
	}
}
