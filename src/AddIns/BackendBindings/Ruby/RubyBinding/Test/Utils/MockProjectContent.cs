// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace RubyBinding.Tests.Utils
{
	/// <summary>
	/// Mock IProjectContent implementation.
	/// </summary>
	public class MockProjectContent : IProjectContent
	{
		bool addNamespaceContentsCalled;
		string namespaceAddedName;
		string namespaceSearchedFor;
		bool lookInReferences;
		LanguageProperties languagePassedToAddNamespaceContents;
		LanguageProperties language = new LanguageProperties(StringComparer.InvariantCulture);
		List<string> namespacesToAdd = new List<string>();
		SearchTypeResult searchTypeResult;
		bool searchTypeCalled;
		SearchTypeRequest searchTypeRequest;
		IClass classToReturnFromGetClass;
		bool getClassCalled;
		string getClassName;
		List<IClass> classesInProjectContent = new List<IClass>();
		ArrayList namespaceContents = new ArrayList();
		string namespaceContentsSearched = String.Empty;
		string classNameForGetClass;
		bool namespaceExistsReturnValue;
		bool namespaceExistsCalled;
		object project;
		
		public MockProjectContent()
		{
		}

		/// <summary>
		/// Gets whether the AddNamespaceContents method was called.
		/// </summary>
		public bool AddNamespaceContentsCalled {
			get { return addNamespaceContentsCalled; }
		}
		
		/// <summary>
		/// Gets the namespace passed to the AddNamespaceContents method.
		/// </summary>
		public string NamespaceAddedName {
			get { return namespaceAddedName; }
		}
		
		/// <summary>
		/// Gets the LookInReferences flag that was passed to the 
		/// AddNamespaceContents method.
		/// </summary>
		public bool LookInReferences {
			get { return lookInReferences; }
		}
		
		public LanguageProperties LanguagePassedToAddNamespaceContents {
			get { return languagePassedToAddNamespaceContents; }
		}
		
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
		
		/// <summary>
		/// Gets the namespace passed to the SearchNamespace method and 
		/// searched for.
		/// </summary>
		public string NamespaceSearchedFor {
			get { return namespaceSearchedFor; }
			set { namespaceSearchedFor = value; }
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
		
		/// <summary>
		/// Gets or sets the class to return from the GetClass Method.
		/// </summary>
		public IClass ClassToReturnFromGetClass
		{
			get { return classToReturnFromGetClass; }
			set { classToReturnFromGetClass = value; }
		}
		
		/// <summary>
		/// Gets or sets the class name that needs to match in order
		/// for the GetClass call to return a class. If nothing is
		/// specified then every class name matches. 
		/// </summary>
		public string ClassNameForGetClass {
			get { return classNameForGetClass; }
			set { classNameForGetClass = value; }
		}

		/// <summary>
		/// Gets whether the GetClass method was called.
		/// </summary>
		public bool GetClassCalled {
			get { return getClassCalled; }
		}

		/// <summary>
		/// Gets the name passed to the GetClass method.
		/// </summary>
		public string GetClassName {
			get { return getClassName; }
		}
		
		/// <summary>
		/// Gets the namespace contents to return from the
		/// GetNamespaceContents method.
		/// </summary>
		public ArrayList NamespaceContentsToReturn {
			get { return namespaceContents; }
		}
		
		/// <summary>
		/// Returns the namespace that was passed to the
		/// GetNamespaceContents method.
		/// </summary>
		public string NamespaceContentsSearched {
			get { return namespaceContentsSearched; }
		}
		
		#region IProjectContent
		public event EventHandler ReferencedContentsChanged;
				
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
				throw new NotImplementedException();
			}
		}
		
		public ICollection<IProjectContent> ReferencedContents {
			get {
				throw new NotImplementedException();
			}
		}
		
		public LanguageProperties Language {
			get { return language; }
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
			getClassCalled = true;
			
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
				
		public void SetNamespaceExistsReturnValue(bool returnValue)
		{
			namespaceExistsReturnValue = returnValue;
		}
		
		public bool NamespaceExists(string name)
		{
			namespaceExistsCalled = true;
			namespaceSearchedFor = name;
			return namespaceExistsReturnValue;
		}
		
		public ArrayList GetNamespaceContents(string nameSpace)
		{
			namespaceContentsSearched = nameSpace;
			return namespaceContents;
		}
		
		public IClass GetClass(string typeName, int typeParameterCount, LanguageProperties language, GetClassOptions options)
		{
			return GetClass(typeName, typeParameterCount);
		}
		
		public bool NamespaceExists(string name, LanguageProperties language, bool lookInReferences)
		{
			throw new NotImplementedException();
		}
		
		public void AddNamespaceContents(ArrayList list, string subNameSpace, LanguageProperties language, bool lookInReferences)
		{
			addNamespaceContentsCalled = true;
			namespaceAddedName = subNameSpace;
			this.lookInReferences = lookInReferences;
			languagePassedToAddNamespaceContents = language;
			
			// Add the namespaces to the list.
			foreach (string ns in namespacesToAdd) {
				list.Add(ns);
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
//			searchNamespaceCalled = true;
//			namespaceSearchedFor = name;
//			return searchNamespace;
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
		#endregion

		protected virtual void OnReferencedContentsChanged(EventArgs e)
		{
			if (ReferencedContentsChanged != null) {
				ReferencedContentsChanged(this, e);
			}
		}
		
		public bool InternalsVisibleTo(IProjectContent otherProjectContent)
		{
			throw new NotImplementedException();
		}
	}
}
