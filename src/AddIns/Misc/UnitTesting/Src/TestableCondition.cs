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

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Supplies a "Unit test" menu item if the class is a test fixture.
	/// </summary>
	public class TestableCondition : IConditionEvaluator
	{
		public static IMember GetMember(object caller)
		{
			if (caller is TestTreeView) {
				return ((TestTreeView)caller).SelectedTestMethod;
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
			if (caller is TestTreeView) {
				return ((TestTreeView)caller).SelectedFixtureClass;
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
		
		public static ICSharpCode.SharpDevelop.Project.IProject GetProject(object caller)
		{
			if (caller is TestTreeView) {
				return ((TestTreeView)caller).SelectedProject;
			}
			IMember m = GetMember(caller);
			IClass c = (m != null) ? m.DeclaringType : GetClass(caller);
			return (ICSharpCode.SharpDevelop.Project.IProject)c.ProjectContent.Project;
		}
		
		public bool IsValid(object caller, Condition condition)
		{
			IMember m = GetMember(caller);
			IClass c = (m != null) ? m.DeclaringType : GetClass(caller);
			if (c.ProjectContent.Project == null)
				return false;
			StringComparer nameComparer = c.ProjectContent.Language.NameComparer;
			string attributeName = (m != null) ? "Test" : "TestFixture";
			foreach (IAttribute attribute in (m ?? (IDecoration)c).Attributes) {
				if (nameComparer.Equals(attribute.Name, attributeName)
				    || nameComparer.Equals(attribute.Name, attributeName + "Attribute")
				    || nameComparer.Equals(attribute.Name, "NUnit.Framework." + attributeName + "Attribute"))
				{
					return true;
				}
			}
			return false;
		}
	}
}
