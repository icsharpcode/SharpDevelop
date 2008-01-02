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
		public Expression AppendIndexer(params uint[] indices)
		{
			List<Ast.Expression> indicesAst = new List<Ast.Expression>();
			foreach(uint indice in indices) {
				indicesAst.Add(new Ast.PrimitiveExpression((int)indice, ((int)indice).ToString()));
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
		
		public static ExpressionCollection StackFrameVariables(Function stackFrame)
		{
			throw new NotImplementedException();
		}
		
		public static Expression StackFrameThis(Function stackFrame)
		{
			throw new NotImplementedException();
		}
		
		public static ExpressionCollection StackFrameParameters(Function stackFrame)
		{
			throw new NotImplementedException();
		}
		
		public static ExpressionCollection StackFrameLocalVariables(Function stackFrame)
		{
			throw new NotImplementedException();
		}
		
		public ValueCollection GetArrayElements()
		{
			throw new NotImplementedException();
		}
		
		public ExpressionCollection GetObjectMembers()
		{
			throw new NotImplementedException();
		}
		
		public ExpressionCollection GetObjectMembers(BindingFlags bindingFlags)
		{
			throw new NotImplementedException();
		}
		
		public ExpressionCollection GetObjectMembers(DebugType type, BindingFlags bindingFlags)
		{
			throw new NotImplementedException();
		}
	}
}
