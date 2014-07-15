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
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.UnitTesting
{
	public interface ITestResultsReader : IDisposable
	{
		event EventHandler<TestFinishedEventArgs> TestFinished;
		string PipeName { get; }
		void Start();
		void Join();
	}
	
	public class TestResultsReader : ITestResultsReader
	{
		TextReader reader;
		readonly NamedPipeServerStream namedPipe;
		readonly string pipeName;
		
		public TestResultsReader()
		{
			pipeName = Guid.NewGuid().ToString();
			namedPipe = new NamedPipeServerStream(this.PipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte);
		}
		
		/// <summary>
		/// Creates a new TestResultsReader that reads from an existing text reader.
		/// </summary>
		public TestResultsReader(TextReader reader)
		{
			this.reader = reader;
		}
		
		public string PipeName {
			get { return pipeName; }
		}
		
		public void Dispose()
		{
			if (reader != null)
				reader.Dispose();
			if (namedPipe != null)
				namedPipe.Dispose();
		}
		
		public event EventHandler<TestFinishedEventArgs> TestFinished;
		
		Thread thread;
		
		/// <summary>
		/// Runs the pipe reader on a background thread.
		/// The returned task will be signalled as completed when the worker process closes the pipe, after
		/// all contents have been read from the pipe.
		/// </summary>
		public void Start()
		{
			thread = new Thread(Run);
			thread.Name = "UnitTesting Pipe Reader";
			thread.Start();
		}
		
		public void Join()
		{
			SD.Log.Debug("Waiting for pipe reader to finish");
			thread.Join();
			SD.Log.Debug("Pipe reader has finished");
		}
		
		/// <summary>
		/// Run the pipe reader on the current thread.
		/// This method blocks until the worker process closes the pipe.
		/// </summary>
		public void Run()
		{
			if (reader == null) {
				SD.Log.Debug("Waiting for connection to pipe");
				namedPipe.WaitForConnection();
				SD.Log.Debug("Start reading pipe");
				reader = new StreamReader(namedPipe);
			}
			char[] buffer = new char[1024];
			int read;
			while ((read = reader.Read(buffer, 0, buffer.Length)) != 0) {
				for (int i = 0; i < read; i++) {
					if (ReadNameValuePair(buffer[i])) {
						if (ReadTestResult()) {
							if (TestFinished != null)
								TestFinished(this, new TestFinishedEventArgs(result));
						}
					}
				}
			}
			SD.Log.Debug("End of pipe");
		}
		
		StringBuilder nameBuilder = new StringBuilder();
		StringBuilder valueBuilder = new StringBuilder();
		bool firstNameChar = true;
		TestResult result;
		
		enum State {
			WaitingForEndOfName     = 0,
			WaitingForStartOfValue  = 1,
			WaitingForEndOfValue    = 2
		}
		
		State state = State.WaitingForEndOfName;
		
		/// <summary>
		/// Reads a name-value pair of the form:
		/// 
		/// Name: Value
		/// 
		/// A value can span multiple lines:
		/// 
		/// Name1: ValueLine1
		/// {SP}ValueLine2
		/// {SP}ValueLine3
		/// Name1: Value2
		/// 
		/// Each continued line of the value must start with
		/// a single space character to distinguish it from
		/// the start of a new name-value pair.
		/// </summary>
		/// <returns>True if a name-value pair has been
		/// successfully read in</returns>
		bool ReadNameValuePair(char ch)
		{
			if (state == State.WaitingForEndOfName) {
				ReadNameChar(ch);
			} else if (state == State.WaitingForStartOfValue) {
				// Makes sure first space is ignored.
				state = State.WaitingForEndOfValue;
			} else {
				return ReadValueChar(ch);
			}
			return false;
		}
		
		void ReadNameChar(char ch)
		{
			if (ch == ':') {
				state = State.WaitingForStartOfValue;
			} else if (ch == '\r' || ch == '\n') {
				nameBuilder = new StringBuilder();
			} else if (ch == ' ' && firstNameChar) {
				state = State.WaitingForEndOfValue;
				valueBuilder.Append("\r\n");
			} else if (firstNameChar) {
				firstNameChar = false;
				nameBuilder = new StringBuilder();
				valueBuilder = new StringBuilder();
				nameBuilder.Append(ch);
			} else {
				nameBuilder.Append(ch);
			}
		}
		
		bool ReadValueChar(char ch)
		{
			if (ch == '\r') {
				// Ignore.
			} else if (ch == '\n') {
				state = State.WaitingForEndOfName;
				firstNameChar = true;
				return true;
			} else {
				valueBuilder.Append(ch);
			}
			return false;
		}
		
		/// <summary>
		/// Creates and updates a TestResult based on the
		/// name-value pair read in,
		/// </summary>
		/// <returns>True if a TestResult is ready to be returned
		/// to the caller.</returns>
		/// <remarks>
		/// The first name-value pair for a test result is the
		/// test name. The last name-value pair is the result of
		/// the test (Success, Failure or Ignored).</remarks>
		bool ReadTestResult()
		{
			string name = nameBuilder.ToString();
			if (name == "Name") {
				result = new TestResult(valueBuilder.ToString());
			} else if (result != null) {
				if (name == "Message") {
					result.Message = valueBuilder.ToString();
				} else if (name == "StackTrace") {
					result.StackTrace = valueBuilder.ToString();
				} else if (name == "Result") {
					UpdateTestSuccess();
					return true;
				}
			}
			return false;
		}
		
		void UpdateTestSuccess()
		{
			string value = valueBuilder.ToString();
			if (value == "Success") {
				result.ResultType = TestResultType.Success;
			} else if (value == "Failure") {
				result.ResultType = TestResultType.Failure;
			} else {
				result.ResultType = TestResultType.Ignored;
			}
		}
	}
}
