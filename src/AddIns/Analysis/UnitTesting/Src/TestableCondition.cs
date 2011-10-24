// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
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
			if (testTreeView != null) {
				return testTreeView.SelectedMember;
			}
			MemberNode memberNode = caller as MemberNode;
			if (memberNode != null) {
				return memberNode.Member;
			} else {
				ClassMemberBookmark mbookmark = caller as ClassMemberBookmark;
				if (mbookmark != null) {
					return mbookmark.Member;
				}
			}
			return null;
		}
		
		public static IClass GetClass(object caller)
		{
			ITestTreeView testTreeView = caller as ITestTreeView;
			if (testTreeView != null) {
				return testTreeView.SelectedClass;
			}
			ClassNode classNode = caller as ClassNode;
			if (classNode != null) {
				return classNode.Class;
			} else {
				ClassBookmark bookmark = caller as ClassBookmark;
				if (bookmark != null) {
					return bookmark.Class;
				}
			}
			return null;
		}
		
		public static IProject GetProject(object caller)
		{
			ITestTreeView testTreeView = caller as ITestTreeView;
			if (testTreeView != null) {
				return testTreeView.SelectedProject;
			}
			IClass c = GetClassFromMemberOrCaller(caller);
			return GetProject(c);
		}
		
		static IClass GetClassFromMemberOrCaller(object caller)
		{
			IMember m = GetMember(caller);
			if (m != null) {
				return m.DeclaringType;
			}
			return GetClass(caller);
		}
		
		static IProject GetProject(IClass c)
		{
			if (c != null) {
				return (IProject)c.ProjectContent.Project;
			}
			return null;
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
			IClass c = GetClass(caller);
			if (ClassHasProject(c)) {
				return testFrameworks.IsTestClass(c);
			}
			return false;
		}
		
		static bool ClassHasProject(IClass c)
		{
			return (c != null) && (c.ProjectContent.Project != null);
		}
	}
}
