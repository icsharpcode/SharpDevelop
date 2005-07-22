// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST 
{
	public abstract class Expression : AbstractNode, INullable
	{
		public static NullExpression Null {
			get {
				return NullExpression.Instance;
			}
		}
		
		public virtual bool IsNull {
			get {
				return false;
			}
		}
		
		public static Expression CheckNull(Expression expression)
		{
			return expression == null ? NullExpression.Instance : expression;
		}
	}
	
	public class NullExpression : Expression
	{
		static NullExpression nullExpression = new NullExpression();
		
		public override bool IsNull {
			get {
				return true;
			}
		}
		
		public static NullExpression Instance {
			get {
				return nullExpression;
			}
		}
		
		NullExpression()
		{
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return this;
		}
		
		public override string ToString()
		{
			return String.Format("[NullExpression]");
		}
	}
}
