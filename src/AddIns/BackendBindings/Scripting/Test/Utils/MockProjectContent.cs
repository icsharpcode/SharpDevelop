// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockProjectContent : IProjectContent
	{
		string namespacePassedToNamespaceExistsMethod;
		List<string> namespacesToAdd = new List<string>();
		SearchTypeResult searchTypeResult;
		bool searchTypeCalled;
		SearchTypeRequest searchTypeRequest;
		IClass classToReturnFromGetClass;
		string getClassName;
		List<IClass> classesInProjectContent = new List<IClass>();
		string namespacePassedToGetNamespaceContentsMethod;
		string classNameForGetClass;
		bool namespaceExistsCalled;
		object project;
		Dictionary<string, List<ICompletionEntry>> namespaceContents = new Dictionary<string, List<ICompletionEntry>>();
		LanguageProperties language = LanguageProperties.CSharp;
		List<IProjectContent> referencedContents = new List<IProjectContent>();
			
		/// <summary>
		/// Gets the namespaces that will be added when the
		/// AddNamespaceContents method is called.
		/// </summary>
		public List<string> NamespacesToAdd {
			get { return namespacesToAdd; }
		}
		
		/// <summary>
		/// Gets whether the NamespaceExists method was called.
		/// </summary>
		public bool NamespaceExistsCalled {
			get { return namespaceExistsCalled; }
		}
		
		public string NamespacePassedToNamespaceExistsMethod {
			get { return namespacePassedToNamespaceExistsMethod; }
		}
		
		/// <summary>
		/// Gets or sets the SearchTypeResult to return from the
		/// SearchType method.
		/// </summary>
		public SearchTypeResult SearchTypeResultToReturn {
			get { return searchTypeResult; }
			set { searchTypeResult = value; }
		}

		/// <summary>
		/// Gets whether the SearchType method was called.
		/// </summary>
		public bool SearchTypeCalled {
			get { return searchTypeCalled; }
		}
		
		/// <summary>
		/// Gets the search type request passed to the SearchType method.
		/// </summary>
		public SearchTypeRequest SearchTypeRequest {
			get { return searchTypeRequest; }
		}
		
		public void SetClassToReturnFromGetClass(string className, IClass classToReturn)
		{
			classToReturnFromGetClass = classToReturn;
			classNameForGetClass = className;
		}

		/// <summary>
		/// Gets the name passed to the GetClass method.
		/// </summary>
		public string GetClassName {
			get { return getClassName; }
		}
		
		public string NamespacePassedToGetNamespaceContentsMethod {
			get { return namespacePassedToGetNamespaceContentsMethod; }
		}
		
		#region IProjectContent
		public event EventHandler ReferencedContentsChanged;
		
		public LanguageProperties Language {
			get { return language; }
		}
		
		public XmlDoc XmlDoc {
			get {
				return null;
			}
		}
		
		public bool IsUpToDate {
			get {
				return true;
			}
		}
		
		public ICollection<IClass> Classes {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICollection<string> NamespaceNames {
			get {
				List<string> names = new List<string>();
				foreach (string existingNamespace in namespaceContents.Keys) {
					names.Add(existingNamespace);
				}
				return names;
			}
		}
		
		public ICollection<IProjectContent> ReferencedContents {
			get { return this.referencedContents; }
		}
		
		public IUsing DefaultImports {
			get { return null; }
		}
		
		public object Project {
			get { return project; }
			set { project = value; }
		}
		
		public SystemTypes SystemTypes {
			get {
				return new SystemTypes(this);
			}
		}
		
		public string GetXmlDocumentation(string memberTag)
		{
			return null;
		}
		
		public void AddClassToNamespaceList(IClass addClass)
		{
			throw new NotImplementedException();
		}
		
		public void RemoveCompilationUnit(ICompilationUnit oldUnit)
		{
			throw new NotImplementedException();
		}
		
		public void UpdateCompilationUnit(ICompilationUnit oldUnit, ICompilationUnit parserOutput, string fileName)
		{
			throw new NotImplementedException();
		}
		
		public IClass GetClass(string typeName, int typeParameterCount)
		{
			getClassName = typeName;
			
			// If a class name is specified then only return a class
			// if we have a match.
			if (classNameForGetClass != null) {
				if (typeName == classNameForGetClass) {
					return classToReturnFromGetClass;
				} else {
					return null;
				}
			}
			return classToReturnFromGetClass;
		}
				
		public void AddExistingNamespaceContents(string namespaceName, List<ICompletionEntry> items)
		{
			namespaceContents.Add(namespaceName, items);
		}
		
		public bool NamespaceExists(string name)
		{
			namespaceExistsCalled = true;
			namespacePassedToNamespaceExistsMethod = name;
			
			return namespaceContents.ContainsKey(name);
		}
		
		public List<ICompletionEntry> GetNamespaceContents(string nameSpace)
		{
			namespacePassedToGetNamespaceContentsMethod = nameSpace;
			
			List<ICompletionEntry> items;
			if (namespaceContents.TryGetValue(nameSpace, out items)) {
				return items;
			}
			return new List<ICompletionEntry>();
		}
		
		public IClass GetClass(string typeName, int typeParameterCount, LanguageProperties language, GetClassOptions options)
		{
			return GetClass(typeName, typeParameterCount);
		}
		
		public bool NamespaceExists(string name, LanguageProperties language, bool lookInReferences)
		{
			throw new NotImplementedException();
		}
		
		public void AddNamespaceContents(List<ICompletionEntry> list, string subNameSpace, LanguageProperties language, bool lookInReferences)
		{
			// Add the namespaces to the list.
			foreach (string ns in namespacesToAdd) {
				list.Add(new NamespaceEntry(ns));
			}
			
			// Add the classes in this project content.
			foreach (IClass c in classesInProjectContent) {
				list.Add(c);
			}
		}
		
		/// <summary>
		/// Gets the list of classes that will be added to the list
		/// when the AddNamespaceContents is called.
		/// </summary>
		public List<IClass> ClassesInProjectContent {
			get { return classesInProjectContent; }
		}
		
		public string SearchNamespace(string name, IClass curType, ICompilationUnit unit, int caretLine, int caretColumn)
		{
			throw new NotImplementedException();
		}
		
		public SearchTypeResult SearchType(SearchTypeRequest request)
		{
			searchTypeCalled = true;
			searchTypeRequest = request;
			return searchTypeResult;
		}
		
		public IClass GetClassByReflectionName(string fullMemberName, bool lookInReferences)
		{
			throw new NotImplementedException();
		}
		
		public FilePosition GetPosition(IEntity entity)
		{
			throw new NotImplementedException();
		}
		
		public void Dispose()
		{
			throw new NotImplementedException();
		}
		
		public IList<IAttribute> GetAssemblyAttributes()
		{
			throw new NotImplementedException();
		}
		
		public bool InternalsVisibleTo(IProjectContent otherProjectContent)
		{
			throw new NotImplementedException();
		}
		
		public string AssemblyName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public List<ICompletionEntry> GetAllContents()
		{
			throw new NotImplementedException();
		}
		
		public void AddAllContents(List<ICompletionEntry> list, LanguageProperties language, bool lookInReferences)
		{
			throw new NotImplementedException();
		}
		#endregion

		protected virtual void OnReferencedContentsChanged(EventArgs e)
		{
			if (ReferencedContentsChanged != null) {
				ReferencedContentsChanged(this, e);
			}
		}
	}
}
