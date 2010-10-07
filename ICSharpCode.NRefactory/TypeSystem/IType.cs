// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ICSharpCode.NRefactory.TypeSystem
{
	[ContractClass(typeof(ITypeContract))]
	public interface IType : ITypeReference, INamedElement, IEquatable<IType>
	{
		/// <summary>
		/// Gets whether the type is a reference type or value type.
		/// </summary>
		/// <returns>
		/// true, if the type is a reference type.
		/// false, if the type is a value type.
		/// null, if the type is not known (e.g. unconstrained generic type parameter or type not found)
		/// </returns>
		bool? IsReferenceType { get; }
		
		/// <summary>
		/// Gets the element type of array or pointer types.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// This type is not an array or pointer type.
		/// </exception>
		IType GetElementType();
		
		/// <summary>
		/// Gets the underlying type definition.
		/// Can return null for types which do not have a type definition (for example type parameters)
		/// </summary>
		ITypeDefinition GetDefinition();
		
		/// <summary>
		/// Gets the parent type, if this is a nested type.
		/// Returns null for top-level types.
		/// </summary>
		IType DeclaringType { get; }
		
		/// <summary>
		/// Gets the number of type parameters.
		/// </summary>
		int TypeParameterCount { get; }
		
		/// <summary>
		/// Calls ITypeVisitor.Visit for this type.
		/// </summary>
		IType AcceptVisitor(TypeVisitor visitor);
		
		/// <summary>
		/// Calls ITypeVisitor.Visit for all children of this type, and reconstructs this type with the children based
		/// by the return values of the visit calls.
		/// </summary>
		IType VisitChildren(TypeVisitor visitor);
	}
	
	[ContractClassFor(typeof(IType))]
	abstract class ITypeContract : ITypeReferenceContract, IType
	{
		Nullable<bool> IType.IsReferenceType {
			get { return null; }
		}
		
		int IType.TypeParameterCount {
			get {
				Contract.Ensures(Contract.Result<int>() >= 0);
				return 0;
			}
		}
		
		IType IType.DeclaringType {
			get { return null; }
		}
		
		string INamedElement.FullName {
			get {
				Contract.Ensures(Contract.Result<string>() != null);
				return null;
			}
		}
		
		string INamedElement.Name {
			get {
				Contract.Ensures(Contract.Result<string>() != null);
				return null;
			}
		}
		
		string INamedElement.Namespace {
			get {
				Contract.Ensures(Contract.Result<string>() != null);
				return null;
			}
		}
		
		string INamedElement.DotNetName {
			get {
				Contract.Ensures(Contract.Result<string>() != null);
				return null;
			}
		}
		
		IType IType.GetElementType()
		{
			Contract.Ensures(Contract.Result<IType>() != null);
			return null;
		}
		
		ITypeDefinition IType.GetDefinition()
		{
			return null;
		}
		
		bool IEquatable<IType>.Equals(IType other)
		{
			return false;
		}
		
		IType IType.AcceptVisitor(TypeVisitor visitor)
		{
			Contract.Requires(visitor != null);
			Contract.Ensures(Contract.Result<IType>() != null);
			return this;
		}
		
		IType IType.VisitChildren(TypeVisitor visitor)
		{
			Contract.Requires(visitor != null);
			Contract.Ensures(Contract.Result<IType>() != null);
			return this;
		}
	}
}
