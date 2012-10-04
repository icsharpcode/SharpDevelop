// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSharpBinding.Refactoring;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;
using Rhino.Mocks;

namespace CSharpBinding.Tests
{
	/// <summary>
	/// Tests that issue providers and context actions from NR are registered in the AddIn-Tree.
	/// </summary>
	[TestFixture]
	public class RegistrationTests
	{
		Type[] exceptions = {
			typeof(MultipleEnumerationIssue), // disabled due to https://github.com/icsharpcode/NRefactory/issues/123
			typeof(RedundantAssignmentIssue), // disabled due to https://github.com/icsharpcode/NRefactory/issues/123
		};
		
		Assembly NRCSharp = typeof(ICodeIssueProvider).Assembly;
		Assembly CSharpBinding = typeof(SDRefactoringContext).Assembly;
		
		AddIn addIn;
		List<Type> registeredIssueProviders;
		List<Type> NRissueProviders;
		List<Type> registeredContextActions;
		List<Type> NRcontextActions;
		
		Type FindType(string @class)
		{
			return CSharpBinding.GetType(@class, false) ?? NRCSharp.GetType(@class, true);
		}
		
		[TestFixtureSetUpAttribute]
		public void FixtureSetUp()
		{
			var addInTree = MockRepository.GenerateStub<IAddInTree>();
			addIn = AddIn.Load(addInTree, "CSharpBinding.addin");
			
			registeredIssueProviders = addIn.Paths["/SharpDevelop/ViewContent/TextEditor/C#/IssueProviders"].Codons
				.Select(c => FindType(c.Properties["class"])).ToList();
			NRissueProviders = NRCSharp.ExportedTypes.Where(t => !t.IsAbstract && t.GetInterface(typeof(ICodeIssueProvider).FullName) != null).ToList();
			
			registeredContextActions = addIn.Paths["/SharpDevelop/ViewContent/TextEditor/C#/ContextActions"].Codons
				.Select(c => FindType(c.Properties["class"])).ToList();
			NRcontextActions = NRCSharp.ExportedTypes.Where(t => !t.IsAbstract && t.GetInterface(typeof(ICodeActionProvider).FullName) != null).ToList();
		}
		
		[Test]
		public void NoDuplicateRegistrations()
		{
			Assert.AreEqual(registeredIssueProviders, registeredIssueProviders.Distinct());
			Assert.AreEqual(registeredContextActions, registeredContextActions.Distinct());
		}
		
		[Test]
		public void AllNRefactoryIssueProvidersAreRegistered()
		{
			var registeredNRissueProviders = registeredIssueProviders.Select(p => p.Assembly == CSharpBinding ? p.BaseType : p);
			Assert.IsEmpty(NRissueProviders.Except(registeredNRissueProviders).Except(exceptions),
			               "There are providers in NR that are not registered in SD");
		}
		
		[Test]
		public void AllNRefactoryCodeActionProvidersAreRegistered()
		{
			var registeredNRcodeActionProviders = registeredContextActions.Select(p => p.Assembly == CSharpBinding ? p.BaseType : p);
			Assert.IsEmpty(NRcontextActions.Except(registeredNRcodeActionProviders).Except(exceptions),
			               "There are context actions in NR that are not registered in SD");
		}
		
		[Test]
		public void ExceptionsAreNotRegistered()
		{
			Assert.IsEmpty(exceptions.Intersect(registeredIssueProviders.Union(registeredContextActions)));
		}
	}
}
