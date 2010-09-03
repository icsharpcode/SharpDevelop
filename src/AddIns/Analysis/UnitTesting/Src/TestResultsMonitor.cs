// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Watches for new test results as they occur. Test results
	/// are written to a file and read in by this class.
	/// </summary>
	public class TestResultsMonitor : ITestResultsMonitor
	{
		FileInfo fileInfo;
		TestResultsReader testResultsReader;
		FileSystemWatcher fileSystemWatcher;

		long initialFilePosition = 3;
		long filePosition;
		
		const int BytesBufferLength = 1024;
		byte[] bytes = new byte[BytesBufferLength];
		
		/// <summary>
		/// Raised when a single test has been completed.
		/// </summary>
		public event TestFinishedEventHandler TestFinished;
		
		public TestResultsMonitor(string fileName)
		{
			fileInfo = new FileInfo(fileName);
			ResetFilePosition();
		}
		
		public TestResultsMonitor()
			: this(Path.GetTempFileName())
		{
			ResetFilePosition();
		}
		
		public long InitialFilePosition {
			get { return initialFilePosition; }
			set { initialFilePosition = value; }
		}
		
		/// <summary>
		/// Gets or sets the test results filename.
		/// </summary>
		public string FileName {
			get { return fileInfo.FullName; }
			set { fileInfo = new FileInfo(value); }
		}
		
		/// <summary>
		/// Starts monitoring for test results.
		/// </summary>
		public void Start()
		{
			testResultsReader = new TestResultsReader();
			ResetFilePosition();
			
			string filter = fileInfo.Name;
			fileSystemWatcher = new FileSystemWatcher(fileInfo.DirectoryName, filter);
			
			if (File.Exists(fileInfo.FullName)) {
				fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
				fileSystemWatcher.Changed += FileChanged;
			} else {
				fileSystemWatcher.Created += FileCreated;
			}
			fileSystemWatcher.Error += FileSystemWatcherError;
			fileSystemWatcher.EnableRaisingEvents = true;
		}
		
		/// <summary>
		/// Stops monitoring.
		/// </summary>
		public void Stop()
		{		
			if (fileSystemWatcher != null) {
				fileSystemWatcher.Dispose();
				fileSystemWatcher = null;
			}
		}
		
		/// <summary>
		/// Reads the rest of the file from the current position.
		/// Raises the TestFinished event for each test result 
		/// still in the file.
		/// </summary>
		public void Read()
		{
			string text = ReadTextAdded();
			if (text != null) {
				TestResult[] results = testResultsReader.Read(text);
				OnTestResultsReceived(results);
			}
		}
		
		/// <summary>
		/// Stops monitoring and releases any resources used
		/// by the TestResultsMonitor.
		/// </summary>
		public void Dispose()
		{
			Stop();
			
			try {
				File.Delete(FileName);
			} catch { }
		}
		
		void FileCreated(object source, FileSystemEventArgs e)
		{
			fileSystemWatcher.Created -= FileCreated;
			fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
			fileSystemWatcher.Changed += FileChanged;
		}
		
		void FileChanged(object source, FileSystemEventArgs e)
		{
			Read();
		}
		
		void OnTestResultsReceived(TestResult[] results)
		{
			if ((results.Length > 0) && (TestFinished != null)) {
				foreach (TestResult result in results) {
					TestFinished(this, new TestFinishedEventArgs(result));
				}
			}
		}
		
		/// <summary>
		/// Reads the text added to the end of the file from the last
		/// position we read from.
		/// </summary>
		string ReadTextAdded()
		{
			StringBuilder text = null;
			try {
				using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
					if (fs.Length > 0) {
						text = new StringBuilder();
						int bytesRead = 0;
						fs.Seek(filePosition, SeekOrigin.Begin);
						do {
							bytesRead = fs.Read(bytes, 0, BytesBufferLength);
							if (bytesRead > 0) {
								filePosition += bytesRead;
								text.Append(UTF8Encoding.UTF8.GetString(bytes, 0, bytesRead));
							}
						} while ((bytesRead > 0) && (filePosition < fs.Length));
					}
				}
			} catch (FileNotFoundException) {
				// Test was aborted before it even started execution
				return null;
			}
			if (text != null) {
				return text.ToString();
			}
			return null;
		}
		
		void FileSystemWatcherError(object source, ErrorEventArgs e)
		{
			Console.WriteLine(e.GetException().ToString());
		}
		
		void ResetFilePosition()
		{
			filePosition = initialFilePosition;
		}
	}
}
