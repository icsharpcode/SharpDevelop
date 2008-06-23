// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Collections.Generic;

namespace Debugger.Expressions
{
	/// <summary>
	/// Indexer to an element of an array.
	/// </summary>
	public class ArrayIndexerExpression: Expression
	{
		Expression targetObject;
		Expression[] arguments;
		string name;
		
		public Expression TargetObject {
			get { return targetObject; }
		}
		
		public Expression[] Arguments {
			get { return arguments; }
		}
		
		public ArrayIndexerExpression(Expression targetObject, params int[] indices)
		{
			if (targetObject == null) throw new ArgumentNullException("targetObject");
			if (indices == null) throw new ArgumentNullException("indices");
			
			this.targetObject = targetObject;
			
			List<Expression> indicesAst = new List<Expression>();
			foreach(int indice in indices) {
				indicesAst.Add(new PrimitiveExpression(indice));
			}
			this.arguments = indicesAst.ToArray();
			this.name = GetName();
		}
		
		public ArrayIndexerExpression(Expression targetObject, Expression[] arguments)
		{
			if (targetObject == null) throw new ArgumentNullException("targetObject");
			if (arguments == null) throw new ArgumentNullException("arguments");
			
			this.targetObject = targetObject;
			this.arguments = arguments;
			this.name = GetName();
		}
		
		string GetName()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			bool isFirst = true;
			foreach(Expression argument in arguments) {
				if (isFirst) {
					isFirst = false;
				} else {
					sb.Append(", ");
				}
				sb.Append(argument.Code);
			}
			sb.Append("]");
			return sb.ToString();
		}
		
		public override string Code {
			get {
				return targetObject.Code + this.CodeTail;
			}
		}
		
		public override string CodeTail {
			get {
				return name;
			}
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			Value targetValue = targetObject.Evaluate(context);
			
			List<int> indicies = new List<int>();
			foreach(Expression argument in arguments) {
				if (argument is PrimitiveExpression) {
					PrimitiveExpression primitiveExpression = (PrimitiveExpression)argument;
					if (primitiveExpression.Value is int) {
						indicies.Add((int)primitiveExpression.Value);
						continue;
					}
				}
				throw new GetValueException("Literal integer index expected");
			}
			
			return targetValue.GetArrayElement(indicies.ToArray());
		}
		
		#region GetHashCode and Equals
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (targetObject != null) hashCode += 1000000007 * targetObject.GetHashCode(); 
				if (arguments != null) hashCode += 1000000009 * GetArrayHashCode(arguments);
			}
			return hashCode;
		}
		
		public override bool Equals(object obj)
		{
			ArrayIndexerExpression other = obj as ArrayIndexerExpression;
			if (other == null) return false; 
			return
				object.Equals(this.targetObject, other.targetObject) &&
				ArrayEquals(this.arguments, other.arguments);
		}
		
		#endregion
	}
}
