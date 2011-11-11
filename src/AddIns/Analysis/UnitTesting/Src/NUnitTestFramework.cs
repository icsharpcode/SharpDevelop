// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Collections.Generic;
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
					var referenceProjectItem = projectItem as ReferenceProjectItem;
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
			if (c == null) return false;
			if (c.IsAbstract) return false;
			StringComparer nameComparer = GetNameComparer(c);
			if (nameComparer != null) {
				NUnitTestAttributeName testAttributeName = new NUnitTestAttributeName("TestFixture", nameComparer);
				foreach (IAttribute attribute in c.Attributes) {
					if (testAttributeName.IsEqual(attribute)) {
						return true;
					}
				}				
			}
			
			while (c != null) {
				if (HasTestMethod(c)) return true;
				c = c.BaseClass;
			}
			return false;
		}
		
		private bool HasTestMethod(IClass c) {
			return GetTestMembersFor(c).Any();
		}
		
		static StringComparer GetNameComparer(IClass c)
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
		public bool IsTestMember(IMember member)
		{
			var method = member as IMethod;
			if (method != null) {
				return IsTestMethod(method);
			}
			return false;
		}
		
		public IEnumerable<TestMember> GetTestMembersFor(IClass @class) {
			return @class.Methods.Where(IsTestMethod).Select(member => new TestMember(member));
		}
		
		static bool IsTestMethod(IMethod method)
		{
			var nameComparer = GetNameComparer(method.DeclaringType);
			if (nameComparer != null) {
				var testAttribute = new NUnitTestAttributeName("Test", nameComparer);
				foreach (IAttribute attribute in method.Attributes) {
					if (testAttribute.IsEqual(attribute)) {
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
