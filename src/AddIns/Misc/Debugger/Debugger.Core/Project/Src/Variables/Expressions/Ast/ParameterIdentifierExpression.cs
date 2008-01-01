// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 2285 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Ast
{
	/// <summary>
	/// Identifier of a method parameter
	/// </summary>
	public class ParameterIdentifierExpression: IdentifierExpression
	{
		int parameterIndex;
		
		public int ParameterIndex {
			get { return parameterIndex; }
		}
		
		public ParameterIdentifierExpression(int parameterIndex, string identifier)
			:base (identifier)
		{
			this.parameterIndex = parameterIndex;
		}
		
		public override string ToString() {
			return string.Format("[ParameterIdentifierExpression Index={0} Identifier={1} TypeArguments={2}]", ParameterIndex, Identifier, GetCollectionString(TypeArguments));
		}
	}
}
