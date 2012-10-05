// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.UnitTesting
{
	public abstract class TestRunnerBase : ITestRunner
	{
		IProgress<double> progress;
		double progressPerTest;
		int testsFinished;
		TaskCompletionSource<object> tcs;
		CancellationTokenRegistration cancellationTokenRegistration;
		bool wasCancelled;
		
		public Task RunAsync(IEnumerable<ITest> selectedTests, IProgress<double> progress, CancellationToken cancellationToken)
		{
			this.progress = progress;
			progressPerTest = 1.0 / GetExpectedNumberOfTestResults(selectedTests);
			tcs = new TaskCompletionSource<object>();
			Start(selectedTests);
			cancellationTokenRegistration = cancellationToken.Register(Cancel, true);
			return tcs.Task;
		}
		
		void Cancel()
		{
			wasCancelled = true;
			Stop();
		}
		
		protected virtual ProcessStartInfo GetProcessStartInfo(IEnumerable<ITest> selectedTests)
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
		
		protected virtual void OnAllTestsFinished(object source, EventArgs e)
		{
			cancellationTokenRegistration.Dispose();
			if (wasCancelled)
				tcs.SetCanceled();
			else
				tcs.SetResult(e);
		}
		
		public event EventHandler<TestFinishedEventArgs> TestFinished;
		
		protected void OnTestFinished(object source, TestFinishedEventArgs e)
		{
			if (TestFinished != null) {
				TestResult testResult = CreateTestResultForTestFramework(e.Result);
				TestFinished(source, new TestFinishedEventArgs(testResult));
			}
			if (!double.IsInfinity(progressPerTest))
				progress.Report(progressPerTest * Interlocked.Increment(ref testsFinished));
		}
		
		protected virtual TestResult CreateTestResultForTestFramework(TestResult testResult)
		{
			return testResult;
		}
		
		public event EventHandler<MessageReceivedEventArgs> MessageReceived;
		
		protected virtual void OnMessageReceived(string message)
		{
			if (MessageReceived != null) {
				MessageReceived(this, new MessageReceivedEventArgs(message));
			}
		}
		
		public abstract int GetExpectedNumberOfTestResults(IEnumerable<ITest> selectedTests);
		public abstract void Dispose();
		public abstract void Stop();
		public abstract void Start(IEnumerable<ITest> selectedTests);
	}
}
