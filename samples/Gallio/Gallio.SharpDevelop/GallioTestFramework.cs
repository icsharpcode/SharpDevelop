// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace Gallio.SharpDevelop
{
	public class GallioTestFramework : ITestFramework
	{
		public GallioTestFramework()
		{
		}
		
		public bool IsTestProject(IProject project)
		{
			if (project != null) {
				foreach (ProjectItem projectItem in project.Items) {
					ReferenceProjectItem referenceProjectItem = projectItem as ReferenceProjectItem;
					if (IsMbUnitFrameworkAssemblyReference(referenceProjectItem)) {
						return true;
					}
				}
			}
			return false;
		}
		
		bool IsMbUnitFrameworkAssemblyReference(ReferenceProjectItem referenceProjectItem)
		{
			if (referenceProjectItem != null) {
				string name = referenceProjectItem.ShortName;
				return name.Equals("MbUnit", StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
		
		public ITestRunner CreateTestRunner()
		{
			return new GallioTestRunner();
		}
		
		public ITestRunner CreateTestDebugger()
		{
			return new GallioTestDebugger();
		}
		
		public bool IsTestClass(IClass c)
		{
			StringComparer nameComparer = GetNameComparer(c);
			if (nameComparer != null) {
				MbUnitTestAttributeName testAttributeName = new MbUnitTestAttributeName("TestFixture", nameComparer);
				foreach (IAttribute attribute in c.Attributes) {
					if (testAttributeName.IsEqual(attribute)) {
						return true;
					}
				}
			}
			return false;
		}
		
		StringComparer GetNameComparer(IClass c)
		{
			if (c != null) {
				IProjectContent projectContent = c.ProjectContent;
				if (projectContent != null) {
					LanguageProperties language = projectContent.Language;
					if (language != null) {
						return language.NameComparer;
					}
				}
			}
			return null;
		}
		
		public bool IsTestMember(IMember member)
		{
			var method = member as IMethod;
			if (method != null) {
				return IsTestMethod(method);
			}
			return false;
		}
		
		public IEnumerable<TestMember> GetTestMembersFor(IClass c)
		{
			return c.Methods.Where(IsTestMethod).Select(member => new TestMember(member));
		}
		
		/// <summary>
		/// Determines whether the method is a test method. A method
		/// is considered to be a test method if it contains the NUnit Test attribute.
		/// If the method has parameters it cannot be a test method.
		/// </summary>
		bool IsTestMethod(IMember member)
		{
			if (member == null) {
				return false;
			}
			
			StringComparer nameComparer = GetNameComparer(member.DeclaringType);
			if (nameComparer != null) {
				MbUnitTestAttributeName testAttribute = new MbUnitTestAttributeName("Test", nameComparer);
				foreach (IAttribute attribute in member.Attributes) {
					if (testAttribute.IsEqual(attribute)) {
						IMethod method = (IMethod)member;
						if (method.Parameters.Count == 0) {
							return true;
						}
					}
				}
			}
			return false;
		}
		
		public bool IsBuildNeededBeforeTestRun {
			get { return true; }
		}
	}
}
