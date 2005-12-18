// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class AnonymousMethodExpression : Expression
	{
		List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>(1);
		
		public List<ParameterDeclarationExpression> Parameters {
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
