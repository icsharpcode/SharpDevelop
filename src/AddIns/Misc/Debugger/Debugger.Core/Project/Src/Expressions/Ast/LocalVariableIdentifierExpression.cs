// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger.MetaData;
using Debugger.Wrappers.CorSym;

namespace Debugger.Expressions
{
	/// <summary>
	/// Identifier of a local variable within a given method.
	/// </summary>
	public class LocalVariableIdentifierExpression: Expression
	{
		MethodInfo method;
		ISymUnmanagedVariable symVar;
		string name;
		
		public MethodInfo Method {
			get { return method; }
		}
		
		public ISymUnmanagedVariable SymVar {
			get { return symVar; }
		}
		
		public LocalVariableIdentifierExpression(MethodInfo method, ISymUnmanagedVariable symVar)
		{
			if (method == null) throw new ArgumentNullException("method");
			if (symVar == null) throw new ArgumentNullException("symVar");
			
			this.method = method;
			this.symVar = symVar;
			this.name = symVar.Name;
		}
		
		public override string Code {
			get {
				return name;
			}
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			if (context.MethodInfo != method) {
				throw new GetValueException("Method " + method.FullName + " expected, " + context.MethodInfo.FullName + " seen");
			}
			
			return context.GetLocalVariableValue(symVar);
		}
		
		#region GetHashCode and Equals
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (method != null) hashCode += 1000000007 * method.GetHashCode(); 
				if (symVar != null) hashCode += 1000000009 * symVar.AddressField1.GetHashCode();
			}
			return hashCode;
		}
		
		public override bool Equals(object obj)
		{
			LocalVariableIdentifierExpression other = obj as LocalVariableIdentifierExpression;
			if (other == null) return false; 
			return
				object.Equals(this.method, other.method) && 
				object.Equals(this.symVar.AddressField1, other.symVar.AddressField1);
		}
		
		#endregion
	}
}
