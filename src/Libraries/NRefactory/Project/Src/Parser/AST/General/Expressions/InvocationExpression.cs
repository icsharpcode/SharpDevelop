using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST {
	
	public class InvocationExpression : Expression
	{
		Expression       targetObject;
//		List<Expression> parameters;
		ArrayList parameters;
		
		public Expression TargetObject {
			get {
				return targetObject;
			}
			set {
				targetObject = Expression.CheckNull(value);
			}
		}
		
		public ArrayList Parameters {
			get {
				return parameters;
			}
			set {
				parameters = value == null ? new ArrayList(1) : value;
			}
		}
		
		public InvocationExpression(Expression targetObject, ArrayList parameters)
		{
			this.TargetObject = targetObject;
			this.Parameters   = parameters;
		}
		
		public InvocationExpression(Expression targetObject)
		{
			this.TargetObject = targetObject;
			this.parameters   = new ArrayList(1);
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[InvocationExpression: TargetObject={0}, parameters={1}]",
			                     TargetObject,
			                     GetCollectionString(Parameters));
		}
	}
}
