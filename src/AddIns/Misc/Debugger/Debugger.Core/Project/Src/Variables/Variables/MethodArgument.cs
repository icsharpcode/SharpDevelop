// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorDebug;

using Ast = ICSharpCode.NRefactory.Ast;

namespace Debugger
{
	/// <summary>
	/// Represents an argument of a function. That is, it refers to
	/// the runtime value of function parameter.
	/// </summary>
	public class MethodArgument: NamedValue
	{
		int index;
		
		/// <summary>
		/// The index of the function parameter starting at 0.
		/// </summary>
		/// <remarks>
		/// The implicit 'this' is excluded. 
		/// </remarks>
		public int Index {
			get {
				return index;
			}
		}
		
		internal MethodArgument(string name,
		                        int index,
		                        Process process,
		                        IExpirable[] expireDependencies,
		                        IMutable[] mutateDependencies,
		                        CorValueGetter corValueGetter)
			:base (name,
			       process,
			       new Ast.IdentifierExpression(name),
			       expireDependencies,
			       mutateDependencies,
			       corValueGetter)
		{
			this.index = index;
		}
	}
}
