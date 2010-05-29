// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.PythonBinding
{
	public class PythonTestFramework : ITestFramework
	{
		public bool IsTestMethod(IMember member)
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
				return c.BaseTypes[0].FullyQualifiedName == "unittest.TestCase";
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
