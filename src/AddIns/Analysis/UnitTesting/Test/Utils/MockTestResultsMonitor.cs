// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public event TestFinishedEventHandler TestFinished;
		
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
