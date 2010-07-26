// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Type parameter of a generic class/method.
	/// </summary>
	[ContractClass(typeof(ITypeParameterContract))]
	public interface ITypeParameter : IFreezable
	{
		/// <summary>
		/// The name of the type parameter (for example "T")
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// Gets the index of the type parameter in the type parameter list of the owning method/class.
		/// </summary>
		int Index { get; }
		
		/// <summary>
		/// Gets the list of attributes declared on this type parameter.
		/// </summary>
		IList<IAttribute> Attributes { get; }
		
		/// <summary>
		/// The method this type parameter is defined for.
		/// This property is null when the type parameter is for a class.
		/// </summary>
		IMethod ParentMethod { get; }
		
		/// <summary>
		/// The class this type parameter is defined for.
		/// When the type parameter is defined for a method, this is the class containing
		/// that method.
		/// </summary>
		ITypeDefinition ParentClass { get; }
		
		/// <summary>
		/// Gets the contraints of this type parameter.
		/// </summary>
		IList<ITypeReference> Constraints { get; }
		
		/// <summary>
		/// Gets if the type parameter has the 'new()' constraint.
		/// </summary>
		bool HasConstructableConstraint { get; }
		
		/// <summary>
		/// Gets if the type parameter has the 'class' constraint.
		/// </summary>
		bool HasReferenceTypeConstraint { get; }
		
		/// <summary>
		/// Gets if the type parameter has the 'struct' constraint.
		/// </summary>
		bool HasValueTypeConstraint { get; }
		
		/// <summary>
		/// Gets the type that was used to bind this type parameter.
		/// This property returns null for generic methods/classes, it
		/// is non-null only for constructed versions of generic methods.
		/// </summary>
		IType BoundTo { get; }
		
		/// <summary>
		/// If this type parameter was bound, returns the unbound version of it.
		/// </summary>
		ITypeParameter UnboundTypeParameter { get; }
	}
	
	
	[ContractClassFor(typeof(ITypeParameter))]
	abstract class ITypeParameterContract : IFreezableContract, ITypeParameter
	{
		string ITypeParameter.Name {
			get {
				Contract.Ensures(Contract.Result<string>() != null);
				return null;
			}
		}
		
		int ITypeParameter.Index {
			get {
				Contract.Ensures(Contract.Result<int>() >= 0);
				return 0;
			}
		}
		
		IList<IAttribute> ITypeParameter.Attributes {
			get {
				Contract.Ensures(Contract.Result<IList<IAttribute>>() != null);
				return null;
			}
		}
		
		IMethod ITypeParameter.ParentMethod {
			get {
				return null;
			}
		}
		
		ITypeDefinition ITypeParameter.ParentClass {
			get {
				Contract.Ensures(Contract.Result<ITypeDefinition>() != null);
				return null;
			}
		}
		
		IList<ITypeReference> ITypeParameter.Constraints {
			get {
				Contract.Ensures(Contract.Result<IList<ITypeReference>>() != null);
				return null;
			}
		}
		
		bool ITypeParameter.HasConstructableConstraint {
			get { return false; }
		}
		
		bool ITypeParameter.HasReferenceTypeConstraint {
			get { return false; }
		}
		
		bool ITypeParameter.HasValueTypeConstraint {
			get { return false; }
		}
		
		IType ITypeParameter.BoundTo {
			get { return null; }
		}
		
		ITypeParameter ITypeParameter.UnboundTypeParameter {
			get {
				ITypeParameter @this = this;
				Contract.Ensures((Contract.Result<ITypeParameter>() != null) == (@this.BoundTo != null));
				return null;
			}
		}
	}
}
