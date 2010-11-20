// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// Represents a local variable.
	/// </summary>
	public class VariableResolveResult : ResolveResult
	{
		readonly IVariable variable;
		
		public VariableResolveResult(IVariable variable, IType type)
			: base(type)
		{
			if (variable == null)
				throw new ArgumentNullException("variable");
			this.variable = variable;
		}
		
		public IVariable Variable {
			get { return variable; }
		}
		
		public bool IsParameter {
			get { return variable is IParameter; }
		}
		
		public override string ToString()
		{
			return string.Format("[VariableResolveResult {0}]", variable);
		}
	}
}
