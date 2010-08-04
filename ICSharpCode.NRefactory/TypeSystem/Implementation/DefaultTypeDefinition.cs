// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Diagnostics.Contracts;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	public class DefaultTypeDefinition : AbstractFreezable, ITypeDefinition
	{
		readonly IProjectContent projectContent;
		readonly ITypeDefinition declaringTypeDefinition;
		
		readonly string ns;
		readonly string name;
		
		ClassType classType;
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
		Accessibility accessibility;
		
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
				return this.ClassType == ClassType.Enum || this.ClassType == ClassType.Struct;
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
					Contract.Assume(!string.IsNullOrEmpty(combinedName)); // help out the static checker
					return combinedName;
				} else {
					if (string.IsNullOrEmpty(ns)) {
						return this.Name;
					} else {
						string combinedName = this.Namespace + "." + this.Name;
						Contract.Assume(!string.IsNullOrEmpty(combinedName)); // help out the static checker
						return combinedName;
					}
				}
			}
		}
		
		public int TypeParameterCount {
			get { return typeParameters != null ? typeParameters.Count : 0; }
		}
		
		public EntityType EntityType {
			get { return EntityType.Class; }
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
		
		public bool IsStatic {
			get {
				throw new NotImplementedException();
			}
		}
		
		public Accessibility Accessibility {
			get { return accessibility; }
			set {
				CheckBeforeMutation();
				accessibility = value;
			}
		}
		
		public bool IsAbstract {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsSealed {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsVirtual {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsOverride {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsOverridable {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsShadowing {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsSynthetic {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IProjectContent ProjectContent {
			get { return projectContent; }
		}
		
		public IType GetBaseType(ITypeResolveContext context)
		{
			throw new NotImplementedException();
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
			return this;
		}
		
		public IList<IType> GetNestedTypes(ITypeResolveContext context)
		{
			throw new NotImplementedException();
		}
		
		public IList<IMethod> GetMethods(ITypeResolveContext context)
		{
			throw new NotImplementedException();
		}
		
		public IList<IProperty> GetProperties(ITypeResolveContext context)
		{
			throw new NotImplementedException();
		}
		
		public IList<IField> GetFields(ITypeResolveContext context)
		{
			throw new NotImplementedException();
		}
		
		public IList<IEvent> GetEvents(ITypeResolveContext context)
		{
			throw new NotImplementedException();
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
	}
}
