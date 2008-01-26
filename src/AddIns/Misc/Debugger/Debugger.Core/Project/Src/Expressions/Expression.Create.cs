// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.MetaData;
using Debugger.Wrappers.CorSym;

namespace Debugger.Expressions
{
	public partial class Expression: DebuggerObject
	{
		public Expression AppendIndexer(params int[] indices)
		{
			return new ArrayIndexerExpression(this, indices);
		}
		
		public Expression[] AppendIndexers(ArrayDimensions dimensions)
		{
			List<Expression> elements = new List<Expression>();
			foreach(int[] indices in dimensions.Indices) {
				elements.Add(this.AppendIndexer(indices));
			}
			return elements.ToArray();
		}
		
		public Expression AppendFieldReference(FieldInfo fieldInfo)
		{
			return AppendMemberReference(fieldInfo);
		}
		
		public Expression AppendPropertyReference(PropertyInfo propertyInfo)
		{
			return AppendMemberReference(propertyInfo);
		}
		
		public Expression AppendMemberReference(MemberInfo memberInfo, params Expression[] args)
		{
			return new MemberReferenceExpression(this, memberInfo, args);
		}
		
		public Expression[] AppendObjectMembers(DebugType type, BindingFlags bindingFlags)
		{
			List<Expression> members = new List<Expression>();
			
			foreach(FieldInfo field in type.GetFields(bindingFlags)) {
				members.Add(this.AppendFieldReference(field));
			}
			foreach(PropertyInfo property in type.GetProperties(bindingFlags)) {
				members.Add(this.AppendPropertyReference(property));
			}
			
			return members.ToArray();
		}
		
		/// <summary> Get all variables for a method - this; parameters; local variables </summary>
		public static Expression[] MethodVariables(MethodInfo methodInfo)
		{
			List<Expression> vars = new List<Expression>();
			
			if (!methodInfo.IsStatic) {
				vars.Add(MethodThis());
			}
			vars.AddRange(MethodParameters(methodInfo));
			vars.AddRange(MethodLocalVariables(methodInfo));
			
			return vars.ToArray();
		}
		
		/// <summary> Get 'this' variable for a method </summary>
		public static Expression MethodThis()
		{
			return new ThisReferenceExpression();
		}
		
		/// <summary> Get parameters of a method </summary>
		public static Expression[] MethodParameters(MethodInfo methodInfo)
		{
			List<Expression> pars = new List<Expression>();
			
			for(int i = 0; i < methodInfo.ParameterCount; i++) {
				pars.Add(new ParameterIdentifierExpression(methodInfo, i));
			}
			
			return pars.ToArray();
		}
		
		/// <summary> Get local variables of a method </summary>
		public static Expression[] MethodLocalVariables(MethodInfo methodInfo)
		{
			List<Expression> vars = new List<Expression>();
			
			foreach(ISymUnmanagedVariable var in methodInfo.LocalVariables) {
				vars.Add(new LocalVariableIdentifierExpression(methodInfo, var));
			}
			
			return vars.ToArray();
		}
	}
}
