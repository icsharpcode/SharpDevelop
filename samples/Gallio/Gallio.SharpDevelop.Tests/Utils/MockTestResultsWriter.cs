// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using Gallio.Extension;
using System.Collections.Generic;

namespace Gallio.SharpDevelop.Tests.Utils
{
	public class MockTestResultsWriter : ITestResultsWriter
	{
		string fileName = String.Empty;
		List<TestResult> testResults = new List<TestResult>();
		bool disposed;
		
		public MockTestResultsWriter(string fileName)
		{
			this.fileName = fileName;
		}
		
		public string FileName {
			get { return fileName; }
		}
		
		public void Write(TestResult testResult)
		{
			testResults.Add(testResult);
		}
		
		public List<TestResult> TestResults {
			get { return testResults; }
		}
		
		public TestResult FirstTestResult {
			get { return testResults[0]; }
		}
		
		public void Dispose()
		{
			disposed = true;
		}
		
		public bool IsDisposed {
			get { return disposed; }
			set { disposed = value; }
		}
	}
}
