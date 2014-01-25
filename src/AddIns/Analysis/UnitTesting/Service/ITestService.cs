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
