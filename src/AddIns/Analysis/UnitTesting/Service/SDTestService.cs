// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.UnitTesting.Frameworks;

namespace ICSharpCode.UnitTesting
{
	sealed class SDTestService : ITestService
	{
		#region Test Framework Management
		const string AddInPath = "/SharpDevelop/UnitTesting/TestFrameworks";
		IReadOnlyList<TestFrameworkDescriptor> testFrameworkDescriptors = SD.AddInTree.BuildItems<TestFrameworkDescriptor>(AddInPath, null);
		
		public ITestFramework GetTestFrameworkForProject(IProject project)
		{
			if (project != null) {
				foreach (TestFrameworkDescriptor descriptor in testFrameworkDescriptors) {
					if (descriptor.IsSupportedProject(project)) {
						return descriptor.TestFramework;
					}
				}
			}
			return null;
		}
		#endregion
		
		#region UnitTestMessageView
		MessageViewCategory unitTestMessageView;
		
		public IOutputCategory UnitTestMessageView {
			get {
				if (unitTestMessageView == null) {
					MessageViewCategory.Create(ref unitTestMessageView,
					                           "UnitTesting",
					                           "${res:ICSharpCode.NUnitPad.NUnitPadContent.PadName}");
				}
				return unitTestMessageView;
			}
		}
		#endregion
		
		#region OpenSolution
		TestSolution solution;
		
		public ITestSolution OpenSolution {
			get {
				SD.MainThread.VerifyAccess();
				if (solution == null)
					solution = new TestSolution(this, SD.ResourceService);
				return solution;
			}
		}
		
		public event EventHandler OpenSolutionChanged { add {} remove {} }
		#endregion
		
		#region RunTests
		CancellationTokenSource runTestsCancellationTokenSource;
		
		public bool IsRunningTests {
			get { return runTestsCancellationTokenSource != null; }
		}
		
		public async Task RunTestsAsync(IEnumerable<ITest> selectedTests, TestExecutionOptions options)
		{
			CancelRunningTests();
			runTestsCancellationTokenSource = new CancellationTokenSource();
			// invalidate commands as IsRunningTests changes
			System.Windows.Input.CommandManager.InvalidateRequerySuggested();
			var executionManager = new TestExecutionManager();
			try {
				await executionManager.RunTestsAsync(selectedTests, options, runTestsCancellationTokenSource.Token);
			} finally {
				runTestsCancellationTokenSource = null;
				// invalidate commands as IsRunningTests changes
				System.Windows.Input.CommandManager.InvalidateRequerySuggested();
			}
		}
		
		public void CancelRunningTests()
		{
			SD.MainThread.VerifyAccess();
			if (runTestsCancellationTokenSource != null) {
				runTestsCancellationTokenSource.Cancel();
				runTestsCancellationTokenSource = null;
			}
		}
		#endregion
	}
}
