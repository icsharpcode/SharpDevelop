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
	public class UnsafeStatement : Statement
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
		
		public UnsafeStatement(Statement block)
		{
			this.Block = block;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[UnsafeStatement: Block={0}]", 
			                     block);
		}
	}
}
