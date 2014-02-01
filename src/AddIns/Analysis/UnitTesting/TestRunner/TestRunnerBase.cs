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
using System.Diagnostics;
using System.IO;
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
		protected TextWriter output;
		
		public Task RunAsync(IEnumerable<ITest> selectedTests, IProgress<double> progress, TextWriter output, CancellationToken cancellationToken)
		{
			this.progress = progress;
			this.output = output;
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
			output.WriteLine(commandLine);
		}
		
		protected string GetCommandLine(ProcessStartInfo startInfo)
		{
			return String.Format("\"{0}\" {1}", startInfo.FileName, startInfo.Arguments);
		}
		
		protected virtual void OnAllTestsFinished()
		{
			cancellationTokenRegistration.Dispose();
			if (wasCancelled)
				tcs.SetCanceled();
			else
				tcs.SetResult(null);
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
		
		public abstract int GetExpectedNumberOfTestResults(IEnumerable<ITest> selectedTests);
		public abstract void Dispose();
		public abstract void Stop();
		public abstract void Start(IEnumerable<ITest> selectedTests);
	}
}
