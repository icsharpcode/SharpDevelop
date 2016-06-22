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
		
		public event EventHandler<TestFinishedEventArgs> TestFinished;
		public event EventHandler AllTestsFinished;
		public event EventHandler<MessageReceivedEventArgs> MessageReceived;
		
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
