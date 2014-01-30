// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.PythonBinding
{
	public class PythonTestFramework : ITestFramework
	{
		public bool IsTestMember(IMember member)
		{
			var method = member as IMethod;
			if (method != null) {
				return IsTestMethod(method);
			}
			return false;
		}
		
		public IEnumerable<TestMember> GetTestMembersFor(IClass @class) {
			return @class.Methods.Where(IsTestMethod).Select(method => new TestMember(method));
		}

		static bool IsTestMethod(IMethod method) {
			return method.Name.StartsWith("test");
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
