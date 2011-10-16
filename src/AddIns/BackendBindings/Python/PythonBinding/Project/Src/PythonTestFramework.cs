// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.PythonBinding
{
	public class PythonTestFramework : ITestFramework
	{
		public bool IsTestMember(IMember member)
		{
			if (member != null) {
				return member.Name.StartsWith("test");
			}
			return false;
		}
		
		public bool IsTestClass(IClass c)
		{
			while (c != null) {
				if (HasTestCaseBaseType(c)) {
					return true;
				}
				c = c.BaseClass;
			}
			return false;
		}
		
		bool HasTestCaseBaseType(IClass c)
		{
			if (c.BaseTypes.Count > 0) {
				string baseTypeName = c.BaseTypes[0].FullyQualifiedName;
				return (baseTypeName == "unittest.TestCase") ||	(baseTypeName == "unittest2.TestCase");
			}
			return false;
		}
		
		public bool IsTestProject(IProject project)
		{
			return project is PythonProject;
		}
		
		public ITestRunner CreateTestRunner()
		{
			return new PythonTestRunner();
		}
		
		public ITestRunner CreateTestDebugger()
		{
			return new PythonTestDebugger();
		}
		
		public bool IsBuildNeededBeforeTestRun {
			get { return false; }
		}
	}
}
