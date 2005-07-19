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
		List<IProjectContent> referencedContents = new List<IProjectContent>();
		
		List<Dictionary<string, IClass>> classLists = new List<Dictionary<string, IClass>>();
		List<Dictionary<string, NamespaceStruct>> namespaces = new List<Dictionary<string, NamespaceStruct>>();
		protected XmlDoc xmlDoc = new XmlDoc();
		
		public List<Dictionary<string, IClass>> ClassLists {
			get {
				if (classLists.Count == 0) {
					classLists.Add(new Dictionary<string, IClass>(language.NameComparer));
				}
				return classLists;
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
					d.Add(pair.Key, pair.Value);
				}
			} else {
				d = new Dictionary<string, IClass>(language.NameComparer);
			}
			classLists.Add(d);
			return d;
		}
		
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
		}
		
		public ICollection<IClass> Classes {
			get {
				return ClassLists[0].Values;
			}
		}
		
		public List<IProjectContent> ReferencedContents {
			get {
				return referencedContents;
			}
		}
		
		public bool HasReferenceTo(IProjectContent content)
		{
			return referencedContents.Contains(content);
		}
		
		LanguageProperties language = LanguageProperties.CSharp;
		
		/// <summary>
		/// Gets/Sets the properties of the language this project content was written in.
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
			string desc = xmlDoc.GetDocumentation(memberTag);
			if (desc != null) {
				return desc;
			}
			foreach (IProjectContent referencedContent in referencedContents) {
				desc = referencedContent.XmlDoc.GetDocumentation(memberTag);
				if (desc != null) {
					return desc;
				}
			}
			return null;
		}
		
		protected static string LookupLocalizedXmlDoc(string fileName)
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
		
		public virtual void Dispose()
		{
			xmlDoc.Dispose();
		}
		
		public void AddClassToNamespaceList(IClass addClass)
		{
			lock (namespaces) {
				AddClassToNamespaceListInternal(addClass);
			}
		}
		
		protected void AddClassToNamespaceListInternal(IClass addClass)
		{
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
		
		public void UpdateCompilationUnit(ICompilationUnit oldUnit, ICompilationUnit parserOutput, string fileName, bool updateCommentTags)
		{
			if (updateCommentTags) {
				TaskService.UpdateCommentTags(fileName, parserOutput.TagComments);
			}
			
			lock (namespaces) {
				ICompilationUnit cu = (ICompilationUnit)parserOutput;
				
				if (oldUnit != null) {
					RemoveClasses(oldUnit, cu);
				}
				
				foreach (IClass c in cu.Classes) {
					AddClassToNamespaceListInternal(c);
				}
			}
		}
		
		void RemoveClasses(ICompilationUnit oldUnit, ICompilationUnit newUnit)
		{
			foreach (IClass c in oldUnit.Classes) {
				bool found = false;
				foreach (IClass c2 in newUnit.Classes) {
					if (c.FullyQualifiedName == c2.FullyQualifiedName) {
						found = true;
						break;
					}
				}
				if (!found) {
					RemoveClass(c);
				}
			}
		}
		
		void RemoveClass(IClass @class)
		{
			string nSpace = @class.Namespace;
			if (nSpace == null) {
				nSpace = String.Empty;
			}
			RemoveClass(@class.FullyQualifiedName, nSpace);
		}
		
		void RemoveClass(string fullyQualifiedName, string nSpace)
		{
			foreach (Dictionary<string, IClass> classes in ClassLists) {
				classes.Remove(fullyQualifiedName);
			}
			
			
			
			// Remove class from namespace lists
			List<IClass> classList = GetNamespaces(this.language)[nSpace].Classes;
			for (int i = 0; i < classList.Count; i++) {
				if (classList[i].FullyQualifiedName == fullyQualifiedName) {
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
			return GetClass(typeName, language, true);
		}
		
		public IClass GetClass(string typeName, LanguageProperties language, bool lookInReferences)
		{
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
					List<IClass> innerClasses = upperClass.InnerClasses;
					if (innerClasses != null) {
						string innerName = typeName.Substring(lastIndex + 1);
						foreach (IClass c in innerClasses) {
							if (language.NameComparer.Equals(c.Name, innerName)) {
								return c;
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
				foreach (IProjectContent content in referencedContents) {
					content.AddNamespaceContents(list, nameSpace, language, false);
				}
			}
			
			Dictionary<string, NamespaceStruct> dict = GetNamespaces(language);
			if (dict.ContainsKey(nameSpace)) {
				NamespaceStruct ns = dict[nameSpace];
				list.AddRange(ns.Classes);
				list.AddRange(ns.SubNamespaces);
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
			
			return GetNamespaces(language).ContainsKey(name);
		}
		
		
		public string SearchNamespace(string name, ICompilationUnit unit, int caretLine, int caretColumn)
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
			if (name == null || name.Length == 0) {
				return null;
			}
			// Try if name is already the full type name
			IClass c = GetClass(name);
			if (c != null) {
				return c;
			}
			if (curType != null) {
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
				
				if (name.IndexOf('.') < 0) {
					// Try inner classes of parent classes
					while ((curType = curType.BaseClass) != null) {
						foreach (IClass innerClass in curType.InnerClasses) {
							if (language.NameComparer.Equals(innerClass.Name, name))
								return innerClass;
						}
					}
				}
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
			return null;
		}
		
		/// <summary>
		/// Gets the position of a member in this project content (not a referenced one).
		/// </summary>
		/// <param name="fullMemberName">Fully qualified member name (always case sensitive).</param>
		public Position GetPosition(string fullMemberName)
		{
			IClass curClass = GetClass(fullMemberName, LanguageProperties.CSharp, false);
			if (curClass != null) {
				return new Position(curClass.CompilationUnit, curClass.Region != null ? curClass.Region.BeginLine : -1, curClass.Region != null ? curClass.Region.BeginColumn : -1);
			}
			int pos = fullMemberName.LastIndexOf('.');
			if (pos > 0) {
				string className = fullMemberName.Substring(0, pos);
				string memberName = fullMemberName.Substring(pos + 1);
				curClass = GetClass(className, LanguageProperties.CSharp, false);
				if (curClass != null) {
					IMember member = curClass.SearchMember(memberName, LanguageProperties.CSharp);
					if (member != null) {
						return new Position(curClass.CompilationUnit, member.Region != null ? member.Region.BeginLine : -1, member.Region != null ? member.Region.BeginColumn : -1);
					}
				}
			}
			return new Position(null, -1, -1);
		}
		#endregion
	}
}
