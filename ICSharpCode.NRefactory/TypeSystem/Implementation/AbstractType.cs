// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

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
		
		public virtual int TypeParameterCount {
			get { return 0; }
		}
		
		public virtual IType DeclaringType {
			get { return null; }
		}
		
		public virtual ITypeDefinition GetDefinition()
		{
			return null;
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			return this;
		}
		
		public virtual IEnumerable<IType> GetBaseTypes(ITypeResolveContext context)
		{
			return EmptyList<IType>.Instance;
		}
		
		public virtual IEnumerable<IType> GetNestedTypes(ITypeResolveContext context)
		{
			return EmptyList<IType>.Instance;
		}

		public virtual IEnumerable<IMethod> GetMethods(ITypeResolveContext context)
		{
			return EmptyList<IMethod>.Instance;
		}
		
		public virtual IEnumerable<IMethod> GetConstructors(ITypeResolveContext context)
		{
			return EmptyList<IMethod>.Instance;
		}
		
		public virtual IEnumerable<IProperty> GetProperties(ITypeResolveContext context)
		{
			return EmptyList<IProperty>.Instance;
		}
		
		public virtual IEnumerable<IField> GetFields(ITypeResolveContext context)
		{
			return EmptyList<IField>.Instance;
		}
		
		public virtual IEnumerable<IEvent> GetEvents(ITypeResolveContext context)
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
		
		public virtual IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitOtherType(this);
		}
		
		public virtual IType VisitChildren(TypeVisitor visitor)
		{
			return this;
		}
	}
}
