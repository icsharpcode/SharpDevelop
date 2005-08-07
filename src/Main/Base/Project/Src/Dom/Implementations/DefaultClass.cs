// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public class DefaultClass : AbstractNamedEntity, IClass, IComparable
	{
		ClassType classType;
		IRegion region;
		
		ICompilationUnit compilationUnit;
		
		List<string>    baseTypes   = null;
		
		List<IClass>    innerClasses = null;
		List<IField>    fields       = null;
		List<IProperty> properties   = null;
		List<IMethod>   methods      = null;
		List<IEvent>    events       = null;
		List<IIndexer>  indexer      = null;
		List<ITypeParameter> typeParameters = null;
		
		public DefaultClass(ICompilationUnit compilationUnit, string fullyQualifiedName) : base(null)
		{
			this.compilationUnit = compilationUnit;
			this.FullyQualifiedName = fullyQualifiedName;
		}
		
		public DefaultClass(ICompilationUnit compilationUnit, IClass declaringType) : base(declaringType)
		{
			this.compilationUnit = compilationUnit;
		}
		
		public DefaultClass(ICompilationUnit compilationUnit, ClassType classType, ModifierEnum modifiers, IRegion region, IClass declaringType) : base(declaringType)
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
				return new GetClassReturnType(ProjectContent, FullyQualifiedName);
			} else {
				return new DefaultReturnType(this);
			}
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
		
		public IRegion Region {
			get {
				return region;
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
		
		public List<string> BaseTypes {
			get {
				if (baseTypes == null) {
					baseTypes = new List<string>();
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
		
		public virtual List<IIndexer> Indexer {
			get {
				if (indexer == null) {
					indexer = new List<IIndexer>();
				}
				return indexer;
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
		
		public virtual List<ITypeParameter> TypeParameters {
			get {
				if (typeParameters == null) {
					typeParameters = new List<ITypeParameter>();
				}
				return typeParameters;
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
				if (UseInheritanceCache) {
					if (inheritanceTreeCache == null) {
						inheritanceTreeCache = new List<IClass>(new ClassInheritanceEnumerator(this));
					}
					return inheritanceTreeCache;
				} else {
					return new ClassInheritanceEnumerator(this);
				}
			}
		}
		
		protected bool UseInheritanceCache = false;
		
		protected override bool CanBeSubclass {
			get {
				return true;
			}
		}

		// used to prevent StackOverflowException because SearchType might search for inner classes in the base type
		bool blockBaseClassSearch = false;

		public IReturnType GetBaseType(int index)
		{
			if (blockBaseClassSearch)
				return null;
			blockBaseClassSearch = true;
			try {
				return ProjectContent.SearchType(BaseTypes[index], this, Region != null ? Region.BeginLine : 0, Region != null ? Region.BeginColumn : 0);
			} finally {
				blockBaseClassSearch = false;
			}
		}
		
		IClass cachedBaseClass;
		
		public IReturnType BaseType {
			get {
				IClass baseClass = cachedBaseClass;
				return (baseClass != null) ? baseClass.DefaultReturnType : null;
			}
		}
		
		public IClass BaseClass {
			get {
				Debug.Assert(ProjectContent != null);
				
				if (BaseTypes.Count > 0) {
					if (UseInheritanceCache && cachedBaseClass != null)
						return cachedBaseClass;
					IReturnType baseType = GetBaseType(0);
					IClass baseClass = (baseType != null) ? baseType.GetUnderlyingClass() : null;
					if (baseClass != null && baseClass.ClassType != ClassType.Interface) {
						if (UseInheritanceCache)
							cachedBaseClass = baseClass;
						return baseClass;
					}
				}
				
				// no baseType found
				switch (ClassType) {
					case ClassType.Class:
						if (FullyQualifiedName != "System.Object") {
							return ReflectionReturnType.Object.GetUnderlyingClass();
						}
						break;
					case ClassType.Module:
						return null;
					case ClassType.Enum:
						return ProjectContentRegistry.Mscorlib.GetClass("System.Enum");
					case ClassType.Delegate:
						return ProjectContentRegistry.Mscorlib.GetClass("System.Delegate");
					case ClassType.Struct:
						return ProjectContentRegistry.Mscorlib.GetClass("System.ValueType");
				}
				return null;
			}
		}
		
		public bool IsTypeInInheritanceTree(IClass possibleBaseClass)
		{
			if (possibleBaseClass == null) {
				return false;
			}
			if (FullyQualifiedName == possibleBaseClass.FullyQualifiedName) {
				return true;
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
			foreach (IIndexer i in Indexer) {
				if (cmp.Equals(i.Name, memberName)) {
					return i;
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
				if (c != null && c.Region != null && c.Region.IsInside(caretLine, caretColumn)) {
					return c.GetInnermostClass(caretLine, caretColumn);
				}
			}
			return this;
		}
		
		public List<IClass> GetAccessibleTypes(IClass callingClass)
		{
			List<IClass> types = new List<IClass>();
			
			bool isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(this);
			foreach (IClass c in InnerClasses) {
				if (c.IsAccessible(callingClass, isClassInInheritanceTree)) {
					types.Add(c);
				}
			}
			IClass baseClass = BaseClass;
			if (baseClass != null) {
				types.AddRange(baseClass.GetAccessibleTypes(callingClass));
			}
			return types;
		}
		
		/*
		public List<IMember> GetAccessibleMembers(IClass callingClass, bool showStatic)
		{
			List<IMember> members = new List<IMember>();
			
			bool isClassInInheritanceTree = false;
			if (callingClass != null)
				isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(this);
			
			foreach (IProperty p in Properties) {
				if (p.MustBeShown(callingClass, showStatic, isClassInInheritanceTree)) {
					members.Add(p);
				}
			}
			
			foreach (IMethod m in Methods) {
				if (m.MustBeShown(callingClass, showStatic, isClassInInheritanceTree)) {
					members.Add(m);
				}
			}
			
			foreach (IEvent e in Events) {
				if (e.MustBeShown(callingClass, showStatic, isClassInInheritanceTree)) {
					members.Add(e);
				}
			}
			
			foreach (IField f in Fields) {
				if (f.MustBeShown(callingClass, showStatic, isClassInInheritanceTree)) {
					members.Add(f);
				}
			}
			
			if (ClassType == ClassType.Interface && !showStatic) {
				foreach (string s in BaseTypes) {
					IReturnType baseType = ProjectContent.SearchType(s, this, Region != null ? Region.BeginLine : -1, Region != null ? Region.BeginColumn : -1);
					List<IMember> baseTypeMembers = new List<IMember>();
					if (baseType != null && baseType.GetUnderlyingClass() != null && baseType.GetUnderlyingClass().ClassType == ClassType.Interface) {
						//members.AddRange(baseClass.GetAccessibleMembers(callingClass, showStatic).ToArray());
						
					}
				}
			} else {
				IClass baseClass = BaseClass;
				if (baseClass != null) {
					members.AddRange(baseClass.GetAccessibleMembers(callingClass, showStatic));
				}
			}
			
			return members;
		}
		*/
		
		public class ClassInheritanceEnumerator : IEnumerator<IClass>, IEnumerable<IClass>
		{
			IClass topLevelClass;
			IClass currentClass  = null;
			
			private struct BaseType {
				internal IClass parent;
				internal string name;
				
				internal BaseType(IClass parent, string name) {
					this.parent = parent;
					this.name = name;
				}
			}
			
			Queue<BaseType> baseTypeQueue = new Queue<BaseType>();
			
			List<IClass> finishedClasses = new List<IClass>();
			
			public ClassInheritanceEnumerator(IClass topLevelClass)
			{
				this.topLevelClass = topLevelClass;
				PutBaseClassesOnStack(topLevelClass);
				baseTypeQueue.Enqueue(new BaseType(null, "System.Object"));
			}
			
			public IEnumerator<IClass> GetEnumerator()
			{
				return this;
			}
			
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this;
			}
			
			void PutBaseClassesOnStack(IClass c)
			{
				foreach (string baseTypeName in c.BaseTypes) {
					baseTypeQueue.Enqueue(new BaseType(c, baseTypeName));
				}
			}
			
			public IClass Current {
				get {
					return currentClass;
				}
			}
			
			object IEnumerator.Current {
				get {
					return currentClass;
				}
			}
			
			bool first = true;
			
			public bool MoveNext()
			{
				try {
					if (first) {
						first = false;
						currentClass = topLevelClass;
						return true;
					}
					if (baseTypeQueue.Count == 0) {
						return false;
					}
					
					BaseType baseTypeStruct = baseTypeQueue.Dequeue();
					
					IClass baseClass;
					if (baseTypeStruct.parent == null) {
						baseClass = ProjectContentRegistry.Mscorlib.GetClass(baseTypeStruct.name);
					} else {
						IReturnType baseType = baseTypeStruct.parent.ProjectContent.SearchType(baseTypeStruct.name, baseTypeStruct.parent, 1, 1);
						baseClass = (baseType != null) ? baseType.GetUnderlyingClass() : null;
					}
					if (baseClass == null || finishedClasses.Contains(baseClass)) {
						// prevent enumerating interfaces multiple times and endless loops when
						// circular inheritance is found
						return MoveNext();
					} else {
						currentClass = baseClass;
						
						finishedClasses.Add(currentClass);
						PutBaseClassesOnStack(currentClass);
						return true;
					}
				} catch (Exception e) {
					MessageService.ShowError(e);
				}
				return false;
			}
			
			public void Reset()
			{
				first = true;
				baseTypeQueue.Clear();
				finishedClasses.Clear();
				PutBaseClassesOnStack(topLevelClass);
				baseTypeQueue.Enqueue(new BaseType(null, "System.Object"));
			}
			
			public void Dispose()
			{
				baseTypeQueue = null;
				finishedClasses = null;
				topLevelClass = null;
				currentClass = null;
			}
		}
	}
}
