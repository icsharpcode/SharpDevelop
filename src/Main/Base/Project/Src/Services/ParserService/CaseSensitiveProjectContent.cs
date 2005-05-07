using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Xml;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.Core
{
	public class DefaultProjectContent : IProjectContent
	{
		List<IProjectContent>      referencedContents = new List<IProjectContent>();
		
		List<Dictionary<string, IClass>> classLists = new List<Dictionary<string, IClass>>();
		Hashtable                  namespaces         = new Hashtable();
		XmlDoc                     xmlDoc             = new XmlDoc();
		
		public List<Dictionary<string, IClass>> ClassLists {
			get {
				if (classLists.Count == 0) {
					classLists.Add(new Dictionary<string, IClass>(language.NameComparer));
				}
				return classLists;
			}
		}
		
		Dictionary<string, IClass> GetClasses(LanguageProperties language)
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
					d.Add(pair.Key, pair.Value);
				}
			} else {
				d = new Dictionary<string, IClass>(language.NameComparer);
			}
			classLists.Add(d);
			return d;
		}
		
		public XmlDoc XmlDoc {
			get {
				return xmlDoc;
			}
		}
		
		public ICollection<IClass> Classes {
			get {
				return ClassLists[0].Values;
			}
		}
		
		LanguageProperties language = LanguageProperties.CSharp;
		
		/// <summary>
		/// Gets the properties of the language this project content was written in.
		/// </summary>
		public LanguageProperties Language {
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
			if (xmlDoc.XmlDescription.ContainsKey(memberTag)) {
				return xmlDoc.XmlDescription[memberTag];
			}
			foreach (IProjectContent referencedContent in referencedContents) {
				if (referencedContent.XmlDoc.XmlDescription.ContainsKey(memberTag)) {
					return referencedContent.XmlDoc.XmlDescription[memberTag];
				}
			}
			return null;
		}
		
		static string LookupLocalizedXmlDoc(string fileName)
		{
			string xmlFileName         = Path.ChangeExtension(fileName, ".xml");
			string localizedXmlDocFile = FileUtility.Combine(System.IO.Path.GetDirectoryName(fileName), Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, System.IO.Path.GetFileName(xmlFileName));
			if (File.Exists(localizedXmlDocFile)) {
				return localizedXmlDocFile;
			}
			if (File.Exists(xmlFileName)) {
				return xmlFileName;
			}
			return null;
		}
		
		public static IProjectContent Create(Assembly assembly)
		{
			DefaultProjectContent newProjectContent = new DefaultProjectContent();
			
			ICompilationUnit assemblyCompilationUnit = new DefaultCompilationUnit(newProjectContent);
			
			foreach (Type type in assembly.GetTypes()) {
				if (!type.FullName.StartsWith("<") && type.IsPublic) {
					newProjectContent.AddClassToNamespaceListInternal(new ReflectionClass(assemblyCompilationUnit, type, null));
				}
			}
			string fileName = LookupLocalizedXmlDoc(assembly.Location);
			// Not found -> look in runtime directory.
			if (fileName == null) {
				string runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
				fileName = LookupLocalizedXmlDoc(Path.Combine(runtimeDirectory, Path.GetFileName(assembly.Location)));
			}
			
			if (fileName != null) {
				newProjectContent.xmlDoc = XmlDoc.Load(fileName);
			}
			
			return newProjectContent;
		}
		
		internal static IProjectContent CreateUninitalized(IProject project)
		{
			DefaultProjectContent newProjectContent = new DefaultProjectContent();
			newProjectContent.project = project;
			newProjectContent.language = project.LanguageProperties;
			newProjectContent.referencedContents.Add(ProjectContentRegistry.GetMscorlibContent());
			return newProjectContent;
		}
		
		public static IProjectContent Create(IProject project)
		{
			IProjectContent newProjectContent = CreateUninitalized(project);
			if (newProjectContent is DefaultProjectContent) {
				((DefaultProjectContent)newProjectContent).Initialize1();
				((DefaultProjectContent)newProjectContent).Initialize2();
			}
			return newProjectContent;
		}
		
		IProject project;
		// project is only used for initialization, the field is set to null after
		// initialization has completed.
		
		internal void Initialize1()
		{
			foreach (ProjectItem item in project.Items.ToArray()) {
				switch (item.ItemType) {
					case ItemType.Reference:
					case ItemType.ProjectReference:
						IProjectContent referencedContent = ProjectContentRegistry.GetProjectContentForReference(item as ReferenceProjectItem);
						if (referencedContent != null) {
							referencedContents.Add(referencedContent);
						}
						break;
				}
			}
		}
		
		internal void Initialize2()
		{
			foreach (ProjectItem item in project.Items.ToArray()) {
				if (item.ItemType == ItemType.Compile) {
					ParseInformation parseInfo = ParserService.ParseFile(item.FileName, null, true, false);
					if (parseInfo != null) {
						UpdateCompilationUnit(null, parseInfo.BestCompilationUnit as ICompilationUnit, item.FileName, true);
					}
				}
			}
			project = null;
		}
		
		public Hashtable AddClassToNamespaceList(IClass addClass)
		{
			lock (namespaces) {
				return AddClassToNamespaceListInternal(addClass);
			}
		}
		
		Hashtable AddClassToNamespaceListInternal(IClass addClass)
		{
			foreach (Dictionary<string, IClass> classes in ClassLists) {
				classes[addClass.FullyQualifiedName] = addClass;
			}
			string nSpace = addClass.Namespace;
			if (nSpace == null) {
				nSpace = String.Empty;
			}
			
			Hashtable cur = namespaces;
			
			if (nSpace.Length > 0) {
				string[] path = nSpace.Split('.');
				for (int i = 0; i < path.Length; ++i) {
					object curPath   = cur[path[i]];
					if (curPath == null) {
						Hashtable hashTable = new Hashtable();
						cur[path[i]] = curPath = hashTable;
					} else {
						if (!(curPath is Hashtable)) {
							return null;
						}
					}
					cur = (Hashtable)curPath;
				}
			}
			
			string name = addClass.Name == null ? "" : addClass.Name;
			
			cur[name] = addClass;
			return cur;
		}
		
		public void UpdateCompilationUnit(ICompilationUnit oldUnit, ICompilationUnit parserOutput, string fileName, bool updateCommentTags)
		{
			
			if (updateCommentTags) {
				TaskService.UpdateCommentTags(fileName, parserOutput.TagComments);
			}
			
			lock (namespaces) {
				if (oldUnit != null) {
					RemoveClasses(oldUnit);
				}
				
				ICompilationUnit cu = (ICompilationUnit)parserOutput;
				foreach (IClass c in cu.Classes) {
					AddClassToNamespaceListInternal(c);
				}
			}
		}
		
		void RemoveClasses(ICompilationUnit cu)
		{
			if (cu != null) {
				foreach (IClass c in cu.Classes) {
					foreach (Dictionary<string, IClass> classes in ClassLists) {
						classes.Remove(c.FullyQualifiedName);
					}
				}
				// TODO: remove classes from namespace lists
			}
		}
		
		#region Default Parser Layer dependent functions
		public IClass GetClass(string typeName)
		{
			return GetClass(typeName, language, true);
		}
		
		public IClass GetClass(string typeName, LanguageProperties language, bool lookInReferences)
		{
//			Console.WriteLine("GetClass({0}) is known:{1}", typeName, classes.ContainsKey(typeName));
			Dictionary<string, IClass> classes = GetClasses(language);
			if (classes.ContainsKey(typeName)) {
				return classes[typeName];
			}
			
			// Search in references:
			if (lookInReferences) {
				foreach (IProjectContent content in referencedContents) {
					IClass classFromContent = content.GetClass(typeName, language, false);
					if (classFromContent != null) {
						return classFromContent;
					}
				}
			}
			
			// not found -> maybe nested type -> trying to find class that contains this one.
			int lastIndex = typeName.LastIndexOf('.');
			if (lastIndex > 0) {
				string outerName = typeName.Substring(0, lastIndex);
				if (classes.ContainsKey(outerName)) {
					IClass upperClass = classes[outerName];
					if (upperClass.InnerClasses != null) {
						string innerName = typeName.Substring(lastIndex + 1);
						foreach (IClass c in upperClass.InnerClasses) {
							if (language.NameComparer.Equals(c.Name, innerName)) {
								return c;
							}
						}
					}
				}
			}
			return null;
		}
		
		public ArrayList GetNamespaceContents(string subNameSpace)
		{
			ArrayList namespaceList = new ArrayList();
			AddNamespaceContents(namespaceList, subNameSpace, language, true);
			return namespaceList;
		}
		
		public void AddNamespaceContents(ArrayList list, string subNameSpace, LanguageProperties language, bool lookInReferences)
		{
			if (subNameSpace == null) {
				return;
			}
			
			if (lookInReferences) {
				foreach (IProjectContent content in referencedContents) {
					content.AddNamespaceContents(list, subNameSpace, language, false);
				}
			}
			
			Hashtable cur = namespaces;
			
			if (subNameSpace.Length > 0) {
				string[] path = subNameSpace.Split('.');
				for (int i = 0; i < path.Length; ++i) {
					if (!(cur[path[i]] is Hashtable)) {
						// namespace does not exist in this project content
						return;
					}
					cur = (Hashtable)cur[path[i]];
				}
			}
			
			foreach (DictionaryEntry entry in cur) {
				if (entry.Value is Hashtable) {
					if (!list.Contains(entry.Key))
						list.Add(entry.Key);
				} else {
					if (!list.Contains(entry.Value))
						list.Add(entry.Value);
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
				foreach (IProjectContent content in referencedContents) {
					if (content.NamespaceExists(name, language, false)) {
						return true;
					}
				}
			}
			
			string[] path = name.Split('.');
			Hashtable cur = namespaces;
			for (int i = 0; i < path.Length; ++i) {
				if (!(cur[path[i]] is Hashtable)) {
					return false;
				}
				cur = (Hashtable)cur[path[i]];
			}
			return true;
		}
		
		
		public string SearchNamespace(string name, ICompilationUnit unit, int caretLine, int caretColumn)
		{
//			Console.WriteLine("SearchNamespace({0})", name);
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
			return null;
		}
		
		public IClass SearchType(string name, IClass curType, int caretLine, int caretColumn)
		{
			if (curType == null) {
				return SearchType(name, null, null, caretLine, caretColumn);
			}
			return SearchType(name, curType, curType.CompilationUnit, caretLine, caretColumn);
		}
		
		public IClass SearchType(string name, IClass curType, ICompilationUnit unit, int caretLine, int caretColumn)
		{
//			Console.WriteLine("SearchType({0})", name);
			if (name == null || name.Length == 0) {
				return null;
			}
			// Try if name is already the full type name
			IClass c = GetClass(name);
			if (c != null) {
				return c;
			}
			if (unit != null) {
				// Combine name with usings
				foreach (IUsing u in unit.Usings) {
					if (u != null) {
						c = u.SearchType(name);
						if (c != null) {
							return c;
						}
					}
				}
			}
			if (curType == null) {
				return null;
			}
			// Try parent namespaces of the current class
			string fullname = curType.FullyQualifiedName;
			string[] namespaces = fullname.Split('.');
			StringBuilder curnamespace = new StringBuilder();
			for (int i = 0; i < namespaces.Length; ++i) {
				curnamespace.Append(namespaces[i]);
				curnamespace.Append('.');
				
				curnamespace.Append(name);
				c = GetClass(curnamespace.ToString());
				if (c != null) {
					return c;
				}
				// remove class name again to try next namespace
				curnamespace.Length -= name.Length;
			}
			return null;
		}
		
		/*
		public ArrayList ListMembers(ArrayList members, IClass curType, IClass callingClass, bool showStatic)
		{
//			Console.WriteLine("ListMembers()");
			DateTime now = DateTime.Now;
			
			// enums must be handled specially, because there are several things defined we don't want to show
			// and enum members have neither the modifier nor the modifier public
			if (curType.ClassType == ClassType.Enum) {
				foreach (IField f in curType.Fields) {
					if (f.IsLiteral) {
						members.Add(f);
					}
				}
				ListMembers(members, GetClass("System.Enum"), callingClass, showStatic);
				return members;
			}
			
			bool isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(curType);
			
			if (showStatic) {
				foreach (IClass c in curType.InnerClasses) {
					if (c.IsAccessible(callingClass, isClassInInheritanceTree)) {
						members.Add(c);
					}
				}
			}
			
			foreach (IProperty p in curType.Properties) {
				if (p.MustBeShown(callingClass, showStatic, isClassInInheritanceTree)) {
					members.Add(p);
				}
			}
			
			foreach (IMethod m in curType.Methods) {
				if (m.MustBeShown(callingClass, showStatic, isClassInInheritanceTree)) {
					members.Add(m);
				}
			}
			
			foreach (IEvent e in curType.Events) {
				if (e.MustBeShown(callingClass, showStatic, isClassInInheritanceTree)) {
					members.Add(e);
				}
			}
			
			foreach (IField f in curType.Fields) {
				if (f.MustBeShown(callingClass, showStatic, isClassInInheritanceTree)) {
					members.Add(f);
				}
			}
			
			if (curType.ClassType == ClassType.Interface && !showStatic) {
				foreach (string s in curType.BaseTypes) {
					IClass baseClass = SearchType(s, curType, curType.Region != null ? curType.Region.BeginLine : -1, curType.Region != null ? curType.Region.BeginColumn : -1);
					if (baseClass != null && baseClass.ClassType == ClassType.Interface) {
						ListMembers(members, baseClass, callingClass, showStatic);
					}
				}
			} else {
				IClass baseClass = curType.BaseClass;
				if (baseClass != null) {
					ListMembers(members, baseClass, callingClass, showStatic);
				}
			}
			
			return members;
		}
		 */
		
		public Position GetPosition(string fullMemberName)
		{
			string[] name = fullMemberName.Split(new char[] {'.'});
			string curName = name[0];
			int i = 1;
			while (i < name.Length && NamespaceExists(curName)) {
				curName += '.' + name[i];
				++i;
			}
			Debug.Assert(i <= name.Length);
			IClass curClass = GetClass(curName);
			if (curClass == null) {
				return new Position(null, -1, -1);
			}
			ICompilationUnit cu = curClass.CompilationUnit;
			while (i < name.Length) {
				List<IClass> innerClasses = curClass.InnerClasses;
				foreach (IClass c in innerClasses) {
					if (c.Name == name[i]) {
						curClass = c;
						break;
					}
				}
				if (curClass.Name != name[i]) {
					break;
				}
				++i;
			}
			if (i >= name.Length) {
				return new Position(cu, curClass.Region != null ? curClass.Region.BeginLine : -1, curClass.Region != null ? curClass.Region.BeginColumn : -1);
			}
			return new Position(cu, -1, -1);
			// TODO: reimplement this
			/*IMember member = curClass.SearchMember(name[i]);
			if (member == null || member.Region == null) {
				return new Position(cu, -1, -1);
			}
			return new Position(cu, member.Region.BeginLine, member.Region.BeginColumn);*/
		}
		#endregion
	}
}
