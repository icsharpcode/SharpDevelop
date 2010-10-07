// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Represents a class, enum, interface, struct, delegate or VB module.
	/// </summary>
	[ContractClass(typeof(ITypeDefinitionContract))]
	public interface ITypeDefinition : IType, IEntity
	{
		ClassType ClassType { get; }
		
		IList<ITypeReference> BaseTypes { get; }
		IList<ITypeParameter> TypeParameters { get; }
		
		/// <summary>
		/// If this is a partial class, gets the compound class containing information from all parts.
		/// If this is not a partial class, a reference to this class is returned.
		/// 
		/// This method will always retrieve the latest version of the class, which might not contain this class as a part.
		/// </summary>
		ITypeDefinition GetCompoundClass();
		
		/// <summary>
		/// If this is a compound class (combination of class parts), this method retrieves all individual class parts.
		/// Otherwise, a list containing <c>this</c> is returned.
		/// </summary>
		IList<ITypeDefinition> GetParts();
		
		IList<ITypeDefinition> InnerClasses { get; }
		IList<IField> Fields { get; }
		IList<IProperty> Properties { get; }
		IList<IMethod> Methods { get; }
		IList<IEvent> Events { get; }
		
		/// <summary>
		/// Gets all members declared in this class. This is the union of Fields,Properties,Methods and Events.
		/// </summary>
		IEnumerable<IMember> Members { get; }
	}
	
	[ContractClassFor(typeof(ITypeDefinition))]
	abstract class ITypeDefinitionContract : ITypeContract, ITypeDefinition
	{
		ClassType ITypeDefinition.ClassType {
			get { return default(ClassType); }
		}
		
		IList<ITypeReference> ITypeDefinition.BaseTypes {
			get {
				Contract.Ensures(Contract.Result<IList<ITypeReference>>() != null);
				return null;
			}
		}
		
		IList<ITypeParameter> ITypeDefinition.TypeParameters {
			get {
				Contract.Ensures(Contract.Result<IList<ITypeParameter>>() != null);
				return null;
			}
		}
		
		IList<ITypeDefinition> ITypeDefinition.InnerClasses {
			get {
				Contract.Ensures(Contract.Result<IList<ITypeDefinition>>() != null);
				return null;
			}
		}
		
		IList<IField> ITypeDefinition.Fields {
			get {
				Contract.Ensures(Contract.Result<IList<IField>>() != null);
				return null;
			}
		}
		
		IList<IProperty> ITypeDefinition.Properties {
			get {
				Contract.Ensures(Contract.Result<IList<IProperty>>() != null);
				return null;
			}
		}
		
		IList<IMethod> ITypeDefinition.Methods {
			get {
				Contract.Ensures(Contract.Result<IList<IMethod>>() != null);
				return null;
			}
		}
		
		IList<IEvent> ITypeDefinition.Events {
			get {
				Contract.Ensures(Contract.Result<IList<IEvent>>() != null);
				return null;
			}
		}
		
		IEnumerable<IMember> ITypeDefinition.Members {
			get {
				Contract.Ensures(Contract.Result<IEnumerable<IMember>>() != null);
				return null;
			}
		}
		
		ITypeDefinition ITypeDefinition.GetCompoundClass()
		{
			Contract.Ensures(Contract.Result<ITypeDefinition>() != null);
			return null;
		}
		
		IList<ITypeDefinition> ITypeDefinition.GetParts()
		{
			Contract.Ensures(Contract.Result<IList<ITypeDefinition>>() != null);
			return null;
		}
		
		#region IEntity
		EntityType IEntity.EntityType {
			get {
				throw new NotImplementedException();
			}
		}
		
		DomRegion IEntity.Region {
			get {
				throw new NotImplementedException();
			}
		}
		
		DomRegion IEntity.BodyRegion {
			get {
				throw new NotImplementedException();
			}
		}
		
		ITypeDefinition IEntity.DeclaringTypeDefinition {
			get {
				throw new NotImplementedException();
			}
		}
		
		IList<IAttribute> IEntity.Attributes {
			get {
				throw new NotImplementedException();
			}
		}
		
		string IEntity.Documentation {
			get {
				throw new NotImplementedException();
			}
		}
		
		bool IEntity.IsStatic {
			get {
				throw new NotImplementedException();
			}
		}
		
		Accessibility IEntity.Accessibility {
			get {
				throw new NotImplementedException();
			}
		}
		
		bool IEntity.IsAbstract {
			get {
				throw new NotImplementedException();
			}
		}
		
		bool IEntity.IsSealed {
			get {
				throw new NotImplementedException();
			}
		}
		
		bool IEntity.IsShadowing {
			get {
				throw new NotImplementedException();
			}
		}
		
		bool IEntity.IsSynthetic {
			get {
				throw new NotImplementedException();
			}
		}
		
		IProjectContent IEntity.ProjectContent {
			get {
				throw new NotImplementedException();
			}
		}
		#endregion
	}
}
