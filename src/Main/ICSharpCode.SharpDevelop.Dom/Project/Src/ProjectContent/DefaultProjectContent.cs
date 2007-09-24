// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class DefaultProjectContent : IProjectContent
	{
		readonly List<IProjectContent> referencedContents = new List<IProjectContent>();
		
		// we use a list of Dictionaries because we need multiple dictionaries:
		// each uses another StringComparer
		// (C#: StringComparer.InvariantCulture, VB: StringComparer.InvariantCultureCaseInsensitive)
		// new dictionaries are added to the list when required
		List<Dictionary<string, IClass>> classLists = new List<Dictionary<string, IClass>>();
		List<Dictionary<string, NamespaceStruct>> namespaces = new List<Dictionary<string, NamespaceStruct>>();
		
		XmlDoc xmlDoc = new XmlDoc();
		IUsing defaultImports;
		
		public IUsing DefaultImports {
			get {
				return defaultImports;
			}
			set {
				defaultImports = value;
			}
		}
		
		public virtual object Project {
			get {
				return null;
			}
		}
		
		/// <summary>
		/// Gets if the project content is representing the current version of the assembly.
		/// This property always returns true for ParseProjectContents but might return false
		/// for ReflectionProjectContent/CecilProjectContent if the file was changed.
		/// </summary>
		public virtual bool IsUpToDate {
			get {
				return true;
			}
		}
		
		public List<Dictionary<string, IClass>> ClassLists {
			get {
				if (classLists.Count == 0) {
					classLists.Add(new Dictionary<string, IClass>(language.NameComparer));
				}
				return classLists;
			}
		}
		
		public ICollection<string> NamespaceNames {
			get {
				return Namespaces[0].Keys;
			}
		}
		
		protected List<Dictionary<string, NamespaceStruct>> Namespaces {
			get {
				if (namespaces.Count == 0) {
					namespaces.Add(new Dictionary<string, NamespaceStruct>(language.NameComparer));
				}
				return namespaces;
			}
		}
		
		protected struct NamespaceStruct
		{
			public readonly List<IClass> Classes;
			public readonly List<string> SubNamespaces;
			
			public NamespaceStruct(string name) // struct must have a parameter
			{
				this.Classes = new List<IClass>();
				this.SubNamespaces = new List<string>();
			}
		}
		
		/// <summary>
		/// Gets the class dictionary that uses the name comparison rules of <paramref name="language"/>.
		/// </summary>
		protected Dictionary<string, IClass> GetClasses(LanguageProperties language)
		{
			for (int i = 0; i < classLists.Count; ++i) {
				if (classLists[i].Comparer == language.NameComparer)
					return classLists[i];
			}
			Dictionary<string, IClass> d;
			if (classLists.Count > 0) {
				Dictionary<string, IClass> oldList = classLists[0];
				d = new Dictionary<string, IClass>(oldList.Count, language.NameComparer);
				foreach (KeyValuePair<string, IClass> pair in oldList) {
					// don't use d.Add(), the new name language might treat two names as equal
					// that were unequal in the old dictionary
					d[pair.Key] = pair.Value;
				}
			} else {
				d = new Dictionary<string, IClass>(language.NameComparer);
			}
			classLists.Add(d);
			return d;
		}
		
		/// <summary>
		/// Gets the namespace dictionary that uses the name comparison rules of <paramref name="language"/>.
		/// </summary>
		protected Dictionary<string, NamespaceStruct> GetNamespaces(LanguageProperties language)
		{
			for (int i = 0; i < namespaces.Count; ++i) {
				if (namespaces[i].Comparer == language.NameComparer)
					return namespaces[i];
			}
			Dictionary<string, NamespaceStruct> d;
			if (namespaces.Count > 0) {
				Dictionary<string, NamespaceStruct> oldList = namespaces[0];
				d = new Dictionary<string, NamespaceStruct>(oldList.Count, language.NameComparer);
				foreach (KeyValuePair<string, NamespaceStruct> pair in oldList) {
					d.Add(pair.Key, pair.Value);
				}
			} else {
				d = new Dictionary<string, NamespaceStruct>(language.NameComparer);
			}
			namespaces.Add(d);
			return d;
		}
		
		public XmlDoc XmlDoc {
			get {
				return xmlDoc;
			}
			protected set {
				xmlDoc = value;
			}
		}
		
		public ICollection<IClass> Classes {
			get {
				lock (namespaces) {
					List<IClass> list = new List<IClass>(ClassLists[0].Count + 10);
					foreach (IClass c in ClassLists[0].Values) {
						if (c is GenericClassContainer) {
							GenericClassContainer gcc = (GenericClassContainer)c;
							list.AddRange(gcc.RealClasses);
						} else {
							list.Add(c);
						}
					}
					return list;
				}
			}
		}
		
		SystemTypes systemTypes;
		
		/// <summary>
		/// Gets a class that allows to conveniently access commonly used types in the system
		/// namespace.
		/// </summary>
		public virtual SystemTypes SystemTypes {
			get {
				if (systemTypes == null) {
					systemTypes = new SystemTypes(this);
				}
				return systemTypes;
			}
		}
		
		public ICollection<IProjectContent> ReferencedContents {
			get {
				return referencedContents;
			}
		}
		
		LanguageProperties language = LanguageProperties.CSharp;
		
		/// <summary>
		/// Gets/Sets the properties of the language this project content was written in.
		/// </summary>
		public LanguageProperties Language {
			[DebuggerStepThrough]
			get {
				return language;
			}
			set {
				if (value == null) throw new ArgumentNullException();
				language = value;
			}
		}
		
		public string GetXmlDocumentation(string memberTag)
		{
			string desc = xmlDoc.GetDocumentation(memberTag);
			if (desc != null) {
				return desc;
			}
			lock (referencedContents) {
				foreach (IProjectContent referencedContent in referencedContents) {
					desc = referencedContent.XmlDoc.GetDocumentation(memberTag);
					if (desc != null) {
						return desc;
					}
				}
			}
			return null;
		}
		
		protected static string LookupLocalizedXmlDoc(string fileName)
		{
			string xmlFileName         = Path.ChangeExtension(fileName, ".xml");
			string localizedXmlDocFile = Path.GetDirectoryName(fileName);
			localizedXmlDocFile = Path.Combine(localizedXmlDocFile, Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
			localizedXmlDocFile = Path.Combine(localizedXmlDocFile, Path.GetFileName(xmlFileName));
			if (File.Exists(localizedXmlDocFile)) {
				return localizedXmlDocFile;
			}
			if (File.Exists(xmlFileName)) {
				return xmlFileName;
			}
			return null;
		}
		
		public virtual void Dispose()
		{
			xmlDoc.Dispose();
		}
		
		public void AddClassToNamespaceList(IClass addClass)
		{
			lock (namespaces) {
				AddClassToNamespaceListInternal(addClass);
			}
			SearchClassReturnType.ClearCache();
		}
		
		/// <summary>
		/// Container class that is used when multiple classes with different type parameter
		/// count have the same class name.
		/// </summary>
		private class GenericClassContainer : DefaultClass
		{
			public GenericClassContainer(string fullyQualifiedName) : base(null, fullyQualifiedName) {}
			
			IClass[] realClasses = new IClass[4];
			
			public IEnumerable<IClass> RealClasses {
				get {
					foreach (IClass c in realClasses) {
						if (c != null) yield return c;
					}
				}
			}
			
			public int RealClassCount {
				get {
					int count = 0;
					foreach (IClass c in realClasses) {
						if (c != null) count += 1;
					}
					return count;
				}
			}
			
			public IClass Get(int typeParameterCount)
			{
				if (realClasses.Length > typeParameterCount)
					return realClasses[typeParameterCount];
				else
					return null;
			}
			
			public IClass GetBest(int typeParameterCount)
			{
				IClass c;
				for (int i = typeParameterCount; i < realClasses.Length; i++) {
					c = Get(i);
					if (c != null) return c;
				}
				for (int i = typeParameterCount - 1; i >= 0; i--) {
					c = Get(i);
					if (c != null) return c;
				}
				return null;
			}
			
			public void Set(IClass c)
			{
				int typeParameterCount = c.TypeParameters.Count;
				if (realClasses.Length <= typeParameterCount) {
					IClass[] newArray = new IClass[typeParameterCount + 2];
					realClasses.CopyTo(newArray, 0);
					realClasses = newArray;
				}
				realClasses[typeParameterCount] = c;
			}
			
			public void Remove(int typeParameterCount)
			{
				if (realClasses.Length > typeParameterCount)
					realClasses[typeParameterCount] = null;
			}
		}
		
		protected void AddClassToNamespaceListInternal(IClass addClass)
		{
			string fullyQualifiedName = addClass.FullyQualifiedName;
			IClass existingClass = GetClassInternal(fullyQualifiedName, addClass.TypeParameters.Count, language);
			if (existingClass != null && existingClass.TypeParameters.Count == addClass.TypeParameters.Count) {
				//LoggingService.Debug("Adding partial class " + addClass.Name + " from " + Path.GetFileName(addClass.CompilationUnit.FileName));
				CompoundClass compound = existingClass as CompoundClass;
				if (compound != null) {
					// mark the class as partial
					// (VB allows specifying the 'partial' modifier only on one part)
					addClass.IsPartial = true;
					
					// possibly replace existing class (look for CU with same filename)
					lock (compound) {
						for (int i = 0; i < compound.parts.Count; i++) {
							if (compound.parts[i].CompilationUnit.FileName == addClass.CompilationUnit.FileName) {
								compound.parts[i] = addClass;
								compound.UpdateInformationFromParts();
								//LoggingService.Debug("Replaced old part!");
								return;
							}
						}
						compound.parts.Add(addClass);
						compound.UpdateInformationFromParts();
					}
					//LoggingService.Debug("Added new part!");
					return;
				} else {
					
					if (addClass.IsPartial || language.ImplicitPartialClasses) {
						LoggingService.Info("Duplicate class " + fullyQualifiedName);
					}
					
					// Instead of overwriting a class with another, treat both parts as partial.
					// This fixes SD2-1217.
					
					// Merge existing non-partial class with addClass
					
					// Ensure partial modifier is set everywhere:
					addClass.IsPartial = true;
					existingClass.IsPartial = true;
					
					addClass = compound = new CompoundClass(addClass);
					compound.parts.Add(existingClass);
					compound.UpdateInformationFromParts();
				}
			} else if (addClass.IsPartial) {
				addClass = new CompoundClass(addClass);
				//LoggingService.Debug("Compound created!");
			}
			
			IClass oldDictionaryClass;
			if (GetClasses(language).TryGetValue(fullyQualifiedName, out oldDictionaryClass)) {
				GenericClassContainer gcc = oldDictionaryClass as GenericClassContainer;
				if (gcc != null) {
					gcc.Set(addClass);
					return;
				} else if (oldDictionaryClass.TypeParameters.Count != addClass.TypeParameters.Count) {
					gcc = new GenericClassContainer(fullyQualifiedName);
					gcc.Set(addClass);
					gcc.Set(oldDictionaryClass);
					addClass = gcc;
				}
			}
			
			foreach (Dictionary<string, IClass> classes in ClassLists) {
				classes[addClass.FullyQualifiedName] = addClass;
			}
			string nSpace = addClass.Namespace;
			if (nSpace == null) {
				nSpace = String.Empty;
			}
			CreateNamespace(nSpace);
			List<IClass> classList = GetNamespaces(this.language)[nSpace].Classes;
			for (int i = 0; i < classList.Count; i++) {
				if (classList[i].FullyQualifiedName == addClass.FullyQualifiedName) {
					classList[i] = addClass;
					return;
				}
			}
			classList.Add(addClass);
		}
		
		void CreateNamespace(string nSpace)
		{
			Dictionary<string, NamespaceStruct> dict = GetNamespaces(this.language);
			if (dict.ContainsKey(nSpace))
				return;
			NamespaceStruct namespaceStruct = new NamespaceStruct(nSpace);
			dict.Add(nSpace, namespaceStruct);
			// use the same namespaceStruct for all dictionaries
			foreach (Dictionary<string, NamespaceStruct> otherDict in namespaces) {
				if (otherDict == dict) continue;
				otherDict.Add(nSpace, namespaceStruct);
			}
			if (nSpace.Length == 0)
				return;
			// add to parent namespace
			int pos = nSpace.LastIndexOf('.');
			string parent;
			string subNs;
			if (pos < 0) {
				parent = "";
				subNs = nSpace;
			} else {
				parent = nSpace.Substring(0, pos);
				subNs = nSpace.Substring(pos + 1);
			}
			CreateNamespace(parent);
			dict[parent].SubNamespaces.Add(subNs);
		}
		
		/// <summary>
		/// Removes the specified namespace from all namespace lists if the namespace is empty.
		/// </summary>
		void RemoveEmptyNamespace(string nSpace)
		{
			if (nSpace == null || nSpace.Length == 0) return;
			Dictionary<string, NamespaceStruct> dict = GetNamespaces(this.language);
			if (!dict.ContainsKey(nSpace))
				return;
			// remove only if really empty
			if (dict[nSpace].Classes.Count > 0 || dict[nSpace].SubNamespaces.Count > 0)
				return;
			// remove the namespace from all dictionaries
			foreach (Dictionary<string, NamespaceStruct> anyDict in namespaces) {
				anyDict.Remove(nSpace);
			}
			// remove the namespace from parent's SubNamespaces list
			int pos = nSpace.LastIndexOf('.');
			string parent;
			string subNs;
			if (pos < 0) {
				parent = "";
				subNs = nSpace;
			} else {
				parent = nSpace.Substring(0, pos);
				subNs = nSpace.Substring(pos + 1);
			}
			dict[parent].SubNamespaces.Remove(subNs);
			RemoveEmptyNamespace(parent); // remove parent if also empty
		}
		
		List<IAttribute> assemblyAttributes = new List<IAttribute>();
		
		public virtual IList<IAttribute> GetAssemblyAttributes()
		{
			lock (namespaces) {
				return assemblyAttributes.ToArray();
			}
		}
		
		public void RemoveCompilationUnit(ICompilationUnit unit)
		{
			lock (namespaces) {
				foreach (IClass c in unit.Classes) {
					RemoveClass(c);
				}
			}
			SearchClassReturnType.ClearCache();
		}
		
		public void UpdateCompilationUnit(ICompilationUnit oldUnit, ICompilationUnit parserOutput, string fileName)
		{
			lock (namespaces) {
				if (oldUnit != null) {
					RemoveClasses(oldUnit, parserOutput);
				}
				
				foreach (IClass c in parserOutput.Classes) {
					AddClassToNamespaceListInternal(c);
				}
			}
			SearchClassReturnType.ClearCache();
		}
		
		void RemoveClasses(ICompilationUnit oldUnit, ICompilationUnit newUnit)
		{
			foreach (IClass c in oldUnit.Classes) {
				bool found = false;
				// Partial classes always have to be removed. Otherwise editing the type
				// arguments of a partial would leave the class registered in the wrong compound
				// class. See SD2-1149.
				if (!c.IsPartial) {
					foreach (IClass c2 in newUnit.Classes) {
						if (c.FullyQualifiedName == c2.FullyQualifiedName) {
							found = true;
							break;
						}
					}
				}
				if (!found) {
					RemoveClass(c);
				}
			}
		}
		
		void RemoveClass(IClass @class)
		{
			string fullyQualifiedName = @class.FullyQualifiedName;
			int typeParameterCount = @class.TypeParameters.Count;
			if (@class.IsPartial) {
				// remove a part of a partial class
				// Use "as" cast to fix SD2-680: the stored class might be a part not marked as partial
				CompoundClass compound = GetClassInternal(fullyQualifiedName, typeParameterCount, language) as CompoundClass;
				if (compound == null) return;
				typeParameterCount = compound.TypeParameters.Count;
				lock (compound) {
					compound.parts.Remove(@class);
					if (compound.parts.Count > 0) {
						compound.UpdateInformationFromParts();
						return;
					} else {
						@class = compound; // all parts removed, remove compound class
					}
				}
			}
			
			IClass classInDictionary;
			if (!GetClasses(language).TryGetValue(fullyQualifiedName, out classInDictionary)) {
				return;
			}
			
			GenericClassContainer gcc = classInDictionary as GenericClassContainer;
			if (gcc != null) {
				gcc.Remove(typeParameterCount);
				if (gcc.RealClassCount > 0) {
					return;
				}
			}
			
			foreach (Dictionary<string, IClass> classes in ClassLists) {
				classes.Remove(fullyQualifiedName);
			}
			
			string nSpace = @class.Namespace;
			if (nSpace == null) {
				nSpace = String.Empty;
			}
			
			// Remove class from namespace lists
			List<IClass> classList = GetNamespaces(this.language)[nSpace].Classes;
			for (int i = 0; i < classList.Count; i++) {
				if (language.NameComparer.Equals(classList[i].FullyQualifiedName, fullyQualifiedName)) {
					classList.RemoveAt(i);
					break;
				}
			}
			if (classList.Count == 0) {
				RemoveEmptyNamespace(nSpace);
			}
		}
		
		#region Default Parser Layer dependent functions
		public IClass GetClass(string typeName)
		{
			return GetClass(typeName, 0);
		}
		
		public IClass GetClass(string typeName, int typeParameterCount)
		{
			return GetClass(typeName, typeParameterCount, language, true);
		}
		
		protected IClass GetClassInternal(string typeName, int typeParameterCount, LanguageProperties language)
		{
			lock (namespaces) {
				IClass c;
				if (GetClasses(language).TryGetValue(typeName, out c)) {
					GenericClassContainer gcc = c as GenericClassContainer;
					if (gcc != null) {
						return gcc.GetBest(typeParameterCount);
					}
					return c;
				}
				return null;
			}
		}
		
		public IClass GetClass(string typeName, int typeParameterCount, LanguageProperties language, bool lookInReferences)
		{
			IClass c = GetClassInternal(typeName, typeParameterCount, language);
			if (c != null && c.TypeParameters.Count == typeParameterCount) {
				return c;
			}
			
			// Search in references:
			if (lookInReferences) {
				lock (referencedContents) {
					foreach (IProjectContent content in referencedContents) {
						IClass contentClass = content.GetClass(typeName, typeParameterCount, language, false);
						if (contentClass != null) {
							if (contentClass.TypeParameters.Count == typeParameterCount) {
								return contentClass;
							} else {
								c = contentClass;
							}
						}
					}
				}
			}
			
			if (c != null) {
				return c;
			}
			
			// not found -> maybe nested type -> trying to find class that contains this one.
			int lastIndex = typeName.LastIndexOf('.');
			if (lastIndex > 0) {
				string outerName = typeName.Substring(0, lastIndex);
				IClass upperClass = GetClassInternal(outerName, typeParameterCount, language);
				if (upperClass != null) {
					foreach (IClass upperBaseClass in upperClass.ClassInheritanceTree) {
						List<IClass> innerClasses = upperBaseClass.InnerClasses;
						if (innerClasses != null) {
							string innerName = typeName.Substring(lastIndex + 1);
							foreach (IClass innerClass in innerClasses) {
								if (language.NameComparer.Equals(innerClass.Name, innerName)) {
									return innerClass;
								}
							}
						}
					}
				}
			}
			return null;
		}
		
		public ArrayList GetNamespaceContents(string nameSpace)
		{
			ArrayList namespaceList = new ArrayList();
			AddNamespaceContents(namespaceList, nameSpace, language, true);
			return namespaceList;
		}
		
		/// <summary>
		/// Adds the contents of the specified <paramref name="nameSpace"/> to the <paramref name="list"/>.
		/// </summary>
		public void AddNamespaceContents(ArrayList list, string nameSpace, LanguageProperties language, bool lookInReferences)
		{
			if (nameSpace == null) {
				return;
			}
			
			if (lookInReferences) {
				lock (referencedContents) {
					foreach (IProjectContent content in referencedContents) {
						content.AddNamespaceContents(list, nameSpace, language, false);
					}
				}
			}
			
			Dictionary<string, NamespaceStruct> dict = GetNamespaces(language);
			if (dict.ContainsKey(nameSpace)) {
				NamespaceStruct ns = dict[nameSpace];
				int newCapacity = list.Count + ns.Classes.Count + ns.SubNamespaces.Count;
				if (list.Capacity < newCapacity)
					list.Capacity = Math.Max(list.Count * 2, newCapacity);
				foreach (IClass c in ns.Classes) {
					if (c is GenericClassContainer) {
						foreach (IClass realClass in ((GenericClassContainer)c).RealClasses) {
							AddNamespaceContentsClass(list, realClass, language, lookInReferences);
						}
					} else {
						AddNamespaceContentsClass(list, c, language, lookInReferences);
					}
				}
				foreach (string subns in ns.SubNamespaces) {
					if (!list.Contains(subns))
						list.Add(subns);
				}
			}
		}
		
		void AddNamespaceContentsClass(ArrayList list, IClass c, LanguageProperties language, bool lookInReferences)
		{
			if (c.IsInternal && !lookInReferences) {
				// internal class and we are looking at it from another project content
				return;
			}
			if (language.ShowInNamespaceCompletion(c))
				list.Add(c);
			if (language.ImportModules && c.ClassType == ClassType.Module) {
				foreach (IMember m in c.Methods) {
					if (m.IsAccessible(null, false))
						list.Add(m);
				}
				foreach (IMember m in c.Events) {
					if (m.IsAccessible(null, false))
						list.Add(m);
				}
				foreach (IMember m in c.Fields) {
					if (m.IsAccessible(null, false))
						list.Add(m);
				}
				foreach (IMember m in c.Properties) {
					if (m.IsAccessible(null, false))
						list.Add(m);
				}
			}
		}
		
		public bool NamespaceExists(string name)
		{
			return NamespaceExists(name, language, true);
		}
		
		public bool NamespaceExists(string name, LanguageProperties language, bool lookInReferences)
		{
			if (name == null) {
				return false;
			}
			
			if (lookInReferences) {
				lock (referencedContents) {
					foreach (IProjectContent content in referencedContents) {
						if (content.NamespaceExists(name, language, false)) {
							return true;
						}
					}
				}
			}
			
			return GetNamespaces(language).ContainsKey(name);
		}
		
		
		public string SearchNamespace(string name, IClass curType, ICompilationUnit unit, int caretLine, int caretColumn)
		{
			if (NamespaceExists(name)) {
				return name;
			}
			
			if (unit == null) {
				return null;
			}
			
			foreach (IUsing u in unit.Usings) {
				if (u != null) {
					string nameSpace = u.SearchNamespace(name);
					if (nameSpace != null) {
						return nameSpace;
					}
				}
			}
			if (defaultImports != null) {
				string nameSpace = defaultImports.SearchNamespace(name);
				if (nameSpace != null) {
					return nameSpace;
				}
			}
			if (curType != null) {
				// Try relative to current namespace and relative to parent namespaces
				string fullname = curType.Namespace;
				while (fullname != null && fullname.Length > 0) {
					string nameSpace = fullname + '.' + name;
					if (NamespaceExists(nameSpace)) {
						return nameSpace;
					}
					
					int pos = fullname.LastIndexOf('.');
					if (pos < 0) {
						fullname = null;
					} else {
						fullname = fullname.Substring(0, pos);
					}
				}
			}
			return null;
		}
		
		public SearchTypeResult SearchType(SearchTypeRequest request)
		{
			string name = request.Name;
			if (name == null || name.Length == 0) {
				return SearchTypeResult.Empty;
			}
			
			// Try if name is already the full type name
			IClass c = GetClass(name, request.TypeParameterCount);
			if (c != null) {
				return new SearchTypeResult(c.DefaultReturnType);
			}
			// fallback-class if the one with the right type parameter count is not found.
			SearchTypeResult fallbackResult = SearchTypeResult.Empty;
			if (request.CurrentType != null) {
				// Try parent namespaces of the current class
				string fullname = request.CurrentType.Namespace;
				while (fullname != null && fullname.Length > 0) {
					string nameSpace = fullname + '.' + name;
					
					c = GetClass(nameSpace, request.TypeParameterCount);
					if (c != null) {
						if (c.TypeParameters.Count == request.TypeParameterCount)
							return new SearchTypeResult(c.DefaultReturnType);
						else
							fallbackResult = new SearchTypeResult(c.DefaultReturnType);
					}
					
					int pos = fullname.LastIndexOf('.');
					if (pos < 0) {
						fullname = null;
					} else {
						fullname = fullname.Substring(0, pos);
					}
				}
				
				if (name.IndexOf('.') < 0) {
					// Try inner classes (in full inheritance tree)
					// Don't use loop with cur = cur.BaseType because of inheritance cycles
					foreach (IClass baseClass in request.CurrentType.ClassInheritanceTree) {
						if (baseClass.ClassType == ClassType.Class) {
							foreach (IClass innerClass in baseClass.InnerClasses) {
								if (language.NameComparer.Equals(innerClass.Name, name))
									return new SearchTypeResult(innerClass.DefaultReturnType);
							}
						}
					}
				}
			}
			if (request.CurrentCompilationUnit != null) {
				// Combine name with usings
				foreach (IUsing u in request.CurrentCompilationUnit.Usings) {
					if (u != null) {
						foreach (IReturnType r in u.SearchType(name, request.TypeParameterCount)) {
							if (r.TypeArgumentCount == request.TypeParameterCount) {
								return new SearchTypeResult(r, u);
							} else {
								fallbackResult = new SearchTypeResult(r, u);
							}
						}
					}
				}
			}
			if (defaultImports != null) {
				foreach (IReturnType r in defaultImports.SearchType(name, request.TypeParameterCount)) {
					if (r.TypeArgumentCount == request.TypeParameterCount) {
						return new SearchTypeResult(r, defaultImports);
					} else {
						fallbackResult = new SearchTypeResult(r, defaultImports);
					}
				}
			}
			return fallbackResult;
		}
		
		IClass GetClassByDotNetName(string className, bool lookInReferences)
		{
			className = className.Replace('+', '.');
			if (className.Length > 2 && className[className.Length - 2] == '`') {
				int typeParameterCount = className[className.Length - 1] - '0';
				if (typeParameterCount < 0) typeParameterCount = 0;
				className = className.Substring(0, className.Length - 2);
				return GetClass(className, typeParameterCount, LanguageProperties.CSharp, lookInReferences);
			} else {
				return GetClass(className, 0, LanguageProperties.CSharp, lookInReferences);
			}
		}
		
		/// <summary>
		/// Gets the position of a member in this project content (not a referenced one).
		/// </summary>
		/// <param name="fullMemberName">The full member name in Reflection syntax (always case sensitive, ` for generics)</param>
		public IDecoration GetElement(string fullMemberName)
		{
			IClass curClass = GetClassByDotNetName(fullMemberName, false);
			if (curClass != null) {
				return curClass;
			}
			int pos = fullMemberName.IndexOf('(');
			if (pos > 0) {
				// is method call
				
				int colonPos = fullMemberName.LastIndexOf(':');
				if (colonPos > 0) {
					fullMemberName = fullMemberName.Substring(0, colonPos);
				}
				
				string memberName = fullMemberName.Substring(0, pos);
				int pos2 = memberName.LastIndexOf('.');
				if (pos2 > 0) {
					string className = memberName.Substring(0, pos2);
					memberName = memberName.Substring(pos2 + 1);
					curClass = GetClassByDotNetName(className, false);
					if (curClass != null) {
						IMethod firstMethod = null;
						foreach (IMethod m in curClass.Methods) {
							if (m.Name == memberName) {
								if (firstMethod == null) firstMethod = m;
								StringBuilder dotnetName = new StringBuilder(m.DotNetName);
								dotnetName.Append('(');
								for (int i = 0; i < m.Parameters.Count; i++) {
									if (i > 0) dotnetName.Append(',');
									if (m.Parameters[i].ReturnType != null) {
										dotnetName.Append(m.Parameters[i].ReturnType.DotNetName);
									}
								}
								dotnetName.Append(')');
								if (dotnetName.ToString() == fullMemberName) {
									return m;
								}
							}
						}
						return firstMethod;
					}
				}
			} else {
				pos = fullMemberName.LastIndexOf('.');
				if (pos > 0) {
					string className = fullMemberName.Substring(0, pos);
					string memberName = fullMemberName.Substring(pos + 1);
					curClass = GetClassByDotNetName(className, false);
					if (curClass != null) {
						// get first method with that name, but prefer method without parameters
						IMethod firstMethod = null;
						foreach (IMethod m in curClass.Methods) {
							if (m.Name == memberName) {
								if (firstMethod == null || m.Parameters.Count == 0)
									firstMethod = m;
							}
						}
						if (firstMethod != null)
							return firstMethod;
						return curClass.SearchMember(memberName, LanguageProperties.CSharp);
					}
				}
			}
			return null;
		}
		
		public FilePosition GetPosition(string fullMemberName)
		{
			IDecoration d = GetElement(fullMemberName);
			IMember m = d as IMember;
			IClass c = d as IClass;
			if (m != null) {
				return new FilePosition(m.DeclaringType.CompilationUnit, m.Region.BeginLine, m.Region.BeginColumn);
			} else if (c != null) {
				return new FilePosition(c.CompilationUnit, c.Region.BeginLine, c.Region.BeginColumn);
			} else {
				return FilePosition.Empty;
			}
		}
		#endregion
		
		public void AddReferencedContent(IProjectContent pc)
		{
			if (pc != null) {
				lock (this.ReferencedContents) {
					this.ReferencedContents.Add(pc);
				}
			}
		}
		
		public event EventHandler ReferencedContentsChanged;
		
		protected virtual void OnReferencedContentsChanged(EventArgs e)
		{
			systemTypes = null; // re-create system types
			SearchClassReturnType.ClearCache();
			if (ReferencedContentsChanged != null) {
				ReferencedContentsChanged(this, e);
			}
		}
		
		public static readonly IProjectContent DummyProjectContent = new DummyContent();
		
		private class DummyContent : DefaultProjectContent
		{
			public override string ToString()
			{
				return "[DummyProjectContent]";
			}
			
			public override SystemTypes SystemTypes {
				get {
					return HostCallback.GetCurrentProjectContent().SystemTypes;
				}
			}
		}
	}
}
