// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation for IType interface.
	/// </summary>
	public abstract class AbstractType : AbstractFreezable, IType
	{
		public virtual string FullName {
			get {
				string ns = this.Namespace;
				string name = this.Name;
				if (string.IsNullOrEmpty(ns)) {
					return name;
				} else {
					string combinedName = ns + "." + name;
					Contract.Assume(!string.IsNullOrEmpty(combinedName));
					return combinedName;
				}
			}
		}
		
		public abstract string Name { get; }
		
		public virtual string Namespace {
			get { return string.Empty; }
		}
		
		public virtual string DotNetName {
			get { return this.FullName; }
		}
		
		public abstract bool? IsReferenceType { get; }
		
		public virtual bool IsArrayType {
			get { return false; }
		}
		
		public virtual bool IsPointerType {
			get { return false; }
		}
		
		public virtual int TypeParameterCount {
			get { return 0; }
		}
		
		public virtual IType DeclaringType {
			get { return null; }
		}
		
		public virtual IType GetElementType()
		{
			throw new InvalidOperationException();
		}
		
		public virtual ITypeDefinition GetDefinition()
		{
			return null;
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			return this;
		}
		
		public IType GetBaseType(ITypeResolveContext context)
		{
			return null;
		}
		
		public virtual IList<IType> GetNestedTypes(ITypeResolveContext context)
		{
			return EmptyList<IType>.Instance;
		}

		public virtual IList<IMethod> GetMethods(ITypeResolveContext context)
		{
			return EmptyList<IMethod>.Instance;
		}
		
		public virtual IList<IProperty> GetProperties(ITypeResolveContext context)
		{
			return EmptyList<IProperty>.Instance;
		}
		
		public virtual IList<IField> GetFields(ITypeResolveContext context)
		{
			return EmptyList<IField>.Instance;
		}
		
		public virtual IList<IEvent> GetEvents(ITypeResolveContext context)
		{
			return EmptyList<IEvent>.Instance;
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as IType);
		}
		
		public abstract override int GetHashCode();
		public abstract bool Equals(IType other);
		
		public override string ToString()
		{
			return this.DotNetName;
		}
	}
}
