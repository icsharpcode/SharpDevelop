// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
		public static IMember GetMember(object caller)
		{
			ITestTreeView testTreeView = caller as ITestTreeView;
			if (testTreeView != null) {
				return testTreeView.SelectedMethod;
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
			IMember m = GetMember(caller);
			IClass c = (m != null) ? m.DeclaringType : GetClass(caller);
			if (c != null && c.ProjectContent != null) {
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
				return TestMethod.IsTestMethod(m);
			}
			IClass c = GetClass(caller);
			if (c == null || c.ProjectContent == null || c.ProjectContent.Project == null) {
				return false;
			}
			return TestClass.IsTestClass(c);
		}
	}
}
