// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

//using System;
//using System.Collections.Generic;
//using System.Linq;
//
//using ICSharpCode.SharpDevelop.Dom;
//using ICSharpCode.SharpDevelop.Project;
//using Rhino.Mocks;
//
//namespace PackageManagement.Tests.Helpers
//{
//	public class ProjectContentHelper
//	{
//		public IProjectContent ProjectContent;
//		public List<string> NamespaceNames = new List<string>();
//		
//		public ProjectContentHelper()
//		{
//			ProjectContent = MockRepository.GenerateStub<IProjectContent>();
//			ProjectContent.Stub(pc => pc.NamespaceNames).Return(NamespaceNames);
//		}
//		
//		public void SetProjectForProjectContent(object project)
//		{
//			ProjectContent.Stub(pc => pc.Project).Return(project);
//		}
//		
//		public IClass AddClassToProjectContentAndCompletionEntries(string namespaceName, string className)
//		{
//			IClass fakeClass = AddClassToProjectContent(className);
//			var namespaceContents = new List<ICompletionEntry>();
//			namespaceContents.Add(fakeClass);
//			AddCompletionEntriesToNamespace(namespaceName, namespaceContents);
//			
//			return fakeClass;
//		}
//		
//		public void AddCompletionEntriesToNamespace(string namespaceName, List<ICompletionEntry> namespaceContents)
//		{
//			ProjectContent.Stub(pc => pc.GetNamespaceContents(namespaceName)).Return(namespaceContents);
//		}
//		
//		public void NoCompletionItemsInNamespace(string namespaceName)
//		{
//			AddCompletionEntriesToNamespace(namespaceName, new List<ICompletionEntry>());
//		}
//		
//		public IClass AddClassToProjectContent(string className)
//		{
//			IClass fakeClass = AddClassToProjectContentCommon(ProjectContent, className);
//			SetClassType(fakeClass, ClassType.Class);
//			return fakeClass;
//		}
//		
//		public void SetClassType(IClass fakeClass, ClassType classType)
//		{
//			fakeClass.Stub(c => c.ClassType).Return(classType);
//		}
//		
//		public void AddClassToDifferentProjectContent(string className)
//		{
//			IProjectContent differentProjectContent = MockRepository.GenerateStub<IProjectContent>();
//			AddClassToProjectContentCommon(differentProjectContent, className);
//		}
//
//		IClass AddClassToProjectContentCommon(IProjectContent projectContent, string className)
//		{
//			IClass fakeClass = MockRepository.GenerateMock<IClass, IEntity>();
//			ProjectContent.Stub(pc => pc.GetClass(className, 0)).Return(fakeClass);
//			fakeClass.Stub(c => c.FullyQualifiedName).Return(className);
//			fakeClass.Stub(c => c.ProjectContent).Return(projectContent);
//			return fakeClass;
//		}
//		
//		public IClass AddInterfaceToProjectContent(string interfaceName)
//		{
//			return AddInterfaceToProjectContent(ProjectContent, interfaceName);
//		}
//		
//		public IClass AddInterfaceToDifferentProjectContent(string interfaceName)
//		{
//			IProjectContent projectContent = MockRepository.GenerateStub<IProjectContent>();
//			return AddInterfaceToProjectContent(projectContent, interfaceName);
//		}
//		
//		public IClass AddInterfaceToProjectContent(IProjectContent projectContent, string interfaceName)
//		{
//			IClass fakeClass = AddClassToProjectContentCommon(projectContent, interfaceName);
//			SetClassType(fakeClass, ClassType.Interface);
//			return fakeClass;
//		}
//		
//		public IClass AddInterfaceToProjectContentAndCompletionEntries(string namespaceName, string className)
//		{
//			IClass fakeClass = AddInterfaceToProjectContent(className);
//			var namespaceContents = new List<ICompletionEntry>();
//			namespaceContents.Add(fakeClass);
//			AddCompletionEntriesToNamespace(namespaceName, namespaceContents);
//			
//			return fakeClass;
//		}
//		
//		public void AddNamespaceNameToProjectContent(string name)
//		{
//			NamespaceNames.Add(name);
//		}
//		
//		public void NoCompletionEntriesForRootNamespace()
//		{
//			NoCompletionItemsInNamespace(String.Empty);
//		}
//		
//		public void AddUnknownCompletionEntryTypeToNamespace(string namespaceName)
//		{
//			var unknownEntry = MockRepository.GenerateStub<ICompletionEntry>();
//			var namespaceContents = new List<ICompletionEntry>();
//			namespaceContents.Add(unknownEntry);
//			AddCompletionEntriesToNamespace(namespaceName, namespaceContents);
//		}
//		
//		public void AddNamespaceCompletionEntryInNamespace(string parentNamespaceName, string namespaceName)
//		{
//			AddNamespaceCompletionEntriesInNamespace(parentNamespaceName, new string[] { namespaceName });
//		}
//		
//		public void AddNamespaceCompletionEntriesInNamespace(string parentNamespaceName, params string[] childNamespaceNames)
//		{
//			List<ICompletionEntry> entries = childNamespaceNames
//				.Select(name => new NamespaceEntry(name) as ICompletionEntry)
//				.ToList();
//			AddCompletionEntriesToNamespace(parentNamespaceName, entries);
//		}
//		
//		public void AddClassCompletionEntriesInDifferentProjectContent(string namespaceName, string className)
//		{
//			IProjectContent differentProjectContent = MockRepository.GenerateStub<IProjectContent>();
//			AddClassToCompletionEntries(differentProjectContent, namespaceName, className);
//		}
//		
//		public void AddClassToCompletionEntries(string namespaceName, string className)
//		{
//			AddClassToCompletionEntries(ProjectContent, namespaceName, className);
//		}
//		
//		void AddClassToCompletionEntries(IProjectContent projectContent, string namespaceName, string className)
//		{
//			IClass fakeClass = MockRepository.GenerateStub<IClass>();
//			fakeClass.Stub(c => c.ProjectContent).Return(projectContent);
//			var entries = new List<ICompletionEntry>();
//			entries.Add(fakeClass);
//			AddCompletionEntriesToNamespace(namespaceName, entries);			
//		}
//		
//		public void ProjectContentIsForCSharpProject()
//		{
//			TestableProject project = ProjectHelper.CreateTestProject();
//			project.FileName = @"c:\projects\myproject.csproj";
//			ProjectContent.Stub(pc => pc.Language).Return(LanguageProperties.CSharp);
//			SetProjectForProjectContent(project);
//		}
//		
//		public void ProjectContentIsForVisualBasicProject()
//		{
//			TestableProject project = ProjectHelper.CreateTestProject();
//			project.FileName = @"c:\projects\myproject.vbproj";
//			ProjectContent.Stub(pc => pc.Language).Return(LanguageProperties.VBNet);
//			SetProjectForProjectContent(project);
//		}
//		
//		public IClass AddPublicClassToProjectContent(string name)
//		{
//			IClass fakeClass = AddClassToProjectContent(name);
//			MakeClassPublic(fakeClass);
//			return fakeClass;
//		}
//		
//		public IClass AddPrivateClassToProjectContent(string name)
//		{
//			IClass fakeClass = AddClassToProjectContent(name);
//			MakeClassPrivate(fakeClass);
//			return fakeClass;
//		}
//		
//		public IClass AddPublicStructToProjectContent(string name)
//		{
//			IClass fakeStruct = AddStructToProjectContent(name);
//			MakeClassPublic(fakeStruct);
//			return fakeStruct;
//		}
//		
//		public IClass AddStructToProjectContent(string name)
//		{
//			IClass fakeStruct = AddClassToProjectContentCommon(ProjectContent, name);
//			SetClassType(fakeStruct, ClassType.Struct);
//			return fakeStruct;
//		}
//		
//		public IClass AddPrivateStructToProjectContent(string name)
//		{
//			IClass fakeStruct = AddStructToProjectContent(name);
//			MakeClassPrivate(fakeStruct);
//			return fakeStruct;
//		}
//		
//		public IClass AddPublicDelegateToProjectContent(string name)
//		{
//			IClass fakeDelegate = AddDelegateToProjectContent(name);
//			MakeClassPublic(fakeDelegate);
//			return fakeDelegate;
//		}
//		
//		public IClass AddDelegateToProjectContent(string name)
//		{
//			IClass fakeDelegate = AddClassToProjectContentCommon(ProjectContent, name);
//			SetClassType(fakeDelegate, ClassType.Delegate);
//			return fakeDelegate;
//		}
//		
//		public void MakeClassPublic(IClass fakeClass)
//		{
//			fakeClass.Stub(c => c.IsPublic).Return(true);
//		}
//		
//		public void MakeClassPrivate(IClass fakeClass)
//		{
//			fakeClass.Stub(c => c.IsPublic).Return(false);
//			fakeClass.Stub(c => c.IsPrivate).Return(true);
//		}
//		
//		public IClass AddPrivateDelegateToProjectContent(string name)
//		{
//			IClass fakeDelegate = AddDelegateToProjectContent(name);
//			MakeClassPrivate(fakeDelegate);
//			return fakeDelegate;
//		}
//	}
//}
