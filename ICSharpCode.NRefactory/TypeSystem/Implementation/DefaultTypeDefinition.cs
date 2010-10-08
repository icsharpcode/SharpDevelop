// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

using ICSharpCode.NRefactory.Util;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	public class DefaultTypeDefinition : AbstractFreezable, ITypeDefinition
	{
		readonly IProjectContent projectContent;
		readonly ITypeDefinition declaringTypeDefinition;
		
		readonly string ns;
		readonly string name;
		
		IList<ITypeReference> baseTypes;
		IList<ITypeParameter> typeParameters;
		IList<ITypeDefinition> innerClasses;
		IList<IField> fields;
		IList<IMethod> methods;
		IList<IProperty> properties;
		IList<IEvent> events;
		IList<IAttribute> attributes;
		
		DomRegion region;
		DomRegion bodyRegion;
		
		// 1 byte per enum + 2 bytes for flags
		ClassType classType;
		Accessibility accessibility;
		BitVector16 flags;
		const ushort FlagSealed    = 0x0001;
		const ushort FlagAbstract  = 0x0002;
		const ushort FlagShadowing = 0x0004;
		const ushort FlagSynthetic = 0x0008;
		const ushort FlagAddDefaultConstructorIfRequired = 0x0010;
		
		protected override void FreezeInternal()
		{
			baseTypes = FreezeList(baseTypes);
			typeParameters = FreezeList(typeParameters);
			innerClasses = FreezeList(innerClasses);
			fields = FreezeList(fields);
			methods = FreezeList(methods);
			properties = FreezeList(properties);
			events = FreezeList(events);
			attributes = FreezeList(attributes);
			base.FreezeInternal();
		}
		
		public DefaultTypeDefinition(ITypeDefinition declaringTypeDefinition, string name)
		{
			if (declaringTypeDefinition == null)
				throw new ArgumentNullException("declaringTypeDefinition");
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("name");
			Contract.EndContractBlock();
			this.projectContent = declaringTypeDefinition.ProjectContent;
			this.declaringTypeDefinition = declaringTypeDefinition;
			this.name = name;
			this.ns = declaringTypeDefinition.Namespace;
		}
		
		public DefaultTypeDefinition(IProjectContent projectContent, string ns, string name)
		{
			if (projectContent == null)
				throw new ArgumentNullException("projectContent");
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("name");
			Contract.EndContractBlock();
			this.projectContent = projectContent;
			this.ns = ns ?? string.Empty;
			this.name = name;
		}
		
		[ContractInvariantMethod]
		void ObjectInvariant()
		{
			Contract.Invariant(projectContent != null);
			Contract.Invariant(!string.IsNullOrEmpty(name));
			Contract.Invariant(ns != null);
		}
		
		public ClassType ClassType {
			get { return classType; }
			set {
				CheckBeforeMutation();
				classType = value;
			}
		}
		
		public IList<ITypeReference> BaseTypes {
			get {
				if (baseTypes == null)
					baseTypes = new List<ITypeReference>();
				return baseTypes;
			}
		}
		
		public IList<ITypeParameter> TypeParameters {
			get {
				if (typeParameters == null)
					typeParameters = new List<ITypeParameter>();
				return typeParameters;
			}
		}
		
		public IList<ITypeDefinition> InnerClasses {
			get {
				if (innerClasses == null)
					innerClasses = new List<ITypeDefinition>();
				return innerClasses;
			}
		}
		
		public IList<IField> Fields {
			get {
				if (fields == null)
					fields = new List<IField>();
				return fields;
			}
		}
		
		public IList<IProperty> Properties {
			get {
				if (properties == null)
					properties = new List<IProperty>();
				return properties;
			}
		}
		
		public IList<IMethod> Methods {
			get {
				if (methods == null)
					methods = new List<IMethod>();
				return methods;
			}
		}
		
		public IList<IEvent> Events {
			get {
				if (events == null)
					events = new List<IEvent>();
				return events;
			}
		}
		
		public IEnumerable<IMember> Members {
			get {
				return this.Fields.Concat<IMember>(this.Properties).Concat(this.Methods).Concat(this.Events);
			}
		}
		
		public bool? IsReferenceType {
			get {
				switch (this.ClassType) {
					case ClassType.Class:
					case ClassType.Interface:
					case ClassType.Delegate:
						return true;
					case ClassType.Enum:
					case ClassType.Struct:
						return false;
					default:
						return null;
				}
			}
		}
		
		public string FullName {
			get {
				if (declaringTypeDefinition != null) {
					string combinedName = declaringTypeDefinition.FullName + "." + this.name;
					Contract.Assume(!string.IsNullOrEmpty(combinedName));
					return combinedName;
				} else if (string.IsNullOrEmpty(ns)) {
					return this.name;
				} else {
					string combinedName = this.ns + "." + this.name;
					Contract.Assume(!string.IsNullOrEmpty(combinedName));
					return combinedName;
				}
			}
		}
		
		public string Name {
			get { return this.name; }
		}
		
		public string Namespace {
			get { return this.ns; }
		}
		
		public string DotNetName {
			get {
				if (declaringTypeDefinition != null) {
					int tpCount = this.TypeParameterCount - declaringTypeDefinition.TypeParameterCount;
					string combinedName;
					if (tpCount > 0)
						combinedName = declaringTypeDefinition.DotNetName + "+" + this.Name + "`" + tpCount.ToString(CultureInfo.InvariantCulture);
					else
						combinedName = declaringTypeDefinition.DotNetName + "+" + this.Name;
					return combinedName;
				} else {
					int tpCount = this.TypeParameterCount;
					if (string.IsNullOrEmpty(ns)) {
						if (tpCount > 0)
							return this.Name + "`" + tpCount.ToString(CultureInfo.InvariantCulture);
						else
							return this.Name;
					} else {
						if (tpCount > 0)
							return this.Namespace + "." + this.Name + "`" + tpCount.ToString(CultureInfo.InvariantCulture);
						else
							return this.Namespace + "." + this.Name;
					}
				}
			}
		}
		
		public int TypeParameterCount {
			get { return typeParameters != null ? typeParameters.Count : 0; }
		}
		
		public EntityType EntityType {
			get { return EntityType.TypeDefinition; }
		}
		
		public DomRegion Region {
			get { return region; }
			set {
				CheckBeforeMutation();
				region = value;
			}
		}
		
		public DomRegion BodyRegion {
			get { return bodyRegion; }
			set {
				CheckBeforeMutation();
				bodyRegion = value;
			}
		}
		
		public ITypeDefinition DeclaringTypeDefinition {
			get { return declaringTypeDefinition; }
		}
		
		public IType DeclaringType {
			get { return declaringTypeDefinition; }
		}
		
		public IList<IAttribute> Attributes {
			get {
				if (attributes == null)
					attributes = new List<IAttribute>();
				return attributes;
			}
		}
		
		public virtual string Documentation {
			get { return null; }
		}
		
		public Accessibility Accessibility {
			get { return accessibility; }
			set {
				CheckBeforeMutation();
				accessibility = value;
			}
		}
		
		public bool IsStatic {
			get { return IsAbstract && IsSealed; }
		}
		
		public bool IsAbstract {
			get { return flags[FlagAbstract]; }
			set {
				CheckBeforeMutation();
				flags[FlagAbstract] = value;
			}
		}
		
		public bool IsSealed {
			get { return flags[FlagSealed]; }
			set {
				CheckBeforeMutation();
				flags[FlagSealed] = value;
			}
		}
		
		public bool IsShadowing {
			get { return flags[FlagShadowing]; }
			set {
				CheckBeforeMutation();
				flags[FlagShadowing] = value;
			}
		}
		
		public bool IsSynthetic {
			get { return flags[FlagSynthetic]; }
			set {
				CheckBeforeMutation();
				flags[FlagSynthetic] = value;
			}
		}
		
		public IProjectContent ProjectContent {
			get { return projectContent; }
		}
		
		public IEnumerable<IType> GetBaseTypes(ITypeResolveContext context)
		{
			if (baseTypes == null || baseTypes.Count == 0) {
				if (this.FullName == "System.Object")
					return EmptyList<IType>.Instance;
				switch (classType) {
					case ClassType.Enum:
						return GetPrimitiveBaseType(typeof(Enum), context);
					case ClassType.Struct:
						return GetPrimitiveBaseType(typeof(ValueType), context);
					case ClassType.Delegate:
						return GetPrimitiveBaseType(typeof(Delegate), context);
					default:
						return GetPrimitiveBaseType(typeof(object), context);
				}
			}
			return baseTypes.Select(t => t.Resolve(context)).Where(t => t != SharedTypes.UnknownType);
		}
		
		static IEnumerable<IType> GetPrimitiveBaseType(Type type, ITypeResolveContext context)
		{
			IType t = context.GetClass(type);
			if (t != null)
				return new [] { t };
			else
				return EmptyList<IType>.Instance;
		}
		
		public virtual ITypeDefinition GetCompoundClass()
		{
			return this;
		}
		
		public virtual IList<ITypeDefinition> GetParts()
		{
			return new ITypeDefinition[] { this };
		}
		
		public IType GetElementType()
		{
			throw new InvalidOperationException();
		}
		
		public ITypeDefinition GetDefinition()
		{
			return this;
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			return this;
		}
		
		public virtual IEnumerable<IType> GetNestedTypes(ITypeResolveContext context)
		{
			ITypeDefinition compound = GetCompoundClass();
			if (compound != this)
				return compound.GetNestedTypes(context);
			
			List<IType> nestedTypes = new List<IType>();
			using (var busyLock = BusyManager.Enter(this)) {
				if (busyLock.Success) {
					foreach (var baseTypeRef in this.BaseTypes) {
						IType baseType = baseTypeRef.Resolve(context);
						ITypeDefinition baseTypeDef = baseType.GetDefinition();
						if (baseTypeDef != null && baseTypeDef.ClassType != ClassType.Interface) {
							// get nested types from baseType (not baseTypeDef) so that generics work correctly
							nestedTypes.AddRange(baseType.GetNestedTypes(context));
							break; // there is at most 1 non-interface base
						}
					}
					foreach (ITypeDefinition innerClass in this.InnerClasses) {
						if (innerClass.TypeParameterCount > 0) {
							// Parameterize inner classes with their own type parameters, as per <remarks> on IType.GetNestedTypes.
							nestedTypes.Add(new ParameterizedType(innerClass, innerClass.TypeParameters));
						} else {
							nestedTypes.Add(innerClass);
						}
					}
				}
			}
			return nestedTypes;
		}
		
		public virtual IEnumerable<IMethod> GetMethods(ITypeResolveContext context)
		{
			ITypeDefinition compound = GetCompoundClass();
			if (compound != this)
				return compound.GetMethods(context);
			
			List<IMethod> methods = new List<IMethod>();
			using (var busyLock = BusyManager.Enter(this)) {
				if (busyLock.Success) {
					foreach (var baseTypeRef in this.BaseTypes) {
						IType baseType = baseTypeRef.Resolve(context);
						ITypeDefinition baseTypeDef = baseType.GetDefinition();
						if (baseTypeDef != null && (baseTypeDef.ClassType != ClassType.Interface || this.ClassType == ClassType.Interface)) {
							methods.AddRange(baseType.GetMethods(context));
						}
					}
					methods.AddRange(this.Methods.Where(m => !m.IsConstructor));
				}
			}
			return methods;
		}
		
		public virtual IEnumerable<IMethod> GetConstructors(ITypeResolveContext context)
		{
			ITypeDefinition compound = GetCompoundClass();
			if (compound != this)
				return compound.GetConstructors(context);
			
			List<IMethod> methods = new List<IMethod>();
			methods.AddRange(this.Methods.Where(m => m.IsConstructor && !m.IsStatic));
			
			if (this.AddDefaultConstructorIfRequired) {
				if (this.ClassType == ClassType.Class && methods.Count == 0
				    || this.ClassType == ClassType.Enum || this.ClassType == ClassType.Struct)
				{
					methods.Add(DefaultMethod.CreateDefaultConstructor(this));
				}
			}
			return methods;
		}
		
		public virtual IEnumerable<IProperty> GetProperties(ITypeResolveContext context)
		{
			ITypeDefinition compound = GetCompoundClass();
			if (compound != this)
				return compound.GetProperties(context);
			
			List<IProperty> properties = new List<IProperty>();
			using (var busyLock = BusyManager.Enter(this)) {
				if (busyLock.Success) {
					foreach (var baseTypeRef in this.BaseTypes) {
						IType baseType = baseTypeRef.Resolve(context);
						ITypeDefinition baseTypeDef = baseType.GetDefinition();
						if (baseTypeDef != null && (baseTypeDef.ClassType != ClassType.Interface || this.ClassType == ClassType.Interface)) {
							properties.AddRange(baseType.GetProperties(context));
						}
					}
					properties.AddRange(this.Properties);
				}
			}
			return properties;
		}
		
		public virtual IEnumerable<IField> GetFields(ITypeResolveContext context)
		{
			ITypeDefinition compound = GetCompoundClass();
			if (compound != this)
				return compound.GetFields(context);
			
			List<IField> fields = new List<IField>();
			using (var busyLock = BusyManager.Enter(this)) {
				if (busyLock.Success) {
					foreach (var baseTypeRef in this.BaseTypes) {
						IType baseType = baseTypeRef.Resolve(context);
						ITypeDefinition baseTypeDef = baseType.GetDefinition();
						if (baseTypeDef != null && (baseTypeDef.ClassType != ClassType.Interface || this.ClassType == ClassType.Interface)) {
							fields.AddRange(baseType.GetFields(context));
						}
					}
					fields.AddRange(this.Fields);
				}
			}
			return fields;
		}
		
		public virtual IEnumerable<IEvent> GetEvents(ITypeResolveContext context)
		{
			ITypeDefinition compound = GetCompoundClass();
			if (compound != this)
				return compound.GetEvents(context);
			
			List<IEvent> events = new List<IEvent>();
			using (var busyLock = BusyManager.Enter(this)) {
				if (busyLock.Success) {
					foreach (var baseTypeRef in this.BaseTypes) {
						IType baseType = baseTypeRef.Resolve(context);
						ITypeDefinition baseTypeDef = baseType.GetDefinition();
						if (baseTypeDef != null && (baseTypeDef.ClassType != ClassType.Interface || this.ClassType == ClassType.Interface)) {
							events.AddRange(baseType.GetEvents(context));
						}
					}
					events.AddRange(this.Events);
				}
			}
			return events;
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as ITypeDefinition);
		}
		
		public bool Equals(IType other)
		{
			return Equals(other as ITypeDefinition);
		}
		
		public bool Equals(ITypeDefinition other)
		{
			if (other == null)
				return false;
			if (declaringTypeDefinition != null) {
				return declaringTypeDefinition.Equals(other.DeclaringTypeDefinition)
					&& this.Name == other.Name
					&& this.TypeParameterCount == other.TypeParameterCount;
			} else {
				// We do not check the project content because assemblies might or might not
				// be equivalent depending on compiler settings and runtime assembly
				// redirection.
				return other.DeclaringTypeDefinition == null
					&& this.Namespace == other.Namespace
					&& this.Name == other.Name
					&& this.TypeParameterCount == other.TypeParameterCount;
			}
		}
		
		public override int GetHashCode()
		{
			if (declaringTypeDefinition != null) {
				return declaringTypeDefinition.GetHashCode() ^ name.GetHashCode();
			} else {
				return ns.GetHashCode() ^ name.GetHashCode() ^ this.TypeParameterCount;
			}
		}
		
		public override string ToString()
		{
			return DotNetName;
		}
		
		/// <summary>
		/// Gets whether a default constructor should be added to this class if it is required.
		/// Such automatic default constructors will not appear in ITypeDefinition.Methods, but will be present
		/// in IType.GetMethods().
		/// </summary>
		/// <remarks>This way of creating the default constructor is necessary because
		/// we cannot create it directly in the IClass - we need to consider partial classes.</remarks>
		public bool AddDefaultConstructorIfRequired {
			get { return flags[FlagAddDefaultConstructorIfRequired]; }
			set {
				CheckBeforeMutation();
				flags[FlagAddDefaultConstructorIfRequired] = value;
			}
		}
		
		public IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitTypeDefinition(this);
		}
		
		public IType VisitChildren(TypeVisitor visitor)
		{
			return this;
		}
	}
}
