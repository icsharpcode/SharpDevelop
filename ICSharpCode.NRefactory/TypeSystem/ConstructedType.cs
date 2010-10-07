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
	/// ConstructedType represents an instance of a generic type.
	/// Example: List&lt;string&gt;
	/// </summary>
	/// <remarks>
	/// When getting the Members, this type modifies the lists in such a way that the
	/// <see cref="GenericReturnType"/>s are replaced with the return types in the
	/// type arguments collection.
	/// </remarks>
	public class ConstructedType : Immutable, IType
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
		}
		
		readonly ITypeDefinition genericType;
		readonly IType[] typeArguments;
		
		public ConstructedType(ITypeDefinition genericType, IEnumerable<IType> typeArguments)
		{
			if (genericType == null)
				throw new ArgumentNullException("genericType");
			if (typeArguments == null)
				throw new ArgumentNullException("typeArguments");
			this.genericType = genericType;
			this.typeArguments = typeArguments.ToArray(); // copy input array to ensure it isn't modified
			if (this.typeArguments.Length == 0)
				throw new ArgumentException("Cannot use ConstructedType with 0 type arguments.");
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
		internal ConstructedType(ITypeDefinition genericType, IType[] typeArguments)
		{
			Debug.Assert(genericType.TypeParameterCount == typeArguments.Length);
			this.genericType = genericType;
			this.typeArguments = typeArguments;
		}
		
		public bool? IsReferenceType {
			get { return genericType.IsReferenceType; }
		}
		
		public IType DeclaringType {
			get { return genericType.DeclaringType; }
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
			return genericType.GetBaseTypes(context).Select(t => t.AcceptVisitor(substitution));
		}
		
		public IList<IType> GetNestedTypes(ITypeResolveContext context)
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
			IList<IType> types = genericType.GetNestedTypes(context);
			for (int i = 0; i < types.Count; i++) {
				types[i] = types[i].AcceptVisitor(substitution);
			}
			return types;
		}
		
		public IList<IMethod> GetMethods(ITypeResolveContext context)
		{
			Substitution substitution = new Substitution(typeArguments);
			IList<IMethod> methods = genericType.GetMethods(context);
			for (int i = 0; i < methods.Count; i++) {
				SpecializedMethod m = new SpecializedMethod(methods[i]);
				m.SetDeclaringType(this);
				m.SubstituteTypes(context, substitution);
				methods[i] = m;
			}
			return methods;
		}
		
		public IList<IProperty> GetProperties(ITypeResolveContext context)
		{
			Substitution substitution = new Substitution(typeArguments);
			IList<IProperty> properties = genericType.GetProperties(context);
			for (int i = 0; i < properties.Count; i++) {
				SpecializedProperty p = new SpecializedProperty(properties[i]);
				p.SetDeclaringType(this);
				p.SubstituteTypes(context, substitution);
				properties[i] = p;
			}
			return properties;
		}
		
		public IList<IField> GetFields(ITypeResolveContext context)
		{
			Substitution substitution = new Substitution(typeArguments);
			IList<IField> fields = genericType.GetFields(context);
			for (int i = 0; i < fields.Count; i++) {
				SpecializedField f = new SpecializedField(fields[i]);
				f.SetDeclaringType(this);
				f.ReturnType = f.ReturnType.Resolve(context).AcceptVisitor(substitution);
				fields[i] = f;
			}
			return fields;
		}
		
		public IList<IEvent> GetEvents(ITypeResolveContext context)
		{
			Substitution substitution = new Substitution(typeArguments);
			IList<IEvent> events = genericType.GetEvents(context);
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
			ConstructedType c = other as ConstructedType;
			if (c == null || typeArguments.Length != c.typeArguments.Length)
				return false;
			if (genericType.Equals(c.genericType)) {
				for (int i = 0; i < typeArguments.Length; i++) {
					if (!typeArguments[i].Equals(c.typeArguments[i]))
						return false;
				}
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
			return visitor.VisitConstructedType(this);
		}
		
		public IType VisitChildren(TypeVisitor visitor)
		{
			IType g = genericType.AcceptVisitor(visitor);
			ITypeDefinition def = g as ITypeDefinition;
			if (def == null)
				return g;
			IType[] ta = new IType[typeArguments.Length];
			bool isSame = g == this;
			for (int i = 0; i < typeArguments.Length; i++) {
				ta[i] = typeArguments[i].AcceptVisitor(visitor);
				isSame &= ta[i] == typeArguments[i];
			}
			if (isSame)
				return this;
			else
				return new ConstructedType(def, ta);
		}
	}
	
	/// <summary>
	/// ConstructedTypeReference is a reference to generic class that specifies the type parameters.
	/// Example: List&lt;string&gt;
	/// </summary>
	public class ConstructedTypeReference : AbstractTypeReference
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
				return new ConstructedType((ITypeDefinition)genericType, ta);
			} else {
				return new ConstructedTypeReference(genericType, typeArgs);
			}
		}
		
		readonly ITypeReference genericType;
		readonly ITypeReference[] typeArguments;
		
		public ConstructedTypeReference(ITypeReference genericType, IEnumerable<ITypeReference> typeArguments)
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
		
		public override IType Resolve(ITypeResolveContext context)
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
			return new ConstructedType(baseTypeDef, resolvedTypes);
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
	}
}
