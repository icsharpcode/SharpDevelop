// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestMonitor : ITestResultsReader
	{
		public event EventHandler<TestFinishedEventArgs> TestFinished;
		public string ResultsFileName { get; set; }
		
		public void Join()
		{
			if (File.Exists(ResultsFileName)) {
				var testResults = new MSTestResults(ResultsFileName);
				UpdateTestResults(testResults);
			}
		}
		
		void UpdateTestResults(MSTestResults testResults)
		{
			foreach (TestResult result in testResults) {
				OnTestFinished(new TestFinishedEventArgs(result));
			}
		}
		
		void OnTestFinished(TestFinishedEventArgs e)
		{
			if (TestFinished != null) {
				TestFinished(this, e);
			}
		}
		
		public void Start()
		{
			TryDeleteResultsFile();
		}
		
		public string PipeName {
			get { return String.Empty; }
		}
		
		public void Dispose()
		{
		}
		
		void TryDeleteResultsFile()
		{
			try {
				Console.WriteLine("Deleting results file: " + ResultsFileName);
				File.Delete(ResultsFileName);
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}
	}
}
