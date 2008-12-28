// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnitTesting.Tests.Utils
{
	public class MockProjectContent : IProjectContent
	{
		LanguageProperties language;
		List<IClass> classes = new List<IClass>();
		object project;
		
		public MockProjectContent()
		{
		}
		
		public event EventHandler ReferencedContentsChanged;
		
		public XmlDoc XmlDoc {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICollection<IClass> Classes {
			get {
				return classes;
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
			get {
				return language;
			}
			set {
				language = value;
			}
		}
		
		public IUsing DefaultImports {
			get {
				throw new NotImplementedException();
			}
		}
		
		public object Project {
			get {
				return project;
			}
			set {
				project = value;
			}
		}
		
		public SystemTypes SystemTypes {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string GetXmlDocumentation(string memberTag)
		{
			throw new NotImplementedException();
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
		
		public IClass GetClass(string typeName)
		{
			throw new NotImplementedException();
		}
		
		public IClass GetClass(string typeName, int typeParameterCount)
		{
			throw new NotImplementedException();
		}
		
		public bool NamespaceExists(string name)
		{
			throw new NotImplementedException();
		}
		
		public ArrayList GetNamespaceContents(string nameSpace)
		{
			throw new NotImplementedException();
		}
		
		public IClass GetClass(string typeName, int typeParameterCount, LanguageProperties language, GetClassOptions options)
		{
			throw new NotImplementedException();
		}
		
		public bool NamespaceExists(string name, LanguageProperties language, bool lookInReferences)
		{
			throw new NotImplementedException();
		}
		
		public void AddNamespaceContents(System.Collections.ArrayList list, string subNameSpace, LanguageProperties language, bool lookInReferences)
		{
			throw new NotImplementedException();
		}
		
		public string SearchNamespace(string name, IClass curType, ICompilationUnit unit, int caretLine, int caretColumn)
		{
			throw new NotImplementedException();
		}
		
		public SearchTypeResult SearchType(SearchTypeRequest request)
		{
			throw new NotImplementedException();
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
		
		void OnReferencedContentsChanged()
		{
			if (ReferencedContentsChanged != null) {
				ReferencedContentsChanged(this, new EventArgs());
			}
		}
		
		public bool IsUpToDate {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<IAttribute> GetAssemblyAttributes()
		{
			throw new NotImplementedException();
		}
		
		public bool InternalsVisibleTo(IProjectContent otherProjectContent)
		{
			throw new NotImplementedException();
		}
	}
}
