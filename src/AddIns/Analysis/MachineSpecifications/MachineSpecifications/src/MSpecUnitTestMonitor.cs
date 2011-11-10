using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.UnitTesting;
using System.IO;
using ICSharpCode.Core;
using System.Xml;

namespace ICSharpCode.MachineSpecifications
{
    public class MSpecUnitTestMonitor : ITestResultsMonitor
    {
        public event TestFinishedEventHandler TestFinished;
        private FileSystemWatcher fileSystemWatcher;
        private ISet<string> reportedResults;

        public MSpecUnitTestMonitor()
        {
            FileName = Path.GetTempFileName();
        }

        public string FileName { get; set; }

        public void Stop()
        {
            if (fileSystemWatcher != null)
            {
                fileSystemWatcher.Dispose();
                fileSystemWatcher = null;
            }        
        }

        public void Start()
        {
            fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(FileName), Path.GetFileName(FileName));
            reportedResults = new HashSet<string>();
            if (File.Exists(FileName))
            {
                fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
                fileSystemWatcher.Changed += ObservedFileChanged;
            } else
                fileSystemWatcher.Created += ObservedFileCreated;
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
            try
            {
                document.Load(FileName);
            }
            catch (XmlException e)
            {
                LoggingService.Warn("Error reading Machine.Specifications test results.", e);
                return;
            }

            var contextNodes = document.SelectNodes("MSpec/assembly/concern/context/specification");
            var results = contextNodes.Cast<XmlNode>().Select(BuildTestResultFrom).ToArray();
            PublishTestResults(results);
        }

        TestResult BuildTestResultFrom(XmlNode node)
        {
            var className = node.SelectSingleNode("../@type-name").InnerText;
            var result = new TestResult(className + "." + node.Attributes["field-name"].InnerText);
            switch (node.Attributes["status"].InnerText)
            {
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
            var errorNode = node.SelectSingleNode("error");
            if (errorNode != null)
            {
                var messageNode = errorNode.SelectSingleNode("message");
                result.Message = messageNode.InnerText;

                var stackTraceNode = errorNode.SelectSingleNode("stack-trace");
                result.StackTrace = stackTraceNode.InnerText;
            }
            return result;
        }

        void PublishTestResults(TestResult[] testResults)
        {
            if (TestFinished != null)
                foreach (var result in testResults)
            		if (!reportedResults.Contains(result.Name)) {
                    	TestFinished(this, new TestFinishedEventArgs(result));
                    	reportedResults.Add(result.Name);
		            }
        }

		public long InitialFilePosition { get; set; }

        public void Dispose()
        {
            Stop();
            try
            {
                File.Delete(FileName);
            }
            catch (Exception e)
            {
                LoggingService.Warn("Could delete temporary file.", e);
            }
        }
    }
}
