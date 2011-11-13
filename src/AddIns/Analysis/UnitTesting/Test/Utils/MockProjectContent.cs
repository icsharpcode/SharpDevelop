// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Project;

namespace UnitTesting.Tests.Utils
{
	public class MockProjectContent : IProjectContent
	{
		LanguageProperties language = LanguageProperties.CSharp;
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
			get { return classes; }
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
			set { language = value; }
		}
		
		public IUsing DefaultImports {
			get {
				throw new NotImplementedException();
			}
		}
		
		public object Project {
			get { return project; }
			set { project = value; }
		}
		
		public IProject ProjectAsIProject {
			get { return project as IProject; }
		}
		
		public SystemTypes SystemTypes {
			get { return new SystemTypes(this); }
		}
		
		public string GetXmlDocumentation(string memberTag)
		{
			return string.Empty;
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
			MockClass c = new MockClass(this);
			c.FullyQualifiedName = typeName;
			return c;
		}
		
		public bool NamespaceExists(string name)
		{
			throw new NotImplementedException();
		}
		
		public List<ICompletionEntry> GetNamespaceContents(string nameSpace)
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
		
		public void AddNamespaceContents(List<ICompletionEntry> list, string subNameSpace, LanguageProperties language, bool lookInReferences)
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
		
		public string AssemblyName {
			get { return String.Empty; }
		}
		
		public void AddAllContents(List<ICompletionEntry> list, LanguageProperties language, bool lookInReferences)
		{
			throw new NotImplementedException();
		}
		
		public List<ICompletionEntry> GetAllContents()
		{
			throw new NotImplementedException();
		}
	}
}
