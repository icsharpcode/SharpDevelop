/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 21.05.2005
 * Time: 21:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Diagnostics;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class AnonymousMethodExpression : Expression
	{
		ArrayList parameters = new ArrayList(4);
		
		public ArrayList Parameters {
			get {
				return parameters;
			}
			set {
				Debug.Assert(value != null);
				parameters = value;
			}
		}
		
		BlockStatement body = BlockStatement.Null;
		
		public BlockStatement Body {
			get {
				return body;
			}
			set {
				body = BlockStatement.CheckNull(value);
			}
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[AnonymousMethodExpression: Parameters={0} Body={1}]",
			                     GetCollectionString(Parameters),
			                     Body);
		}
	}
}
