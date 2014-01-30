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
using System.IO;
using System.Linq;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	public class MSpecUnitTestMonitor : ITestResultsReader
	{
		public event EventHandler<TestFinishedEventArgs> TestFinished;
		FileSystemWatcher fileSystemWatcher;
		ISet<string> reportedResults;

		public MSpecUnitTestMonitor()
		{
			FileName = Path.GetTempFileName();
		}

		public string FileName { get; set; }

		public void Join()
		{
			if (fileSystemWatcher != null) {
				fileSystemWatcher.Dispose();
				fileSystemWatcher = null;
			}
		}

		public void Start()
		{
			fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(FileName), Path.GetFileName(FileName));
			reportedResults = new HashSet<string>();
			if (File.Exists(FileName)) {
				fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
				fileSystemWatcher.Changed += ObservedFileChanged;
			} else {
				fileSystemWatcher.Created += ObservedFileCreated;
			}
			fileSystemWatcher.Error += FileObservationError;
			fileSystemWatcher.EnableRaisingEvents = true;
		}

		void FileObservationError(object sender, ErrorEventArgs e)
		{
			LoggingService.Error("Error while waiting for unit test session report modification.", e.GetException());
		}

		void ObservedFileCreated(object sender, FileSystemEventArgs e)
		{
			fileSystemWatcher.Created -= ObservedFileCreated;
			fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
			fileSystemWatcher.Changed += ObservedFileChanged;
		}

		void ObservedFileChanged(object sender, FileSystemEventArgs e)
		{
			Read();
		}

		public void Read()
		{
			var document = new XmlDocument();
			try {
				document.Load(FileName);
			}
			catch (XmlException ex) {
				LoggingService.Warn("Error reading Machine.Specifications test results.", ex);
				return;
			}

			XmlNodeList contextNodes = document.SelectNodes("MSpec/assembly/concern/context/specification");
			TestResult[] results = contextNodes.Cast<XmlNode>().Select(BuildTestResultFrom).ToArray();
			PublishTestResults(results);
		}

		TestResult BuildTestResultFrom(XmlNode node)
		{
			string className = node.SelectSingleNode("../@type-name").InnerText;
			var result = new TestResult(className + "." + node.Attributes["field-name"].InnerText);
			switch (node.Attributes["status"].InnerText) {
				case "failed":
					result.ResultType = TestResultType.Failure;
					break;
				case "passed":
					result.ResultType = TestResultType.Success;
					break;
				case "not-implemented":
				case "ignored":
					result.ResultType = TestResultType.Ignored;
					break;
			}
			XmlNode errorNode = node.SelectSingleNode("error");
			if (errorNode != null) {
				XmlNode messageNode = errorNode.SelectSingleNode("message");
				result.Message = messageNode.InnerText;

				XmlNode stackTraceNode = errorNode.SelectSingleNode("stack-trace");
				result.StackTrace = stackTraceNode.InnerText;
			}
			return result;
		}

		void PublishTestResults(TestResult[] testResults)
		{
			if (TestFinished != null) {
				foreach (TestResult result in testResults) {
					if (!reportedResults.Contains(result.Name)) {
						TestFinished(this, new TestFinishedEventArgs(new NUnitTestResult(result)));
						reportedResults.Add(result.Name);
					}
				}
			}
		}

		public void Dispose()
		{
			Join();
			try {
				File.Delete(FileName);
			} catch (Exception ex) {
				LoggingService.Warn("Could delete temporary file.", ex);
			}
		}
		
		public string PipeName {
			get { return String.Empty; }
		}
	}
}
