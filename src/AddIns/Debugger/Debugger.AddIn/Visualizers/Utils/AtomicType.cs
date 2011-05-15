// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
			return type.IsPrimitive || type.FullName == "System.String" || type.IsEnum();
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
