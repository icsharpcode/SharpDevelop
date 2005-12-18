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
	public class UncheckedStatement : Statement
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
		
		public UncheckedStatement(Statement block)
		{
			this.Block = block;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[UncheckedStatement: Block={0}]", 
			                     block);
		}
	}
}
