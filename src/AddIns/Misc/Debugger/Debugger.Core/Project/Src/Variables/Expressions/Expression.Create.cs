// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 2285 $</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.PrettyPrinter;
using Ast = ICSharpCode.NRefactory.Ast;
using Debugger.Wrappers.CorSym;

namespace Debugger
{
	public partial class Expression: DebuggerObject
	{
		public Expression AppendIndexer(params int[] indices)
		{
			List<Ast.Expression> indicesAst = new List<Ast.Expression>();
			foreach(int indice in indices) {
				indicesAst.Add(new Ast.PrimitiveExpression(indice, indice.ToString()));
			}
			return new Ast.IndexerExpression(
				this.AbstractSynatxTree,
				indicesAst
			);
		}
		
		public Expression AppendFieldReference(FieldInfo fieldInfo)
		{
			return new Ast.FieldReferenceExpression(
				this.AbstractSynatxTree,
				fieldInfo
			);
		}
		
		public Expression AppendPropertyReference(PropertyInfo propertyInfo)
		{
			return new Ast.PropertyReferenceExpression(
				this.AbstractSynatxTree,
				propertyInfo
			);
		}
		
		/// <summary> Get all variables for a method - this; parameters; local variables </summary>
		public static ExpressionCollection MethodVariables(MethodInfo methodInfo)
		{
			ExpressionCollection vars = new ExpressionCollection();
			
			vars.Add(MethodThis(methodInfo));
			vars.AddRange(MethodParameters(methodInfo));
			vars.AddRange(MethodLocalVariables(methodInfo));
			
			return vars;
		}
		
		/// <summary> Get 'this' variable for a method </summary>
		public static Expression MethodThis(MethodInfo methodInfo)
		{
			if (methodInfo.IsStatic) throw new DebuggerException(methodInfo.FullName + " is static method");
			
			return new Ast.ThisReferenceExpression();
		}
		
		/// <summary> Get parameters of a method </summary>
		public static ExpressionCollection MethodParameters(MethodInfo methodInfo)
		{
			ExpressionCollection pars = new ExpressionCollection();
			
			for(int i = 0; i < methodInfo.ParameterCount; i++) {
				pars.Add(new Ast.ParameterIdentifierExpression(i, methodInfo.GetParameterName(i)));
			}
			
			return pars;
		}
		
		/// <summary> Get local variables of a method </summary>
		public static ExpressionCollection MethodLocalVariables(MethodInfo methodInfo)
		{
			ExpressionCollection vars = new ExpressionCollection();
			
			foreach(ISymUnmanagedVariable var in methodInfo.LocalVariables) {
				vars.Add(new Ast.LocalVariableIdentifierExpression(var));
			}
			
			return vars;
		}
		
		/// <summary>
		/// Evaluate the expression and return expressions for all array elements.
		/// The expression must evaluate to array.
		/// </summary>
		public ExpressionCollection EvaluateAndGetArrayElements()
		{
			ExpressionCollection elements = new ExpressionCollection();
			foreach(int[] indices in this.Evaluate().ArrayDimensions.Indices) {
				elements.Add(this.AppendIndexer(indices));
			}
			return elements;
		}
		
		/// <summary>
		/// Evaluate the expression and return object members.
		/// The expression must evaluate to object.
		/// </summary>
		public ExpressionCollection EvaluateAndGetObjectMembers(BindingFlags bindingFlags)
		{
			ExpressionCollection members = new ExpressionCollection();
			
			DebugType currentType = this.Evaluate().Type;
			while (currentType != null) {
				members.AddRange(GetObjectMembers(currentType, bindingFlags));
				currentType = currentType.BaseType;
			}
			
			return members;
		}
		
		public ExpressionCollection GetObjectMembers(DebugType type, BindingFlags bindingFlags)
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
	}
}
