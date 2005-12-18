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
	public enum CastType
	{
		/// <summary>
		/// direct cast (C#, VB "DirectCast")
		/// </summary>
		Cast,
		/// <summary>
		/// try cast (C# "as", VB "TryCast")
		/// </summary>
		TryCast,
		/// <summary>
		/// converting cast (VB "CType")
		/// </summary>
		Conversion,
		/// <summary>
		/// primitive converting cast (VB "CString" etc.)
		/// </summary>
		PrimitiveConversion
	}
	
	public class CastExpression : Expression
	{
		TypeReference castTo;
		Expression    expression;
		CastType      castType;
		
		public TypeReference CastTo {
			get {
				return castTo;
			}
			set {
				castTo = TypeReference.CheckNull(value);
			}
		}
		
		public Expression Expression {
			get {
				return expression;
			}
			set {
				expression = Expression.CheckNull(value);
			}
		}
		
		public CastType CastType {
			get {
				return castType;
			}
			set {
				castType = value;
			}
		}
		
		public CastExpression(TypeReference castTo)
		{
			this.CastTo     = castTo;
			this.expression = Expression.Null;
		}
		
		public CastExpression(TypeReference castTo, Expression expression)
		{
			this.CastTo     = castTo;
			this.Expression = expression;
		}
		
		public CastExpression(TypeReference castTo, Expression expression, CastType castType)
		{
			this.CastTo     = castTo;
			this.Expression = expression;
			this.castType = castType;
		}
		
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[CastExpression: CastTo={0}, Expression={1} CastType={2}]",
			                     castTo,
			                     expression,
			                     castType);
		}
	}
}
