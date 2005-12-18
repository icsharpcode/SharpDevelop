// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST {
	
	public class ArrayInitializerExpression : Expression
	{
		List<Expression> createExpressions;
		
		public new static ArrayInitializerExpression Null {
			get {
				return NullArrayInitializerExpression.Instance;
			}
		}
		
		public static ArrayInitializerExpression CheckNull(ArrayInitializerExpression arrayInitializerExpression)
		{
			return arrayInitializerExpression ?? NullArrayInitializerExpression.Instance;
		}
		
		public List<Expression> CreateExpressions {
			get {
				return createExpressions;
			}
			set {
				createExpressions = value ?? new List<Expression>(1);
			}
		}
		
		public ArrayInitializerExpression()
		{
			createExpressions = new List<Expression>(1);
		}
		
		public ArrayInitializerExpression(List<Expression> createExpressions)
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
}
