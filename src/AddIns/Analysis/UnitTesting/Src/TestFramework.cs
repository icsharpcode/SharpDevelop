// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public interface ITestFramework
	{
		bool IsTestMethod(IMember member);
		bool IsTestClass(IClass c);
		bool IsTestProject(IProject project);
		
		ITestRunner CreateTestRunner();
		ITestRunner CreateTestDebugger();
		
		bool IsBuildNeededBeforeTestRun { get; }
	}
}
