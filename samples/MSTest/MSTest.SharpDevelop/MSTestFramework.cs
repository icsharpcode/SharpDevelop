// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestFramework : ITestFramework
	{
		public bool IsBuildNeededBeforeTestRun {
			get { return true; }
		}
		
		public bool IsTestMember(IMember member)
		{
			var method = member as IMethod;
			if (method == null)
				return false;
			
			return IsTestMethod(method);
		}
		
		bool IsTestMethod(IMethod method)
		{
			foreach (IAttribute attribute in method.Attributes) {
				if (IsMSTestMethodAttribute(attribute)) {
					return true;
				}
			}
			
			return false;
		}
		
		bool IsMSTestMethodAttribute(IAttribute attribute)
		{
			return IsMSTestMethodAttribute(attribute.AttributeType.FullyQualifiedName);
		}
		
		bool IsMSTestMethodAttribute(string name)
		{
			return 
				name == "TestMethod" ||
				name == "TestMethodAttribute" ||
				name == "Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute";
		}
		
		public bool IsTestClass(IClass c)
		{
			if ((c == null) || (c.IsAbstract))
				return false;
			
			foreach (IAttribute attribute in c.Attributes) {
				if (IsMSTestClassAttribute(attribute)) {
					return true;
				}
			}
			
			return false;
		}
		
		bool IsMSTestClassAttribute(IAttribute attribute)
		{
			return IsMSTestClassAttribute(attribute.AttributeType.FullyQualifiedName);
		}
		
		bool IsMSTestClassAttribute(string name)
		{
			return 
				name == "TestClass" ||
				name == "TestClassAttribute" ||
				name == "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute";
		}
		
		public bool IsTestProject(IProject project)
		{
			if (project == null)
				return false;
			
			foreach (ProjectItem item in project.Items) {
				if (item.IsMSTestAssemblyReference()) {
					return true;
				}
			}
			return false;
		}
		
		public IEnumerable<TestMember> GetTestMembersFor(IClass c)
		{
			return c.Methods
				.Where(IsTestMethod)
				.Select(method => new TestMember(method));
		}
		
		public ITestRunner CreateTestRunner()
		{
			return new MSTestRunner();
		}
		
		public ITestRunner CreateTestDebugger()
		{
			return new MSTestDebugger();
		}
	}
}