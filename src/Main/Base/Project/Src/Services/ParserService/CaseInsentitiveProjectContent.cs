//using System;
//using System.IO;
//using System.Threading;
//using System.Collections;
//using System.Collections.Utility;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Security;
//using System.Security.Permissions;
//using System.Security.Policy;
//using System.Xml;
//using System.Text;
//
//using ICSharpCode.Core;
//using ICSharpCode.SharpDevelop.Project;
//using ICSharpCode.SharpDevelop.Gui;
//using ICSharpCode.SharpDevelop.Dom;
//
//namespace ICSharpCode.Core
//{
//	public class CaseInsensitiveProjectContent : IProjectContent
//	{
//		const string CaseInsensitiveKey = "__CASE_INSENSITIVE_HASH";		
//		
//		List<Assembly>             references             = new List<Assembly>();
//		
//		Dictionary<string, IClass> classes                = new Dictionary<string, IClass>();
//		Dictionary<string, IClass> caseInsensitiveClasses = new Dictionary<string, IClass>();
//		
//		Hashtable namespaces                = new Hashtable();
//		Hashtable caseInsensitiveNamespaces = new Hashtable();
//		
//		public static ProjectContent Create(IProject project)
//		{
//			ProjectContent newProjectContent = new ProjectContent();
//			newProjectContent.references.Add(typeof(object).Assembly);
//			foreach (ProjectItem item in project.Items) {
//				switch (item.ItemType) {
//					case ItemType.Reference:
//						newProjectContent.references.Add(Assembly.ReflectionOnlyLoadFrom(item.FileName));
//						break;
//					case ItemType.Compile:
//						ParserService.ParseFile(item.FileName);
//						break;
//				}
//			}
//			return newProjectContent;
//		}
//		
//		public Hashtable AddClassToNamespaceList(IClass addClass)
//		{
//			string nSpace = addClass.Namespace;
//			if (nSpace == null) {
//				nSpace = String.Empty;
//			}
//			
//			string[] path = nSpace.Split('.');
//			
//			lock (namespaces) {
//				Hashtable cur                = namespaces;
//				Hashtable caseInsensitiveCur = caseInsensitiveNamespaces;
//				
//				for (int i = 0; i < path.Length; ++i) {
//					object curPath   = cur[path[i]];
//					string lowerPath = path[i].ToLower();
//					if (curPath == null) {
//						Hashtable hashTable                = new Hashtable();
//						Hashtable caseInsensitivehashTable = new Hashtable();
//						cur[path[i]] = curPath = hashTable;
//						caseInsensitiveCur[lowerPath] = caseInsensitivehashTable;
//						caseInsensitivehashTable[CaseInsensitiveKey] = hashTable;
//					} else {
//						if (!(curPath is Hashtable)) {
//							return null;
//						}
//					}
//					cur = (Hashtable)curPath;
//					
//					if (!caseInsensitiveCur.ContainsKey(lowerPath)) {
//					    caseInsensitiveCur[lowerPath] = new Hashtable();
//					}
//					caseInsensitiveCur = (Hashtable)caseInsensitiveCur[lowerPath];
//				}
//				
//				string name = addClass.Name == null ? "" : addClass.Name;
//				
//				caseInsensitiveCur[name.ToLower()] = cur[name] = addClass;
//				return cur;
//			}
//		}
//			
//		public void UpdateCompilationUnit(ICompilationUnit parserOutput, string fileName, bool updateCommentTags) 
//		{
//			if (updateCommentTags) {
//				TaskService.RemoveCommentTasks(fileName);
//				if (parserOutput.TagComments.Count > 0) {
//					foreach (Tag tag in parserOutput.TagComments) {
//						TaskService.CommentTasks.Add(new Task(fileName, tag.Key + tag.CommentString, tag.Region.BeginColumn, tag.Region.BeginLine, TaskType.Comment));
//					}
//					TaskService.NotifyTaskChange();
//				}
//			}
//			
//			ICompilationUnit cu = (ICompilationUnit)parserOutput;
//			foreach (IClass c in cu.Classes) {
//				AddClassToNamespaceList(c);
//			}
//		}
//		
//		void RemoveClasses(ICompilationUnit cu)
//		{
//			if (cu != null) {
//				lock (classes) {
//					foreach (IClass c in cu.Classes) {
//							classes.Remove(c.FullyQualifiedName);
//							caseInsensitiveClasses.Remove(c.FullyQualifiedName.ToLower());
//					}
//				}
//			}
//		}
//		
//		#region Default Parser Layer dependent functions
//		public IClass GetClass(string typeName)
//		{
//			return GetClass(typeName, true);
//		}
//		public IClass GetClass(string typeName, bool caseSensitive)
//		{
//			if (!caseSensitive) {
//				typeName = typeName.ToLower();
//				if (caseInsensitiveClasses.ContainsKey(typeName)) {
//					return caseInsensitiveClasses[typeName];
//				}
//			} else {
//				if (classes.ContainsKey(typeName)) {
//					return classes[typeName];
//				}
//			}
//			
//			// not found -> maybe nested type -> trying to find class that contains this one.
//			int lastIndex = typeName.LastIndexOf('.');
//			if (lastIndex > 0) {
//				string innerName = typeName.Substring(lastIndex + 1);
//				string outerName = typeName.Substring(0, lastIndex);
//				IClass upperClass = GetClass(outerName, caseSensitive);
//				if (upperClass != null && upperClass.InnerClasses != null) {
//					foreach (IClass c in upperClass.InnerClasses) {
//						if (c.Name == innerName) {
//							return c;
//						}
//					}
//				}
//			}
//			return null;
//		}
//		
//		public string[] GetNamespaceList(string subNameSpace)
//		{
//			return GetNamespaceList(subNameSpace, true);
//		}
//		public string[] GetNamespaceList(string subNameSpace, bool caseSensitive)
//		{
//			Console.WriteLine("GET NS LIST : " + subNameSpace);
//			System.Diagnostics.Debug.Assert(subNameSpace != null);
//			if (!caseSensitive) {
//				subNameSpace = subNameSpace.ToLower();
//			}
//			
//			string[] path = subNameSpace.Split('.');
//			Hashtable cur = caseSensitive ? namespaces : caseInsensitiveNamespaces;
//			
//			if (subNameSpace.Length > 0) {
//				for (int i = 0; i < path.Length; ++i) {
//					if (!(cur[path[i]] is Hashtable)) {
//						return null;
//					}
//					cur = (Hashtable)cur[path[i]];
//				}
//			}
//			
//			if (!caseSensitive) {
//				cur = (Hashtable)cur[CaseInsensitiveKey];
//			}
//			
//			ArrayList namespaceList = new ArrayList();
//			foreach (DictionaryEntry entry in cur) {
//				if (entry.Value is Hashtable && entry.Key.ToString().Length > 0) {
//					namespaceList.Add(entry.Key);
//				}
//			}
//			
//			return (string[])namespaceList.ToArray(typeof(string));
//		}
//		
//		public ArrayList GetNamespaceContents(string subNameSpace)
//		{
//			return GetNamespaceContents(subNameSpace, true);
//		}
//		
//		public ArrayList GetNamespaceContents(string subNameSpace, bool caseSensitive)
//		{
//			Console.WriteLine("GET NS CONTENTS : " + subNameSpace);			
//			ArrayList namespaceList = new ArrayList();
//			if (subNameSpace == null) {
//				return namespaceList;
//			}
//			if (!caseSensitive) {
//				subNameSpace = subNameSpace.ToLower();
//			}
//			
//			string[] path = subNameSpace.Split('.');
//			Hashtable cur = caseSensitive ? namespaces : caseInsensitiveNamespaces;
//			
//			for (int i = 0; i < path.Length; ++i) {
//				if (!(cur[path[i]] is Hashtable)) {
//					foreach (DictionaryEntry entry in cur)  {
//						if (entry.Value is Hashtable) {
//							namespaceList.Add(entry.Key);
//						}
//					}
//					
//					return namespaceList;
//				}
//				cur = (Hashtable)cur[path[i]];
//			}
//			
//			if (!caseSensitive) {
//				cur = (Hashtable)cur[CaseInsensitiveKey];
//			}
//			
//			foreach (DictionaryEntry entry in cur) {
//				if (entry.Value is Hashtable) {
//					namespaceList.Add(entry.Key);
//				} else {
//					namespaceList.Add(entry.Value);
//				}
//			}
//			return namespaceList;
//		}
//		
//		public bool NamespaceExists(string name)
//		{
//			return NamespaceExists(name, true);
//		}
//		
//		public bool NamespaceExists(string name, bool caseSensitive)
//		{
//			Console.WriteLine("GET NamespaceExists : " + name);
//			if (name == null) {
//				return false;
//			}
//			if (!caseSensitive) {
//				name = name.ToLower();
//			}
//			string[] path = name.Split('.');
//			Hashtable cur = caseSensitive ? namespaces : caseInsensitiveNamespaces;
//			
//			for (int i = 0; i < path.Length; ++i) {
//				if (!(cur[path[i]] is Hashtable)) {
//					return false;
//				}
//				cur = (Hashtable)cur[path[i]];
//			}
//			return true;
//		}
//		
//		/// <remarks>
//		/// Returns the innerst class in which the carret currently is, returns null
//		/// if the carret is outside any class boundaries.
//		/// </remarks>
//		public IClass GetInnermostClass(ICompilationUnit cu, int caretLine, int caretColumn)
//		{
//			if (cu != null) {
//				foreach (IClass c in cu.Classes) {
//					if (c != null && c.Region != null && c.Region.IsInside(caretLine, caretColumn)) {
//						return GetInnermostClass(c, caretLine, caretColumn);
//					}
//				}
//			}
//			return null;
//		}
//		IClass GetInnermostClass(IClass curClass, int caretLine, int caretColumn)
//		{
//			if (curClass == null) {
//				return null;
//			}
//			if (curClass.InnerClasses == null) {
//				return curClass;
//			}
//			foreach (IClass c in curClass.InnerClasses) {
//				if (c != null && c.Region != null && c.Region.IsInside(caretLine, caretColumn)) {
//					return GetInnermostClass(c, caretLine, caretColumn);
//				}
//			}
//			return curClass;
//		}
//		
//		/// <remarks>
//		/// Returns all (nestet) classes in which the carret currently is exept
//		/// the innermost class, returns an empty collection if the carret is in 
//		/// no class or only in the innermost class.
//		/// the most outer class is the last in the collection.
//		/// </remarks>
//		public List<IClass> GetOuterClasses(ICompilationUnit cu, int caretLine, int caretColumn)
//		{
//			List<IClass> classes = new List<IClass>();
//			if (cu != null) {
//				foreach (IClass c in cu.Classes) {
//					if (c != null && c.Region != null && c.Region.IsInside(caretLine, caretColumn)) {
//						if (c != GetInnermostClass(cu, caretLine, caretColumn)) {
//							GetOuterClasses(classes, c, cu, caretLine, caretColumn);
//							if (!classes.Contains(c)) {
//								classes.Add(c);
//							}
//						}
//						break;
//					}
//				}
//			}
//			
//			return classes;
//		}
//		void GetOuterClasses(List<IClass> classes, IClass curClass, ICompilationUnit cu, int caretLine, int caretColumn)
//		{
//			if (curClass != null) {
//				foreach (IClass c in curClass.InnerClasses) {
//					if (c != null && c.Region != null && c.Region.IsInside(caretLine, caretColumn)) {
//						if (c != GetInnermostClass(cu, caretLine, caretColumn)) {
//							GetOuterClasses(classes, c, cu, caretLine, caretColumn);
//							if (!classes.Contains(c)) {
//								classes.Add(c);
//							}
//						}
//						break;
//					}
//				}
//			}
//		}
//		public string SearchNamespace(string name, ICompilationUnit unit, int caretLine, int caretColumn)
//		{
//			return SearchNamespace(name, unit, caretLine, caretColumn, true);
//		}
//		
//		/// <remarks>
//		/// use the usings to find the correct name of a namespace
//		/// </remarks>
//		public string SearchNamespace(string name, ICompilationUnit unit, int caretLine, int caretColumn, bool caseSensitive)
//		{
//			if (NamespaceExists(name, caseSensitive)) {
//				return name;
//			}
//			if (unit == null) {
//				return null;
//			}
//			
//			foreach (IUsing u in unit.Usings) {
//				if (u != null && (u.Region == null || u.Region.IsInside(caretLine, caretColumn))) {
//					string nameSpace = u.SearchNamespace(name, caseSensitive);
//					if (nameSpace != null) {
//						return nameSpace;
//					}
//				}
//			}
//			return null;
//		}
//		
//		/// <remarks>
//		/// use the usings and the name of the namespace to find a class
//		/// </remarks>
//		public IClass SearchType(string name, IClass curType, int caretLine, int caretColumn)
//		{
//			return SearchType(name, curType, caretLine, caretColumn, true);
//		}
//		public IClass SearchType(string name, IClass curType, int caretLine, int caretColumn, bool caseSensitive)
//		{
//			if (curType == null) {
//				return SearchType(name, null, null, caretLine, caretColumn, caseSensitive);
//			}
//			return SearchType(name, curType, curType.CompilationUnit, caretLine, caretColumn, caseSensitive);
//		}
//		
//		public IClass SearchType(string name, IClass curType, ICompilationUnit unit, int caretLine, int caretColumn)
//		{
//			return SearchType(name, curType, unit, caretLine, caretColumn, true);
//		}
//		
//		/// <remarks>
//		/// use the usings and the name of the namespace to find a class
//		/// </remarks>
//		public IClass SearchType(string name, IClass curType, ICompilationUnit unit, int caretLine, int caretColumn, bool caseSensitive)
//		{
//			if (name == null || name == String.Empty) {
//				return null;
//			}
//			IClass c  = GetClass(name, caseSensitive);
//			if (c != null) {
//				return c;
//			}
//			if (unit != null) {
//				foreach (IUsing u in unit.Usings) {
//					if (u != null && (u.Region == null || u.Region.IsInside(caretLine, caretColumn))) {
//						c = u.SearchType(name, caseSensitive);
//						if (c != null) {
//							return c;
//						}
//					}
//				}
//			}
//			if (curType == null) {
//				return null;
//			}
//			string fullname = curType.FullyQualifiedName;
//			string[] namespaces = fullname.Split('.');
//			StringBuilder curnamespace = new StringBuilder();
//			for (int i = 0; i < namespaces.Length; ++i) {
//				curnamespace.Append(namespaces[i]);
//				curnamespace.Append('.');
//				StringBuilder nms=new StringBuilder(curnamespace.ToString());
//				nms.Append(name);
//				c = GetClass(nms.ToString(), caseSensitive);
//				if (c != null) {
//					return c;
//				}
//			}
////// Alex: try to find in namespaces referenced excluding system ones which were checked already
//			string[] innamespaces=GetNamespaceList("");
//			foreach (string ns in innamespaces) {
////				if (Array.IndexOf(ParserService.assemblyList,ns)>=0) continue;
//				ArrayList objs=GetNamespaceContents(ns);
//				if (objs==null) continue;
//				foreach (object o in objs) {
//					if (o is IClass) {
//						IClass oc=(IClass)o;
//						//  || oc.Name==name
//						if (oc.FullyQualifiedName == name) {
//							//Debug.WriteLine(((IClass)o).Name);
//							/// now we can set completion data
//							objs.Clear();
//							objs = null;
//							return oc;
//						}
//					}
//				}
//				if (objs == null) {
//					break;
//				}
//			}
//			innamespaces=null;
////// Alex: end of mod
//			return null;
//		}
//		
//		/// <remarks>
//		/// Returns true, if class possibleBaseClass is in the inheritance tree from c
//		/// </remarks>
//		public bool IsClassInInheritanceTree(IClass possibleBaseClass, IClass c)
//		{
//			return IsClassInInheritanceTree(possibleBaseClass, c, true);
//		}
//		
//		public bool IsClassInInheritanceTree(IClass possibleBaseClass, IClass c, bool caseSensitive)
//		{
//			if (possibleBaseClass == null || c == null) {
//				return false;
//			}
//			if (caseSensitive && possibleBaseClass.FullyQualifiedName == c.FullyQualifiedName ||
//			    !caseSensitive && possibleBaseClass.FullyQualifiedName.ToLower() == c.FullyQualifiedName.ToLower()) {
//				return true;
//			}
//			foreach (string baseClass in c.BaseTypes) {
//				if (IsClassInInheritanceTree(possibleBaseClass, SearchType(baseClass, c, c.CompilationUnit, c.Region != null ? c.Region.BeginLine : -1, c.Region != null ? c.Region.BeginColumn : -1))) {
//					return true;
//				}
//			}
//			return false;
//		}
//		
//		public IClass BaseClass(IClass curClass)
//		{
//			return BaseClass(curClass, true);
//		}
//		
//		public IClass BaseClass(IClass curClass, bool caseSensitive)
//		{
//			foreach (string s in curClass.BaseTypes) {
//				IClass baseClass = SearchType(s, curClass, curClass.Region != null ? curClass.Region.BeginLine : 0, curClass.Region != null ? curClass.Region.BeginColumn : 0, caseSensitive);
//				if (baseClass != null && baseClass.ClassType != ClassType.Interface) {
//					return baseClass;
//				}
//			}
//			// no baseType found
//			if (curClass.ClassType == ClassType.Enum) {
//				return GetClass("System.Enum", true);
//			} else if (curClass.ClassType == ClassType.Class) {
//				if (curClass.FullyQualifiedName != "System.Object") {
//					return GetClass("System.Object", true);
//				}
//			} else if (curClass.ClassType == ClassType.Delegate) {
//				return GetClass("System.Delegate", true);
//			} else if (curClass.ClassType == ClassType.Struct) {
//				return GetClass("System.ValueType", true);
//			}
//			return null;
//		}
//		
//		bool IsInnerClass(IClass c, IClass possibleInnerClass)
//		{
//			foreach (IClass inner in c.InnerClasses) {
//				if (inner.FullyQualifiedName == possibleInnerClass.FullyQualifiedName) {
//					return true;
//				}
//				if (IsInnerClass(inner, possibleInnerClass)) {
//					return true;
//				}
//			}
//			return false;
//		}
//		
//		// TODO: check inner classes for protected members too
//		// TODO: look for FullyQualifiedName == FullyQualifiedName. Must be replaced by a function wich pays attention to the case.
//		//       Look at NRefactoryResolver.IsSameName. Also pay attention if you can put this Function in IClass, and if you have to
//		//       compare the names instead of the FullyQualifiedNames
//		public bool IsAccessible(IClass c, IDecoration member, IClass callingClass, bool isClassInInheritanceTree)
//		{
//			if ((member.Modifiers & ModifierEnum.Internal) == ModifierEnum.Internal) {
//				return true;
//			}
//			if ((member.Modifiers & ModifierEnum.Public) == ModifierEnum.Public) {
//				return true;
//			}
//			if ((member.Modifiers & ModifierEnum.Protected) == ModifierEnum.Protected && (isClassInInheritanceTree)) {
//				return true;
//			}
//			return c != null && callingClass != null && (c.FullyQualifiedName == callingClass.FullyQualifiedName || IsInnerClass(c, callingClass));
//		}
//		
//		public bool MustBeShown(IClass c, IDecoration member, IClass callingClass, bool showStatic, bool isClassInInheritanceTree)
//		{
//			if (c != null && c.ClassType == ClassType.Enum) {
//				return true;
//			}
//			if ((!showStatic &&  ((member.Modifiers & ModifierEnum.Static) == ModifierEnum.Static)) ||
//			    ( showStatic && !(((member.Modifiers & ModifierEnum.Static) == ModifierEnum.Static) ||
//			                      ((member.Modifiers & ModifierEnum.Const)  == ModifierEnum.Const)))) { // const is automatically static
//				return false;
//			}
//			return IsAccessible(c, member, callingClass, isClassInInheritanceTree);
//		}
//		
//		public ArrayList ListTypes(ArrayList types, IClass curType, IClass callingClass)
//		{
//			bool isClassInInheritanceTree = IsClassInInheritanceTree(curType, callingClass);
//			foreach (IClass c in curType.InnerClasses) {
//				if (((c.ClassType == ClassType.Class) || (c.ClassType == ClassType.Struct)) &&
//				      IsAccessible(curType, c, callingClass, isClassInInheritanceTree)) {
//					types.Add(c);
//				}
//			}
//			IClass baseClass = BaseClass(curType);
//			if (baseClass != null) {
//				ListTypes(types, baseClass, callingClass);
//			}
//			return types;
//		}
//		
//		public ArrayList ListMembers(ArrayList members, IClass curType, IClass callingClass, bool showStatic)
//		{
//			DateTime now = DateTime.Now;
//			
//			// enums must be handled specially, because there are several things defined we don't want to show
//			// and enum members have neither the modifier nor the modifier public
//			if (curType.ClassType == ClassType.Enum) {
//				foreach (IField f in curType.Fields) {
//					if (f.IsLiteral) {
//						members.Add(f);
//					}
//				}
//				ListMembers(members, GetClass("System.Enum", true), callingClass, showStatic);
//				return members;
//			}
//			
//			bool isClassInInheritanceTree = IsClassInInheritanceTree(curType, callingClass);
//			
//			if (showStatic) {
//				foreach (IClass c in curType.InnerClasses) {
//					if (IsAccessible(curType, c, callingClass, isClassInInheritanceTree)) {
//						members.Add(c);
//					}
//				}
//			}
//			
//			foreach (IProperty p in curType.Properties) {
//				if (MustBeShown(curType, p, callingClass, showStatic, isClassInInheritanceTree)) {
//					members.Add(p);
//				}
//			}
//			
//			foreach (IMethod m in curType.Methods) {
//				if (MustBeShown(curType, m, callingClass, showStatic, isClassInInheritanceTree)) {
//					members.Add(m);
//				}
//			}
//			
//			foreach (IEvent e in curType.Events) {
//				if (MustBeShown(curType, e, callingClass, showStatic, isClassInInheritanceTree)) {
//					members.Add(e);
//				}
//			}
//			
//			foreach (IField f in curType.Fields) {
//				if (MustBeShown(curType, f, callingClass, showStatic, isClassInInheritanceTree)) {
//					members.Add(f);
//				}
//			}
//			
//			if (curType.ClassType == ClassType.Interface && !showStatic) {
//				foreach (string s in curType.BaseTypes) {
//					IClass baseClass = SearchType(s, curType, curType.Region != null ? curType.Region.BeginLine : -1, curType.Region != null ? curType.Region.BeginColumn : -1);
//					if (baseClass != null && baseClass.ClassType == ClassType.Interface) {
//						ListMembers(members, baseClass, callingClass, showStatic);
//					}
//				}
//			} else {
//				IClass baseClass = BaseClass(curType);
//				if (baseClass != null) {
//					ListMembers(members, baseClass, callingClass, showStatic);
//				}
//			}
//			
//			return members;
//		}
//		
//		public IMember SearchMember(IClass declaringType, string memberName)
//		{
//			if (declaringType == null || memberName == null || memberName.Length == 0) {
//				return null;
//			}
//			foreach (IField f in declaringType.Fields) {
//				if (f.Name == memberName) {
//					return f;
//				}
//			}
//			foreach (IProperty p in declaringType.Properties) {
//				if (p.Name == memberName) {
//					return p;
//				}
//			}
//			foreach (IIndexer i in declaringType.Indexer) {
//				if (i.Name == memberName) {
//					return i;
//				}
//			}
//			foreach (IEvent e in declaringType.Events) {
//				if (e.Name == memberName) {
//					return e;
//				}
//			}
//			foreach (IMethod m in declaringType.Methods) {
//				if (m.Name == memberName) {
//					return m;
//				}
//			}
//			if (declaringType.ClassType == ClassType.Interface) {
//				foreach (string baseType in declaringType.BaseTypes) {
//					int line = -1;
//					int col = -1;
//					if (declaringType.Region != null) {
//						line = declaringType.Region.BeginLine;
//						col = declaringType.Region.BeginColumn;
//					}
//					IClass c = SearchType(baseType, declaringType, line, col);
//					if (c != null) {
//						return SearchMember(c, memberName);
//					}
//				}
//			} else {
//				IClass c = BaseClass(declaringType);
//				return SearchMember(c, memberName);
//			}
//			return null;
//		}
//		
//		public Position GetPosition(string fullMemberName)
//		{
//			string[] name = fullMemberName.Split(new char[] {'.'});
//			string curName = name[0];
//			int i = 1;
//			while (i < name.Length && NamespaceExists(curName)) {
//				curName += '.' + name[i];
//				++i;
//			}
//			Debug.Assert(i <= name.Length);
//			IClass curClass = GetClass(curName);
//			if (curClass == null) {
//				return new Position(null, -1, -1);
//			}
//			ICompilationUnit cu = curClass.CompilationUnit;
//			while (i < name.Length) {
//				List<IClass> innerClasses = curClass.InnerClasses;
//				foreach (IClass c in innerClasses) {
//					if (c.Name == name[i]) {
//						curClass = c;
//						break;
//					}
//				}
//				if (curClass.Name != name[i]) {
//					break;
//				}
//				++i;
//			}
//			if (i >= name.Length) {
//				return new Position(cu, curClass.Region != null ? curClass.Region.BeginLine : -1, curClass.Region != null ? curClass.Region.BeginColumn : -1);
//			}
//			IMember member = SearchMember(curClass, name[i]);
//			if (member == null || member.Region == null) {
//				return new Position(cu, -1, -1);
//			}
//			return new Position(cu, member.Region.BeginLine, member.Region.BeginColumn);
//		}
//		#endregion
//		
//		int GetAddedItems(ICompilationUnit original, ICompilationUnit changed, ICompilationUnit result)
//		{
//			int count = 0;
//			result.Classes.Clear();
//			count += DiffUtility.GetAddedItems(original.Classes,      changed.Classes,      result.Classes);
//			return count;
//		}
//		
//		int GetRemovedItems(ICompilationUnit original, ICompilationUnit changed, ICompilationUnit result) 
//		{
//			return GetAddedItems(changed, original, result);
//		}
//		
//		
//		void OnParseInformationAdded(ParseInformationEventArgs e)
//		{
//			if (ParseInformationAdded != null) {
//				ParseInformationAdded(null, e);
//			}
//		}
//		
//		void OnParseInformationRemoved(ParseInformationEventArgs e)
//		{
//			if (ParseInformationRemoved != null) {
//				ParseInformationRemoved(null, e);
//			}
//		}
//		
//		void OnParseInformationChanged(ParseInformationEventArgs e)
//		{
//			if (ParseInformationChanged != null) {
//				ParseInformationChanged(null, e);
//			}
//		}
//		
//		public event ParseInformationEventHandler ParseInformationAdded;
//		public event ParseInformationEventHandler ParseInformationRemoved;
//		public event ParseInformationEventHandler ParseInformationChanged;
//	}
//}
//
//
////readonly static string[] assemblyList = {
////			"Microsoft.VisualBasic",
////			"Microsoft.JScript",
////			"mscorlib",
////			"System.Data",
////			"System.Design",
////			"System.DirectoryServices",
////			"System.Drawing.Design",
////			"System.Drawing",
////			"System.EnterpriseServices",
////			"System.Management",
////			"System.Messaging",
////			"System.Runtime.Remoting",
////			"System.Runtime.Serialization.Formatters.Soap",
////
////			"System.Security",
////			"System.ServiceProcess",
////			"System.Web.Services",
////			"System.Web",
////			"System.Windows.Forms",
////			"System",
////			"System.XML"
////		};
////		
////		
