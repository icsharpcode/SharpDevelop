// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Diagnostics.Contracts;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Represents a variable (name/return type pair).
	/// </summary>
	[ContractClass(typeof(IVariableContract))]
	public interface IVariable
	{
		/// <summary>
		/// Gets the name of the variable.
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// Gets the type of the variable.
		/// </summary>
		ITypeReference Type { get; }
	}
	
	[ContractClassFor(typeof(IVariable))]
	abstract class IVariableContract : IVariable
	{
		string IVariable.Name {
			get {
				Contract.Ensures(Contract.Result<string>() != null);
				return null;
			}
		}
		
		ITypeReference IVariable.Type {
			get {
				Contract.Ensures(Contract.Result<ITypeReference>() != null);
				return null;
			}
		}
	}
}
