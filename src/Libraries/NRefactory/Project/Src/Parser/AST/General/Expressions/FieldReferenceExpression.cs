// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST {
	
	public class FieldReferenceExpression : Expression
	{
		Expression targetObject;
		string fieldName;
		
		public Expression TargetObject {
			get {
				return targetObject;
			}
			set {
				targetObject = Expression.CheckNull(value);
			}
		}
		
		public string FieldName {
			get {
				return fieldName;
			}
			set {
				fieldName = value == null ? String.Empty : value;
			}
		}
		
		public FieldReferenceExpression(Expression targetObject, string fieldName)
		{
			this.TargetObject = targetObject;
			this.FieldName    = fieldName;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[FieldReferenceExpression: FieldName={0}, TargetObject={1}]",
			                     fieldName,
			                     targetObject);
		}
	}
}
