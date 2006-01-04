// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST 
{
	public class IndexerExpression : Expression
	{
		Expression       targetObject;
		List<Expression> indices;
		
		public Expression TargetObject {
			get {
				return targetObject;
			}
			set {
				targetObject = Expression.CheckNull(value);
			}
		}
		
		public List<Expression> Indices {
			get {
				return indices;
			}
			set {
				indices = value ?? new List<Expression>(1);
			}
		}
		
		public IndexerExpression(Expression targetObject, List<Expression> indices)
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
