// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.PrettyPrinter;
using Ast = ICSharpCode.NRefactory.Ast;
using Debugger.Wrappers.CorSym;

namespace Debugger
{
	/// <summary>
	/// Represents a piece of code that can be evaluated.
	/// For example "a[15] + 15".
	/// </summary>
	public partial class Expression: DebuggerObject
	{
		Ast.Expression abstractSynatxTree;
		
		public string Code {
			get {
				if (abstractSynatxTree != null) {
					CSharpOutputVisitor csOutVisitor = new CSharpOutputVisitor();
					abstractSynatxTree.AcceptVisitor(csOutVisitor, null);
					return csOutVisitor.Text;
				} else {
					return string.Empty;
				}
			}
		}
		
		public Ast.Expression AbstractSynatxTree {
			get { return abstractSynatxTree; }
		}
		
		public Expression(Ast.Expression abstractSynatxTree)
		{
			this.abstractSynatxTree = abstractSynatxTree;
		}
		
		public Expression(string code)
		{
			throw new NotImplementedException();
		}
		
		public static implicit operator Expression(Ast.Expression abstractSynatxTree)
		{
			return new Expression(abstractSynatxTree);
		}
		
		public static implicit operator Ast.Expression(Expression expression)
		{
			if (expression == null) {
				return null;
			} else {
				return expression.AbstractSynatxTree;
			}
		}
		
		public override string ToString()
		{
			return this.Code;
		}
	}
}
