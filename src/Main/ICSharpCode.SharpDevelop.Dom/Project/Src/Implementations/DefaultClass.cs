// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class DefaultClass : AbstractNamedEntity, IClass, IComparable
	{
		ClassType classType;
		DomRegion region;
		DomRegion bodyRegion;
		
		ICompilationUnit compilationUnit;
		
		List<IReturnType> baseTypes   = null;
		
		List<IClass>    innerClasses = null;
		List<IField>    fields       = null;
		List<IProperty> properties   = null;
		List<IMethod>   methods      = null;
		List<IEvent>    events       = null;
		IList<ITypeParameter> typeParameters = null;
		
		byte flags;
		const byte hasPublicOrInternalStaticMembersFlag = 0x02;
		const byte hasExtensionMethodsFlag              = 0x04;
		internal byte Flags {
			get {
				if (flags == 0) {
					flags = 1;
					foreach (IMember m in this.Fields) {
						if (m.IsStatic && (m.IsPublic || m.IsInternal)) {
							flags |= hasPublicOrInternalStaticMembersFlag;
						}
					}
					foreach (IProperty m in this.Properties) {
						if (m.IsStatic && (m.IsPublic || m.IsInternal)) {
							flags |= hasPublicOrInternalStaticMembersFlag;
						}
						if (m.IsExtensionMethod) {
							flags |= hasExtensionMethodsFlag;
						}
					}
					foreach (IMethod m in this.Methods) {
						if (m.IsStatic && (m.IsPublic || m.IsInternal)) {
							flags |= hasPublicOrInternalStaticMembersFlag;
						}
						if (m.IsExtensionMethod) {
							flags |= hasExtensionMethodsFlag;
						}
					}
					foreach (IMember m in this.Events) {
						if (m.IsStatic && (m.IsPublic || m.IsInternal)) {
							flags |= hasPublicOrInternalStaticMembersFlag;
						}
					}
					foreach (IClass c in this.InnerClasses) {
						if (c.IsPublic || c.IsInternal) {
							flags |= hasPublicOrInternalStaticMembersFlag;
						}
					}
				}
				return flags;
			}
			set {
				flags = value;
			}
		}
		public bool HasPublicOrInternalStaticMembers {
			get {
				return (Flags & hasPublicOrInternalStaticMembersFlag) == hasPublicOrInternalStaticMembersFlag;
			}
		}
		public bool HasExtensionMethods {
			get {
				return (Flags & hasExtensionMethodsFlag) == hasExtensionMethodsFlag;
			}
		}
		
		public DefaultClass(ICompilationUnit compilationUnit, string fullyQualifiedName) : base(null)
		{
			this.compilationUnit = compilationUnit;
			this.FullyQualifiedName = fullyQualifiedName;
		}
		
		public DefaultClass(ICompilationUnit compilationUnit, IClass declaringType) : base(declaringType)
		{
			this.compilationUnit = compilationUnit;
		}
		
		public DefaultClass(ICompilationUnit compilationUnit, ClassType classType, ModifierEnum modifiers, DomRegion region, IClass declaringType) : base(declaringType)
		{
			this.compilationUnit = compilationUnit;
			this.region = region;
			this.classType = classType;
			Modifiers = modifiers;
		}
		
		IReturnType defaultReturnType;
		
		public IReturnType DefaultReturnType {
			get {
				if (defaultReturnType == null)
					defaultReturnType = CreateDefaultReturnType();
				return defaultReturnType;
			}
		}
		
		protected virtual IReturnType CreateDefaultReturnType()
		{
			if (IsPartial) {
				return new GetClassReturnType(ProjectContent, FullyQualifiedName, TypeParameters.Count);
			} else {
				return new DefaultReturnType(this);
			}
		}
		
		public IClass GetCompoundClass()
		{
			return this.DefaultReturnType.GetUnderlyingClass() ?? this;
		}
		
		protected override void OnFullyQualifiedNameChanged(EventArgs e)
		{
			base.OnFullyQualifiedNameChanged(e);
			GetClassReturnType rt = defaultReturnType as GetClassReturnType;
			if (rt != null) {
				rt.SetFullyQualifiedName(FullyQualifiedName);
			}
		}
		
		public ICompilationUnit CompilationUnit {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				return compilationUnit;
			}
		}
		
		public IProjectContent ProjectContent {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				return CompilationUnit.ProjectContent;
			}
		}
		
		public ClassType ClassType {
			get {
				return classType;
			}
			set {
				classType = value;
			}
		}
		
		public DomRegion Region {
			get {
				return region;
			}
			set {
				region = value;
			}
		}
		
		public DomRegion BodyRegion {
			get {
				return bodyRegion;
			}
			set {
				bodyRegion = value;
			}
		}
		
		public override string DotNetName {
			get {
				if (typeParameters == null || typeParameters.Count == 0) {
					return FullyQualifiedName;
				} else {
					return FullyQualifiedName + "`" + typeParameters.Count;
				}
			}
		}
		
		public override string DocumentationTag {
			get {
				return "T:" + DotNetName;
			}
		}
		
		public List<IReturnType> BaseTypes {
			get {
				if (baseTypes == null) {
					baseTypes = new List<IReturnType>();
				}
				return baseTypes;
			}
		}
		
		public virtual List<IClass> InnerClasses {
			get {
				if (innerClasses == null) {
					innerClasses = new List<IClass>();
				}
				return innerClasses;
			}
		}
		
		public virtual List<IField> Fields {
			get {
				if (fields == null) {
					fields = new List<IField>();
				}
				return fields;
			}
		}
		
		public virtual List<IProperty> Properties {
			get {
				if (properties == null) {
					properties = new List<IProperty>();
				}
				return properties;
			}
		}
		
		public virtual List<IMethod> Methods {
			get {
				if (methods == null) {
					methods = new List<IMethod>();
				}
				return methods;
			}
		}
		
		public virtual List<IEvent> Events {
			get {
				if (events == null) {
					events = new List<IEvent>();
				}
				return events;
			}
		}
		
		public virtual IList<ITypeParameter> TypeParameters {
			get {
				if (typeParameters == null) {
					typeParameters = new List<ITypeParameter>();
				}
				return typeParameters;
			}
			set {
				typeParameters = value;
			}
		}
		
		public virtual int CompareTo(IClass value)
		{
			int cmp;
			
			if(0 != (cmp = base.CompareTo((IDecoration)value))) {
				return cmp;
			}
			
			if (FullyQualifiedName != null) {
				cmp = FullyQualifiedName.CompareTo(value.FullyQualifiedName);
				if (cmp != 0) {
					return cmp;
				}
				return this.TypeParameters.Count - value.TypeParameters.Count;
			}
			return -1;
		}
		
		int IComparable.CompareTo(object o)
		{
			return CompareTo((IClass)o);
		}
		
		List<IClass> inheritanceTreeCache;
		
		public IEnumerable<IClass> ClassInheritanceTree {
			get {
				if (inheritanceTreeCache != null)
					return inheritanceTreeCache;
				List<IClass> visitedList = new List<IClass>();
				Queue<IReturnType> typesToVisit = new Queue<IReturnType>();
				bool enqueuedLastBaseType = false;
				IClass currentClass = this;
				IReturnType nextType;
				do {
					if (currentClass != null) {
						if (!visitedList.Contains(currentClass)) {
							visitedList.Add(currentClass);
							foreach (IReturnType type in currentClass.BaseTypes) {
								typesToVisit.Enqueue(type);
							}
						}
					}
					if (typesToVisit.Count > 0) {
						nextType = typesToVisit.Dequeue();
					} else {
						nextType = enqueuedLastBaseType ? null : GetBaseTypeByClassType();
						enqueuedLastBaseType = true;
					}
					if (nextType != null) {
						currentClass = nextType.GetUnderlyingClass();
					}
				} while (nextType != null);
				if (UseInheritanceCache)
					inheritanceTreeCache = visitedList;
				return visitedList;
			}
		}
		
		protected bool UseInheritanceCache = false;
		
		protected override bool CanBeSubclass {
			get {
				return true;
			}
		}

		public IReturnType GetBaseType(int index)
		{
			return BaseTypes[index];
		}
		
		IReturnType cachedBaseType;
		
		public IReturnType BaseType {
			get {
				if (cachedBaseType == null) {
					foreach (IReturnType baseType in this.BaseTypes) {
						IClass baseClass = baseType.GetUnderlyingClass();
						if (baseClass != null && baseClass.ClassType == this.ClassType) {
							cachedBaseType = baseType;
							break;
						}
					}
				}
				if (cachedBaseType == null) {
					return GetBaseTypeByClassType();
				} else {
					return cachedBaseType;
				}
			}
		}
		
		IReturnType GetBaseTypeByClassType()
		{
			switch (ClassType) {
				case ClassType.Class:
				case ClassType.Interface:
					if (FullyQualifiedName != "System.Object") {
						return this.ProjectContent.SystemTypes.Object;
					}
					break;
				case ClassType.Enum:
					return this.ProjectContent.SystemTypes.Enum;
				case ClassType.Delegate:
					return this.ProjectContent.SystemTypes.Delegate;
				case ClassType.Struct:
					return this.ProjectContent.SystemTypes.ValueType;
			}
			return null;
		}
		
		public IClass BaseClass {
			get {
				foreach (IReturnType baseType in this.BaseTypes) {
					IClass baseClass = baseType.GetUnderlyingClass();
					if (baseClass != null && baseClass.ClassType == this.ClassType)
						return baseClass;
				}
				switch (ClassType) {
					case ClassType.Class:
						if (FullyQualifiedName != "System.Object") {
							return this.ProjectContent.SystemTypes.Object.GetUnderlyingClass();
						}
						break;
					case ClassType.Enum:
						return this.ProjectContent.SystemTypes.Enum.GetUnderlyingClass();
					case ClassType.Delegate:
						return this.ProjectContent.SystemTypes.Delegate.GetUnderlyingClass();
					case ClassType.Struct:
						return this.ProjectContent.SystemTypes.ValueType.GetUnderlyingClass();
				}
				return null;
			}
		}
		
		public bool IsTypeInInheritanceTree(IClass possibleBaseClass)
		{
			if (possibleBaseClass == null) {
				return false;
			}
			foreach (IClass baseClass in this.ClassInheritanceTree) {
				if (possibleBaseClass.FullyQualifiedName == baseClass.FullyQualifiedName)
					return true;
			}
			return false;
		}
		
		/// <summary>
		/// Searches the member with the specified name. Returns the first member/overload found.
		/// </summary>
		public IMember SearchMember(string memberName, LanguageProperties language)
		{
			if (memberName == null || memberName.Length == 0) {
				return null;
			}
			StringComparer cmp = language.NameComparer;
			foreach (IProperty p in Properties) {
				if (cmp.Equals(p.Name, memberName)) {
					return p;
				}
			}
			foreach (IEvent e in Events) {
				if (cmp.Equals(e.Name, memberName)) {
					return e;
				}
			}
			foreach (IField f in Fields) {
				if (cmp.Equals(f.Name, memberName)) {
					return f;
				}
			}
			foreach (IMethod m in Methods) {
				if (cmp.Equals(m.Name, memberName)) {
					return m;
				}
			}
			return null;
		}
		
		public IClass GetInnermostClass(int caretLine, int caretColumn)
		{
			foreach (IClass c in InnerClasses) {
				if (c != null && c.Region.IsInside(caretLine, caretColumn)) {
					return c.GetInnermostClass(caretLine, caretColumn);
				}
			}
			return this;
		}
		
		public List<IClass> GetAccessibleTypes(IClass callingClass)
		{
			List<IClass> types = new List<IClass>();
			List<IClass> visitedTypes = new List<IClass>();
			
			IClass currentClass = this;
			do {
				if (visitedTypes.Contains(currentClass))
					break;
				visitedTypes.Add(currentClass);
				bool isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(currentClass);
				foreach (IClass c in currentClass.InnerClasses) {
					if (c.IsAccessible(callingClass, isClassInInheritanceTree)) {
						types.Add(c);
					}
				}
				currentClass = currentClass.BaseClass;
			} while (currentClass != null);
			return types;
		}
	}
}
