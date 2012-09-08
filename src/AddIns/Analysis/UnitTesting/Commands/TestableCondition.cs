// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Editor.Commands;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Supplies a "Unit test" menu item if the class is a test fixture.
	/// </summary>
	public class TestableCondition : IConditionEvaluator
	{
		IRegisteredTestFrameworks testFrameworks;
		
		public TestableCondition(IRegisteredTestFrameworks testFrameworks)
		{
			this.testFrameworks = testFrameworks;
		}
		
		public TestableCondition()
			: this(TestService.RegisteredTestFrameworks)
		{
		}
		
		public static IMember GetMember(object caller)
		{
			ITestTreeView testTreeView = caller as ITestTreeView;
			if (testTreeView != null && testTreeView.SelectedMember != null) {
				return testTreeView.SelectedMember.Resolve();
			}
			IEntity entity = ResolveResultMenuCommand.GetEntity(caller);
			return entity as IMember;
		}
		
		public static ITypeDefinition GetClass(object caller)
		{
			ITestTreeView testTreeView = caller as ITestTreeView;
			if (testTreeView != null && testTreeView.SelectedClass != null) {
				return testTreeView.SelectedClass.Resolve();
			}
			IEntity entity = ResolveResultMenuCommand.GetEntity(caller);
			return entity as ITypeDefinition;
		}
		
		public static IProject GetProject(object caller)
		{
			ITestTreeView testTreeView = caller as ITestTreeView;
			if (testTreeView != null) {
				return testTreeView.SelectedProject;
			}
			ITypeDefinition c = GetClassFromMemberOrCaller(caller);
			return GetProject(c);
		}
		
		public static ITypeDefinition GetClassFromMemberOrCaller(object caller)
		{
			IMember m = GetMember(caller);
			if (m != null) {
				return m.DeclaringTypeDefinition;
			}
			return GetClass(caller);
		}
		
		static IProject GetProject(ITypeDefinition c)
		{
			return c != null ? c.ParentAssembly.GetProject() : null;
		}
		
		/// <summary>
		/// Returns the namespace selected if any.
		/// </summary>
		public static string GetNamespace(object caller)
		{
			ITestTreeView testTreeView = caller as ITestTreeView;
			if (testTreeView != null) {
				return testTreeView.SelectedNamespace;
			}
			return null;
		}
		
		public bool IsValid(object caller, Condition condition)
		{
			IMember m = GetMember(caller);
			if (m != null) {
				return testFrameworks.IsTestMember(m);
			}
			ITypeDefinition c = GetClass(caller);
			if (ClassHasProject(c)) {
				return testFrameworks.IsTestClass(c);
			}
			return false;
		}
		
		static bool ClassHasProject(ITypeDefinition c)
		{
			return (c != null) && (c.ParentAssembly.GetProject() != null);
		}
	}
}
