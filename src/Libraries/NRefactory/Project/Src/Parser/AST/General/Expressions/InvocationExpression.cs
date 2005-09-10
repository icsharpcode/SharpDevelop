// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST {
	
	public class InvocationExpression : Expression
	{
		Expression       targetObject;
//		List<Expression> arguments;
		ArrayList arguments;
		List<TypeReference> typeArguments;
		
		public Expression TargetObject {
			get {
				return targetObject;
			}
			set {
				targetObject = Expression.CheckNull(value);
			}
		}
		
		public ArrayList Arguments {
			get {
				return arguments;
			}
			set {
				arguments = value == null ? new ArrayList(1) : value;
			}
		}
		
		public List<TypeReference> TypeArguments {
			get {
				return typeArguments;
			}
			set {
				typeArguments = value == null ? new List<TypeReference>(1) : value;
			}
		}
		
		public InvocationExpression(Expression targetObject, ArrayList parameters)
		{
			this.TargetObject = targetObject;
			this.Arguments   = parameters;
		}
		
		public InvocationExpression(Expression targetObject, ArrayList parameters, List<TypeReference> typeParameters)
		{
			this.TargetObject = targetObject;
			this.Arguments   = parameters;
			this.TypeArguments = typeParameters;
		}
		
		public InvocationExpression(Expression targetObject)
		{
			this.TargetObject = targetObject;
			this.arguments   = new ArrayList(1);
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[InvocationExpression: TargetObject={0}, parameters={1}]",
			                     TargetObject,
			                     GetCollectionString(Arguments));
		}
	}
}
