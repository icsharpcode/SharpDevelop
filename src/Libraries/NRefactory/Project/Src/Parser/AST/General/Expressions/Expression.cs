// Expression.cs
// Copyright (C) 2003 Mike Krueger (mike@icsharpcode.net)
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

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
