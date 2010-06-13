// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class DerivedRunTestCommand : AbstractRunTestCommand
	{
		bool onBeforeRunTestsMethodCalled;
		bool onAfterRunTestsMethodCalled;
		AbstractRunTestCommand runningTestCommandWhenOnBeforeRunCalled;
		bool runningTestWhenOnBeforeRunCalled;
		bool onStopMethodCalled;
		List<NUnitConsoleApplication> helpers = new List<NUnitConsoleApplication>();
		List<MockTestRunner> testRunnersCreated = new List<MockTestRunner>();
		
		public DerivedRunTestCommand(IRunTestCommandContext context)
			: base(context)
		{
		}
		
		public bool IsOnBeforeRunTestsMethodCalled {
			get { return onBeforeRunTestsMethodCalled; }
		}

		public void CallOnBeforeRunTestsMethod()
		{
			OnBeforeRunTests();
		}
		
		protected override void OnBeforeRunTests()
		{
			onBeforeRunTestsMethodCalled = true;
			runningTestCommandWhenOnBeforeRunCalled = AbstractRunTestCommand.RunningTestCommand;
			runningTestWhenOnBeforeRunCalled = AbstractRunTestCommand.IsRunningTest;
		}
		
		protected override ITestRunner CreateTestRunner(IProject project)
		{
			MockTestRunner testRunner = new MockTestRunner();
			testRunnersCreated.Add(testRunner);
			return testRunner;
		}
		
		public List<MockTestRunner> TestRunnersCreated {
			get { return testRunnersCreated; }
		}
		
		protected override void RunTests(NUnitConsoleApplication helper)
		{
			helpers.Add(helper);
		}
		
		public List<NUnitConsoleApplication> Helpers {
			get { return helpers; }
		}
		
		public AbstractRunTestCommand RunningTestCommandPropertyWhenOnBeforeRunCalled {
			get { return runningTestCommandWhenOnBeforeRunCalled; }
		}
		
		public bool IsRunningTestPropertyWhenOnBeforeRunCalled {
			get { return runningTestWhenOnBeforeRunCalled; }
		}
		
		public bool IsOnStopMethodCalled {
			get { return onStopMethodCalled; }
		}
		
		protected override void OnStop()
		{
			onStopMethodCalled = true;
		}
		
		public Action<TestResult> ShowResultAction {
			get { return ShowResult; }
		}
		
		public void CallTestsCompleted()
		{
			base.TestRunCompleted();
		}
		
		public bool IsOnAfterRunTestsMethodCalled {
			get { return onAfterRunTestsMethodCalled; }
		}

		public void CallOnAfterRunTestsMethod()
		{
			OnAfterRunTests();
		}
		
		protected override void OnAfterRunTests()
		{
			onAfterRunTestsMethodCalled = true;
		}
		
		public ITestRunner CallCreateTestRunner(IProject project)
		{
			return CreateTestRunner(project);
		}
	}
}
