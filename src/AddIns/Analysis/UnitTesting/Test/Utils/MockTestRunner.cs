// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Util;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockTestRunner : ITestRunner
	{
		bool disposed = false;
		bool stopped = false;
		bool started = false;
		SelectedTests selectedTests;
		
		public event TestFinishedEventHandler TestFinished;
		public event EventHandler AllTestsFinished;
		public event MessageReceivedEventHandler MessageReceived;
		
		public void Dispose()
		{
			disposed = true;
		}
		
		public bool IsDisposeCalled {
			get { return disposed; }
		}
		
		public void Start(SelectedTests selectedTests)
		{
			started = true;
			this.selectedTests = selectedTests;
		}
		
		public bool IsStartCalled {
			get { return started; }
		}
		
		public SelectedTests SelectedTestsPassedToStartMethod {
			get { return selectedTests; }
		}
		
		public void Stop()
		{
			stopped = true;
		}
		
		public bool IsStopCalled {
			get { return stopped; }
		}
		
		public void FireTestFinishedEvent(TestResult testResult)
		{
			if (TestFinished != null) {
				TestFinished(this, new TestFinishedEventArgs(testResult));
			}
		}
		
		public void FireAllTestsFinishedEvent()
		{
			if (AllTestsFinished != null) {
				AllTestsFinished(this, new EventArgs());
			}			
		}
		
		public void FireMessageReceivedEvent(string message)
		{
			if (MessageReceived != null) {
				MessageReceived(this, new MessageReceivedEventArgs(message));
			}
		}
	}
}
