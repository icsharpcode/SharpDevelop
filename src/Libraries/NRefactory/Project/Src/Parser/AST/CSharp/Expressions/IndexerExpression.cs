using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST 
{
	public class IndexerExpression : Expression
	{
		Expression       targetObject;
//		List<Expression> indices;
		ArrayList indices;
		
		public Expression TargetObject {
			get {
				return targetObject;
			}
			set {
				targetObject = Expression.CheckNull(value);
			}
		}
		
		public ArrayList Indices {
			get {
				return indices;
			}
			set {
				indices = value == null ? new ArrayList(1) : value;
			}
		}
		
		public IndexerExpression(Expression targetObject, ArrayList indices)
		{
			this.TargetObject = targetObject;
			this.Indices      = indices;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[IndexerExpression: TargetObject={0}, Indices={1}]",
			                     targetObject,
			                     GetCollectionString(indices));
		}
	}
}
