// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class NUnitTestFramework : ITestFramework
	{
		/// <summary>
		/// Determines whether the project is a test project. A project
		/// is considered to be a test project if it contains a reference
		/// to the NUnit.Framework assembly.
		/// </summary>
		public bool IsTestProject(IProject project)
		{
			if (project != null) {
				foreach (ProjectItem projectItem in project.Items) {
					ReferenceProjectItem referenceProjectItem = projectItem as ReferenceProjectItem;
					if (IsNUnitFrameworkAssemblyReference(referenceProjectItem)) {
						return true;
					}
				}
			}
			return false;
		}
		
		bool IsNUnitFrameworkAssemblyReference(ReferenceProjectItem referenceProjectItem)
		{
			if (referenceProjectItem != null) {
				string name = referenceProjectItem.ShortName;
				return name.Equals("NUnit.Framework", StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
		
		/// <summary>
		/// Determines whether the class is a test fixture. A class
		/// is considered to be a test class if it contains a TestFixture attribute.
		/// </summary>
		public bool IsTestClass(IClass c)
		{
			StringComparer nameComparer = GetNameComparer(c);
			if (nameComparer != null) {
				NUnitTestAttributeName testAttributeName = new NUnitTestAttributeName("TestFixture", nameComparer);
				foreach (IAttribute attribute in c.Attributes) {
					if (testAttributeName.IsEqual(attribute)) {
						return true;
					}
				}
				if (c.DeclaringType != null)
					return IsTestClass(c.DeclaringType);
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
		
		/// <summary>
		/// Determines whether the method is a test method. A method
		/// is considered to be a test method if it contains the NUnit Test attribute.
		/// If the method has parameters it cannot be a test method.
		/// </summary>
		public bool IsTestMethod(IMember member)
		{
			if (member == null) {
				return false;
			}
			
			StringComparer nameComparer = GetNameComparer(member.DeclaringType);
			if (nameComparer != null) {
				NUnitTestAttributeName testAttribute = new NUnitTestAttributeName("Test", nameComparer);
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
		
		public ITestRunner CreateTestRunner()
		{
			return new NUnitTestRunner();
		}
		
		public ITestRunner CreateTestDebugger()
		{
			return new NUnitTestDebugger();
		}
	}
}
