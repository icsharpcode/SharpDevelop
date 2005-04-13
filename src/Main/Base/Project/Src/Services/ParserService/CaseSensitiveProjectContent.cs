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
	public class CaseSensitiveProjectContent : IProjectContent
	{
		List<IProjectContent>      referencedContents = new List<IProjectContent>();
		
		Dictionary<string, IClass> classes            = new Dictionary<string, IClass>();
		Hashtable                  namespaces         = new Hashtable();
		XmlDoc                     xmlDoc             = new XmlDoc();
		
		public XmlDoc XmlDoc {
			get {
				return xmlDoc;
			}
		}

		public ICollection<IClass> Classes {
			get {
				return classes.Values;
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
			CaseSensitiveProjectContent newProjectContent = new CaseSensitiveProjectContent();
			
			ICompilationUnit assemblyCompilationUnit = new ICSharpCode.SharpDevelop.Dom.NRefactoryResolver.NRefactoryASTConvertVisitor.CompilationUnit(newProjectContent);
			
			foreach (Type type in assembly.GetTypes()) {
				if (!type.FullName.StartsWith("<") && type.IsPublic) {
					newProjectContent.AddClassToNamespaceList(new ReflectionClass(assemblyCompilationUnit, type, null));
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
		
		public static IProjectContent Create(IProject project)
		{
			CaseSensitiveProjectContent newProjectContent = new CaseSensitiveProjectContent();
			newProjectContent.referencedContents.Add(ProjectContentRegistry.GetMscorlibContent());
			foreach (ProjectItem item in project.Items.ToArray()) {
				switch (item.ItemType) {
					case ItemType.Reference:
					case ItemType.ProjectReference:
						IProjectContent referencedContent = ProjectContentRegistry.GetProjectContentForReference(item as ReferenceProjectItem);
						if (referencedContent != null) {
							newProjectContent.referencedContents.Add(referencedContent);
						}
						break;
					case ItemType.Compile:
						ParseInformation parseInfo = ParserService.ParseFile(item.FileName, null, true, false);
						if (parseInfo != null) {
							newProjectContent.UpdateCompilationUnit(null, parseInfo.BestCompilationUnit as ICompilationUnit, item.FileName, true);
						}
						break;
				}
			}
			
			return newProjectContent;
		}
		
		public Hashtable AddClassToNamespaceList(IClass addClass)
		{
			classes[addClass.FullyQualifiedName] = addClass;
			string nSpace = addClass.Namespace;
			if (nSpace == null) {
				nSpace = String.Empty;
			}
			
			string[] path = nSpace.Split('.');
			
			lock (namespaces) {
				Hashtable cur = namespaces;
				
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
				
				string name = addClass.Name == null ? "" : addClass.Name;
				
				cur[name] = addClass;
				return cur;
			}
		}
			
		public void UpdateCompilationUnit(ICompilationUnit oldUnit, ICompilationUnit parserOutput, string fileName, bool updateCommentTags) 
		{
			
			if (updateCommentTags) {
				TaskService.UpdateCommentTags(fileName, parserOutput.TagComments);
			}
			
			if (oldUnit != null) {
				RemoveClasses(oldUnit);
			}
			
			ICompilationUnit cu = (ICompilationUnit)parserOutput;
			foreach (IClass c in cu.Classes) {
				AddClassToNamespaceList(c);
			}
		}
		
		void RemoveClasses(ICompilationUnit cu)
		{
			if (cu != null) {
				lock (classes) {
					foreach (IClass c in cu.Classes) {
						classes.Remove(c.FullyQualifiedName);
					}
				}
			}
		}
		
		#region Default Parser Layer dependent functions
		public IClass GetClass(string typeName)
		{
//			Console.WriteLine("GetClass({0}) is known:{1}", typeName, classes.ContainsKey(typeName));
			if (classes.ContainsKey(typeName)) {
				return classes[typeName];
			}
			
			// Search in references:
			foreach (IProjectContent content in referencedContents) {
				IClass classFromContent = content.GetClass(typeName);
				if (classFromContent != null) {
					return classFromContent;
				}
			}
			
			// not found -> maybe nested type -> trying to find class that contains this one.
			int lastIndex = typeName.LastIndexOf('.');
			if (lastIndex > 0) {
				string innerName = typeName.Substring(lastIndex + 1);
				string outerName = typeName.Substring(0, lastIndex);
				IClass upperClass = GetClass(outerName);
				if (upperClass != null && upperClass.InnerClasses != null) {
					foreach (IClass c in upperClass.InnerClasses) {
						if (c.Name == innerName) {
							return c;
						}
					}
				}
			}
			return null;
		}
		
		public string[] GetNamespaceList(string subNameSpace)
		{
//			Console.WriteLine("GetNamespaceList({0})", subNameSpace);
			System.Diagnostics.Debug.Assert(subNameSpace != null);
			List<string> namespaceList = new List<string>();
			
			
			foreach (IProjectContent content in referencedContents) {
				string[] referencedNamespaces = content.GetNamespaceList(subNameSpace);
				if (referencedNamespaces != null) {
					namespaceList.AddRange(referencedNamespaces);
				}
			}
			
			string[] path = subNameSpace.Split('.');
			Hashtable cur = namespaces;
			if (subNameSpace.Length > 0) {
				for (int i = 0; i < path.Length; ++i) {
					if (!(cur[path[i]] is Hashtable)) {
						return namespaceList.ToArray();
					}
					cur = (Hashtable)cur[path[i]];
				}
			}
			
			foreach (DictionaryEntry entry in cur) {
				if (entry.Value is Hashtable && entry.Key.ToString().Length > 0) {
					namespaceList.Add(entry.Key.ToString());
				}
			}
			return namespaceList.ToArray();
		}
		
		public ArrayList GetNamespaceContents(string subNameSpace)
		{
			ArrayList namespaceList = new ArrayList();
			if (subNameSpace == null) {
				return namespaceList;
			}
			
			foreach (IProjectContent content in referencedContents) {
				ArrayList referencedNamespaceContents = content.GetNamespaceContents(subNameSpace);
				namespaceList.AddRange(referencedNamespaceContents.ToArray());
			}
			
			string[] path = subNameSpace.Split('.');
			Hashtable cur = namespaces;
			
			for (int i = 0; i < path.Length; ++i) {
				if (!(cur[path[i]] is Hashtable)) {
					foreach (DictionaryEntry entry in cur)  {
						if (entry.Value is Hashtable) {
							namespaceList.Add(entry.Key);
						}
					}
					return namespaceList;
				}
				cur = (Hashtable)cur[path[i]];
			}
			
			foreach (DictionaryEntry entry in cur) {
				if (entry.Value is Hashtable) {
					namespaceList.Add(entry.Key);
				} else {
					namespaceList.Add(entry.Value);
				}
			}
			return namespaceList;
		}
		
		public bool NamespaceExists(string name)
		{
//			Console.WriteLine("NamespaceExists({0}) == ", name);
			if (name == null) {
				return false;
			}
			
			foreach (IProjectContent content in referencedContents) {
				if (content.NamespaceExists(name)) {
					return true;
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
				if (u != null && (u.Region == null || u.Region.IsInside(caretLine, caretColumn))) {
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
			if (name == null || name == String.Empty) {
				return null;
			}
			IClass c  = GetClass(name);
			if (c != null) {
				return c;
			}
			if (unit != null) {
				foreach (IUsing u in unit.Usings) {
					if (u != null && (u.Region == null || u.Region.IsInside(caretLine, caretColumn))) {
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
			string fullname = curType.FullyQualifiedName;
			string[] namespaces = fullname.Split('.');
			StringBuilder curnamespace = new StringBuilder();
			for (int i = 0; i < namespaces.Length; ++i) {
				curnamespace.Append(namespaces[i]);
				curnamespace.Append('.');
				StringBuilder nms=new StringBuilder(curnamespace.ToString());
				nms.Append(name);
				c = GetClass(nms.ToString());
				if (c != null) {
					return c;
				}
			}

			//// Alex: try to find in namespaces referenced excluding system ones which were checked already
			string[] innamespaces = GetNamespaceList("");
			foreach (string ns in innamespaces) {
//				if (Array.IndexOf(ParserService.assemblyList,ns)>=0) continue;
				ArrayList objs=GetNamespaceContents(ns);
				if (objs==null) continue;
				foreach (object o in objs) {
					if (o is IClass) {
						IClass oc=(IClass)o;
						//  || oc.Name==name
						if (oc.FullyQualifiedName == name) {
							//Debug.WriteLine(((IClass)o).Name);
							/// now we can set completion data
							objs.Clear();
							objs = null;
							return oc;
						}
					}
				}
				if (objs == null) {
					break;
				}
			}
			innamespaces=null;
//// Alex: end of mod
			return null;
		}
		
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
			IMember member = curClass.SearchMember(name[i]);
			if (member == null || member.Region == null) {
				return new Position(cu, -1, -1);
			}
			return new Position(cu, member.Region.BeginLine, member.Region.BeginColumn);
		}
		#endregion
		
	}
}
