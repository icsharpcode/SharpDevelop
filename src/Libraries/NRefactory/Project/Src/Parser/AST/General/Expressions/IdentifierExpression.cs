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
	public class IdentifierExpression : Expression
	{
		string identifier;
		
		public string Identifier {
			get {
				return identifier;
			}
			set {
				identifier = value == null ? String.Empty : value;
			}
		}
		
		public IdentifierExpression(string identifier)
		{
			this.Identifier = identifier;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[IdentifierExpression: Identifier={0}]",
			                     identifier);
		}
	}
}
