// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// ParameterizedType represents an instance of a generic type.
	/// Example: List&lt;string&gt;
	/// </summary>
	/// <remarks>
	/// When getting the members, this type modifies the lists so that
	/// type parameters in the signatures of the members are replaced with
	/// the type arguments.
	/// </remarks>
	public sealed class ParameterizedType : Immutable, IType
	{
		sealed class Substitution : TypeVisitor
		{
			readonly IType[] typeArguments;
			
			public Substitution(IType[] typeArguments)
			{
				this.typeArguments = typeArguments;
			}
			
			public override IType VisitTypeParameter(ITypeParameter type)
			{
				int index = type.Index;
				if (type.OwnerType == EntityType.TypeDefinition) {
					if (index >= 0 && index < typeArguments.Length)
						return typeArguments[index];
					else
						return SharedTypes.UnknownType;
				} else {
					return base.VisitTypeParameter(type);
				}
			}
			
			public override string ToString()
			{
				StringBuilder b = new StringBuilder();
				b.Append('[');
				for (int i = 0; i < typeArguments.Length; i++) {
					if (i > 0) b.Append(", ");
					b.Append('`');
					b.Append(i);
					b.Append(" -> ");
					b.Append(typeArguments[i]);
				}
				b.Append(']');
				return b.ToString();
			}
		}
		
		readonly ITypeDefinition genericType;
		readonly IType[] typeArguments;
		
		public ParameterizedType(ITypeDefinition genericType, IEnumerable<IType> typeArguments)
		{
			if (genericType == null)
				throw new ArgumentNullException("genericType");
			if (typeArguments == null)
				throw new ArgumentNullException("typeArguments");
			this.genericType = genericType;
			this.typeArguments = typeArguments.ToArray(); // copy input array to ensure it isn't modified
			if (this.typeArguments.Length == 0)
				throw new ArgumentException("Cannot use ParameterizedType with 0 type arguments.");
			if (genericType.TypeParameterCount != this.typeArguments.Length)
				throw new ArgumentException("Number of type arguments must match the type definition's number of type parameters");
			for (int i = 0; i < this.typeArguments.Length; i++) {
				if (this.typeArguments[i] == null)
					throw new ArgumentNullException("typeArguments[" + i + "]");
			}
		}
		
		/// <summary>
		/// Fast internal version of the constructor. (no safety checks)
		/// Keeps the array that was passed and assumes it won't be modified.
		/// </summary>
		internal ParameterizedType(ITypeDefinition genericType, IType[] typeArguments)
		{
			Debug.Assert(genericType.TypeParameterCount == typeArguments.Length);
			this.genericType = genericType;
			this.typeArguments = typeArguments;
		}
		
		public TypeKind Kind {
			get { return genericType.Kind; }
		}
		
		public bool? IsReferenceType(ITypeResolveContext context)
		{
			return genericType.IsReferenceType(context);
		}
		
		public IType DeclaringType {
			get {
				ITypeDefinition declaringTypeDef = genericType.DeclaringTypeDefinition;
				if (declaringTypeDef != null && declaringTypeDef.TypeParameterCount > 0
				    && declaringTypeDef.TypeParameterCount <= genericType.TypeParameterCount)
				{
					IType[] newTypeArgs = new IType[declaringTypeDef.TypeParameterCount];
					Array.Copy(this.typeArguments, 0, newTypeArgs, 0, newTypeArgs.Length);
					return new ParameterizedType(declaringTypeDef, newTypeArgs);
				}
				return declaringTypeDef;
			}
		}
		
		public int TypeParameterCount {
			get { return genericType.TypeParameterCount; }
		}
		
		public string FullName {
			get { return genericType.FullName; }
		}
		
		public string Name {
			get { return genericType.Name; }
		}
		
		public string Namespace {
			get { return genericType.Namespace;}
		}
		
		public string ReflectionName {
			get {
				StringBuilder b = new StringBuilder(genericType.ReflectionName);
				b.Append('[');
				for (int i = 0; i < typeArguments.Length; i++) {
					if (i > 0)
						b.Append(',');
					b.Append('[');
					b.Append(typeArguments[i].ReflectionName);
					b.Append(']');
				}
				b.Append(']');
				return b.ToString();
			}
		}
		
		public override string ToString()
		{
			return ReflectionName;
		}
		
		public ReadOnlyCollection<IType> TypeArguments {
			get {
				return Array.AsReadOnly(typeArguments);
			}
		}
		
		public ITypeDefinition GetDefinition()
		{
			return genericType;
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			return this;
		}
		
		/// <summary>
		/// Substitutes the class type parameters in the <paramref name="type"/> with the
		/// type arguments of this parameterized type.
		/// </summary>
		public IType SubstituteInType(IType type)
		{
			return type.AcceptVisitor(new Substitution(typeArguments));
		}
		
		/// <summary>
		/// Gets a type visitor that performs the substitution of class type parameters with the type arguments
		/// of this parameterized type.
		/// </summary>
		public TypeVisitor GetSubstitution()
		{
			return new Substitution(typeArguments);
		}
		
		public IEnumerable<IType> GetBaseTypes(ITypeResolveContext context)
		{
			Substitution substitution = new Substitution(typeArguments);
			return genericType.GetBaseTypes(context).Select(t => t.AcceptVisitor(substitution));
		}
		
		public IEnumerable<IType> GetNestedTypes(ITypeResolveContext context, Predicate<ITypeDefinition> filter = null)
		{
			return GetNestedTypes(this, context, filter);
		}
		
		internal static IEnumerable<IType> GetNestedTypes(IType type, ITypeResolveContext context, Predicate<ITypeDefinition> filter)
		{
			return type.GetNonInterfaceBaseTypes(context).SelectMany(t => GetNestedTypesInternal(t, context, filter));
		}
		
		static IEnumerable<IType> GetNestedTypesInternal(IType baseType, ITypeResolveContext context, Predicate<ITypeDefinition> filter)
		{
			ITypeDefinition baseTypeDef = baseType.GetDefinition();
			if (baseTypeDef == null)
				yield break;
			baseTypeDef = baseTypeDef.GetCompoundClass();
			int outerTypeParameterCount = baseTypeDef.TypeParameterCount;
			ParameterizedType pt = baseType as ParameterizedType;
			foreach (ITypeDefinition nestedType in baseTypeDef.NestedTypes) {
				if (!(filter == null || filter(nestedType)))
					continue;
				int innerTypeParameterCount = nestedType.TypeParameterCount;
				if (innerTypeParameterCount == 0 || (pt == null && innerTypeParameterCount == outerTypeParameterCount)) {
					// The nested type has no new type parameters, and there are no type arguments
					// to copy from the outer type
					// -> we can directly return the nested type definition
					yield return nestedType;
				} else {
					// We need to parameterize the nested type
					IType[] newTypeArguments = new IType[innerTypeParameterCount];
					for (int i = 0; i < outerTypeParameterCount; i++) {
						newTypeArguments[i] = pt != null ? pt.typeArguments[i] : baseTypeDef.TypeParameters[i];
					}
					for (int i = outerTypeParameterCount; i < innerTypeParameterCount; i++) {
						newTypeArguments[i] = SharedTypes.UnboundTypeArgument;
					}
					yield return new ParameterizedType(nestedType, newTypeArguments);
				}
			}
		}
		
		public IEnumerable<IMethod> GetConstructors(ITypeResolveContext context, Predicate<IMethod> filter = null)
		{
			Substitution substitution = new Substitution(typeArguments);
			Func<ITypeReference, ITypeReference> substitutionFunc = t => t.Resolve(context).AcceptVisitor(substitution);
			List<IMethod> methods = genericType.GetConstructors(context, filter).ToList();
			for (int i = 0; i < methods.Count; i++) {
				SpecializedMethod m = new SpecializedMethod(methods[i]);
				m.SetDeclaringType(this);
				m.SubstituteTypes(substitutionFunc);
				methods[i] = m;
			}
			return methods;
		}
		
		public IEnumerable<IMethod> GetMethods(ITypeResolveContext context, Predicate<IMethod> filter = null)
		{
			return GetMethods(this, context, filter);
		}
		
		internal static IEnumerable<IMethod> GetMethods(IType type, ITypeResolveContext context, Predicate<IMethod> filter)
		{
			Predicate<IMethod> newFilter;
			if (filter == null)
				newFilter = m => !m.IsConstructor;
			else
				newFilter = m => !m.IsConstructor && filter(m);
			return type.GetNonInterfaceBaseTypes(context).SelectMany(t => GetMethodsInternal(t, context, newFilter));
		}
		
		static IEnumerable<IMethod> GetMethodsInternal(IType baseType, ITypeResolveContext context, Predicate<IMethod> filter)
		{
			ITypeDefinition baseTypeDef = baseType.GetDefinition();
			if (baseTypeDef == null)
				yield break;
			baseTypeDef = baseTypeDef.GetCompoundClass();
			ParameterizedType pt = baseType as ParameterizedType;
			if (pt != null) {
				Substitution substitution = null;
				Func<ITypeReference, ITypeReference> substitutionFunc = null;
				foreach (IMethod m in baseTypeDef.Methods) {
					if (!(filter == null || filter(m)))
						continue;
					if (substitution == null) {
						substitution = new Substitution(pt.typeArguments);
						substitutionFunc = t => t.Resolve(context).AcceptVisitor(substitution);
					}
					SpecializedMethod sm = new SpecializedMethod(m);
					sm.SetDeclaringType(pt);
					sm.SubstituteTypes(substitutionFunc);
					yield return sm;
				}
			} else {
				foreach (IMethod m in baseTypeDef.Methods) {
					if (filter == null || filter(m))
						yield return m;
				}
			}
		}
		
		public IEnumerable<IProperty> GetProperties(ITypeResolveContext context, Predicate<IProperty> filter = null)
		{
			return GetProperties(this, context, filter);
		}
		
		internal static IEnumerable<IProperty> GetProperties(IType type, ITypeResolveContext context, Predicate<IProperty> filter)
		{
			return type.GetNonInterfaceBaseTypes(context).SelectMany(t => GetPropertiesInternal(t, context, filter));
		}
		
		static IEnumerable<IProperty> GetPropertiesInternal(IType baseType, ITypeResolveContext context, Predicate<IProperty> filter)
		{
			ITypeDefinition baseTypeDef = baseType.GetDefinition();
			if (baseTypeDef == null)
				yield break;
			baseTypeDef = baseTypeDef.GetCompoundClass();
			ParameterizedType pt = baseType as ParameterizedType;
			if (pt != null) {
				Substitution substitution = null;
				Func<ITypeReference, ITypeReference> substitutionFunc = null;
				foreach (IProperty p in baseTypeDef.Properties) {
					if (!(filter == null || filter(p)))
						continue;
					if (substitution == null) {
						substitution = new Substitution(pt.typeArguments);
						substitutionFunc = t => t.Resolve(context).AcceptVisitor(substitution);
					}
					SpecializedProperty sp = new SpecializedProperty(p);
					sp.SetDeclaringType(pt);
					sp.SubstituteTypes(substitutionFunc);
					yield return sp;
				}
			} else {
				foreach (IProperty p in baseTypeDef.Properties) {
					if (filter == null || filter(p))
						yield return p;
				}
			}
		}
		
		public IEnumerable<IField> GetFields(ITypeResolveContext context, Predicate<IField> filter = null)
		{
			return GetFields(this, context, filter);
		}
		
		internal static IEnumerable<IField> GetFields(IType type, ITypeResolveContext context, Predicate<IField> filter)
		{
			return type.GetNonInterfaceBaseTypes(context).SelectMany(t => GetFieldsInternal(t, context, filter));
		}
		
		static IEnumerable<IField> GetFieldsInternal(IType baseType, ITypeResolveContext context, Predicate<IField> filter)
		{
			ITypeDefinition baseTypeDef = baseType.GetDefinition();
			if (baseTypeDef == null)
				yield break;
			baseTypeDef = baseTypeDef.GetCompoundClass();
			ParameterizedType pt = baseType as ParameterizedType;
			if (pt != null) {
				Substitution substitution = null;
				Func<ITypeReference, ITypeReference> substitutionFunc = null;
				foreach (IField f in baseTypeDef.Fields) {
					if (!(filter == null || filter(f)))
						continue;
					if (substitution == null) {
						substitution = new Substitution(pt.typeArguments);
						substitutionFunc = t => t.Resolve(context).AcceptVisitor(substitution);
					}
					SpecializedField sf = new SpecializedField(f);
					sf.SetDeclaringType(pt);
					sf.ReturnType = f.ReturnType.Resolve(context).AcceptVisitor(substitution);
					yield return sf;
				}
			} else {
				foreach (IField f in baseTypeDef.Fields) {
					if (filter == null || filter(f))
						yield return f;
				}
			}
		}
		
		public IEnumerable<IEvent> GetEvents(ITypeResolveContext context, Predicate<IEvent> filter = null)
		{
			return GetEvents(this, context, filter);
		}
		
		internal static IEnumerable<IEvent> GetEvents(IType type, ITypeResolveContext context, Predicate<IEvent> filter)
		{
			return type.GetNonInterfaceBaseTypes(context).SelectMany(t => GetEventsInternal(t, context, filter));
		}
		
		static IEnumerable<IEvent> GetEventsInternal(IType baseType, ITypeResolveContext context, Predicate<IEvent> filter)
		{
			ITypeDefinition baseTypeDef = baseType.GetDefinition();
			if (baseTypeDef == null)
				yield break;
			baseTypeDef = baseTypeDef.GetCompoundClass();
			ParameterizedType pt = baseType as ParameterizedType;
			if (pt != null) {
				Substitution substitution = null;
				Func<ITypeReference, ITypeReference> substitutionFunc = null;
				foreach (IEvent e in baseTypeDef.Events) {
					if (!(filter == null || filter(e)))
						continue;
					if (substitution == null) {
						substitution = new Substitution(pt.typeArguments);
						substitutionFunc = t => t.Resolve(context).AcceptVisitor(substitution);
					}
					SpecializedEvent se = new SpecializedEvent(e);
					se.SetDeclaringType(pt);
					se.ReturnType = e.ReturnType.Resolve(context).AcceptVisitor(substitution);
					yield return se;
				}
			} else {
				foreach (IEvent e in baseTypeDef.Events) {
					if (filter == null || filter(e))
						yield return e;
				}
			}
		}
		
		public IEnumerable<IMember> GetMembers(ITypeResolveContext context, Predicate<IMember> filter = null)
		{
			return GetMembers(this, context, filter);
		}
		
		internal static IEnumerable<IMember> GetMembers(IType type, ITypeResolveContext context, Predicate<IMember> filter)
		{
			Predicate<IMethod> methodFilter;
			if (filter == null)
				methodFilter = m => !m.IsConstructor;
			else
				methodFilter = m => !m.IsConstructor && filter(m);
			return type.GetNonInterfaceBaseTypes(context).SelectMany(
				delegate (IType t) {
					IEnumerable<IMember> members = GetMethodsInternal(t, context, methodFilter);
					members = members.Concat(GetPropertiesInternal(t, context, filter));
					members = members.Concat(GetFieldsInternal(t, context, filter));
					members = members.Concat(GetEventsInternal(t, context, filter));
					return members;
				});
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as IType);
		}
		
		public bool Equals(IType other)
		{
			ParameterizedType c = other as ParameterizedType;
			if (c == null || !genericType.Equals(c.genericType) || typeArguments.Length != c.typeArguments.Length)
				return false;
			for (int i = 0; i < typeArguments.Length; i++) {
				if (!typeArguments[i].Equals(c.typeArguments[i]))
					return false;
			}
			return true;
		}
		
		public override int GetHashCode()
		{
			int hashCode = genericType.GetHashCode();
			unchecked {
				foreach (var ta in typeArguments) {
					hashCode *= 1000000007;
					hashCode += 1000000009 * ta.GetHashCode();
				}
			}
			return hashCode;
		}
		
		public IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitParameterizedType(this);
		}
		
		public IType VisitChildren(TypeVisitor visitor)
		{
			IType g = genericType.AcceptVisitor(visitor);
			ITypeDefinition def = g as ITypeDefinition;
			if (def == null)
				return g;
			// Keep ta == null as long as no elements changed, allocate the array only if necessary.
			IType[] ta = (g != genericType) ? new IType[typeArguments.Length] : null;
			for (int i = 0; i < typeArguments.Length; i++) {
				IType r = typeArguments[i].AcceptVisitor(visitor);
				if (r == null)
					throw new NullReferenceException("TypeVisitor.Visit-method returned null");
				if (ta == null && r != typeArguments[i]) {
					// we found a difference, so we need to allocate the array
					ta = new IType[typeArguments.Length];
					for (int j = 0; j < i; j++) {
						ta[j] = typeArguments[j];
					}
				}
				if (ta != null)
					ta[i] = r;
			}
			if (ta == null)
				return this;
			else
				return new ParameterizedType(def, ta);
		}
	}

	/// <summary>
	/// ParameterizedTypeReference is a reference to generic class that specifies the type parameters.
	/// Example: List&lt;string&gt;
	/// </summary>
	public sealed class ParameterizedTypeReference : ITypeReference, ISupportsInterning
	{
		public static ITypeReference Create(ITypeReference genericType, IEnumerable<ITypeReference> typeArguments)
		{
			if (genericType == null)
				throw new ArgumentNullException("genericType");
			if (typeArguments == null)
				throw new ArgumentNullException("typeArguments");
			
			ITypeReference[] typeArgs = typeArguments.ToArray();
			if (typeArgs.Length == 0) {
				return genericType;
			} else if (genericType is ITypeDefinition && Array.TrueForAll(typeArgs, t => t is IType)) {
				IType[] ta = new IType[typeArgs.Length];
				for (int i = 0; i < ta.Length; i++) {
					ta[i] = (IType)typeArgs[i];
				}
				return new ParameterizedType((ITypeDefinition)genericType, ta);
			} else {
				return new ParameterizedTypeReference(genericType, typeArgs);
			}
		}
		
		ITypeReference genericType;
		ITypeReference[] typeArguments;
		
		public ParameterizedTypeReference(ITypeReference genericType, IEnumerable<ITypeReference> typeArguments)
		{
			if (genericType == null)
				throw new ArgumentNullException("genericType");
			if (typeArguments == null)
				throw new ArgumentNullException("typeArguments");
			this.genericType = genericType;
			this.typeArguments = typeArguments.ToArray();
			for (int i = 0; i < this.typeArguments.Length; i++) {
				if (this.typeArguments[i] == null)
					throw new ArgumentNullException("typeArguments[" + i + "]");
			}
		}
		
		public ITypeReference GenericType {
			get { return genericType; }
		}
		
		public ReadOnlyCollection<ITypeReference> TypeArguments {
			get {
				return Array.AsReadOnly(typeArguments);
			}
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			ITypeDefinition baseTypeDef = genericType.Resolve(context).GetDefinition();
			if (baseTypeDef == null)
				return SharedTypes.UnknownType;
			int tpc = baseTypeDef.TypeParameterCount;
			if (tpc == 0)
				return baseTypeDef;
			IType[] resolvedTypes = new IType[tpc];
			for (int i = 0; i < resolvedTypes.Length; i++) {
				if (i < typeArguments.Length)
					resolvedTypes[i] = typeArguments[i].Resolve(context);
				else
					resolvedTypes[i] = SharedTypes.UnknownType;
			}
			return new ParameterizedType(baseTypeDef, resolvedTypes);
		}
		
		public override string ToString()
		{
			StringBuilder b = new StringBuilder(genericType.ToString());
			b.Append('[');
			for (int i = 0; i < typeArguments.Length; i++) {
				if (i > 0)
					b.Append(',');
				b.Append('[');
				b.Append(typeArguments[i].ToString());
				b.Append(']');
			}
			b.Append(']');
			return b.ToString();
		}
		
		void ISupportsInterning.PrepareForInterning(IInterningProvider provider)
		{
			genericType = provider.Intern(genericType);
			for (int i = 0; i < typeArguments.Length; i++) {
				typeArguments[i] = provider.Intern(typeArguments[i]);
			}
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			int hashCode = genericType.GetHashCode();
			unchecked {
				foreach (ITypeReference t in typeArguments) {
					hashCode *= 27;
					hashCode += t.GetHashCode();
				}
			}
			return hashCode;
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			ParameterizedTypeReference o = other as ParameterizedTypeReference;
			if (o != null && genericType == o.genericType && typeArguments.Length == o.typeArguments.Length) {
				for (int i = 0; i < typeArguments.Length; i++) {
					if (typeArguments[i] != o.typeArguments[i])
						return false;
				}
				return true;
			}
			return false;
			
		}
	}
}
