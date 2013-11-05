// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.UnitTesting
{
	[SDService]
	public interface ITestService
	{
		/// <summary>
		/// Gets the test framework that supports the specified project.
		/// </summary>
		ITestFramework GetTestFrameworkForProject(IProject project);
		
		IOutputCategory UnitTestMessageView { get; }
		
		/// <summary>
		/// Gets the current test solution.
		/// This property should only be accessed by the main thread.
		/// </summary>
		ITestSolution OpenSolution { get; }
		
		/// <summary>
		/// Occurs when the open test solution has changed.
		/// </summary>
		event EventHandler OpenSolutionChanged;
		
		/// <summary>
		/// Builds the project (if necessary) and runs the specified tests.
		/// If tests are already running, the existing run is cancelled.
		/// </summary>
		Task RunTestsAsync(IEnumerable<ITest> selectedTests, TestExecutionOptions options);
		
		/// <summary>
		/// Gets whether tests are currently running.
		/// </summary>
		bool IsRunningTests { get; }
		
		/// <summary>
		/// Aborts the current test run.
		/// This method has no effect if no tests are running.
		/// </summary>
		void CancelRunningTests();
	}
}
