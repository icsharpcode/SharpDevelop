using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST {
	
	public class ArrayInitializerExpression : Expression
	{
//		List<Expression> createExpressions = new List<Expression>(1);
		ArrayList createExpressions;
		
		public new static ArrayInitializerExpression Null {
			get {
				return NullArrayInitializerExpression.Instance;
			}
		}
		
		public static ArrayInitializerExpression CheckNull(ArrayInitializerExpression arrayInitializerExpression)
		{
			return arrayInitializerExpression == null ? NullArrayInitializerExpression.Instance : arrayInitializerExpression;
		}
		
		public ArrayList CreateExpressions {
			get {
				return createExpressions;
			}
			set {
				createExpressions = value == null ? new ArrayList(1) : value;
			}
		}
		
		public ArrayInitializerExpression()
		{
			createExpressions = new ArrayList(1);
		}
		
		public ArrayInitializerExpression(ArrayList createExpressions)
		{
			this.createExpressions = createExpressions;
		}
		
		
		
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ArrayInitializerExpression: CreateExpressions={0}]", 
			                     GetCollectionString(createExpressions));
		}
		
	}
	
	public class NullArrayInitializerExpression : ArrayInitializerExpression
	{
		static NullArrayInitializerExpression nullArrayInitializerExpression = new NullArrayInitializerExpression();
		
		public override bool IsNull {
			get {
				return true;
			}
		}
		
		public static NullArrayInitializerExpression Instance {
			get {
				return nullArrayInitializerExpression;
			}
		}
		
		NullArrayInitializerExpression()
		{
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return this;
		}
		
		public override string ToString()
		{
			return String.Format("[NullArrayInitializerExpression]");
		}
	}
	
	public class ArrayCreationParameter : Expression 
	{
//		List<Expression> expressions = new List<Expression>(1);
		ArrayList expressions = new ArrayList(1);
		
		int       dimensions  = -1;
		
		public bool IsExpressionList {
			get {
				return expressions != null;
			}
		}
		
		public ArrayList Expressions {
			get {
				return expressions;
			}
			set {
				expressions = value;
			}
		}
		
		public int Dimensions {
			get {
				return dimensions;
			}
			set {
				dimensions = value;
			}
		}
		
		public ArrayCreationParameter(ArrayList expressions)
		{
			this.expressions = expressions;
		}
		
		public ArrayCreationParameter(int dimensions)
		{
			this.dimensions = dimensions;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ArrayCreationParameter: Dimensions={0}, Expressions={1}",
			                     dimensions,
			                     GetCollectionString(expressions));
		}
	}
}
