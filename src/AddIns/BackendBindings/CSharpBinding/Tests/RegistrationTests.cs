// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSharpBinding.Refactoring;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
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
			typeof(PossibleMultipleEnumerationIssue), // disabled due to https://github.com/icsharpcode/NRefactory/issues/123
			typeof(RedundantAssignmentIssue), // disabled due to https://github.com/icsharpcode/NRefactory/issues/123
			typeof(RedundantCastIssue), // disabled due to plenty of false positives (e.g. when cast is necessary for overload resolution)
		};
		
		Assembly NRCSharp = typeof(CodeIssueProvider).Assembly;
		Assembly NRCSharpRefactoring = typeof(AddBracesAction).Assembly;
		Assembly CSharpBinding = typeof(SDRefactoringContext).Assembly;
		
		AddIn addIn;
		List<Type> registeredIssueProviders;
		List<Type> NRissueProviders;
		List<Type> registeredContextActions;
		List<Type> NRcontextActions;
		
		Type FindType(string @class)
		{
			return CSharpBinding.GetType(@class, false) ?? NRCSharpRefactoring.GetType(@class, false);
		}
		
		[TestFixtureSetUpAttribute]
		public void FixtureSetUp()
		{
			var addInTree = MockRepository.GenerateStub<IAddInTree>();
			addIn = AddIn.Load(addInTree, "CSharpBinding.addin");
			
			registeredIssueProviders = addIn.Paths["/SharpDevelop/ViewContent/TextEditor/C#/IssueProviders"].Codons
				.Select(c => FindType(c.Properties["class"])).Where(t => t != null).ToList();
			NRissueProviders = NRCSharpRefactoring.ExportedTypes.Where(t => t.GetCustomAttribute<IssueDescriptionAttribute>() != null).ToList();
			
			registeredContextActions = addIn.Paths["/SharpDevelop/ViewContent/TextEditor/C#/ContextActions"].Codons
				.Select(c => FindType(c.Properties["class"])).Where(t => t != null).ToList();
			NRcontextActions = NRCSharpRefactoring.ExportedTypes.Where(t => t.GetCustomAttribute<ContextActionAttribute>() != null).ToList();
		}
		
		
		[Test]
		public void NoMissingClasses()
		{
			var classNames = addIn.Paths["/SharpDevelop/ViewContent/TextEditor/C#/IssueProviders"].Codons
				.Concat(addIn.Paths["/SharpDevelop/ViewContent/TextEditor/C#/ContextActions"].Codons)
				.Select(c => c.Properties["class"]);
			Assert.IsEmpty(classNames.Where(c => FindType(c) == null));
		}
		
		[Test]
		public void NoDuplicateRegistrations()
		{
			Assert.AreEqual(registeredIssueProviders, registeredIssueProviders.Distinct());
			Assert.AreEqual(registeredContextActions, registeredContextActions.Distinct());
		}
		
		[Test]
		public void AllAreRegisteredIssueProvidersHaveAttribute()
		{
			foreach (var t in registeredIssueProviders) {
				Assert.IsNotNull(t.GetCustomAttribute<IssueDescriptionAttribute>(), t.FullName);
			}
		}
		
		[Test]
		public void AllAreRegisteredContextActionsHaveAttribute()
		{
			Assert.IsTrue(registeredContextActions.All(t => t.GetCustomAttribute<ContextActionAttribute>() != null));
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
