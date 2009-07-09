// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Debugger.MetaData;
using System;
using Debugger.Expressions;
using ICSharpCode.SharpDevelop.Services;

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
		public static bool IsOfAtomicType(this Expression expr)
		{
			DebugType typeOfValue = expr.Evaluate(WindowsDebugger.CurrentProcess).Type;
			return !(typeOfValue.IsClass || typeOfValue.IsValueType /* = struct in C# */);
		}
	}
}
