// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Debugger.MetaData;
using System;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.NRefactory.Ast;

namespace Debugger.AddIn.Visualizers.Utils
{
	/// <summary>
	/// Atomic type is a type that "cannot be expanded".
	/// </summary>
	public static class AtomicType
	{
		/// <summary>
		/// Checks whether given expression's type is atomic.
		/// </summary>
		/// <param name="expr">Expression.</param>
		/// <returns>True if expression's type is atomic, False otherwise.</returns>
		public static bool IsAtomic(this DebugType type)
		{
			return !(type.IsClass || type.IsValueType /* = struct in C# */);
		}
		
		/// <summary>
		/// Checks whether given expression's type is atomic.
		/// </summary>
		/// <param name="expr">Expression.</param>
		/// <returns>True if expression's type is atomic, False otherwise.</returns>
		public static bool IsOfAtomicType(this Expression expr)
		{
			DebugType typeOfValue = expr.Evaluate(WindowsDebugger.CurrentProcess).Type;
			return AtomicType.IsAtomic(typeOfValue);
		}
	}
}
