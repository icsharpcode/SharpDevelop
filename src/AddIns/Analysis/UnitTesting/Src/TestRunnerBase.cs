// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;

namespace ICSharpCode.UnitTesting
{
	public abstract class TestRunnerBase : ITestRunner
	{
		public TestRunnerBase()
		{
		}
		
		protected virtual ProcessStartInfo GetProcessStartInfo(SelectedTests selectedTests)
		{
			return new ProcessStartInfo();
		}
		
		protected void LogCommandLine(ProcessStartInfo startInfo)
		{
			string commandLine = GetCommandLine(startInfo);
			OnMessageReceived(commandLine);
		}
		
		protected string GetCommandLine(ProcessStartInfo startInfo)
		{
			return String.Format("\"{0}\" {1}", startInfo.FileName, startInfo.Arguments);
		}
		
		public event EventHandler AllTestsFinished;
		
		protected void OnAllTestsFinished(object source, EventArgs e)
		{
			if (AllTestsFinished != null) {
				AllTestsFinished(source, e);
			}
		}
		
		public event TestFinishedEventHandler TestFinished;
		
		protected void OnTestFinished(object source, TestFinishedEventArgs e)
		{
			if (TestFinished != null) {
				TestResult testResult = CreateTestResultForTestFramework(e.Result);
				TestFinished(source, new TestFinishedEventArgs(testResult));
			}
		}
		
		protected virtual TestResult CreateTestResultForTestFramework(TestResult testResult)
		{
			return testResult;
		}
		
		public event MessageReceivedEventHandler MessageReceived;
		
		protected virtual void OnMessageReceived(string message)
		{
			if (MessageReceived != null) {
				MessageReceived(this, new MessageReceivedEventArgs(message));
			}
		}
		
		public abstract void Dispose();
		public abstract void Stop();
		public abstract void Start(SelectedTests selectedTests);
	}
}
