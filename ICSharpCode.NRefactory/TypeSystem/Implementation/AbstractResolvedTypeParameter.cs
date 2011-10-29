// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	public abstract class AbstractResolvedTypeParameter : ITypeParameter
	{
		readonly EntityType ownerType;
		readonly IEntity owner;
		readonly int index;
		readonly string name;
		readonly IList<IAttribute> attributes;
		readonly DomRegion region;
		readonly VarianceModifier variance;
		
		protected AbstractResolvedTypeParameter(IEntity owner, int index, string name, VarianceModifier variance, IList<IAttribute> attributes, DomRegion region)
		{
			if (owner == null)
				throw new ArgumentNullException("owner");
			this.owner = owner;
			this.ownerType = owner.EntityType;
			this.index = index;
			this.name = name ?? ((this.OwnerType == EntityType.Method ? "!!" : "!") + index.ToString());
			this.attributes = attributes ?? EmptyList<IAttribute>.Instance;
			this.region = region;
			this.variance = variance;
		}
		
		protected AbstractResolvedTypeParameter(EntityType ownerType, int index, string name, VarianceModifier variance, IList<IAttribute> attributes, DomRegion region)
		{
			this.ownerType = ownerType;
			this.index = index;
			this.name = name ?? ((this.OwnerType == EntityType.Method ? "!!" : "!") + index.ToString());
			this.attributes = attributes ?? EmptyList<IAttribute>.Instance;
			this.region = region;
			this.variance = variance;
		}
		
		public EntityType OwnerType {
			get { return ownerType; }
		}
		
		public IEntity Owner {
			get { return owner; }
		}
		
		public int Index {
			get { return index; }
		}
		
		public IList<IAttribute> Attributes {
			get { return attributes; }
		}
		
		public VarianceModifier Variance {
			get { return variance; }
		}
		
		public DomRegion Region {
			get { return region; }
		}
		
		public ICompilation Compilation {
			get { return owner.Compilation; }
		}
		
		public IType EffectiveBaseClass {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<IType> EffectiveInterfaceSet {
			get {
				throw new NotImplementedException();
			}
		}
		
		public abstract bool HasDefaultConstructorConstraint { get; }
		public abstract bool HasReferenceTypeConstraint { get; }
		public abstract bool HasValueTypeConstraint { get; }
		
		public TypeKind Kind {
			get { return TypeKind.TypeParameter; }
		}
		
		public bool? IsReferenceType {
			get {
				if (this.HasValueTypeConstraint)
					return false;
				if (this.HasReferenceTypeConstraint)
					return true;
				
				// A type parameter is known to be a reference type if it has the reference type constraint
				// or its effective base class is not object or System.ValueType.
				IType effectiveBaseClass = this.EffectiveBaseClass;
				if (effectiveBaseClass.Kind == TypeKind.Class || effectiveBaseClass.Kind == TypeKind.Delegate) {
					if (effectiveBaseClass.Namespace == "System" && effectiveBaseClass.TypeParameterCount == 0) {
						switch (effectiveBaseClass.Name) {
							case "Object":
							case "ValueType":
							case "Enum":
								return null;
						}
					}
					return true;
				} else if (effectiveBaseClass.Kind == TypeKind.Struct || effectiveBaseClass.Kind == TypeKind.Enum) {
					return false;
				}
				return null;
			}
		}
		
		IType IType.DeclaringType {
			get { return null; }
		}
		
		int IType.TypeParameterCount {
			get { return 0; }
		}
		
		public abstract IEnumerable<IType> DirectBaseTypes { get; }
		
		public string Name {
			get { return name; }
		}
		
		string INamedElement.Namespace {
			get { return string.Empty; }
		}
		
		string INamedElement.FullName {
			get { return name; }
		}
		
		public string ReflectionName {
			get {
				return (this.OwnerType == EntityType.Method ? "``" : "`") + index.ToString();
			}
		}
		
		ITypeDefinition IType.GetDefinition()
		{
			return null;
		}
		
		public IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitTypeParameter(this);
		}
		
		public IType VisitChildren(TypeVisitor visitor)
		{
			return this;
		}
		
		public ITypeReference ToTypeReference()
		{
			return new TypeParameterReference(this.OwnerType, this.Index);
		}
		
		IEnumerable<IType> IType.GetNestedTypes(Predicate<ITypeDefinition> filter, GetMemberOptions options)
		{
			return EmptyList<IType>.Instance;
		}
		
		IEnumerable<IType> IType.GetNestedTypes(IList<IType> typeArguments, Predicate<ITypeDefinition> filter, GetMemberOptions options)
		{
			return EmptyList<IType>.Instance;
		}
		
		public IEnumerable<IMethod> GetConstructors(Predicate<IMethod> filter = null, GetMemberOptions options = GetMemberOptions.IgnoreInheritedMembers)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers) {
				if (this.HasDefaultConstructorConstraint || this.HasValueTypeConstraint) {
					throw new NotImplementedException();
					//DefaultMethod m = DefaultMethod.CreateDefaultConstructor(GetDummyClassForTypeParameter(constraints));
					//if (filter(m))
					//	return new [] { m };
				}
				return EmptyList<IMethod>.Instance;
			} else {
				return GetMembersHelper.GetConstructors(this, filter, options);
			}
		}
		
		public IEnumerable<IMethod> GetMethods(Predicate<IMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IMethod>.Instance;
			else
				return GetMembersHelper.GetMethods(this, FilterNonStatic(filter), options);
		}
		
		public IEnumerable<IMethod> GetMethods(IList<IType> typeArguments, Predicate<IMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IMethod>.Instance;
			else
				return GetMembersHelper.GetMethods(this, typeArguments, FilterNonStatic(filter), options);
		}
		
		public IEnumerable<IProperty> GetProperties(Predicate<IProperty> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IProperty>.Instance;
			else
				return GetMembersHelper.GetProperties(this, FilterNonStatic(filter), options);
		}
		
		public IEnumerable<IField> GetFields(Predicate<IField> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IField>.Instance;
			else
				return GetMembersHelper.GetFields(this, FilterNonStatic(filter), options);
		}
		
		public IEnumerable<IEvent> GetEvents(Predicate<IEvent> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IEvent>.Instance;
			else
				return GetMembersHelper.GetEvents(this, FilterNonStatic(filter), options);
		}
		
		public IEnumerable<IMember> GetMembers(Predicate<IMember> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IMember>.Instance;
			else
				return GetMembersHelper.GetMembers(this, FilterNonStatic(filter), options);
		}
		
		static Predicate<T> FilterNonStatic<T>(Predicate<T> filter) where T : class, IMember
		{
			if (filter == null)
				return member => !member.IsStatic;
			else
				return member => !member.IsStatic && filter(member);
		}
		
		public sealed override bool Equals(object obj)
		{
			return Equals(obj as IType);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public virtual bool Equals(IType other)
		{
			return this == other; // use reference equality for type parameters
		}
	}
	
	/*
	/// <summary>
	/// Base class for <see cref="ITypeParameter"/> implementations.
	/// </summary>
	[Serializable]
	public abstract class AbstractTypeParameter : AbstractFreezable, ITypeParameter
	{
		EntityType ownerType;
		int index;
		string name;
		
		IList<IAttribute> attributes;
		DomRegion region;
		VarianceModifier variance;
		
		protected override void FreezeInternal()
		{
			attributes = FreezeList(attributes);
			base.FreezeInternal();
		}
		
		protected AbstractTypeParameter(EntityType ownerType, int index, string name)
		{
			if (!(ownerType == EntityType.TypeDefinition || ownerType == EntityType.Method))
				throw new ArgumentException("owner must be a type or a method", "ownerType");
			if (index < 0)
				throw new ArgumentOutOfRangeException("index", index, "Value must not be negative");
			if (name == null)
				throw new ArgumentNullException("name");
			this.ownerType = ownerType;
			this.index = index;
			this.name = name;
		}
		
		public EntityType OwnerType {
			get { return ownerType; }
		}
		
		public int Index {
			get { return index; }
		}
		
		public TypeKind Kind {
			get { return TypeKind.TypeParameter; }
		}
		
		public string Name {
			get { return name; }
		}
		
		string INamedElement.FullName {
			get { return name; }
		}
		
		string INamedElement.Namespace {
			get { return string.Empty; }
		}
		
		public string ReflectionName {
			get {
				if (this.OwnerType == EntityType.Method)
					return "``" + index.ToString();
				else
					return "`" + index.ToString();
			}
		}
		
		public abstract bool? IsReferenceType { get; }
		
		protected bool? IsReferenceTypeHelper(IType effectiveBaseClass)
		{
			// A type parameter is known to be a reference type if it has the reference type constraint
			// or its effective base class is not object or System.ValueType.
			if (effectiveBaseClass.Kind == TypeKind.Class || effectiveBaseClass.Kind == TypeKind.Delegate) {
				if (effectiveBaseClass.Namespace == "System" && effectiveBaseClass.TypeParameterCount == 0) {
					switch (effectiveBaseClass.Name) {
						case "Object":
						case "ValueType":
						case "Enum":
							return null;
					}
				}
				return true;
			} else if (effectiveBaseClass.Kind == TypeKind.Struct || effectiveBaseClass.Kind == TypeKind.Enum) {
				return false;
			}
			return null;
		}
		
		int IType.TypeParameterCount {
			get { return 0; }
		}
		
		IType IType.DeclaringType {
			get { return null; }
		}
		
		ITypeDefinition IType.GetDefinition()
		{
			return null;
		}
		
		public virtual bool Equals(IType other)
		{
			// Use reference equality for type parameters. While we could consider any types with same
			// ownerType + index as equal for the type system, doing so makes it difficult to cache calculation
			// results based on types - e.g. the cache in the Conversions class.
			return this == other;
			// We can still consider type parameters of different methods/classes to be equal to each other,
			// if they have been interned. But then also all constraints are equal, so caching conversions
			// are valid in that case.
		}
		
		public IList<IAttribute> Attributes {
			get {
				if (attributes == null)
					attributes = new List<IAttribute>();
				return attributes;
			}
		}
		
		public VarianceModifier Variance {
			get { return variance; }
			set {
				CheckBeforeMutation();
				variance = value;
			}
		}
		
		public DomRegion Region {
			get { return region; }
			set {
				CheckBeforeMutation();
				region = value;
			}
		}
		
		public IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitTypeParameter(this);
		}
		
		public IType VisitChildren(TypeVisitor visitor)
		{
			return this;
		}
		
		static readonly SimpleProjectContent dummyProjectContent = new SimpleProjectContent();
		
		DefaultTypeDefinition GetDummyClassForTypeParameter(ITypeParameterConstraints constraints)
		{
			DefaultTypeDefinition c = new DefaultTypeDefinition(dummyProjectContent, string.Empty, this.Name);
			c.Region = this.Region;
			if (constraints.HasValueTypeConstraint) {
				c.Kind = TypeKind.Struct;
			} else if (constraints.HasDefaultConstructorConstraint) {
				c.Kind = TypeKind.Class;
			} else {
				c.Kind = TypeKind.Interface;
			}
			return c;
		}
		
		IEnumerable<IType> IType.GetNestedTypes(ITypeResolveContext context, Predicate<ITypeDefinition> filter, GetMemberOptions options)
		{
			return EmptyList<IType>.Instance;
		}
		
		IEnumerable<IType> IType.GetNestedTypes(IList<IType> typeArguments, ITypeResolveContext context, Predicate<ITypeDefinition> filter, GetMemberOptions options)
		{
			return EmptyList<IType>.Instance;
		}
		
		public abstract IType GetEffectiveBaseClass(ITypeResolveContext context);
		
		public abstract IEnumerable<IType> GetEffectiveInterfaceSet(ITypeResolveContext context);
		
		public abstract ITypeParameterConstraints GetConstraints(ITypeResolveContext context);
		
		public virtual IEnumerable<IType> GetBaseTypes(ITypeResolveContext context)
		{
			ITypeParameterConstraints constraints = GetConstraints(context);
			bool hasNonInterfaceConstraint = false;
			foreach (IType c in constraints) {
				yield return c;
				if (c.Kind != TypeKind.Interface)
					hasNonInterfaceConstraint = true;
			}
			// Do not add the 'System.Object' constraint if there is another constraint with a base class.
			if (constraints.HasValueTypeConstraint || !hasNonInterfaceConstraint) {
				IType defaultBaseType = context.GetTypeDefinition("System", constraints.HasValueTypeConstraint ? "ValueType" : "Object", 0, StringComparer.Ordinal);
				if (defaultBaseType != null)
					yield return defaultBaseType;
			}
		}
		
		protected virtual void PrepareForInterning(IInterningProvider provider)
		{
			name = provider.Intern(name);
			attributes = provider.InternList(attributes);
		}
		
		protected virtual int GetHashCodeForInterning()
		{
			unchecked {
				int hashCode = index + name.GetHashCode();
				if (ownerType == EntityType.Method)
					hashCode += 7613561;
				if (attributes != null)
					hashCode += attributes.GetHashCode();
				hashCode += 900103 * (int)variance;
				return hashCode;
			}
		}
		
		protected bool EqualsForInterning(AbstractTypeParameter other)
		{
			return other != null
				&& this.attributes == other.attributes
				&& this.name == other.name
				&& this.ownerType == other.ownerType
				&& this.index == other.index
				&& this.variance == other.variance;
		}
		
		public override string ToString()
		{
			return this.name;
		}
	}
	 */
}
