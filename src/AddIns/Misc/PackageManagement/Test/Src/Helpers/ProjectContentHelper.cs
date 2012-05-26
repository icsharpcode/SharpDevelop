// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class ProjectContentHelper
	{
		public IProjectContent FakeProjectContent;
		public List<string> NamespaceNames = new List<string>();
		
		public ProjectContentHelper()
		{
			FakeProjectContent = MockRepository.GenerateStub<IProjectContent>();
			FakeProjectContent.Stub(pc => pc.NamespaceNames).Return(NamespaceNames);
		}
		
		public void SetProjectForProjectContent(IProject project)
		{
			FakeProjectContent.Stub(pc => pc.Project).Return(project);
		}
		
		public IClass AddClassToProjectContentAndCompletionEntries(string namespaceName, string className)
		{
			IClass fakeClass = AddClassToProjectContent(className);
			var namespaceContents = new List<ICompletionEntry>();
			namespaceContents.Add(fakeClass);
			AddCompletionEntriesToNamespace(namespaceName, namespaceContents);
			
			return fakeClass;
		}
		
		public void AddCompletionEntriesToNamespace(string namespaceName, List<ICompletionEntry> namespaceContents)
		{
			FakeProjectContent.Stub(pc => pc.GetNamespaceContents(namespaceName)).Return(namespaceContents);
		}
		
		public void NoCompletionItemsInNamespace(string namespaceName)
		{
			AddCompletionEntriesToNamespace(namespaceName, new List<ICompletionEntry>());
		}
		
		public IClass AddClassToProjectContent(string className)
		{
			IClass fakeClass = AddClassToProjectContentCommon(FakeProjectContent, className);
			fakeClass.Stub(c => c.ClassType).Return(ClassType.Class);
			
			return fakeClass;
		}
		
		public void AddClassToDifferentProjectContent(string className)
		{
			IProjectContent differentProjectContent = MockRepository.GenerateStub<IProjectContent>();
			AddClassToProjectContentCommon(differentProjectContent, className);
		}

		IClass AddClassToProjectContentCommon(IProjectContent projectContent, string className)
		{
			IClass fakeClass = MockRepository.GenerateMock<IClass, IEntity>();
			FakeProjectContent.Stub(pc => pc.GetClass(className, 0)).Return(fakeClass);
			fakeClass.Stub(c => c.FullyQualifiedName).Return(className);
			fakeClass.Stub(c => c.ProjectContent).Return(projectContent);
			return fakeClass;
		}
		
		public IClass AddInterfaceToProjectContent(string interfaceName)
		{
			return AddInterfaceToProjectContent(FakeProjectContent, interfaceName);
		}
		
		public IClass AddInterfaceToDifferentProjectContent(string interfaceName)
		{
			IProjectContent projectContent = MockRepository.GenerateStub<IProjectContent>();
			return AddInterfaceToProjectContent(projectContent, interfaceName);
		}
		
		public IClass AddInterfaceToProjectContent(IProjectContent projectContent, string interfaceName)
		{
			IClass fakeClass = AddClassToProjectContentCommon(projectContent, interfaceName);
			fakeClass.Stub(c => c.ClassType).Return(ClassType.Interface);
			return fakeClass;
		}
		
		public void AddNamespaceNameToProjectContent(string name)
		{
			NamespaceNames.Add(name);
		}
		
		public void NoCompletionEntriesForRootNamespace()
		{
			NoCompletionItemsInNamespace(String.Empty);
		}
		
		public void AddUnknownCompletionEntryTypeToNamespace(string namespaceName)
		{
			var unknownEntry = MockRepository.GenerateStub<ICompletionEntry>();
			var namespaceContents = new List<ICompletionEntry>();
			namespaceContents.Add(unknownEntry);
			AddCompletionEntriesToNamespace(namespaceName, namespaceContents);
		}
		
		public void AddNamespaceCompletionEntryInNamespace(string parentNamespaceName, string namespaceName)
		{
			AddNamespaceCompletionEntriesInNamespace(parentNamespaceName, new string[] { namespaceName });
		}
		
		public void AddNamespaceCompletionEntriesInNamespace(string parentNamespaceName, params string[] childNamespaceNames)
		{
			List<ICompletionEntry> entries = childNamespaceNames
				.Select(name => new NamespaceEntry(name) as ICompletionEntry)
				.ToList();
			AddCompletionEntriesToNamespace(parentNamespaceName, entries);
		}
		
		public void AddClassCompletionEntriesInDifferentProjectContent(string namespaceName, string className)
		{
			IProjectContent differentProjectContent = MockRepository.GenerateStub<IProjectContent>();
			AddClassToCompletionEntries(differentProjectContent, namespaceName, className);
		}
		
		public void AddClassToCompletionEntries(string namespaceName, string className)
		{
			AddClassToCompletionEntries(FakeProjectContent, namespaceName, className);
		}
		
		void AddClassToCompletionEntries(IProjectContent projectContent, string namespaceName, string className)
		{
			IClass fakeClass = MockRepository.GenerateStub<IClass>();
			fakeClass.Stub(c => c.ProjectContent).Return(projectContent);
			var entries = new List<ICompletionEntry>();
			entries.Add(fakeClass);
			AddCompletionEntriesToNamespace(namespaceName, entries);			
		}
		
		public void ProjectContentIsForCSharpProject()
		{
			TestableProject project = ProjectHelper.CreateTestProject();
			project.FileName = @"c:\projects\myproject.csproj";
			SetProjectForProjectContent(project);
		}
		
		public void ProjectContentIsForVisualBasicProject()
		{
			TestableProject project = ProjectHelper.CreateTestProject();
			project.FileName = @"c:\projects\myproject.vbproj";
			SetProjectForProjectContent(project);
		}
	}
}
