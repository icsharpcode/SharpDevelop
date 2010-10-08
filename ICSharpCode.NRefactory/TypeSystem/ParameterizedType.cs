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
	/// When getting the Members, this type modifies the lists in such a way that the
	/// <see cref="GenericReturnType"/>s are replaced with the return types in the
	/// type arguments collection.
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
				if (type.ParentClass != null) {
					if (index >= 0 && index < typeArguments.Length)
						return typeArguments[index];
					else
						return SharedTypes.UnknownType;
				} else {
					return base.VisitTypeParameter(type);
				}
			}
			
			public IType Apply(IType type)
			{
				return type.AcceptVisitor(this);
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
		
		public bool? IsReferenceType {
			get { return genericType.IsReferenceType; }
		}
		
		public IType DeclaringType {
			get {
				ITypeDefinition declaringTypeDef = genericType.DeclaringTypeDefinition;
				if (declaringTypeDef != null && declaringTypeDef.TypeParameterCount > 0) {
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
		
		public string DotNetName {
			get {
				StringBuilder b = new StringBuilder(genericType.DotNetName);
				b.Append('[');
				for (int i = 0; i < typeArguments.Length; i++) {
					if (i > 0)
						b.Append(',');
					b.Append('[');
					b.Append(typeArguments[i].DotNetName);
					b.Append(']');
				}
				b.Append(']');
				return b.ToString();
			}
		}
		
		public IType GetElementType()
		{
			throw new NotSupportedException();
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
		
		public IEnumerable<IType> GetBaseTypes(ITypeResolveContext context)
		{
			Substitution substitution = new Substitution(typeArguments);
			return genericType.GetBaseTypes(context).Select(substitution.Apply);
		}
		
		public IEnumerable<IType> GetNestedTypes(ITypeResolveContext context)
		{
			/*
			class Base<T> {
				class Nested {}
			}
			class Derived<A, B> : Base<B> {}
			
			Derived<string,int>.GetNestedTypes() = { Base`1+Nested<int> }
			Derived.GetNestedTypes() = { Base`1+Nested<B> }
			Base<B>.GetNestedTypes() = { Base`1+Nested<B> }
			Base.GetNestedTypes() = { Base`1+Nested<T2> } where T2 = copy of T in Base`1+Nested
			 */
			Substitution substitution = new Substitution(typeArguments);
			List<IType> types = genericType.GetNestedTypes(context).ToList();
			for (int i = 0; i < types.Count; i++) {
				types[i] = types[i].AcceptVisitor(substitution);
			}
			return types;
		}
		
		public IEnumerable<IMethod> GetMethods(ITypeResolveContext context)
		{
			Substitution substitution = new Substitution(typeArguments);
			List<IMethod> methods = genericType.GetMethods(context).ToList();
			for (int i = 0; i < methods.Count; i++) {
				SpecializedMethod m = new SpecializedMethod(methods[i]);
				m.SetDeclaringType(this);
				m.SubstituteTypes(context, substitution);
				methods[i] = m;
			}
			return methods;
		}
		
		public IEnumerable<IMethod> GetConstructors(ITypeResolveContext context)
		{
			Substitution substitution = new Substitution(typeArguments);
			List<IMethod> methods = genericType.GetConstructors(context).ToList();
			for (int i = 0; i < methods.Count; i++) {
				SpecializedMethod m = new SpecializedMethod(methods[i]);
				m.SetDeclaringType(this);
				m.SubstituteTypes(context, substitution);
				methods[i] = m;
			}
			return methods;
		}
		
		public IEnumerable<IProperty> GetProperties(ITypeResolveContext context)
		{
			Substitution substitution = new Substitution(typeArguments);
			List<IProperty> properties = genericType.GetProperties(context).ToList();
			for (int i = 0; i < properties.Count; i++) {
				SpecializedProperty p = new SpecializedProperty(properties[i]);
				p.SetDeclaringType(this);
				p.SubstituteTypes(context, substitution);
				properties[i] = p;
			}
			return properties;
		}
		
		public IEnumerable<IField> GetFields(ITypeResolveContext context)
		{
			Substitution substitution = new Substitution(typeArguments);
			List<IField> fields = genericType.GetFields(context).ToList();
			for (int i = 0; i < fields.Count; i++) {
				SpecializedField f = new SpecializedField(fields[i]);
				f.SetDeclaringType(this);
				f.ReturnType = f.ReturnType.Resolve(context).AcceptVisitor(substitution);
				fields[i] = f;
			}
			return fields;
		}
		
		public IEnumerable<IEvent> GetEvents(ITypeResolveContext context)
		{
			Substitution substitution = new Substitution(typeArguments);
			List<IEvent> events = genericType.GetEvents(context).ToList();
			for (int i = 0; i < events.Count; i++) {
				SpecializedEvent e = new SpecializedEvent(events[i]);
				e.SetDeclaringType(this);
				e.ReturnType = e.ReturnType.Resolve(context).AcceptVisitor(substitution);
				events[i] = e;
			}
			return events;
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
			IType[] ta = new IType[typeArguments.Length];
			bool isSame = g == genericType;
			for (int i = 0; i < typeArguments.Length; i++) {
				ta[i] = typeArguments[i].AcceptVisitor(visitor);
				isSame &= ta[i] == typeArguments[i];
			}
			if (isSame)
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
			if (genericType is ITypeDefinition && Array.TrueForAll(typeArgs, t => t is IType)) {
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
