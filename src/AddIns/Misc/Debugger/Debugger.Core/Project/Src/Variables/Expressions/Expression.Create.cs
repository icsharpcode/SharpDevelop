// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorSym;

namespace Debugger.Expressions
{
	public partial class Expression: DebuggerObject
	{
		public Expression AppendIndexer(params int[] indices)
		{
			List<Expression> indicesAst = new List<Expression>();
			foreach(int indice in indices) {
				indicesAst.Add(new PrimitiveExpression(indice));
			}
			return new ArrayIndexerExpression(this, indicesAst.ToArray());
		}
		
		public ExpressionCollection AppendIndexers(ArrayDimensions dimensions)
		{
			ExpressionCollection elements = new ExpressionCollection();
			foreach(int[] indices in dimensions.Indices) {
				elements.Add(this.AppendIndexer(indices));
			}
			return elements;
		}
		
		public Expression AppendFieldReference(FieldInfo fieldInfo)
		{
			return new MemberReferenceExpression(this, fieldInfo, null);
		}
		
		public Expression AppendPropertyReference(PropertyInfo propertyInfo)
		{
			return new MemberReferenceExpression(this, propertyInfo, null);
		}
		
		public ExpressionCollection AppendObjectMembers(DebugType type, BindingFlags bindingFlags)
		{
			ExpressionCollection members = new ExpressionCollection();
			
			foreach(FieldInfo field in type.GetFields(bindingFlags)) {
				members.Add(this.AppendFieldReference(field));
			}
			foreach(PropertyInfo property in type.GetProperties(bindingFlags)) {
				members.Add(this.AppendPropertyReference(property));
			}
			
			return members;
		}
		
		/// <summary> Get all variables for a method - this; parameters; local variables </summary>
		public static ExpressionCollection MethodVariables(MethodInfo methodInfo)
		{
			ExpressionCollection vars = new ExpressionCollection();
			
			if (!methodInfo.IsStatic) {
				vars.Add(MethodThis());
			}
			vars.AddRange(MethodParameters(methodInfo));
			vars.AddRange(MethodLocalVariables(methodInfo));
			
			return vars;
		}
		
		/// <summary> Get 'this' variable for a method </summary>
		public static Expression MethodThis()
		{
			return new ThisReferenceExpression();
		}
		
		/// <summary> Get parameters of a method </summary>
		public static ExpressionCollection MethodParameters(MethodInfo methodInfo)
		{
			ExpressionCollection pars = new ExpressionCollection();
			
			for(int i = 0; i < methodInfo.ParameterCount; i++) {
				pars.Add(new ParameterIdentifierExpression(methodInfo, i, methodInfo.GetParameterName(i)));
			}
			
			return pars;
		}
		
		/// <summary> Get local variables of a method </summary>
		public static ExpressionCollection MethodLocalVariables(MethodInfo methodInfo)
		{
			ExpressionCollection vars = new ExpressionCollection();
			
			foreach(ISymUnmanagedVariable var in methodInfo.LocalVariables) {
				vars.Add(new LocalVariableIdentifierExpression(methodInfo, var));
			}
			
			return vars;
		}
	}
}
