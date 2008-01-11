// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorSym;

namespace ICSharpCode.NRefactory.Ast
{
	/// <summary>
	/// Identifier of a local variable
	/// </summary>
	public class LocalVariableIdentifierExpression: IdentifierExpression
	{
		ISymUnmanagedVariable symVar;
		
		public ISymUnmanagedVariable SymVar {
			get { return symVar; }
		}
		
		public LocalVariableIdentifierExpression(ISymUnmanagedVariable symVar)
			:base (symVar.Name)
		{
			this.symVar = symVar;
		}
		
		public override string ToString() {
			return string.Format("[LocalVariableIdentifierExpression Identifier={0} TypeArguments={1}]", Identifier, GetCollectionString(TypeArguments));
		}
	}
}
