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
	public class PointerReferenceExpression : Expression
	{
		Expression targetObject;
		string     identifier;
		
		public Expression TargetObject {
			get {
				return targetObject;
			}
			set {
				targetObject = Expression.CheckNull(value);
			}
		}
		
		public string Identifier {
			get {
				return identifier;
			}
			set {
				identifier = value == null ? String.Empty : value;
			}
		}
		
		public PointerReferenceExpression(Expression targetObject, string identifier)
		{
			this.TargetObject = targetObject;
			this.Identifier   = identifier;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[PointerReferenceExpression: TargetObject={0}, Identifier={1}]",
			                     targetObject,
			                     identifier);
		}
	}
}
