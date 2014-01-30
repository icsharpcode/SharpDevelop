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
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockTestResultsMonitor : ITestResultsMonitor
	{
		bool startMethodCalled;
		bool stopMethodCalled;
		bool disposeMethodCalled;
		bool readMethodCalled;
		string fileName;
		long filePosition;
		
		public MockTestResultsMonitor()
		{
		}
		
		public event EventHandler<TestFinishedEventArgs> TestFinished;
		
		public long InitialFilePosition {
			get { return filePosition; }
			set { filePosition = value; }
		}
		
		public bool IsStartMethodCalled {
			get { return startMethodCalled; }
		}
		
		public void Start()
		{
			startMethodCalled = true;
		}
		
		public bool IsStopMethodCalled {
			get { return stopMethodCalled; }
			set { stopMethodCalled = value; }
		}
		
		public void Stop()
		{
			stopMethodCalled = true;
		}
		
		public bool IsDisposeMethodCalled {
			get { return disposeMethodCalled; }
		}
		
		public void Dispose()
		{
			disposeMethodCalled = true;
		}
		
		public bool IsReadMethodCalled {
			get { return readMethodCalled; }
		}
		
		public void Read()
		{
			readMethodCalled = true;
		}
		
		public string FileName {
			get { return fileName; }
			set { fileName = value; }
		}
		
		public void FireTestFinishedEvent(TestResult testResult)
		{
			OnTestFinished(new TestFinishedEventArgs(testResult));
		}
		
		protected virtual void OnTestFinished(TestFinishedEventArgs e)
		{
			if (TestFinished != null) {
				TestFinished(this, e);
			}
		}
	}
}
