// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.UnitTesting
{
	public class TestProcessRunnerBase : TestRunnerBase
	{
		IUnitTestProcessRunner processRunner;
		ITestResultsMonitor testResultsMonitor;
		
		public TestProcessRunnerBase()
			: this(new UnitTestProcessRunner(),
				new TestResultsMonitor())
		{
		}
		
		public TestProcessRunnerBase(IUnitTestProcessRunner processRunner,
			ITestResultsMonitor testResultsMonitor)
		{
			this.processRunner = processRunner;
			this.testResultsMonitor = testResultsMonitor;
			
			processRunner.LogStandardOutputAndError = false;
			processRunner.OutputLineReceived += OutputLineReceived;
			processRunner.ErrorLineReceived += OutputLineReceived;
			processRunner.ProcessExited += OnAllTestsFinished;
			testResultsMonitor.TestFinished += OnTestFinished;
		}
		
		protected ITestResultsMonitor TestResultsMonitor {
			get { return testResultsMonitor; }
		}
		
		protected IUnitTestProcessRunner ProcessRunner {
			get { return processRunner; }
		}
		
		void OutputLineReceived(object source, LineReceivedEventArgs e)
		{
			OnMessageReceived(e.Line);
		}
		
		public override void Start(SelectedTests selectedTests)
		{
			ProcessStartInfo startInfo = GetProcessStartInfo(selectedTests);
			Start(startInfo);
		}
		
		void Start(ProcessStartInfo processStartInfo)
		{
			LogCommandLine(processStartInfo);
			
			testResultsMonitor.Start();
			processRunner.WorkingDirectory = processStartInfo.WorkingDirectory;
			processRunner.Start(processStartInfo.FileName, processStartInfo.Arguments);
		}
		
		public override void Stop()
		{
			processRunner.Kill();
			testResultsMonitor.Stop();
			testResultsMonitor.Read();
		}
		
		public override void Dispose()
		{
			testResultsMonitor.Dispose();
			testResultsMonitor.TestFinished -= OnTestFinished;
			processRunner.ErrorLineReceived -= OutputLineReceived;
			processRunner.OutputLineReceived -= OutputLineReceived;
		}
	}
}
