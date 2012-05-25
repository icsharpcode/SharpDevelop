// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
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
		
		public IClass AddClassToProjectContent(string namespaceName, string className)
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
			IClass fakeClass = AddClassToProjectContentCommon(className);
			fakeClass.Stub(c => c.ClassType).Return(ClassType.Class);
			
			return fakeClass;
		}

		IClass AddClassToProjectContentCommon(string className)
		{
			IClass fakeClass = MockRepository.GenerateMock<IClass, IEntity>();
			FakeProjectContent.Stub(pc => pc.GetClass(className, 0)).Return(fakeClass);
			fakeClass.Stub(c => c.FullyQualifiedName).Return(className);
			return fakeClass;
		}
		
		public IClass AddInterfaceToProjectContent(string interfaceName)
		{
			IClass fakeClass = AddClassToProjectContentCommon(interfaceName);
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
	}
}
