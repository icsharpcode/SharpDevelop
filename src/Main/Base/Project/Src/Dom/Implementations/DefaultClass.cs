// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
		
		public ICompilationUnit CompilationUnit {
			get {
				return compilationUnit;
			}
		}
		
		public IProjectContent ProjectContent {
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
			}
			
			if (Region != null) {
				cmp = Region.CompareTo(value.Region);
				if (cmp != 0) {
					return cmp;
				}
			}
			
			cmp = DiffUtility.Compare(BaseTypes, value.BaseTypes);
			if(cmp != 0)
				return cmp;
			
			cmp = DiffUtility.Compare(InnerClasses, value.InnerClasses);
			if(cmp != 0)
				return cmp;
			
			cmp = DiffUtility.Compare(Fields, value.Fields);
			if(cmp != 0)
				return cmp;
			
			cmp = DiffUtility.Compare(Properties, value.Properties);
			if(cmp != 0)
				return cmp;
			
			cmp = DiffUtility.Compare(Indexer, value.Indexer);
			if(cmp != 0)
				return cmp;
			
			cmp = DiffUtility.Compare(Methods, value.Methods);
			if(cmp != 0)
				return cmp;
			
			return DiffUtility.Compare(Events, value.Events);
		}
		
		int IComparable.CompareTo(object o) 
		{
			return CompareTo((IClass)o);
		}
		
		public IEnumerable ClassInheritanceTree {
			get {
				return new ClassInheritanceEnumerator(this);
			}
		}
		
		protected override bool CanBeSubclass {
			get {
				return true;
			}
		}
		
		public IClass BaseClass {
			get {
				Debug.Assert(ProjectContent != null);
				
				if (BaseTypes.Count > 0) {
					IClass baseClass = ProjectContent.SearchType(BaseTypes[0], this, Region != null ? Region.BeginLine : 0, Region != null ? Region.BeginColumn : 0);
					if (baseClass != null && baseClass.ClassType != ClassType.Interface) {
						return baseClass;
					}
				}
				
				// no baseType found
				switch (ClassType) {
					case ClassType.Enum:
						return ProjectContent.GetClass("System.Enum");
					case ClassType.Class:
						if (FullyQualifiedName != "System.Object") {
							return ProjectContent.GetClass("System.Object");
						}
						break;
					case ClassType.Delegate:
						return ProjectContent.GetClass("System.Delegate");
					case ClassType.Struct:
						return ProjectContent.GetClass("System.ValueType");
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
			
			foreach (string baseClassName in BaseTypes) {
				IClass baseClass = ProjectContent.SearchType(baseClassName, this, CompilationUnit, Region != null ? Region.BeginLine : -1, Region != null ? Region.BeginColumn : -1);
				if (baseClass != null && baseClass.IsTypeInInheritanceTree(possibleBaseClass)) {
					return true;
				}
			}
			return false;
		}
		
		public IMember SearchMember(string memberName)
		{
			if (memberName == null || memberName.Length == 0) {
				return null;
			}
			foreach (IField f in Fields) {
				if (f.Name == memberName) {
					return f;
				}
			}
			foreach (IProperty p in Properties) {
				if (p.Name == memberName) {
					return p;
				}
			}
			foreach (IIndexer i in Indexer) {
				if (i.Name == memberName) {
					return i;
				}
			}
			foreach (IEvent e in Events) {
				if (e.Name == memberName) {
					return e;
				}
			}
			foreach (IMethod m in Methods) {
				if (m.Name == memberName) {
					return m;
				}
			}
			if (ClassType == ClassType.Interface) {
				foreach (string baseType in BaseTypes) {
					int line = -1;
					int col = -1;
					if (Region != null) {
						line = Region.BeginLine;
						col = Region.BeginColumn;
					}
					IClass c = ProjectContent.SearchType(baseType, this, line, col);
					if (c != null) {
						return c.SearchMember(memberName);
					}
				}
			} else {
				IClass c = BaseClass;
				return c.SearchMember(memberName);
			}
			return null;
		}
		
		public IClass GetInnermostClass(int caretLine, int caretColumn)
		{
			if (InnerClasses == null) {
				return this;
			}
			
			foreach (IClass c in InnerClasses) {
				if (c != null && c.Region != null && c.Region.IsInside(caretLine, caretColumn)) {
					return c.GetInnermostClass(caretLine, caretColumn);
				}
			}
			return this;
		}
		
		public ArrayList GetAccessibleTypes(IClass callingClass)
		{
			ArrayList types = new ArrayList();
			
			bool isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(this);
			foreach (IClass c in InnerClasses) {
				if (((c.ClassType == ClassType.Class) || (c.ClassType == ClassType.Struct)) &&
				      c.IsAccessible(callingClass, isClassInInheritanceTree)) {
					types.Add(c);
				}
			}
			IClass baseClass = BaseClass;
			if (baseClass != null) {
				types.AddRange(baseClass.GetAccessibleTypes(callingClass).ToArray());
			}
			return types;
		}
		
		public ArrayList GetAccessibleMembers(IClass callingClass, bool showStatic)
		{
			ArrayList members = new ArrayList();
			
			DateTime now = DateTime.Now;
			
			// enums must be handled specially, because there are several things defined we don't want to show
			// and enum members have neither the modifier nor the modifier public
			/*if (ClassType == ClassType.Enum) {
				foreach (IField f in Fields) {
					if (f.IsLiteral) {
						members.Add(f);
					}
				}
				members.AddRange(ProjectContent.GetClass("System.Enum").GetAccessibleMembers(callingClass, showStatic).ToArray());
				return members;
			}*/
			
			bool isClassInInheritanceTree = false;
			if (callingClass != null)
				isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(this);
			
			if (showStatic) {
				foreach (IClass c in InnerClasses) {
					if (c.IsAccessible(callingClass, isClassInInheritanceTree)) {
						members.Add(c);
					}
				}
			}
			
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
					IClass baseClass = ProjectContent.SearchType(s, this, Region != null ? Region.BeginLine : -1, Region != null ? Region.BeginColumn : -1);
					if (baseClass != null && baseClass.ClassType == ClassType.Interface) {
						members.AddRange(baseClass.GetAccessibleMembers(callingClass, showStatic).ToArray());
					}
				}
			} else {
				IClass baseClass = BaseClass;
				if (baseClass != null) {
					members.AddRange(baseClass.GetAccessibleMembers(callingClass, showStatic).ToArray());
				}
			}
			
			return members;
		}
		
		public class ClassInheritanceEnumerator : IEnumerator, IEnumerable
		{
			IClass topLevelClass;
			IClass currentClass  = null;
			Queue  baseTypeQueue = new Queue();

			public ClassInheritanceEnumerator(IClass topLevelClass)
			{
				this.topLevelClass = topLevelClass;
				baseTypeQueue.Enqueue(topLevelClass.FullyQualifiedName);
				PutBaseClassesOnStack(topLevelClass);
				baseTypeQueue.Enqueue("System.Object");
			}
			
			public IEnumerator GetEnumerator()
			{
				return this;
			}
			
			void PutBaseClassesOnStack(IClass c)
			{
				foreach (string baseTypeName in c.BaseTypes) {
					baseTypeQueue.Enqueue(baseTypeName);
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
			
			public bool MoveNext()
			{
				try {
					if (baseTypeQueue.Count == 0) {
						return false;
					}
					string baseTypeName = baseTypeQueue.Dequeue().ToString();
					
					IClass baseType = ParserService.CurrentProjectContent.GetClass(baseTypeName);
					
					// search through all usings the top level class compilation unit has.
					if (baseType == null) {
						ICompilationUnit unit = currentClass == null ? null : currentClass.CompilationUnit;
						if (unit != null) {
							foreach (IUsing u in unit.Usings) {
								baseType = u.SearchType(baseTypeName);
								if (baseType != null) {
									break;
								}
							}
						}
					}
					
					// search through all namespaces the top level class is defined in.
					if (baseType == null) {
						string[] namespaces = topLevelClass.Namespace.Split('.');
						for (int i = namespaces.Length; i > 0 && baseType == null; --i) {
							baseType = ParserService.CurrentProjectContent.GetClass(String.Join(".", namespaces, 0, i) + "." + baseTypeName);
						}
					}
					
					if (baseType != null) {
						currentClass = baseType;
						PutBaseClassesOnStack(currentClass);
					}
					return baseType != null;
				} catch (Exception e) {
					Console.WriteLine(e);
				}
				return false;
			}
			
			public void Reset()
			{
				baseTypeQueue.Clear();
				baseTypeQueue.Enqueue(topLevelClass.FullyQualifiedName);
				PutBaseClassesOnStack(topLevelClass);
				baseTypeQueue.Enqueue("System.Object");
			}
		}
	}
}
