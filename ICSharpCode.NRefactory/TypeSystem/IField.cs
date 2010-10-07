// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Diagnostics.Contracts;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Represents a field or constant.
	/// </summary>
	[ContractClass(typeof(IFieldContract))]
	public interface IField : IMember, IVariable
	{
		/// <summary>
		/// Gets the name of the field.
		/// </summary>
		new string Name { get; } // solve ambiguity between INamedElement.Name and IVariable.Name
		
		/// <summary>
		/// Gets whether this field is a constant (C#-like const).
		/// </summary>
		bool IsConst { get; }
		
		/// <summary>
		/// Gets whether this field is readonly.
		/// </summary>
		bool IsReadOnly { get; }
		
		/// <summary>
		/// Gets whether this field is volatile.
		/// </summary>
		bool IsVolatile { get; }
		
		/// <summary>
		/// If this field is a constant, retrieves the value.
		/// </summary>
		IConstantValue ConstantValue { get; }
	}
	
	[ContractClassFor(typeof(IField))]
	abstract class IFieldContract : IMemberContract, IField
	{
		string IField.Name {
			get {
				Contract.Ensures(Contract.Result<string>() != null);
				return null;
			}
		}
		
		bool IField.IsConst {
			get {
				IField @this = this;
				Contract.Ensures(Contract.Result<bool>() == (@this.ConstantValue != null));
				return false;
			}
		}
		
		bool IField.IsReadOnly {
			get { return false; }
		}
		
		bool IField.IsVolatile {
			get { return false; }
		}
		
		IConstantValue IField.ConstantValue {
			get { return null; }
		}
		
		string IVariable.Name {
			get { return null;  }
		}
		
		ITypeReference IVariable.Type {
			get { return null; }
		}
	}
}
